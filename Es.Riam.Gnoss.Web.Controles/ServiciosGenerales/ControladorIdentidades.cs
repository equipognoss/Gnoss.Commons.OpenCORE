using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.Live;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.CL.Usuarios;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.Organizador.Correo;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.Solicitudes;
using Es.Riam.Gnoss.Web.Controles.Suscripcion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Es.Riam.Gnoss.Web.Controles.ServiciosGenerales
{

    /// <summary>
    /// Controlador de identidades encargado de cargar datos relativos a identidades
    /// </summary>
    public class ControladorIdentidades : ControladorBase
    {

        #region Miembros

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestorIdentidades;
        private EntityContextBASE mEntityContextBASE;
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        public ControladorIdentidades(GestionIdentidades pGestorIdentidades, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorIdentidades> logger,ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContextBASE = entityContextBASE;
            mGestorIdentidades = pGestorIdentidades;
            mlogger = logger;
            mloggerFactory = loggerFactory;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el gestor de identidades
        /// </summary>
        public GestionIdentidades GestorIdentidades
        {
            get
            {
                return mGestorIdentidades;
            }
        }

        bool? perfilPersonalDisponible = null;

        /// <summary>
        /// Obtiene si los usuarios van a tener el perfil personal disponible en este entorno
        /// </summary>
        private bool PerfilPersonalDisponible
        {
            get
            {
                if (!perfilPersonalDisponible.HasValue)
                {
                    perfilPersonalDisponible = true;
                    //rametroAplicacionDS.ParametroAplicacionRow[] filasParametro = (ParametroAplicacionDS.ParametroAplicacionRow[])ParametroAplicacionDS.ParametroAplicacion.Select("parametro='PerfilPersonalDisponible'");
                    List<AD.EntityModel.ParametroAplicacion> filasParametro = ParametroAplicacionDS.Where(parametro => parametro.Parametro.Equals("PerfilPersonalDisponible")).ToList();

                    if (filasParametro.Count > 0 && filasParametro.First().Valor.ToLower().Equals("false"))
                    {
                        perfilPersonalDisponible = false;
                    }
                }

                return perfilPersonalDisponible.Value;
            }
        }

        public void CompletarCargaIdentidad(object identidadID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Metodos generales

        /// <summary>
        /// Lista de nombres de los grupos de comunidad u organización(nombreCortoOrg|||nombreCortoGr) a partir de sus identificadores
        /// </summary>
        /// <param name="pListaGruposID">Lista de identificadores de los grupos</param>
        /// <returns>Lista de nombres de los grupos de comunidad u organización(nombreCortoOrg|||nombreCortoGr) a partir de sus identificadores.</returns>
        public Dictionary<Guid, string> ObtenerNombresGrupos(List<Guid> pListaGruposID)
        {
            Dictionary<Guid, string> listaNombres = new Dictionary<Guid, string>();
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            if (pListaGruposID.Count > 0)
            {
                DataWrapperIdentidad identDW = identCN.ObtenerGruposPorIDGrupo(pListaGruposID, false);

                foreach (Guid grupoID in pListaGruposID)
                {
                    try
                    {
                        string nombreGr = identCN.ObtenerNombreCortoGrupoPorID(grupoID);

                        List<AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesOrganizacion> filasGrupoOrg = identDW.ListaGrupoIdentidadesOrganizacion.Where(grupoIdenOrg => grupoIdenOrg.GrupoID.Equals(grupoID)).ToList();

                        if (filasGrupoOrg != null && filasGrupoOrg.Count > 0)
                        {
                            Guid orgID = filasGrupoOrg[0].OrganizacionID;
                            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                            List<Guid> listaOrgs = new List<Guid>();
                            listaOrgs.Add(orgID);
                            nombreGr = orgCN.ObtenerNombreOrganizacionesPorIDs(listaOrgs)[orgID].Value + "|||" + nombreGr;
                            orgCN.Dispose();
                        }

                        if (!listaNombres.ContainsKey(grupoID))
                        {
                            listaNombres.Add(grupoID, nombreGr);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, mlogger);
                    }
                }
            }

            identCN.Dispose();
            return listaNombres;
        }

        public void RegistrarOrganizacionEnProyecto(AD.EntityModel.Models.IdentidadDS.Perfil pFilaPerfil, AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion pFilaPerfilOrg, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, IAvailableServices pAvailableServices)
        {
            RegistrarOrganizacionEnProyecto(pFilaPerfil, pFilaPerfilOrg, pProyectoSeleccionado, 0, pAvailableServices);
        }

        public void RegistrarOrganizacionEnProyecto(AD.EntityModel.Models.IdentidadDS.Perfil pFilaPerfil, AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion pFilaPerfilOrg, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, int pRegistroAutomaticoEnProy, IAvailableServices pAvailableServices)
        {
            RegistrarOrganizacionEnProyecto(pFilaPerfil, pFilaPerfilOrg, pProyectoSeleccionado, pRegistroAutomaticoEnProy, false, pAvailableServices);
        }

        public void RegistrarOrganizacionEnProyecto(AD.EntityModel.Models.IdentidadDS.Perfil pFilaPerfil, AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion pFilaPerfilOrg, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, int pRegistroAutomaticoEnProy, bool pActualizarLive, IAvailableServices pAvailableServices)
        {
            DataWrapperOrganizacion organizacionDW = null;
            if (pFilaPerfil.OrganizacionID.HasValue)
            {
                organizacionDW = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory).ObtenerOrganizacionPorID(pFilaPerfil.OrganizacionID.Value);
            }


            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            DataWrapperIdentidad identDW = identCN.ObtenerIdentidadesDePerfil(pFilaPerfil.PerfilID, true);
            GestionIdentidades gestIdentidad = new GestionIdentidades(identDW, null, new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identCN.Dispose();

            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = null;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();
            proyCN.Dispose();

            List<AD.EntityModel.Models.IdentidadDS.Identidad> listaIdentidades = identDW.ListaIdentidad.Where(iden => iden.ProyectoID.Equals(pProyectoSeleccionado.FilaProyecto.ProyectoID) && iden.PerfilID.Equals(pFilaPerfil.PerfilID)).ToList();
            if (listaIdentidades.Count >= 1) // TENEMOS QUE RETOMAR LA IDENTIDAD VIEJA
            {
                filaIdentidad = listaIdentidades.First();

                if (filaIdentidad.FechaBaja.HasValue)
                {
                    DateTime fechaActual = DateTime.Now;

                    //Actualizamos la identidad
                    gestIdentidad.RetomarIdentidadPerfil(filaIdentidad.IdentidadID);

                    //HABRIA QUE METER EN "HistoricoProyectoUsuario" ?? para una identidad de tipo 3 (Organizacion) ?)?
                    //De momento no metemos historico

                    //Inserto en "OrganizacionParticipaProy"
                    OrganizacionParticipaProy filaOrgParticipaProy = new OrganizacionParticipaProy();

                    filaOrgParticipaProy.FechaInicio = fechaActual;
                    filaOrgParticipaProy.IdentidadID = filaIdentidad.IdentidadID;
                    filaOrgParticipaProy.OrganizacionID = pFilaPerfilOrg.OrganizacionID;
                    filaOrgParticipaProy.OrganizacionProyectoID = pProyectoSeleccionado.FilaProyecto.OrganizacionID;
                    filaOrgParticipaProy.ProyectoID = pProyectoSeleccionado.FilaProyecto.ProyectoID;
                    filaOrgParticipaProy.RegistroAutomatico = (short)pRegistroAutomaticoEnProy;
                    filaOrgParticipaProy.EstaBloqueada = false;

                    mEntityContext.OrganizacionParticipaProy.Add(filaOrgParticipaProy);

                    //Actualizo los tags del modelo BASE
                    ControladorOrganizaciones contrOrg = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorOrganizaciones>(), mLoggerFactory);
                    contrOrg.ActualizarModeloBASE(gestIdentidad.ListaIdentidades[filaIdentidad.IdentidadID], pProyectoSeleccionado.Clave, true, true, PrioridadBase.Alta, pAvailableServices);

                    //Guardo
                    mEntityContext.SaveChanges();
                    gestIdentidad.Dispose();
                }
            }
            else if (listaIdentidades.Count == 0)//CREAMOS POR PRIMERA VEZ LA IDENTIDAD DE LA ORG EN ESTE PROYECTO
            {
                DateTime fechaActual = System.DateTime.Now;

                //Inserto en "Identidad"
                Identidad ident = gestIdentidad.AgregarIdentidadPerfil(gestIdentidad.ListaPerfiles[pFilaPerfil.PerfilID], pProyectoSeleccionado.FilaProyecto.OrganizacionID, pProyectoSeleccionado.FilaProyecto.ProyectoID, recibirNewsletterDefectoProyectos);

                filaIdentidad = ident.FilaIdentidad;

                //HABRIA QUE METER EN "HistoricoProyectoUsuario" ?? para una identidad de tipo 3 (Organizacion) ??
                //De momento no metemos historico

                //Inserto en "OrganizacionParticipaProy"
                OrganizacionParticipaProy filaOrgParticipaProy = new OrganizacionParticipaProy();

                filaOrgParticipaProy.FechaInicio = fechaActual;
                filaOrgParticipaProy.IdentidadID = ident.Clave;
                filaOrgParticipaProy.OrganizacionID = pFilaPerfilOrg.OrganizacionID;
                filaOrgParticipaProy.OrganizacionProyectoID = pProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaOrgParticipaProy.ProyectoID = pProyectoSeleccionado.FilaProyecto.ProyectoID;
                filaOrgParticipaProy.RegistroAutomatico = (short)pRegistroAutomaticoEnProy;
                filaOrgParticipaProy.EstaBloqueada = false;

                mEntityContext.OrganizacionParticipaProy.Add(filaOrgParticipaProy);

                //Actualizo los tags del modelo BASE
                ControladorOrganizaciones contrOrg = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorOrganizaciones>(), mLoggerFactory);
                contrOrg.ActualizarModeloBASE(ident, pProyectoSeleccionado.Clave, true, true, PrioridadBase.Alta, pAvailableServices);

                //Guardo
                mEntityContext.SaveChanges();
                gestIdentidad.Dispose();
            }

            if (filaIdentidad != null && filaIdentidad.Perfil != null)
            {
                ControladorOrganizaciones controladorOrganizaciones = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorOrganizaciones>(), mLoggerFactory);
                controladorOrganizaciones.ActualizarModeloBaseSimple(filaIdentidad.Perfil.OrganizacionID.Value, pProyectoSeleccionado.Clave, PrioridadBase.Alta, pAvailableServices);
            }

            #region Actualizar cola GnossLIVE
            if (pActualizarLive)
            {
                new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(),mloggerFactory).ActualizarGnossLive(pProyectoSeleccionado.Clave, pFilaPerfilOrg.PerfilID, AccionLive.Agregado, (int)TipoLive.Miembro, false, PrioridadLive.Alta, pAvailableServices);
            }
            #endregion
        }

        /// <summary>
        /// Carga la persona y la organización de la identidad si aún no las tiene cargadas
        /// </summary>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        /// <param name="pIdentidadID">Identificador de la identidad que se desea cargar</param>
        public void CompletarCargaIdentidad(Guid pIdentidadID)
        {
            Identidad ident = null;
            if (GestorIdentidades.ListaIdentidades.ContainsKey(pIdentidadID))
            {
                ident = GestorIdentidades.ListaIdentidades[pIdentidadID];
            }
            else
            {
                throw new Exception("La identidad " + pIdentidadID + " no se encuentra en la lista de identidades. Es necesario cerrar sesión y volver a entrar para que se cargue correctamente.");
            }

            //Si la identidad que comparte no está obtenida por completo, la obtengo ahora
            if (ident != null && (!ident.EsOrganizacion) && (ident.Persona == null))
            {
                //Cargo la persona
                PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
                //PersonaDS persDS = (PersonaDS)persCN.ObtenerPersonaPorIdentidadCargaLigera(pIdentidadID).Table.DataSet;
                dataWrapperPersona.ListaPersona.Add(persCN.ObtenerPersonaPorIdentidadCargaLigera(pIdentidadID));
                persCN.Dispose();

                if (GestorIdentidades.GestorPersonas == null)
                {
                    GestorIdentidades.GestorPersonas = new GestionPersonas(new DataWrapperPersona(), mLoggingService, mEntityContext);
                }

                GestorIdentidades.GestorPersonas.DataWrapperPersonas.Merge(dataWrapperPersona);
                GestorIdentidades.GestorPersonas.RecargarPersonas();
            }
            if (ident != null && (ident.TrabajaConOrganizacion) && (ident.OrganizacionPerfil == null))
            {
                //Cargo la organización
                OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                DataWrapperOrganizacion orgDW = orgCN.ObtenerOrganizacionDeIdentidad(pIdentidadID);
                orgCN.Dispose();

                if (GestorIdentidades.GestorOrganizaciones == null)
                {
                    GestorIdentidades.GestorOrganizaciones = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);
                }

                GestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(orgDW);
                GestorIdentidades.GestorOrganizaciones.CargarOrganizaciones();
            }
        }

        public void RegistrarPerfilPersonalEnProyecto(Guid pUsuarioID, Identidad pIdentidad, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, IAvailableServices pAvailableServices)
        {
            RegistrarPerfilPersonalEnProyecto(pUsuarioID, pIdentidad, pProyectoSeleccionado, false, pAvailableServices);
        }

        public void RegistrarPerfilPersonalEnProyecto(Guid pUsuarioID, Identidad pIdentidad, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, bool pActualizarLive, IAvailableServices pAvailableServices)
        {
            Guid usuarioID = pUsuarioID;

            if (pUsuarioID == Guid.Empty)
            {
                if (pIdentidad.Persona == null)
                {
                    ControladorIdentidades contrIdentidades = new ControladorIdentidades(pIdentidad.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<ControladorIdentidades>(),mloggerFactory);
                    contrIdentidades.CompletarCargaIdentidad(pIdentidad.Clave);
                }
                if (pIdentidad.Persona.FilaPersona.UsuarioID.HasValue)
                {
                    usuarioID = pIdentidad.Persona.FilaPersona.UsuarioID.Value;
                }

            }

            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            DataWrapperUsuario dataWrapperUsuario = usuarioCN.ObtenerUsuarioCompletoPorID(usuarioID);

            usuarioCN.Dispose();
            GestionUsuarios gestorUsuario = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = gestorUsuario.DataWrapperUsuario.ListaUsuario.FirstOrDefault();

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerIdentidadesDePerfil(pIdentidad.FilaIdentidad.PerfilID, true);
            identidadCN.Dispose();
            GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, pIdentidad.GestorIdentidades.GestorPersonas, pIdentidad.GestorIdentidades.GestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = null;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();
            proyCN.Dispose();

            ////Si no tiene identidad en el proyecto, la creamos
            //if (gestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + pIdentidad.PerfilUsuario.Clave + "' AND ProyectoID = '" + pProyectoSeleccionado.Clave + "'").Length == 0)
            //{
            Identidad ObjetoIdentidadProy = AgregarIdentidadPerfilYUsuarioAProyecto(gestorIdentidades, gestorUsuario, pProyectoSeleccionado.FilaProyecto.OrganizacionID, pProyectoSeleccionado.Clave, filaUsuario, pIdentidad.PerfilUsuario, recibirNewsletterDefectoProyectos);

			ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDeSolicitudes>(), mLoggerFactory);
			controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(pIdentidad.PerfilUsuario, filaUsuario, gestorUsuario, gestorIdentidades);

			//Invalido la cache de Mis comunidades
			ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            proyCL.InvalidarMisProyectos(ObjetoIdentidadProy.PerfilID);
            proyCL.Dispose();

            gestorIdentidades.RecargarHijos();
            dataWrapperIdentidad = gestorIdentidades.DataWrapperIdentidad;
            filaIdentidad = ObjetoIdentidadProy.FilaIdentidad;
            //}
            ////Si tiene una identidad dada de baja, la retomamos
            ////else if (gestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + pIdentidad.PerfilUsuario.Clave + "' AND ProyectoID = '" + pProyectoSeleccionado.Clave + "'").Length >= 1)
            //{
            //    filaIdentidad = (IdentidadDS.IdentidadRow)gestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + pIdentidad.PerfilUsuario.Clave + "' AND ProyectoID = '" + pProyectoSeleccionado.Clave + "'")[0];

            //    Guid identidadID = (Guid)filaIdentidad["IdentidadID"];

            //    if (gestorUsuario.GestorIdentidades == null)
            //    {
            //        gestorUsuario.GestorIdentidades = gestorIdentidades;
            //    }
            //    gestorUsuario.RetomarUsuarioEnProyecto(filaUsuario, pProyectoSeleccionado.FilaProyecto.OrganizacionID, pProyectoSeleccionado.Clave, identidadID);
            //    gestorIdentidades.RecargarHijos();

            //    identidadDS = gestorIdentidades.IdentidadesDS;
            //}
            //else if (gestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + pIdentidad.PerfilUsuario.Clave + "' AND ProyectoID = '" + pProyectoSeleccionado.Clave + "'").Length > 1)
            //{
            //    //Nunca debería entrar por aquí
            //}
            Guid identidadAuxID = filaIdentidad.IdentidadID;
            Guid perfilAuxID = filaIdentidad.PerfilID;

            gestorIdentidades.DataWrapperIdentidad.Merge(dataWrapperIdentidad);
            gestorIdentidades.RecargarHijos();

            #region Agrego cláusulas adicionales

            string clausulasAdicionales = "";

            foreach (string key in mHttpContextAccessor.HttpContext.Request.Query.Keys)
            {
                if (key.Contains("txtHackClausulasSelecc"))
                {
                    clausulasAdicionales = mHttpContextAccessor.HttpContext.Request.Query[key];
                    break;
                }
            }

            if (clausulasAdicionales != "")
            {
                gestorUsuario.DataWrapperUsuario.Merge(new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory).ObtenerClausulasRegitroProyecto(pProyectoSeleccionado.Clave));
                gestorUsuario.DataWrapperUsuario.Merge(new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory).ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto));

                List<AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg> listaAuxiliarProyRolUsuClausulaReg = gestorUsuario.DataWrapperUsuario.ListaProyRolUsuClausulaReg.Where(item => item.UsuarioID.Equals(filaUsuario.UsuarioID) && item.ProyectoID.Equals(pProyectoSeleccionado.Clave) && item.OrganizacionID.Equals(pProyectoSeleccionado.FilaProyecto.OrganizacionID)).ToList();
                foreach (AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg filaProyRolClau in listaAuxiliarProyRolUsuClausulaReg)
                {
                    mEntityContext.EliminarElemento(filaProyRolClau);
                    gestorUsuario.DataWrapperUsuario.ListaProyRolUsuClausulaReg.Remove(filaProyRolClau);
                }

                List<Guid> clausulasTrue = new List<Guid>();
                string[] clausulasTexto = clausulasAdicionales.Substring(clausulasAdicionales.IndexOf("||") + 2).Split(',');

                foreach (string clausula in clausulasTexto)
                {
                    if (!string.IsNullOrEmpty(clausula))
                    {
                        clausulasTrue.Add(new Guid(clausula));
                    }
                }

                UsuarioCN usuarioCN2 = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                DataWrapperUsuario usuDW = usuarioCN2.ObtenerClausulasRegitroPorID(clausulasTrue);
                usuarioCN2.Dispose();

                Dictionary<KeyValuePair<Guid, Guid>, List<Guid>> listaProyectoClausulas = new Dictionary<KeyValuePair<Guid, Guid>, List<Guid>>();
                foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in usuDW.ListaClausulaRegistro)
                {
                    KeyValuePair<Guid, Guid> orgProy = new KeyValuePair<Guid, Guid>(filaClausula.OrganizacionID, filaClausula.ProyectoID);
                    if (listaProyectoClausulas.ContainsKey(orgProy))
                    {
                        listaProyectoClausulas[orgProy].Add(filaClausula.ClausulaID);
                    }
                    else
                    {
                        List<Guid> nuevaLista = new List<Guid>();
                        nuevaLista.Add(filaClausula.ClausulaID);
                        listaProyectoClausulas.Add(orgProy, nuevaLista);
                    }
                }

                foreach (KeyValuePair<Guid, Guid> proyectoClausulaID in listaProyectoClausulas.Keys)
                {
                    gestorUsuario.AgregarClausulasAdicionalesRegistroProy(filaUsuario.UsuarioID, proyectoClausulaID.Key, proyectoClausulaID.Value, listaProyectoClausulas[proyectoClausulaID]);
                }
            }

            #endregion

            mEntityContext.SaveChanges();

            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            controladorPersonas.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[filaIdentidad.IdentidadID], pProyectoSeleccionado.Clave, true, true, PrioridadBase.Alta, pAvailableServices);

            NotificarEdicionPerfilEnProyectos(TipoAccionExterna.Registro, gestorIdentidades.ListaIdentidades[filaIdentidad.IdentidadID].PersonaID.Value, string.Empty, string.Empty);


            #region Actualizar cola GnossLIVE
            if (pActualizarLive)
            {
                new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<ControladorDocumentacion>(), mloggerFactory).ActualizarGnossLive(pProyectoSeleccionado.Clave, pIdentidad.FilaIdentidad.PerfilID, AccionLive.Agregado, (int)TipoLive.Miembro, false, PrioridadLive.Alta, pAvailableServices);
            }
            #endregion


            //Pongo a null estas dos variables para que no haga Dispose() de ellas, ya que no se han cargado en esta pantalla y puede que se usen en otro sitio. 
            gestorIdentidades.GestorPersonas = null;
            gestorIdentidades.GestorOrganizaciones = null;

            gestorIdentidades.Dispose();
            gestorIdentidades = null;

            gestorUsuario.Dispose();
            gestorUsuario = null;

            //Cuando un usuario se registra en una comunidad se actualiza automaticamente el nº de miembros en la BD, pero el DS del gestor (el que tenemos en memoria) esta desactualizado. Por ello hacemos el siguiente Merge de abajo para refrescar las estadisticas en memoria
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

            DataWrapperProyecto dataWrapperProyecto = proyectoCN.ObtenerProyectoPorID(pProyectoSeleccionado.Clave);
            pProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.Merge(dataWrapperProyecto);

            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            proyectoCL.InvalidarProyectosRecomendados(pIdentidad.PerfilID);
            proyectoCL.InvalidarCacheListaProyectosPerfil(pIdentidad.PerfilID);
            proyectoCL.InvalidarCacheListaProyectosUsuario(usuarioID);
            proyectoCL.Dispose();

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
            identidadCL.EliminarPerfilMVC(pIdentidad.PerfilID);
            identidadCL.EliminarCacheGestorIdentidad(pIdentidad.Clave, pIdentidad.PersonaID.Value);
            identidadCL.Dispose();


            //UtilUsuario.ActualizarCookiePerfil(perfilAuxID, identidadAuxID);
        }

        /// <summary>
        /// Registra un usuario en un proyecto
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <param name="pIdentidad">Identidad del usuario</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        public void RegistrarUsuarioEnProyecto(Guid pUsuarioID, Perfil pPerfil, Guid pProyectoID, TiposIdentidad pTipoIdentidad, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos, IAvailableServices pAvailableServices)
        {
            RegistrarUsuarioEnProyecto(pUsuarioID, pPerfil, pProyectoID, pTipoIdentidad, false, pRecibirNewsletterDefectoProyectos, pAvailableServices);
        }

        /// <summary>
        /// Registra un usuario en un proyecto
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <param name="pIdentidad">Identidad del usuario</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        public void RegistrarUsuarioEnProyecto(Guid pUsuarioID, Perfil pPerfil, Guid pProyectoID, TiposIdentidad pTipoIdentidad, bool pActualizarLive, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos, IAvailableServices pAvailableServices)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerProyectoPorID(pProyectoID);
            proyCN.Dispose();
            GestionProyecto gestorProy = new GestionProyecto(dataWrapperProyecto, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
            Elementos.ServiciosGenerales.Proyecto proy = gestorProy.ListaProyectos[pProyectoID];

            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            DataWrapperUsuario dataWrapperUsuario = usuarioCN.ObtenerUsuarioCompletoPorID(pUsuarioID);
            usuarioCN.Dispose();
            GestionUsuarios gestorUsuario = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = gestorUsuario.DataWrapperUsuario.ListaUsuario.FirstOrDefault();

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadDePerfilEnProyecto(pPerfil.Clave, pProyectoID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = null;

            PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            gestorIdentidades.GestorPersonas = new GestionPersonas(persCN.ObtenerPersonaPorUsuario(pUsuarioID), mLoggingService, mEntityContext);

            Identidad ObjetoIdentidadProy = AgregarIdentidadPerfilYUsuarioAProyecto(gestorIdentidades, gestorUsuario, proy.FilaProyecto.OrganizacionID, proy.Clave, filaUsuario, pPerfil, pRecibirNewsletterDefectoProyectos);

			ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDeSolicitudes>(), mLoggerFactory);
			controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(pPerfil, filaUsuario, gestorUsuario, gestorIdentidades);

            ObjetoIdentidadProy.FilaIdentidad.Tipo = (short)pTipoIdentidad;
            filaIdentidad = ObjetoIdentidadProy.FilaIdentidad;

            //Invalido la cache de Mis comunidades
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            proyCL.InvalidarMisProyectos(ObjetoIdentidadProy.PerfilID);
            proyCL.Dispose();

            gestorIdentidades.RecargarHijos();

            Guid identidadAuxID = filaIdentidad.IdentidadID;
            Guid perfilAuxID = filaIdentidad.PerfilID;

            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
            controladorPersonas.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[filaIdentidad.IdentidadID], pProyectoID, true, true, PrioridadBase.Alta, pAvailableServices);

            mEntityContext.SaveChanges();

            gestorIdentidades.Dispose();
            gestorIdentidades = null;

            gestorUsuario.Dispose();
            gestorUsuario = null;

            if (pUsuarioID.Equals(pUsuarioID))
            {
                ActualizarCookiePerfil(perfilAuxID, identidadAuxID, pUsuarioID);
            }

            #region Actualizar cola GnossLIVE
            if (pActualizarLive)
            {
                LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<LiveCN>(), mLoggerFactory);
                LiveDS liveDS = new LiveDS();

                try
                {
                    ParametroAplicacion busqueda = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("EcosistemaSinHomeUsuario"));
                    if (!(busqueda != null && busqueda.Valor == "true"))
                    {
                        InsertarFilaEnColaRabbitMQ(pProyectoID, pPerfil.Clave, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0, pAvailableServices);
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'", mlogger);
                    liveDS.Cola.AddColaRow(pProyectoID, pPerfil.Clave, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, false, 0, null);
                }

                //liveDS.ColaHomePerfil.AddColaHomePerfilRow(pProyectoID, pPerfil.Clave, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, (short)PrioridadLive.Alta);
                liveCN.ActualizarBD(liveDS);
                liveCN.Dispose();
                liveDS.Dispose();
            }
            #endregion

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
            identidadCL.EliminarPerfilMVC(filaIdentidad.PerfilID);
            identidadCL.Dispose();
        }

        public bool AbandonarComunidad(Guid pProyectoID, Guid pUsuarioID, Guid pIdentidadID, Guid pOrganizacionID, Guid pPerfilID, IAvailableServices pAvailableServices)
        {
            bool error = false;
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

            if (proyCN.EsUsuarioAdministradorProyecto(pUsuarioID, pProyectoID))
            {
                proyCN.Dispose();
                error = true;
            }
            else
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                GestionProyecto gestorProyectos = new GestionProyecto(new DataWrapperProyecto(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                GestionUsuarios gestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerUsuarioCompletoPorID(pUsuarioID), mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
                GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(pIdentidadID, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionPersonas gestorPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorPerfil(pPerfilID), mLoggingService, mEntityContext);
                gestorProyectos.GestionPersonas = gestorPersonas;
                gestorProyectos.GestionUsuarios = gestorUsuarios;
                gestorProyectos.GestionUsuarios.GestorIdentidades = gestorIdentidades;
                gestorUsuarios.GestorSuscripciones = new GestionSuscripcion(new DataWrapperSuscripcion(), mLoggingService, mEntityContext);
                gestorUsuarios.GestorSuscripciones.GestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(),mloggerFactory);
				Elementos.ServiciosGenerales.Persona persona = gestorPersonas.ListaPersonas[personaCN.ObtenerPersonaIDPorPerfil(pPerfilID)];
                List<Guid> listaProyectos = new List<Guid>();
                listaProyectos.Add(pProyectoID);
                gestorProyectos.DataWrapperProyectos.Merge(proyCN.ObtenerAccionesExternasProyectoPorListaIDs(listaProyectos));

                JsonEstado respuestaAccionExterna = AccionEnServicioExternoProyecto(TipoAccionExterna.InvalidarUsuario, persona, gestorIdentidades.ListaIdentidades[pIdentidadID].FilaIdentidad.ProyectoID, pIdentidadID, "", "", gestorIdentidades.ListaIdentidades[pIdentidadID].FilaIdentidad.FechaAlta, gestorProyectos.DataWrapperProyectos);

                //si no hay acciones externas o la respuesta de la misma es correcta, damos de baja
                if (respuestaAccionExterna == null || respuestaAccionExterna.Correcto)
                {
                    gestorProyectos.EliminarUsuarioDeProyecto(pUsuarioID, pProyectoID, pOrganizacionID, pIdentidadID, gestorUsuarios, gestorIdentidades);

                    //Invalido la cache de Mis comunidades
                    DataWrapperIdentidad idenDW = identidadCN.ObtenerIdentidadPorID(pIdentidadID, true);
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    proyCL.InvalidarMisProyectos(pPerfilID);

                    proyCL.InvalidarCacheListaProyectosPerfil(pPerfilID);
                    proyCL.InvalidarCacheListaProyectosUsuario(pUsuarioID);
                    proyCL.Dispose();


                    ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                    controladorPersonas.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[pIdentidadID], pProyectoID, false, true, PrioridadBase.Alta, pAvailableServices);

                    #region Eliminación de las Suscripciones de la identidad que abandona el proyecto

                    #region Eliminamos todas las suscripciones de esta identidad (No se incluyen las suscripciones a identidades)

                    SuscripcionCN suscripCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
                    DataWrapperSuscripcion suscripcionDW = suscripCN.ObtenerSuscripcionesDeIdentidad(pIdentidadID, true);
                    if (suscripcionDW.ListaSuscripcion.Count > 0)
                    {
                        NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                        List<Guid> listaSuscripciones = new List<Guid>();
                        gestorUsuarios.GestorSuscripciones.SuscripcionDW.Merge(suscripcionDW);
                        foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in suscripcionDW.ListaSuscripcion)
                        {
                            if (!listaSuscripciones.Contains(filaSuscripcion.SuscripcionID))
                            {
                                listaSuscripciones.Add(filaSuscripcion.SuscripcionID);
                            }
                        }

                        gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDW.Merge(notificacionCN.ObtenerNotificacionesDeSolicitudes(listaSuscripciones));

                        gestorUsuarios.GestorSuscripciones.EliminarSuscripciones(pIdentidadID);
                        notificacionCN.Dispose();
                        listaSuscripciones.Clear();
                    }
                    suscripCN.Dispose();

                    #endregion

                    #region Eliminamos todas las suscripciones a identidades en este proyecto

                    SuscripcionCN suscripCN2 = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
                    Guid identidadIDMyGNOSS = identidadCN.ObtenerIdentidadIDDeMyGNOSSPorIdentidad(pIdentidadID);
                    DataWrapperSuscripcion suscripcionDS2 = suscripCN2.ObtenerSuscripcionesDeIdentidad(identidadIDMyGNOSS, true);
                    if (suscripcionDS2.ListaSuscripcion.Count > 0)
                    {
                        NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                        List<Guid> listaSuscripciones = new List<Guid>();
                        gestorUsuarios.GestorSuscripciones.SuscripcionDW.Merge(suscripcionDS2);
                        foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in suscripcionDS2.ListaSuscripcion)
                        {
                            if (!listaSuscripciones.Contains(filaSuscripcion.SuscripcionID))
                            {
                                listaSuscripciones.Add(filaSuscripcion.SuscripcionID);
                            }
                        }

                        gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDW.Merge(notificacionCN.ObtenerNotificacionesDeSolicitudes(listaSuscripciones));

                        gestorUsuarios.GestorSuscripciones.EliminarSuscripcionesDeIdentidadEnProyecto(identidadIDMyGNOSS, pProyectoID);
                        notificacionCN.Dispose();
                        listaSuscripciones.Clear();
                    }
                    suscripCN2.Dispose();


                    #endregion

                    #endregion

                    #region Eliminamos la configuración de datos extra del proyecto

                    gestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(pIdentidadID));
                    List<DatoExtraProyectoOpcionIdentidad> listaDatoExtraProyectoOpcionIdentidad = gestorIdentidades.DataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad;
                    foreach (DatoExtraProyectoOpcionIdentidad fila in listaDatoExtraProyectoOpcionIdentidad)
                    {
                        gestorIdentidades.DataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.Remove(fila);
                        mEntityContext.EliminarElemento(fila);
                    }

                    #endregion

                    mEntityContext.SaveChanges();

                    //Eliminamos la cache de contactos en la comunidad y eliminamos los amigos de la identidad que se sale
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<AmigosCL>(),mloggerFactory);
                    amigosCL.InvalidarAmigos(identidadIDMyGNOSS);
                    DataWrapperIdentidad dwIdentidad = identidadCN.ObtenerIdentidadesDeProyecto(pProyectoID);

                    foreach(AD.EntityModel.Models.IdentidadDS.Identidad identidad in dwIdentidad.ListaIdentidad)
                    {
                        Guid identidadMyGnoss = identidadCN.ObtenerIdentidadIDDeMyGNOSSPorIdentidad(identidad.IdentidadID);
                        amigosCL.InvalidarAmigos(identidadMyGnoss);
                        amigosCL.RefrescarCacheAmigos(identidadIDMyGNOSS, mEntityContextBASE, pAvailableServices);
                    }

                    AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
                    GestionAmigos gestionAmigos = new GestionAmigos(amigosCN.CargarAmigosCompleto(identidadIDMyGNOSS), gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    gestionAmigos.EliminarAmigosReciproco(identidadIDMyGNOSS);

                    amigosCL.Dispose();
                    amigosCN.Dispose();
                    gestionAmigos.Dispose();

                    #region Actualizar cola GnossLIVE

                    new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mloggerFactory).ActualizarGnossLive(pProyectoID, pPerfilID, AccionLive.Eliminado, (int)TipoLive.Miembro, false, PrioridadLive.Alta, pAvailableServices);

                    #endregion

                    identidadCN.ActualizaIdentidades();

                    IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                    //Se debe eliminar la caché para todos los perfiles del usuario, no para el perfil de la identidad actual, ya que puede dar problemas con otros perfiles sobre la misma comunidad.
                    identidadCL.EliminarCacheGestorTodasIdentidadesUsuario(pUsuarioID, gestorIdentidades.ListaIdentidades[pIdentidadID].PersonaID.Value);

                    List<string> listaClavesInvalidar = new List<string>();

                    string prefijoClave;

                    if (!string.IsNullOrEmpty(identidadCL.Dominio))
                    {
                        prefijoClave = identidadCL.Dominio;
                    }
                    else
                    {
                        prefijoClave = IdentidadCL.DominioEstatico;
                    }

                    prefijoClave = prefijoClave + "_" + identidadCL.ClaveCache[0] + "_";
                    prefijoClave = prefijoClave.ToLower();


                    string rawKey = string.Concat("IdentidadActual_", gestorIdentidades.ListaIdentidades[pIdentidadID].PersonaID.Value, "_", pPerfilID);
                    string rawKey2 = "PerfilMVC_" + pPerfilID;
                    listaClavesInvalidar.Add(prefijoClave + rawKey.ToLower());
                    listaClavesInvalidar.Add(prefijoClave + rawKey2.ToLower());
                    identidadCL.InvalidarCachesMultiples(listaClavesInvalidar);
                    identidadCL.EliminarPerfilMVC(pPerfilID);
                    identidadCL.InvalidarFichaIdentidadMVC(pIdentidadID);

                    identidadCL.Dispose();
                    usuarioCN.Dispose();
                    identidadCN.Dispose();
                    personaCN.Dispose();
                    gestorProyectos.GestionUsuarios.GestorSuscripciones.Dispose();
                    gestorProyectos.GestionUsuarios.GestorSuscripciones.GestorNotificaciones.Dispose();
                    gestorProyectos.GestionUsuarios.GestorSuscripciones.GestorNotificaciones = null;
                    gestorProyectos.GestionUsuarios.GestorSuscripciones = null;
                    gestorProyectos.GestionUsuarios.GestorIdentidades.Dispose();
                    gestorProyectos.GestionUsuarios.GestorIdentidades = null;
                    gestorProyectos.GestionUsuarios.Dispose();
                    gestorProyectos.GestionPersonas.Dispose();
                    gestorProyectos.GestionUsuarios = null;
                    gestorProyectos.Dispose();
                    gestorProyectos = null;
                }
            }
            proyCN.Dispose();

            return error;
        }

        public void AbandonarComunidadMasivo(Elementos.ServiciosGenerales.Proyecto pProyecto, Dictionary<Guid, Guid> dicUsuariosIDsPerfilesIDs, Dictionary<Guid, Guid> dicUsuariosIDsIdentidadesIDs, Guid pGrupoID, IAvailableServices pAvailableServices)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);

            Dictionary<Guid, Guid> administradoresProyecto = proyCN.ObtenerUsuarioIDPerfilIDAdministradoresDeProyecto(pProyecto.Clave);

            //quita los administradores de la comunidad de la lista de usuarios a abandonar la comunidad
            foreach (Guid usuarioID in administradoresProyecto.Keys)
            {
                dicUsuariosIDsPerfilesIDs.Remove(usuarioID);
                dicUsuariosIDsIdentidadesIDs.Remove(usuarioID);
            }

            //Cargamos la lista habiendo quitado los usuarios administradores
            List<Guid> listaUsuariosIDs = new List<Guid>(dicUsuariosIDsPerfilesIDs.Keys);
            List<Guid> listaPerfilesIDs = new List<Guid>(dicUsuariosIDsPerfilesIDs.Values);
            List<Guid> listaIdentidadesIDs = new List<Guid>(dicUsuariosIDsIdentidadesIDs.Values);
            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesPorID(listaIdentidadesIDs, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            //si es un grupo el que está abandonando la comunidad, quita los usuarios miembros que pertenecían a la comunidad antes de que el grupo se hiciera miembro y los que ya están dados de baja
            if (!pGrupoID.Equals(Guid.Empty))
            {
                DateTime? fechaAltaGrupoOrg = proyCN.ObtenerFechaAltaGrupoOrganizacionEnProyecto(pGrupoID, pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave);

                foreach (Guid usuarioID in listaUsuariosIDs)
                {
                    if (dicUsuariosIDsIdentidadesIDs.ContainsKey(usuarioID))
                    {
                        Guid identidadID = dicUsuariosIDsIdentidadesIDs[usuarioID];
                        bool quitarDeLista = false;

                        //quita los usuarios que ya se hayan dado de baja
                        if (gestorIdentidades.ListaIdentidades[identidadID].FilaIdentidad.FechaBaja.HasValue)
                        {
                            quitarDeLista = true;
                        }
                        else if (fechaAltaGrupoOrg != null)
                        {
                            //quita los usuarios que pertenecían a la comunidad antes de que el grupo se hiciera miembro
                            if (gestorIdentidades.ListaIdentidades[identidadID].FilaIdentidad.FechaAlta < fechaAltaGrupoOrg)
                            {
                                quitarDeLista = true;
                            }
                        }

                        if (quitarDeLista)
                        {
                            dicUsuariosIDsPerfilesIDs.Remove(usuarioID);
                            dicUsuariosIDsIdentidadesIDs.Remove(usuarioID);
                        }
                    }
                }
            }

            //Recarga la lista habiendo quitado los usuarios que ya pertenecían a la comunidad antes que el grupo y los que ya se habían dado de baja antes de que lo hiciera el grupo
            listaUsuariosIDs = new List<Guid>(dicUsuariosIDsPerfilesIDs.Keys);
            listaPerfilesIDs = new List<Guid>(dicUsuariosIDsPerfilesIDs.Values);
            listaIdentidadesIDs = new List<Guid>(dicUsuariosIDsIdentidadesIDs.Values);
            gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesPorID(listaIdentidadesIDs, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            GestionPersonas gestorPersonas = new GestionPersonas(personaCN.ObtenerPersonasPorPerfilesID(listaPerfilesIDs), mLoggingService, mEntityContext);
            GestionUsuarios gestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerUsuariosCompletosPorID(listaUsuariosIDs), mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
            GestionProyecto gestorProyectos = new GestionProyecto(new DataWrapperProyecto(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
            gestorProyectos.GestionPersonas = gestorPersonas;
            gestorProyectos.GestionUsuarios = gestorUsuarios;
            gestorProyectos.GestionUsuarios.GestorIdentidades = gestorIdentidades;
            gestorIdentidades.GestorPersonas = gestorPersonas;
            gestorUsuarios.GestorSuscripciones = new GestionSuscripcion(new DataWrapperSuscripcion(), mLoggingService, mEntityContext);
            gestorUsuarios.GestorSuscripciones.GestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mloggerFactory);

            List<Guid> listaProyectos = new List<Guid>();
            listaProyectos.Add(pProyecto.Clave);
            gestorProyectos.DataWrapperProyectos.Merge(proyCN.ObtenerAccionesExternasProyectoPorListaIDs(listaProyectos));

            foreach (Guid usuarioID in listaUsuariosIDs)
            {
                Guid identidadID = dicUsuariosIDsIdentidadesIDs[usuarioID];
                Elementos.ServiciosGenerales.Persona persona = gestorPersonas.ListaPersonas[personaCN.ObtenerPersonaIDPorPerfil(dicUsuariosIDsPerfilesIDs[usuarioID])];
                JsonEstado respuestaAccionExterna = AccionEnServicioExternoProyecto(TipoAccionExterna.InvalidarUsuario, persona, gestorIdentidades.ListaIdentidades[identidadID].FilaIdentidad.ProyectoID, identidadID, "", "", gestorIdentidades.ListaIdentidades[identidadID].FilaIdentidad.FechaAlta, gestorProyectos.DataWrapperProyectos);

                if (respuestaAccionExterna != null && !respuestaAccionExterna.Correcto)
                {
                    dicUsuariosIDsPerfilesIDs.Remove(usuarioID);
                    dicUsuariosIDsIdentidadesIDs.Remove(usuarioID);
                }
            }

            //Recarga la lista habiendo quitado los usuarios que han fallado en las acciones externas
            listaUsuariosIDs = new List<Guid>(dicUsuariosIDsPerfilesIDs.Keys);
            listaPerfilesIDs = new List<Guid>(dicUsuariosIDsPerfilesIDs.Values);
            listaIdentidadesIDs = new List<Guid>(dicUsuariosIDsIdentidadesIDs.Values);
            Dictionary<Guid, Guid> dicUsuarioIDPersonaID = personaCN.ObtenerPersonasIDDeUsuariosID(listaUsuariosIDs);

            foreach (Guid usuarioID in listaUsuariosIDs)
            {
                Guid identidadID = dicUsuariosIDsIdentidadesIDs[usuarioID];
                gestorProyectos.EliminarUsuarioDeProyecto(usuarioID, pProyecto.Clave, pProyecto.FilaProyecto.OrganizacionID, identidadID, gestorUsuarios, gestorIdentidades);
            }

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            int tablaBaseProyectoID = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyecto.Clave);
            bool entraEnCom = false;
            bool actualizarTagsPersonas = true;
            bool actualizarModeloAcido = true;


            mEntityContext.NoConfirmarTransacciones = true;

            try
            {
                List<Identidad> listaIdentidades = new List<Identidad>(gestorIdentidades.ListaIdentidades.Values);
                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                controladorPersonas.ActualizarModeloBASEMasivo(listaIdentidades, pProyecto.Clave, tablaBaseProyectoID, entraEnCom, actualizarTagsPersonas, actualizarModeloAcido, PrioridadBase.Alta, pAvailableServices);

                #region Eliminación de las Suscripciones de la identidad que abandona el proyecto

                #region Eliminamos todas las suscripciones de esta identidad (No se incluyen las suscripciones a identidades)

                SuscripcionCN suscripCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
                DataWrapperSuscripcion suscripcionDW = suscripCN.ObtenerSuscripcionesDeListaIdentidades(listaIdentidadesIDs, true);
                if (suscripcionDW.ListaSuscripcion.Count > 0)
                {
                    NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                    List<Guid> listaSuscripciones = new List<Guid>();
                    gestorUsuarios.GestorSuscripciones.SuscripcionDW.Merge(suscripcionDW);
                    foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in suscripcionDW.ListaSuscripcion)
                    {
                        if (!listaSuscripciones.Contains(filaSuscripcion.SuscripcionID))
                        {
                            listaSuscripciones.Add(filaSuscripcion.SuscripcionID);
                        }
                    }

                    gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDW.Merge(notificacionCN.ObtenerNotificacionesDeSolicitudes(listaSuscripciones));

                    gestorUsuarios.GestorSuscripciones.EliminarSuscripciones(listaIdentidadesIDs);
                    notificacionCN.Dispose();
                    listaSuscripciones.Clear();
                }

                #endregion

                #region Eliminamos todas las suscripciones a identidades en este proyecto

                List<Guid> identidadesIDMyGNOSS = identidadCN.ObtenerIdentidadesIDDeMyGNOSSPorIdentidades(listaIdentidadesIDs);
                DataWrapperSuscripcion suscripcionDW2 = suscripCN.ObtenerSuscripcionesDeListaIdentidades(identidadesIDMyGNOSS, true);
                if (suscripcionDW2.ListaSuscripcion.Count > 0)
                {
                    NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                    List<Guid> listaSuscripciones = new List<Guid>();
                    gestorUsuarios.GestorSuscripciones.SuscripcionDW.Merge(suscripcionDW2);
                    foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in suscripcionDW2.ListaSuscripcion)
                    {
                        if (!listaSuscripciones.Contains(filaSuscripcion.SuscripcionID))
                        {
                            listaSuscripciones.Add(filaSuscripcion.SuscripcionID);
                        }
                    }

                    gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDW.Merge(notificacionCN.ObtenerNotificacionesDeSolicitudes(listaSuscripciones));

                    gestorUsuarios.GestorSuscripciones.EliminarSuscripcionesDeIdentidadEnProyecto(identidadesIDMyGNOSS, pProyecto.Clave);
                    notificacionCN.Dispose();
                    listaSuscripciones.Clear();
                }
                suscripCN.Dispose();

                #endregion

                #endregion

                #region Eliminamos la configuración de datos extra del proyecto

                gestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(listaIdentidadesIDs));
                List<DatoExtraProyectoOpcionIdentidad> listaAuxiliarDatoExtra = gestorIdentidades.DataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad;
                foreach (DatoExtraProyectoOpcionIdentidad fila in listaAuxiliarDatoExtra)
                {
                    mEntityContext.EliminarElemento(fila);
                    gestorIdentidades.DataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.Remove(fila);
                }

                #endregion

                mEntityContext.SaveChanges();

                identidadCN.ActualizaIdentidades();

                if (!pGrupoID.Equals(Guid.Empty))
                {
                    proyCN.BorrarFilaGrupoOrgParticipaProy(pGrupoID, pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave);
                }

                // Actualizar cola GnossLIVE masivo

                new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mloggerFactory).ActualizarGnossLiveMasivo(pProyecto.Clave, listaPerfilesIDs, AccionLive.Eliminado, (int)TipoLive.Miembro, false, "base", PrioridadLive.Alta, null, pAvailableServices);
                mEntityContext.TerminarTransaccionesPendientes(true);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                mEntityContext.TerminarTransaccionesPendientes(false);
            }

            #region Caché masivo

            //Invalido la cache de Mis comunidades
            proyCL.InvalidarMisProyectosListaPerfiles(listaPerfilesIDs);
            proyCL.InvalidarCacheListaProyectosListaPerfiles(listaPerfilesIDs);
            proyCL.InvalidarCacheListaProyectosListaUsuarios(listaUsuariosIDs);


            //Eliminamos la cache de contactos en la comunidad
            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<AmigosCL>(),mloggerFactory);
            amigosCL.InvalidarAmigosPertenecenProyecto(pProyecto.Clave);

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
            //Se debe eliminar la caché para todos los perfiles del usuario, no para el perfil de la identidad actual, ya que puede dar problemas con otros perfiles sobre la misma comunidad.
            identidadCL.EliminarCacheGestorTodasIdentidadesListaUsuarios(dicUsuarioIDPersonaID, listaUsuariosIDs);

            List<string> listaClavesInvalidar = new List<string>();
            string prefijoClave = string.Empty;

            if (!string.IsNullOrEmpty(identidadCL.Dominio))
            {
                prefijoClave = identidadCL.Dominio;
            }
            else
            {
                prefijoClave = IdentidadCL.DominioEstatico;
            }

            prefijoClave = prefijoClave + "_" + identidadCL.ClaveCache[0] + "_";
            prefijoClave = prefijoClave.ToLower();

            foreach (Guid usuarioID in dicUsuariosIDsPerfilesIDs.Keys)
            {
                if (dicUsuarioIDPersonaID.ContainsKey(usuarioID))
                {
                    string rawKey = string.Concat("IdentidadActual_", dicUsuarioIDPersonaID[usuarioID], "_", dicUsuariosIDsPerfilesIDs[usuarioID]);
                    string rawKey2 = "PerfilMVC_" + dicUsuariosIDsPerfilesIDs[usuarioID];
                    listaClavesInvalidar.Add(prefijoClave + rawKey.ToLower());
                    listaClavesInvalidar.Add(prefijoClave + rawKey2.ToLower());
                }
            }

            identidadCL.InvalidarCachesMultiples(listaClavesInvalidar);
            identidadCL.InvalidarFichasIdentidadesMVC(listaIdentidadesIDs);
            identidadCL.Dispose();
            amigosCL.Dispose();
            proyCL.Dispose();

            #endregion

            gestorProyectos.GestionUsuarios.GestorSuscripciones.Dispose();
            gestorProyectos.GestionUsuarios.GestorSuscripciones.GestorNotificaciones.Dispose();
            gestorProyectos.GestionUsuarios.GestorSuscripciones.GestorNotificaciones = null;
            gestorProyectos.GestionUsuarios.GestorSuscripciones = null;
            gestorProyectos.GestionUsuarios.GestorIdentidades.Dispose();
            gestorProyectos.GestionUsuarios.GestorIdentidades = null;
            gestorProyectos.GestionUsuarios.Dispose();
            gestorProyectos.GestionPersonas.Dispose();
            gestorProyectos.GestionUsuarios = null;
            gestorProyectos.Dispose();
            gestorProyectos = null;
            usuarioCN.Dispose();
            identidadCN.Dispose();
            personaCN.Dispose();
            proyCN.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AceptarInvitacionOrganizacion(PeticionInvOrganizacion pInvitacionAOrganizacion, Identidad pIdentidadActual, string pUrlIntraGnoss, GnossIdentity pUsuario, IAvailableServices pAvailableServices, Guid? pProyectoID = null)
        {
            bool correcto = true;

            if (pInvitacionAOrganizacion != null)
            {
                //Aceptamos la petición
                GestionIdentidades gestorIdentidades = new GestionIdentidades(new DataWrapperIdentidad(), pIdentidadActual.GestorIdentidades.GestorPersonas, pIdentidadActual.GestorIdentidades.GestorUsuarios, pIdentidadActual.GestorIdentidades.GestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionIdentidades>(), mLoggerFactory);
                pInvitacionAOrganizacion.FilaPeticion.FechaProcesado = DateTime.Now;
                pInvitacionAOrganizacion.FilaPeticion.Estado = (short)EstadoPeticion.Aceptada;
                Guid organizacionID = pInvitacionAOrganizacion.FilaInvitacionOrganizacion.OrganizacionID;

                if (pIdentidadActual.Persona.FilaPersona.EmailTutor != null)
                {
                    pInvitacionAOrganizacion.FilaInvitacionOrganizacion.Email = pIdentidadActual.Persona.FilaPersona.EmailTutor;
                }
                else
                {
                    pInvitacionAOrganizacion.FilaInvitacionOrganizacion.Email = pIdentidadActual.Persona.FilaPersona.Email;
                }

                #region Usuario Actual

                pInvitacionAOrganizacion.FilaPeticion.UsuarioID = pUsuario.UsuarioID;

                //Necesitamos cargar la tabla: OrganizacionParticipaProy para registrar a la org en esas comunidades...
                GestionOrganizaciones gestOrg = new GestionOrganizaciones(new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory).ObtenerOrganizacionPorID(organizacionID), mLoggingService, mEntityContext);

                Elementos.ServiciosGenerales.Organizacion organizacion = gestOrg.ListaOrganizaciones[organizacionID];

                DatosTrabajoPersonaOrganizacion perfilPersonaOrganizacion = gestOrg.VincularPersonaOrganizacion(organizacion, pUsuario.PersonaID);
                PersonaVinculoOrganizacion filaOrgPersona = perfilPersonaOrganizacion.FilaVinculo;
                if (pIdentidadActual.Persona.FilaPersona.EmailTutor != null)
                {
                    filaOrgPersona.EmailTrabajo = pIdentidadActual.Persona.FilaPersona.EmailTutor;
                }
                else
                {
                    filaOrgPersona.EmailTrabajo = pIdentidadActual.Persona.FilaPersona.Email;
                }

                if (pInvitacionAOrganizacion.FilaInvitacionOrganizacion.Cargo != null)
                {
                    filaOrgPersona.Cargo = pInvitacionAOrganizacion.FilaInvitacionOrganizacion.Cargo;
                }

                if (pIdentidadActual != null)
                {
                    gestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(gestOrg.OrganizacionDW);
                    gestorIdentidades.GestorOrganizaciones.CargarOrganizaciones();
                }

                //Creamos el perfil persona + organización o lo retomamos si ya existía
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperIdentidad identDW = identCN.ObtenerPerfilesDePersona(pIdentidadActual.Persona.Clave, false, pIdentidadActual.Clave);

                Perfil perfil = null;

                if (!pIdentidadActual.Persona.UsuarioCargado)
                {
                    if (pIdentidadActual.Persona.GestorPersonas.GestorUsuarios == null)
                    {
                        pIdentidadActual.Persona.GestorPersonas.GestorUsuarios = new GestionUsuarios(new DataWrapperUsuario(), mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
                    }

                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<UsuarioCN>(), mloggerFactory);
                    AD.EntityModel.Models.UsuarioDS.Usuario usuario = usuarioCN.ObtenerUsuarioPorID(pIdentidadActual.Persona.UsuarioID);

                    if (usuario != null)
                    {
                        DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
                        dataWrapperUsuario.ListaUsuario.Add(usuario);
                        pIdentidadActual.Persona.GestorPersonas.GestorUsuarios.DataWrapperUsuario.Merge(dataWrapperUsuario);
                        pIdentidadActual.Persona.GestorPersonas.GestorUsuarios.RecargarUsuarios();
                    }
                }

                DataWrapperUsuario usuDW = pIdentidadActual.Persona.GestorPersonas.GestorUsuarios.DataWrapperUsuario;
                GestionUsuarios gestorUsuario = new GestionUsuarios(usuDW, mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);

                gestorIdentidades.GestorUsuarios = gestorUsuario;
                gestorIdentidades.GestorPersonas.GestorUsuarios = gestorUsuario;

                AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg filaPerfilPersonaOrg = null;
                List<string> listaContactosOrganizacion = new List<string>();
                string identidadReal = "";

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();

                if (!identDW.ListaPerfilPersonaOrg.Any(perfilPersOrg => perfilPersOrg.OrganizacionID.Equals(organizacionID) && perfilPersOrg.PersonaID.Equals(pIdentidadActual.Persona.Clave)))
                {
                    //Lo creamos nuevo
                    LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<LiveCN>(), mLoggerFactory);
                    LiveDS liveDS = new LiveDS();
                    identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    gestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesDeOrganizacion(organizacionID, ProyectoAD.MetaProyecto));
                    AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
                    gestorIdentidades.GestorAmigos = new GestionAmigos(new DataWrapperAmigos(), gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    perfil = AgregarPerfilPersonaOrganizacion(gestorIdentidades, gestorIdentidades.GestorOrganizaciones, gestorIdentidades.GestorUsuarios, pIdentidadActual.Persona, organizacion, true, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, liveDS, recibirNewsletterDefectoProyectos, pAvailableServices);

                    liveCN.ActualizarBD(liveDS);
                    amigosCN.Actualizar();


                    filaPerfilPersonaOrg = gestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.FirstOrDefault(perfilPersonOrg => perfilPersonOrg.OrganizacionID.Equals(organizacionID) && perfilPersonOrg.PersonaID.Equals(pIdentidadActual.Persona.Clave));
                    AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = gestorUsuario.DataWrapperUsuario.ListaUsuario.FirstOrDefault(usuario => usuario.UsuarioID.Equals(pUsuario.UsuarioID));
                    gestorUsuario.AgregarUsuarioAProyecto(filaUsuario, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, perfil.IdentidadMyGNOSS.Clave, false);

                    if (pProyectoID.HasValue && !pProyectoID.Value.Equals(ProyectoAD.MetaProyecto) && !gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(ident => ident.ProyectoID.Equals(pProyectoID.Value) && ident.Tipo != 3))
                    {
                        //Hay que añadir al usuario al proyecto pProyectoID
                        Identidad identidadProyecto = AgregarIdentidadPerfilYUsuarioAProyecto(gestorIdentidades, gestorUsuario, ProyectoAD.MetaOrganizacion, pProyectoID.Value, filaUsuario, perfil, recibirNewsletterDefectoProyectos);

						ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDeSolicitudes>(), mLoggerFactory);
						controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(perfil, filaUsuario, gestorUsuario, gestorIdentidades, pProyectoID);
					}

                    //Invalido la cache de Mis comunidades
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    proyCL.InvalidarMisProyectos(filaPerfilPersonaOrg.PerfilID);

                    gestorIdentidades.RecargarHijos();

                    //Agregamos al modelo base las comunidades en las que se ha hecho miembro el usuario:
                    foreach (LiveDS.ColaRow colaLiveRow in liveDS.Cola)
                    {
                        List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidad = perfil.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(colaLiveRow.ProyectoId) && ident.PerfilID.Equals(perfil.Clave)).ToList();

                        if (filasIdentidad.Count > 0)
                        {
                            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                            controladorPersonas.ActualizarModeloBASE(perfil.GestorIdentidades.ListaIdentidades[(filasIdentidad[0]).IdentidadID], colaLiveRow.ProyectoId, true, true, PrioridadBase.Alta, pAvailableServices);
                        }
                    }

                    //Agregamos como contactos a las personas de la organización y a la propia organización
                    ControladorAmigos controlador = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory);
                    listaContactosOrganizacion = controlador.AgregarContactosOrganizacion(organizacionID, perfil.IdentidadMyGNOSS.Clave, gestorIdentidades);
                    identidadReal = perfil.IdentidadMyGNOSS.Clave.ToString();
                }
                else
                {
                    //Lo tenemos que retomar
                    filaPerfilPersonaOrg = identDW.ListaPerfilPersonaOrg.FirstOrDefault(perfilPersonOrg => perfilPersonOrg.OrganizacionID.Equals(organizacionID) && perfilPersonOrg.PersonaID.Equals(pIdentidadActual.Persona.Clave));
                    AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = identDW.ListaPerfil.Find(perf => perf.PerfilID.Equals(filaPerfilPersonaOrg.PerfilID));

                    AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = identDW.ListaIdentidad.FirstOrDefault(iden => iden.PerfilID.Equals(filaPerfil.PerfilID) && iden.ProyectoID.Equals(ProyectoAD.MetaProyecto));

                    if (filaPerfil.Eliminado)
                    {
                        gestorIdentidades.DataWrapperIdentidad.ListaPerfil.Add(filaPerfil);
                        gestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Add(filaPerfilPersonaOrg);
                        gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Add(filaIdentidad);
                        gestorIdentidades.RecargarHijos();

                        //Agregamos como contactos a las personas de la organización y a la propia organización
                        ControladorAmigos controlador = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory);
                        listaContactosOrganizacion = controlador.AgregarContactosOrganizacion(organizacionID, filaIdentidad.IdentidadID, gestorIdentidades);
                        identidadReal = filaIdentidad.IdentidadID.ToString();
                        //Retomamos el perfil y la identidad en el metaproyecto
                        RetomarPerfil(filaPerfilPersonaOrg.PerfilID, gestorIdentidades, gestorUsuario, gestorIdentidades.GestorOrganizaciones, true, ProyectoAD.MetaProyecto, recibirNewsletterDefectoProyectos, pUsuario.UsuarioID);

                        //Agregamos ProyectoUsuarioIdentidad en el metaproyecto
                        gestorUsuario.AgregarUsuarioAProyecto(gestorUsuario.ListaUsuarios[pUsuario.UsuarioID].FilaUsuario, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, filaIdentidad.IdentidadID, false);

                        //Invalido la cache de Mis comunidades
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                        proyCL.InvalidarMisProyectos(filaIdentidad.PerfilID);

                        gestorIdentidades.RecargarHijos();
                    }
                    else
                    {
                        correcto = false;
                        return correcto;
                    }
                }
                gestorUsuario.AgregarOrganizacionRolUsuario(pUsuario.UsuarioID, organizacionID);

                NotificacionCN notifCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notifCN.ObtenerInvitacionesPendientesDeMyGnoss(pIdentidadActual.FilaIdentidad.PerfilID, pIdentidadActual.Persona.Clave, null, false), gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<GestionNotificaciones>(),mloggerFactory);


                int numInvitacionesAcepRecha = 1;

                foreach (Elementos.Invitacion.Invitacion invitacion in gestorNotificaciones.ListaInvitaciones.Values)
                {
                    if ((invitacion.TipoInvitacion == TiposNotificacion.InvitacionUsuarioAOrgCorp
                    || invitacion.TipoInvitacion == TiposNotificacion.InvitacionUsuarioAOrgPers)
                    && invitacion.FilaInvitacion.Estado == (int)EstadoInvitacion.Pendiente
                    && invitacion.FilaInvitacion.ElementoVinculadoID == organizacionID)
                    {
                        invitacion.FilaInvitacion.Estado = (int)EstadoInvitacion.Rechazada;
                        numInvitacionesAcepRecha++;
                    }
                }

                mEntityContext.SaveChanges();

                //Agregamos a Virtuoso e Insertamos una fila en BASE
                ControladorContactos contrContactos = new ControladorContactos(mLoggingService, mEntityContext, mConfigService, mEntityContextBASE, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorContactos>(), mLoggerFactory);
                FacetadoCN facetadoCN = new FacetadoCN(pUrlIntraGnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                if (listaContactosOrganizacion.Count > 0)
                {
                    foreach (string idAmigo in listaContactosOrganizacion)
                    {
                        facetadoCN.InsertarNuevoContacto(identidadReal, idAmigo);
                        contrContactos.ActualizarModeloBaseSimple(new Guid(identidadReal), new Guid(idAmigo));
                    }
                }
                facetadoCN.Dispose();

                new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory).AgregarNotificacionInvitacionLeidaAPerfil(pIdentidadActual.PerfilID, numInvitacionesAcepRecha);

                #endregion

                //Borro la caché para que aparezca la identidad en el menú superior:
                IdentidadCL identCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                identCL.EliminarCacheGestorTodasIdentidadesUsuario(pUsuario.UsuarioID, pUsuario.PersonaID);
                identCL.EliminarCacheGestorIdentidadActual(pUsuario.UsuarioID, pUsuario.IdentidadID, pUsuario.PersonaID);

                identCL.InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(organizacionID);

                UsuarioCL usuarioCL = new UsuarioCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCL>(), mLoggerFactory);
                usuarioCL.EliminarCacheUsuariosCargaLigeraParaFiltros(organizacionID);
                usuarioCL.Dispose();
            }

            return correcto;
        }


        /// <summary>
        /// Aceptamos una invitación a ser contacto y hacemos contactos a las dos identidades
        /// </summary>
        /// <param name="pIdentidadInvitacionContactoID">ID de la identidad que quiere ser contacto de otra.</param>
        /// <param name="pIdentidadActual">Identidad actual que se va a hacer contacto de la pIdentidadInvitacionContactoID</param>
        /// <param name="pUrlIntragnoss"></param>
        /// <param name="pBaseUrl"></param>
        /// <param name="pBaseURLIdioma"></param>
        /// <param name="pProyectoSeleccionado">Proyecto en el que se van a hacer contactos</param>
        /// <param name="pPeticionActual"></param>
        public void AceptarInvitacionContacto(Guid pIdentidadInvitacionContactoID, Identidad pIdentidadActual, string pUrlIntragnoss, string pBaseUrl, string pBaseURLIdioma, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, Peticion pPeticionActual, string pLanguageCode, GnossIdentity pUsuario, IAvailableServices pAvailableServices)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            GestionPersonas GestorPersonas = new GestionPersonas(personaCN.ObtenerPersonasPorIdentidad(pIdentidadInvitacionContactoID), mLoggingService, mEntityContext);
            personaCN.Dispose();

            OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
            GestionOrganizaciones GestorOrganizaciones = new GestionOrganizaciones(organizacionCN.ObtenerOrganizacionesPorIdentidad(pIdentidadInvitacionContactoID), mLoggingService, mEntityContext);
            personaCN.Dispose();


            IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            Identidad identidadOrigen = new GestionIdentidades(idenCN.ObtenerIdentidadPorID(pIdentidadInvitacionContactoID, true), GestorPersonas, GestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication).ListaIdentidades[pIdentidadInvitacionContactoID];
            Identidad identidadDestino = pIdentidadActual;
            idenCN.Dispose();

            ControladorAmigos controladorAmigos = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorAmigos>(), mLoggerFactory);
            //controladorAmigos.CargarAmigos(identidadOrigen);
            //controladorAmigos.CargarAmigos(pIdentidadActual);

            #region Cargo Grupo Necesarios

            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCN>(), mLoggerFactory);
            //gestionAmigos.AmigosDS.Merge(amigosCN.ObtenerGruposAmigosConAccesoParaIdentidad(identidadOrigen.IdentidadMyGNOSS.Clave));
            //gestionAmigos.AmigosDS.Merge(amigosCN.ObtenerGruposAmigosConAccesoParaIdentidad(pIdentidadActual.Clave));
            //gestionAmigos.AmigosDS.Merge(amigosCN.ObtenerGruposAmigosConAccesoParaIdentidad(pIdentidadActual.IdentidadMyGNOSS.Clave));
            amigosCN.Dispose();

            #endregion

            GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mloggerFactory);

            gestorNotificaciones.AgregarNotificacionCorreo(pIdentidadActual, identidadOrigen, TiposNotificacion.ConfirmacionContacto, pBaseUrl, pProyectoSeleccionado, pLanguageCode);

            mEntityContext.SaveChanges();

            NotificacionCN notCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
            notCN.ActualizarNotificacion(pAvailableServices);
            notCN.Dispose();

            //Agregamos a Virtuoso
            FacetadoCN facetadoCN = new FacetadoCN(pUrlIntragnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            facetadoCN.InsertarNuevoContacto(identidadOrigen.IdentidadMyGNOSS.Clave.ToString(), pIdentidadActual.IdentidadMyGNOSS.Clave.ToString());
            facetadoCN.Dispose();

            //Agregamos una fila al BASE
            ControladorContactos contrContactos = new ControladorContactos(mLoggingService, mEntityContext, mConfigService, mEntityContextBASE, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorContactos>(), mLoggerFactory);
            contrContactos.ActualizarModeloBaseSimple(identidadOrigen.IdentidadMyGNOSS.Clave, pIdentidadActual.IdentidadMyGNOSS.Clave);

            //Limpiamos la cache de los contactos
            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<AmigosCL>(), mloggerFactory);
            amigosCL.InvalidarAmigos(identidadOrigen.IdentidadMyGNOSS.Clave);
            amigosCL.InvalidarAmigos(pIdentidadActual.IdentidadMyGNOSS.Clave);
            amigosCL.Dispose();


            //Enviamos un correo interno a la persona con la noticia:

            #region Correo

            UtilIdiomas utilIdiomas = new UtilIdiomas(pUsuario.Idioma, mLoggingService, mEntityContext, mConfigService,mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);

            GestionCorreo gestorCorreo = new GestionCorreo(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, pAvailableServices, mLoggerFactory.CreateLogger<GestionCorreo>(), mLoggerFactory);
            List<Guid> listaDestinatarioCorreo = new List<Guid>();
            listaDestinatarioCorreo.Add(identidadOrigen.Clave);

            string asunto = utilIdiomas.GetText("INVITACIONES", "ASUNTO_ACEPTACIONINVITACIONCONTACTO", identidadDestino.Nombre());
            string cuerpo = utilIdiomas.GetText("INVITACIONES", "CUERPO_ACEPTACIONINVITACIONCONTACTO", identidadDestino.Nombre(), UrlsSemanticas.GetURLPerfilDeIdentidad(pBaseURLIdioma, utilIdiomas, identidadDestino));
            Guid correoEnviado = gestorCorreo.AgregarCorreo(identidadDestino.Clave, listaDestinatarioCorreo, asunto, cuerpo, null, TipoEnvioCorreoBienvenida.CorreoInterno, pProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, pLanguageCode);

            ControladorDocumentacion controladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mloggerFactory);
            string destinatarios = "";
            foreach (Guid destinatario in listaDestinatarioCorreo)
            {
                destinatarios += $"|{destinatario.ToString()}";
            }

            if (destinatarios.StartsWith("|")) destinatarios = destinatarios.Substring(1);

            controladorDocumentacion.AgregarMensajeFacModeloBaseSimple(correoEnviado, identidadDestino.Clave, pProyectoSeleccionado.Clave, "base", destinatarios, null, PrioridadBase.Alta, pAvailableServices);

            //Pongo el correo como eliminado para que no le aparezca a la persona que envia el correo en su bandeja de salida:
            foreach (CorreoDS.CorreoInternoRow fila in gestorCorreo.CorreoDS.CorreoInterno.Rows)
            {
                if (fila.CorreoID == correoEnviado)
                {
                    Correo correo = new Correo(fila, gestorCorreo);
                    if (correo.Enviado)
                    {
                        correo.FilaCorreo.Eliminado = true;
                        correo.FilaCorreo.EnPapelera = true;
                    }
                }
            }

            CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoCN>(), mLoggerFactory);
            correoCN.ActualizarCorreo(gestorCorreo.CorreoDS);
            gestorCorreo.CorreoDS.Dispose();
            gestorCorreo.Dispose();

            #endregion

            //Limpiamos la cache de los contactos
            controladorAmigos.InvalidarAmigosIdentidad(identidadOrigen.IdentidadMyGNOSS.Clave);
            controladorAmigos.InvalidarAmigosIdentidad(identidadDestino.Clave);

            pPeticionActual.FilaPeticion.FechaProcesado = DateTime.Now;
            pPeticionActual.FilaPeticion.Estado = (short)EstadoPeticion.Aceptada;
            pPeticionActual.FilaPeticion.UsuarioID = pUsuario.UsuarioID;

            PeticionCN petCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PeticionCN>(), mLoggerFactory);
            petCN.ActualizarBD();
            petCN.Dispose();
        }

        public void AceptarInvitacionComunidadConInvitacion(Guid pProyectoID, Guid pOrganizacionID, Identidad pIdentidadActual, DataWrapperUsuario pDataWrapperUsuario, Guid pUsuarioID, IAvailableServices pAvailableServices)
        {
            GestionUsuarios gestorUsuarios = new GestionUsuarios(new DataWrapperUsuario(), mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = pDataWrapperUsuario.ListaUsuario.FirstOrDefault(usuario => usuario.UsuarioID.Equals(pUsuarioID));

            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyectoCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();


            if (!proyectoCN.ParticipaUsuarioEnProyecto(pProyectoID, pUsuarioID))
            {
                Elementos.ServiciosGenerales.Proyecto proyecto = new GestionProyecto(proyectoCN.ObtenerProyectoPorID(pProyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory).ListaProyectos[pProyectoID];

                Identidad ObjetoIdentidadProy = AgregarIdentidadPerfilYUsuarioAProyecto(pIdentidadActual.GestorIdentidades, gestorUsuarios, pOrganizacionID, pProyectoID, filaUsuario, pIdentidadActual.PerfilUsuario, recibirNewsletterDefectoProyectos);

				ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDeSolicitudes>(), mLoggerFactory);
				controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(pIdentidadActual.PerfilUsuario, filaUsuario, gestorUsuarios, pIdentidadActual.GestorIdentidades, pProyectoID);

				//Invalido la cache de Mis comunidades
				ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                proyCL.InvalidarMisProyectos(ObjetoIdentidadProy.PerfilID);
                proyCL.Dispose();

                pIdentidadActual.GestorIdentidades.RecargarHijos();
                //listaProyectos.Add(proyectoID);

                //Limpiar la caché de los miembros de la comunidad para el servicio web autocompletar de editores...
                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                identidadCL.InvalidarCacheMiembrosComunidad(pProyectoID);
                identidadCL.Dispose();

                //Limpiar la cache de amigos de los miembros de la comunidad
                AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<AmigosCL>(),mloggerFactory);
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperIdentidad dwIdentidad = identidadCN.ObtenerIdentidadesDeProyecto(pProyectoID);

                foreach (AD.EntityModel.Models.IdentidadDS.Identidad identidad in dwIdentidad.ListaIdentidad)
                {
                    Guid identidadMyGnoss = identidadCN.ObtenerIdentidadIDDeMyGNOSSPorIdentidad(identidad.IdentidadID);
                    amigosCL.InvalidarAmigos(identidadMyGnoss);
                    amigosCL.RefrescarCacheAmigos(identidadMyGnoss, mEntityContextBASE, pAvailableServices);
                }

                mEntityContext.SaveChanges();
            }

            proyectoCN.Dispose();
        }

        #region Métodos Privados SmartFocus


        /// <summary>
        /// Realiza una petición HTTP
        /// </summary>
        /// <param name="pUrl"></param>
        /// <param name="pContentType"></param>
        /// <param name="pCuerpo"></param>
        /// <param name="pAuthorization"></param>
        /// <param name="pMetodo"></param>
        /// <returns></returns>
        public static string HacerPeticion(string pUrl, string pContentType = null, string pCuerpo = null, string pAuthorization = null, string pMetodo = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pUrl);

            // Set some reasonable limits on resources used by this request
            request.MaximumAutomaticRedirections = 4;
            //request.MaximumResponseHeadersLength = 4;
            // Set credentials to use for this request.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = UtilWeb.GenerarUserAgent();

            if (!string.IsNullOrEmpty(pMetodo))
            {
                request.Method = pMetodo;
            }

            if (!string.IsNullOrEmpty(pAuthorization))
            {
                request.PreAuthenticate = true;
                request.Headers["Authorization"] = pAuthorization;
            }

            request.UserAgent = "gnoss";
            request.KeepAlive = true;

            if (!string.IsNullOrEmpty(pContentType))
            {
                request.ContentType = pContentType;
            }

            if (!string.IsNullOrEmpty(pCuerpo))
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] paramByte = encoding.GetBytes(pCuerpo);
                request.ContentLength = paramByte.Length;

                Stream streamRequest = request.GetRequestStream();
                streamRequest.Write(paramByte, 0, paramByte.Length);
                streamRequest.Flush();
                streamRequest.Close();
                streamRequest.Dispose();
            }

            HttpWebResponse response = null;
            Stream receiveStream = null;
            StreamReader readStream = null;
            string respuesta = "";

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                // Get the stream associated with the response.
                receiveStream = response.GetResponseStream();
                // Pipes the stream to a higher level stream reader with the required encoding format. 
                readStream = new StreamReader(receiveStream, Encoding.UTF8);

                respuesta = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                readStream.Dispose();
            }
            catch (WebException webEx)
            {
                try
                {
                    StreamReader sr = new StreamReader(webEx.Response.GetResponseStream());
                    string error = sr.ReadToEnd();
                    sr.Close();
                    throw;
                }
                catch { }
            }

            return respuesta;
        }

        #endregion

        public Perfil AgregarPerfilPersonaOrganizacion(GestionIdentidades pGestorIdentidades, GestionOrganizaciones pGestorOrganizaciones, GestionUsuarios pGestorUsuarios, Es.Riam.Gnoss.Elementos.ServiciosGenerales.Persona pPersona, Elementos.ServiciosGenerales.Organizacion pOrganizacion, bool pCrearIdentidadEnMetaProyecto, Guid? pMetaOrganizacionID, Guid? pMetaProyectoID, LiveDS pLiveDS, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos, IAvailableServices pAvailableServices)
        {
            Perfil perfil = pGestorIdentidades.AgregarPerfilPersonaOrganizacion(pPersona, pOrganizacion, pCrearIdentidadEnMetaProyecto, pMetaOrganizacionID, pMetaProyectoID, pRecibirNewsletterDefectoProyectos);

            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = (AD.EntityModel.Models.UsuarioDS.Usuario)pGestorUsuarios.ListaUsuarios[pPersona.UsuarioID].FilaElementoEntity;

            List<string> filasInsertarBase = new List<string>();

            foreach (OrganizacionParticipaProy filaOrgPartProy in pGestorOrganizaciones.OrganizacionDW.ListaOrganizacionParticipaProy.Where(item => item.OrganizacionID.Equals(perfil.OrganizacionID) && item.RegistroAutomatico > 0))
            {
                //Org del proycto seleccionado
                Guid perfilID = perfil.Clave;
                Guid organizacionID = filaOrgPartProy.OrganizacionProyectoID;
                Guid comunidadID = filaOrgPartProy.ProyectoID;
                Guid identidadID = Guid.Empty;

                bool participaYaConOtroPerfil = false;

                foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in pGestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.PersonaID.Equals(perfil.PersonaID)).ToList())
                {//"PerfilID='" + filaPerfil.PerfilID + "' AND ProyectoID = '" + comunidadID + "' AND FechaBaja is null AND FechaExpulsion is null"
                    if (pGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(ident => ident.PerfilID.Equals(filaPerfil.PerfilID) && ident.ProyectoID.Equals(comunidadID) && !ident.FechaBaja.HasValue && !ident.FechaExpulsion.HasValue))
                    {
                        participaYaConOtroPerfil = true;
                    }
                }

                //Si no participa en la comunidad con un perfil distinto
                if (!participaYaConOtroPerfil)
                {
                    ////Si el perfil no tiene identidad
                    //if (pGestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + perfilID + "' AND ProyectoID = '" + comunidadID + "'").Length == 0)
                    //{
                    ControladorIdentidades controladorIdentidades = new ControladorIdentidades(pGestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ControladorIdentidades>(), mloggerFactory);
                    Identidad ObjetoIdentidadProy = controladorIdentidades.AgregarIdentidadPerfilYUsuarioAProyecto(pGestorIdentidades, pGestorUsuarios, organizacionID, comunidadID, filaUsuario, perfil, pRecibirNewsletterDefectoProyectos);

					ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDeSolicitudes>(), mLoggerFactory);
					controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(perfil, filaUsuario, pGestorUsuarios, pGestorIdentidades);

                    identidadID = ObjetoIdentidadProy.Clave;
                    ObjetoIdentidadProy.Tipo = (TiposIdentidad)filaOrgPartProy.RegistroAutomatico;
                    pGestorIdentidades.RecargarHijos();
                    //}
                    ////si la tiene es que esta dado de baja
                    //else if (pGestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + perfilID + "' AND ProyectoID = '" + comunidadID + "'").Length == 1)
                    //{
                    //    identidadID = (Guid)pGestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + perfilID + "' AND ProyectoID = '" + comunidadID + "'")[0]["IdentidadID"];

                    //    if (pGestorUsuarios.GestorIdentidades == null)
                    //    {
                    //        pGestorUsuarios.GestorIdentidades = pGestorIdentidades;
                    //    }
                    //    pGestorUsuarios.RetomarUsuarioEnProyecto(filaUsuario, organizacionID, comunidadID, identidadID);
                    //    pGestorIdentidades.RecargarHijos();
                    //}

                    DateTime fechaAlta = pGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.FirstOrDefault(ident => ident.PerfilID.Equals(perfilID) && ident.ProyectoID.Equals(comunidadID)).FechaAlta;


                    ParametroAplicacion busqueda = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("EcosistemaSinHomeUsuario"));
                    if (!(busqueda != null && busqueda.Valor == "true"))
                    {
                        filasInsertarBase.Add(PreprarFilaParaColaRabbitMQ(filaOrgPartProy.ProyectoID, perfilID, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, fechaAlta, false, (short)PrioridadLive.Alta));
                    }

                }
            }

            #region Actualizar Base

            if (filasInsertarBase.Count > 0)
            {
                InsertarFilasEnColaRabbitMQ(filasInsertarBase);
            }

            #endregion

            return perfil;
        }

        public Identidad AgregarIdentidadPerfilYUsuarioAProyecto(GestionIdentidades pGestorIdentidades, GestionUsuarios pGestorUsuarios, Guid pOrganizacionID, Guid pProyectoID, AD.EntityModel.Models.UsuarioDS.Usuario pFilaUsuario, Perfil pPerfilPersona, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos)
        {
            Identidad identidad = null;
            //"PerfilID = '" + pPerfilPersona.Clave + "' AND ProyectoID = '" + pProyectoID + "'"
            List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidades = pGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(pPerfilPersona.Clave) && ident.ProyectoID.Equals(pProyectoID)).ToList();

            if (filasIdentidades.Count > 0)
            {
                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = filasIdentidades[0];

                filaIdentidad.FechaAlta = DateTime.Now;
                filaIdentidad.FechaBaja = null;
                filaIdentidad.FechaExpulsion = null;

                identidad = pGestorIdentidades.ListaIdentidades[filaIdentidad.IdentidadID];
            }

            //Puede que la identidad no se retome con el mismo perfil
            if (identidad == null)
            {
                //se agrega la identidad al perfil
                identidad = pGestorIdentidades.AgregarIdentidadPerfil(pPerfilPersona, pOrganizacionID, pProyectoID, pRecibirNewsletterDefectoProyectos);
            }

            pGestorUsuarios.AgregarUsuarioAProyecto(pFilaUsuario, pOrganizacionID, pProyectoID, identidad.Clave);

            if (pGestorIdentidades.GestorSuscripciones == null)
            {
                pGestorIdentidades.GestorSuscripciones = new GestionSuscripcion(new DataWrapperSuscripcion(), mLoggingService, mEntityContext);
            }
            new ControladorSuscripciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorSuscripciones>(), mLoggerFactory).SuscribirIdentidadATesauroProyecto(pGestorIdentidades.GestorSuscripciones, identidad.Clave, pProyectoID);

            return identidad;
        }

        /// <summary>
        /// Crea el perfil persona+organización para la persona y la organización pasadas por parámetro
        /// </summary>
        /// <param name="pPersona">Persona</param>
        /// <param name="pOrganizacion">Organización</param>
        /// <param name="pCrearIdentidadEnMetaProyecto">TRUE si debe crear la identidad para ese perfil en el metaproyecto, FALSE en caso contrario</param>
        /// <param name="pMetaOrganizacionID">Identificador de la metaorganización</param>
        /// <param name="pMetaProyectoID">Identificador del metaproyecto</param>
        /// <param name="pLiveDS">Dataset para el modelo LIVE</param>
        public void CrearPerfilPersonaOrganizacion(Elementos.ServiciosGenerales.Persona pPersona, Elementos.ServiciosGenerales.Organizacion pOrganizacion, bool pCrearIdentidadEnMetaProyecto, Guid? pMetaOrganizacionID, Guid? pMetaProyectoID, LiveDS pLiveDS, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos, IAvailableServices pAvailableServices)
        {
            //Lo tengo que crear nuevo
            if (!pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Any(item => item.OrganizacionID.Equals(pOrganizacion.Clave) && item.PersonaID.Equals(pPersona.Clave)))
            {
                AgregarPerfilPersonaOrganizacion(pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades, pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorOrganizaciones, pPersona.GestorPersonas.GestorUsuarios, pPersona, pOrganizacion, pCrearIdentidadEnMetaProyecto, pMetaOrganizacionID, pMetaProyectoID, pLiveDS, pRecibirNewsletterDefectoProyectos, pAvailableServices);
            }
            else
            {
                //Lo tengo que retomar (y está cargado en memoria)
                if (pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.Any(item => item.OrganizacionID.Equals(pOrganizacion.Clave) && item.PersonaID.Equals(pPersona.Clave)))
                {
                    Guid PerfilID = pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.FirstOrDefault(item => item.OrganizacionID.Equals(pOrganizacion.Clave) && item.PersonaID.Equals(pPersona.Clave)).PerfilID;
                    RetomarPerfil(PerfilID, pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades, pPersona.GestorPersonas.GestorUsuarios, pPersona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorOrganizaciones, pCrearIdentidadEnMetaProyecto, pMetaProyectoID, pRecibirNewsletterDefectoProyectos, pPersona.UsuarioID);
                }
            }
        }

        public void RetomarPerfil(Guid pPerfilID, GestionIdentidades pGestorIdentidades, GestionUsuarios pGestorUsuarios, GestionOrganizaciones pGestorOrganizaciones, bool pRetomarIdentidadEnMetaProyecto, Guid? pMetaProyectoID, Dictionary<Guid, bool> pRecibirNewsletterDefectoProyectos, Guid pUsuarioID)
        {
            AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = pGestorIdentidades.DataWrapperIdentidad.ListaPerfil.Find(perf => perf.PerfilID.Equals(pPerfilID));
            filaPerfil.Eliminado = false;

            if (pRetomarIdentidadEnMetaProyecto)
            {//"PerfilID = '" + pPerfilID + "' AND ProyectoID = '" + pMetaProyectoID + "'"
                Guid identidadID = pGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.FirstOrDefault(ident => ident.PerfilID.Equals(pPerfilID) && ident.ProyectoID.Equals(pMetaProyectoID)).IdentidadID;
                pGestorIdentidades.RetomarIdentidadPerfil(pGestorIdentidades.ListaIdentidades[identidadID]);
            }

            Perfil perfil = pGestorIdentidades.ListaPerfiles[pPerfilID];
            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = (AD.EntityModel.Models.UsuarioDS.Usuario)pGestorUsuarios.ListaUsuarios[pUsuarioID].FilaElementoEntity;

            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            pGestorUsuarios.DataWrapperUsuario.Merge(usuCN.ObtenerUsuarioCompletoPorID(filaUsuario.UsuarioID));

            foreach (OrganizacionParticipaProy filaOrgPartProy in pGestorOrganizaciones.OrganizacionDW.ListaOrganizacionParticipaProy.Where(item => item.OrganizacionID.Equals(perfil.OrganizacionID) && item.RegistroAutomatico > 0))
            {
                //Org del proycto seleccionado
                Guid perfilID = perfil.Clave;
                Guid organizacionID = filaOrgPartProy.OrganizacionProyectoID;
                Guid comunidadID = filaOrgPartProy.ProyectoID;

                ////Si el perfil no tiene identidad
                //if (pGestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + perfilID + "' AND ProyectoID = '" + comunidadID + "'").Length == 0)
                //{
                //Variable que nos indica si este usuario ya pertenece a este proyecto aunque con otra identidad
                AgregarIdentidadPerfilYUsuarioAProyecto(pGestorIdentidades, pGestorUsuarios, organizacionID, comunidadID, filaUsuario, perfil, pRecibirNewsletterDefectoProyectos);

				ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDeSolicitudes>(), mLoggerFactory);
				controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(perfil, filaUsuario, pGestorUsuarios, pGestorIdentidades);

                pGestorIdentidades.RecargarHijos();
                //}
                ////si la tiene es que esta dado de baja
                //else if (pGestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + perfilID + "' AND ProyectoID = '" + comunidadID + "'").Length == 1)
                //{
                //    Guid identidadID = (Guid)pGestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + perfilID + "' AND ProyectoID = '" + comunidadID + "'")[0]["IdentidadID"];

                //    if (pGestorUsuarios.GestorIdentidades == null)
                //    {
                //        pGestorUsuarios.GestorIdentidades = pGestorIdentidades;
                //    }
                //    pGestorUsuarios.RetomarUsuarioEnProyecto(filaUsuario, organizacionID, comunidadID, identidadID);
                //    pGestorIdentidades.RecargarHijos();
                //}
            }
        }

        #endregion

        #region Creación de usuarios


        /// <summary>
        /// Método que acepta una solicitud de usuario
        /// </summary>
        /// <param name="pUsuario">Usuario a aceptar</param>
        /// <param name="pIdSolicitud">ID de la solicitud a aceptar</param>
        /// <param name="pSolicitudDW">DataSet de las solicitudes</param>
        /// <param name="pDataWrapperUsuario">DataSet del usuario</param>
        /// <param name="pProyectoSeleccionado">Proyecto en el que nos estamos registrando</param>
        /// <param name="pUrlIntraGnoss">URLIntraGnoss</param>
        public void AceptarUsuario(Guid pIdSolicitud, DataWrapperSolicitud pSolicitudDW, DataWrapperUsuario pDataWrapperUsuario, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, string pBaseURL, string pUrlIntraGnoss, string pUrlIntraGnossServicios, IAvailableServices pAvailableServices)
        {
            AceptarUsuario(null, pIdSolicitud, pSolicitudDW, pDataWrapperUsuario, null, pProyectoSeleccionado, null, null, null, null, new Dictionary<string, string>(), pBaseURL, null, pUrlIntraGnoss, pUrlIntraGnossServicios, null, null, null, null, pAvailableServices);
        }

        /// <summary>
        /// Método que acepta una solicitud de usuario
        /// </summary>
        /// <param name="pIdentidadActual">IdentidadActual (del usuario que se va a aceptar)</param>
        /// <param name="pUsuario">Usuario a aceptar</param>
        /// <param name="pIdSolicitud">ID de la solicitud a aceptar</param>
        /// <param name="pSolicitudDW">DataSet de las solicitudes</param>
        /// <param name="pDataWrapperUsuario">DataSet del usuario</param>
        /// <param name="pParametroGeneralRow">Fila de parametros generales del proyecto en el que se está aceptando la solicitud</param>
        /// <param name="pProyectoSeleccionado">Proyecto en el que nos estamos registrando</param>
        /// <param name="pAnio">Año de Nacimiento</param>
        /// <param name="pMes">Mes de Nacimiento</param>
        /// <param name="pDia">Dia de Nacimiento</param>
        /// <param name="pInvitacionAEventoComunidad">Invitacion a un evento de comunidad</param>
        /// <param name="pEnviarMensajeBienvenida">Enviar mensaje de bienvenida</param>filaPersona
        /// <param name="pBaseURL">BaseURL</param>
        /// <param name="pInvitacionAComunidad">Invitacion a una comunidad</param>
        /// <param name="pUrlIntraGnoss">URLIntraGnoss</param>
        /// <param name="pEsInvitacionAOrganizacion"></param>
        /// <param name="pInvitacionAOrganizacion"></param>        
        public void AceptarUsuario(Identidad pIdentidadActual, Guid pIdSolicitud, DataWrapperSolicitud pSolicitudDW, DataWrapperUsuario pDataWrapperUsuario, ParametroGeneral pParametroGeneralRow, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, int? pAnio, int? pMes, int? pDia, ProyectoEvento pInvitacionAEventoComunidad, Dictionary<string, string> pParametroProyecto, string pBaseURL, PeticionInvComunidad pInvitacionAComunidad, string pUrlIntraGnoss, string pUrlIntragnossServicios, PeticionInvOrganizacion pInvitacionAOrganizacion, TipoRedSocialLogin? pTipoRedSocial, string pIDEnRedSocial, bool? pEsRegistroPreActivado, IAvailableServices pAvailableServices, bool pRegistroDesdeAdmin = false)
        {
            SolicitudNuevoUsuario filaSU = pSolicitudDW.ListaSolicitudNuevoUsuario.Where(item => item.SolicitudID.Equals(pIdSolicitud)).FirstOrDefault();
            UtilIdiomas utilIdiomas = new UtilIdiomas(filaSU.Idioma, mLoggingService, mEntityContext, mConfigService,mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);

            Solicitud solicitud = pSolicitudDW.ListaSolicitud.Where(item => item.SolicitudID.Equals(pIdSolicitud)).FirstOrDefault();

            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = pDataWrapperUsuario.ListaUsuario.FirstOrDefault();
            filaUsuario.Validado = (short)ValidacionUsuario.Verificado;

            //Persona
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
			Elementos.ServiciosGenerales.Persona persona = gestorPersonas.AgregarPersona();

            AD.EntityModel.Models.PersonaDS.Persona filaPersona = persona.FilaPersona;

            filaPersona.UsuarioID = filaSU.UsuarioID;
            filaPersona.Nombre = filaSU.Nombre;
            filaPersona.Apellidos = filaSU.Apellidos;
            filaPersona.Email = filaSU.Email.ToLower();
            filaPersona.EmailTutor = filaSU.EmailTutor;
            filaPersona.CPPersonal = filaSU.CP;
            filaPersona.EsBuscable = filaSU.EsBuscable;
            filaPersona.EsBuscableExternos = filaSU.EsBuscableExterno;
            filaPersona.EstadoCorreccion = (short)EstadoCorreccion.NoCorreccion;
            filaPersona.Sexo = filaSU.Sexo;
            filaPersona.Idioma = filaSU.Idioma;
            filaPersona.LocalidadPersonal = filaSU.Poblacion;

            if (pParametroGeneralRow != null && pParametroGeneralRow.FechaNacimientoObligatoria && pAnio.HasValue && pMes.HasValue && pDia.HasValue)
            {
                filaPersona.FechaNacimiento = new DateTime(pAnio.Value, pMes.Value, pDia.Value);
            }
            else if (filaSU.FechaNacimiento.HasValue)
            {
                filaPersona.FechaNacimiento = filaSU.FechaNacimiento;
            }

            if (filaSU.PaisID.HasValue)
            {
                filaPersona.PaisPersonalID = filaSU.PaisID;
            }

            if (!filaSU.ProvinciaID.HasValue)
            {
                filaPersona.ProvinciaPersonal = filaSU.Provincia;
            }
            else
            {
                filaPersona.ProvinciaPersonalID = filaSU.ProvinciaID;
            }

            filaPersona.Sexo = filaSU.Sexo;

            filaUsuario.Persona.Add(filaPersona);
            filaUsuario.Persona.Add(filaPersona);
            //EntityContext.Usuario.Add(filaUsuario);
            //EntityContext.SaveChanges();
            mEntityContext.Persona.Add(filaPersona);

            //EntityContext.SaveChanges();
            dataWrapperPersona.ListaPersona.Add(filaPersona);
            //Guardar foto en el servidor y en la BD
            GuardarFoto(filaPersona, filaSU.SolicitudID, mConfigService.ObtenerUrlServicioInterno());

            AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona filaConfigPers = gestorPersonas.AgregarConfiguracionGnossPersona(filaPersona.PersonaID);

            if (filaSU.EsBuscable)
            {
                filaConfigPers.VerRecursos = true;
                filaConfigPers.VerAmigos = true;
                filaConfigPers.VerRecursos = true;
            }

            if (filaSU.EsBuscableExterno)
            {
                filaConfigPers.VerRecursos = true;
                filaConfigPers.VerAmigos = true;
                filaConfigPers.VerRecursos = true;

                filaConfigPers.VerRecursosExterno = true;
                filaConfigPers.VerAmigos = true;
                filaConfigPers.VerRecursosExterno = true;
            }

            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();
            GestionUsuarios gestorUsuarios = new GestionUsuarios(pDataWrapperUsuario, mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            gestorUsuarios.DataWrapperUsuario.Merge(usuarioCN.ObtenerRolesUsuarioPorPerfilYProyecto(filaUsuario.UsuarioID, Guid.Empty, Guid.Empty));
            usuarioCN.Dispose();

            GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, gestorPersonas, gestorUsuarios, new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionIdentidades>(), mLoggerFactory);

            if (!persona.UsuarioCargado)
            {
                persona.GestorPersonas.GestorUsuarios = gestorUsuarios;
                //PersonaDL personaDL = new PersonaDL();
                //personaDL.CargarDatosPesados(persona, TipoCargaElemento.UsuarioPersona, false);
            }

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

            Perfil perfilPersonal = null;
            Identidad objetoIdentidad = null;
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = null;

            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();

            if (PerfilPersonalDisponible)
            {
                perfilPersonal = gestorIdentidades.AgregarPerfilPersonal(filaPersona, true, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, recibirNewsletterDefectoProyectos);

                objetoIdentidad = (Identidad)perfilPersonal.Hijos[0];
                filaIdentidad = objetoIdentidad.FilaIdentidad;

                if (!proyCN.ParticipaUsuarioEnProyecto(ProyectoAD.MetaProyecto, filaUsuario.UsuarioID))
                {
                    gestorUsuarios.AgregarUsuarioAProyecto(filaUsuario, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, filaIdentidad.IdentidadID);
                }

                gestorIdentidades.RecargarHijos();

                //Invalido la cache de Mis comunidades
                proyCL.InvalidarMisProyectos(filaIdentidad.PerfilID);
                proyCL.Dispose();

                Guid organizacionRegistroUsuario = filaSU.Solicitud.OrganizacionID;
                Guid proyectoRegistroUsuario = filaSU.Solicitud.ProyectoID;

                RegistrosAutomaticosEnComunidades(filaSU, perfilPersonal, filaUsuario, gestorUsuarios, gestorIdentidades);

                //Si el proyecto es Didactalia, no actualizamos la home de los usuarios registrados en gnoss y a los que se les asocia didactalia directamente.
                if (!filaSU.Solicitud.ProyectoID.Equals(ProyectoAD.ProyectoDidactalia))
                {
                    AD.EntityModel.Models.IdentidadDS.Identidad identidad = dataWrapperIdentidad.ListaIdentidad.FirstOrDefault(ident => ident.ProyectoID.Equals(ProyectoAD.ProyectoDidactalia));
                    if (identidad != null)
                    {
                        identidad.ActualizaHome = false;
                    }
                }
                else
                {
                    //se registra directamente en didactalia
                    AD.EntityModel.Models.IdentidadDS.Identidad identidad = dataWrapperIdentidad.ListaIdentidad.FirstOrDefault(ident => ident.ProyectoID.Equals(ProyectoAD.ProyectoDidactalia));
                    if (identidad != null)
                    {
                        identidad.ActivoEnComunidad = true;
                    }
                }

                //Invalido la cache de Mis comunidades
                DataWrapperIdentidad idenDW = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory).ObtenerIdentidadPorID(gestorIdentidades.ObtenerIdentidadDeProyecto(ProyectoAD.ProyectoFAQ, filaPersona.PersonaID), true);

                if (idenDW.ListaIdentidad.Count > 0)
                {
                    proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    proyCL.InvalidarMisProyectos(idenDW.ListaIdentidad.First().PerfilID);
                    proyCL.Dispose();
                }
            }
            else
            {
                gestorUsuarios.AgregarProyectoRolUsuario(filaUsuario.UsuarioID, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto);
            }

            if (!filaSU.Solicitud.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !filaSU.Solicitud.ProyectoID.Equals(ProyectoAD.ProyectoFAQ) && !filaSU.Solicitud.ProyectoID.Equals(ProyectoAD.ProyectoNoticias) && !filaSU.Solicitud.ProyectoID.Equals(ProyectoAD.ProyectoDidactalia))
            {
                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                Elementos.ServiciosGenerales.Proyecto proyecto = new GestionProyecto(proyectoCN.ObtenerProyectoPorID(filaSU.Solicitud.ProyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory).ListaProyectos[filaSU.Solicitud.ProyectoID];
                bool registroAbierto = pParametroProyecto.ContainsKey(ParametroAD.RegistroAbierto) && pParametroProyecto[ParametroAD.RegistroAbierto].Equals("1");

                if (proyecto.TipoAcceso == TipoAcceso.Publico || pInvitacionAComunidad != null || registroAbierto || (pEsRegistroPreActivado.HasValue && pEsRegistroPreActivado.Value) || pRegistroDesdeAdmin)
                {
                    bool registroProyecto = true;
                    GestionOrganizaciones gestOrg = null;
                    if (pInvitacionAOrganizacion != null)
                    {
                        gestOrg = new GestionOrganizaciones(new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory).ObtenerOrganizacionPorID(pInvitacionAOrganizacion.FilaInvitacionOrganizacion.OrganizacionID), mLoggingService, mEntityContext);
                        if (gestOrg != null && gestOrg.OrganizacionDW.ListaOrganizacionParticipaProy.Any(item => item.ProyectoID.Equals(proyecto.Clave)))
                        {
                            registroProyecto = false;
                        }
                    }

                    if (registroProyecto && PerfilPersonalDisponible)
                    {
                        Guid organizacionID = filaSU.Solicitud.OrganizacionID;
                        Guid proyectoID = filaSU.Solicitud.ProyectoID;

                        //si el usuario aún no participa en el proyecto se agrega
                        if (!proyCN.ParticipaUsuarioEnProyecto(proyectoID, filaUsuario.UsuarioID) && !gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(ident => ident.ProyectoID.Equals(proyectoID)))
                        {
                            Identidad ObjetoIdentidadProy = AgregarIdentidadPerfilYUsuarioAProyecto(gestorIdentidades, gestorUsuarios, organizacionID, proyectoID, filaUsuario, perfilPersonal, recibirNewsletterDefectoProyectos);

							ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDeSolicitudes>(), mLoggerFactory);
							controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(perfilPersonal, filaUsuario, gestorUsuarios, gestorIdentidades, proyectoID);

							//Invalido la cache de Mis comunidades
							proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                            proyCL.InvalidarMisProyectos(ObjetoIdentidadProy.PerfilID);
                            proyCL.Dispose();

                            gestorIdentidades.RecargarHijos();
                            //listaProyectos.Add(proyectoID);
                        }
                    }
                }

                //Limpiar la caché de los miembros de la comunidad para el servicio web autocompletar de editores...
                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                identidadCL.InvalidarCacheMiembrosComunidad(filaSU.Solicitud.ProyectoID);
                identidadCL.Dispose();
            }

            #region Agrego cláusulas adicionales

            bool clausulasProyectoActual = false;
            if (pParametroGeneralRow != null)
            {
                clausulasProyectoActual = pParametroGeneralRow.ClausulasRegistro;
            }
            bool clausulasMetaProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<ControladorProyecto>(),mloggerFactory).ObtenerFilaParametrosGeneralesDeProyecto(ProyectoAD.MetaProyecto).ClausulasRegistro;

            if (clausulasProyectoActual || clausulasMetaProyecto)
            {
                gestorUsuarios.DataWrapperUsuario.Merge(new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory).ObtenerClausulasRegitroProyecto(filaSU.Solicitud.ProyectoID));
                gestorUsuarios.DataWrapperUsuario.Merge(new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory).ObtenerClausulasRegitroProyecto(ProyectoAD.MetaProyecto));

                List<Guid> clausulasTrue = new List<Guid>();

                if (!string.IsNullOrEmpty(filaSU.ClausulasAdicionales))
                {
                    string[] clausulasTexto = filaSU.ClausulasAdicionales.Split(',');

                    foreach (string clausula in clausulasTexto)
                    {
                        if (!string.IsNullOrEmpty(clausula))
                        {
                            clausulasTrue.Add(new Guid(clausula));
                        }
                    }
                }

                usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                DataWrapperUsuario usuDW = usuarioCN.ObtenerClausulasRegitroPorID(clausulasTrue);
                usuarioCN.Dispose();

                Dictionary<KeyValuePair<Guid, Guid>, List<Guid>> listaProyectoClausulas = new Dictionary<KeyValuePair<Guid, Guid>, List<Guid>>();
                foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in usuDW.ListaClausulaRegistro)
                {
                    KeyValuePair<Guid, Guid> orgProy = new KeyValuePair<Guid, Guid>(filaClausula.OrganizacionID, filaClausula.ProyectoID);
                    if (listaProyectoClausulas.ContainsKey(orgProy))
                    {
                        listaProyectoClausulas[orgProy].Add(filaClausula.ClausulaID);
                    }
                    else
                    {
                        List<Guid> nuevaLista = new List<Guid>();
                        nuevaLista.Add(filaClausula.ClausulaID);
                        listaProyectoClausulas.Add(orgProy, nuevaLista);
                    }
                }

                foreach (KeyValuePair<Guid, Guid> proyectoClausulaID in listaProyectoClausulas.Keys)
                {
                    gestorUsuarios.AgregarClausulasAdicionalesRegistroProy(filaUsuario.UsuarioID, proyectoClausulaID.Key, proyectoClausulaID.Value, listaProyectoClausulas[proyectoClausulaID]);
                }
            }

            #endregion

            gestorPersonas.CrearDatosTrabajoPersonaLibre(persona);

            gestorUsuarios.GestorTesauro = new GestionTesauro(new DataWrapperTesauro(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
            gestorUsuarios.GestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
            gestorUsuarios.CompletarUsuarioNuevo(filaUsuario, utilIdiomas.GetText("TESAURO", "RECURSOSPUBLICOS"), utilIdiomas.GetText("TESAURO", "RECURSOSPRIVADOS"));

            if (PerfilPersonalDisponible)
            {
                GuardarDatosExtraSolicitud(dataWrapperIdentidad, filaIdentidad.PerfilID, pIdSolicitud, pSolicitudDW);
            }

            DataWrapperSuscripcion suscripcionDW = null;
            if (gestorIdentidades.GestorSuscripciones != null && gestorIdentidades.GestorSuscripciones.SuscripcionDW.ListaSuscripcion.Count > 0)
            {
                //lo hago así porque este dataset puede no rellenarse en ControladorIdentidades.CrearPerfilPersonaOrganizacion
                suscripcionDW = gestorIdentidades.GestorSuscripciones.SuscripcionDW;
            }

            mEntityContext.SaveChanges();

            NotificarEdicionPerfilEnProyectos(TipoAccionExterna.Registro, persona.Clave, string.Empty, string.Empty);

            if (PerfilPersonalDisponible)
            {
                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                Identidad identidadnuevo = new GestionIdentidades(idenCN.ObtenerIdentidadPorID(filaIdentidad.IdentidadID, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication).ListaIdentidades[filaIdentidad.IdentidadID];
                PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                identidadnuevo.GestorIdentidades.GestorPersonas = new GestionPersonas(persCN.ObtenerPersonaPorID(identidadnuevo.PersonaID.Value), mLoggingService, mEntityContext);

                identidadnuevo.GestorIdentidades.GestorPersonas.CargarGestor();

                //Actualizo el modelo base:
                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorPersonas>(), mLoggerFactory);
                foreach (Identidad iden in gestorIdentidades.ListaIdentidades.Values)
                {
                    //controladorPersonas.ActualizarModeloBaseSimple(filaPersona.PersonaID, iden.FilaIdentidad.ProyectoID, PrioridadBase.Alta);
                    controladorPersonas.ActualizarModeloBaseSimple(iden, iden.FilaIdentidad.ProyectoID, UrlIntragnoss);
                }
            }

            #region Actualizar cola GnossLIVE

            List<string> filasAInsertar = new List<string>();
            foreach (Identidad iden in gestorIdentidades.ListaIdentidades.Values)
            {
                //No se notifica al LIVE los proyectos a los que se le ha hecho miembro automáticamente
                if (((!iden.FilaIdentidad.ProyectoID.Equals(ProyectoAD.ProyectoNoticias)) &&
                    (!iden.FilaIdentidad.ProyectoID.Equals(ProyectoAD.ProyectoFAQ)) &&
                    (!iden.FilaIdentidad.ProyectoID.Equals(ProyectoAD.ProyectoDidactalia))) ||
                    (iden.FilaIdentidad.ProyectoID.Equals(filaSU.Solicitud.ProyectoID)))
                {
                    ParametroAplicacion busqueda = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("EcosistemaSinHomeUsuario"));
                    if (!(busqueda != null && busqueda.Valor == "true"))
                    {
                        if (iden.FilaIdentidad.ProyectoID.Equals(ProyectoAD.ProyectoDidactalia))
                        {
                            filasAInsertar.Add(PreprarFilaParaColaRabbitMQ(iden.FilaIdentidad.ProyectoID, iden.FilaIdentidad.PerfilID, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta, "didactalia"));
                        }
                        else
                        {
                            filasAInsertar.Add(PreprarFilaParaColaRabbitMQ(iden.FilaIdentidad.ProyectoID, iden.FilaIdentidad.PerfilID, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta));
                        }
                    }
                }
            }

            if (filasAInsertar.Count > 0)
            {
                InsertarFilasEnColaRabbitMQ(filasAInsertar);
            }

            #endregion

            if (!filaSU.Solicitud.ProyectoID.Equals(ProyectoAD.MetaProyecto) && pInvitacionAComunidad == null && PerfilPersonalDisponible)
            {
                ProyectoCN proyectoCN2 = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                Elementos.ServiciosGenerales.Proyecto proyecto2 = new GestionProyecto(proyectoCN2.ObtenerProyectoPorID(filaSU.Solicitud.ProyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory).ListaProyectos[filaSU.Solicitud.ProyectoID];
                if (proyecto2.TipoAcceso == TipoAcceso.Restringido)
                {
                    SolicitudCN solCN2 = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SolicitudCN>(), mLoggerFactory);
                    DataWrapperSolicitud solDW2 = new DataWrapperSolicitud();

                    Solicitud filaSolicitud = new Solicitud();
                    filaSolicitud.SolicitudID = Guid.NewGuid();
                    filaSolicitud.Estado = (short)EstadoSolicitud.Espera;
                    filaSolicitud.FechaSolicitud = DateTime.Now;
                    filaSolicitud.OrganizacionID = filaSU.Solicitud.OrganizacionID;
                    filaSolicitud.ProyectoID = filaSU.Solicitud.ProyectoID;

                    mEntityContext.Solicitud.Add(filaSolicitud);
                    solDW2.ListaSolicitud.Add(filaSolicitud);

                    SolicitudUsuario filaSolUsu = new SolicitudUsuario();
                    filaSolUsu.Solicitud = filaSolicitud;
                    filaSolUsu.PersonaID = objetoIdentidad.PerfilUsuario.PersonaID.Value;
                    filaSolUsu.UsuarioID = filaUsuario.UsuarioID;
                    filaSolUsu.PerfilID = objetoIdentidad.PerfilID;

                    if (!string.IsNullOrEmpty(filaSU.ClausulasAdicionales))
                    {
                        filaSolUsu.ClausulasAdicionales = filaSU.ClausulasAdicionales;
                    }

                    solDW2.ListaSolicitudUsuario.Add(filaSolUsu);
                    mEntityContext.SolicitudUsuario.Add(filaSolUsu);

                    GestionNotificaciones gestorNotificaciones2 = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<GestionNotificaciones>(), mloggerFactory);
                    gestorNotificaciones2.AgregarNotificacionSolicitud(filaSolicitud.SolicitudID, DateTime.Now, pProyectoSeleccionado.Nombre, "admin", "", TiposNotificacion.SolicitudPendiente, persona.FilaPersona.Email, persona, pProyectoSeleccionado.FilaProyecto.OrganizacionID, pProyectoSeleccionado.Clave, UtilIdiomas.LanguageCode);

                    mEntityContext.SaveChanges();
                    solCN2.Dispose();
                }
            }

            if (PerfilPersonalDisponible)
            {
                LiveUsuariosCL liveUsuariosCL = new LiveUsuariosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<LiveUsuariosCL>(), mLoggerFactory);
                liveUsuariosCL.ClonarLiveProyectoAHomeUsu(filaSU.UsuarioID, perfilPersonal.Clave, pProyectoSeleccionado.Clave);
            }

            //Iniciamos sesión automáticamente
            if (!pRegistroDesdeAdmin)
            {
                if (!string.IsNullOrEmpty(filaPersona.Email) || ComprobarPersonaEsMayorAnios(filaPersona.FechaNacimiento.Value, 14))
                {
                    GnossIdentity identity = new UtilUsuario(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilUsuario>(), mLoggerFactory).ValidarUsuario(filaUsuario.Login, filaSU.Solicitud.OrganizacionID, filaSU.Solicitud.ProyectoID);
                    if (identity != null)
                    {
                        mHttpContextAccessor.HttpContext.Session.Clear();
                        //TODO Javier Juan migrar cuando lleguemos a la web
                        mHttpContextAccessor.HttpContext.Session.Set("Usuario", identity);
                        mHttpContextAccessor.HttpContext.Session.Set("MantenerConectado", true);
                        mHttpContextAccessor.HttpContext.Session.Set("CrearCookieEnServicioLogin", true);
                        mHttpContextAccessor.HttpContext.Session.Remove("EnvioCookie");
                        AgregarObjetoAPeticionActual("GnossIdentity", identity);
                    }
                }
            }

            AceptarExtrasInvitacionAComunidad(persona, pInvitacionAComunidad, pProyectoSeleccionado, pUrlIntraGnoss, pInvitacionAEventoComunidad, UtilIdiomas.LanguageCode, pAvailableServices);
        }

        /// <summary>
        /// Comprobar si es mayor de X años
        /// </summary>
        /// <param name="fecha">Fecha de nacimineto.</param>
        /// <param name="edad">Edad limite.</param>
        public static bool ComprobarPersonaEsMayorAnios(DateTime fecha, int edad)
        {
            DateTime hoy = DateTime.Now;
            int anios = hoy.Year - fecha.Year;
            int meses = hoy.Month - fecha.Month;

            if (meses < 0 || (meses == 0 && hoy.Day < fecha.Day))
            {
                anios--;
            }

            bool esMayor = (anios >= edad);

            return esMayor;
        }

        /// <summary>
        /// Agregamos los campos extra de la solicitud a la identidadDS.
        /// </summary>
        /// <param name="pIidentidadDS">DS de identidades donde agregamos los nuevos campos.</param>
        /// <param name="pIdSolicitud">GUID de la solicitud de usuario</param>
        /// <param name="pSolicitudDW">DS con los datos de la solicitud</param>
        private void GuardarDatosExtraSolicitud(DataWrapperIdentidad pDataWrapperIdentidad, Guid pPerfilID, Guid pIdSolicitud, DataWrapperSolicitud pSolicitudDW)
        {
            //Recorrer las tablas datoextrasolicitudproyectovirtuoso, datoextrasolicitudecosistemavirtuoso, datoextrasolicitudproyecto, datoextrasolicitudecosistema y para la solicitudID agregar los campos para el perfilID.
            foreach (DatoExtraProyectoOpcionSolicitud deevs in pSolicitudDW.ListaDatoExtraProyectoOpcionSolicitud.Where(item => item.SolicitudID.Equals(pIdSolicitud)).ToList())
            {
                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = pDataWrapperIdentidad.ListaIdentidad.FirstOrDefault(ident => ident.PerfilID.Equals(pPerfilID) && ident.ProyectoID.Equals(deevs.ProyectoID));

                DatoExtraProyectoOpcionIdentidad datoExtraProyectoOpcionIdentidad = new DatoExtraProyectoOpcionIdentidad();
                datoExtraProyectoOpcionIdentidad.OrganizacionID = deevs.OrganizacionID;
                datoExtraProyectoOpcionIdentidad.ProyectoID = deevs.ProyectoID;
                datoExtraProyectoOpcionIdentidad.DatoExtraID = deevs.DatoExtraID;
                datoExtraProyectoOpcionIdentidad.OpcionID = deevs.OpcionID;
                datoExtraProyectoOpcionIdentidad.IdentidadID = filaIdentidad.IdentidadID;
                pDataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad.Add(datoExtraProyectoOpcionIdentidad);
                mEntityContext.DatoExtraProyectoOpcionIdentidad.Add(datoExtraProyectoOpcionIdentidad);
            }

            foreach (DatoExtraEcosistemaOpcionSolicitud deevs in pSolicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud.Where(item => item.SolicitudID.Equals(pIdSolicitud)).ToList())
            {
                AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil datoExtraEcosistemaOpcionPerfil = new AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaOpcionPerfil();
                datoExtraEcosistemaOpcionPerfil.DatoExtraID = deevs.DatoExtraID;
                datoExtraEcosistemaOpcionPerfil.OpcionID = deevs.OpcionID;
                datoExtraEcosistemaOpcionPerfil.PerfilID = pPerfilID;
                pDataWrapperIdentidad.ListaDatoExtraEcosistemaOpcionPerfil.Add(datoExtraEcosistemaOpcionPerfil);
                mEntityContext.DatoExtraEcosistemaOpcionPerfil.Add(datoExtraEcosistemaOpcionPerfil);
            }

            foreach (DatoExtraProyectoVirtuosoSolicitud deevs in pSolicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud.Where(item => item.SolicitudID.Equals(pIdSolicitud)).ToList())
            {
                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = pDataWrapperIdentidad.ListaIdentidad.FirstOrDefault(ident => ident.PerfilID.Equals(pPerfilID) && ident.ProyectoID.Equals(deevs.ProyectoID));

                AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad datoExtraProyectoVirtuosoIdentidad = new AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad();
                datoExtraProyectoVirtuosoIdentidad.OrganizacionID = deevs.OrganizacionID;
                datoExtraProyectoVirtuosoIdentidad.ProyectoID = deevs.ProyectoID;
                datoExtraProyectoVirtuosoIdentidad.DatoExtraID = deevs.DatoExtraID;
                datoExtraProyectoVirtuosoIdentidad.Opcion = deevs.Opcion;
                datoExtraProyectoVirtuosoIdentidad.IdentidadID = filaIdentidad.IdentidadID;
                datoExtraProyectoVirtuosoIdentidad.Identidad = filaIdentidad;
                filaIdentidad.DatoExtraProyectoVirtuosoIdentidad.Add(datoExtraProyectoVirtuosoIdentidad);
                pDataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad.Add(datoExtraProyectoVirtuosoIdentidad);
                mEntityContext.DatoExtraProyectoVirtuosoIdentidad.Add(datoExtraProyectoVirtuosoIdentidad);
            }

            foreach (DatoExtraEcosistemaVirtuosoSolicitud deevs in pSolicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud.Where(item => item.SolicitudID.Equals(pIdSolicitud)).ToList())
            {
                AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil datoExtraEcosistemaVirtuosoPerfil = new AD.EntityModel.Models.IdentidadDS.DatoExtraEcosistemaVirtuosoPerfil();
                datoExtraEcosistemaVirtuosoPerfil.DatoExtraID = deevs.DatoExtraID;
                datoExtraEcosistemaVirtuosoPerfil.PerfilID = pPerfilID;
                datoExtraEcosistemaVirtuosoPerfil.Opcion = deevs.Opcion;
                pDataWrapperIdentidad.ListaDatoExtraEcosistemaVirtuosoPerfil.Add(datoExtraEcosistemaVirtuosoPerfil);
            }
        }

        /// <summary>
        /// Guarda la foto del perfil
        /// </summary>
        /// <param name="pFila">Fila de la persona o de la solicitud</param>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        private void GuardarFoto(AD.EntityModel.Models.PersonaDS.Persona pFila, Guid pSolicitudID, string pUrlIntragnossServicios)
        {
            try
            {
                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
                //string url = pUrlIntragnossServicios.Replace("https://", "http://");
                servicioImagenes.Url = pUrlIntragnossServicios;
                byte[] resultado = servicioImagenes.ObtenerImagen(UtilArchivos.ContentImagenesSolicitudes + "/" + pSolicitudID.ToString(), ".png");

                if (resultado != null)
                {
                    if (pFila is AD.EntityModel.Models.PersonaDS.Persona)
                    {
                        pFila.Foto = resultado;
                    }
                    servicioImagenes.AgregarImagen(resultado, UtilArchivos.ContentImagenesPersonas + "/" + pFila.PersonaID.ToString(), ".png");
                    servicioImagenes.BorrarImagen(UtilArchivos.ContentImagenesSolicitudes + "/" + pSolicitudID.ToString() + ".png");
                    Image image = UtilImages.ConvertirArrayBytesEnImagen(resultado);

                    //Ajusto su tamaño
                    SizeF tamanioProporcional = UtilImages.CalcularTamanioProporcionado(image, 54, 54);
                    image = UtilImages.AjustarImagen(image, tamanioProporcional.Width, tamanioProporcional.Height);

                    //Guardo la imagen en un archivo temporal
                    MemoryStream ms = new MemoryStream();
                    image.SaveAsPng(ms);

                    servicioImagenes.AgregarImagen(ms.ToArray(), UtilArchivos.ContentImagenesPersonas + "/" + pFila.PersonaID.ToString() + "_peque", ".png");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void AceptarExtrasInvitacionAComunidad(Elementos.ServiciosGenerales.Persona pPersona, PeticionInvComunidad pInvitacionAComunidad, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, string pUrlIntragnoss, ProyectoEvento pInvitacionAEventoComunidad, string pLanguageCode, IAvailableServices pAvailableServices)
        {
            if (pInvitacionAComunidad != null)
            {
                //si es una invitacion a grupo aceptamos el grupo
                if (pInvitacionAComunidad.GestorPeticiones.PeticionDW.ListaPeticionInvitacionGrupo.Count == 1)
                {
                    AD.EntityModel.Models.Peticion.PeticionInvitacionGrupo filaPeticionGrupo = (pInvitacionAComunidad.GestorPeticiones.PeticionDW.ListaPeticionInvitacionGrupo.FirstOrDefault());
                    string[] grupos = filaPeticionGrupo.GruposID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<Guid> listaGrupos = new List<Guid>();

                    foreach (string grupo in grupos)
                    {
                        Guid grupoID = new Guid(grupo);
                        if (!listaGrupos.Contains(grupoID))
                        {
                            listaGrupos.Add(grupoID);
                        }
                    }
                    DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();
                    IdentidadCN identidadCN2 = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    GestionIdentidades gestorIden = new GestionIdentidades(identidadCN2.ObtenerIdentidadDePersonaEnProyecto(filaPeticionGrupo.ProyectoID, pPersona.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    List<Guid> listaGruposPertenece = identidadCN2.ObtenerIDGruposDeIdentidad(gestorIden.ListaIdentidades.Keys[0]);

                    List<Guid> listaGruposEsxisten = identidadCN2.ComprobarSiIDGruposExisten(listaGrupos);

                    string tripletasVirtuoso = "";

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                    LiveUsuariosCL liveUsuariosCL = new LiveUsuariosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<LiveUsuariosCL>(), mLoggerFactory);

                    foreach (Guid grupoID in listaGrupos)
                    {
                        string nombrecortoGrupo = identidadCN.ObtenerNombreCortoGrupoPorID(grupoID);
                        if (!listaGruposPertenece.Contains(grupoID) && listaGruposEsxisten.Contains(grupoID))
                        {
                            AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();

                            filaGrupoIdentidadesParticipacion.GrupoID = grupoID;
                            filaGrupoIdentidadesParticipacion.IdentidadID = gestorIden.ListaIdentidades.Keys[0];
                            filaGrupoIdentidadesParticipacion.FechaAlta = DateTime.Now;

                            dataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);

                            //Asignar la caché del grupo al usuario
                            liveUsuariosCL.ClonarLiveGrupoProyectoAUsu(pPersona.UsuarioID, filaPeticionGrupo.ProyectoID, grupoID);
                            identidadCL.InvalidarCacheGrupoPorNombreCortoYProyecto(nombrecortoGrupo, filaPeticionGrupo.ProyectoID);

                            tripletasVirtuoso = FacetadoAD.GenerarTripleta("<http://gnoss/" + grupoID.ToString().ToUpper() + ">", "<http://gnoss/hasparticipanteID>", "<http://gnoss/" + filaGrupoIdentidadesParticipacion.IdentidadID.ToString().ToUpper() + ">");
                            tripletasVirtuoso += FacetadoAD.GenerarTripleta("<http://gnoss/" + grupoID.ToString().ToUpper() + ">", "<http://gnoss/hasparticipante>", "\"" + gestorIden.ListaIdentidades.Values[0].NombreCompuesto().ToLower() + "\"");
                        }
                    }

                    liveUsuariosCL.Dispose();

                    identidadCN2.ActualizaIdentidades();
                    identidadCN2.Dispose();

                    if (!string.IsNullOrEmpty(tripletasVirtuoso))
                    {
                        FacetadoCN facetadoCN2 = new FacetadoCN(pUrlIntragnoss, false, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                        facetadoCN2.InsertaTripletas(filaPeticionGrupo.ProyectoID.ToString(), tripletasVirtuoso, (short)PrioridadLive.Alta, true);
                        facetadoCN2.Dispose();
                    }

                    identidadCL.InvalidarCacheQueContengaCadena("Grupo_*_" + filaPeticionGrupo.ProyectoID);
                    identidadCN.Dispose();
                    identidadCL.Dispose();
                }

                pInvitacionAComunidad.FilaPeticion.Estado = (short)EstadoPeticion.Aceptada;
                pInvitacionAComunidad.FilaPeticion.FechaProcesado = DateTime.Now;
                pInvitacionAComunidad.FilaPeticion.UsuarioID = pPersona.UsuarioID;
                PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PeticionCN>(), mLoggerFactory);
                peticionCN.ActualizarBD();
                peticionCN.Dispose();
            }
            else if (pInvitacionAEventoComunidad != null)
            {
                AgregarParticipanteEvento(pPersona, pProyectoSeleccionado, pInvitacionAEventoComunidad, pUrlIntragnoss, pLanguageCode, pAvailableServices);
            }
        }

        public void AgregarParticipanteEvento(Elementos.ServiciosGenerales.Persona pPersona, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, ProyectoEvento pInvitacionAEventoComunidad, string pUrlIntragnoss, string pLanguageCode, IAvailableServices pAvailableServices)
        {
            try
            {
                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                GestionIdentidades gestorIden = new GestionIdentidades(idenCN.ObtenerIdentidadDePersonaEnProyecto(pProyectoSeleccionado.Clave, pPersona.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                idenCN.Dispose();

                DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
                if (gestorIden.ListaIdentidades.Count > 0)
                {
                    Guid identidadID = gestorIden.ListaIdentidades.FirstOrDefault().Key;
                    ProyectoEventoParticipante proyectoEventoParticipante = new ProyectoEventoParticipante();
                    proyectoEventoParticipante.ProyectoEvento = pInvitacionAEventoComunidad;
                    proyectoEventoParticipante.IdentidadID = identidadID;
                    proyectoEventoParticipante.Fecha = DateTime.Now;
                    dataWrapperProyecto.ListaProyectoEventoParticipante.Add(proyectoEventoParticipante);
                    mEntityContext.ProyectoEventoParticipante.Add(proyectoEventoParticipante);
                }

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                proyCN.ActualizarProyectos();
                proyCN.Dispose();

                if (!string.IsNullOrEmpty(pInvitacionAEventoComunidad.Descripcion))
                {
                    GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(),mloggerFactory);
                    gestorNotificaciones.AgregarNotificacionConfirmacionEventoComunidad(gestorIden.ListaIdentidades.Values[0].Nombre(), pInvitacionAEventoComunidad.Nombre, pInvitacionAEventoComunidad.Descripcion, pPersona.Clave, pProyectoSeleccionado.FilaProyecto.OrganizacionID, pProyectoSeleccionado.FilaProyecto.ProyectoID, pProyectoSeleccionado.Nombre, gestorIden.ListaIdentidades.Values[0].Email, pLanguageCode);
                    NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                    notificacionCN.ActualizarNotificacion(pAvailableServices);
                    notificacionCN.Dispose();
                }

                if (!string.IsNullOrEmpty(pInvitacionAEventoComunidad.Grupo))
                {
                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();

                    Guid grupoID = identidadCN.ObtenerGrupoIDPorNombreCorto(pInvitacionAEventoComunidad.Grupo);

                    AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion filaGrupoIdentidadesParticipacion = new AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion();

                    filaGrupoIdentidadesParticipacion.GrupoID = grupoID;
                    filaGrupoIdentidadesParticipacion.IdentidadID = gestorIden.ListaIdentidades.Keys[0];
                    filaGrupoIdentidadesParticipacion.FechaAlta = DateTime.Now;

                    identidadDW.ListaGrupoIdentidadesParticipacion.Add(filaGrupoIdentidadesParticipacion);

                    identidadCN.ActualizaIdentidades();
                    identidadCN.Dispose();

                    string tripletasVirtuoso = "";
                    tripletasVirtuoso = FacetadoAD.GenerarTripleta("<http://gnoss/" + grupoID.ToString().ToUpper() + ">", "<http://gnoss/hasparticipanteID>", "<http://gnoss/" + gestorIden.ListaIdentidades.Keys[0].ToString().ToUpper() + ">");
                    tripletasVirtuoso += FacetadoAD.GenerarTripleta("<http://gnoss/" + grupoID.ToString().ToUpper() + ">", "<http://gnoss/hasparticipante>", "\"" + gestorIden.ListaIdentidades.Values[0].NombreCompuesto().ToLower() + "\"");

                    if (!string.IsNullOrEmpty(tripletasVirtuoso))
                    {
                        FacetadoCN facetadoCN2 = new FacetadoCN(pUrlIntragnoss, false, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                        facetadoCN2.InsertaTripletas(pProyectoSeleccionado.Clave.ToString(), tripletasVirtuoso, (short)PrioridadLive.Alta, true);
                        facetadoCN2.Dispose();
                    }

                    IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                    identidadCL.InvalidarCacheQueContengaCadena("Grupo_*_" + pProyectoSeleccionado.Clave);
                    identidadCN.Dispose();
                    identidadCL.Dispose();
                }
            }
            catch
            {
                //Ya esta registrado en el evento, controlamos el error y seguimos
            }
        }

        #endregion

        #region Acciones con servicios Externos

        public JsonEstado AccionEnServicioExternoEcosistema(TipoAccionExterna pTipoAccionExterna, Guid pProyectoID, Guid pUsuarioID, string pNombre, string pApellidos, string pEmail, string pPass, GestorParametroAplicacion pParametroAplicacionDS, DataWrapperProyecto pDatosExtraProyectoDataWrapperProyecto, Dictionary<int, string> pDicDatosExtraEcosistemaVirtuoso, Dictionary<int, string> pDicDatosExtraProyectoVirtuoso, string pDatoAuxiliar, string pIdioma = "")
        {
            if (!pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                if (pParametroAplicacionDS.ListaAccionesExternas != null && pParametroAplicacionDS.ListaAccionesExternas.Where(accion => accion.TipoAccion.Equals((short)pTipoAccionExterna)).ToList().Count == 1)
                {
                    List<JsonDatosExtraUsuario> datosExtraUsuario = new List<JsonDatosExtraUsuario>();

                    if (pDicDatosExtraEcosistemaVirtuoso != null)
                    {
                        foreach (int orden in pDicDatosExtraEcosistemaVirtuoso.Keys)
                        {
                            if (!string.IsNullOrEmpty(pDicDatosExtraEcosistemaVirtuoso[orden].Trim()) && pDicDatosExtraEcosistemaVirtuoso[orden].Trim() != "|")
                            {
                                //EcosistemaVirtuoso
                                DatoExtraEcosistemaVirtuoso depvr = pDatosExtraProyectoDataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso.Where(dato => dato.Orden.Equals(orden)).First();

                                string valor = pDicDatosExtraEcosistemaVirtuoso[orden];
                                if (valor.EndsWith("|"))
                                {
                                    valor = valor.Substring(0, valor.Length - 1);
                                }

                                JsonDatosExtraUsuario datoExtraUsuario = new JsonDatosExtraUsuario();
                                datoExtraUsuario.name = depvr.Titulo;
                                datoExtraUsuario.name_id = depvr.DatoExtraID;
                                datoExtraUsuario.value = valor;
                                datosExtraUsuario.Add(datoExtraUsuario);
                            }
                        }
                    }

                    if (pDicDatosExtraProyectoVirtuoso != null)
                    {
                        foreach (int orden in pDicDatosExtraProyectoVirtuoso.Keys)
                        {
                            if (!string.IsNullOrEmpty(pDicDatosExtraProyectoVirtuoso[orden].Trim()) && pDicDatosExtraProyectoVirtuoso[orden].Trim() != "|")
                            {
                                //ProyectoVirtuoso
                                DatoExtraProyectoVirtuoso depvr = pDatosExtraProyectoDataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.FirstOrDefault(dato => dato.Orden.Equals(orden));

                                string valor = pDicDatosExtraProyectoVirtuoso[orden];
                                if (valor.EndsWith("|"))
                                {
                                    valor = valor.Substring(0, valor.Length - 1);
                                }

                                JsonDatosExtraUsuario datoExtraUsuario = new JsonDatosExtraUsuario();
                                datoExtraUsuario.name = depvr.Titulo;
                                datoExtraUsuario.name_id = depvr.DatoExtraID;
                                datoExtraUsuario.value = valor;
                                datosExtraUsuario.Add(datoExtraUsuario);
                            }
                        }
                    }
                    AccionesExternas filaAccionesExrternas = pParametroAplicacionDS.ListaAccionesExternas.Where(acciones => acciones.TipoAccion.Equals((short)pTipoAccionExterna)).ToList()[0];

                    JsonUsuario usuario = new JsonUsuario();
                    usuario.name = pNombre;
                    usuario.last_name = pApellidos;
                    usuario.email = pEmail;
                    usuario.password = pPass;
                    usuario.community_id = pProyectoID;
                    usuario.user_id = pUsuarioID;
                    usuario.extra_data = datosExtraUsuario;
                    usuario.aux_data = pDatoAuxiliar;
                    usuario.languaje = pIdioma;

                    string url = filaAccionesExrternas.URL;//POST
                    string respuesta = string.Empty;
                    try
                    {
                        respuesta = new UtilWeb(mHttpContextAccessor).WebRequest(UtilWeb.Metodo.POST, url, JsonConvert.SerializeObject(usuario), "application/json");//POST
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Error en servicio externo: " + url,mlogger);
                    }

                    JsonEstado jsonEstado = new JsonEstado();
                    jsonEstado.Correcto = false;
                    jsonEstado.InfoExtra = "";

                    if (!string.IsNullOrEmpty(respuesta))
                    {
                        jsonEstado = JsonConvert.DeserializeObject<JsonEstado>(respuesta);
                    }

                    //log
                    if (!jsonEstado.Correcto)
                    {
                        Exception ex = new Exception(jsonEstado.InfoExtra);
                        mLoggingService.GuardarLogError(ex, "ERROR AccionEnServicioExterno. Respuesta servicio externo : " + respuesta, mlogger);
                    }

                    return jsonEstado;
                }
            }
            return null;
        }


        public JsonEstado AccionEnServicioExternoProyecto(TipoAccionExterna pTipoAccionExterna, Elementos.ServiciosGenerales.Persona pPersona, Guid pProyectoID, Guid pIdentidadID, string pPass, string pDatoAuxiliar, DateTime? pFechaRegistroComunidad, DataWrapperProyecto pProyectoDS)
        {
            return AccionEnServicioExternoProyecto(pTipoAccionExterna, pProyectoID, pIdentidadID, pPersona.UsuarioID, pPersona.Nombre, pPersona.Apellidos, pPersona.Mail, pPass, pDatoAuxiliar, pPersona.Fecha, pPersona.PaisID, pPersona.Localidad, pPersona.Sexo, pFechaRegistroComunidad, pProyectoDS, pPersona.ProvinciaID, pPersona.Provincia, pPersona.CodPostal, pPersona.ValorDocumentoAcreditativo, pPersona.FilaPersona.Idioma);
        }


        public JsonEstado AccionEnServicioExternoProyecto(TipoAccionExterna pTipoAccionExterna, Guid pProyectoID, Guid pIdentidadID, Guid pUsuarioID, string pNombre, string pApellidos, string pEmail, string pPass, string pDatoAuxiliar, DateTime? pFechaNacimiento, Guid? pPaisID, string pLocalidad, string pSexo, DateTime? pFechaRegistroComunidad, DataWrapperProyecto pdataWrapperProyecto, Guid pProvinciaID, string pProvincia, string pCodigoPostal, string pDNI = null, string pIdioma = null)
        {
            if (!pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                //cuando se llame a este método en un bucle, proyectoDS deberá estar cargado con los datos de todos los proyectos del usuario 
                //para no cargar n veces el DS
                //cuando se llama para un solo proyecto, proyectoDS puede mandarse null y que se cargue porque sólo lo hará una vez
                if (pdataWrapperProyecto == null)
                {
                    //Compruebo que hay algún proyecto con acciones externas 
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    List<Guid> proyectosConAccionesExternas = proyectoCL.ObtenerListaIDsProyectosConAccionesExternas();
                    proyectoCL.Dispose();

                    if (proyectosConAccionesExternas != null && proyectosConAccionesExternas.Contains(pProyectoID))
                    {
                        //Cargamos todos los datos del proyecto
                        ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                        pdataWrapperProyecto = proyectoCN.ObtenerDatosExtraProyectoPorID(pProyectoID);
                        pdataWrapperProyecto.Merge(proyectoCN.ObtenerAccionesExternasProyectoPorProyectoID(pProyectoID));
                        proyectoCN.Dispose();
                    }
                }
                //("ProyectoID='" + pProyectoID + "' And TipoAccion=" + (short)pTipoAccionExterna).Length == 1)
                if (pdataWrapperProyecto != null && pdataWrapperProyecto.ListaAccionesExternasProyecto != null && pdataWrapperProyecto.ListaAccionesExternasProyecto.Where(acciones => acciones.ProyectoID.Equals(pProyectoID) && acciones.TipoAccion.Equals((short)pTipoAccionExterna)).ToList().Count == 1)
                {
                    List<JsonEventoUsuario> listadoEventosusuarioProyecto;
                    List<JsonDatosExtraUsuario> listaDatosExtra = new List<JsonDatosExtraUsuario>();
                    //Obtención de las cláusulas del registro y eventos activos
                    ObtenerClausulasYEventosUsuarioEnProyecto(pUsuarioID, pProyectoID, ref listaDatosExtra, out listadoEventosusuarioProyecto);

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                    DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(pIdentidadID);
                    dataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadPorID(pIdentidadID, false));
                    GestionIdentidades gestorIdent = new GestionIdentidades(dataWrapperIdentidad, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    bool recibirNews = gestorIdent.ListaIdentidades[pIdentidadID].FilaIdentidad.RecibirNewsLetter;
                    identidadCN.Dispose();

                    foreach (DatoExtraProyectoOpcionIdentidad filaDatoExtraProyIdent in dataWrapperIdentidad.ListaDatoExtraProyectoOpcionIdentidad)
                    {
                        if (pdataWrapperProyecto.ListaDatoExtraProyecto != null && pdataWrapperProyecto.ListaDatoExtraProyecto.Count > 0)
                        {
                            //Select("ProyectoID='" + pProyectoID + "' and DatoExtraID='" + filaDatoExtraProyIdent.DatoExtraID + "'");
                            List<DatoExtraProyecto> filasDatoExtraProy = pdataWrapperProyecto.ListaDatoExtraProyecto.Where(dato => dato.ProyectoID.Equals(pProyectoID) && dato.DatoExtraID.Equals(filaDatoExtraProyIdent.DatoExtraID)).ToList();
                            //Select("ProyectoID='" + pProyectoID + "' and OpcionID='" + filaDatoExtraProyIdent.OpcionID + "'")
                            List<DatoExtraProyectoOpcion> filasDatoExtraProyOpcion = pdataWrapperProyecto.ListaDatoExtraProyectoOpcion.Where(dato => dato.ProyectoID.Equals(pProyectoID) && dato.OpcionID.Equals(filaDatoExtraProyIdent.OpcionID)).ToList();

                            if (filasDatoExtraProy.Count > 0 && filasDatoExtraProyOpcion.Count > 0)
                            {
                                JsonDatosExtraUsuario datosExtraUsuario = new JsonDatosExtraUsuario();
                                datosExtraUsuario.name = filasDatoExtraProy[0].Titulo;
                                datosExtraUsuario.name_id = filasDatoExtraProy[0].DatoExtraID;
                                datosExtraUsuario.value = filasDatoExtraProyOpcion[0].Opcion;
                                datosExtraUsuario.value_id = filasDatoExtraProyOpcion[0].OpcionID;
                                listaDatosExtra.Add(datosExtraUsuario);
                            }
                        }
                    }

                    foreach (AD.EntityModel.Models.IdentidadDS.DatoExtraProyectoVirtuosoIdentidad filaDatoExtraProyVirtuosoIdent in dataWrapperIdentidad.ListaDatoExtraProyectoVirtuosoIdentidad)
                    {
                        if (pdataWrapperProyecto.ListaDatoExtraProyectoVirtuoso != null && pdataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.Count > 0)
                        {
                            //.Select("ProyectoID='" + pProyectoID + "' and DatoExtraID='" + filaDatoExtraProyVirtuosoIdent.DatoExtraID + "'")[0];

                            DatoExtraProyectoVirtuoso filaDatoExtraVirtuosoProy = pdataWrapperProyecto.ListaDatoExtraProyectoVirtuoso.FirstOrDefault(dato => dato.ProyectoID.Equals(pProyectoID) && dato.DatoExtraID.Equals(filaDatoExtraProyVirtuosoIdent.DatoExtraID));

                            if (filaDatoExtraVirtuosoProy != null)
                            {
                                JsonDatosExtraUsuario datosExtraUsuario = new JsonDatosExtraUsuario();
                                datosExtraUsuario.name = filaDatoExtraVirtuosoProy.Titulo;
                                datosExtraUsuario.name_id = filaDatoExtraVirtuosoProy.DatoExtraID;
                                datosExtraUsuario.value = filaDatoExtraProyVirtuosoIdent.Opcion;
                                listaDatosExtra.Add(datosExtraUsuario);
                            }
                        }
                    }

                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    string nombreCortoUsu = usuarioCN.ObtenerNombreCortoUsuarioPorID(pUsuarioID);
                    usuarioCN.Dispose();

                    JsonUsuario usuario = new JsonUsuario();
                    usuario.name = pNombre;
                    usuario.last_name = pApellidos;
                    usuario.email = pEmail;
                    usuario.community_id = pProyectoID;
                    usuario.user_id = pUsuarioID;
                    usuario.user_short_name = nombreCortoUsu;
                    usuario.receive_newsletter = recibirNews;
                    usuario.languaje = pIdioma;

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    usuario.community_short_name = proyCN.ObtenerNombreCortoProyecto(pProyectoID);
                    proyCN.Dispose();

                    if (listaDatosExtra.Count > 0)
                    {
                        usuario.extra_data = listaDatosExtra;
                    }
                    usuario.user_events = listadoEventosusuarioProyecto;
                    usuario.aux_data = pDatoAuxiliar;

                    if (pFechaNacimiento.HasValue)
                    {
                        usuario.born_date = pFechaNacimiento.Value;
                    }

                    PaisCN paisCN = new PaisCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PaisCN>(), mLoggerFactory);
                    if (pPaisID.HasValue && !pPaisID.Value.Equals(Guid.Empty))
                    {
                        usuario.country_id = pPaisID.Value;
                        usuario.country = paisCN.ObtenerNombrePais(pPaisID.Value);
                    }

                    usuario.province_id = pProvinciaID;
                    usuario.provice = pProvincia;
                    if (pPaisID.HasValue && !pPaisID.Value.Equals(Guid.Empty) && !pProvinciaID.Equals(Guid.Empty) && string.IsNullOrEmpty(pProvincia))
                    {
                        usuario.provice = paisCN.ObtenerNombreProvincia(pPaisID.Value, pProvinciaID);
                    }

                    if (pFechaRegistroComunidad.HasValue)
                    {
                        usuario.join_community_date = pFechaRegistroComunidad.Value;
                    }
                    paisCN.Dispose();

                    usuario.city = pLocalidad;
                    usuario.sex = pSexo;
                    usuario.postal_code = pCodigoPostal;
                    usuario.id_card = pDNI;

                    //Suscripciones
                    SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
                    DataWrapperSuscripcion suscDW = suscCN.ObtenerSuscripcionesDeIdentidad(pIdentidadID, true);
                    GestionSuscripcion gestorSuscripciones = new GestionSuscripcion(suscDW, mLoggingService, mEntityContext);
                    suscCN.Dispose();
                    Elementos.Suscripcion.Suscripcion suscripcion = gestorSuscripciones.ObtenerSuscripcionAProyecto(pProyectoID);

                    usuario.preferences = new Dictionary<Guid, string>();
                    usuario.ListaPreferenciasJerarquicas = new List<JsonPreferenciaJerarquica>();

                    if (suscripcion != null && suscripcion.FilasCategoriasVinculadas != null)
                    {
                        TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);
                        GestionTesauro gestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(pProyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                        gestorTesauro.CargarCategorias();
                        tesauroCL.Dispose();

                        foreach (AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip filaCat in suscripcion.FilasCategoriasVinculadas)
                        {
                            string nomCat = gestorTesauro.ListaCategoriasTesauro[filaCat.CategoriaTesauroID].Nombre["es"];
                            usuario.preferences.Add(filaCat.CategoriaTesauroID, nomCat);
                            usuario.ListaPreferenciasJerarquicas.Add(ObtenerCategoriasJerarquicas(filaCat.CategoriaTesauroID, gestorTesauro.ListaCategoriasTesauro));
                        }
                    }
                    //.Select("TipoAccion=" + (short)pTipoAccionExterna + " AND ProyectoID='" + pProyectoID + "'")[0];
                    AccionesExternasProyecto filaAccionesExrternasProyecto = pdataWrapperProyecto.ListaAccionesExternasProyecto.FirstOrDefault(accion => accion.TipoAccion.Equals((short)pTipoAccionExterna) && accion.ProyectoID.Equals(pProyectoID));

                    //string url = filaAccionesExrternasProyecto.URL + "?json=" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(usuario));//GET
                    //string respuesta = UtilWeb.WebRequest(UtilWeb.Metodo.GET, url, "");//GET
                    string url = filaAccionesExrternasProyecto.URL;//POST
                    string respuesta = string.Empty;
                    try
                    {
                        respuesta = new UtilWeb(mHttpContextAccessor).WebRequest(UtilWeb.Metodo.POST, url, JsonConvert.SerializeObject(usuario), "application/json");//POST
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Error en servicio externo: " + url, mlogger);
                    }

                    JsonEstado jsonEstado = new JsonEstado();
                    jsonEstado.Correcto = false;
                    jsonEstado.InfoExtra = "";

                    if (!string.IsNullOrEmpty(respuesta))
                    {
                        jsonEstado = JsonConvert.DeserializeObject<JsonEstado>(respuesta);
                    }

                    //log
                    if (!jsonEstado.Correcto)
                    {
                        Exception ex = new Exception(jsonEstado.InfoExtra);
                        mLoggingService.GuardarLogError(ex, "ERROR AccionEnServicioExternoProyecto. Respuesta servicio externo : " + respuesta, mlogger);
                    }

                    return jsonEstado;
                }
            }
            return null;
        }


        public void NotificarEdicionPerfilEnProyectos(TipoAccionExterna pTipoAccionExterna, Guid pPersonaID, string pPass, string pDatoAuxiliar)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            DataWrapperPersona dataWrapperPersona = personaCN.ObtenerPersonaPorID(pPersonaID);
            personaCN.Dispose();

            GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
            gestorPersonas.CargarGestor();
            Elementos.ServiciosGenerales.Persona persona = gestorPersonas.ListaPersonas[pPersonaID];

            IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestorIdentidades = new GestionIdentidades(idenCN.ObtenerIdentidadesDePersonaMuyLigera(pPersonaID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            idenCN.Dispose();

            List<Guid> listaproyectosUsuario = new List<Guid>();
            foreach (Guid identidadID in gestorIdentidades.ListaIdentidades.Keys)
            {
                if (!listaproyectosUsuario.Contains(gestorIdentidades.ListaIdentidades[identidadID].FilaIdentidad.ProyectoID))
                {
                    listaproyectosUsuario.Add(gestorIdentidades.ListaIdentidades[identidadID].FilaIdentidad.ProyectoID);
                }
            }

            //Compruebo que hay algún proyecto con acciones externas 
            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            List<Guid> proyectosConAccionesExternas = proyectoCL.ObtenerListaIDsProyectosConAccionesExternas();
            proyectoCL.Dispose();

            if (proyectosConAccionesExternas != null && proyectosConAccionesExternas.Count > 0)
            {
                //Cargamos todos los datos de todos los proyectos en los que participa el usuario
                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                DataWrapperProyecto dataWrapperProyecto = proyectoCN.ObtenerAccionesExternasProyectoPorListaIDs(listaproyectosUsuario);
                dataWrapperProyecto.Merge(proyectoCN.ObtenerDatosExtraProyectoPorListaIDs(listaproyectosUsuario));
                proyectoCN.Dispose();

                foreach (Guid identidadID in gestorIdentidades.ListaIdentidades.Keys)
                {
                    AccionEnServicioExternoProyecto(pTipoAccionExterna, persona, gestorIdentidades.ListaIdentidades[identidadID].FilaIdentidad.ProyectoID, identidadID, pPass, pDatoAuxiliar, gestorIdentidades.ListaIdentidades[identidadID].FilaIdentidad.FechaAlta, dataWrapperProyecto);
                }
            }
        }

        public void BloquearDesbloquearUsuario(Identidad pIdentidad, bool pBloquear)
        {
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<UsuarioCN>(), mloggerFactory);
            SuscripcionCN suscripCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);

            if (pIdentidad.EsOrganizacion)
            {
                //Cargar todas las identidades de la organización
                //Cargar los usuarios de las identidades cargadas
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);

                GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesDeOrganizacion(pIdentidad.OrganizacionID.Value, pIdentidad.FilaIdentidad.ProyectoID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorIdentidades.GestorPersonas = new GestionPersonas(personaCN.ObtenerPersonasDeOrganizacion(pIdentidad.OrganizacionID.Value), mLoggingService, mEntityContext);

                DataWrapperUsuario proyectoRolUsuarioDW = usuarioCN.ObtenerUsuariosCompletosPorID(gestorIdentidades.GestorPersonas.ListaPersonas.Values.Select(persona => persona.UsuarioID).ToList());
                usuarioCN.Dispose();

                gestorIdentidades.GestorUsuarios = new GestionUsuarios(proyectoRolUsuarioDW, mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);

                DataWrapperSuscripcion TodasSuscripcionDW = new DataWrapperSuscripcion();
                Guid organizacionSeleccionadaID = (Guid)pIdentidad.OrganizacionID;

                // Bloqueamos/DesBloqueamos a la Organizacion en si (perfil tipo 3) del proyecto

                OrganizacionCN organizaCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                DataWrapperOrganizacion orgDW = organizaCN.ObtenerOrganizacionPorID(organizacionSeleccionadaID);
                organizaCN.Dispose();

                OrganizacionParticipaProy filaOrganizacionParticipaProy = orgDW.ListaOrganizacionParticipaProy.Where(item => item.OrganizacionID.Equals((Guid)pIdentidad.OrganizacionID) && item.OrganizacionProyectoID.Equals(pIdentidad.FilaIdentidad.OrganizacionID) && item.ProyectoID.Equals(pIdentidad.FilaIdentidad.ProyectoID)).ToList().FirstOrDefault();

                if (filaOrganizacionParticipaProy != null)
                {
                    filaOrganizacionParticipaProy.EstaBloqueada = pBloquear;
                }

                // Bloqueamos/Desbloqueamos a los usuarios de la organizacion que participan en este proyecto (perfil tipo 1 o 2)

                // Selcciono las identidades que tienen un perfil de tipo 1 o 2 vinculadas a la organizacion --> "IdentidadPagina.EsOrganizacion"

                List<Identidad> listaIdentidadesDeEmpleadosDeOrg = gestorIdentidades.ListaIdentidades.Values.Where(identidad => !identidad.EsOrganizacion && identidad.TrabajaPersonaConOrganizacion && identidad.PerfilUsuario.FilaPerfil.OrganizacionID == organizacionSeleccionadaID && identidad.FilaIdentidad.ProyectoID == pIdentidad.FilaIdentidad.ProyectoID).ToList();

                // Bloqueo/Desbloqueo dichas identidades del proyecto
                foreach (Identidad identidadDeEmpleado in listaIdentidadesDeEmpleadosDeOrg)
                {//FindByOrganizacionGnossIDProyectoIDUsuarioID(pIdentidad.FilaIdentidad.OrganizacionID, pIdentidad.FilaIdentidad.ProyectoID, identidadDeEmpleado.Usuario.Clave)
                    AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario proyectoRolUsuario = proyectoRolUsuarioDW.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs => proyRolUs.OrganizacionGnossID.Equals(pIdentidad.FilaIdentidad.OrganizacionID) && proyRolUs.ProyectoID.Equals(pIdentidad.FilaIdentidad.ProyectoID) && proyRolUs.UsuarioID.Equals(identidadDeEmpleado.Usuario.Clave));
                    if (proyectoRolUsuario != null)
                    {
                        proyectoRolUsuario.EstaBloqueado = pBloquear;

                        // Bloqueamos/Desbloqueamos las Suscripciones de la identidad que abandona el proyecto

                        DataWrapperSuscripcion suscripcionDW = suscripCN.ObtenerSuscripcionesDeIdentidad(identidadDeEmpleado.Clave, true);

                        if (suscripcionDW.ListaSuscripcion.Count > 0)
                        {
                            foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in suscripcionDW.ListaSuscripcion.Where(item => item.IdentidadID.Equals(identidadDeEmpleado.Clave)))
                            {
                                filaSuscripcion.Bloqueada = pBloquear;
                            }

                            TodasSuscripcionDW.Merge(suscripcionDW);
                        }
                    }
                }
                listaIdentidadesDeEmpleadosDeOrg.Clear();

                mEntityContext.SaveChanges();
            }
            else
            {
                DataWrapperUsuario proyectoRolUsuarioDW = null;
                if (pIdentidad.Persona.FilaPersona.UsuarioID.HasValue)
                {
                    proyectoRolUsuarioDW = usuarioCN.ObtenerUsuarioCompletoPorID(pIdentidad.Persona.FilaPersona.UsuarioID.Value);
                }
                usuarioCN.Dispose();

                pIdentidad.GestorIdentidades.GestorUsuarios = new GestionUsuarios(proyectoRolUsuarioDW, mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
                // Bloqueamos/Desbloqueamos al usuario en el proyecto (perfil tipo 0 , 1)
                if (proyectoRolUsuarioDW != null)
                {
                    AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario proyectoRolUsuario = proyectoRolUsuarioDW.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs => proyRolUs.OrganizacionGnossID.Equals(pIdentidad.FilaIdentidad.OrganizacionID) && proyRolUs.ProyectoID.Equals(pIdentidad.FilaIdentidad.ProyectoID) && proyRolUs.UsuarioID.Equals(pIdentidad.Usuario.Clave));
                    if (proyectoRolUsuario != null)
                    {
                        proyectoRolUsuario.EstaBloqueado = pBloquear;

                        // Bloqueamos/Desbloqueamos las Suscripciones de la IdentidadPagina que abandona el proyecto

                        DataWrapperSuscripcion suscripcionDW = suscripCN.ObtenerSuscripcionesDeIdentidad(pIdentidad.Clave, true);

                        if (suscripcionDW.ListaSuscripcion.Count > 0)
                        {
                            foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in suscripcionDW.ListaSuscripcion.Where(item => item.IdentidadID.Equals(pIdentidad.Clave)))
                            {
                                filaSuscripcion.Bloqueada = pBloquear;
                            }
                        }

                        mEntityContext.SaveChanges();
                    }
                }
            }

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            if (pIdentidad.Persona.FilaPersona.UsuarioID.HasValue)
            {
                proyCL.InvalidarUsuarioBloqueadoProyecto(pIdentidad.FilaIdentidad.ProyectoID, pIdentidad.Persona.FilaPersona.UsuarioID.Value);
            }

            proyCL.Dispose();
        }

        public bool CambiarRolUsuarioEnProyecto(Identidad pIdentidad, short pRol)
        {
            bool rolCambiado;

            TipoRolUsuario tipoRolUsuario;
            if (!Enum.TryParse(pRol.ToString(), out tipoRolUsuario))
            {
                mLoggingService.GuardarLogError(new Exception("ERROR: CambiarRolUsuarioEnProyecto. El rol no es válido. Rol: " + pRol), mlogger);
                return false;
            }

            Guid usuarioID = pIdentidad.Persona.UsuarioID;
            Guid proyectoID = pIdentidad.FilaIdentidad.ProyectoID;
            Guid organizacionID = pIdentidad.FilaIdentidad.OrganizacionID;

            short rolAnterior = 2; // usuario

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto administradorProyectoDS = proyCN.ObtenerAdministradorProyectoDeProyecto(proyectoID);

            AD.EntityModel.Models.ProyectoDS.AdministradorProyecto filaAdministrador = administradorProyectoDS.ListaAdministradorProyecto.FirstOrDefault(filaAdminProy => filaAdminProy.ProyectoID.Equals(proyectoID) && filaAdminProy.UsuarioID.Equals(usuarioID));

            if (filaAdministrador != null)
            {
                rolAnterior = filaAdministrador.Tipo;
            }

            rolCambiado = rolAnterior != pRol;
            if (rolCambiado)
            {
                UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                GestionUsuarios gestorUsuarios = new GestionUsuarios(usuCN.ObtenerUsuarioCompletoPorID(usuarioID), mLoggingService, mEntityContext, mConfigService, mLoggerFactory.CreateLogger<GestionUsuarios>(), mLoggerFactory);
                usuCN.Dispose();
                AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyectoRolUsuario = gestorUsuarios.DataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs => proyRolUs.OrganizacionGnossID.Equals(organizacionID) && proyRolUs.ProyectoID.Equals(proyectoID) && proyRolUs.UsuarioID.Equals(usuarioID));

                if (pRol == (short)TipoRolUsuario.Usuario && filaAdministrador != null)
                {
                    administradorProyectoDS.ListaAdministradorProyecto.Remove(filaAdministrador);
                    mEntityContext.EliminarElemento(filaAdministrador);
                }
                else if (filaAdministrador == null && pRol != (short)TipoRolUsuario.Usuario)
                {
                    AdministradorProyecto adminProy = new AdministradorProyecto();
                    adminProy.OrganizacionID = organizacionID;
                    adminProy.ProyectoID = proyectoID;
                    adminProy.UsuarioID = usuarioID;
                    adminProy.Tipo = pRol;

                    administradorProyectoDS.ListaAdministradorProyecto.Add(adminProy);
                    mEntityContext.AdministradorProyecto.Add(adminProy);
                }
                else if (filaAdministrador != null)
                {
                    AdministradorProyecto adminProy = new AdministradorProyecto();
                    adminProy.OrganizacionID = filaAdministrador.OrganizacionID;
                    adminProy.ProyectoID = filaAdministrador.ProyectoID;
                    adminProy.UsuarioID = filaAdministrador.UsuarioID;
                    adminProy.Tipo = pRol;
                    administradorProyectoDS.DeleteAdministradorProyecto(filaAdministrador);
                    mEntityContext.AdministradorProyecto.Remove(filaAdministrador);
                    //mEntityContext.SaveChanges();
                    administradorProyectoDS.ListaAdministradorProyecto.Add(adminProy);
                    mEntityContext.AdministradorProyecto.Add(adminProy);
                }

                if (pRol == (short)TipoRolUsuario.Administrador)
                {
                    // Le pongo como administrador
                    //Le doy todos los permisos
                    filaProyectoRolUsuario.RolPermitido = UsuarioAD.FilaPermisosAdministrador;
                    //No le deniego ninguno
                    filaProyectoRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                }
                else if (pRol == (short)TipoRolUsuario.Supervisor)
                {
                    // Le pongo como Supervisor
                    string RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                    string RolDenegado = UsuarioAD.FilaPermisosSinDefinir;

                    //Si no tiene el permiso permitido
                    if ((Convert.ToUInt64(RolPermitido, 16) & (ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos) == 0)
                    {
                        RolPermitido = (Convert.ToUInt64(RolPermitido, 16) | (ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos).ToString("X16");
                    }

                    filaProyectoRolUsuario.RolPermitido = RolPermitido;

                    filaProyectoRolUsuario.RolDenegado = RolDenegado;

                }
                else if (pRol == (short)TipoRolUsuario.Usuario)
                {
                    // Le pongo como Usuario normal
                    string rolPermitido = filaProyectoRolUsuario.RolPermitido;
                    string rolDenegado;

                    if (filaProyectoRolUsuario.RolDenegado == null)
                    {
                        rolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                    }
                    else
                    {
                        rolDenegado = filaProyectoRolUsuario.RolDenegado;
                    }

                    //Si tiene el permiso permitido
                    if ((Convert.ToUInt64(rolPermitido, 16) & (ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos) != 0)
                    {
                        rolPermitido = (Convert.ToUInt64(rolPermitido, 16) & (ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos).ToString("X16");
                    }

                    //Si no tiene el permiso denegado
                    if ((Convert.ToUInt64(rolDenegado, 16) & (ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos) == 0)
                    {
                        rolDenegado = (Convert.ToUInt64(rolDenegado, 16) | (ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos).ToString("X16");
                    }

                    filaProyectoRolUsuario.RolPermitido = rolPermitido;
                    filaProyectoRolUsuario.RolDenegado = rolDenegado;
                }
                else if (pRol == (short)TipoRolUsuario.Diseniador)
                {
                    // Le pongo como Diseñador
                    string rolPermitido = UsuarioAD.FilaPermisosSinDefinir;

                    //Si no tiene el permiso permitido
                    if ((Convert.ToUInt64(rolPermitido, 16) & (ulong)Capacidad.Proyecto.CapacidadesGenerales.Diseniador) == 0)
                    {
                        rolPermitido = (Convert.ToUInt64(rolPermitido, 16) | (ulong)Capacidad.Proyecto.CapacidadesGenerales.Diseniador).ToString("X16");
                    }

                    filaProyectoRolUsuario.RolPermitido = rolPermitido;
                    filaProyectoRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                }

                mEntityContext.SaveChanges();

                ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                proyectoCL.InvalidarHTMLAdministradoresProyecto(proyectoID);
                proyectoCL.InvalidarFilaProyecto(proyectoID);

                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                identidadCL.EliminarCacheGestorTodasIdentidadesUsuario(usuarioID, pIdentidad.PersonaID.Value);
            }

            return rolCambiado;
        }

        /// <summary>
        /// Registra el usuario en las comunidades configuradas como registro automático y obligatorio. Si el usuario ha sido creado a partir de una invitación a una organización, lo añadirá a las comunidades 
        /// que tenga la organización configuradas como registro automático.
        /// </summary>
        /// <param name="pSolicitudNuevoUsuario">Solicitud de la creación del usuario del cual vamos a crear las identidades</param>
        /// <param name="pPerfil">Perfil a partir del cual se crearán las identidades</param>
        /// <param name="pUsuario">Usuario del cual se van a crear las identidades</param>
        /// <param name="pGestionUsuarios">Gestor de usuarios necesario para el registro</param>
        /// <param name="pGestionIdentidades">Gestor de identidades necesario para el registro</param>		
        private void RegistrosAutomaticosEnComunidades(SolicitudNuevoUsuario pSolicitudNuevoUsuario, Perfil pPerfil, AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, GestionUsuarios pGestionUsuarios, GestionIdentidades pGestionIdentidades)
        {
			ControladorDeSolicitudes controladorDeSolicitudes = new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDeSolicitudes>(), mLoggerFactory);

            controladorDeSolicitudes.RegistrarUsuarioEnProyectosObligatorios(pSolicitudNuevoUsuario.Solicitud.OrganizacionID, pSolicitudNuevoUsuario.Solicitud.ProyectoID, pPerfil.PersonaID.Value, pPerfil, pUsuario, pGestionUsuarios, pGestionIdentidades);
            controladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(pPerfil, pUsuario, pGestionUsuarios, pGestionIdentidades);
            RegistrarEnProyectoAutoacceso(controladorDeSolicitudes, pSolicitudNuevoUsuario, pUsuario, pGestionIdentidades, pGestionUsuarios, pPerfil);

            pGestionIdentidades.RecargarHijos();
        }

        /// <summary>
        /// Se registra en los proyectos definidos en el campo ProyectoAutoAcceso de la tabla SolicitudNuevoUsuario
        /// </summary>
        /// <param name="pControladorDeSolicitudes">Controlador de solicitudes para realizar la gestión del registro</param>
        /// <param name="pSolicitudNuevoUsuario">Solicitud de registro del usuario nuevo</param>
        /// <param name="pUsuario">Usuario del cual se crearán las identidades en la comunidad</param>
        /// <param name="pGestionIdentidades">Gestor de identidades necesario para el registro</param>
        /// <param name="pGestionUsuarios">Gestor de usuarios necesario para el registro</param>
        /// <param name="pPerfil">Perfil a partir del cual se crearán las identidades</param>
        private void RegistrarEnProyectoAutoacceso(ControladorDeSolicitudes pControladorDeSolicitudes, SolicitudNuevoUsuario pSolicitudNuevoUsuario, AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, GestionIdentidades pGestionIdentidades, GestionUsuarios pGestionUsuarios, Perfil pPerfil)
        {
			if (pSolicitudNuevoUsuario.ProyectosAutoAcceso != null && pSolicitudNuevoUsuario.ProyectosAutoAcceso != "")
			{
                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
				string[] proysAutoAccesoID = pSolicitudNuevoUsuario.ProyectosAutoAcceso.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string proyAutoAccID in proysAutoAccesoID)
                {
                    Guid proyAutoAccesoID = new Guid(proyAutoAccID);

                    if (!proyectoCN.ParticipaUsuarioEnProyecto(proyAutoAccesoID, pUsuario.UsuarioID))
                    {
                        Guid orgAutoAccID = proyectoCN.ObtenerOrganizacionIDProyecto(proyAutoAccesoID);
                        AgregarIdentidadPerfilYUsuarioAProyecto(pGestionIdentidades, pGestionUsuarios, orgAutoAccID, proyAutoAccesoID, pUsuario, pPerfil, null);

                        pControladorDeSolicitudes.RegistrarUsuarioEnProyectoAutomatico(pPerfil, pUsuario, pGestionUsuarios, pGestionIdentidades);
                    }
                }
            }
        }

        #region Métodos Auxiliares AccionesExternas

        private static JsonPreferenciaJerarquica ObtenerCategoriasJerarquicas(Guid pCategoriaID, SortedList<Guid, CategoriaTesauro> pListaCategoriasTesauro)
        {
            JsonPreferenciaJerarquica preferenciaJerarquica = new JsonPreferenciaJerarquica();
            preferenciaJerarquica.CategoriaID = pCategoriaID;
            preferenciaJerarquica.Nombre = pListaCategoriasTesauro[pCategoriaID].Nombre["es"];
            preferenciaJerarquica.CategoriaPadre = null;

            if (pListaCategoriasTesauro[pCategoriaID].Padre != null && pListaCategoriasTesauro[pCategoriaID].Padre is CategoriaTesauro)
            {
                JsonPreferenciaJerarquica padre = ObtenerCategoriasJerarquicas(((CategoriaTesauro)pListaCategoriasTesauro[pCategoriaID].Padre).Clave, pListaCategoriasTesauro);

                preferenciaJerarquica.CategoriaPadre = padre;
            }

            return preferenciaJerarquica;
        }

        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario que participa en el proyecto con alguna identidad</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que participa el usuario</param>
        /// <param name="pListaJsonDatosExtra">Lista de DatosExtra con las clausulas del registro</param>
        /// <param name="listadoEventosusuarioProyecto">Eventos del proyecto activos durante el registro del usuario</param>
        ///// <param name="recibirComMedio">Booleano que indica si el usuario desea recibir comunicados por parte del medio</param>
        ///// <param name="recibirComTerceros">Booleano que indica si el usuario desea recibir comunicados por parte de terceros</param>
        ///// <param name="medio_origen">Medio de origen con el que se registro el usuario (campaña web, redes sociales...)</param>
        ///// <param name="nombre_medio_origen">Nombre del medio de origen con el que se registro el usuario(Landing..., Facebook, Twitter...)</param>
        private void ObtenerClausulasYEventosUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID, ref List<JsonDatosExtraUsuario> pListaJsonDatosExtra, /*out bool recibirComMedio, out bool recibirComTerceros, out string medio_origen, out string nombre_medio_origen,*/out List<JsonEventoUsuario> pListadoEventosUsuarioProyecto)
        {
            if (pListaJsonDatosExtra == null)
            {
                pListaJsonDatosExtra = new List<JsonDatosExtraUsuario>();
            }
            pListadoEventosUsuarioProyecto = new List<JsonEventoUsuario>();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            DataWrapperUsuario usuClauProyDW = proyCL.ObtenerClausulasRegitroProyecto(pProyectoID);
            proyCL.Dispose();

            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(),mLoggerFactory);
            usuClauProyDW.Merge(usuCN.ObtenerProyClausulasUsuPorUsuarioID(pUsuarioID));
            usuCN.Dispose();

            foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in usuClauProyDW.ListaClausulaRegistro.Where(item => item.Tipo.Equals((short)TipoClausulaAdicional.Opcional)))
            {
                AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg filasProyRolClau = usuClauProyDW.ListaProyRolUsuClausulaReg.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.ProyectoID.Equals(pProyectoID) && item.ClausulaID.Equals(filaClausula.ClausulaID)).FirstOrDefault();

                if (filasProyRolClau != null)
                {
                    JsonDatosExtraUsuario jsonDatosExtra = new JsonDatosExtraUsuario();
                    jsonDatosExtra.name_id = filaClausula.ClausulaID;
                    jsonDatosExtra.name = filaClausula.Texto;
                    jsonDatosExtra.value = filasProyRolClau.Valor.ToString();
                    pListaJsonDatosExtra.Add(jsonDatosExtra);
                }
            }

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            Guid identidadID = identCN.ObtenerIdentidadUsuarioEnProyecto(pUsuarioID, pProyectoID);
            identCN.Dispose();

            if (!identidadID.Equals(Guid.Empty))
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                DataSet ds = proyCN.ObtenerEventoProyectoIdentidadID(pProyectoID, identidadID);
                proyCN.Dispose();

                if (ds.Tables["EventosProyectoIdentidad"] != null && ds.Tables["EventosProyectoIdentidad"].Rows.Count > 0)
                {
                    foreach (DataRow fila in ds.Tables["EventosProyectoIdentidad"].Rows)
                    {
                        JsonEventoUsuario jsonEventoUsuario = new JsonEventoUsuario();
                        jsonEventoUsuario.event_id = (Guid)fila["EventoID"];
                        jsonEventoUsuario.name = fila["InfoExtra"].ToString();
                        jsonEventoUsuario.Date = (DateTime)fila["Fecha"];
                        pListadoEventosUsuarioProyecto.Add(jsonEventoUsuario);
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
