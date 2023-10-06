using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.ComparticionAutomatica;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ComparticionAutomatica;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.ComparticionAutomatica;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ExportacionBusqueda;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Proyectos
{

    public enum ObjectState
    {
        SinCambios = 0,
        Editado = 1,
        Agregado = 2
    }
    /// <summary>
    /// Controlador para Proyectos
    /// </summary>
    public class ControladorProyecto : ControladorBase
    {

        #region Miembros

        private Dictionary<string, List<Object>> mListaOriginalesEF = new Dictionary<string, List<Object>>();
        private Dictionary<string, List<Object>> mListaModificadasEF = new Dictionary<string, List<Object>>();
        private Dictionary<string, string> mParametroProyecto;
        private List<string> mRutasPestanyasInvalidar = new List<string>();
        private List<string> mRutasPestanyasRegistrar = new List<string>();
        private List<string> mRutasPestanyas = new List<string>();

        public bool IgnorarErroresGrupos = false;
        public bool CrearFilasPropiedadesExportacion = false;

        private EntityContextBASE mEntityContextBASE;

        #region Constantes

        /// <summary>
        /// Nombre de la pestaña de búsqueda avanzada para configuración.
        /// </summary>
        public const string NombrePestaniaBusquedaAvanzada = "busqueda";
        public List<IntegracionContinuaPropiedad> FilasPropiedadesIntegracion = null;

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Lista de Rutas de pestanyas que se van a invalidar
        /// </summary>
        public List<string> RutasPestanyasInvalidar
        {
            get
            {
                return mRutasPestanyasInvalidar;
            }
            set
            {
                mRutasPestanyasInvalidar = value;
            }
        }

        /// <summary>
        /// Lista de Rutas de pestanyas que se van a registrar
        /// </summary>
        public List<string> RutasPestanyasRegistrar
        {
            get
            {
                return mRutasPestanyasRegistrar;
            }
            set
            {
                mRutasPestanyasRegistrar = value;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    mParametroProyecto = proyectoCL.ObtenerParametrosProyecto(ProyectoSeleccionado.Clave);
                    proyectoCL.Dispose();
                }

                return mParametroProyecto;
            }
        }

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pPage">Página</param>
        public ControladorProyecto(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mEntityContextBASE = entityContextBASE;
        }

        #endregion

        #region Metodos generales

        /// <summary>
        /// Añade los usuarios de la lista como administradores de la comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuarios</param>
        /// <returns>Cadena vacía si todo va bien. Descripción del error en caso contrario</returns>
        public string AgregarAdministradoresAComunidad(Guid pOrganizacionID, Guid pProyectoID, List<Guid> pListaUsuariosID)
        {
            return AgregarAdministradoresAComunidad(pOrganizacionID, pProyectoID, pListaUsuariosID, true);
        }

        /// <summary>
        /// Añade los usuarios de la lista como administradores de la comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuarios</param>
        /// <returns>Cadena vacía si todo va bien. Descripción del error en caso contrario</returns>
        public string AgregarAdministradoresAComunidad(Guid pOrganizacionID, Guid pProyectoID, List<Guid> pListaUsuariosID, bool pActualizarLive)
        {
            string error = string.Empty;

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<Guid> identidadesAdministradores = proyectoCN.ObtenerListaIdentidadesAdministradoresPorProyecto(pProyectoID);

            //Cargo los permisos de la tabla Administradorproyecto
            //ProyectoDS dataWrapperProyecto = proyectoCN.ObtenerAdministradorProyectoDeProyecto(pProyectoID);
            //SelectAdministradorProyecto = "SELECT " + IBD.CargarGuid("AdministradorProyecto.OrganizacionID") + ", " + IBD.CargarGuid("AdministradorProyecto.ProyectoID") + ", " + IBD.CargarGuid("AdministradorProyecto.UsuarioID") + ", AdministradorProyecto.Tipo ";
            //bCommand commandSQL = ObtenerComando(SelectAdministradorProyecto + " FROM AdministradorProyecto WHERE ProyectoID = " + IBD.GuidParamValor("ProyectoID"));
            //AgregarParametro(commandSQL, IBD.ToParam("ProyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            //CargarDataSet(commandSQL, proyectoDS, "AdministradorProyecto");
            ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD(mEntityContext);
            var administradorPoyecto = proyectoGBD.CargaAdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID)).Select(adminProy => new
            {
                OrganizacionID = adminProy.OrganizacionID,
                ProyectoID = adminProy.ProyectoID,
                UsuarioID = adminProy.UsuarioID,
                Tipo = adminProy.Tipo
            });

            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            foreach (var adminProyect in administradorPoyecto.ToList())
            {
                AdministradorProyecto adminProyec = new AdministradorProyecto();
                adminProyec.OrganizacionID = adminProyect.OrganizacionID;
                adminProyec.ProyectoID = adminProyect.ProyectoID;
                adminProyec.UsuarioID = adminProyect.UsuarioID;
                adminProyec.Tipo = adminProyect.Tipo;
                dataWrapperProyecto.ListaAdministradorProyecto.Add(adminProyec);
            }
            GestionUsuarios gestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerRolListaUsuariosEnProyecto(pProyectoID, pListaUsuariosID), mLoggingService, mEntityContext, mConfigService);

            foreach (Guid guidUsuarioID in pListaUsuariosID)
            {
                try
                {
                    List<Guid> listaAuxUsuarioID = new List<Guid>();
                    listaAuxUsuarioID.Add(guidUsuarioID);
                    List<Guid> listaIdentidadesUsuario = identidadCN.ObtenerIdentidadesIDDeusuariosEnProyecto(pProyectoID, listaAuxUsuarioID, true);
                    Guid identidadID = listaIdentidadesUsuario[0];

                    //Comprueba si la identidad ya administra el proyecto
                    if (!identidadesAdministradores.Contains(identidadID))
                    {
                        //dataWrapperProyecto.ListaAdministradorProyecto.FindByOrganizacionIDProyectoIDUsuarioIDTipo(pOrganizacionID, pProyectoID, guidUsuarioID, (short)TipoRolUsuario.Administrador) == null
                        AdministradorProyecto adminProyecto = dataWrapperProyecto.ListaAdministradorProyecto.FirstOrDefault(adminProy => adminProy.OrganizacionID.Equals(pOrganizacionID) && adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(guidUsuarioID) && adminProy.Tipo.Equals(TipoRolUsuario.Administrador));
                        if (adminProyecto == null)
                        {
                            //Lo añado a la tabla del gestor de proyectos AdministradorProyecto como Administrador
                            adminProyecto = new AdministradorProyecto();
                            adminProyecto.OrganizacionID = pOrganizacionID;
                            adminProyecto.ProyectoID = pProyectoID;
                            adminProyecto.UsuarioID = guidUsuarioID;
                            adminProyecto.Tipo = (short)TipoRolUsuario.Administrador;
                            dataWrapperProyecto.ListaAdministradorProyecto.Add(adminProyecto);
                            mEntityContext.AdministradorProyecto.Add(adminProyecto);

                            //Si estaba de editor lo quito de editor
                            AdministradorProyecto adminProyectoSupervisor = dataWrapperProyecto.ListaAdministradorProyecto.FirstOrDefault(adminProy => adminProy.OrganizacionID.Equals(pOrganizacionID) && adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(guidUsuarioID) && adminProy.Tipo.Equals((short)TipoRolUsuario.Supervisor));
                            if (adminProyectoSupervisor != null)
                            {
                                dataWrapperProyecto.ListaAdministradorProyecto.Remove(adminProyectoSupervisor);
                                proyectoGBD.DeleteAdministradorProyecto(adminProyectoSupervisor);
                                proyectoGBD.GuardarCambios();
                            }

                            //Si estaba de diseñador lo quito de diseñador
                            AdministradorProyecto adminProyectoDiseniador = dataWrapperProyecto.ListaAdministradorProyecto.FirstOrDefault(adminProy => adminProy.OrganizacionID.Equals(pOrganizacionID) && adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(guidUsuarioID) && adminProy.Tipo.Equals((short)TipoRolUsuario.Diseniador));
                            if (adminProyectoDiseniador != null)
                            {
                                dataWrapperProyecto.ListaAdministradorProyecto.Remove(adminProyectoDiseniador);
                                proyectoGBD.DeleteAdministradorProyecto(adminProyectoDiseniador);
                                proyectoGBD.GuardarCambios();
                            }

                            //Le actualizo los permisos del proyecto
                            AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyectoRolUsuario = gestorUsuarios.DataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs => proyRolUs.OrganizacionGnossID.Equals(pOrganizacionID) && proyRolUs.ProyectoID.Equals(pProyectoID) && proyRolUs.UsuarioID.Equals(guidUsuarioID));

                            //Le doy todos los permisos
                            string RolPermitido = UsuarioAD.FilaPermisosAdministrador;
                            //No le deniego ninguno
                            string RolDenegado = UsuarioAD.FilaPermisosSinDefinir;

                            filaProyectoRolUsuario.RolPermitido = RolPermitido;
                            filaProyectoRolUsuario.RolDenegado = RolDenegado;
                        }

                        mEntityContext.SaveChanges();

                        if (pActualizarLive)
                        {
                            //Agregamos el evento a la cola del live
                            LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            LiveDS liveDS = new LiveDS();

                            try
                            {
                                InsertarFilaEnColaRabbitMQ(pProyectoID, identidadID, (int)AccionLive.ComunidadAbierta, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta);
                            }
                            catch (Exception ex)
                            {
                                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                                liveDS.Cola.AddColaRow(pProyectoID, identidadID, (int)AccionLive.ComunidadAbierta, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta, null);
                            }


                            liveCN.ActualizarBD(liveDS);
                            liveDS.Dispose();
                        }

                        ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        proyectoCL.InvalidarHTMLAdministradoresProyecto(pProyectoID);
                        proyectoCL.InvalidarFilaProyecto(pProyectoID);

                        IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        Guid? personaID = personaCN.ObtenerPersonaIDPorUsuarioID(guidUsuarioID);

                        if (personaID.HasValue)
                        {
                            identidadCL.EliminarCacheGestorTodasIdentidadesUsuario(guidUsuarioID, personaID.Value);
                        }
                    }
                    else
                    {
                        error += $"ERROR: El usuario {guidUsuarioID} ya administra el proyecto ||";
                    }
                }
                catch (Exception)
                {
                    error += $"\r\n ERROR: El usuario {guidUsuarioID} ha fallado al añadirlo como administrador del proyecto";
                }
            }
            
            return error;
        }


        /// <summary>
        /// Elimina los usuarios de la lista de administrador de una comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuariosID">Identificador de usuarios</param>
        /// <returns>Cadena vacía si todo va bien. Descripción del error en caso contrario</returns>
        public string EliminarAdministradorComunidad(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuariosID, bool pActualizarLive)
        {
            string error = string.Empty;

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<Guid> identidadesAdministradores = proyectoCN.ObtenerListaIdentidadesAdministradoresPorProyecto(pProyectoID);

            ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD(mEntityContext);
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
            dataWrapperUsuario.ListaProyectoRolUsuario.Add(usuarioCN.ObtenerRolUsuarioEnProyecto(pProyectoID, pUsuariosID));
            GestionUsuarios gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);
            try
            {
                List<Guid> listaAuxUsuarioID = new List<Guid>();
                listaAuxUsuarioID.Add(pUsuariosID);
                List<Guid> listaIdentidadesUsuario = identidadCN.ObtenerIdentidadesIDDeusuariosEnProyecto(pProyectoID, listaAuxUsuarioID, true);
                Guid identidadID = listaIdentidadesUsuario[0];

                //Comprueba si la identidad ya administra el proyecto
                if (identidadesAdministradores.Contains(identidadID))
                {
                    AdministradorProyecto adminProyecto = proyectoGBD.CargaAdministradorProyecto.FirstOrDefault(adminProy => adminProy.OrganizacionID.Equals(pOrganizacionID) && adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(pUsuariosID) && adminProy.Tipo.Equals((short)TipoRolUsuario.Administrador));
                    if (adminProyecto != null)
                    {
                        //Si estaba de administrador lo quito de administrador
                        proyectoGBD.DeleteAdministradorProyecto(adminProyecto);
                        //Le actualizo los permisos del proyecto
                        AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyectoRolUsuario = gestorUsuarios.DataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs => proyRolUs.OrganizacionGnossID.Equals(pOrganizacionID) && proyRolUs.ProyectoID.Equals(pProyectoID) && proyRolUs.UsuarioID.Equals(pUsuariosID));

                        //Le doy todos los permisos
                        string RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                        //No le deniego ninguno
                        string RolDenegado = UsuarioAD.FilaPermisosSinDefinir;

                        filaProyectoRolUsuario.RolPermitido = RolPermitido;
                        filaProyectoRolUsuario.RolDenegado = RolDenegado;
                    }

                    mEntityContext.SaveChanges();

                    if (pActualizarLive)
                    {
                        //Agregamos el evento a la cola del live
                        LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        LiveDS liveDS = new LiveDS();

                        try
                        {
                            InsertarFilaEnColaRabbitMQ(pProyectoID, identidadID, (int)AccionLive.ComunidadAbierta, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                            liveDS.Cola.AddColaRow(pProyectoID, identidadID, (int)AccionLive.ComunidadAbierta, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta, null);
                        }


                        liveCN.ActualizarBD(liveDS);
                        liveDS.Dispose();
                    }

                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    proyectoCL.InvalidarHTMLAdministradoresProyecto(pProyectoID);
                    proyectoCL.InvalidarFilaProyecto(pProyectoID);

                    IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Guid? personaID = personaCN.ObtenerPersonaIDPorUsuarioID(pUsuariosID);

                    if (personaID.HasValue)
                    {
                        identidadCL.EliminarCacheGestorTodasIdentidadesUsuario(pUsuariosID, personaID.Value);
                    }
                }
                else
                {
                    error += "\r\n ERROR: El usuario " + pUsuariosID + " ya no administra el proyecto";
                }
            }
            catch (Exception)
            {
                error += "\r\n ERROR: El usuario " + pUsuariosID + " ha fallado al eliminarlo como administrador del proyecto";
            }


            return error;
        }

        /// <summary>
        /// Expulsa a un usuario de una comunidad
        /// </summary>
        /// <param name="pIidentidad">Identidad del usuario en la comunidad</param>
        /// <param name="pProyecto">Proyecto en el que participa el usuario</param>
        /// <param name="pMotivoExpulsion">Texto con el motivo de la expulsión</param>
        public void ExpulsarUsuarioComunidad(Identidad pIdentidad, Elementos.ServiciosGenerales.Proyecto pProyecto, string pMotivoExpulsion, string pLanguageCode)
        {
            LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            LiveDS liveDS = new LiveDS();

            if (pIdentidad.EsOrganizacion)
            {
                #region Expulso a la organización y a sus miembros

                Guid proyectoID = pProyecto.Clave;
                Guid organizacionIDdelProyecto = pProyecto.FilaProyecto.OrganizacionID;
                Guid organizacionID = (Guid)pIdentidad.OrganizacionID;

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionProyecto gestorProyectos = new GestionProyecto(new DataWrapperProyecto(), mLoggingService, mEntityContext);
                GestionUsuarios gestorUsuarios = new GestionUsuarios(new DataWrapperUsuario(), mLoggingService, mEntityContext, mConfigService);
                GestionIdentidades gestorIdentidades = new GestionIdentidades(new DataWrapperIdentidad(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorProyectos.GestionUsuarios = gestorUsuarios;
                gestorProyectos.GestionUsuarios.GestorIdentidades = gestorIdentidades;
                gestorUsuarios.GestorSuscripciones = new GestionSuscripcion(new DataWrapperSuscripcion(), mLoggingService, mEntityContext);
                gestorUsuarios.GestorSuscripciones.GestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                gestorProyectos.GestionUsuarios.DataWrapperUsuario.Merge(usuarioCN.CargarUsuariosDeOrganizacionYsusIdentidadesYPermisosEnProyectosConPerfilDeDichaOrg(organizacionID));
                gestorProyectos.GestionUsuarios.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesDeOrganizacionYEmpleados(organizacionID));
                gestorProyectos.GestionUsuarios.GestorIdentidades.GestorOrganizaciones = new GestionOrganizaciones(organizacionCN.ObtenerOrganizacionesPorSusIdentidadesDeProyecto(proyectoID), mLoggingService, mEntityContext);

                List<EmailsMiembrosDeProyecto> miembrosProyectoDS = proyCN.ObtenerEmailsMiembrosDeProyecto(proyectoID);

                foreach (AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario in gestorProyectos.GestionUsuarios.DataWrapperUsuario.ListaUsuario)
                {
                    Guid usuarioID = filaUsuario.UsuarioID;
                    AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad proyectoUsuarioIdentidad = gestorProyectos.GestionUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.FirstOrDefault(item => item.UsuarioID.Equals(usuarioID) && item.ProyectoID.Equals(proyectoID));
                    if (proyectoUsuarioIdentidad != null)
                    {
                        Guid identidadID = proyectoUsuarioIdentidad.IdentidadID;
                        List<EmailsMiembrosDeProyecto> listaFiltrada = miembrosProyectoDS.Where(objeto => objeto.IdentidadID.Equals(identidadID)).ToList();
                        if (listaFiltrada.Count > 0)
                        {
                            string email = listaFiltrada[0].Email;
                            string nombre = listaFiltrada[0].Nombre;
                            Guid personaID = listaFiltrada[0].PersonaID.Value;

                            //Mandamos un email a cada miembro del proyecto avisando de su cierre temporal
                            gestorUsuarios.GestorSuscripciones.GestorNotificaciones.AgregarNotificacionExpulsionUsuarioDeComunidad(pProyecto.FilaProyecto.OrganizacionID, pProyecto.FilaProyecto.ProyectoID, email, personaID, DateTime.Now, nombre, pProyecto.FilaProyecto.Nombre, pMotivoExpulsion, pLanguageCode);
                        }
                        gestorProyectos.EliminarUsuarioDeProyecto(usuarioID, proyectoID, organizacionIDdelProyecto, identidadID, gestorUsuarios, gestorIdentidades);

                        //Invalido la cache de Mis comunidades
                        DataWrapperIdentidad idenDW = identidadCN.ObtenerIdentidadPorID(identidadID, true);
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        proyCL.InvalidarMisProyectos(idenDW.ListaIdentidad.First().PerfilID);
                        proyCL.Dispose();

                        //Lo marcamos como expulsado
                        gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(identidadID)).FechaExpulsion = DateTime.Now;

                        #region Eliminación de las Suscripciones de la identidad que abandona el proyecto

                        SuscripcionCN suscripCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperSuscripcion suscripcionDW = suscripCN.ObtenerSuscripcionesDeIdentidad(identidadID, true);

                        if (suscripcionDW.ListaSuscripcion.Count > 0)
                        {
                            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
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

                            gestorUsuarios.GestorSuscripciones.EliminarSuscripciones(identidadID);
                            listaSuscripciones.Clear();
                        }

                        #endregion

                        try
                        {
                            InsertarFilaEnColaRabbitMQ(pProyecto.Clave, gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(identidadID)).PerfilID, (int)AccionLive.Eliminado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                            liveDS.Cola.AddColaRow(pProyecto.Clave, gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(identidadID)).PerfilID, (int)AccionLive.Eliminado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta, null);

                        }
                    }
                }
                //Invalidamos la cache de amigos en la comunidad
                AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                amigosCL.InvalidarAmigosPertenecenProyecto(proyectoID);
                amigosCL.Dispose();

                //Elimino de OrganizacionParticipaProy
                gestorProyectos.GestionUsuarios.GestorIdentidades.GestorOrganizaciones.EliminarOrganizacionDeProyecto(organizacionID, organizacionIDdelProyecto, proyectoID);

                Guid PerfilID = gestorProyectos.GestionUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilOrganizacion.Where(item => item.OrganizacionID.Equals(organizacionID)).Select(item => item.PerfilID).FirstOrDefault();

                Identidad IdentidadOrganizacion = gestorProyectos.GestionUsuarios.GestorIdentidades.ListaIdentidades[pIdentidad.Clave];
                IdentidadOrganizacion.FilaIdentidad.FechaExpulsion = DateTime.Now;

                ControladorOrganizaciones controladorOrg = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                controladorOrg.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[pIdentidad.Clave], proyectoID, true, true, PrioridadBase.Alta);

                //Guardo
                mEntityContext.SaveChanges();

                gestorProyectos.GestionUsuarios.GestorSuscripciones.GestorNotificaciones.Dispose();
                gestorProyectos.GestionUsuarios.GestorSuscripciones.GestorNotificaciones = null;
                gestorProyectos.GestionUsuarios.GestorSuscripciones.Dispose();
                gestorProyectos.GestionUsuarios.GestorSuscripciones = null;
                gestorProyectos.GestionUsuarios.GestorIdentidades.Dispose();
                gestorProyectos.GestionUsuarios.GestorIdentidades = null;
                gestorProyectos.GestionUsuarios.Dispose();
                gestorProyectos.GestionUsuarios = null;
                gestorProyectos.Dispose();
                gestorProyectos = null;

                #endregion
            }
            else
            {
                #region Expulso a el usuario del proyecto

                Guid proyectoID = pProyecto.Clave;
                Guid usuarioID = pIdentidad.Persona.UsuarioID;
                Guid identidadID = pIdentidad.Clave;
                Guid organizacionID = pProyecto.FilaProyecto.OrganizacionID;
                Guid personaID = pIdentidad.Persona.Clave;

                GestionProyecto gestorProyectos = new GestionProyecto(new DataWrapperProyecto(), mLoggingService, mEntityContext);

                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionUsuarios gestorUsuarios = new GestionUsuarios(usuarioCN.ObtenerUsuarioCompletoPorID(usuarioID), mLoggingService, mEntityContext, mConfigService);

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadDePersonaEnProyecto(proyectoID, personaID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                GestionSuscripcion gestorSuscripciones = new GestionSuscripcion(new DataWrapperSuscripcion(), mLoggingService, mEntityContext);
                gestorUsuarios.GestorSuscripciones = gestorSuscripciones;

                GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorUsuarios.GestorSuscripciones.GestorNotificaciones = gestorNotificaciones;
                gestorUsuarios.GestorIdentidades = gestorIdentidades;

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (!proyCN.EsUsuarioAdministradorProyecto(usuarioID, proyectoID))
                {
                    List<EmailsMiembrosDeProyecto> miembrosProyectoDS = proyCN.ObtenerEmailsMiembrosDeProyecto(proyectoID);
                    List<EmailsMiembrosDeProyecto> listaFiltrada = miembrosProyectoDS.Where(objeto => objeto.IdentidadID.Equals(identidadID)).ToList();
                    if (listaFiltrada.Count > 0)
                    {
                        //Mandamos un email al usuario que será expulsado
                        string email = listaFiltrada[0].Email;
                        string nombre = listaFiltrada[0].Nombre;
                        gestorNotificaciones.AgregarNotificacionExpulsionUsuarioDeComunidad(organizacionID, proyectoID, email, personaID, DateTime.Now, nombre, pProyecto.FilaProyecto.Nombre, pMotivoExpulsion, pLanguageCode);
                    }
                    gestorProyectos.EliminarUsuarioDeProyecto(usuarioID, proyectoID, organizacionID, identidadID, gestorUsuarios, gestorIdentidades);

                    //Invalido la cache de Mis comunidades
                    DataWrapperIdentidad idenDW = identidadCN.ObtenerIdentidadPorID(identidadID, true);
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    proyCL.InvalidarMisProyectos(idenDW.ListaIdentidad.First().PerfilID);
                    proyCL.Dispose();

                    //Invalidamos la cache de amigos en la comunidad
                    AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    amigosCL.InvalidarAmigosPertenecenProyecto(proyectoID);
                    amigosCL.Dispose();

                    //Lo marcamos como expulsado
                    gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(identidadID)).FechaExpulsion = DateTime.Now;

                    #region Eliminación de las Suscripciones de la identidad que abandona el proyecto

                    SuscripcionCN suscripCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperSuscripcion suscripcionDW = suscripCN.ObtenerSuscripcionesDeIdentidad(identidadID, true);

                    if (suscripcionDW.ListaSuscripcion.Count > 0)
                    {
                        NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        List<Guid> listaSuscripciones = new List<Guid>();
                        gestorUsuarios.GestorSuscripciones.SuscripcionDW.Merge(suscripcionDW);

                        foreach (AD.EntityModel.Models.Suscripcion.Suscripcion filaSuscripcion in suscripcionDW.ListaSuscripcion)
                        {
                            if (!listaSuscripciones.Contains(filaSuscripcion.SuscripcionID))
                            {
                                listaSuscripciones.Add(filaSuscripcion.SuscripcionID);
                            }
                        }
                        gestorUsuarios.GestorSuscripciones.GestorNotificaciones.NotificacionDW.Merge(notificacionCN.ObtenerNotificacionesDeSuscripciones(listaSuscripciones));

                        gestorUsuarios.GestorSuscripciones.EliminarSuscripciones(identidadID);
                        listaSuscripciones.Clear();
                    }

                    #endregion

                    mEntityContext.SaveChanges();
                }

                ControladorPersonas controladorPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorIdentidades.GestorPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorID(gestorIdentidades.ListaIdentidades[identidadID].PersonaID.Value), mLoggingService, mEntityContext);
                gestorIdentidades.GestorPersonas.CargarGestor();
                // Lo paso a true para marcarla como privada, así sílo se le muestra al administrador para que pueda readmitirlo
                controladorPers.ActualizarModeloBASE(gestorIdentidades.ListaIdentidades[identidadID], proyectoID, true, false, PrioridadBase.Alta);

                #endregion
            }

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (pIdentidad.PersonaID.HasValue)
            {
                identidadCL.EliminarCacheGestorIdentidad(pIdentidad.Clave, pIdentidad.PersonaID.Value);
            }
            identidadCL.EliminarPerfilMVC(pIdentidad.PerfilID);
            identidadCL.Dispose();

            //Agregamos el evento a la cola del live

            try
            {
                InsertarFilaEnColaRabbitMQ(pProyecto.Clave, pIdentidad.PerfilID, (int)AccionLive.Eliminado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                liveDS.Cola.AddColaRow(pProyecto.Clave, pIdentidad.PerfilID, (int)AccionLive.Eliminado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta, null);
            }


            liveCN.ActualizarBD(liveDS);

            liveDS.Dispose();
        }

        /// <summary>
        /// Obtiene atraves de un guid y el ParametroGeneralDS la fila de parametros generales del proyecto correspondiente
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar</param>
        /// <returns>Fila del parametro general</returns>
        public ParametroGeneral ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        {
            ParametroGeneral filaParametroGeneral = null;
            //ParametroGeneralDS paramDS;            
            ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            //paramDS = paramCL.ObtenerParametrosGeneralesDeProyecto(pProyectoID);
            //paramCL.Dispose();
            GestorParametroGeneral gestorParametroGeneral = paramCL.ObtenerParametrosGeneralesDeProyecto(pProyectoID);

            // List<ParametroGeneral> listaParametrosGenerales = gestorParametroGeneral.ListaParametroGeneral;
            if (gestorParametroGeneral != null && gestorParametroGeneral.ListaParametroGeneral.FirstOrDefault() != null)
            {
                // filaParametroGeneral = (ParametroGeneralDSName.ParametroGeneralRow)paramDS.ParametroGeneral.Select("ProyectoID = '" + pProyectoID + "'")[0];
                filaParametroGeneral = gestorParametroGeneral.ListaParametroGeneral[0];
            }
            return filaParametroGeneral;
        }

        /// <summary>
        /// Obtiene un parámetro de tipo bool de la lista de parámetros
        /// </summary>
        /// <param name="pListaParametrosProyecto">Lista de parámetros</param>
        /// <param name="pNombreParametro">Nombre del parámetro</param>
        /// <param name="pValorPorDefecto">Valor por defecto del parámetro</param>
        /// <returns></returns>
        public static bool ObtenerParametroBooleano(Dictionary<string, string> pListaParametrosProyecto, string pNombreParametro, bool pValorPorDefecto = false)
        {
            if (pListaParametrosProyecto.ContainsKey(pNombreParametro))
            {
                if (pListaParametrosProyecto[pNombreParametro] == "1" || pListaParametrosProyecto[pNombreParametro] == "true")
                {
                    return true;
                }
                else if (pListaParametrosProyecto[pNombreParametro] == "0" || pListaParametrosProyecto[pNombreParametro] == "false")
                {
                    return false;
                }
            }
            return pValorPorDefecto;
        }

        /// <summary>
        /// Obtiene un parámetro de tipo String de la lista de parámetros
        /// </summary>
        /// <param name="pListaParametrosProyecto">Lista de parámetros</param>
        /// <param name="pNombreParametro">Nombre del parámetro</param>
        /// <returns></returns>
        public static string ObtenerParametroString(Dictionary<string, string> pListaParametrosProyecto, string pNombreParametro)
        {
            if (pListaParametrosProyecto.ContainsKey(pNombreParametro))
            {
                return pListaParametrosProyecto[pNombreParametro];
            }
            return "";
        }

        /// <summary>
        /// Obtiene un parámetro de tipo Int de la lista de parámetros (0 si no lo ha encontrado)
        /// </summary>
        /// <param name="pListaParametrosProyecto">Lista de parámetros</param>
        /// <param name="pNombreParametro">Nombre del parámetro</param>
        /// <returns></returns>
        public static int ObtenerParametroInt(Dictionary<string, string> pListaParametrosProyecto, string pNombreParametro)
        {
            int aux = 0;
            if (pListaParametrosProyecto.ContainsKey(pNombreParametro))
            {
                int.TryParse(pListaParametrosProyecto[pNombreParametro], out aux);
            }

            return aux;
        }

        /// <summary>
        /// Guarda un parámetro booleano en el data set de parametro general
        /// </summary>
        /// <param name="pParametrosGeneralesDS">Data set de parámetro general</param>
        /// <param name="pNombreParametro">Nombre del parámetro</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pValorPorDefecto">Valor por defecto</param>
        public void GuardarParametroBooleano(GestorParametroGeneral pParametrosGeneralesDS, string pNombreParametro, bool pValor, bool pValorPorDefecto = false)
        {
            if (pValor != pValorPorDefecto)
            {
                GuardarParametroString(pParametrosGeneralesDS, pNombreParametro, pValor ? "1" : "0");
            }
            else
            {
                GuardarParametroString(pParametrosGeneralesDS, pNombreParametro, null);
            }
        }

        /// <summary>
        /// Guarda un parámetro string en el data set de parámetro general
        /// </summary>
        /// <param name="pGestorParametroGeneral">Data set de parámetro general</param>
        /// <param name="pNombreParametro">Nombre del parámetro</param>
        /// <param name="pValor">Valor</param>
        public void GuardarParametroString(GestorParametroGeneral pGestorParametroGeneral, string pNombreParametro, string pValor)
        {
            ParametroProyecto filaParametro = pGestorParametroGeneral.ListaParametroProyecto.Where(parametro => parametro.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametro.ProyectoID.Equals(ProyectoSeleccionado.Clave) && parametro.Parametro.Equals(pNombreParametro)).FirstOrDefault();
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
            
            if (!string.IsNullOrEmpty(pValor))
            {
                if (filaParametro != null && !filaParametro.Valor.Equals(pValor))
                {
                    // El parametro existe, lo modifico
                    filaParametro.Valor = pValor;
                }
                else if (filaParametro == null)
                {
                    // La fila no existe, la creo
                    ParametroProyecto parametro = new ParametroProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, pNombreParametro, pValor);
                    pGestorParametroGeneral.ListaParametroProyecto.Add(parametro);
                    gestorController.NuevoParametroProyecto(parametro);
                }
            }
            else if (filaParametro != null)
            {
                // El valor es null, elimino el parámetro
                gestorController.EliminarParametroProyecto(filaParametro);
                pGestorParametroGeneral.ListaParametroProyecto.Remove(filaParametro);
            }
        }

        /// <summary>
        /// Carga los administradores de un proyecto
        /// </summary>
        /// <param name="pProyecto">Proyecto del que se quieren cargar los administradores</param>
        /// <returns>Lista con los administradores</returns>
        public List<Identidad> CargarAdministradoresProyecto(Elementos.ServiciosGenerales.Proyecto pProyecto)
        {
            List<Identidad> listaUsuarioDef = new List<Identidad>();
            try
            {
                //Obtenemos la lista de administradores del proyecto
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<Guid> listaGuidIdentidades = proyCN.ObtenerListaIdentidadesAdministradoresPorProyecto(pProyecto.Clave);

                //Obtenemos las personas y las organizaciones de esas personas
                PersonaCN PersonaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionPersonas gestorPersonas = new GestionPersonas(PersonaCN.ObtenerPersonasPorIdentidadesCargaLigera(listaGuidIdentidades), mLoggingService, mEntityContext);
                gestorPersonas.GestorUsuarios = new GestionUsuarios(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                List<Guid> listapersonasID = new List<Guid>();
                foreach (Guid personaID in gestorPersonas.ListaPersonas.Keys)
                {
                    listapersonasID.Add(personaID);
                }

                OrganizacionCN OrganizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionOrganizaciones gestorOrg = new GestionOrganizaciones(OrganizacionCN.ObtenerOrganizacionesDeListaPersona(listapersonasID), mLoggingService, mEntityContext);

                List<Guid> listaOrganizacionesID = new List<Guid>();
                foreach (Guid organizacionID in gestorOrg.ListaOrganizaciones.Keys)
                {
                    listaOrganizacionesID.Add(organizacionID);
                }

                //Obtenemos las identidades de los administradores y de las organizaciones de esos administradores
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperIdentidad dataWrapperIdentidad = identCN.ObtenerIdentidadesPorID(listaGuidIdentidades, true);

                if (listaOrganizacionesID.Count > 0)
                {
                    dataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesDeOrganizaciones(listaOrganizacionesID, pProyecto.Clave, TiposIdentidad.Organizacion));
                }

                GestionIdentidades GestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, gestorPersonas, gestorOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                SortedDictionary<string, List<Identidad>> listaUsuarios = new SortedDictionary<string, List<Identidad>>();
                List<Guid> listaOrganizacionesAdministradoras = new List<Guid>();

                foreach (Guid identidadID in listaGuidIdentidades)
                {
                    if (GestorIdentidades.ListaIdentidades.ContainsKey(identidadID))
                    {
                        if (GestorIdentidades.ListaIdentidades[identidadID].ModoParticipacion == TiposIdentidad.Personal || GestorIdentidades.ListaIdentidades[identidadID].ModoParticipacion == TiposIdentidad.Profesor || (GestorIdentidades.ListaIdentidades[identidadID].PerfilUsuario.FilaPerfil.OrganizacionID.HasValue && !listaOrganizacionesAdministradoras.Contains(GestorIdentidades.ListaIdentidades[identidadID].PerfilUsuario.FilaPerfil.OrganizacionID.Value)))
                        {
                            Identidad identidadParaAgregar = GestorIdentidades.ListaIdentidades[identidadID];

                            GestorIdentidades.ListaIdentidades[identidadID].NumeroRecursosCompartidos = "-1";
                            string nombreIdentidad = null;

                            if (GestorIdentidades.ListaIdentidades[identidadID].ModoParticipacion == TiposIdentidad.Personal || GestorIdentidades.ListaIdentidades[identidadID].ModoParticipacion == TiposIdentidad.Profesor)
                            {
                                nombreIdentidad = GestorIdentidades.ListaIdentidades[identidadID].Nombre();
                            }
                            else
                            {
                                nombreIdentidad = GestorIdentidades.ListaIdentidades[identidadID].NombreOrganizacion;
                                if (GestorIdentidades.ListaIdentidades[identidadID].PerfilUsuario.FilaPerfil.OrganizacionID.HasValue)
                                {
                                    listaOrganizacionesAdministradoras.Add(GestorIdentidades.ListaIdentidades[identidadID].PerfilUsuario.FilaPerfil.OrganizacionID.Value);
                                }

                                IdentidadCN identAuxCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                Guid identidadOrgID = identAuxCN.ObtenerIdentidadIDDeOrganizacionEnProyecto(GestorIdentidades.ListaIdentidades[identidadID].PerfilUsuario.NombreCortoOrg, pProyecto.Clave);

                                if (GestorIdentidades.ListaIdentidades.ContainsKey(identidadOrgID))
                                {
                                    identidadParaAgregar = GestorIdentidades.ListaIdentidades[identidadOrgID];
                                    identidadParaAgregar.NumeroRecursosCompartidos = "-1";
                                }
                                else
                                {
                                    DataWrapperIdentidad identidadAuxDW = identAuxCN.ObtenerIdentidadPorID(identidadOrgID, true);
                                    GestionIdentidades gestorIdentidadesAux = new GestionIdentidades(identidadAuxDW, gestorPersonas, gestorOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                                    if (gestorIdentidadesAux.ListaIdentidades.Count > 0)
                                    {
                                        identidadParaAgregar = gestorIdentidadesAux.ListaIdentidades.Values[0];
                                        identidadParaAgregar.NumeroRecursosCompartidos = "-1";
                                    }
                                }
                            }

                            if (listaUsuarios.ContainsKey(nombreIdentidad))
                            {
                                listaUsuarios[nombreIdentidad].Add(identidadParaAgregar);
                            }
                            else
                            {
                                List<Identidad> listaElem = new List<Identidad>();
                                listaElem.Add(identidadParaAgregar);
                                listaUsuarios.Add(nombreIdentidad, listaElem);
                            }
                        }
                    }
                }

                foreach (List<Identidad> listaIdent in listaUsuarios.Values)
                {
                    foreach (Identidad identidad in listaIdent)
                    {
                        listaUsuarioDef.Add(identidad);
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }
            return listaUsuarioDef;
        }

        /// <summary>
        /// Carga las Personas de un proyecto
        /// </summary>
        /// <param name="pProyecto">Proyecto del que se quieren cargar las comunidades relacionadas</param>
        /// <returns>Lista con las personas relacionadas</returns>
        public List<ElementoGnoss> CargarComunidadesRelacionadasProyecto(Elementos.ServiciosGenerales.Proyecto pProyecto)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Guid> listaClavesProyectoRelacionados = new List<Guid>();

            bool manual;
            listaClavesProyectoRelacionados = proyCN.ObtenerListaProyectoRelacionados(pProyecto.Clave, out manual);
            DataWrapperProyecto dataWrapperProyecto = proyCN.ObtenerProyectosPorID(listaClavesProyectoRelacionados);
            GestionProyecto gestorProyectosRelacionados = new GestionProyecto(dataWrapperProyecto, mLoggingService, mEntityContext);
            List<ElementoGnoss> listaProyectosRelacionados = new List<ElementoGnoss>();

            int numeroProyectos = gestorProyectosRelacionados.ListaProyectos.Count;
            if (numeroProyectos > 4 && !manual)
            {
                numeroProyectos = 4;
            }


            int i = 0;
            foreach (Guid proyectoID in listaClavesProyectoRelacionados)
            {
                //Solo listaremos los 4 primeros mas Activos que sean de los proyectos relacionados con el ProyectoActual
                if (i < numeroProyectos)
                {
                    if (proyectoID != pProyecto.Clave && gestorProyectosRelacionados.ListaProyectos.ContainsKey(proyectoID))
                    {
                        Elementos.ServiciosGenerales.Proyecto proy = gestorProyectosRelacionados.ListaProyectos[proyectoID];
                        if (proy.TipoAcceso != TipoAcceso.Reservado)
                        {
                            if (!listaProyectosRelacionados.Contains(proy))
                            {
                                listaProyectosRelacionados.Add(proy);
                                i++;
                            }
                        }
                    }
                }
            }
            return listaProyectosRelacionados;
            /*}*/
        }

        /// <summary>
        /// Carga las comunidades relacionadas de un proyecto
        /// </summary>
        /// <param name="pProyecto">Proyecto del que se quieren cargar las comunidades relacionadas</param>
        /// <param name="pNumeroMiembros">Número de personas que se deben obtener</param>
        /// <param name="pOrdenarPorFechaAlta">Indica si se debe ordenar por fecha de alta (TRUE) o por número de recursos(FALSE)</param>
        /// <returns>Lista con las comunidades relacionadas</returns>
        public Dictionary<Identidad, int> CargarPersonasProyecto(Elementos.ServiciosGenerales.Proyecto pProyecto, int pNumeroMiembros, bool pOrdenarPorFechaAlta)
        {
            Dictionary<Identidad, int> listaIdentidades = new Dictionary<Identidad, int>();

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionIdentidades gestIdent = new GestionIdentidades(identidadCN.ObtenerMiembrosDeProyectoParaMosaico(pProyecto.Clave, pNumeroMiembros, pOrdenarPorFechaAlta), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in gestIdent.DataWrapperIdentidad.ListaIdentidad.OrderByDescending(item => item.FechaAlta))
            {
                listaIdentidades.Add(gestIdent.ListaIdentidades[filaIdent.IdentidadID], 0);
            }

            return listaIdentidades;
        }

        /// <summary>
        /// Crear un nueo proyecto completo
        /// </summary>
        /// <param name="pNombre">Nombre del proyecto</param>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <param name="pDescripcion">Descripcion del proyecto</param>
        /// <param name="pTipoAcceso">Tipo de acceso del proyecto</param>
        /// <param name="pUsuarioCreadorID">ID del usuario creador</param>
        /// <param name="pPerfilCreadorID">ID del perfil creador</param>
        /// <param name="pOrganizacionID">ID de la organizacion del proyecto padre</param>
        /// <param name="pComPrivadaPadreID">ID del proyecto padre</param>
        /// <param name="pInvitacionesDisponibles">Invitaciones disponibles</param>
        /// <param name="pPreguntasDisponibles">Preguntas disponibles</param>
        /// <param name="pEncuestasDisponibles">Encuestas disponibles</param>
        /// <param name="pDebatesDisponibles">Debates disponibles</param>
        /// <param name="pBrightcoveDisponible">Brightcove disponibles</param>
        /// <returns></returns>
        public Elementos.ServiciosGenerales.Proyecto CrearNuevoProyecto(string pNombre, string pNombreCorto, string pDescripcion, string[] pEtiquetas, short pTipoAcceso, short pTipoProyecto, Guid pUsuarioCreadorID, Guid pPerfilCreadorID, Guid pOrganizacionID, Guid pComPrivadaPadreID, bool pInvitacionesDisponibles, bool pPreguntasDisponibles, bool pEncuestasDisponibles, bool pDebatesDisponibles, bool pBrightcoveDisponible, byte[] pImagenLogo, out DataWrapperOrganizacion pOrganizacionDW, out DataWrapperProyecto pProyectoDS, out GestorParametroGeneral pParametroGeneralDS, out DataWrapperTesauro pTesauroDW, out DataWrapperDocumentacion pDataWrapperDocumentacion, out DataWrapperUsuario pUsuarioDW, out DataWrapperIdentidad pIdentidadDS, string pUrlsPropias = null)
        {
            return CrearNuevoProyecto(pNombre, pNombreCorto, pDescripcion, pEtiquetas, pTipoAcceso, pTipoProyecto, pUsuarioCreadorID, pPerfilCreadorID, pOrganizacionID, pComPrivadaPadreID, pInvitacionesDisponibles, pPreguntasDisponibles, pEncuestasDisponibles, pDebatesDisponibles, pBrightcoveDisponible, pImagenLogo, out pOrganizacionDW, out pProyectoDS, out pParametroGeneralDS, out pTesauroDW, out pDataWrapperDocumentacion, out pUsuarioDW, out pIdentidadDS, true, pUrlsPropias, null);
        }

        /// <summary>
        /// Crear un nueo proyecto completo
        /// </summary>
        /// <param name="pNombre">Nombre del proyecto</param>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <param name="pDescripcion">Descripcion del proyecto</param>
        /// <param name="pTipoAcceso">Tipo de acceso del proyecto</param>
        /// <param name="pUsuarioCreadorID">ID del usuario creador</param>
        /// <param name="pPerfilCreadorID">ID del perfil creador</param>
        /// <param name="pOrganizacionID">ID de la organizacion del proyecto padre</param>
        /// <param name="pComPrivadaPadreID">ID del proyecto padre</param>
        /// <param name="pInvitacionesDisponibles">Invitaciones disponibles</param>
        /// <param name="pPreguntasDisponibles">Preguntas disponibles</param>
        /// <param name="pEncuestasDisponibles">Encuestas disponibles</param>
        /// <param name="pDebatesDisponibles">Debates disponibles</param>
        /// <param name="pBrightcoveDisponible">Brightcove disponibles</param>
        /// <returns></returns>
        public Elementos.ServiciosGenerales.Proyecto CrearNuevoProyecto(string pNombre, string pNombreCorto, string pDescripcion, string[] pEtiquetas, short pTipoAcceso, short pTipoProyecto, Guid pUsuarioCreadorID, Guid pPerfilCreadorID, Guid pOrganizacionID, Guid pComPrivadaPadreID, bool pInvitacionesDisponibles, bool pPreguntasDisponibles, bool pEncuestasDisponibles, bool pDebatesDisponibles, bool pBrightcoveDisponible, byte[] pImagenLogo, out DataWrapperOrganizacion pOrganizacionDW, out DataWrapperProyecto pDataWrapperProyecto, out GestorParametroGeneral pParametroGeneralDS, out DataWrapperTesauro pTesauroDW, out DataWrapperDocumentacion pDataWrapperDocumentacion, out DataWrapperUsuario pDataWrapperUsuario, out DataWrapperIdentidad pIdentidadDW, bool pActualizarLive, string pUrlsPropias, string pDominio)
        {
            DataWrapperOrganizacion orgaDW = new DataWrapperOrganizacion();
            //Cremos el proy en DEFINICION
            Guid organizacionID = ProyectoAD.MetaOrganizacion;
            DateTime fechaActual = System.DateTime.Now;

            if (pComPrivadaPadreID != Guid.Empty)
            {
                organizacionID = pOrganizacionID;
            }
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            GestionProyecto gestorProyectos = new GestionProyecto(dataWrapperProyecto, mLoggingService, mEntityContext);

            Guid idPadre = pComPrivadaPadreID;

            AD.EntityModel.Models.ProyectoDS.Proyecto FilaProyectoNuevo = gestorProyectos.NuevaFilaProyecto(organizacionID, idPadre);

            FilaProyectoNuevo.Nombre = pNombre;
            FilaProyectoNuevo.NombreCorto = pNombreCorto;
            FilaProyectoNuevo.Descripcion = HttpUtility.UrlDecode(pDescripcion);
            FilaProyectoNuevo.TipoAcceso = pTipoAcceso;
            FilaProyectoNuevo.TipoProyecto = pTipoProyecto;
            FilaProyectoNuevo.FechaInicio = fechaActual;

            if (pEtiquetas != null && pEtiquetas.Length > 0)
            {
                string coma = "";
                foreach (string etiqueta in pEtiquetas)
                {
                    FilaProyectoNuevo.Tags += coma + etiqueta;
                    coma = ",";
                }
            }

            if (!string.IsNullOrEmpty(pDominio))
            {
                FilaProyectoNuevo.URLPropia = pDominio;
            }
            else
            {
                if (string.IsNullOrEmpty(pUrlsPropias))
                {
                    FilaProyectoNuevo.URLPropia = ObtenerUrlPropiaProyecto(pTipoAcceso);
                }
                else
                {
                    FilaProyectoNuevo.URLPropia = pUrlsPropias;
                }
            }
            
            
            ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD(mEntityContext);

            proyectoGBD.AddProyecto(FilaProyectoNuevo);
            dataWrapperProyecto.ListaProyecto.Add(FilaProyectoNuevo);

            #region Pestañas
            //Comunidades Padre
            if (idPadre == Guid.Empty)
            {
                //Home
                Guid idHome = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idHome, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Home, null, null, 0, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idHome.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idHome, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Home, null, null, 0, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idHome.ToString(), true, "", null, true, "");
                //Indice
                Guid idIndice = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idIndice, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Indice, null, null, 1, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idIndice.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idIndice, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Indice, null, null, 1, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idIndice.ToString(), true, "", null, true, "");
                //Recursos
                Guid idRecursos = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idRecursos, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Recursos, null, null, 3, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idRecursos.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idRecursos, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Recursos, null, null, 3, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idRecursos.ToString(), true, "", null, true, "");
                //Debates
                Guid idDebates = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idDebates, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Debates, null, null, 4, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idDebates.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idDebates, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Debates, null, null, 4, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idDebates.ToString(), true, "", null, true, "");
                //Preguntas
                Guid idPreguntas = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idPreguntas, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Preguntas, null, null, 5, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idPreguntas.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idPreguntas, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Preguntas, null, null, 5, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idPreguntas.ToString(), true, "", null, true, "");
                //Encuestas
                Guid idEncuestas = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idEncuestas, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Encuestas, null, null, 6, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idEncuestas.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idEncuestas, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.Encuestas, null, null, 6, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idEncuestas.ToString(), true, "", null, true, "");
                //Personas y organizaciones
                Guid idPersonas = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idPersonas, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.PersonasYOrganizaciones, "", "", 7, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idPersonas.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idPersonas, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.PersonasYOrganizaciones, "", "", 7, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idPersonas.ToString(), true, "", null, true, "");
                //Acerca-de
                Guid idAcerca = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idAcerca, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.AcercaDe, null, null, 8, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idAcerca.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idAcerca, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.AcercaDe, null, null, 8, false, true, (short)TipoPrivacidadPagina.Normal, null, null, null, idAcerca.ToString(), true, "", null, true, "");
                //Busqueda-avanzada
                Guid idBusqueda = Guid.NewGuid();
                dataWrapperProyecto.AddProyectoPestanyaMenuRow(idBusqueda, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.BusquedaAvanzada, null, null, 9, false, false, (short)TipoPrivacidadPagina.Normal, null, null, null, idBusqueda.ToString(), true, "", null, true, "");
                proyectoGBD.AddProyectoPestanyaMenuRow(idBusqueda, organizacionID, FilaProyectoNuevo.ProyectoID, null, (short)TipoPestanyaMenu.BusquedaAvanzada, null, null, 9, false, false, (short)TipoPrivacidadPagina.Normal, null, null, null, idBusqueda.ToString(), true, "", null, true, "");
            }
            proyectoGBD.GuardarCambios();
            #endregion

            //Creamos las filas para el administrador del proyecto
            #region Administrador del proyecto

            //Añado el usuario al gestor
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = usuarioCN.ObtenerUsuarioPorID(pUsuarioCreadorID);
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
            dataWrapperUsuario.ListaUsuario.Add(filaUsuario);
            GestionUsuarios gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);

            //Creo la identidad
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerPerfilesDeUsuario(pUsuarioCreadorID);
            gestorUsuarios.GestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            Perfil perfil = new Perfil(gestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Find(perf => perf.PerfilID.Equals(pPerfilCreadorID)), gestorUsuarios.GestorIdentidades, mLoggingService);

            if (perfil.OrganizacionID.HasValue)
            {
                gestorUsuarios.GestorIdentidades.GestorOrganizaciones = new GestionOrganizaciones(new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerOrganizacionPorID(perfil.OrganizacionID.Value), mLoggingService, mEntityContext);
            }

            if (perfil.PersonaID.HasValue)
            {
                gestorUsuarios.GestorIdentidades.GestorPersonas = new GestionPersonas(new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerPersonaPorIDCargaLigera(perfil.PersonaID.Value), mLoggingService, mEntityContext);
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();

            //Añado la fila de ProyectoRolusuario para ese usuario
            AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario proyectoRolUsuario = new AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario();
            proyectoRolUsuario.OrganizacionGnossID = FilaProyectoNuevo.OrganizacionID;
            proyectoRolUsuario.ProyectoID = FilaProyectoNuevo.ProyectoID;
            proyectoRolUsuario.UsuarioID = pUsuarioCreadorID;
            proyectoRolUsuario.RolPermitido = UsuarioAD.FilaPermisosAdministrador;
            proyectoRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
            proyectoRolUsuario.EstaBloqueado = false;
            gestorUsuarios.DataWrapperUsuario.ListaProyectoRolUsuario.Add(proyectoRolUsuario);
            mEntityContext.ProyectoRolUsuario.Add(proyectoRolUsuario);

            Identidad identidad = new ControladorIdentidades(gestorUsuarios.GestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarIdentidadPerfilYUsuarioAProyecto(gestorUsuarios.GestorIdentidades, gestorUsuarios, organizacionID, FilaProyectoNuevo.ProyectoID, filaUsuario, perfil, recibirNewsletterDefectoProyectos);

            LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            LiveDS liveDS = new LiveDS();

            try
            {
                InsertarFilaEnColaRabbitMQ(FilaProyectoNuevo.ProyectoID, identidad.PerfilUsuario.Clave, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, identidad.FilaIdentidad.FechaAlta, false, (short)PrioridadLive.Alta, null);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                liveDS.Cola.AddColaRow(FilaProyectoNuevo.ProyectoID, identidad.PerfilUsuario.Clave, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, identidad.FilaIdentidad.FechaAlta, false, (short)PrioridadLive.Alta, null);
            }


            //liveDS.ColaHomePerfil.AddColaHomePerfilRow(FilaProyectoNuevo.ProyectoID, identidad.PerfilUsuario.Clave, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, identidad.FilaIdentidad.FechaAlta, (short)PrioridadLive.Alta);

            //Creo fila en "HistoricoProyectoUsuario"
            AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario filaHistoricoProyectoUsuario = new AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario();
            filaHistoricoProyectoUsuario.IdentidadID = identidad.Clave;
            filaHistoricoProyectoUsuario.UsuarioID = pUsuarioCreadorID;
            filaHistoricoProyectoUsuario.OrganizacionGnossID = FilaProyectoNuevo.OrganizacionID;
            filaHistoricoProyectoUsuario.ProyectoID = FilaProyectoNuevo.ProyectoID;
            filaHistoricoProyectoUsuario.FechaEntrada = fechaActual;
            filaHistoricoProyectoUsuario.FechaSalida = null;

            gestorUsuarios.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Add(filaHistoricoProyectoUsuario);
            mEntityContext.HistoricoProyectoUsuario.Add(filaHistoricoProyectoUsuario);

            //Creo la fila "AdministradorProyecto"
            //gestorProyectos.DataWrapperProyecto.AddAdministradorProyectoRow(FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID, pUsuarioCreadorID, (short)TipoRolUsuario.Administrador);
            AdministradorProyecto adminProy = new AdministradorProyecto();
            adminProy.OrganizacionID = FilaProyectoNuevo.OrganizacionID;
            adminProy.ProyectoID = FilaProyectoNuevo.ProyectoID;
            adminProy.UsuarioID = pUsuarioCreadorID;
            adminProy.Tipo = (short)TipoRolUsuario.Administrador;
            gestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.Add(adminProy);
            proyectoGBD.AddAdministradorProyecto(adminProy);
            proyectoGBD.GuardarCambios();
            //Ha seleccionado un perfil de organización, habrá que meter a la organización del perfil para que participe en el proyecto

            if (perfil.OrganizacionID.HasValue && perfil.PersonaID.HasValue)
            {
                AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg perfilPersonaOrg = dataWrapperIdentidad.ListaPerfilPersonaOrg.Find(perfilPersOrg => perfilPersOrg.PersonaID.Equals(perfil.PersonaID.Value) && perfilPersOrg.OrganizacionID.Equals(perfil.OrganizacionID.Value) && perfilPersOrg.PerfilID.Equals(pPerfilCreadorID));
                if (perfilPersonaOrg != null)
                {
                    Guid OrganizacionIDSeleccionada = perfilPersonaOrg.OrganizacionID;

                    #region Le concedo acceso a la organizacion para que participe en el proyecto

                    IdentidadCN identiCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperIdentidad identiDW = identiCN.ObtenerPerfilDeOrganizacion(OrganizacionIDSeleccionada);

                    OrganizacionCN orgaCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    orgaDW.ListaOrganizacion.Add(orgaCN.ObtenerNombreOrganizacionPorID(OrganizacionIDSeleccionada));
                    string nombreOrg = (orgaDW.ListaOrganizacion.Where(item => item.OrganizacionID.Equals(OrganizacionIDSeleccionada)).Select(item => item.Alias).FirstOrDefault());

                    //Organizacion sin perfil, hay que crearselo
                    //"OrganizacionID ='" + OrganizacionIDSeleccionada + "'"
                    if (!identiDW.ListaPerfilOrganizacion.Any(perfilOrg => perfilOrg.OrganizacionID.Equals(OrganizacionIDSeleccionada)))
                    {
                        //Creo filas "Perfil"
                        AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = new AD.EntityModel.Models.IdentidadDS.Perfil();
                        filaPerfil.Eliminado = false;
                        filaPerfil.NombreOrganizacion = nombreOrg;
                        filaPerfil.NombrePerfil = nombreOrg;
                        filaPerfil.PerfilID = Guid.NewGuid();
                        filaPerfil.OrganizacionID = OrganizacionIDSeleccionada;
                        filaPerfil.CaducidadResSusc = 0;

                        gestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Add(filaPerfil);
                        mEntityContext.Perfil.Add(filaPerfil);

                        //Creo "PerfilOrganizacion" 
                        AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion filaPerfilOrganizacion = new AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion();
                        filaPerfilOrganizacion.PerfilID = filaPerfil.PerfilID;
                        filaPerfilOrganizacion.OrganizacionID = OrganizacionIDSeleccionada;

                        gestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilOrganizacion.Add(filaPerfilOrganizacion);
                        mEntityContext.PerfilOrganizacion.Add(filaPerfilOrganizacion);
                    }
                    //Tenia perfil, lo agrego al gestor y lo uso
                    else
                    {
                        gestorUsuarios.GestorIdentidades.DataWrapperIdentidad.Merge(identiDW);
                    }
                    gestorUsuarios.GestorIdentidades.RecargarHijos();
                    Guid ClavePerfil = (Guid)gestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaPerfilOrganizacion.Where(item => item.OrganizacionID.Equals(OrganizacionIDSeleccionada)).Select(item => item.PerfilID).FirstOrDefault();
                    if (!gestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(ident => ident.PerfilID.Equals(ClavePerfil) && ident.ProyectoID.Equals(FilaProyectoNuevo.ProyectoID)))
                    {
                        //Organizacion sin identidad en este proyecto, hay que crearla
                        //Creo fila "Identidad"
                        Identidad identidadOrg = gestorUsuarios.GestorIdentidades.AgregarIdentidadPerfil(gestorUsuarios.GestorIdentidades.ListaPerfiles[ClavePerfil], FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID, recibirNewsletterDefectoProyectos);
                    }
                    //"PerfilID ='" + ClavePerfil + "' AND ProyectoID = '" + FilaProyectoNuevo.ProyectoID + "'"
                    Guid IdentidadOrgID = (Guid)gestorUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(ClavePerfil) && ident.ProyectoID.Equals(FilaProyectoNuevo.ProyectoID)).Select(ident => ident.IdentidadID).FirstOrDefault();

                    //Creo fila "OrganizacionParticipaProy" 
                    OrganizacionParticipaProy filaOrganizacionParticipaProy = new OrganizacionParticipaProy();
                    filaOrganizacionParticipaProy.FechaInicio = fechaActual;
                    filaOrganizacionParticipaProy.IdentidadID = IdentidadOrgID;
                    filaOrganizacionParticipaProy.OrganizacionID = OrganizacionIDSeleccionada;
                    filaOrganizacionParticipaProy.OrganizacionProyectoID = FilaProyectoNuevo.OrganizacionID;
                    filaOrganizacionParticipaProy.ProyectoID = FilaProyectoNuevo.ProyectoID;
                    filaOrganizacionParticipaProy.EstaBloqueada = false;
                    filaOrganizacionParticipaProy.RegistroAutomatico = 0;

                    orgaDW.ListaOrganizacionParticipaProy.Add(filaOrganizacionParticipaProy);
                    mEntityContext.OrganizacionParticipaProy.Add(filaOrganizacionParticipaProy);

                    if (!perfil.IdentidadMyGNOSS.EsIdentidadProfesor)
                    {
                        //El administrador participa en modo corporativo
                        identidad.FilaIdentidad.Tipo = (short)TiposIdentidad.ProfesionalCorporativo;
                        identidad.FilaIdentidad.NombreCortoIdentidad = nombreOrg;
                    }

                    //Actualizo el modelo Live
                    try
                    {
                        InsertarFilaEnColaRabbitMQ(FilaProyectoNuevo.ProyectoID, ClavePerfil, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'");
                        liveDS.Cola.AddColaRow(FilaProyectoNuevo.ProyectoID, ClavePerfil, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, false, (short)PrioridadLive.Alta, null);
                    }

                    //liveDS.ColaHomePerfil.AddColaHomePerfilRow(FilaProyectoNuevo.ProyectoID, ClavePerfil, (int)AccionLive.Agregado, (int)TipoLive.Miembro, 0, DateTime.Now, (short)PrioridadLive.Alta);

                    #endregion;
                }
            }
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (identidad.PersonaID.HasValue)
            {
                identidadCL.EliminarCacheGestorIdentidad(identidad.Clave, identidad.PersonaID.Value);
            }
            identidadCL.EliminarPerfilMVC(identidad.PerfilID);
            identidadCL.Dispose();

            #endregion

            #region Parametro General
            //Creamos los datos de parámetros generales        
            GestorParametroGeneral paramGenDS = new GestorParametroGeneral();

            gestorProyectos.ParametroGeneralDS = paramGenDS;

            //ParametroGeneralDSName.ParametroGeneralRow filaParametrosGenerales = gestorProyectos.NuevaFilaParametros(organizacionID, FilaProyectoNuevo.ProyectoID);
            ParametroGeneral filaParametrosGenerales = new ParametroGeneral(organizacionID, FilaProyectoNuevo.ProyectoID);
            filaParametrosGenerales.InvitacionesDisponibles = pInvitacionesDisponibles;
            filaParametrosGenerales.DebatesDisponibles = pDebatesDisponibles;
            filaParametrosGenerales.EncuestasDisponibles = pEncuestasDisponibles;
            filaParametrosGenerales.PreguntasDisponibles = pPreguntasDisponibles;
            filaParametrosGenerales.PlataformaVideoDisponible = (short)PlataformaVideoDisponible.Ninguna;
            filaParametrosGenerales.IdiomaDefecto = "es";
            filaParametrosGenerales.NumeroRecursosRelacionados = 5;
            filaParametrosGenerales.FechaNacimientoObligatoria = true;
            filaParametrosGenerales.SolicitarCoockieLogin = true;
            filaParametrosGenerales.CMSDisponible = true;
            if (pTipoProyecto.Equals((short)TipoProyecto.EducacionExpandida) || pTipoProyecto.Equals((short)TipoProyecto.EducacionPrimaria))
            {
                filaParametrosGenerales.ComunidadGNOSS = true;
            }
            if (pBrightcoveDisponible)
            {
                filaParametrosGenerales.PlataformaVideoDisponible = (short)PlataformaVideoDisponible.Brightcove;
            }

            if (FilaProyectoNuevo.TipoAcceso == (short)TipoAcceso.Publico || FilaProyectoNuevo.TipoAcceso == (short)TipoAcceso.Restringido)
            {
                filaParametrosGenerales.RdfDisponibles = true;
                filaParametrosGenerales.RssDisponibles = true;
            }

            filaParametrosGenerales.LogoProyecto = pImagenLogo;

            filaParametrosGenerales.CodigoGoogleAnalytics = ObtenerCodigoGoogleAnalyticsProyecto(pTipoAcceso);

            if (pImagenLogo != null && pImagenLogo.Length > 0)
            {
                SixLabors.ImageSharp.Image imagen = UtilImages.ConvertirArrayBytesEnImagen(pImagenLogo);
                filaParametrosGenerales.CoordenadasSup = "[ 0, 0, " + imagen.Width.ToString() + ", " + imagen.Height.ToString() + " ]";
            }

            if (ProyectoPrincipalUnico != ProyectoAD.MetaProyecto)
            {
                ParametroGeneral filaParametrosGeneralesProyUnico = ParametrosGenerales;
                if (filaParametrosGeneralesProyUnico == null || FilaProyectoNuevo.ProyectoID != ProyectoPrincipalUnico)
                {
                    filaParametrosGeneralesProyUnico = ObtenerFilaParametrosGeneralesDeProyecto(ProyectoPrincipalUnico);
                }

                filaParametrosGenerales.MostrarAccionesEnListados = filaParametrosGeneralesProyUnico.MostrarAccionesEnListados;

                filaParametrosGenerales.VotacionesDisponibles = filaParametrosGeneralesProyUnico.VotacionesDisponibles;
                filaParametrosGenerales.PermitirVotacionesNegativas = filaParametrosGeneralesProyUnico.PermitirVotacionesNegativas;
                filaParametrosGenerales.VerVotaciones = filaParametrosGeneralesProyUnico.VerVotaciones;

                filaParametrosGenerales.ComentariosDisponibles = filaParametrosGeneralesProyUnico.ComentariosDisponibles;

                filaParametrosGenerales.CompartirRecursosPermitido = filaParametrosGeneralesProyUnico.CompartirRecursosPermitido;

            }
            mEntityContext.ParametroGeneral.Add(filaParametrosGenerales);
            paramGenDS.ListaParametroGeneral.Add(filaParametrosGenerales);
            #endregion

            #region Tesauro
            //Creamos los datos de tesauro necesarios 
            DataWrapperTesauro tesauroDW = new DataWrapperTesauro();
            AD.EntityModel.Models.Tesauro.Tesauro filaTesauro = new AD.EntityModel.Models.Tesauro.Tesauro();
            filaTesauro.TesauroID = Guid.NewGuid();
            tesauroDW.ListaTesauro.Add(filaTesauro);
            mEntityContext.Tesauro.Add(filaTesauro);

            AD.EntityModel.Models.Tesauro.TesauroProyecto filaTesauroProyecto = new AD.EntityModel.Models.Tesauro.TesauroProyecto();
            filaTesauroProyecto.OrganizacionID = FilaProyectoNuevo.OrganizacionID;
            filaTesauroProyecto.ProyectoID = FilaProyectoNuevo.ProyectoID;
            filaTesauroProyecto.Tesauro = filaTesauro;
            tesauroDW.ListaTesauroProyecto.Add(filaTesauroProyecto);
            mEntityContext.TesauroProyecto.Add(filaTesauroProyecto);
            #endregion

            #region Base de recursos
            //Creamos los datos de Base de recursos necesarios 
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            AD.EntityModel.Models.Documentacion.BaseRecursos filaBaseRecursos = new AD.EntityModel.Models.Documentacion.BaseRecursos();
            filaBaseRecursos.BaseRecursosID = Guid.NewGuid();
            dataWrapperDocumentacion.ListaBaseRecursos.Add(filaBaseRecursos);
            mEntityContext.BaseRecursos.Add(filaBaseRecursos);

            AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBaseRecursosProyecto = new AD.EntityModel.Models.Documentacion.BaseRecursosProyecto();
            filaBaseRecursosProyecto.BaseRecursos = filaBaseRecursos;
            filaBaseRecursosProyecto.ProyectoID = FilaProyectoNuevo.ProyectoID;
            filaBaseRecursosProyecto.OrganizacionID = FilaProyectoNuevo.OrganizacionID;
            dataWrapperDocumentacion.ListaBaseRecursosProyecto.Add(filaBaseRecursosProyecto);
            mEntityContext.BaseRecursosProyecto.Add(filaBaseRecursosProyecto);
            #endregion

            //Actualizamos el número de organizaciones que pueden estar registradas en el nuevo proyecto (por el tema de los perfiles de los administradores)
            AD.EntityModel.Models.ProyectoDS.Proyecto filaProyecto = gestorProyectos.DataWrapperProyectos.ListaProyecto.FirstOrDefault(proyect => proyect.OrganizacionID.Equals(FilaProyectoNuevo.OrganizacionID) && proyect.ProyectoID.Equals(FilaProyectoNuevo.ProyectoID));//FindByOrganizacionIDProyectoID(FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID);

            //Creamos los permisos de las utilidades
            CrearPermisosUtilidades(gestorProyectos, FilaProyectoNuevo);

            //Actualizamos la tabla de proyectos mas activos insertando el proyecto nuevo con valores por defecto (por el tema de que un proyecto recien creado aparezca en la lista de las comunidades web )
            ProyectosMasActivos filaProyectoMasActivo = new ProyectosMasActivos();
            filaProyectoMasActivo.Nombre = filaProyecto.Nombre;
            filaProyectoMasActivo.NumeroConsultas = 0;
            filaProyectoMasActivo.OrganizacionID = filaProyecto.OrganizacionID;
            filaProyectoMasActivo.ProyectoID = filaProyecto.ProyectoID;
            filaProyectoMasActivo.Peso = 0;

            if (gestorProyectos.DataWrapperProyectos.ListaProyectosMasActivos.FirstOrDefault(proyMasActivos => proyMasActivos.OrganizacionID.Equals(filaProyecto.OrganizacionID) && proyMasActivos.ProyectoID.Equals(filaProyecto.ProyectoID)) == null)
            {
                gestorProyectos.DataWrapperProyectos.ListaProyectosMasActivos.Add(filaProyectoMasActivo);
                proyectoGBD.AddProyectoMasActivo(filaProyectoMasActivo);
                proyectoGBD.GuardarCambios();
            }
            pOrganizacionDW = orgaDW;
            pDataWrapperProyecto = gestorProyectos.DataWrapperProyectos;
            pParametroGeneralDS = gestorProyectos.ParametroGeneralDS;
            pTesauroDW = tesauroDW;
            pDataWrapperDocumentacion = dataWrapperDocumentacion;
            pDataWrapperUsuario = gestorUsuarios.DataWrapperUsuario;
            pIdentidadDW = gestorUsuarios.GestorIdentidades.DataWrapperIdentidad;

            #region Creo lo Gadgets por defecto
            dataWrapperProyecto.AddProyectoGadgetRow(FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID, Guid.NewGuid(), UtilIdiomas.GetText("COMADMININFOGENERAL", "TIPOGADLOMASINTERESANTE"), "", 0, (short)TipoGadget.LoMasInteresante, "", null, (short)TipoUbicacionGadget.LateralHomeComunidad, true, false, Guid.Empty, false, "", "lomasinteresante");
            proyectoGBD.AddProyectoGadget(FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID, Guid.NewGuid(), UtilIdiomas.GetText("COMADMININFOGENERAL", "TIPOGADLOMASINTERESANTE"), " ", 0, (short)TipoGadget.LoMasInteresante, " ", null, (short)TipoUbicacionGadget.LateralHomeComunidad, true, false, Guid.Empty, false, "", "lomasinteresante");
            //proyectoGBD.GuardarCambios();
            dataWrapperProyecto.AddProyectoGadgetRow(FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID, Guid.NewGuid(), UtilIdiomas.GetText("COMADMININFOGENERAL", "TIPOGADPROYRELACIONADOS"), "", 1, (short)TipoGadget.ProyRelacionados, "", null, (short)TipoUbicacionGadget.LateralHomeComunidad, true, false, Guid.Empty, false, "", "comunidadesrelacionadas");
            proyectoGBD.AddProyectoGadget(FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID, Guid.NewGuid(), UtilIdiomas.GetText("COMADMININFOGENERAL", "TIPOGADPROYRELACIONADOS"), " ", 1, (short)TipoGadget.ProyRelacionados, " ", null, (short)TipoUbicacionGadget.LateralHomeComunidad, true, false, Guid.Empty, false, "", "comunidadesrelacionadas");
            // proyectoGBD.GuardarCambios();
            dataWrapperProyecto.AddProyectoGadgetRow(FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID, Guid.NewGuid(), UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOSFICHA", "TEPUEDEINTERESAR"), "", 0, (short)TipoGadget.RecursosRelacionados, "", null, (short)TipoUbicacionGadget.FichaRecursoComunidad, true, false, Guid.Empty, false, "", "tepuedeinteresar");
            proyectoGBD.AddProyectoGadget(FilaProyectoNuevo.OrganizacionID, FilaProyectoNuevo.ProyectoID, Guid.NewGuid(), UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOSFICHA", "TEPUEDEINTERESAR"), " ", 0, (short)TipoGadget.RecursosRelacionados, " ", null, (short)TipoUbicacionGadget.FichaRecursoComunidad, true, false, Guid.Empty, false, "", "tepuedeinteresar");
            proyectoGBD.GuardarCambios();

            #endregion

            Elementos.ServiciosGenerales.Proyecto proy = new Elementos.ServiciosGenerales.Proyecto(FilaProyectoNuevo, gestorProyectos, mLoggingService, mEntityContext);

            if (pActualizarLive)
            {
                liveCN.ActualizarBD(liveDS);
            }

            liveDS.Dispose();

            proy.IdentidadCreadoraProyecto = identidad;

            //Borramos la cache de las comunidades de la organizacion
            if (perfil.OrganizacionID.HasValue)
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                proyCL.InvalidarCacheProyectosOrgCargaLigeraParaFiltros(perfil.OrganizacionID.Value);
                proyCL.Dispose();
            }

            return proy;
        }

        //TODO: Esto no tiene sentido, ya que nunca se le da valor
        /// <summary>
        /// Fila con los parámetros generales del proyecto actual
        /// </summary>
        private ParametroGeneral mFilaParametros;

        /// <summary>
        /// Parámetros generales de la BD.
        /// </summary>
        public ParametroGeneral ParametrosGenerales
        {
            get
            {
                return mFilaParametros;
            }
            set
            {
                mFilaParametros = value;
            }
        }

        /// <summary>
        /// Obtiene la url propia de un proyecto en función de su tipo de acceso.
        /// </summary>
        /// <param name="pTipoAcceso">Tipo de acceso del proyecto</param>
        /// <returns>Url propia</returns>
        public string ObtenerUrlPropiaProyecto(short pTipoAcceso)
        {
            //string[] urls = ((ParametroAplicacionDS.ParametroAplicacionRow)ParametroAplicacionDS.ParametroAplicacion.Select("Parametro='" + TiposParametrosAplicacion.UrlsPropiasProyecto + "'")[0]).Valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string[] urls = ParametroAplicacionDS.Find(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.UrlsPropiasProyecto)).Valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string url in urls)
            {
                if (short.Parse(url.Substring(0, url.IndexOf("="))) == pTipoAcceso)
                {
                    return url.Substring(url.IndexOf("=") + 1);
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene el codigo de ggolge analytics de un proyecto en función de su tipo de acceso.
        /// </summary>
        /// <param name="pTipoAcceso">Tipo de acceso del proyecto</param>
        /// <returns>Codigo google analytics</returns>
        public string ObtenerCodigoGoogleAnalyticsProyecto(short pTipoAcceso)
        {
            List<Es.Riam.Gnoss.AD.EntityModel.ParametroAplicacion> busqueda = ParametroAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.CodigoGoogleAnalyticsProyecto)).ToList();
            if (busqueda.Count > 0)
            {
                //string[] urls = ((ParametroAplicacionDS.ParametroAplicacionRow)ParametroAplicacionDS.ParametroAplicacion.Select("Parametro='" + TiposParametrosAplicacion.CodigoGoogleAnalyticsProyecto + "'")[0]).Valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                string[] urls = busqueda.First().Valor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string url in urls)
                {
                    if (short.Parse(url.Substring(0, url.IndexOf("="))) == pTipoAcceso)
                    {
                        return url.Substring(url.IndexOf("=") + 1);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Establece los permisos en las utilidades para un proyecto nuevo
        /// </summary>
        /// <param name="pGestorProyecto">Gestor de proyectos</param>
        /// <param name="pProyecto">Proyecto</param>
        private void CrearPermisosUtilidades(GestionProyecto pGestorProyecto, AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProyecto)
        {
            List<TipoDocDispRolUsuarioProy> tablaPermisos = pGestorProyecto.DataWrapperProyectos.ListaTipoDocDispRolUsuarioProy;

            TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProy = new TipoDocDispRolUsuarioProy();
            tipoDocDispRolUsuarioProy.OrganizacionID = pFilaProyecto.OrganizacionID;
            tipoDocDispRolUsuarioProy.ProyectoID = pFilaProyecto.ProyectoID;
            tipoDocDispRolUsuarioProy.TipoDocumento = (short)TiposDocumentacion.Nota;
            tipoDocDispRolUsuarioProy.RolUsuario = (short)TipoRolUsuario.Usuario;
            //tablaPermisos.AddTipoDocDispRolUsuarioProyRow(pFilaProyecto.OrganizacionID, pFilaProyecto.ProyectoID, (short)TiposDocumentacion.Nota, (short)TipoRolUsuario.Usuario);
            tablaPermisos.Add(tipoDocDispRolUsuarioProy);
            bool existe = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipo => tipo.OrganizacionID.Equals(tipoDocDispRolUsuarioProy.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocDispRolUsuarioProy.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocDispRolUsuarioProy.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocDispRolUsuarioProy.RolUsuario)).ToList().Count > 0;
            if (!existe)
            {
                mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDispRolUsuarioProy);
            }

            TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProy2 = new TipoDocDispRolUsuarioProy();
            tipoDocDispRolUsuarioProy2.OrganizacionID = pFilaProyecto.OrganizacionID;
            tipoDocDispRolUsuarioProy2.ProyectoID = pFilaProyecto.ProyectoID;
            tipoDocDispRolUsuarioProy2.TipoDocumento = (short)TiposDocumentacion.Hipervinculo;
            tipoDocDispRolUsuarioProy2.RolUsuario = (short)TipoRolUsuario.Usuario;
            //tablaPermisos.AddTipoDocDispRolUsuarioProyRow(pFilaProyecto.OrganizacionID, pFilaProyecto.ProyectoID, (short)TiposDocumentacion.Hipervinculo, (short)TipoRolUsuario.Usuario);
            tablaPermisos.Add(tipoDocDispRolUsuarioProy2);
            existe = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipo => tipo.OrganizacionID.Equals(tipoDocDispRolUsuarioProy2.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocDispRolUsuarioProy2.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocDispRolUsuarioProy2.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocDispRolUsuarioProy2.RolUsuario)).ToList().Count > 0;
            if (!existe)
            {
                mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDispRolUsuarioProy2);
            }

            TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProy3 = new TipoDocDispRolUsuarioProy();
            tipoDocDispRolUsuarioProy3.OrganizacionID = pFilaProyecto.OrganizacionID;
            tipoDocDispRolUsuarioProy3.ProyectoID = pFilaProyecto.ProyectoID;
            tipoDocDispRolUsuarioProy3.TipoDocumento = (short)TiposDocumentacion.Imagen;
            tipoDocDispRolUsuarioProy3.RolUsuario = (short)TipoRolUsuario.Usuario;
            //tablaPermisos.AddTipoDocDispRolUsuarioProyRow(pFilaProyecto.OrganizacionID, pFilaProyecto.ProyectoID, (short)TiposDocumentacion.Imagen, (short)TipoRolUsuario.Usuario);
            tablaPermisos.Add(tipoDocDispRolUsuarioProy3);
            existe = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipo => tipo.OrganizacionID.Equals(tipoDocDispRolUsuarioProy3.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocDispRolUsuarioProy3.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocDispRolUsuarioProy3.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocDispRolUsuarioProy3.RolUsuario)).ToList().Count > 0;
            if (!existe)
            {
                mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDispRolUsuarioProy3);
            }

            TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProy4 = new TipoDocDispRolUsuarioProy();
            tipoDocDispRolUsuarioProy4.OrganizacionID = pFilaProyecto.OrganizacionID;
            tipoDocDispRolUsuarioProy4.ProyectoID = pFilaProyecto.ProyectoID;
            tipoDocDispRolUsuarioProy4.TipoDocumento = (short)TiposDocumentacion.FicheroServidor;
            tipoDocDispRolUsuarioProy4.RolUsuario = (short)TipoRolUsuario.Usuario;
            //tablaPermisos.AddTipoDocDispRolUsuarioProyRow(pFilaProyecto.OrganizacionID, pFilaProyecto.ProyectoID, (short)TiposDocumentacion.FicheroServidor, (short)TipoRolUsuario.Usuario);
            tablaPermisos.Add(tipoDocDispRolUsuarioProy4);
            existe = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipo => tipo.OrganizacionID.Equals(tipoDocDispRolUsuarioProy4.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocDispRolUsuarioProy4.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocDispRolUsuarioProy4.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocDispRolUsuarioProy4.RolUsuario)).ToList().Count > 0;
            if (!existe)
            {
                mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDispRolUsuarioProy4);
            }

            TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProy5 = new TipoDocDispRolUsuarioProy();
            tipoDocDispRolUsuarioProy5.OrganizacionID = pFilaProyecto.OrganizacionID;
            tipoDocDispRolUsuarioProy5.ProyectoID = pFilaProyecto.ProyectoID;
            tipoDocDispRolUsuarioProy5.TipoDocumento = (short)TiposDocumentacion.Video;
            tipoDocDispRolUsuarioProy5.RolUsuario = (short)TipoRolUsuario.Usuario;
            //tablaPermisos.AddTipoDocDispRolUsuarioProyRow(pFilaProyecto.OrganizacionID, pFilaProyecto.ProyectoID, (short)TiposDocumentacion.Video, (short)TipoRolUsuario.Usuario);
            tablaPermisos.Add(tipoDocDispRolUsuarioProy5);
            existe = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipo => tipo.OrganizacionID.Equals(tipoDocDispRolUsuarioProy5.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocDispRolUsuarioProy5.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocDispRolUsuarioProy5.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocDispRolUsuarioProy5.RolUsuario)).ToList().Count > 0;
            if (!existe)
            {
                mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDispRolUsuarioProy5);
            }

            TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProy6 = new TipoDocDispRolUsuarioProy();
            tipoDocDispRolUsuarioProy6.OrganizacionID = pFilaProyecto.OrganizacionID;
            tipoDocDispRolUsuarioProy6.ProyectoID = pFilaProyecto.ProyectoID;
            tipoDocDispRolUsuarioProy6.TipoDocumento = (short)TiposDocumentacion.Pregunta;
            tipoDocDispRolUsuarioProy6.RolUsuario = (short)TipoRolUsuario.Usuario;
            // tablaPermisos.AddTipoDocDispRolUsuarioProyRow(pFilaProyecto.OrganizacionID, pFilaProyecto.ProyectoID, (short)TiposDocumentacion.Pregunta, (short)TipoRolUsuario.Usuario);
            tablaPermisos.Add(tipoDocDispRolUsuarioProy6);
            existe = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipo => tipo.OrganizacionID.Equals(tipoDocDispRolUsuarioProy6.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocDispRolUsuarioProy6.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocDispRolUsuarioProy6.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocDispRolUsuarioProy6.RolUsuario)).ToList().Count > 0;
            if (!existe)
            {
                mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDispRolUsuarioProy6);
            }

            TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProy7 = new TipoDocDispRolUsuarioProy();
            tipoDocDispRolUsuarioProy7.OrganizacionID = pFilaProyecto.OrganizacionID;
            tipoDocDispRolUsuarioProy7.ProyectoID = pFilaProyecto.ProyectoID;
            tipoDocDispRolUsuarioProy7.TipoDocumento = (short)TiposDocumentacion.Debate;
            tipoDocDispRolUsuarioProy7.RolUsuario = (short)TipoRolUsuario.Usuario;
            //tablaPermisos.AddTipoDocDispRolUsuarioProyRow(pFilaProyecto.OrganizacionID, pFilaProyecto.ProyectoID, (short)TiposDocumentacion.Debate, (short)TipoRolUsuario.Usuario);
            tablaPermisos.Add(tipoDocDispRolUsuarioProy7);
            existe = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipo => tipo.OrganizacionID.Equals(tipoDocDispRolUsuarioProy7.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocDispRolUsuarioProy7.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocDispRolUsuarioProy7.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocDispRolUsuarioProy7.RolUsuario)).ToList().Count > 0;
            if (!existe)
            {
                mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDispRolUsuarioProy7);
            }

            TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProy8 = new TipoDocDispRolUsuarioProy();
            tipoDocDispRolUsuarioProy8.OrganizacionID = pFilaProyecto.OrganizacionID;
            tipoDocDispRolUsuarioProy8.ProyectoID = pFilaProyecto.ProyectoID;
            tipoDocDispRolUsuarioProy8.TipoDocumento = (short)TiposDocumentacion.Encuesta;
            tipoDocDispRolUsuarioProy8.RolUsuario = (short)TipoRolUsuario.Usuario;
            //tablaPermisos.AddTipoDocDispRolUsuarioProyRow(pFilaProyecto.OrganizacionID, pFilaProyecto.ProyectoID, (short)TiposDocumentacion.Encuesta, (short)TipoRolUsuario.Usuario);
            tablaPermisos.Add(tipoDocDispRolUsuarioProy8);
            existe = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipo => tipo.OrganizacionID.Equals(tipoDocDispRolUsuarioProy8.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocDispRolUsuarioProy8.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocDispRolUsuarioProy8.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocDispRolUsuarioProy8.RolUsuario)).ToList().Count > 0;
            if (!existe)
            {
                mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDispRolUsuarioProy8);
            }
        }

        /// <summary>
        /// Carga las últimas preguntas de una comunidad (pagina de inicio)
        /// </summary>
        /// <param name="pProyecto">Proyecto del que se quieren cargar las preguntas</param>
        /// <param name="pIdentidadActual">Identidad Actual</param>
        /// <param name="pNumeroResultadosPagina">Numero de preguntas que se quieren cargar</param>
        /// <returns>DocumentacionDS con las últimas preguntas del proyecto</returns>
        public DataWrapperDocumentacion CargarRecursosPopularesProyecto(Elementos.ServiciosGenerales.Proyecto pProyecto, int pNumeroResultadosPagina, Guid pUsuarioID)
        {
            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            docCL.ObtenerRecursosPopularesProyecto(dataWrapperDocumentacion, pProyecto.Clave, pNumeroResultadosPagina);
            docCL.ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, pProyecto.Clave, pProyecto.FilaProyecto.OrganizacionID, pUsuarioID);

            docCL.Dispose();

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Trae de la BD los editores de los documentos cargados.
        /// </summary>
        /// <param name="pGestorDocumental">Gestor Documental</param>
        private void CargarEditoresDocumentos(GestorDocumental pGestorDocumental)
        {
            List<Guid> listaEditorIDs = new List<Guid>();

            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaEditorDoc in pGestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolIdentidad)
            {
                if (!listaEditorIDs.Contains(filaEditorDoc.PerfilID))
                {
                    listaEditorIDs.Add(filaEditorDoc.PerfilID);
                }
            }
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            pGestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesDePerfiles(listaEditorIDs));

            pGestorDocumental.GestorIdentidades.RecargarHijos();

            List<Guid> listaIdentidadesEditoras = new List<Guid>();

            foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in pGestorDocumental.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad)
            {
                if (listaEditorIDs.Contains(filaIdent.PerfilID))
                {
                    listaIdentidadesEditoras.Add(filaIdent.IdentidadID);
                }
            }

            pGestorDocumental.GestorIdentidades.ListaIdentidadesVisiblesExternos = identCN.ObtenerListaIdentidadesVisiblesExternos(listaIdentidadesEditoras)
    ;
        }

        public void ActualizarModeloBase(Guid pProyectoID, PrioridadBase pPrioridadBase)
        {
            BaseProyectosDS baseProyectosDS = new BaseProyectosDS();

            string idProy = Constantes.ID_TAG_PROY + pProyectoID.ToString() + Constantes.ID_TAG_PROY;

            BaseProyectosDS.ColaTagsProyectosRow filaProy = baseProyectosDS.ColaTagsProyectos.NewColaTagsProyectosRow();
            filaProy.TablaBaseProyectoID = 0;
            filaProy.Tags = idProy;
            filaProy.Tipo = (short)TiposElementosEnCola.Agregado;
            filaProy.Estado = (short)EstadosColaTags.EnEspera;
            filaProy.FechaPuestaEnCola = DateTime.Now;
            filaProy.Prioridad = (short)pPrioridadBase;

            baseProyectosDS.ColaTagsProyectos.AddColaTagsProyectosRow(filaProy);

            BaseComunidadCN baseProyectosCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            baseProyectosCN.InsertarFilasEnRabbit("ColaTagsProyectos", baseProyectosDS);
        }

        /// <summary>
        /// Indica si al registrar a un usuario en un proyecto se le debe registrar en didactalia.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns></returns>
        public bool ProyectoDebeRegistrarEnDidactalia(Guid pProyectoID)
        {
            ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool regDidactalia = paramCN.ObtenerRegistroDidactaliaProyecto(pProyectoID);

            return regDidactalia;
        }

        /// <summary>
        /// Configura una comunidad a partir de los datos recibidos en un documento XML
        /// </summary>
        /// <param name="pXML">Ruta del archivo XML</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void ConfigurarComunidadConXML(string pXML, Guid pOrganizacionID, Guid pProyectoID)
        {
            if (string.IsNullOrEmpty(pXML))
            {
                throw new Exception("El fichero no puede estar vacío");
            }
            //cargo el XML
            XmlDocument configXML = new XmlDocument();
            configXML.LoadXml(pXML);

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ParametroGeneralCN parametroGralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            string nombreCortoProy = proyectoCN.ObtenerNombreCortoProyecto(pProyectoID).ToLower();
            XmlNode nodoNombreComunidad = configXML.SelectSingleNode("/Comunidad/NombreComunidad");
            if (nodoNombreComunidad != null)
            {
                string nombreComunidad = nodoNombreComunidad.InnerText.ToLower();
                if (string.IsNullOrEmpty(nombreComunidad) || !nombreComunidad.Equals(nombreCortoProy))
                {
                    throw new Exception("Está subiendo el Xml de configuración de otra comunidad");
                }
            }
            else
            {
                throw new Exception("Debe configurar el NombreComunidad");
            }

            XmlNode nodoViejoContextos = configXML.SelectSingleNode("/Comunidad/Contextos");

            if (nodoViejoContextos != null)
            {
                throw new Exception("Está subiendo el Xml con la antigua configuración de contextos. Debe sustituirla para poder subir el Xml");
            }

            //Validación nombres cortos GruposPermitidosSeleccionarPrivacidadRecursoAbierto
            List<Guid> listaGruposPermitidosSelPriv = new List<Guid>();
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            XmlNodeList gruposPermitidosPrivacidad = configXML.SelectNodes("/Comunidad/GruposPermitidosSeleccionarPrivacidadRecursoAbierto/Grupo");
            if (gruposPermitidosPrivacidad != null && gruposPermitidosPrivacidad.Count > 0)
            {
                foreach (XmlNode nodoGrupo in gruposPermitidosPrivacidad)
                {
                    string nombreGrupo = nodoGrupo.InnerText;
                    if (!string.IsNullOrEmpty(nombreGrupo))
                    {
                        Guid grupoID = Guid.Empty;

                        if (nombreGrupo.Contains("|||"))
                        {
                            string[] nombres = nombreGrupo.Split(new string[] { "|||" }, StringSplitOptions.None);
                            if (nombres.Length == 2 && !string.IsNullOrEmpty(nombres[0]) && !string.IsNullOrEmpty(nombres[1]))
                            {
                                string nombreCortoOrg = nombres[0];
                                string nombreCortoGr = nombres[1];

                                OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                Guid organizacionID = orgCN.ObtenerOrganizacionesIDPorNombre(nombreCortoOrg);

                                if (!organizacionID.Equals(Guid.Empty))
                                {
                                    List<string> listaNombres = new List<string>();
                                    listaNombres.Add(nombreCortoGr);
                                    List<Guid> lista = identidadCN.ObtenerGruposIDPorNombreCortoYOrganizacion(listaNombres, organizacionID);
                                    if (lista.Count > 0)
                                    {
                                        grupoID = lista[0];
                                    }
                                    else
                                    {
                                        throw new Exception("La organización " + nombreCortoOrg + " configurada en el nodo GruposPermitidosSeleccionarPrivacidadRecursoAbierto no tiene un grupo llamado " + nombreCortoGr);
                                    }
                                }
                                else
                                {
                                    throw new Exception("La organización " + nombreCortoOrg + " en el nodo GruposPermitidosSeleccionarPrivacidadRecursoAbierto no existe");
                                }
                            }
                            else
                            {
                                throw new Exception("Existe algún grupo de organización en el nodo GruposPermitidosSeleccionarPrivacidadRecursoAbierto con formato incorrecto");
                            }
                        }
                        else
                        {
                            grupoID = identidadCN.ObtenerGrupoIDPorNombreYProyectoID(nodoGrupo.InnerText, pProyectoID);
                        }

                        if (!grupoID.Equals(Guid.Empty))
                        {
                            if (!listaGruposPermitidosSelPriv.Contains(grupoID))
                            {
                                listaGruposPermitidosSelPriv.Add(grupoID);
                            }
                        }
                        else
                        {
                            throw new Exception("No existe el nombre corto de grupo: " + nodoGrupo.InnerText);
                        }
                    }
                    else
                    {
                        throw new Exception("El nodo GruposPermitidosSeleccionarPrivacidadRecursoAbierto no puede tener nombres de grupos vacíos");
                    }
                }
            }

            //obtenemos la configuración de la compartición de formularios semánticos de ese proyecto, se traen las eliminadas tambien
            ComparticionAutomaticaCN comparticionAutomaticaCN = new ComparticionAutomaticaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperComparticionAutomatica compAutoDW = comparticionAutomaticaCN.ObtenerComparticionProyectoPorProyectoID(pOrganizacionID, pProyectoID, true);

            //Obtengo el cms de las paginas
            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperCMS cmsDS = cmsCN.ObtenerCMSDeProyecto(pProyectoID);

            //obtengo el proyecto a partir 1º de su nombre corto y después a partir de su identificador
            //ProyectoDS proyectoDS = proyectoCN.ObtenerProyectoPorID(pProyectoID);
            ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD(mEntityContext);
            DataWrapperProyecto dataWrapperProyecto = proyectoGBD.ObtenerProyectoPorID(pProyectoID);
            //ProyectoDS proyectoDS = proyectoCN.ObtenerProyectoPorID(pProyectoID);
            //cargar las tablas necesarias de proyecto
            dataWrapperProyecto.Merge(proyectoCN.ObtenerSeccionesHomeCatalogoDeProyecto(pProyectoID));
            dataWrapperProyecto.Merge(proyectoCN.ObtenerPresentacionSemantico(pProyectoID));
            dataWrapperProyecto.Merge(proyectoCN.ObtenerFiltrosOrdenesDeProyecto(pProyectoID));
            dataWrapperProyecto.Merge(proyectoCN.ObtenerRecursosRelacionadosPresentacion(pProyectoID));
            dataWrapperProyecto.Merge(proyectoCN.ObtenerTesaurosSemanticosConfigEdicionDeProyecto(pProyectoID));
            //proyectoCN.ObtenerGadgetsProyecto(pProyectoID, proyectoDS);
            proyectoCN.ObtenerGadgetsProyectoUbicacion(pProyectoID, dataWrapperProyecto, TipoUbicacionGadget.FichaRecursoComunidad);
            GestionProyecto gestorProyecto = new GestionProyecto(dataWrapperProyecto, mLoggingService, mEntityContext);
            Elementos.ServiciosGenerales.Proyecto proyecto = gestorProyecto.ListaProyectos[pProyectoID];

            if (mProyecto == null)
            {
                mProyecto = proyecto;
            }

            ExportacionBusquedaCN exportacionBusquedaCN = new ExportacionBusquedaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperExportacionBusqueda exportacionBusquedaDW = exportacionBusquedaCN.ObtenerExportacionesProyecto(pProyectoID);

            //obtengo la configuración de ese proyecto desde parámetro general
            //GestorParametroGeneral paramGralDS = parametroGralCN.ObtenerParametrosGeneralesDeProyecto(pProyectoID);
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
            GestorParametroGeneral paramGralDS = new GestorParametroGeneral();
            paramGralDS = gestorController.ObtenerParametrosGeneralesDeProyecto(paramGralDS, pProyectoID);
            //sabemos que sílo va a venir una
            //ParametroGeneralDSName.ParametroGeneralRow filaParametroGral = paramGralDS.ParametroGeneral[0];
            ParametroGeneral filaParametroGral = paramGralDS.ListaParametroGeneral[0];

            //obtengo las configuración de las facetas de ese proyecto
            FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            //DataWrapperFacetas facetaDW = new DataWrapperFacetas();
            DataWrapperFacetas facetaDW = new DataWrapperFacetas();
            //cargo las tablas en el ds
            facetaDW = facetaCN.ObtenerTodasFacetasDeProyecto(pOrganizacionID, pProyectoID);

            //Obtengo la configuración del tesauro de ese proyecto
            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperTesauro tesauroDW = new DataWrapperTesauro();
            Guid tesauroID = tesauroCN.ObtenerIDTesauroDeProyecto(pProyectoID);
            tesauroDW = tesauroCN.ObtenerCategoriasPermitidasPorTipoRecurso(tesauroID);

            //Obtengo la configuración de las etiquetas
            var listadoConfigSearchProy = mEntityContext.ConfigSearchProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(pProyectoID);

            //nodo único en el documento           
            XmlNode ambitoTodoGnossVisible = configXML.SelectSingleNode("/Comunidad/Pestanyas/AmbitoTodoGnossVisible");
            XmlNode ambitoTodaLaComunidadVisible = configXML.SelectSingleNode("/Comunidad/Pestanyas/AmbitoTodaLaComunidadVisible");
            XmlNode cmsDisponible = configXML.SelectSingleNode("/Comunidad/Pestanyas/CMSDisponible");
            filaParametroGral.CMSDisponible = false;
            if (cmsDisponible != null)
            {
                filaParametroGral.CMSDisponible = cmsDisponible.InnerText.Equals("1");
            }

            #region Gestión cambios inserción masiva al base

            DataWrapperProyecto proyAuxCambiosMasBaseDS = new DataWrapperProyecto();
            //proyAuxCambiosMasBaseDS.Merge(dataWrapperProyecto.ListaProyectoPestanyaBusqueda);
            //proyAuxCambiosMasBaseDS.Merge(dataWrapperProyecto.ListaProyectoPestanyaMenu);
            proyAuxCambiosMasBaseDS.ListaProyectoPestanyaBusqueda = dataWrapperProyecto.ListaProyectoPestanyaBusqueda;
            proyAuxCambiosMasBaseDS.ListaProyectoPestanyaMenu = dataWrapperProyecto.ListaProyectoPestanyaMenu;
            DataWrapperFacetas facetaCambiosMasBaseDW = new DataWrapperFacetas();
            facetaCambiosMasBaseDW.ListaFacetaObjetoConocimientoProyecto.AddRange(facetaDW.ListaFacetaObjetoConocimientoProyecto);

            #endregion

            XmlNodeList comparticionesEnProyecto = configXML.SelectNodes("/Comunidad/ComparticionAutomatica/ComparticionEnProyecto");
            XmlNodeList pestanyas = configXML.SelectNodes("/Comunidad/Pestanyas/Pestanya");
            //XmlNodeList configuracionAmbitoBusqueda = configXML.SelectNodes("/Comunidad/Pestanyas/ConfiguracionAmbitoBusqueda");
            XmlNodeList ontologias = configXML.SelectNodes("/Comunidad/Ocs/Oc");
            XmlNodeList gadgets = configXML.SelectNodes("/Comunidad/Gadgets/Gadget");
            XmlNodeList todasFacetas = configXML.SelectNodes("/Comunidad/Facetas");
            XmlNodeList listaConfigAutocompletarProy = configXML.SelectNodes("Comunidad/ConfigAutocompletarProy");
            XmlNodeList listaConfigBBDDAutocompletarProyecto = configXML.SelectNodes("Comunidad/ConfigBBDDAutocompletarProyecto");
            XmlNodeList listaAgruparRegistrosUsuariosEnProyecto = configXML.SelectNodes("Comunidad/AgruparRegistrosUsuariosEnProyecto");
            XmlNodeList listaPintarEnlacesLODEtiquetasEnProyecto = configXML.SelectNodes("Comunidad/PintarEnlacesLODEtiquetasEnProyecto");
            XmlNodeList listaConfigSearchProy = configXML.SelectNodes("Comunidad/ConfigSearchProy");
            XmlNodeList listaTesaurosSemanticos = configXML.SelectNodes("Comunidad/TesaurosSemanticos/TesauroSemantico");
            XmlNodeList usarOntosOtroProyecto = configXML.SelectNodes("Comunidad/UsarOntologiasDeProyecto");
            XmlNodeList listaUtilidades = configXML.SelectNodes("Comunidad/Utilidades");

            //obtenemos la configuración de las categorías compartidas
            XmlNode comunidadOrigenCatCompartidas = configXML.SelectSingleNode("/Comunidad/CategoriasCompartidas/ComunidadOrigen");
            XmlNodeList categoriasCompartidas = configXML.SelectNodes("/Comunidad/CategoriasCompartidas/CategoriaID");

            #region CategoriasCompartidas

            string comOrigenCatCompartidas = string.Empty;

            mLoggingService.AgregarEntrada("Antes de categorias compartidas");
            if (comunidadOrigenCatCompartidas != null)
            {
                comOrigenCatCompartidas = comunidadOrigenCatCompartidas.InnerText;

                if (!string.IsNullOrEmpty(comOrigenCatCompartidas))
                {
                    if (categoriasCompartidas != null)
                    {
                        List<Guid> categoriasOrigen = new List<Guid>();

                        foreach (XmlNode nodoCategoria in categoriasCompartidas)
                        {
                            Guid catID = Guid.Empty;
                            if (Guid.TryParse(nodoCategoria.InnerText, out catID))
                            {
                                categoriasOrigen.Add(catID);
                            }
                        }

                        if (categoriasOrigen.Count > 0)
                        {
                            mLoggingService.AgregarEntrada("Tiene" + categoriasOrigen.Count + " categorias origen");

                            //cargar categorias compartidas de la comunidad origen hacia esta comunidad
                            Guid proyectoOrigen = proyectoCN.ObtenerProyectoIDPorNombre(comOrigenCatCompartidas);
                            Guid tesauroOrigenID = tesauroCN.ObtenerIDTesauroDeProyecto(proyectoOrigen);
                            //Guid tesauroDestinoID = tesauroCN.ObtenerIDTesauroDeProyecto(pProyectoID);
                            TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                            GestionTesauro gestorTesauroOrigen = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(proyectoOrigen), mLoggingService, mEntityContext);
                            GestionTesauro gestorTesauroDestino = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(pProyectoID), mLoggingService, mEntityContext);
                            IList<Guid> listaCatTesOrigen = gestorTesauroOrigen.ListaCategoriasTesauro.Keys;
                            IList<Guid> listaCatTesDestino = gestorTesauroDestino.ListaCategoriasTesauro.Keys;
                            short i = 0;

                            foreach (Guid catID in categoriasOrigen)
                            {
                                mLoggingService.AgregarEntrada("Antes de mirar si la categotria " + catID.ToString() + " pertenece al tesauro");

                                //si la categoria del xml pertenece al origen y no está ya compartida en destino
                                if (listaCatTesOrigen.Contains(catID) && !listaCatTesDestino.Contains(catID))
                                {
                                    mLoggingService.AgregarEntrada("La categoria de origen pertenece al tesauro de origen");

                                    AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaCatTesauroCompartida = new AD.EntityModel.Models.Tesauro.CatTesauroCompartida();
                                    filaCatTesauroCompartida.TesauroOrigenID = tesauroOrigenID;
                                    filaCatTesauroCompartida.CategoriaOrigenID = catID;
                                    filaCatTesauroCompartida.TesauroDestinoID = tesauroID;
                                    filaCatTesauroCompartida.Orden = i;
                                    tesauroDW.ListaCatTesauroCompartida.Add(filaCatTesauroCompartida);
                                    mEntityContext.CatTesauroCompartida.Add(filaCatTesauroCompartida);
                                    i++;
                                }
                            }

                            tesauroCL.Dispose();
                        }
                    }
                }
            }

            #endregion

            #region ComparticionFormulariosSemanticos

            BaseComunidadDS baseComDS = new BaseComunidadDS();

            if (comparticionesEnProyecto != null && comparticionesEnProyecto.Count > 0)
            {
                //obtener las comunidades de destino para obtener la identidad del publicador
                XmlNodeList nodosComunidadesDestino = configXML.SelectNodes("/Comunidad/ComparticionAutomatica/ComparticionEnProyecto/ComunidadDestino");
                Dictionary<string, Guid> listaComunidadesDestinoID = new Dictionary<string, Guid>();

                foreach (XmlNode nodo in nodosComunidadesDestino)
                {
                    if (!listaComunidadesDestinoID.ContainsKey(nodo.InnerText))
                    {
                        listaComunidadesDestinoID.Add(nodo.InnerText, proyectoCN.ObtenerProyectoIDPorNombre(nodo.InnerText));
                    }
                }

                XmlNode nodoPublicador = configXML.SelectSingleNode("/Comunidad/ComparticionAutomatica/Publicador");
                string publicador = nodoPublicador != null ? nodoPublicador.InnerText : "";

                foreach (string comDestino in listaComunidadesDestinoID.Keys)
                {
                    foreach (XmlNode comparticionEnProyecto in comparticionesEnProyecto)
                    {
                        //seleccionar el nodo de la que comunidad correspondiente no seleccionar el [0]
                        if (comparticionEnProyecto.SelectSingleNode("ComunidadDestino").InnerText.Equals(comDestino))
                        {
                            //dataset para el control de cambios en la compartición
                            DataWrapperComparticionAutomatica compAutoCambiosDW = new DataWrapperComparticionAutomatica();

                            ConfigurarComparticionComunidadConXML(comparticionEnProyecto, compAutoDW, compAutoCambiosDW, pOrganizacionID, pProyectoID, publicador);
                        }
                    }

                    foreach (AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomatica filaComparticion in compAutoDW.ListaComparticionAutomatica.ToList())
                    {
                        //si no es añadida, ni está modificada(aunque no tengan cambios, las que permanecen se guardan en modificadas), ni ya está eliminada, hay que eliminarla
                        if (!mListaModificadasEF[typeof(AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomatica).Name].Contains(filaComparticion) && !mEntityContext.Entry(filaComparticion).State.Equals(EntityState.Added) && !filaComparticion.Eliminada)
                        {
                            filaComparticion.Eliminada = true;
                        }

                        //si tiene cambios
                        if (!mEntityContext.Entry(filaComparticion).State.Equals(EntityState.Unchanged))
                        {
                            BaseComunidadDS.ColaComparticionAutomaticaRow filaCola = baseComDS.ColaComparticionAutomatica.NewColaComparticionAutomaticaRow();
                            filaCola.OrdenEjecucion = Guid.NewGuid();
                            filaCola.Estado = 0;
                            filaCola.ID = filaComparticion.ComparticionID;
                            filaCola.Fecha = DateTime.Now;
                            filaCola.Prioridad = 1;

                            if (filaComparticion.Eliminada)
                            {
                                filaCola.Tipo = (short)TiposEventoComparticion.ConfiguracionEliminada;
                            }
                            else
                            {
                                filaCola.Tipo = (short)TiposEventoComparticion.ConfiguracionNueva;
                            }

                            baseComDS.ColaComparticionAutomatica.AddColaComparticionAutomaticaRow(filaCola);
                        }
                    }
                }
            }
            else
            {
                //hay comparticiones?? marcarlas como eliminada = true y por cada fila crear una fila en el base
                ComparticionAutomaticaCN comparticionCN = new ComparticionAutomaticaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                foreach (AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomatica filaComparticion in compAutoDW.ListaComparticionAutomatica.ToList())
                {
                    filaComparticion.Eliminada = true;

                    BaseComunidadDS.ColaComparticionAutomaticaRow filaCola = baseComDS.ColaComparticionAutomatica.NewColaComparticionAutomaticaRow();
                    filaCola.OrdenEjecucion = Guid.NewGuid();
                    filaCola.Tipo = (short)TiposEventoComparticion.ConfiguracionEliminada;
                    filaCola.Estado = 0;
                    filaCola.ID = filaComparticion.ComparticionID;
                    filaCola.Fecha = DateTime.Now;
                    filaCola.Prioridad = 1;
                    baseComDS.ColaComparticionAutomatica.AddColaComparticionAutomaticaRow(filaCola);
                }
            }

            #endregion ComparticionFormulariosSemanticos

            #region ConfiguracionAmbitoBusqueda

            //ParametroGeneralDSName.ConfiguracionAmbitoBusquedaProyectoRow filaConfiguracionAmbitoBusqueda = ConfigurarAmbitoBusqueda(paramGralDS, pProyectoID, pOrganizacionID, ambitoTodoGnossVisible, ambitoTodaLaComunidadVisible);
            ConfiguracionAmbitoBusquedaProyecto filaConfiguracionAmbitoBusqueda = ConfigurarAmbitoBusqueda(paramGralDS, pProyectoID, pOrganizacionID, ambitoTodoGnossVisible, ambitoTodaLaComunidadVisible);


            #endregion ConfiguracionAmbitoBusqueda

            #region ProyectoPestanya

            Dictionary<Guid, KeyValuePair<TipoPestanyaMenu, XmlNode>> listaIDsPestanyas = new Dictionary<Guid, KeyValuePair<TipoPestanyaMenu, XmlNode>>();
            List<string> listaNombresCortosPestanyas = new List<string>();
            List<short> listaUbicacionesCMS = new List<short>();
            if (pestanyas != null && pestanyas.Count > 0)
            {
                ConfigurarPestanyasComunidadConXML(pestanyas, dataWrapperProyecto, cmsDS, pOrganizacionID, pProyectoID, null, exportacionBusquedaDW, ref listaIDsPestanyas, ref listaNombresCortosPestanyas, ref listaUbicacionesCMS, ref filaConfiguracionAmbitoBusqueda);
            }
            else
            {
                List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> filasPestanyas = dataWrapperProyecto.ListaProyectoPestanyaMenu.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).ToList();//("ProyectoID='" + pProyectoID.ToString() + "'");
                if (filasPestanyas != null && filasPestanyas.Count > 0)
                {
                    foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pest in filasPestanyas)
                    {
                        if (pest.TipoPestanya != (short)TipoPestanyaMenu.CMS)
                        {
                            BorrarPestanya(proyectoGBD, pest, filasPestanyas, dataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos);

                            dataWrapperProyecto.ListaProyectoPestanyaMenu.Remove(pest);
                        }
                    }
                    proyectoGBD.GuardarCambios();
                }
            }

            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> listaAniadidas = mEntityContext.ProyectoPestanyaMenu.Local.Where(pestanya => mEntityContext.Entry(pestanya).State.Equals(EntityState.Added)).ToList();

            #endregion

            #region Usar ontologías otro proyecto
            Guid idProyOntologias = pProyectoID;
            if (usarOntosOtroProyecto != null && usarOntosOtroProyecto.Count > 0 && !string.IsNullOrEmpty(usarOntosOtroProyecto[0].InnerText))
            {
                ProyectoCL proyOCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                Guid proyExtID = proyOCL.ObtenerProyectoIDPorNombreCorto(usarOntosOtroProyecto[0].InnerText);
                proyOCL.Dispose();

                if (proyExtID == Guid.Empty)
                {
                    throw new Exception("Error la configurar 'UsarOntologiasDeProyecto'. No existe el proyecto con nombre corto '" + usarOntosOtroProyecto[0].InnerText + "'.");
                }
                idProyOntologias = proyExtID;

                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, ParametroAD.ProyectoIDPatronOntologias, proyExtID.ToString());
            }
            else
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.BorrarParametroDeProyecto(pProyectoID, pOrganizacionID, ParametroAD.ProyectoIDPatronOntologias);
            }


            #endregion

            #region ontologias

            if (ontologias != null && ontologias.Count > 0)
            {
                //ontologías ya están sus listas agregadas a mListaModificadas desde ConfigurarOntologias
                ConfigurarOntologias(configXML, ontologias, pOrganizacionID, pProyectoID, dataWrapperProyecto, facetaDW, tesauroDW, documentacionCN, idProyOntologias);
            }
            else
            {
                //si no viene presentación en el xml borro las filas de esa ontología del DS
                List<OntologiaProyecto> filasOnt = facetaDW.ListaOntologiaProyecto.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();

                //borro las ontologías del DS y las filas relacionadas de las tablas que dependen de esa ontología
                foreach (OntologiaProyecto ont in filasOnt)
                {
                    #region pasar esto a funcion
                    Guid ontologiaID = documentacionCN.ObtenerOntologiaAPartirNombre(pProyectoID, ont.OntologiaProyecto1 + ".owl");
                    mEntityContext.Entry(ont).State = EntityState.Deleted;
                    //borro las filas de PresentacionListadoSemantico
                    List<PresentacionListadoSemantico> filasListadoSem = dataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacionListadoSem => presentacionListadoSem.OrganizacionID.Equals(pOrganizacionID) && presentacionListadoSem.ProyectoID.Equals(pProyectoID) && presentacionListadoSem.OntologiaID.Equals(ontologiaID)).ToList();
                    //("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and OntologiaID='" + ontologiaID.ToString() + "'");

                    if (filasListadoSem != null && filasListadoSem.Count > 0)
                    {
                        foreach (PresentacionListadoSemantico pls in filasListadoSem)
                        {
                            dataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(pls);
                            proyectoGBD.DeletePresentacionListadoSemantico(pls);
                            proyectoGBD.GuardarCambios();
                            //pls.Delete();
                        }
                    }

                    //borro las filas de PresentacionMosaicoSemantico
                    List<PresentacionMosaicoSemantico> filasMosaicoSem = dataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacionMosaicoSem => presentacionMosaicoSem.OrganizacionID.Equals(pOrganizacionID) && presentacionMosaicoSem.ProyectoID.Equals(pProyectoID) && presentacionMosaicoSem.OntologiaID.Equals(ontologiaID)).ToList();
                    //("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and OntologiaID='" + ontologiaID.ToString() + "'");

                    if (filasMosaicoSem != null && filasMosaicoSem.Count > 0)
                    {
                        foreach (PresentacionMosaicoSemantico pms in filasMosaicoSem)
                        {
                            dataWrapperProyecto.ListaPresentacionMosaicoSemantico.Remove(pms);
                            proyectoGBD.DeletePresentacionMosaicoSemantico(pms);
                            proyectoGBD.GuardarCambios();
                            //pms.Delete();
                        }
                    }

                    //borro las filas de PresentacionMapaSemantico
                    List<PresentacionMapaSemantico> filasMapaSem = dataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacionMapaSem => presentacionMapaSem.OrganizacionID.Equals(pOrganizacionID) && presentacionMapaSem.ProyectoID.Equals(pProyectoID) && presentacionMapaSem.OntologiaID.Equals(ontologiaID)).ToList();
                    //("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and OntologiaID='" + ontologiaID.ToString() + "'");

                    if (filasMapaSem != null && filasMapaSem.Count > 0)
                    {
                        foreach (PresentacionMapaSemantico pms in filasMapaSem)
                        {
                            dataWrapperProyecto.ListaPresentacionMapaSemantico.Remove(pms);
                            proyectoGBD.DeletePresentacionMapaSemantico(pms);
                            proyectoGBD.GuardarCambios();
                            //pms.Delete();
                        }
                    }

                    //borro las filas de RecursosRelacionadosPresentacion
                    List<RecursosRelacionadosPresentacion> listaPlsr = dataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(recursosRelacionadosPresentacion => recursosRelacionadosPresentacion.OrganizacionID.Equals(pOrganizacionID) && recursosRelacionadosPresentacion.ProyectoID.Equals(pProyectoID) && recursosRelacionadosPresentacion.OntologiaID.Equals(ontologiaID)).ToList();//("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and OntologiaID='" + ontologiaID.ToString() + "'");
                    foreach (RecursosRelacionadosPresentacion rrp in listaPlsr)
                    {
                        dataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Remove(rrp);
                        proyectoGBD.DeleteRecursosRelacionadosPresentacion(rrp);
                        proyectoGBD.GuardarCambios();
                        //rrp.Delete();
                    }

                    //borro las filas de CatTesauroPermiteTipoRec
                    List<CatTesauroPermiteTipoRec> listaCatTes = tesauroDW.ListaCatTesauroPermiteTipoRec.Where(item => item.OntologiasID.Equals(ontologiaID)).ToList();

                    foreach (CatTesauroPermiteTipoRec rrp in listaCatTes)
                    {
                        mEntityContext.EliminarElemento(rrp);
                        tesauroDW.ListaCatTesauroPermiteTipoRec.Remove(rrp);
                    }
                    #endregion pasar esto a funcion
                }

            }

            #endregion ontologias

            #region Contextos

            if (gadgets != null && gadgets.Count > 0)
            {
                //compruebo que los nombres de los contextos son unicos
                XmlNodeList nodosNombres = configXML.SelectNodes("/Comunidad/Gadgets/Gadget/NombreCorto");
                List<string> nombres = new List<string>();
                foreach (XmlNode nom in nodosNombres)
                {
                    if (string.IsNullOrEmpty(nom.InnerText))
                    {
                        throw new Exception("Todos los contextos deben tener nombre corto");
                    }
                    if (nom.InnerText.Contains(" "))
                    {
                        throw new Exception("El contexto " + nom.InnerText + " no puede contener espacios en el nombrecorto");
                    }
                    if (nom.InnerText.Contains("#"))
                    {
                        throw new Exception("El contexto " + nom.InnerText + " no puede contener # en el nombrecorto");
                    }
                    if (!nombres.Contains(nom.InnerText))
                    {
                        nombres.Add(nom.InnerText);
                    }
                    else
                    {
                        throw new Exception("El nombre corto del contexto ya existe: " + nom.InnerText);
                    }
                }

                ConfigurarGadgets(gadgets, pOrganizacionID, pProyectoID, dataWrapperProyecto);
            }
            else
            {
                //si no viene presentación en el xml borro los gadgets de recursos (TipoUbicacion = 1, sílo están cargados los de este tipo)
                List<ProyectoGadget> filasProyGadget = dataWrapperProyecto.ListaProyectoGadget.Where(proyectoGadget => proyectoGadget.OrganizacionID.Equals(pOrganizacionID) && proyectoGadget.ProyectoID.Equals(pProyectoID) && proyectoGadget.TipoUbicacion.Equals((short)TipoUbicacionGadget.FichaRecursoComunidad)).ToList();//("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and TipoUbicacion=" + (short)TipoUbicacionGadget.FichaRecursoComunidad); 

                //para cada gadget tengo que borrar también sus filas dependientes
                foreach (ProyectoGadget filaPG in filasProyGadget)
                {
                    string gadgetID = filaPG.GadgetID.ToString();
                    dataWrapperProyecto.ListaProyectoGadget.Remove(filaPG);
                    proyectoGBD.DeleteProyectoGadget(filaPG);
                    proyectoGBD.GuardarCambios();
                    //filaPG.Delete();

                    //ProyectoGadgetContexto
                    List<ProyectoGadgetContexto> filasProyGadgetContexto = dataWrapperProyecto.ListaProyectoGadgetContexto.Where(proyectoGadgetContext => proyectoGadgetContext.OrganizacionID.Equals(pOrganizacionID) && proyectoGadgetContext.ProyectoID.Equals(pProyectoID)).ToList();//("GadgetID='" + gadgetID + "' and OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID + "'");
                    foreach (ProyectoGadgetContexto filaPGC in filasProyGadgetContexto)
                    {
                        dataWrapperProyecto.ListaProyectoGadgetContexto.Remove(filaPGC);
                        proyectoGBD.DeleteProyectoGadgetContexto(filaPGC);
                        proyectoGBD.GuardarCambios();
                        //filaPGC.Delete();
                    }

                    //ProyectoGadgetIdioma
                    List<ProyectoGadgetIdioma> filasProyGadgetIdioma = dataWrapperProyecto.ListaProyectoGadgetIdioma.Where(proyectoGadgetIdioma => proyectoGadgetIdioma.GadgetID.Equals(gadgetID) && proyectoGadgetIdioma.OrganizacionID.Equals(pOrganizacionID) && proyectoGadgetIdioma.ProyectoID.Equals(pProyectoID)).ToList();//("GadgetID='" + gadgetID + "' and OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID + "'");
                    foreach (ProyectoGadgetIdioma filaPGI in filasProyGadgetIdioma)
                    {
                        dataWrapperProyecto.ListaProyectoGadgetIdioma.Remove(filaPGI);
                        proyectoGBD.DeleteProyectoGadgetIdioma(filaPGI);
                        proyectoGBD.GuardarCambios();
                        //filaPGI.Delete();
                    }
                }
            }
            #endregion Contextos

            #region Facetas

            if (todasFacetas.Count > 0)
            {
                foreach (XmlNode facetaRaiz in todasFacetas)
                {
                    #region FacetasExcluidas

                    XmlNodeList facetasExcluidas = facetaRaiz.SelectNodes("FacetasExcluidas/FacetaExcluida");
                    if (facetasExcluidas != null && facetasExcluidas.Count > 0)
                    {
                        ConfigurarFacetasExcluidas(facetasExcluidas, facetaCN, pOrganizacionID, pProyectoID, facetaDW);
                    }
                    else
                    {
                        //si no viene presentación en el xml borro las filas de esa ontología del DS
                        List<FacetaExcluida> listaExcluidas = facetaDW.ListaFacetaExcluida.Where(item => item.OrganizacionID.Equals(pOrganizacionID.ToString()) && item.ProyectoID.Equals(pProyectoID.ToString())).ToList();
                        foreach (FacetaExcluida fe in listaExcluidas)
                        {
                            mEntityContext.EliminarElemento(fe);
                            facetaDW.ListaFacetaExcluida.Remove(fe);
                        }
                    }
                    #endregion FacetasExcluidas

                    #region FacetaEntidadesExternas

                    XmlNodeList facetasEntidadesExternas = facetaRaiz.SelectNodes("FacetaEntidadesExternas");
                    if (facetasEntidadesExternas != null && facetasEntidadesExternas.Count > 0)
                    {
                        ConfigurarFacetaEntidadesExternas(facetasEntidadesExternas, facetaCN, pOrganizacionID, pProyectoID, facetaDW);
                    }
                    else
                    {
                        //si no viene presentación en el xml borro las filas de esa ontología del DS
                        List<FacetaEntidadesExternas> listaExternas = facetaDW.ListaFacetaEntidadesExternas.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
                        foreach (FacetaEntidadesExternas fee in listaExternas)
                        {
                            mEntityContext.EliminarElemento(fee);
                            facetaDW.ListaFacetaEntidadesExternas.Remove(fee);
                        }
                    }
                    #endregion FacetaEntidadesExternas

                    #region Faceta

                    XmlNodeList listaFaceta = facetaRaiz.SelectNodes("Faceta");

                    ConfigurarFacetas(listaFaceta, facetaDW, pOrganizacionID, pProyectoID, dataWrapperProyecto);

                    ActualizarSearchComunidad(facetaDW, baseComDS);

                    #endregion Faceta
                }
            }
            else
            {
                //si no viene presentación en el xml borro las filas de esa ontología del DS
                List<FacetaExcluida> listaExcluidas = facetaDW.ListaFacetaExcluida.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();

                foreach (FacetaExcluida fe in listaExcluidas)
                {
                    facetaDW.ListaFacetaExcluida.Remove(fe);
                    mEntityContext.EliminarElemento(fe);
                }

                //si no viene presentación en el xml borro las filas de esa ontología del DS
                List<FacetaEntidadesExternas> listaExternas = facetaDW.ListaFacetaEntidadesExternas.Where(item => item.OrganizacionID.Equals(pOrganizacionID.ToString()) && item.ProyectoID.Equals(pProyectoID.ToString())).ToList();
                foreach (FacetaEntidadesExternas fee in listaExternas)
                {
                    facetaDW.ListaFacetaEntidadesExternas.Remove(fee);
                    mEntityContext.EliminarElemento(fee);
                }
            }

            #endregion Facetas

            #region FacetaConfiguracionProyectoRangoFecha

            XmlNodeList configFacProyRangoFecha = configXML.SelectNodes("Comunidad/ConfigFacetadoProyRangoFecha");
            if (configFacProyRangoFecha != null)
            {
                ConfigurarFacetaConfiguracionProyectoRangoFechaConXML(configFacProyRangoFecha, facetaDW, pOrganizacionID, pProyectoID);
            }
            else
            {
                //si no viene presentación en el xml borro las filas de esa ontología del DS
                List<FacetaConfigProyRangoFecha> filaConfigProyRangoFecha = facetaDW.ListaFacetaConfigProyRangoFecha.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
                if (filaConfigProyRangoFecha != null && filaConfigProyRangoFecha.Count > 0)
                {
                    foreach (FacetaConfigProyRangoFecha fila in filaConfigProyRangoFecha)
                    {
                        mEntityContext.EliminarElemento(fila);
                        facetaDW.ListaFacetaConfigProyRangoFecha.Remove(fila);
                    }
                }
            }
            #endregion FacetaConfiguracionProyectoMapa

            #region FacetaConfiguracionProyectoMapa

            XmlNodeList configFacProyMapa = configXML.SelectNodes("Comunidad/ConfigFacetadoProyMapa");
            if (configFacProyMapa != null)
            {
                ConfigurarFacetaConfiguracionProyectoMapaConXML(configFacProyMapa, facetaDW, pOrganizacionID, pProyectoID);
            }
            else
            {
                //si no viene presentación en el xml borro las filas de esa ontología del DS
                FacetaConfigProyMapa filaConfigProyMapa = facetaDW.ListaFacetaConfigProyMapa.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
                if (filaConfigProyMapa != null)
                {
                    mEntityContext.EliminarElemento(filaConfigProyMapa);
                    facetaDW.ListaFacetaConfigProyMapa.Remove(filaConfigProyMapa);
                }
            }
            #endregion FacetaConfiguracionProyectoMapa

            #region FacetaConfiguracionProyChart

            XmlNodeList listaConfigFacProyChart = configXML.SelectNodes("Comunidad/ConfigFacetadoProyChart");
            if (listaConfigFacProyChart != null && listaConfigFacProyChart.Count > 0)
            {
                ConfigurarFacetaConfiguracionProyChart(listaConfigFacProyChart, facetaDW, pOrganizacionID, pProyectoID);
            }
            else
            {
                //si no viene presentación en el xml borro las filas de esa ontología del DS
                List<FacetaConfigProyChart> listaConfigAuto = facetaDW.ListaFacetaConfigProyChart.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
                foreach (FacetaConfigProyChart fcp in listaConfigAuto)
                {
                    mEntityContext.EliminarElemento(fcp);
                    facetaDW.ListaFacetaConfigProyChart.Remove(fcp);
                }
            }
            #endregion FacetaConfiguracionProyChart

            #region ConfiguracionAutocompletarProyecto

            bool existenFilas = ConfigurarAutocompletarProy(listaConfigAutocompletarProy, listaIDsPestanyas, dataWrapperProyecto, pOrganizacionID, pProyectoID);
            if (!existenFilas)
            {
                //si no hay ninguna configuracion de autocompletar borro todas las filas del autocompletar
                List<ConfigAutocompletarProy> listaConfigAuto = dataWrapperProyecto.ListaConfigAutocompletarProy.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
                foreach (ConfigAutocompletarProy capr in listaConfigAuto)
                {
                    mEntityContext.Entry(capr).State = EntityState.Deleted;
                    dataWrapperProyecto.ListaConfigAutocompletarProy.Remove(capr);
                }
            }
            #endregion ConfiguracionAutocompletarProyecto

            #region ConfigBBDDAutocompletarProyecto

            if (listaConfigBBDDAutocompletarProyecto != null && listaConfigBBDDAutocompletarProyecto.Count > 0)
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "ConfigBBDDAutocompletarProyecto", listaConfigBBDDAutocompletarProyecto[0].InnerText);
            }
            else
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "ConfigBBDDAutocompletarProyecto", ((short)TipoBusquedasAutocompletar.BBDDTags).ToString());
            }

            #endregion ConfigBBDDAutocompletarProyecto

            #region AgruparRegistrosUsuariosEnProyecto

            if (listaAgruparRegistrosUsuariosEnProyecto != null && listaAgruparRegistrosUsuariosEnProyecto.Count > 0)
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "AgruparRegistrosUsuariosEnProyecto", listaAgruparRegistrosUsuariosEnProyecto[0].InnerText);
            }
            else
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "AgruparRegistrosUsuariosEnProyecto", ((short)TipoBusquedasAutocompletar.BBDDTags).ToString());
            }

            #endregion AgruparRegistrosUsuariosEnProyecto

            #region PintarEnlacesLODEtiquetasEnProyecto

            if (listaPintarEnlacesLODEtiquetasEnProyecto != null && listaPintarEnlacesLODEtiquetasEnProyecto.Count > 0)
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "PintarEnlacesLODEtiquetasEnProyecto", listaPintarEnlacesLODEtiquetasEnProyecto[0].InnerText);
            }
            else
            {
                ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                paramCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "PintarEnlacesLODEtiquetasEnProyecto", (1).ToString());
            }

            #endregion PintarEnlacesLODEtiquetasEnProyecto

            #region ConfiguracionSearchProyecto

            if (listaConfigSearchProy != null && listaConfigSearchProy.Count > 0)
            {
                ConfigurarSearchProy(listaConfigSearchProy, listadoConfigSearchProy, pOrganizacionID, pProyectoID);
            }
            else
            {
                //si no viene presentación en el xml borro las filas de esa ontología del DS
                foreach (var item in listadoConfigSearchProy)
                {
                    mEntityContext.EliminarElemento(item);
                }
            }
            #endregion

            #region Utilidades

            if (listaUtilidades != null && listaUtilidades.Count > 0)
            {
                ConfigurarUtilidades(listaUtilidades[0], filaParametroGral, vistaVirtualDW, pOrganizacionID, pProyectoID);
            }
            #endregion

            #region Tesauros semánticos

            if (listaTesaurosSemanticos != null && listaTesaurosSemanticos.Count > 0)
            {
                ConfigurarEdicionTesSem(listaTesaurosSemanticos, dataWrapperProyecto, pProyectoID);
            }
            else
            {
                //si no viene presentación en el xml borro las filas de esa ontología del DS
                //"ProyectoID='" + pProyectoID.ToString() + "' AND Tipo=" + (short)TipoConfigExtraSemantica.TesauroSemantico
                foreach (ProyectoConfigExtraSem fila in dataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proyectConfigExtraSem => proyectConfigExtraSem.ProyectoID.Equals(pProyectoID) && proyectConfigExtraSem.Tipo.Equals((short)TipoConfigExtraSemantica.TesauroSemantico)).ToList())
                {
                    proyectoGBD.DeleteProyectoConfigExtraSem(fila);
                    dataWrapperProyecto.ListaProyectoConfigExtraSem.Remove(fila);
                    proyectoGBD.GuardarCambios();
                    //fila.Delete();
                }
            }

            #endregion

            //Borro las filas que no han sido modificadas y no vienen en el xml
            BorrarFilasSobrantes();

            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> listaEliminadas = dataWrapperProyecto.ListaProyectoPestanyaMenu.Where(pestanya => mEntityContext.Entry(pestanya).State.Equals(EntityState.Deleted)).ToList();

            //obtengo la lista de filas eliminadas
            if (listaEliminadas != null)
            {
                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanyaEliminada in listaEliminadas)
                {
                    string ruta = string.Empty;

                    ruta = filaPestanyaEliminada.Ruta;

                    if (!mRutasPestanyasInvalidar.Contains(ruta))
                    {
                        mRutasPestanyasInvalidar.Add(ruta);
                    }

                    if (proyAuxCambiosMasBaseDS.ListaProyectoPestanyaBusqueda.Any(item => item.PestanyaID.Equals(filaPestanyaEliminada.PestanyaID)))
                    {
                        proyAuxCambiosMasBaseDS.ListaProyectoPestanyaBusqueda.Remove(dataWrapperProyecto.ListaProyectoPestanyaBusqueda.First(item => item.PestanyaID.Equals(filaPestanyaEliminada.PestanyaID)));
                    }
                    proyAuxCambiosMasBaseDS.ListaProyectoPestanyaMenu.Remove(proyAuxCambiosMasBaseDS.ListaProyectoPestanyaMenu.First(item => item.PestanyaID.Equals(filaPestanyaEliminada.PestanyaID)));

                    if (dataWrapperProyecto.ListaProyectoPestanyaBusqueda.Any(item => item.PestanyaID.Equals(filaPestanyaEliminada.PestanyaID)))
                    {
                        dataWrapperProyecto.ListaProyectoPestanyaBusqueda.Remove(dataWrapperProyecto.ListaProyectoPestanyaBusqueda.First(item => item.PestanyaID.Equals(filaPestanyaEliminada.PestanyaID)));
                    }
                    dataWrapperProyecto.ListaProyectoPestanyaMenu.Remove(filaPestanyaEliminada);
                }
            }

            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> listaModificadas = dataWrapperProyecto.ListaProyectoPestanyaMenu.Where(pestanya => mEntityContext.Entry(pestanya).State.Equals(EntityState.Modified)).ToList();
            if (listaModificadas != null)
            {
                //obtengo la lista de filas modificadas            
                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanyaModificada in listaModificadas)
                {
                    string rutaOriginal = "";

                    rutaOriginal = filaPestanyaModificada.Ruta;

                    string rutaActual = filaPestanyaModificada.Ruta;
                    bool recalcular = rutaOriginal != rutaActual;

                    if (filaPestanyaModificada.ProyectoPestanyaBusqueda != null)
                    {
                        ProyectoPestanyaBusqueda filaBusqueda = filaPestanyaModificada.ProyectoPestanyaBusqueda;
                        string filtroOriginal = "";

                        filtroOriginal = filaBusqueda.CampoFiltro;

                        string filtroActual = filaBusqueda.CampoFiltro;

                        recalcular = recalcular || (filtroOriginal != filtroActual);
                    }

                    if (recalcular)
                    {
                        if (!mRutasPestanyasInvalidar.Contains(rutaOriginal))
                        {
                            mRutasPestanyasInvalidar.Add(rutaOriginal);
                        }
                        if (!mRutasPestanyasRegistrar.Contains(rutaActual))
                        {
                            mRutasPestanyasRegistrar.Add(rutaActual);
                        }
                    }
                }
            }

            //obtengo la lista de filas añadidas para registrarlas
            //List <AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> listaAniadidas = dataWrapperProyecto.ListaProyectoPestanyaMenu.Where(pestanya => mEntityContext.Entry(pestanya).State.Equals(EntityState.Added)).ToList();
            if (listaAniadidas != null)
            {
                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanyaAniadida in listaAniadidas)
                {
                    if (!mRutasPestanyasRegistrar.Contains(filaPestanyaAniadida.Ruta))
                    {
                        mRutasPestanyasRegistrar.Add(filaPestanyaAniadida.Ruta);
                    }
                }
            }

            List<AD.EntityModel.Models.CMS.CMSPagina> listaAuxiliar = cmsDS.ListaCMSPagina.ToList();

            //Eliminamos las páginas que no tengan fila en ProyectoPestanyaCMS (excepto las de home)
            foreach (AD.EntityModel.Models.CMS.CMSPagina cmsPaginaRow in listaAuxiliar)
            {
                short ubicacion = cmsPaginaRow.Ubicacion;
                List<ProyectoPestanyaCMS> filasPestanyasMenu = dataWrapperProyecto.ListaProyectoPestanyaCMS.Where(fila =>/* fila.RowState != DataRowState.Deleted && */fila.Ubicacion.Equals(ubicacion)).ToList();

                if (filasPestanyasMenu.Count == 0 && ubicacion != (short)TipoUbicacionCMS.HomeProyecto && ubicacion != (short)TipoUbicacionCMS.HomeProyectoMiembro && ubicacion != (short)TipoUbicacionCMS.HomeProyectoNoMiembro)
                {
                    GestionCMS gestorCMS = new GestionCMS(cmsDS, mLoggingService, mEntityContext);
                    List<CMSBloque> listaCMSBloque = gestorCMS.ListaBloques.Values.ToList();
                    foreach (CMSBloque bloqueCMS in listaCMSBloque)
                    {
                        if (bloqueCMS.TipoUbicacion == ubicacion)
                        {
                            List<AD.EntityModel.Models.CMS.CMSBloqueComponente> filasBloqueComponente = bloqueCMS.FilaBloque.CMSBloqueComponente.ToList();
                            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente in filasBloqueComponente)
                            {
                                List<AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente> filasBloqueComponentePropiedadComponente = filaBloqueComponente.CMSBloqueComponentePropiedadComponente.ToList();
                                foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaBloqueComponentePropiedadComponente in filasBloqueComponentePropiedadComponente)
                                {
                                    //Borramos CMSBloqueComponentePropiedadComponenteRow
                                    mEntityContext.EliminarElemento(filaBloqueComponentePropiedadComponente);
                                    filaBloqueComponente.CMSBloqueComponentePropiedadComponente.Remove(filaBloqueComponentePropiedadComponente);
                                }
                                //Borramos CMSBloqueComponenteRow
                                mEntityContext.EliminarElemento(filaBloqueComponente);
                                bloqueCMS.FilaBloque.CMSBloqueComponente.Remove(filaBloqueComponente);
                            }
                            //Borramos CMSBloqueRow
                            mEntityContext.EliminarElemento(bloqueCMS.FilaBloque);
                            gestorCMS.ListaBloques.Remove(bloqueCMS.FilaBloque.BloqueID);
                        }
                    }

                    //Borramos CMSPaginaRow
                    mEntityContext.EliminarElemento(cmsPaginaRow);
                    cmsDS.ListaCMSPagina.Remove(cmsPaginaRow);
                }

            }

            //guardo cambios
            mEntityContext.SaveChanges();

            #region Gestión cambios inserción masiva al base

            bool insercion = HayQueInsertarCambiosMasivosAlBase(proyAuxCambiosMasBaseDS, dataWrapperProyecto, facetaCambiosMasBaseDW, facetaDW);

            //proyAuxCambiosMasBaseDS.Dispose();
            //facetaCambiosMasBaseDW.Dispose();

            if (insercion)
            {
                new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarTodosLosRecursosComunidadModeloBase(pProyectoID, true, null, PrioridadBase.Media);
            }

            #endregion

            BaseComunidadCN baseComCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            baseComCN.ActualizarBD(baseComDS);

            #region ParametrosProyecto

            //configuro los grupos que pueden abrir la visibilidad de un recurso en la comunidad
            //como parametroproyecto se inserta directamente en BD lo hago lo último, sabiendo que el resto del xml ha ido bien
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (listaGruposPermitidosSelPriv.Count > 0)
            {
                string valor = "";
                foreach (Guid grupoID in listaGruposPermitidosSelPriv)
                {
                    valor += grupoID.ToString() + "|||";
                }
                parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "GruposPermitidosSeleccionarPrivacidadRecursoAbierto", valor);
            }
            else if (ParametroProyecto.ContainsKey("GruposPermitidosSeleccionarPrivacidadRecursoAbierto"))
            {
                parametroCN.BorrarParametroDeProyecto(pProyectoID, pOrganizacionID, "GruposPermitidosSeleccionarPrivacidadRecursoAbierto");
            }


            //NumeroCaracteresDescripcionSuscripcion de ParametroProyecto
            XmlNode nodoNumeroCaracteresDescripcionSuscripcion = configXML.SelectSingleNode("/Comunidad/NumeroCaracteresDescripcionSuscripcion");
            if (nodoNumeroCaracteresDescripcionSuscripcion != null)
            {
                int numCaracteres = 0;
                string strNumCaracteres = nodoNumeroCaracteresDescripcionSuscripcion.InnerText.ToLower();
                if (!int.TryParse(strNumCaracteres, out numCaracteres))
                {
                    throw new Exception("El nodo 'NumeroCaracteresDescripcionSuscripcion' no está configurado con un número entero");
                }
                parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, ParametroAD.NumeroCaracteresDescripcion, strNumCaracteres);
            }
            else if (ParametroProyecto.ContainsKey(ParametroAD.NumeroCaracteresDescripcion))
            {
                parametroCN.BorrarParametroDeProyecto(pProyectoID, pOrganizacionID, ParametroAD.NumeroCaracteresDescripcion);
            }

            //PermitirDescargaIdentidadInvitada de ParametroProyecto
            XmlNode nodoPermitirDescargaIdentidadInvitada = configXML.SelectSingleNode("/Comunidad/" + ParametroAD.PermitirDescargaIdentidadInvitada);
            if (nodoPermitirDescargaIdentidadInvitada != null)
            {
                bool permitida = nodoPermitirDescargaIdentidadInvitada.InnerText.Equals("1");
                parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, ParametroAD.PermitirDescargaIdentidadInvitada, nodoPermitirDescargaIdentidadInvitada.InnerText);
            }
            else if (ParametroProyecto.ContainsKey(ParametroAD.PermitirDescargaIdentidadInvitada))
            {
                parametroCN.BorrarParametroDeProyecto(pProyectoID, pOrganizacionID, ParametroAD.PermitirDescargaIdentidadInvitada);
            }

            //CargarEditoresLectoresEnBusqueda de ParametroProyecto
            XmlNode nodoCargarEditoresLectoresEnBusqueda = configXML.SelectSingleNode("/Comunidad/" + ParametroAD.CargarEditoresLectoresEnBusqueda);
            if (nodoCargarEditoresLectoresEnBusqueda != null)
            {
                bool permitida = nodoCargarEditoresLectoresEnBusqueda.InnerText.Equals("1");
                parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, ParametroAD.CargarEditoresLectoresEnBusqueda, nodoCargarEditoresLectoresEnBusqueda.InnerText);
            }
            else if (ParametroProyecto.ContainsKey(ParametroAD.CargarEditoresLectoresEnBusqueda))
            {
                parametroCN.BorrarParametroDeProyecto(pProyectoID, pOrganizacionID, ParametroAD.CargarEditoresLectoresEnBusqueda);
            }

            //NombrePoliticaCookies de ParametroProyecto
            XmlNode nodoNombrePoliticaCookies = configXML.SelectSingleNode("/Comunidad/" + ParametroAD.NombrePoliticaCookies);
            if (nodoNombrePoliticaCookies != null)
            {
                parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, ParametroAD.NombrePoliticaCookies, nodoNombrePoliticaCookies.InnerText);
            }
            else if (ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
            {
                parametroCN.BorrarParametroDeProyecto(pProyectoID, pOrganizacionID, ParametroAD.NombrePoliticaCookies);
            }

            #endregion

            //invalido las caches
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarPestanyasProyecto(pProyectoID);
            proyCL.InvalidarSeccionesHomeCatalogoDeProyecto(pProyectoID);
            proyCL.InvalidarFilaProyecto(pProyectoID);
            proyCL.InvalidarGadgetsProyecto(pProyectoID);
            proyCL.InvalidarFiltrosOrdenesDeProyecto(pProyectoID);
            proyCL.InvalidarParametrosProyecto(pProyectoID, pOrganizacionID);
            proyCL.InvalidarPresentacionSemantico(pProyectoID);
            proyCL.InvalidarComunidadMVC(pProyectoID);
            proyCL.InvalidarCabeceraMVC(pProyectoID);
            proyCL.InvalidarFormularioRegistroMVC(pProyectoID);
            ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            paramGeneralCL.InvalidarCacheParametrosGeneralesDeProyecto(pProyectoID);
            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            facetaCL.InvalidarCacheFacetasMayusculas();

            bool cachearFacetas = !(this.ParametroProyecto.ContainsKey("CacheFacetas") && this.ParametroProyecto["CacheFacetas"].Equals("0"));
            facetaCL.InvalidarCacheFacetasProyecto(pProyectoID, cachearFacetas);

            facetaCL.InvalidarOntologiasProyecto(pProyectoID);
            facetaCL.EliminarDatosChartProyecto(pProyectoID);
            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(pProyectoID, "*");
            TesauroCL tesCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            tesCL.BorrarCategoriasPermitidasPorTipoRecurso(pProyectoID);
            tesCL.InvalidarCacheDeTesauroDeProyecto(pProyectoID);
            tesCL.Dispose();

            ExportacionBusquedaCL exportacionCL = new ExportacionBusquedaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            exportacionCL.InvalidarCacheExportacionesProyecto(pProyectoID);
            exportacionCL.Dispose();

            mGnossCache.VersionarCacheLocal(pProyectoID);
        }

        public void InsertarParametrosAdministracion(Guid pProyectoID, Guid pOrganizacionID)
        {
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            //Poner administración a todas las comunidades
            parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "AdministracionDesarrolladoresPermitido", "1");
            parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "AdministracionPaginasPermitido", "1");
            parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "AdministracionSemanticaPermitido", "1");
            parametroCN.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, "AdministracionVistasPermitido", "1");
            ProyectoCL proyecto = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyecto.InvalidarParametrosProyecto(pProyectoID, pOrganizacionID);
        }
        public void BorrarPestanya(ProyectoGBD.ProyectoGBD proyectoGBD, AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu padre, List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> filasPestanyas, List<ProyectoPestanyaFiltroOrdenRecursos> listaProyectoPestanyaFiltroOrdenRecursos)
        {
            if (padre.TipoPestanya != (short)TipoPestanyaMenu.CMS)
            {
                var listaHijas = filasPestanyas.Where(item => item.PestanyaPadreID.HasValue && item.PestanyaPadreID.Value.Equals(padre.PestanyaID)).ToList();
                List<ProyectoPestanyaFiltroOrdenRecursos> filasFiltros2 = listaProyectoPestanyaFiltroOrdenRecursos.Where(proyect => proyect.PestanyaID.Equals(padre.PestanyaID)).ToList();//("PestanyaID='" + pest.PestanyaID + "'");
                if (filasFiltros2 != null && filasFiltros2.Count > 0)
                {
                    foreach (ProyectoPestanyaFiltroOrdenRecursos filtro in filasFiltros2)
                    {
                        proyectoGBD.DeleteProyectoPestanyaFiltroOrdenRecursos(filtro);
                    }
                }
                foreach (var hija in listaHijas)
                {
                    BorrarPestanya(proyectoGBD, hija, filasPestanyas, listaProyectoPestanyaFiltroOrdenRecursos);
                }
                List<ProyectoPestanyaBusqueda> listaProyectoPestanyaBusquedapadre = mEntityContext.ProyectoPestanyaBusqueda.Where(item => item.PestanyaID.Equals(padre.PestanyaID)).ToList();

                List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades> listaProyectoPestanyaMenuRolGrupoIdentidadespadre = mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Where(item => item.PestanyaID.Equals(padre.PestanyaID)).ToList();
                foreach (var item in listaProyectoPestanyaMenuRolGrupoIdentidadespadre)
                {
                    mEntityContext.EliminarElemento(item);
                    proyectoGBD.GuardarCambios();
                }
                List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaBusquedaExportacion> listaProyectoProyectoPestanyaBusquedaExportacionpadre = mEntityContext.ProyectoPestanyaBusquedaExportacion.Where(item => item.PestanyaID.Equals(padre.PestanyaID)).ToList();
                foreach (var item in listaProyectoProyectoPestanyaBusquedaExportacionpadre)
                {
                    List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaBusquedaExportacionPropiedad> listaProyectoPestanyaBusquedaExportacionPropiedad = mEntityContext.ProyectoPestanyaBusquedaExportacionPropiedad.Where(exportacion => exportacion.ExportacionID.Equals(item.ExportacionID)).ToList();
                    foreach (var exportacion in listaProyectoPestanyaBusquedaExportacionPropiedad)
                    {
                        mEntityContext.EliminarElemento(exportacion);
                        proyectoGBD.GuardarCambios();
                    }
                    mEntityContext.EliminarElemento(item);
                    proyectoGBD.GuardarCambios();
                }
                foreach (var item in listaProyectoPestanyaBusquedapadre)
                {
                    mEntityContext.EliminarElemento(item);
                    proyectoGBD.GuardarCambios();
                }
                proyectoGBD.DeleteProyectoPestanyaMenu(padre);
                proyectoGBD.GuardarCambios();

            }
        }

        public string ObtenerTesauroComunidadConXML(Guid pProyectoID)
        {
            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionTesauro gestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroDeProyecto(pProyectoID), mLoggingService, mEntityContext);

            var xml = new XDocument(new XElement("Thesaurus", new XElement("Categories", gestorTesauro.ListaCategoriasTesauroPrimerNivel.Values.Select(categoria => CrearCategoriaEnXML(categoria)))));
            xml.Declaration = new XDeclaration("1.0", "utf-8", null);

            gestorTesauro.Dispose();

            return $"{xml.Declaration}{Environment.NewLine}{xml.ToString()}";
        }

        private static XElement CrearCategoriaEnXML(Elementos.Tesauro.CategoriaTesauro pCategoria)
        {
            return new XElement("Category", new XElement("Name", pCategoria.FilaCategoria.Nombre), new XElement("ID", pCategoria.Clave), pCategoria.SubCategorias.Select(subcategoria => CrearCategoriaEnXML(subcategoria)));
        }

        public void ConfigurarTesauroComunidadConXML(Guid pProyectoID, string pXML)
        {
            DataWrapperTesauro tesauroDW = new DataWrapperTesauro();

            LimpiarTesauroComunidad(pProyectoID);

            XmlDocument tesauroXML = new XmlDocument();
            tesauroXML.LoadXml(pXML);
            Guid tesauroID = Guid.NewGuid();

            //Tesauro
            Tesauro tesauro = new Tesauro();
            tesauro.TesauroID = tesauroID;
            tesauroDW.ListaTesauro.Add(tesauro);
            mEntityContext.Tesauro.Add(tesauro);

            //TesauroProyecto
            TesauroProyecto filaTesauroProyecto = new TesauroProyecto();
            filaTesauroProyecto.TesauroID = tesauroID;
            filaTesauroProyecto.OrganizacionID = ProyectoAD.MetaOrganizacion;
            filaTesauroProyecto.ProyectoID = pProyectoID;
            filaTesauroProyecto.IdiomaDefecto = null;
            tesauroDW.ListaTesauroProyecto.Add(filaTesauroProyecto);
            mEntityContext.TesauroProyecto.Add(filaTesauroProyecto);


            //Proceso del xml
            XmlNodeList categorias = tesauroXML.SelectNodes("Thesaurus/Categories/Category");

            if (categorias == null || categorias.Count == 0)
            {
                categorias = tesauroXML.SelectNodes("TesauroComunidad/Categorias/Categoria");
            }

            ProcesarXmlTesauro(pProyectoID, tesauroID, Guid.Empty, categorias, tesauroDW);

            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            tesauroCN.ActualizarTesauro();

            //invalidamos cache del tesauro
            TesauroCL tesCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            tesCL.InvalidarCacheDeTesauroDeProyecto(pProyectoID);
            tesCL.Dispose();

            //invalidamos cache del proyecto
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarFilaProyecto(pProyectoID);
            proyCL.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pTesauroID"></param>
        /// <param name="pCategoriaSuperiorID"></param>
        /// <param name="pNodosCategorias"></param>
        /// <param name="pTesauroDW"></param>
        private void ProcesarXmlTesauro(Guid pProyectoID, Guid pTesauroID, Guid pCategoriaSuperiorID, XmlNodeList pNodosCategorias, DataWrapperTesauro pTesauroDW)
        {
            short orden = 0;
            foreach (XmlNode nodoCategoria in pNodosCategorias)
            {
                string nombreCategoria = (string)UtilXML.LeerNodo(nodoCategoria, "Name", typeof(string));
                Guid categoriaID;
                short estructurante = 0;

                if (string.IsNullOrEmpty(nombreCategoria))
                {
                    nombreCategoria = (string)UtilXML.LeerNodo(nodoCategoria, "Nombre", typeof(string));
                }

                if (!Guid.TryParse((string)UtilXML.LeerNodo(nodoCategoria, "ID", typeof(string)), out categoriaID))
                {
                    categoriaID = Guid.NewGuid();
                }

                if (!string.IsNullOrEmpty((string)UtilXML.LeerNodo(nodoCategoria, "Estructurante", typeof(string))))
                {
                    short.TryParse((string)UtilXML.LeerNodo(nodoCategoria, "Estructurante", typeof(string)), out estructurante);
                }

                //añadir fila a CategoriaTesauro
                AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCategoria = new AD.EntityModel.Models.Tesauro.CategoriaTesauro();
                filaCategoria.TesauroID = pTesauroID;
                filaCategoria.CategoriaTesauroID = categoriaID;
                filaCategoria.Nombre = nombreCategoria;
                filaCategoria.Orden = orden;
                filaCategoria.NumeroRecursos = 0;
                filaCategoria.NumeroPreguntas = 0;
                filaCategoria.NumeroDebates = 0;
                filaCategoria.NumeroDafos = 0;
                filaCategoria.TieneFoto = false;
                filaCategoria.VersionFoto = 0;
                filaCategoria.Estructurante = estructurante;
                pTesauroDW.ListaCategoriaTesauro.Add(filaCategoria);
                mEntityContext.CategoriaTesauro.Add(filaCategoria);

                if (!pCategoriaSuperiorID.Equals(Guid.Empty))
                {
                    //es hija, añadir fila a CatTesauroAgCatTesauro
                    CatTesauroAgCatTesauro catTesauroAgCatTesauro = new CatTesauroAgCatTesauro();
                    catTesauroAgCatTesauro.TesauroID = pTesauroID;
                    catTesauroAgCatTesauro.CategoriaSuperiorID = pCategoriaSuperiorID;
                    catTesauroAgCatTesauro.CategoriaInferiorID = categoriaID;
                    catTesauroAgCatTesauro.Orden = orden;
                    pTesauroDW.ListaCatTesauroAgCatTesauro.Add(catTesauroAgCatTesauro);
                    mEntityContext.CatTesauroAgCatTesauro.Add(catTesauroAgCatTesauro);
                }


                //la categoria recien insertada será la categoriaSuperior de las hijas
                XmlNodeList categoriasHijas = nodoCategoria.SelectNodes("Category");

                if (categoriasHijas == null || categoriasHijas.Count == 0)
                {
                    categoriasHijas = nodoCategoria.SelectNodes("Categoria");
                }

                if (categoriasHijas != null && categoriasHijas.Count > 0)
                {
                    ProcesarXmlTesauro(pProyectoID, pTesauroID, categoriaID, categoriasHijas, pTesauroDW);
                }

                orden++;
            }

        }

        /// <summary>
        /// Borra el tesauro de la comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        private void LimpiarTesauroComunidad(Guid pProyectoID)
        {
            try
            {
                DataWrapperTesauro tesauroDW = new DataWrapperTesauro();
                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                tesauroDW = tesauroCN.ObtenerTesauroDeProyecto(pProyectoID);
                tesauroDW.Merge(tesauroCN.ObtenerTesauroCompletoPorID(tesauroCN.ObtenerIDTesauroDeProyecto(pProyectoID)));

                List<CatTesauroAgCatTesauro> listaCatTesauroAgCatTesauroBorrar = tesauroDW.ListaCatTesauroAgCatTesauro.ToList();
                foreach (CatTesauroAgCatTesauro filaCatTesAgCatTes in listaCatTesauroAgCatTesauroBorrar)
                {
                    tesauroDW.ListaCatTesauroAgCatTesauro.Remove(filaCatTesAgCatTes);
                    mEntityContext.EliminarElemento(filaCatTesAgCatTes);
                }

                List<AD.EntityModel.Models.Tesauro.CategoriaTesauro> listaCatTesBorrar = tesauroDW.ListaCategoriaTesauro.ToList();
                foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCategoriaTesauro in listaCatTesBorrar)
                {
                    tesauroDW.ListaCategoriaTesauro.Remove(filaCategoriaTesauro);
                    mEntityContext.EliminarElemento(filaCategoriaTesauro);
                }

                List<TesauroProyecto> listaTesauroProyectoBorrar = tesauroDW.ListaTesauroProyecto.ToList();
                foreach (TesauroProyecto filaTesProyecto in listaTesauroProyectoBorrar)
                {
                    tesauroDW.ListaTesauroProyecto.Remove(filaTesProyecto);
                    mEntityContext.EliminarElemento(filaTesProyecto);
                }

                List<Tesauro> listaTesauroBorrar = tesauroDW.ListaTesauro.ToList();
                foreach (Tesauro filaTesauro in listaTesauroBorrar)
                {
                    tesauroDW.ListaTesauro.Remove(filaTesauro);
                    mEntityContext.EliminarElemento(filaTesauro);
                }

                tesauroCN.ActualizarTesauro();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                throw new Exception("No se ha podido eliminar el tesauro");
            }
        }

        private bool HayQueInsertarCambiosMasivosAlBase(DataWrapperProyecto pProyAuxCambiosMasBaseDS, DataWrapperProyecto pDataWrapperProyecto, DataWrapperFacetas pFacetaCambiosMasBaseDW, DataWrapperFacetas pFacetaDW)
        {
            bool insercion = (pProyAuxCambiosMasBaseDS.ListaProyectoPestanyaBusqueda.Count != pDataWrapperProyecto.ListaProyectoPestanyaBusqueda.Count);

            if (!insercion)
            {
                for (int i = 0; i < pProyAuxCambiosMasBaseDS.ListaProyectoPestanyaBusqueda.Count; i++)
                {
                    ProyectoPestanyaBusqueda filIn = pProyAuxCambiosMasBaseDS.ListaProyectoPestanyaBusqueda[i];
                    ProyectoPestanyaBusqueda filPo = pDataWrapperProyecto.ListaProyectoPestanyaBusqueda[i];

                    if (/*filPo.RowState == DataRowState.Deleted ||*/ filIn.CampoFiltro != filPo.CampoFiltro || (filIn.ProyectoPestanyaMenu != null && filPo.ProyectoPestanyaMenu != null && filIn.ProyectoPestanyaMenu.Ruta != filPo.ProyectoPestanyaMenu.Ruta) || (!filIn.ProyectoOrigenID.HasValue && filPo.ProyectoOrigenID.HasValue) || (filIn.ProyectoOrigenID.HasValue && !filPo.ProyectoOrigenID.HasValue) || (filIn.ProyectoOrigenID.HasValue && filPo.ProyectoOrigenID.HasValue && filIn.ProyectoOrigenID != filPo.ProyectoOrigenID))
                    {
                        insercion = true;
                    }
                }
            }

            if (!insercion)
            {
                insercion = (pFacetaCambiosMasBaseDW.ListaFacetaObjetoConocimientoProyecto.Count != pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Count);
            }

            if (!insercion)
            {
                for (int i = 0; i < pFacetaCambiosMasBaseDW.ListaFacetaObjetoConocimientoProyecto.Count; i++)
                {
                    FacetaObjetoConocimientoProyecto filIn = pFacetaCambiosMasBaseDW.ListaFacetaObjetoConocimientoProyecto[i];
                    FacetaObjetoConocimientoProyecto filPo = pFacetaDW.ListaFacetaObjetoConocimientoProyecto[i];
                    var entry = mEntityContext.Entry(filPo);
                    if (entry.State == EntityState.Deleted || filIn.ObjetoConocimiento != filPo.ObjetoConocimiento || filIn.Faceta != filPo.Faceta || filIn.TipoPropiedad != filPo.TipoPropiedad)
                    {
                        insercion = true;
                    }
                }
            }

            return insercion;
        }

        private void ActualizarSearchComunidad(DataWrapperFacetas pFacetaDW, BaseComunidadDS pBaseComDS)
        {
            foreach (FacetaObjetoConocimientoProyecto focpr in pFacetaDW.ListaFacetaObjetoConocimientoProyecto)
            {
                //comprobamos si las FacetaObjetoConocimientoProyecto han sido modficadas o si son nuevas y con autocompletar
                var entry = mEntityContext.Entry(focpr);
                //var entryVersion = mEntityContext.Entry(focpr).OriginalValues.GetValue<bool>(nameof(focpr.Autocompletar));
                var entryVersion = mEntityContext.ObtenerValorOriginalDeObjeto<bool>(focpr, nameof(focpr.Autocompletar));
                if ((entry.State == EntityState.Modified && !entryVersion.Equals(DataRowVersion.Original) || (entry.State == EntityState.Added && focpr.Autocompletar)))
                {
                    //las agregamos al base en ColaModificacionSearch
                    AgregarFilaBaseColaModificacionSearch(pBaseComDS, 0, focpr.ProyectoID, 1);
                    break;
                }
            }
        }

        private void AgregarFilaBaseColaModificacionSearch(BaseComunidadDS pBaseComDS, short pTipo, Guid pProyectoID, short pPrioridad)
        {
            BaseComunidadDS.ColaModificacionSearchRow filaCola = pBaseComDS.ColaModificacionSearch.NewColaModificacionSearchRow();
            filaCola.Tipo = pTipo;
            filaCola.Estado = 0;
            filaCola.ProyectoID = pProyectoID;
            filaCola.FechaPuestaEnCola = DateTime.Now;
            filaCola.Prioridad = pPrioridad;
            pBaseComDS.ColaModificacionSearch.AddColaModificacionSearchRow(filaCola);
        }

        /// <summary>
        /// Configura la compartición automática para una comunidad de origen
        /// </summary>
        /// <param name="pReglasComparticion">Lista de nodos de la compartición</param>
        /// <param name="pCompAutoDW">DataSet con la configuración actual de la compartición de la comunidad</param>
        /// <param name="pCompAutoCambiosDW">DataSet para guardar las filas afectadas por cambios</param>        
        /// <param name="pOrgOrigenID">Identificador de la organización de origen</param>
        /// <param name="pProyOrigenID">Identificador de la comunidad de origen</param>
        /// <param name="pIdentidadPublicadoraID">Identificador de la identidad publicadora</param>
        public void ConfigurarComparticionComunidadConXML(XmlNode pComparticionProyecto, DataWrapperComparticionAutomatica pCompAutoDW, DataWrapperComparticionAutomatica pCompAutoCambiosDW, Guid pOrgOrigenID, Guid pProyOrigenID, string pPublicador)
        {
            BaseComunidadDS baseRecursosComDS = new BaseComunidadDS();

            //foreach (XmlNode comparticionProyecto in pReglasComparticion)
            //{
            Dictionary<List<string>, List<string>> duplasReglasCategorias = new Dictionary<List<string>, List<string>>();
            Dictionary<string, string> listaReglasComp = null;
            List<string> listaReglasMapping = null;
            List<string> listaCatDestinoMapping = null;
            Dictionary<string, List<Guid>> dictCatTesauro = null;
            Guid orgDestinoID = Guid.Empty;
            Guid proyDestinoID = Guid.Empty;

            ComparticionAutomaticaCN comparticionAutomaticaCN = new ComparticionAutomaticaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            string nomComparticion = (string)LeerNodo(pComparticionProyecto, "NombreComparticion", typeof(string));
            string nomComunidadDestino = (string)LeerNodo(pComparticionProyecto, "ComunidadDestino", typeof(string));
            string actualizar = (string)LeerNodo(pComparticionProyecto, "ActualizarHome", typeof(string));
            bool actualizarHome = actualizar.Equals("1");
            XmlNodeList nodosReglaComparticion = pComparticionProyecto.SelectNodes("ReglaComparticion");
            XmlNodeList nodosMapping = pComparticionProyecto.SelectNodes("MappingCategorias/Mapping");

            Guid? comparticionID = comparticionAutomaticaCN.ObtenerComparticionIDPorNombre(pProyOrigenID, nomComparticion);
            //obtener comparticionID a partir del nombre de la regla
            if (!comparticionID.HasValue)
            {
                comparticionID = Guid.NewGuid();
            }


            string publicador = (string)LeerNodo(pComparticionProyecto, "Publicador", typeof(string));
            if (string.IsNullOrEmpty(publicador))
            {
                publicador = pPublicador;
            }

            //if (FilasPropiedadesIntegracion != null && FilasPropiedadesIntegracion.Count > 0)
            //{
            //    var propiedadFiltrosDestino = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == nomComparticion && prop.TipoObjeto.Equals((short)TipoObjeto.Comparticion) && prop.TipoPropiedad.Equals((short)TipoPropiedad.IdentidadComparticion));

            //    if (propiedadFiltrosDestino != null && propiedadFiltrosDestino.ValorPropiedad == publicador && propiedadFiltrosDestino.Revisada && !propiedadFiltrosDestino.MismoValor)
            //    {
            //        publicador = propiedadFiltrosDestino.ValorPropiedadDestino;
            //    }
            //}

            //obtengo OrganizacionID y proyectoID del origen
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Guid> lista = proyectoCN.ObtenerProyectoIDOrganizacionIDPorNombreCorto(nomComunidadDestino);
            Guid identidadPublicadoraID = Guid.Empty;

            //existe comunidad de destino
            if (lista != null && lista.Count > 0)
            {
                orgDestinoID = lista[0];
                proyDestinoID = lista[1];

                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadPublicadoraID = identCN.ObtenerIdentidadIDPorNombreCorto(publicador, proyDestinoID);

                if (!identidadPublicadoraID.Equals(Guid.Empty) || (identidadPublicadoraID.Equals(Guid.Empty) && publicador.ToLower().Equals("###publicador###")))
                {

                    #region ReglaComparticion
                    if (nodosReglaComparticion != null && nodosReglaComparticion.Count > 0)
                    {
                        listaReglasComp = new Dictionary<string, string>();
                        foreach (XmlNode reg in nodosReglaComparticion)
                        {
                            string regla = (string)LeerNodo(reg, "Regla", typeof(string));
                            string navegacion = (string)LeerNodo(pComparticionProyecto, "ReglaComparticion/Navegacion", typeof(string));

                            if (regla != string.Empty)
                            {
                                //añado las Reglas a la lista que posteriormente generará filas en la tabla ComparticionAutomaticaReglas
                                listaReglasComp.Add(regla, navegacion);
                            }
                        }
                        #region mappingcategorias
                        if (nodosMapping != null && nodosMapping.Count > 0)
                        {
                            //listaReglasMapping = new List<string>();
                            //listaCatDestinoMapping = new List<string>();
                            dictCatTesauro = new Dictionary<string, List<Guid>>();
                            //recorro los nodos mapping
                            foreach (XmlNode mapping in nodosMapping)
                            {
                                listaReglasMapping = new List<string>();
                                listaCatDestinoMapping = new List<string>();
                                XmlNodeList nodosReglaMapping = mapping.SelectNodes("ReglaOrigen");
                                XmlNodeList nodosCatDestinoMapping = mapping.SelectNodes("CatDestino");
                                //añado las reglasOrigen a la lista
                                foreach (XmlNode nodo in nodosReglaMapping)
                                {
                                    string r = nodo.InnerText;
                                    if (!r.Equals(string.Empty))
                                    {
                                        listaReglasMapping.Add(r);
                                    }
                                }
                                //añado las catDestino a la lista
                                foreach (XmlNode nodo in nodosCatDestinoMapping)
                                {
                                    string cat = nodo.InnerText;
                                    if (!cat.Equals(string.Empty))
                                    {
                                        listaCatDestinoMapping.Add(cat);

                                        if (!cat.Equals("###categoria###"))
                                        {
                                            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                            List<Guid> listaCatTesauro = tesauroCN.ObtenerTesauroYCategoria(proyDestinoID, cat);
                                            if (listaCatTesauro != null)
                                            {
                                                dictCatTesauro.Add(cat, listaCatTesauro);
                                            }
                                            else
                                            {
                                                throw new Exception("Esa categoría no existe en este proyecto");
                                            }
                                        }
                                    }
                                }

                                duplasReglasCategorias.Add(listaReglasMapping, listaCatDestinoMapping);
                                ////ahora tengo las listas de reglas y categorias para ese mapping, agregarlo al ComparticionFormularioSemDS si no existen ya
                                //AgregarReglasComparticionADataSet(pCompFormSemDS, comparticionID, nomComparticion, pOrgOrigenID, pProyOrigenID, orgDestinoID, proyDestinoID, listaReglasComp, listaReglasMapping, listaCatDestinoMapping, dictCatTesauro, pIdentidadPublicadoraID);

                                //limpio las listas para el siguiente mapping
                                listaReglasMapping = null;
                                listaCatDestinoMapping = null;
                            }
                        }
                        else
                        {
                            //no hay configuración de mapeo en el XML
                            throw new Exception("No hay configuración de mapeo en el XML");
                        }
                        #endregion mappingcategorias
                    }
                    else
                    {
                        //no hay reglas de comparticion, lanzo una excepción
                        throw new Exception("No hay reglas de compartición en el XML");
                    }
                    #endregion ReglaComparticion
                }
                else
                {
                    //no existe comunidad de destino
                    throw new Exception("No existe el publicador en el XML");
                }
            }
            else
            {
                //no existe comunidad de destino
                throw new Exception("No existe comunidad de destino en el XML");
            }

            //si llega hasta aquí las listas tienen que estar llenas, si no se han configurado saltará alguna excepción de las anteriores
            //ahora tengo las listas de reglas y categorias para ese mapping, agregarlo al dataset si no existen ya
            Guid idComparticion = AgregarReglasComparticionADataSet(pCompAutoDW, pCompAutoCambiosDW, comparticionID.Value, nomComparticion, pOrgOrigenID, pProyOrigenID, orgDestinoID, proyDestinoID, listaReglasComp, duplasReglasCategorias, dictCatTesauro, identidadPublicadoraID, false, actualizarHome);

            //}

            //return baseRecursosComDS;
        }

        /// <summary>
        /// Guarda la configuración de la compartición automática para un proyecto
        /// </summary>
        /// <param name="pCompAutoDW">DataSet con la configuración de la compartición automática de la comunidad</param>
        /// <param name="pCompAutoCambiosDW">DataSet para guardar las filas afectadas por cambios</param>
        /// <param name="pComparticionID">Identificador de la compartición</param>
        /// <param name="pOrgOrigenID">Identificador de la organización de origen</param>
        /// <param name="pProyOrigenID">Identificador del proyecto de origen</param>
        /// <param name="pOrgDestinoID">Identificador de la organización de destino</param>
        /// <param name="pProyDestinoID">Identificador del proyecto de destino</param>
        /// <param name="pListaReglasComp">Lista de reglas de compartición</param>
        /// <param name="pListaReglasMapping">Lista de reglas de mapeo</param>
        /// <param name="pListaCatDestinoMapping">Lista de categorías de mapeo</param>
        /// <param name="pDictCatTesauro">Diccionario que relaciona un nombre de tesauro con su tesauroID y categoriatesauroID</param>
        private Guid AgregarReglasComparticionADataSet(DataWrapperComparticionAutomatica pCompAutoDW, DataWrapperComparticionAutomatica pCompAutoCambiosDW, Guid pComparticionID, string pNomComparticion, Guid pOrgOrigenID, Guid pProyOrigenID, Guid pOrgDestinoID, Guid pProyDestinoID, Dictionary<string, string> pListaReglasComp, Dictionary<List<string>, List<string>> pDuplasReglasCategorias, Dictionary<string, List<Guid>> pDictCatTesauro, Guid pIdentidadPublicadoraID, bool pEliminada, bool pActualizarHome)
        {
            #region ComparticionAutomatica
            //tabla ComparticionAutomatica
            List<ComparticionAutomatica> listaModificadasCompAuto = new List<ComparticionAutomatica>();
            Dictionary<string, object> listaCamposCompAuto = new Dictionary<string, object>();
            ComparticionAutomatica compAutAux = new ComparticionAutomatica();
            listaCamposCompAuto.Add(nameof(compAutAux.ComparticionID), pComparticionID);
            listaCamposCompAuto.Add(nameof(compAutAux.Nombre), pNomComparticion);
            listaCamposCompAuto.Add(nameof(compAutAux.OrganizacionOrigenID), pOrgOrigenID);
            listaCamposCompAuto.Add(nameof(compAutAux.ProyectoOrigenID), pProyOrigenID);
            listaCamposCompAuto.Add(nameof(compAutAux.OrganizacionDestinoID), pOrgDestinoID);
            listaCamposCompAuto.Add(nameof(compAutAux.ProyectoDestinoID), pProyDestinoID);
            listaCamposCompAuto.Add(nameof(compAutAux.IdentidadPublicadoraID), pIdentidadPublicadoraID);
            listaCamposCompAuto.Add(nameof(compAutAux.Eliminada), pEliminada);
            listaCamposCompAuto.Add(nameof(compAutAux.ActualizarHome), pActualizarHome);

            //miro si existe esa fila en el DS
            ComparticionAutomatica filaCompAuto = pCompAutoDW.ListaComparticionAutomatica.Where(item => item.ComparticionID.Equals(pComparticionID)).FirstOrDefault();
            //si la fila existe tengo que comprobar los cambios
            ObjectState resultado = InsertarCambiosEnEF(listaCamposCompAuto, mEntityContext.ComparticionAutomatica, compAutAux, filaCompAuto);
            if (resultado.Equals(ObjectState.Agregado))
            {
                mEntityContext.ComparticionAutomatica.Add(compAutAux);
            }

            if (filaCompAuto != null)
            {
                listaModificadasCompAuto.Add(filaCompAuto);

                //agegar esta fila a un DS que guarde los cambios que se van realizando para luego pasar los eventos correspondientes
                pCompAutoCambiosDW.ListaComparticionAutomatica.Add(filaCompAuto);
            }
            //marco como eliminadas las que estan en el DS pero no en el xml
            //BorrarFilasDS(pCompFormSemDS.ComparticionAutomatica, listaModificadasCompAuto);
            BorrarFilas(typeof(ComparticionAutomatica).Name, new List<object>(listaModificadasCompAuto));
            if (!mListaModificadasEF.ContainsKey(typeof(ComparticionAutomatica).Name))
            {
                mListaOriginalesEF.Add(typeof(ComparticionAutomatica).Name, new List<object>(pCompAutoCambiosDW.ListaComparticionAutomatica));
                mListaModificadasEF.Add(typeof(ComparticionAutomatica).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ComparticionAutomatica).Name].AddRange(listaModificadasCompAuto);
            #endregion ComparticionAutomatica

            #region ComparticionAutomaticaReglas
            //tabla ComparticionAutomaticaReglas
            List<ComparticionAutomaticaReglas> listaModificadasReglas = null;
            foreach (string reglaAux in pListaReglasComp.Keys)
            {
                string regla = reglaAux;
                string catDestino = pListaReglasComp[regla];

                //if (FilasPropiedadesIntegracion != null && FilasPropiedadesIntegracion.Count > 0)
                //{
                //    var propiedadMapping = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == pNomComparticion && prop.TipoObjeto.Equals((short)TipoObjeto.Comparticion) && prop.TipoPropiedad.Equals((short)TipoPropiedad.MappingComparticion));

                //    if (propiedadMapping != null && propiedadMapping.ValorPropiedad == regla + "$$$" + catDestino && propiedadMapping.Revisada && !propiedadMapping.MismoValor)
                //    {
                //        try
                //        {
                //            string[] comparticionMapping = propiedadMapping.ValorPropiedadDestino.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);

                //            if (comparticionMapping.Length > 1)
                //            {
                //                regla = comparticionMapping[0];
                //                catDestino = comparticionMapping[1];
                //            }
                //        }
                //        catch
                //        {
                //        }
                //    }
                //}

                listaModificadasReglas = new List<ComparticionAutomaticaReglas>();
                //diccionario para el control de cambios
                Dictionary<string, object> listaCamposCompAutoReglas = new Dictionary<string, object>();
                ComparticionAutomaticaReglas comparticionAutomaticaReglasAux = new ComparticionAutomaticaReglas();
                listaCamposCompAutoReglas.Add(nameof(comparticionAutomaticaReglasAux.ComparticionID), pComparticionID);
                listaCamposCompAutoReglas.Add(nameof(comparticionAutomaticaReglasAux.Regla), regla);
                listaCamposCompAutoReglas.Add(nameof(comparticionAutomaticaReglasAux.Navegacion), catDestino);

                //miro si existe esa fila en el DS
                ComparticionAutomaticaReglas filaCompAutoRegla = pCompAutoDW.ListaComparticionAutomaticaReglas.Where(item => item.ComparticionID.Equals(pComparticionID) && item.Regla.Equals(regla)).FirstOrDefault();
                //si la fila existe tengo que comprobar los cambios
                ObjectState resultadoCambios = InsertarCambiosEnEF(listaCamposCompAutoReglas, mEntityContext.ComparticionAutomaticaReglas, comparticionAutomaticaReglasAux, filaCompAutoRegla);
                if (resultadoCambios.Equals(ObjectState.Agregado))
                {
                    mEntityContext.ComparticionAutomaticaReglas.Add(comparticionAutomaticaReglasAux);
                }
                if (filaCompAutoRegla != null)
                {
                    listaModificadasReglas.Add(filaCompAutoRegla);
                    //agegar esta fila a un DS que guarde los cambios que se van realizando para luego pasar los eventos correspondientes
                    pCompAutoCambiosDW.ListaComparticionAutomaticaReglas.Add(filaCompAutoRegla);
                }
            }
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pCompAutoDS.ComparticionAutomaticaReglas, listaModificadasReglas);
            if (!mListaModificadasEF.ContainsKey(typeof(ComparticionAutomaticaReglas).Name))
            {
                mListaOriginalesEF.Add(typeof(ComparticionAutomaticaReglas).Name, new List<object>(pCompAutoCambiosDW.ListaComparticionAutomaticaReglas));
                mListaModificadasEF.Add(typeof(ComparticionAutomaticaReglas).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ComparticionAutomaticaReglas).Name].AddRange(listaModificadasReglas);

            #endregion ComparticionAutomaticaReglas

            #region ComparticionAutomaticaMapping
            //tabla ComparticionAutomaticaMapping
            List<ComparticionAutomaticaMapping> listaModificadasMapping = null;
            //contador para el grupo de mapping
            int grupoMapping = 0;
            foreach (List<string> reglas in pDuplasReglasCategorias.Keys)
            {
                List<string> categorias = pDuplasReglasCategorias[reglas];
                grupoMapping++;
                listaModificadasMapping = new List<ComparticionAutomaticaMapping>();
                foreach (string regla in reglas)
                {
                    foreach (string cat in categorias)
                    {
                        Guid tesauroID = Guid.Empty;
                        Guid categoriaTesauroID = Guid.Empty;

                        //solo se guarda fila si no es la de por defecto
                        if (!cat.Equals("###categoria###"))
                        {
                            tesauroID = pDictCatTesauro[cat][0];
                            categoriaTesauroID = pDictCatTesauro[cat][1];

                            //diccionario para el control de cambios
                            Dictionary<string, object> listaCamposCompAutoMapping = new Dictionary<string, object>();
                            ComparticionAutomaticaMapping compAutMapAux = new ComparticionAutomaticaMapping();
                            listaCamposCompAutoMapping.Add(nameof(compAutMapAux.ComparticionID), pComparticionID);
                            listaCamposCompAutoMapping.Add(nameof(compAutMapAux.ReglaMapping), regla);
                            listaCamposCompAutoMapping.Add(nameof(compAutMapAux.TesauroID), tesauroID);
                            listaCamposCompAutoMapping.Add(nameof(compAutMapAux.CategoriaTesauroID), categoriaTesauroID);
                            listaCamposCompAutoMapping.Add(nameof(compAutMapAux.GrupoMapping), grupoMapping);

                            //miro si existe esa fila en el DS
                            ComparticionAutomaticaMapping filaCompAutoMapping = pCompAutoDW.ListaComparticionAutomaticaMapping.Where(item => item.ComparticionID.Equals(pComparticionID) && item.ReglaMapping.Equals(regla) && item.TesauroID.Equals(tesauroID) && item.CategoriaTesauroID.Equals(categoriaTesauroID)).FirstOrDefault();

                            //si la fila existe tengo que comprobar los cambios
                            ObjectState resultadoCambios = InsertarCambiosEnEF(listaCamposCompAutoMapping, mEntityContext.ComparticionAutomaticaMapping, compAutMapAux, filaCompAutoMapping);
                            if (resultadoCambios.Equals(ObjectState.Agregado))
                            {
                                mEntityContext.ComparticionAutomaticaMapping.Add(compAutMapAux);
                            }
                            if (filaCompAutoMapping != null)
                            {
                                listaModificadasMapping.Add(filaCompAutoMapping);
                                //agregar esta fila a un DS que guarde los cambios que se van realizando para luego pasar los eventos correspondientes
                                pCompAutoCambiosDW.ListaComparticionAutomaticaMapping.Add(filaCompAutoMapping);
                            }
                        }
                    }
                }
                ////borro las que estan en el DS pero no en el xml
                //BorrarFilasDS(pCompAutoDS.ComparticionAutomaticaMapping, listaModificadasMapping);
                if (!mListaModificadasEF.ContainsKey(typeof(ComparticionAutomatica).Name))
                {
                    mListaOriginalesEF.Add(typeof(ComparticionAutomatica).Name, new List<object>(pCompAutoCambiosDW.ListaComparticionAutomatica));
                    mListaModificadasEF.Add(typeof(ComparticionAutomatica).Name, new List<object>());
                }
                mListaModificadasEF[typeof(ComparticionAutomatica).Name].AddRange(listaModificadasMapping);
            }
            #endregion ComparticionAutomaticaMapping

            if (filaCompAuto != null)
            {
                return filaCompAuto.ComparticionID;
            }
            else
            {
                return pComparticionID;
            }
        }

        /// <summary>
        /// Configura la SeccionProyectoCatalogo
        /// </summary>
        /// <param name="pListaSeccionProyCatalogo">Nodo xml con la configuración de SeccionProyectoCatalogo</param>
        /// <param name="pProyectoDW">Dataset del proyecto</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void ConfigurarSeccionProyectoCatalogo(XmlNodeList pListaSeccionProyCatalogo, DataWrapperProyecto pProyectoDW, Guid pOrganizacionID, Guid pProyectoID)
        {
            List<object> listaModificadas = new List<object>();
            foreach (XmlNode seccion in pListaSeccionProyCatalogo)
            {
                string nodoProyectoBusqueda = (string)LeerNodo(seccion, "ProyectoBusqueda", typeof(string));
                short ordenSeccion = (short)LeerNodo(seccion, "Orden", typeof(short));
                string nombreSeccion = (string)LeerNodo(seccion, "Nombre", typeof(string));
                string facetaSeccion = (string)LeerNodo(seccion, "Faceta", typeof(string));
                string filtroSeccion = (string)LeerNodo(seccion, "Filtro", typeof(string));
                short numeroResultados = (short)LeerNodo(seccion, "NumeroResultados", typeof(short));
                short tipoSeccion = (short)LeerNodo(seccion, "Tipo", typeof(short));
                Guid organizacionBuqueda = Guid.Empty;
                Guid proyectoBusqueda = Guid.Empty;
                //si la organizacion de busqueda viene vacío lo configuro con pOrganizacion
                if (string.IsNullOrEmpty(nodoProyectoBusqueda))
                {
                    proyectoBusqueda = pProyectoID;
                    organizacionBuqueda = pOrganizacionID;
                }
                else
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    List<Guid> lista = proyectoCL.ObtenerProyectoIDOrganizacionIDPorNombreCorto(nodoProyectoBusqueda);
                    if (lista != null)
                    {
                        organizacionBuqueda = lista[0];
                        proyectoBusqueda = lista[1];
                    }
                }
                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCamposProyCatalogo = new Dictionary<string, object>();
                SeccionProyCatalogo seccionProyCatalogoAux = new SeccionProyCatalogo();
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.OrganizacionID), pOrganizacionID);
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.ProyectoID), pProyectoID);
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.OrganizacionBusquedaID), organizacionBuqueda);
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.ProyectoBusquedaID), proyectoBusqueda);
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.Orden), ordenSeccion);
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.Nombre), nombreSeccion);
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.Faceta), facetaSeccion);
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.Filtro), filtroSeccion);


                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.NumeroResultados), numeroResultados);
                listaCamposProyCatalogo.Add(nameof(seccionProyCatalogoAux.Tipo), tipoSeccion);

                SeccionProyCatalogo filaProyCatalogo = pProyectoDW.ListaSeccionProyCatalogo.Where(seccionConsulta => seccionConsulta.OrganizacionID.Equals(pOrganizacionID) && seccionConsulta.ProyectoID.Equals(pProyectoID) && seccionConsulta.OrganizacionBusquedaID.Equals(organizacionBuqueda) && seccionConsulta.ProyectoBusquedaID.Equals(proyectoBusqueda) && seccionConsulta.Orden.Equals(ordenSeccion)).FirstOrDefault();//FindByOrganizacionIDProyectoIDOrganizacionBusquedaIDProyectoBusquedaIDOrden(pOrganizacionID, pProyectoID, organizacionBuqueda, proyectoBusqueda, ordenSeccion);
                ObjectState resultadoCambios = InsertarCambiosEnEF(listaCamposProyCatalogo, mEntityContext.SeccionProyCatalogo, seccionProyCatalogoAux, filaProyCatalogo);
                if (resultadoCambios.Equals(ObjectState.Agregado))
                {
                    mEntityContext.SeccionProyCatalogo.Add(seccionProyCatalogoAux);
                }
                if (filaProyCatalogo != null)
                {
                    listaModificadas.Add(filaProyCatalogo);
                }
            }
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pProyectoDS.SeccionProyCatalogo, listaModificadas);
            if (!mListaModificadasEF.ContainsKey(typeof(SeccionProyCatalogo).Name))
            {
                mListaOriginalesEF.Add(typeof(SeccionProyCatalogo).Name, new List<object>(pProyectoDW.ListaSeccionProyCatalogo));
                mListaModificadasEF.Add(typeof(SeccionProyCatalogo).Name, new List<object>());
            }
            mListaModificadasEF[typeof(SeccionProyCatalogo).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura el AutocompletarProyecto con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pConfigAutocompletarProy">Nodo xml con la configuración de Autocompletar proyecto</param>
        /// <param name="pListaConfigAutocompletarProy">DataSet de TagsAuto</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="listaIDsPestanyas"></param>
        /// <param name="pListaConfigAutocompletarProy"></param>
        /// <returns>booleano que indica si hay alguna configuracion de autocompletar</returns>
        private bool ConfigurarAutocompletarProy(XmlNodeList pListaConfigAutocompletarProy, Dictionary<Guid, KeyValuePair<TipoPestanyaMenu, XmlNode>> listaIDsPestanyas, DataWrapperProyecto pDataWrapperProyecto, Guid pOrganizacionID, Guid pProyectoID)
        {
            bool existenFilas = false;

            List<ConfigAutocompletarProy> listaModificadas = new List<ConfigAutocompletarProy>();
            if (pListaConfigAutocompletarProy != null && pListaConfigAutocompletarProy.Count > 0)
            {
                foreach (XmlNode config in pListaConfigAutocompletarProy)
                {
                    existenFilas = true;
                    string clave = (string)LeerNodo(config, "Clave", typeof(string));
                    string valor = (string)LeerNodo(config, "Valor", typeof(string));

                    //creo el diccionario para control de cambios para ProyectoGadget
                    Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                    ConfigAutocompletarProy configAutocompletarProyAux = new ConfigAutocompletarProy();
                    listaCampos.Add(nameof(configAutocompletarProyAux.OrganizacionID), pOrganizacionID);
                    listaCampos.Add(nameof(configAutocompletarProyAux.ProyectoID), pProyectoID);
                    listaCampos.Add(nameof(configAutocompletarProyAux.Clave), clave);
                    listaCampos.Add(nameof(configAutocompletarProyAux.Valor), valor);
                    if (clave.ToLower().Contains("FacetasPag_".ToLower()))
                    {
                        throw new Exception("Las facetas del autocompletar de una búsqueda hay que configurarlas dentro de la propia búsqueda");
                    }

                    ConfigAutocompletarProy filaAutocompletar = pDataWrapperProyecto.ListaConfigAutocompletarProy.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.Clave.Equals(clave)).FirstOrDefault();
                    ObjectState resultadoCambios = InsertarCambiosEnEF(listaCampos, mEntityContext.ConfigAutocompletarProy, configAutocompletarProyAux, filaAutocompletar);
                    if (resultadoCambios.Equals(ObjectState.Agregado))
                    {
                        mEntityContext.ConfigAutocompletarProy.Add(configAutocompletarProyAux);
                    }
                    if (filaAutocompletar != null)
                    {
                        listaModificadas.Add(filaAutocompletar);
                    }
                }
            }

            foreach (Guid idPestanya in listaIDsPestanyas.Keys)
            {
                XmlNode busqueda = listaIDsPestanyas[idPestanya].Value.SelectSingleNode("ConfiguracionBusqueda");
                if (busqueda != null)
                {
                    //Autocompletar
                    string autocompletar = "";
                    if (busqueda.SelectSingleNode("ConfigAutocompletar") != null)
                    {
                        autocompletar = (string)LeerNodo(busqueda, "ConfigAutocompletar", typeof(string));
                    }
                    if (!string.IsNullOrEmpty(autocompletar))
                    {
                        existenFilas = true;
                        string clave = "FacetasPag_" + idPestanya.ToString();

                        //creo el diccionario para control de cambios para ProyectoGadget
                        Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                        ConfigAutocompletarProy configAutocompletarProyAux = new ConfigAutocompletarProy();
                        listaCampos.Add(nameof(configAutocompletarProyAux.OrganizacionID), pOrganizacionID);
                        listaCampos.Add(nameof(configAutocompletarProyAux.ProyectoID), pProyectoID);
                        listaCampos.Add(nameof(configAutocompletarProyAux.Clave), clave);
                        listaCampos.Add(nameof(configAutocompletarProyAux.Valor), autocompletar);
                        listaCampos.Add(nameof(configAutocompletarProyAux.PestanyaID), idPestanya);

                        ConfigAutocompletarProy filaAutocompletar = pDataWrapperProyecto.ListaConfigAutocompletarProy.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.Clave.Equals(clave)).FirstOrDefault();
                        ObjectState resultadoCambios = InsertarCambiosEnEF(listaCampos, mEntityContext.ConfigAutocompletarProy, configAutocompletarProyAux, filaAutocompletar);
                        if (resultadoCambios.Equals(ObjectState.Agregado))
                        {
                            mEntityContext.ConfigAutocompletarProy.Add(configAutocompletarProyAux);
                        }
                        if (filaAutocompletar != null)
                        {
                            listaModificadas.Add(filaAutocompletar);
                        }
                    }
                }
            }

            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pTagsAutoDS.ConfigAutocompletarProy, listaModificadas);
            if (!mListaModificadasEF.ContainsKey(typeof(ConfigAutocompletarProy).Name))
            {
                mListaOriginalesEF.Add(typeof(ConfigAutocompletarProy).Name, new List<object>(pDataWrapperProyecto.ListaConfigAutocompletarProy));
                mListaModificadasEF.Add(typeof(ConfigAutocompletarProy).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ConfigAutocompletarProy).Name].AddRange(listaModificadas);
            return existenFilas;
        }

        /// <summary>
        /// Configura el campo search del proyecto con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pConfigSearchProy">Nodo xml con la configuración del search del proyecto</param>
        /// <param name="pTagsAutoDS">DataSet de TagsAuto</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void ConfigurarSearchProy(XmlNodeList pListaConfigSearchProy, List<ConfigSearchProy> pListadoConfigSearchProy, Guid pOrganizacionID, Guid pProyectoID)
        {
            List<ConfigSearchProy> listaModificadas = new List<ConfigSearchProy>();
            foreach (XmlNode config in pListaConfigSearchProy)
            {
                string clave = (string)LeerNodo(config, "Clave", typeof(string));
                string valor = (string)LeerNodo(config, "Valor", typeof(string));

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCamposConfigSearchProy = new Dictionary<string, object>();
                ConfigSearchProy configSearchProy = new ConfigSearchProy();
                listaCamposConfigSearchProy.Add(nameof(configSearchProy.OrganizacionID), pOrganizacionID);
                listaCamposConfigSearchProy.Add(nameof(configSearchProy.ProyectoID), pProyectoID);
                listaCamposConfigSearchProy.Add(nameof(configSearchProy.Clave), clave);
                listaCamposConfigSearchProy.Add(nameof(configSearchProy.Valor), valor);

                ConfigSearchProy filaProyCatalogo = pListadoConfigSearchProy.FirstOrDefault(seccionConsulta => seccionConsulta.OrganizacionID.Equals(pOrganizacionID) && seccionConsulta.ProyectoID.Equals(pProyectoID) && seccionConsulta.Clave.Equals(clave));
                ObjectState resultadoCambios = InsertarCambiosEnEF(listaCamposConfigSearchProy, mEntityContext.SeccionProyCatalogo, configSearchProy, filaProyCatalogo);
                if (resultadoCambios.Equals(ObjectState.Agregado))
                {
                    mEntityContext.ConfigSearchProy.Add(configSearchProy);
                }
                if (filaProyCatalogo != null)
                {
                    listaModificadas.Add(filaProyCatalogo);
                }
            }
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pTagsAutoDS.ConfigSearchProy, listaModificadas);
            if (!mListaModificadasEF.ContainsKey(typeof(ConfigSearchProy).Name))
            {
                mListaOriginalesEF.Add(typeof(ConfigSearchProy).Name, new List<object>(pListadoConfigSearchProy));
                mListaModificadasEF.Add(typeof(ConfigSearchProy).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ConfigSearchProy).Name].AddRange(listaModificadas);
        }


        /// <summary>
        /// Configura las utilidades
        /// </summary>
        ///<param name="listaUtilidades"></param>
        ///<param name="filaParametroGral"></param>
        ///<param name="pOrganizacionID"></param>
        ///<param name="pProyectoID"></param>
        private void ConfigurarUtilidades(XmlNode pUtilidades, ParametroGeneral pFilaParametroGral, DataWrapperVistaVirtual pVistaVirtualDW, Guid pOrganizacionID, Guid pProyectoID)
        {
            XmlNode nodoVotaciones = pUtilidades.SelectNodes("Votaciones")[0];
            if (nodoVotaciones != null)
            {
                //VotacionesDisponibles
                if (nodoVotaciones.SelectSingleNode("VotacionesDisponibles") != null)
                {
                    string strVotacionesDisponibles = ((string)LeerNodo(nodoVotaciones, "VotacionesDisponibles", typeof(string))).ToLower();
                    if (strVotacionesDisponibles.Equals("0") || strVotacionesDisponibles.Equals("false"))
                    {
                        pFilaParametroGral.VotacionesDisponibles = false;
                    }
                    else if (strVotacionesDisponibles.Equals("1") || strVotacionesDisponibles.Equals("true"))
                    {
                        pFilaParametroGral.VotacionesDisponibles = true;
                    }
                }

                //VotacionesNegativasDisponibles
                if (nodoVotaciones.SelectSingleNode("VotacionesNegativasDisponibles") != null)
                {
                    string strVotacionesNegativasDisponibles = ((string)LeerNodo(nodoVotaciones, "VotacionesNegativasDisponibles", typeof(string))).ToLower();
                    if (strVotacionesNegativasDisponibles.Equals("0") || strVotacionesNegativasDisponibles.Equals("false"))
                    {
                        pFilaParametroGral.PermitirVotacionesNegativas = false;
                    }
                    else if (strVotacionesNegativasDisponibles.Equals("1") || strVotacionesNegativasDisponibles.Equals("true"))
                    {
                        pFilaParametroGral.PermitirVotacionesNegativas = true;
                    }
                }
            }

            //InvitacionesDisponibles
            if (pUtilidades.SelectSingleNode("InvitacionesDisponibles") != null)
            {
                string strInvitacionesDisponibles = ((string)LeerNodo(pUtilidades, "InvitacionesDisponibles", typeof(string))).ToLower();
                if (strInvitacionesDisponibles.Equals("0") || strInvitacionesDisponibles.Equals("false"))
                {
                    pFilaParametroGral.InvitacionesDisponibles = false;
                }
                else if (strInvitacionesDisponibles.Equals("1") || strInvitacionesDisponibles.Equals("true"))
                {
                    pFilaParametroGral.InvitacionesDisponibles = true;
                }
            }

            if (pUtilidades.SelectSingleNode("PersonalizacionComunidad") != null)
            {
                Guid personalizacionComunidad;
                string strPersonalizacionComunidad = ((string)LeerNodo(pUtilidades, "PersonalizacionComunidad", typeof(string))).ToLower();
                if (!string.IsNullOrEmpty(strPersonalizacionComunidad) && Guid.TryParse(strPersonalizacionComunidad, out personalizacionComunidad))
                {
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                    AD.EntityModel.Models.VistaVirtualDS.VistaVirtualPersonalizacion filaPersonalizacion = pVistaVirtualDW.ListaVistaVirtualPersonalizacion.FirstOrDefault(vista => vista.PersonalizacionID.Equals(personalizacionComunidad));
                    List<AD.EntityModel.Models.VistaVirtualDS.VistaVirtualProyecto> filasPersonalizacionProyecto = pVistaVirtualDW.ListaVistaVirtualProyecto.Where(vistaVirtual => vistaVirtual.ProyectoID.Equals(ProyectoSeleccionado.Clave)).ToList();

                    if (filaPersonalizacion == null)
                    {
                        filaPersonalizacion = new AD.EntityModel.Models.VistaVirtualDS.VistaVirtualPersonalizacion();
                        filaPersonalizacion.PersonalizacionID = personalizacionComunidad;
                        pVistaVirtualDW.ListaVistaVirtualPersonalizacion.Add(filaPersonalizacion);
                    }

                    if (!vistaVirtualCN.ComprobarExistePersonalizacionID(personalizacionComunidad))
                    {

                        mEntityContext.VistaVirtualPersonalizacion.Add(filaPersonalizacion);
                    }

                    if (filasPersonalizacionProyecto != null && filasPersonalizacionProyecto.Count > 0 && filasPersonalizacionProyecto[0].PersonalizacionID != personalizacionComunidad)
                    {
                        // Modifico la personalización de esta comunidad
                        //pVistaVirtualDW.ListaVistaVirtualProyecto.FirstOrDefault().PersonalizacionID = personalizacionComunidad;
                        mEntityContext.EliminarElemento(pVistaVirtualDW.ListaVistaVirtualProyecto.FirstOrDefault());
                        /*
                        if (filasPersonalizacionProyecto.Count == 1)
                        {
                            mEntityContext.EliminarElemento(mEntityContext.VistaVirtualPersonalizacion.First(item => item.PersonalizacionID.Equals(filaPersonalizacion.PersonalizacionID)));
                        }
                        */
                        filasPersonalizacionProyecto = null;
                    }
                    if (filasPersonalizacionProyecto == null || filasPersonalizacionProyecto.Count == 0)
                    {
                        // Añado la personalización para este proyecto
                        AD.EntityModel.Models.VistaVirtualDS.VistaVirtualProyecto vistaVirtualProyect = new AD.EntityModel.Models.VistaVirtualDS.VistaVirtualProyecto();
                        vistaVirtualProyect.OrganizacionID = pOrganizacionID;
                        vistaVirtualProyect.ProyectoID = pProyectoID;
                        vistaVirtualProyect.PersonalizacionID = filaPersonalizacion.PersonalizacionID;
                        pVistaVirtualDW.ListaVistaVirtualProyecto.Add(vistaVirtualProyect);
                        mEntityContext.VistaVirtualProyecto.Add(vistaVirtualProyect);
                    }
                }
            }

            //MostrarAccionesEnListados
            if (pUtilidades.SelectSingleNode("MostrarAccionesEnListados") != null)
            {
                string strMostrarAccionesEnListados = ((string)LeerNodo(pUtilidades, "MostrarAccionesEnListados", typeof(string))).ToLower();
                if (strMostrarAccionesEnListados.Equals("0") || strMostrarAccionesEnListados.Equals("false"))
                {
                    pFilaParametroGral.MostrarAccionesEnListados = false;
                }
                else if (strMostrarAccionesEnListados.Equals("1") || strMostrarAccionesEnListados.Equals("true"))
                {
                    pFilaParametroGral.MostrarAccionesEnListados = true;
                }
            }
        }

        /// <summary>
        /// Configura los tesauros semánticos que son editables en la comunidad.
        /// </summary>
        /// <param name="pListaEdicionTesSem">Nodo xml con la configuración de los tesauros semánticos editables</param>
        /// <param name="pDataWrapperProyecto">DataSet de proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void ConfigurarEdicionTesSem(XmlNodeList pListaEdicionTesSem, DataWrapperProyecto pDataWrapperProyecto, Guid pProyectoID)
        {
            List<object> listaModificadas = new List<object>();
            foreach (XmlNode config in pListaEdicionTesSem)
            {
                string nombre = (string)LeerNodo(config, "Nombre", typeof(string));
                string prefijo = (string)LeerNodo(config, "Prefijo", typeof(string));
                string idiomas = (string)LeerNodo(config, "Idiomas", typeof(string));
                bool editable = ((string)LeerNodo(config, "Editable", typeof(string))).Equals("1");
                string source = (string)LeerNodo(config, "FuenteTesauro", typeof(string));
                string ontologia = (string)LeerNodo(config, "Ontologia", typeof(string));
                ontologia = ontologia.Trim();

                if (!ontologia.EndsWith(".owl"))
                {
                    ontologia += ".owl";
                }

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool existeOnto = docCN.ExisteOntologiaEnProyecto(pProyectoID, ontologia);

                if (!existeOnto)
                {
                    throw new Exception("Se está intentando configurar la ontología '" + ontologia + "' como Tesauro Semántico, pero no existe.");
                }

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                ProyectoConfigExtraSem proyectoConfigExtraSemAux = new ProyectoConfigExtraSem();
                listaCampos.Add(nameof(proyectoConfigExtraSemAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(proyectoConfigExtraSemAux.UrlOntologia), ontologia);
                listaCampos.Add(nameof(proyectoConfigExtraSemAux.SourceTesSem), source);
                listaCampos.Add(nameof(proyectoConfigExtraSemAux.Tipo), (short)TipoConfigExtraSemantica.TesauroSemantico);
                listaCampos.Add(nameof(proyectoConfigExtraSemAux.Nombre), nombre);
                listaCampos.Add(nameof(proyectoConfigExtraSemAux.Idiomas), idiomas);
                listaCampos.Add(nameof(proyectoConfigExtraSemAux.PrefijoTesSem), prefijo);
                listaCampos.Add(nameof(proyectoConfigExtraSemAux.Editable), editable);

                ProyectoConfigExtraSem filaExtraSem = pDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proyectoConfig => proyectoConfig.ProyectoID.Equals(pProyectoID) && proyectoConfig.UrlOntologia.Equals(ontologia) && proyectoConfig.SourceTesSem.Equals(source)).FirstOrDefault();//FindByProyectoIDUrlOntologiaSourceTesSem(pProyectoID, ontologia, source);
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.ProyectoConfigExtraSem, proyectoConfigExtraSemAux, filaExtraSem);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.ProyectoConfigExtraSem.Add(proyectoConfigExtraSemAux);
                }
                if (filaExtraSem != null)
                {
                    listaModificadas.Add(filaExtraSem);
                }
            }

            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoConfigExtraSem).Name))
            {
                mListaOriginalesEF.Add(typeof(ProyectoConfigExtraSem).Name, new List<object>(pDataWrapperProyecto.ListaProyectoConfigExtraSem));
                mListaModificadasEF.Add(typeof(ProyectoConfigExtraSem).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ProyectoConfigExtraSem).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura las ontologias de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pConfigXML">Documento xml con la configuración de la comunidad</param>
        /// <param name="pOntologias">Lista de nodos con la configuracion de las ontologías</param>
        /// <param name="pContextos">Lista de nodos con la configuracion de los contextos</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapperProyecto">DataSet de Proyectos</param>
        /// <param name="pFacetaDW">DataSet de Facetas</param>
        /// <param name="pDocumentacionCN">DocumentacionCN</param>
        private void ConfigurarOntologias(XmlDocument pConfigXML, XmlNodeList pNodosOntologias, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, DataWrapperFacetas pFacetaDW, DataWrapperTesauro pTesauroDW, DocumentacionCN pDocumentacionCN, Guid pIdProyOntologias)
        {
            ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD(mEntityContext);
            List<OntologiaProyecto> listaFilasOnt = new List<OntologiaProyecto>();
            List<CatTesauroPermiteTipoRec> listaFilasCatTesauro = new List<CatTesauroPermiteTipoRec>();
            Dictionary<CatTesauroPermiteTipoRec, string> filasTesauroOntologias = new Dictionary<CatTesauroPermiteTipoRec, string>();

            foreach (XmlNode ont in pNodosOntologias)
            {
                string ontologiaProyecto = (string)LeerNodo(ont, "Ontologia", typeof(string));
                string nombreOntologiaFaceta = (string)LeerNodo(ont, "NombreOnt", typeof(string));
                string nombreEspacio = (string)LeerNodo(ont, "Namespace", typeof(string));
                string namespacesExtra = (string)LeerNodo(ont, "NamespacesExtra", typeof(string));
                string nombreTesauro = (string)LeerNodo(ont, "NombreTesauroExclusivo", typeof(string));
                string nombreCortoOnt = (string)LeerNodo(ont, "NombreCortoOnt", typeof(string));


                bool cachearDatosSemanticos = true;
                string cachearDatos = (string)LeerNodo(ont, "CachearDatosSemanticos", typeof(string));
                if (cachearDatos.Equals("0"))
                {
                    cachearDatosSemanticos = false;
                }

                bool esBuscable = true;
                string buscable = (string)LeerNodo(ont, "EsBuscable", typeof(string));
                if (buscable.Equals("0"))
                {
                    esBuscable = false;
                }

                string subTipos = "";
                XmlNode nodoSubTipos = ont.SelectSingleNode("SubTipos");

                if (nodoSubTipos != null)
                {
                    foreach (XmlNode notoSubTipo in nodoSubTipos.SelectNodes("SubTipo"))
                    {
                        string tipoSubTipo = (string)LeerNodo(notoSubTipo, "Tipo", typeof(string));
                        string nombreSubTipo = (string)LeerNodo(notoSubTipo, "NombreSubTipo", typeof(string));

                        subTipos += tipoSubTipo + "|||" + nombreSubTipo + "[|||]";
                    }
                }

                //compruebo cambios en la ontología
                OntologiaProyecto filaOntologia = pFacetaDW.ListaOntologiaProyecto.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.OntologiaProyecto1.Equals(ontologiaProyecto)).FirstOrDefault();

                if (filaOntologia != null)
                {
                    //añado la fila a la lista de ontologías para poder borrar de la tabla las que no están en el xml
                    listaFilasOnt.Add(filaOntologia);
                }
                //creo el diccionario para control de cambios para ProyectoGadget

                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                OntologiaProyecto ontologiaProyectoAux = new OntologiaProyecto();

                listaCampos.Add(nameof(ontologiaProyectoAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(ontologiaProyectoAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(ontologiaProyectoAux.OntologiaProyecto1), ontologiaProyecto);
                listaCampos.Add(nameof(ontologiaProyectoAux.NombreOnt), nombreOntologiaFaceta);
                listaCampos.Add(nameof(ontologiaProyectoAux.Namespace), nombreEspacio);
                listaCampos.Add(nameof(ontologiaProyectoAux.NamespacesExtra), namespacesExtra);
                listaCampos.Add(nameof(ontologiaProyectoAux.SubTipos), subTipos);
                listaCampos.Add(nameof(ontologiaProyectoAux.NombreCortoOnt), nombreCortoOnt);
                listaCampos.Add(nameof(ontologiaProyectoAux.CachearDatosSemanticos), cachearDatosSemanticos);
                listaCampos.Add(nameof(ontologiaProyectoAux.EsBuscable), esBuscable);

                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.OntologiaProyecto, ontologiaProyectoAux, filaOntologia);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.OntologiaProyecto.Add(ontologiaProyectoAux);
                }

                //obtengo el ontologiaID
                Guid ontologiaID = pDocumentacionCN.ObtenerOntologiaAPartirNombre(pIdProyOntologias, ontologiaProyecto + ".owl");

                #region PresentacionListadoSemantico

                if (ontologiaID.Equals(Guid.Empty))
                {
                    ontologiaID = Guid.NewGuid();
                }

                //listado semantico contiene orden, ontologia, propiedad y nombre
                XmlNodeList listadoSemantico = ont.SelectNodes("PresentacionListadoSemantico");
                if (listadoSemantico != null && listadoSemantico.Count > 0)
                {
                    ConfigurarPresentacionListadoSemantico(listadoSemantico, pOrganizacionID, pProyectoID, pDataWrapperProyecto, ontologiaID, ontologiaProyecto);
                }
                else
                {
                    //si no viene presentación en el xml borro las filas de esa ontología del DS
                    List<PresentacionListadoSemantico> listaPlsr = pDataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID.ToString()) && presentacion.ProyectoID.Equals(pProyectoID.ToString()) && presentacion.OntologiaID.Equals(ontologiaID.ToString())).ToList();
                    foreach (PresentacionListadoSemantico plsr in listaPlsr)
                    {
                        pDataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(plsr);
                        bool existe = mEntityContext.PresentacionListadoSemantico.Any(presentacion => presentacion.OrganizacionID.Equals(plsr.OrganizacionID) && presentacion.ProyectoID.Equals(plsr.ProyectoID) && presentacion.OntologiaID.Equals(plsr.OntologiaID) && presentacion.Orden == plsr.Orden);
                        if (existe)
                        {
                            mEntityContext.EliminarElemento(plsr);
                        }
                    }
                }

                #endregion PresentacionListadoSemantico

                #region presentacionMosaicoSemantico

                //mosaico semantico contiene orden, ontologia, propiedad y nombre
                XmlNodeList mosaicoSemantico = ont.SelectNodes("PresentacionMosaicoSemantico");
                if (mosaicoSemantico != null)
                {
                    ConfigurarPresentacionMosaicoSemantico(mosaicoSemantico, pOrganizacionID, pProyectoID, pDataWrapperProyecto, ontologiaID, ontologiaProyecto);
                }
                else
                {
                    //si no viene presentación en el xml borro las filas de esa ontología del DS
                    List<PresentacionMosaicoSemantico> listaPlsr = pDataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacionMosaicoSemantico => presentacionMosaicoSemantico.OrganizacionID.Equals(pOrganizacionID) && presentacionMosaicoSemantico.ProyectoID.Equals(pProyectoID) && presentacionMosaicoSemantico.OntologiaID.Equals(ontologiaID)).ToList();
                    //"OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and OntologiaID='" + ontologiaID.ToString() + "'");
                    foreach (PresentacionMosaicoSemantico plsr in listaPlsr)
                    {
                        pDataWrapperProyecto.ListaPresentacionMosaicoSemantico.Remove(plsr);
                        proyectoGBD.DeletePresentacionMosaicoSemantico(plsr);
                        //plsr.Delete();
                    }
                }

                #endregion presentacionMosaicoSemantico

                #region presentacionMapaSemantico

                //mapa semantico contiene orden, ontologia, propiedad y nombre
                XmlNodeList mapaSemantico = ont.SelectNodes("PresentacionMapaSemantico");
                if (mapaSemantico != null)
                {
                    ConfigurarPresentacionMapaSemantico(mapaSemantico, pOrganizacionID, pProyectoID, pDataWrapperProyecto, ontologiaID, ontologiaProyecto);
                }
                else
                {
                    //si no viene presentación en el xml borro las filas de esa ontología del DS
                    List<PresentacionMapaSemantico> listaPlsr = pDataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacionMapaSemantico => presentacionMapaSemantico.OrganizacionID.Equals(pOrganizacionID) && presentacionMapaSemantico.ProyectoID.Equals(pProyectoID) && presentacionMapaSemantico.OntologiaID.Equals(ontologiaID)).ToList();
                    //("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and OntologiaID='" + ontologiaID.ToString() + "'");
                    foreach (PresentacionMapaSemantico plsr in listaPlsr)
                    {
                        pDataWrapperProyecto.ListaPresentacionMapaSemantico.Remove(plsr);
                        proyectoGBD.DeletePresentacionMapaSemantico(plsr);
                        //proyectoGBD.GuardarCambios();
                        //plsr.Delete();
                    }
                }

                #endregion presentacionMapaSemantico

                #region RecursosRelacionadosPresentacion

                XmlNodeList recursosRelacionadosPresentacion = ont.SelectNodes("RecursosRelacionadosPresentacion");
                if (recursosRelacionadosPresentacion != null && recursosRelacionadosPresentacion.Count > 0)
                {
                    ConfigurarRecursosRelacionados(recursosRelacionadosPresentacion, pOrganizacionID, pProyectoID, pDataWrapperProyecto, ontologiaID, ontologiaProyecto);
                }
                else
                {
                    //si no viene presentación en el xml borro las filas de esa ontología del DS
                    List<RecursosRelacionadosPresentacion> listaPlsr = pDataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(recurso => recurso.OrganizacionID.Equals(pOrganizacionID.ToString()) && recurso.ProyectoID.Equals(pProyectoID.ToString()) && recurso.OntologiaID.Equals(ontologiaID.ToString())).ToList();
                    foreach (RecursosRelacionadosPresentacion rrp in listaPlsr)
                    {
                        pDataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Remove(rrp);
                        proyectoGBD.DeleteRecursosRelacionadosPresentacion(rrp);
                        // proyectoGBD.GuardarCambios();
                        //rrp.Delete();
                    }
                }

                #endregion RecursosRelacionadosPresentacion

                #region CatTesauroPermiteTipoRec
                if (nombreTesauro != string.Empty)
                {
                    //busco el tesauroid y categoriatesauroid para insertar la fila en la tabla categoriatesauro
                    TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> tesauroCat = tesauroCN.ObtenerTesauroYCategoria(pProyectoID, nombreTesauro);
                    if (tesauroCat != null)
                    {
                        Guid tesauroID = tesauroCat[0];
                        Guid categoriaTesauroID = tesauroCat[1];


                        CatTesauroPermiteTipoRec fila = pTesauroDW.ListaCatTesauroPermiteTipoRec.FirstOrDefault(item => item.TesauroID.Equals(tesauroID) && item.CategoriaTesauroID.Equals(categoriaTesauroID) && item.TipoRecurso.Equals(5));
                        //si no existe la fila la añado al ds
                        if (fila == null)
                        {
                            CatTesauroPermiteTipoRec catTesauroPermiteTipoRec = new CatTesauroPermiteTipoRec();
                            catTesauroPermiteTipoRec.TesauroID = tesauroID;
                            catTesauroPermiteTipoRec.CategoriaTesauroID = categoriaTesauroID;
                            catTesauroPermiteTipoRec.TipoRecurso = 5;
                            catTesauroPermiteTipoRec.OntologiasID = ontologiaID.ToString();
                            pTesauroDW.ListaCatTesauroPermiteTipoRec.Add(catTesauroPermiteTipoRec);
                        }
                        //si existe la fila
                        else
                        {
                            if (!filasTesauroOntologias.ContainsKey(fila))
                            {
                                filasTesauroOntologias.Add(fila, ontologiaID.ToString());
                            }
                            else
                            {
                                filasTesauroOntologias[fila] += ", " + ontologiaID.ToString();
                            }

                            //listaFilasCatTesauro.Add(fila);
                        }
                    }
                }

                #endregion CatTesauroPermiteTipoRec
            }

            foreach (CatTesauroPermiteTipoRec filaTesOnt in filasTesauroOntologias.Keys)
            {
                //vuelco las ontologias de todos los objetos de conocimiento de esa categoriatesauro
                filaTesOnt.OntologiasID = filasTesauroOntologias[filaTesOnt];
                if (!listaFilasCatTesauro.Contains(filaTesOnt))
                {
                    //añado la fila a la lista de modificadas
                    listaFilasCatTesauro.Add(filaTesOnt);
                }
            }

            //añado las listas de filas que han sido modificadas para no duplicarlas
            if (!mListaModificadasEF.ContainsKey(typeof(OntologiaProyecto).Name))
            {
                mListaOriginalesEF.Add(typeof(OntologiaProyecto).Name, new List<object>(pDataWrapperProyecto.ListaOntologiaProyecto));
                mListaModificadasEF.Add(typeof(OntologiaProyecto).Name, new List<object>());
            }
            mListaModificadasEF[typeof(OntologiaProyecto).Name].AddRange(listaFilasOnt);

            if (!mListaModificadasEF.ContainsKey(typeof(CatTesauroPermiteTipoRec).Name))
            {
                mListaOriginalesEF.Add(typeof(CatTesauroPermiteTipoRec).Name, new List<object>(pTesauroDW.ListaCatTesauroPermiteTipoRec));
                mListaModificadasEF.Add(typeof(CatTesauroPermiteTipoRec).Name, new List<object>());
            }
            mListaModificadasEF[typeof(CatTesauroPermiteTipoRec).Name].AddRange(listaFilasCatTesauro);
        }

        /// <summary>
        /// Configura el listado semantico de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pListadoSemantico">Nodo xml de un listado semantico</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapper">DataSet de Proyectos</param>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        private void ConfigurarPresentacionListadoSemantico(XmlNodeList pListadosSemantico, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapper, Guid pOntologiaID, string pOntologiaProyecto)
        {
            List<object> listaModificadas = new List<object>();
            foreach (XmlNode listadoSemantico in pListadosSemantico)
            {
                short ordenListadoSem = (short)LeerNodo(listadoSemantico, "Orden", typeof(short));
                //ontologia viene heredada de <Oc> pOntologiaProyecto
                //string nombreOntologiaListado = (string)LeerNodo(listadoSemantico, "Ontologia", typeof(string));
                string propiedadListadoSem = (string)LeerNodo(listadoSemantico, "Propiedad", typeof(string));
                string nombreListado = (string)LeerNodo(listadoSemantico, "Nombre", typeof(string));

                //si no es semántica la ontologia será una cadena vacía
                string nombreOntologiaListado = "";
                if (propiedadListadoSem.Contains(":"))
                {
                    nombreOntologiaListado = "http://gnoss.com/Ontologia/" + pOntologiaProyecto + ".owl#";
                }

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                PresentacionListadoSemantico presentacionListadoSemanticoAux = new PresentacionListadoSemantico();
                listaCampos.Add(nameof(presentacionListadoSemanticoAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(presentacionListadoSemanticoAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(presentacionListadoSemanticoAux.Orden), ordenListadoSem);
                listaCampos.Add(nameof(presentacionListadoSemanticoAux.Nombre), nombreListado);
                listaCampos.Add(nameof(presentacionListadoSemanticoAux.OntologiaID), pOntologiaID);
                listaCampos.Add(nameof(presentacionListadoSemanticoAux.Ontologia), nombreOntologiaListado);
                listaCampos.Add(nameof(presentacionListadoSemanticoAux.Propiedad), propiedadListadoSem);

                PresentacionListadoSemantico filaListadoSem = pDataWrapper.ListaPresentacionListadoSemantico.Find(presentListadoSemantico => presentListadoSemantico.OrganizacionID.Equals(pOrganizacionID) && presentListadoSemantico.ProyectoID.Equals(pProyectoID) && presentListadoSemantico.OntologiaID.Equals(pOntologiaID) && presentListadoSemantico.Orden.Equals(ordenListadoSem));//FindByOrganizacionIDProyectoIDOntologiaIDOrden(pOrganizacionID, pProyectoID, pOntologiaID, ordenListadoSem);
                ObjectState resultado =InsertarCambiosEnEF(listaCampos, mEntityContext.PresentacionListadoSemantico, presentacionListadoSemanticoAux, filaListadoSem);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.PresentacionListadoSemantico.Add(presentacionListadoSemanticoAux);
                }
                if (filaListadoSem != null)
                {
                    listaModificadas.Add(filaListadoSem);
                }
            }
            if (!mListaModificadasEF.ContainsKey(typeof(PresentacionListadoSemantico).Name))
            {
                mListaOriginalesEF.Add(typeof(PresentacionListadoSemantico).Name, new List<object>(pDataWrapper.ListaPresentacionListadoSemantico));
                mListaModificadasEF.Add(typeof(PresentacionListadoSemantico).Name, new List<object>());
            }
            mListaModificadasEF[typeof(PresentacionListadoSemantico).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura el mosaico semantico de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pMosaicoSemantico">Lista de los nodos xml de un mosaico semantico</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapperProyecto">DataSet de Proyectos</param>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        private void ConfigurarPresentacionMosaicoSemantico(XmlNodeList pMosaicosSemantico, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, Guid pOntologiaID, string pOntologiaProyecto)
        {
            List<object> listaModificadas = new List<object>();
            foreach (XmlNode mosaicoSemantico in pMosaicosSemantico)
            {
                short ordenMosaicoSem = (short)LeerNodo(mosaicoSemantico, "Orden", typeof(short));
                //ontologia viene heredada de <Oc> pOntologiaProyecto
                //string nombreOntologiaMosaico = (string)LeerNodo(mosaicoSemantico, "Ontologia", typeof(string));
                string propiedadMosaicoSem = (string)LeerNodo(mosaicoSemantico, "Propiedad", typeof(string));
                string nombreMosaico = (string)LeerNodo(mosaicoSemantico, "Nombre", typeof(string));

                //si no es semántica la ontologia será una cadena vacía
                string nombreOntologiaMosaico = "";
                if (propiedadMosaicoSem.Contains(":"))
                {
                    nombreOntologiaMosaico = "http://gnoss.com/Ontologia/" + pOntologiaProyecto + ".owl#";
                }

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                PresentacionMosaicoSemantico presentacionMosaicoSemanticoAux = new PresentacionMosaicoSemantico();
                listaCampos.Add(nameof(presentacionMosaicoSemanticoAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(presentacionMosaicoSemanticoAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(presentacionMosaicoSemanticoAux.OntologiaID), pOntologiaID);
                listaCampos.Add(nameof(presentacionMosaicoSemanticoAux.Orden), ordenMosaicoSem);
                listaCampos.Add(nameof(presentacionMosaicoSemanticoAux.Nombre), nombreMosaico);
                listaCampos.Add(nameof(presentacionMosaicoSemanticoAux.Ontologia), nombreOntologiaMosaico);
                listaCampos.Add(nameof(presentacionMosaicoSemanticoAux.Propiedad), propiedadMosaicoSem);

                //busco la fila para controlar los cambios
                PresentacionMosaicoSemantico filaMosaicoSem = pDataWrapperProyecto.ListaPresentacionMosaicoSemantico.Find(presentacionMosaicoSemantico => presentacionMosaicoSemantico.OrganizacionID.Equals(pOrganizacionID) && presentacionMosaicoSemantico.ProyectoID.Equals(pProyectoID) && presentacionMosaicoSemantico.OntologiaID.Equals(pOntologiaID) && presentacionMosaicoSemantico.Orden.Equals(ordenMosaicoSem));//FindByOrganizacionIDProyectoIDOntologiaIDOrden(pOrganizacionID, pProyectoID, pOntologiaID, ordenMosaicoSem);
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.PresentacionMosaicoSemantico, presentacionMosaicoSemanticoAux, filaMosaicoSem);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.PresentacionMosaicoSemantico.Add(presentacionMosaicoSemanticoAux);
                }
                if (filaMosaicoSem != null)
                {
                    listaModificadas.Add(filaMosaicoSem);
                }
            }
            if (!mListaModificadasEF.ContainsKey(typeof(PresentacionMosaicoSemantico).Name))
            {
                mListaOriginalesEF.Add(typeof(PresentacionMosaicoSemantico).Name, new List<object>(pDataWrapperProyecto.ListaPresentacionMosaicoSemantico));
                mListaModificadasEF.Add(typeof(PresentacionMosaicoSemantico).Name, new List<object>());
            }
            mListaModificadasEF[typeof(PresentacionMosaicoSemantico).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura el mapa semantico de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pMapaSemantico">Lista de los nodos xml de un mapa semantico</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapperProyecto">DataSet de Proyectos</param>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        private void ConfigurarPresentacionMapaSemantico(XmlNodeList pMapasSemantico, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, Guid pOntologiaID, string pOntologiaProyecto)
        {
            List<object> listaModificadas = new List<object>();
            foreach (XmlNode mapaSemantico in pMapasSemantico)
            {
                short ordenMapaSem = (short)LeerNodo(mapaSemantico, "Orden", typeof(short));
                //ontologia viene heredada de <Oc> pOntologiaProyecto
                //string nombreOntologiaMapa = (string)LeerNodo(mapaSemantico, "Ontologia", typeof(string));
                string propiedadMapaSem = (string)LeerNodo(mapaSemantico, "Propiedad", typeof(string));
                string nombreMapa = (string)LeerNodo(mapaSemantico, "Nombre", typeof(string));

                //si no es semántica la ontologia será una cadena vacía
                string nombreOntologiaMapa = "";
                if (propiedadMapaSem.Contains(":"))
                {
                    nombreOntologiaMapa = "http://gnoss.com/Ontologia/" + pOntologiaProyecto + ".owl#";
                }
                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                PresentacionMapaSemantico presentacionMapaSemanticoAux = new PresentacionMapaSemantico();
                listaCampos.Add(nameof(presentacionMapaSemanticoAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(presentacionMapaSemanticoAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(presentacionMapaSemanticoAux.OntologiaID), pOntologiaID);
                listaCampos.Add(nameof(presentacionMapaSemanticoAux.Orden), ordenMapaSem);
                listaCampos.Add(nameof(presentacionMapaSemanticoAux.Nombre), nombreMapa);
                listaCampos.Add(nameof(presentacionMapaSemanticoAux.Ontologia), nombreOntologiaMapa);
                listaCampos.Add(nameof(presentacionMapaSemanticoAux.Propiedad), propiedadMapaSem);

                //Cambiar por where
                PresentacionMapaSemantico filaMapaSem = pDataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(ordenMapaSem)).FirstOrDefault();//FindByOrganizacionIDProyectoIDOntologiaIDOrden(pOrganizacionID, pProyectoID, pOntologiaID, ordenMapaSem);
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.PresentacionMapaSemantico, presentacionMapaSemanticoAux, filaMapaSem);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.PresentacionMapaSemantico.Add(presentacionMapaSemanticoAux);
                }
                if (filaMapaSem != null)
                {
                    listaModificadas.Add(filaMapaSem);
                }
            }
            if (!mListaModificadasEF.ContainsKey(typeof(PresentacionMapaSemantico).Name))
            {
                mListaOriginalesEF.Add(typeof(PresentacionMapaSemantico).Name, new List<object>(pDataWrapperProyecto.ListaPresentacionMapaSemantico));
                mListaModificadasEF.Add(typeof(PresentacionMapaSemantico).Name, new List<object>());
            }
            mListaModificadasEF[typeof(PresentacionMapaSemantico).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura el listado semantico de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pListadoSemantico">Nodo xml de un listado semantico</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapperProyecto">DataSet de Proyectos</param>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        private void ConfigurarPresentacionListadoSemanticoPestanya(XmlNodeList pListadosSemantico, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, Guid pPestanyaID)
        {
            List<object> listaModificadas = new List<object>();
            foreach (XmlNode listadoSemantico in pListadosSemantico)
            {
                short ordenListadoSem = (short)LeerNodo(listadoSemantico, "Orden", typeof(short));
                string propiedadListadoSem = (string)LeerNodo(listadoSemantico, "Propiedad", typeof(string));
                string nombreListado = (string)LeerNodo(listadoSemantico, "Nombre", typeof(string));
                string nombreOntologiaListado = (string)LeerNodo(listadoSemantico, "Ontologia", typeof(string));
                Guid ontologiaID = new Guid((string)LeerNodo(listadoSemantico, "OntologiaID", typeof(string)));

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                PresentacionPestanyaListadoSemantico presentacionPestanyaListadoSemanticoAux = new PresentacionPestanyaListadoSemantico();
                listaCampos.Add(nameof(presentacionPestanyaListadoSemanticoAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(presentacionPestanyaListadoSemanticoAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(presentacionPestanyaListadoSemanticoAux.Orden), ordenListadoSem);
                listaCampos.Add(nameof(presentacionPestanyaListadoSemanticoAux.Nombre), nombreListado);
                listaCampos.Add(nameof(presentacionPestanyaListadoSemanticoAux.OntologiaID), ontologiaID);
                listaCampos.Add(nameof(presentacionPestanyaListadoSemanticoAux.Ontologia), nombreOntologiaListado);
                listaCampos.Add(nameof(presentacionPestanyaListadoSemanticoAux.Propiedad), propiedadListadoSem);
                listaCampos.Add(nameof(presentacionPestanyaListadoSemanticoAux.PestanyaID), pPestanyaID);

                PresentacionPestanyaListadoSemantico filaListadoSem = pDataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.PestanyaID.Equals(pPestanyaID) && presentacion.OntologiaID.Equals(ontologiaID) && presentacion.Orden.Equals(ordenListadoSem)).FirstOrDefault();//FindByOrganizacionIDProyectoIDPestanyaIDOntologiaIDOrden(pOrganizacionID, pProyectoID, pPestanyaID, ontologiaID, ordenListadoSem);
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.PresentacionPestanyaListadoSemantico, presentacionPestanyaListadoSemanticoAux, filaListadoSem);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.PresentacionPestanyaListadoSemantico.Add(presentacionPestanyaListadoSemanticoAux);
                }
                if (filaListadoSem != null)
                {
                    listaModificadas.Add(filaListadoSem);
                }
            }
            if (!mListaModificadasEF.ContainsKey(typeof(PresentacionPestanyaListadoSemantico).Name))
            {
                mListaOriginalesEF.Add(typeof(PresentacionPestanyaListadoSemantico).Name, new List<object>(pDataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico));
                mListaModificadasEF.Add(typeof(PresentacionPestanyaListadoSemantico).Name, new List<object>());
            }
            mListaModificadasEF[typeof(PresentacionPestanyaListadoSemantico).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura el mosaico semantico de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pMosaicoSemantico">Lista de los nodos xml de un mosaico semantico</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapperProyecto">DataSet de Proyectos</param>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        private void ConfigurarPresentacionMosaicoSemanticoPestanya(XmlNodeList pMosaicosSemantico, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, Guid pPestanyaID)
        {
            List<Object> listaModificadas = new List<Object>();
            foreach (XmlNode mosaicoSemantico in pMosaicosSemantico)
            {
                short ordenMosaicoSem = (short)LeerNodo(mosaicoSemantico, "Orden", typeof(short));
                string propiedadMosaicoSem = (string)LeerNodo(mosaicoSemantico, "Propiedad", typeof(string));
                string nombreMosaico = (string)LeerNodo(mosaicoSemantico, "Nombre", typeof(string));
                string nombreOntologiaMosaico = (string)LeerNodo(mosaicoSemantico, "Ontologia", typeof(string));
                Guid ontologiaID = (Guid)LeerNodo(mosaicoSemantico, "OntologiaID", typeof(Guid));

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                PresentacionPestanyaMosaicoSemantico presentacionPestanyaMosaicoSemanticoAux = new PresentacionPestanyaMosaicoSemantico();
                listaCampos.Add(nameof(presentacionPestanyaMosaicoSemanticoAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(presentacionPestanyaMosaicoSemanticoAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(presentacionPestanyaMosaicoSemanticoAux.OntologiaID), ontologiaID);
                listaCampos.Add(nameof(presentacionPestanyaMosaicoSemanticoAux.Orden), ordenMosaicoSem);
                listaCampos.Add(nameof(presentacionPestanyaMosaicoSemanticoAux.Nombre), nombreMosaico);
                listaCampos.Add(nameof(presentacionPestanyaMosaicoSemanticoAux.Ontologia), nombreOntologiaMosaico);
                listaCampos.Add(nameof(presentacionPestanyaMosaicoSemanticoAux.Propiedad), propiedadMosaicoSem);
                listaCampos.Add(nameof(presentacionPestanyaMosaicoSemanticoAux.PestanyaID), pPestanyaID);

                //busco la fila para controlar los cambios
                PresentacionPestanyaMosaicoSemantico filaMosaicoSem = pDataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.PestanyaID.Equals(pPestanyaID) && presentacion.OntologiaID.Equals(ontologiaID) && presentacion.Orden.Equals(ordenMosaicoSem)).FirstOrDefault();//FindByOrganizacionIDProyectoIDPestanyaIDOntologiaIDOrden(pOrganizacionID, pProyectoID, pPestanyaID, ontologiaID, ordenMosaicoSem);
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.PresentacionPestanyaMosaicoSemantico, presentacionPestanyaMosaicoSemanticoAux, filaMosaicoSem);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.PresentacionPestanyaMosaicoSemantico.Add(presentacionPestanyaMosaicoSemanticoAux);
                }
                if (filaMosaicoSem != null)
                {
                    listaModificadas.Add(filaMosaicoSem);
                }
            }
            if (!mListaModificadasEF.ContainsKey(typeof(PresentacionPestanyaMosaicoSemantico).Name))
            {
                mListaOriginalesEF.Add(typeof(PresentacionPestanyaMosaicoSemantico).Name, new List<object>(pDataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico));
                mListaModificadasEF.Add(typeof(PresentacionPestanyaMosaicoSemantico).Name, new List<Object>());
            }
            mListaModificadasEF[typeof(PresentacionPestanyaMosaicoSemantico).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura el mapa semantico de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pMapaSemantico">Lista de los nodos xml de un mapa semantico</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapperProyecto">DataSet de Proyectos</param>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        private void ConfigurarPresentacionMapaSemanticoPestanya(XmlNodeList pMapasSemantico, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, Guid pPestanyaID)
        {
            List<Object> listaModificadas = new List<Object>();
            foreach (XmlNode mapaSemantico in pMapasSemantico)
            {
                short ordenMapaSem = (short)LeerNodo(mapaSemantico, "Orden", typeof(short));
                string propiedadMapaSem = (string)LeerNodo(mapaSemantico, "Propiedad", typeof(string));
                string nombreMapa = (string)LeerNodo(mapaSemantico, "Nombre", typeof(string));
                string nombreOntologiaMapa = (string)LeerNodo(mapaSemantico, "Ontologia", typeof(string));
                Guid ontologiaID = (Guid)LeerNodo(mapaSemantico, "OntologiaID", typeof(Guid));

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                PresentacionPestanyaMapaSemantico presentacionPestanyaMapaSemanticoAux = new PresentacionPestanyaMapaSemantico();
                listaCampos.Add(nameof(presentacionPestanyaMapaSemanticoAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(presentacionPestanyaMapaSemanticoAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(presentacionPestanyaMapaSemanticoAux.OntologiaID), ontologiaID);
                listaCampos.Add(nameof(presentacionPestanyaMapaSemanticoAux.Orden), ordenMapaSem);
                listaCampos.Add(nameof(presentacionPestanyaMapaSemanticoAux.Nombre), nombreMapa);
                listaCampos.Add(nameof(presentacionPestanyaMapaSemanticoAux.Ontologia), nombreOntologiaMapa);
                listaCampos.Add(nameof(presentacionPestanyaMapaSemanticoAux.Propiedad), propiedadMapaSem);
                listaCampos.Add(nameof(presentacionPestanyaMapaSemanticoAux.PestanyaID), pPestanyaID);

                PresentacionPestanyaMapaSemantico filaMapaSem = pDataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.PestanyaID.Equals(pPestanyaID) && presentacion.OntologiaID.Equals(ontologiaID) && presentacion.Orden.Equals(ordenMapaSem)).FirstOrDefault();//.FindByOrganizacionIDProyectoIDPestanyaIDOntologiaIDOrden(pOrganizacionID, pProyectoID, pPestanyaID, ontologiaID, ordenMapaSem);
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.PresentacionPestanyaMapaSemantico, presentacionPestanyaMapaSemanticoAux, filaMapaSem);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.PresentacionPestanyaMapaSemantico.Add(presentacionPestanyaMapaSemanticoAux);
                }
                if (filaMapaSem != null)
                {
                    listaModificadas.Add(filaMapaSem);
                }
            }
            if (!mListaModificadasEF.ContainsKey(typeof(PresentacionPestanyaMapaSemantico).Name))
            {
                mListaOriginalesEF.Add(typeof(PresentacionPestanyaMapaSemantico).Name, new List<object>(pDataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico));
                mListaModificadasEF.Add(typeof(PresentacionPestanyaMapaSemantico).Name, new List<Object>());
            }
            mListaModificadasEF[typeof(PresentacionPestanyaMapaSemantico).Name].AddRange(listaModificadas);
        }


        /// <summary>
        /// Configura los RecursosRelacionados de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pRecurso">Lista de los nodos xml de los recursos relacionados de un objeto de conocimiento</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapperProyecto">DataSet de Proyectos</param>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pOntologiaProyecto">Url de la ontolgia</param>
        private void ConfigurarRecursosRelacionados(XmlNodeList pRecursosRelacionadosPresentacion, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, Guid pOntologiaID, string pOntologiaProyecto)
        {
            List<object> listaModificadas = new List<object>();
            foreach (XmlNode recurso in pRecursosRelacionadosPresentacion)
            {
                short orden = (short)LeerNodo(recurso, "Orden", typeof(short));
                string propiedadRecursosPresentacion = (string)LeerNodo(recurso, "Propiedad", typeof(string));
                string nombreRecursosPresentacion = (string)LeerNodo(recurso, "Nombre", typeof(string));
                //short imagen = (short)LeerNodo(recurso, "Imagen", typeof(short));
                string nodoImagen = (string)LeerNodo(recurso, "Imagen", typeof(string));
                short imagen = 0;
                switch (nodoImagen)
                {
                    case "sin imagen":
                        imagen = 0;
                        break;
                    case "imagen normal":
                        imagen = 1;
                        break;
                    case "imagen reducida":
                        imagen = 2;
                        break;
                }

                string nombreOntologiaRecursos = "http://gnoss.com/Ontologia/" + pOntologiaProyecto + ".owl#";

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                RecursosRelacionadosPresentacion recusosRelacionadosPresentacionAux = new RecursosRelacionadosPresentacion();
                listaCampos.Add(nameof(recusosRelacionadosPresentacionAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(recusosRelacionadosPresentacionAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(recusosRelacionadosPresentacionAux.Orden), orden);
                listaCampos.Add(nameof(recusosRelacionadosPresentacionAux.OntologiaID), pOntologiaID);
                listaCampos.Add(nameof(recusosRelacionadosPresentacionAux.Ontologia), nombreOntologiaRecursos);
                listaCampos.Add(nameof(recusosRelacionadosPresentacionAux.Propiedad), propiedadRecursosPresentacion);
                listaCampos.Add(nameof(recusosRelacionadosPresentacionAux.Nombre), nombreRecursosPresentacion);
                listaCampos.Add(nameof(recusosRelacionadosPresentacionAux.Imagen), imagen);

                //obtengo la fila de RecursosRelacionadosPresentacion
                RecursosRelacionadosPresentacion filaRecursosRelacionados = pDataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(orden)).FirstOrDefault();//.FindByOrganizacionIDProyectoIDOntologiaIDOrden(pOrganizacionID, pProyectoID, pOntologiaID, orden);
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.RecursosRelacionadosPresentacion, recusosRelacionadosPresentacionAux, filaRecursosRelacionados);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.RecursosRelacionadosPresentacion.Add(recusosRelacionadosPresentacionAux);
                }
                if (filaRecursosRelacionados != null)
                {
                    listaModificadas.Add(filaRecursosRelacionados);
                }
            }
            if (!mListaModificadasEF.ContainsKey(typeof(RecursosRelacionadosPresentacion).Name))
            {
                mListaOriginalesEF.Add(typeof(RecursosRelacionadosPresentacion).Name, new List<object>(pDataWrapperProyecto.ListaRecursosRelacionadosPresentacion));
                mListaModificadasEF.Add(typeof(RecursosRelacionadosPresentacion).Name, new List<object>());
            }
            mListaModificadasEF[typeof(RecursosRelacionadosPresentacion).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura el Contexto de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pContexto">Lista de los nodos xml de Contexto</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDataWrapperProyecto">DataSet de Proyectos</param>
        private void ConfigurarGadgets(XmlNodeList pGadgets, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto)
        {
            List<ProyectoGadget> listaGadgetsModificados = new List<ProyectoGadget>();
            List<object> listaGadgetsContextoModificados = new List<object>();
            List<object> listaGadgetsIdiomaModificados = new List<object>();
            List<string> listaIdiomasBBDD = mConfigService.ObtenerListaIdiomas();
            //esta lista servirá para ignorar campos que se modifican siempre, por ejemplo los autonuméricos, y que no queremos tener en cuenta
            //para el borrado de la cache en caso de cambios en el DS.
            List<string> listaCamposIgnorados = new List<string>();
            //listaCamposIgnorados.Add("Orden"); -> ahora el orden si se va a tener en cuenta para editar la fila            
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            int orden = 0;
            foreach (XmlNode gadget in pGadgets)
            {
                orden++;
                string titulo = (string)LeerNodo(gadget, "Titulo", typeof(string));
                if (string.IsNullOrEmpty(titulo))
                {
                    throw new Exception("El gadget configurado en la posición " + orden + " no tiene definido el nodo Titulo");
                }
                string nombreCorto = (string)LeerNodo(gadget, "NombreCorto", typeof(string));
                if (string.IsNullOrEmpty(nombreCorto))
                {
                    throw new Exception("El gadget " + titulo + " no tiene definido el nodo NombreCorto");
                }

                //ahora el orden se toma del orden en el que vienen los gadgets en el xml - 07/04/16
                ////eliminado 28/06/13, ahora se incrementa a partir del mayor almacenado en BD
                //short orden = (short)LeerNodo(gadget, "Orden", typeof(short));
                //if (orden > mayorOrden)
                //{
                //    mayorOrden = orden;
                //}

                string strTipoGadget = (string)LeerNodo(gadget, "Tipo", typeof(string));
                if (string.IsNullOrEmpty(strTipoGadget))
                {
                    throw new Exception("El gadget " + nombreCorto + " no tiene definido el nodo Tipo");
                }
                short tipo = -1;
                try
                {
                    tipo = (short)(TipoGadget)Enum.Parse(typeof(TipoGadget), strTipoGadget);
                }
                catch
                {
                    throw new Exception("El gadget " + nombreCorto + " tiene definido el Tipo " + strTipoGadget + " que no existe");
                }

                string contenido = "";
                string ubicacion = "10";
                short tipoUbicacion = 1;
                string clases = (string)LeerNodo(gadget, "Clases", typeof(string));
                string comunidadDestinoFiltros = (string)LeerNodo(gadget, "ComunidadDestinoFiltros", typeof(string));

                if (FilasPropiedadesIntegracion != null && FilasPropiedadesIntegracion.Count > 0)
                {
                    var propiedadFiltrosDestino = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == nombreCorto && prop.TipoObjeto.Equals((short)TipoObjeto.Gadget) && prop.TipoPropiedad.Equals((short)TipoPropiedad.FiltrosDestinoGadget));

                    if (propiedadFiltrosDestino != null && propiedadFiltrosDestino.ValorPropiedad == comunidadDestinoFiltros && propiedadFiltrosDestino.Revisada && !propiedadFiltrosDestino.MismoValor)
                    {
                        comunidadDestinoFiltros = propiedadFiltrosDestino.ValorPropiedadDestino;
                    }
                }

                bool visible = false;
                string visi = (string)LeerNodo(gadget, "Visible", typeof(string));
                if (visi.Equals("1"))
                {
                    visible = true;
                }
                bool multiIdioma = false;
                bool cargarPorAjax = false;
                if (tipo.Equals((short)TipoGadget.CMS))
                {
                    string cargaAjax = (string)LeerNodo(gadget, "CargarPorAjax", typeof(string));
                    if (cargaAjax.Equals("1"))
                    {
                        cargarPorAjax = true;
                    }
                }
                else if (tipo.Equals((short)TipoGadget.RecursosContextos))
                {
                    cargarPorAjax = true;
                }
                else if (tipo.Equals((short)TipoGadget.HtmlIncrustado))
                {
                    string multi = (string)LeerNodo(gadget, "MultiIdioma", typeof(string));
                    if (multi.Equals("1"))
                    {
                        multiIdioma = true;
                    }
                }

                //creo el diccionario para control de cambios para ProyectoGadget
                Dictionary<string, object> listaCamposProyectoGadget = new Dictionary<string, object>();
                ProyectoGadget proyectoGadgetAux = new ProyectoGadget();
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.OrganizacionID), pOrganizacionID);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.ProyectoID), pProyectoID);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Titulo), titulo);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Tipo), tipo);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Ubicacion), ubicacion);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Clases), clases);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.TipoUbicacion), tipoUbicacion);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Visible), visible);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.MultiIdioma), multiIdioma);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Orden), orden);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.CargarPorAjax), cargarPorAjax);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.ComunidadDestinoFiltros), comunidadDestinoFiltros);
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.NombreCorto), nombreCorto);

                Guid gadgetID = Guid.NewGuid();
                ProyectoGadget filaProyectoGadget = null;
                List<ProyectoGadget> filasProyGad = pDataWrapperProyecto.ListaProyectoGadget.Where(proyectGadget => proyectGadget.NombreCorto.Equals(nombreCorto)).ToList();//("NombreCorto = '" + nombreCorto + "'");
                if (filasProyGad.Count == 1)
                {
                    filaProyectoGadget = filasProyGad[0];
                    gadgetID = filaProyectoGadget.GadgetID;
                    //la añado a las modificadas
                    listaGadgetsModificados.Add(filaProyectoGadget);
                }
                else
                {
                    if (proyCN.ExisteNombreCortoProyectoGadget(nombreCorto, pProyectoID))
                    {
                        throw new Exception("La comunidad ya tiene un gadget con el NombreCorto " + nombreCorto + ". Cámbielo para poder subir el Xml");
                    }
                }
                listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.GadgetID), gadgetID);

                //si es de tipo HtmlIncrustado - Consulta - CMS, el contenido se tiene que guardar en ProyectoGadget después de tratarlo pero antes de guardar en ProyectoGadgetIdioma
                if (!tipo.Equals((short)TipoGadget.HtmlIncrustado) && !tipo.Equals((short)TipoGadget.Consulta) && !tipo.Equals((short)TipoGadget.CMS))
                {
                    listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Contenido), contenido);
                    ObjectState resultado =InsertarCambiosEnEF(listaCamposProyectoGadget, mEntityContext.ProyectoGadget, proyectoGadgetAux, filaProyectoGadget, listaCamposIgnorados);
                    if (resultado.Equals(ObjectState.Agregado))
                    {
                        mEntityContext.ProyectoGadget.Add(proyectoGadgetAux);
                    }
                }

                if (tipo.Equals((short)TipoGadget.HtmlIncrustado) || tipo.Equals((short)TipoGadget.Consulta) || tipo.Equals((short)TipoGadget.CMS))
                {
                    if (!multiIdioma)
                    {
                        string nombreNodoContenido = "";
                        switch (tipo)
                        {
                            case (short)TipoGadget.HtmlIncrustado:
                                nombreNodoContenido = "HTML";
                                contenido = (string)LeerNodo(gadget, nombreNodoContenido, typeof(string));
                                break;
                            case (short)TipoGadget.Consulta:
                                nombreNodoContenido = "UrlBusqueda";
                                contenido = (string)LeerNodo(gadget, nombreNodoContenido, typeof(string));
                                break;
                            case (short)TipoGadget.CMS:
                                nombreNodoContenido = "NombreCortoComponenteCMS";
                                contenido = (string)LeerNodo(gadget, nombreNodoContenido, typeof(string));
                                if (string.IsNullOrEmpty(contenido))
                                {
                                    nombreNodoContenido = "IDComponenteCMS";
                                    contenido = (string)LeerNodo(gadget, nombreNodoContenido, typeof(string));
                                    Guid idComp = Guid.Empty;
                                    if (!Guid.TryParse(contenido, out idComp))
                                    {
                                        throw new Exception("El gadget " + nombreCorto + " de tipo " + ((TipoGadget)tipo).ToString() + " contiene un Guid incorrecto para el nodo IDComponenteCMS " + contenido);
                                    }
                                }
                                else
                                {
                                    CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    Guid? idComp = cmsCN.ObtenerIDComponentePorNombreEnProyecto(contenido, pProyectoID);
                                    if (idComp.HasValue)
                                    {
                                        contenido = idComp.ToString();
                                    }
                                    else
                                    {
                                        throw new Exception("El gadget " + nombreCorto + " de tipo " + ((TipoGadget)tipo).ToString() + " contiene un NombreCortoComponente incorrecto para el nodo NombreCortoComponenteCMS " + contenido);
                                    }
                                }
                                break;
                        }

                        if (!string.IsNullOrEmpty(contenido))
                        {
                            listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Contenido), contenido);
                            ObjectState resultado = InsertarCambiosEnEF(listaCamposProyectoGadget, mEntityContext.ProyectoGadget, proyectoGadgetAux, filaProyectoGadget, listaCamposIgnorados);
                            if (resultado.Equals(ObjectState.Agregado))
                            {
                                mEntityContext.ProyectoGadget.Add(proyectoGadgetAux);
                            }
                        }
                        else
                        {
                            throw new Exception("El gadget " + nombreCorto + " de tipo " + ((TipoGadget)tipo).ToString() + " no tiene configurado el nodo " + nombreNodoContenido);
                        }
                    }
                    else
                    {
                        //sílo los gadget de HtmlIncrustado pueden ser multiidioma
                        if (!tipo.Equals((short)TipoGadget.HtmlIncrustado))
                        {
                            throw new Exception("El gadget " + nombreCorto + " está configurado como MultiIdioma pero sílo pueden ser multiidioma los gadgets de tipo HtmlIncrustado");
                        }
                        else
                        {
                            //si es multiidioma no hay que hacer nada con el campo contenido en ProyectoGadget y lo podemos guardar directamente porque el contenido se lleva a ProyectoGadgetIdioma
                            listaCamposProyectoGadget.Add(nameof(proyectoGadgetAux.Contenido), contenido);
                            ObjectState resultado = InsertarCambiosEnEF(listaCamposProyectoGadget, mEntityContext.ProyectoGadget, proyectoGadgetAux, filaProyectoGadget, listaCamposIgnorados);
                            if (resultado.Equals(ObjectState.Agregado))
                            {
                                mEntityContext.ProyectoGadget.Add(proyectoGadgetAux);
                            }

                            //obtener nodo contexto
                            XmlNode nodoContenidoMultiIdioma = gadget.SelectSingleNode("ContenidoMultiIdioma");
                            if (nodoContenidoMultiIdioma == null || string.IsNullOrEmpty(nodoContenidoMultiIdioma.InnerXml))
                            {
                                throw new Exception("El gadget " + nombreCorto + " está configurado como MultiIdioma pero no se ha configurado ningún idioma");
                            }

                            //lista para controlar que se configura cada idioma una única vez
                            List<string> listaNodosContenidoIdioma = new List<string>();

                            foreach (XmlNode nodoContenidoIdioma in nodoContenidoMultiIdioma.ChildNodes)
                            {
                                string idioma = nodoContenidoIdioma.Name;
                                string contenidoIdioma = nodoContenidoIdioma.InnerText;
                                if (/*TODO Javier descomentar esto listaIdiomasBBDD.ContainsKey(idioma)*/true/*BORRAR True*/)
                                {
                                    if (!listaNodosContenidoIdioma.Contains(idioma))
                                    {
                                        listaNodosContenidoIdioma.Add(idioma);

                                        //creo el diccionario para control de cambios para ProyectoGadget
                                        Dictionary<string, object> listaCamposProyectoGadgetIdioma = new Dictionary<string, object>();
                                        ProyectoGadgetIdioma auxIdioma = new ProyectoGadgetIdioma();
                                        listaCamposProyectoGadgetIdioma.Add(nameof(auxIdioma.GadgetID), gadgetID);
                                        listaCamposProyectoGadgetIdioma.Add(nameof(auxIdioma.OrganizacionID), pOrganizacionID);
                                        listaCamposProyectoGadgetIdioma.Add(nameof(auxIdioma.ProyectoID), pProyectoID);
                                        listaCamposProyectoGadgetIdioma.Add(nameof(auxIdioma.Idioma), idioma);
                                        listaCamposProyectoGadgetIdioma.Add(nameof(auxIdioma.Contenido), contenidoIdioma);

                                        ProyectoGadgetIdioma filaProyectoGadgetIdioma = pDataWrapperProyecto.ListaProyectoGadgetIdioma.Where(gadgetIdioma => gadgetIdioma.GadgetID.Equals(gadgetID) && gadgetIdioma.OrganizacionID.Equals(pOrganizacionID) && gadgetIdioma.ProyectoID.Equals(pProyectoID) && gadgetIdioma.Idioma.Equals(idioma)).FirstOrDefault();//FindByGadgetIDOrganizacionIDProyectoIDIdioma(gadgetID, pOrganizacionID, pProyectoID, idioma);

                                        if (filaProyectoGadgetIdioma != null)
                                        {
                                            listaGadgetsIdiomaModificados.Add(filaProyectoGadgetIdioma);
                                        }

                                        ObjectState resultadoCambios = InsertarCambiosEnEF(listaCamposProyectoGadgetIdioma, mEntityContext.ProyectoGadget, proyectoGadgetAux, filaProyectoGadgetIdioma, listaCamposIgnorados);
                                        if (resultadoCambios.Equals(ObjectState.Agregado))
                                        {
                                            mEntityContext.ProyectoGadget.Add(proyectoGadgetAux);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("El gadget " + nombreCorto + " tiene configurados más de un contenido para el idioma " + idioma);
                                    }
                                }
                                else
                                {
                                    throw new Exception("El gadget " + nombreCorto + " tiene configurado un contenido para un idioma inválido " + nodoContenidoIdioma.Name);
                                }
                            }
                        }
                    }
                }
                else if (tipo.Equals((short)TipoGadget.RecursosContextos))
                {
                    //obtener nodo contexto
                    XmlNode contexto = gadget.SelectSingleNode("Contexto");
                    if (contexto == null || string.IsNullOrEmpty(contexto.InnerXml))
                    {
                        throw new Exception("El gadget " + nombreCorto + " es de tipo " + TipoGadget.RecursosContextos.ToString() + " pero no se ha configurado el nodo Contexto");
                    }

                    string comunidadOrigen = (string)LeerNodo(contexto, "ComunidadOrigen", typeof(string));
                    string comunidadOrigenFiltros = (string)LeerNodo(contexto, "ComunidadOrigenFiltros", typeof(string));
                    string filtrosOrigenDestino = (string)LeerNodo(contexto, "FiltrosOrigenDestino", typeof(string));

                    if (FilasPropiedadesIntegracion != null && FilasPropiedadesIntegracion.Count > 0)
                    {
                        var propiedadComunidadOrigen = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == filaProyectoGadget.NombreCorto && prop.TipoObjeto.Equals((short)TipoObjeto.Gadget) && prop.TipoPropiedad.Equals((short)TipoPropiedad.ComunidadOrigenGadget));

                        if (propiedadComunidadOrigen != null && propiedadComunidadOrigen.ValorPropiedad == comunidadOrigen && propiedadComunidadOrigen.Revisada && !propiedadComunidadOrigen.MismoValor)
                        {
                            comunidadOrigen = propiedadComunidadOrigen.ValorPropiedadDestino;
                        }

                        var propiedadFiltrosOrigen = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == filaProyectoGadget.NombreCorto && prop.TipoObjeto.Equals((short)TipoObjeto.Gadget) && prop.TipoPropiedad.Equals((short)TipoPropiedad.FiltrosOrigenGadget));

                        if (propiedadFiltrosOrigen != null && propiedadFiltrosOrigen.ValorPropiedad == comunidadOrigenFiltros && propiedadFiltrosOrigen.Revisada && !propiedadFiltrosOrigen.MismoValor)
                        {
                            comunidadOrigenFiltros = propiedadFiltrosOrigen.ValorPropiedadDestino;
                        }

                        var propiedadRelacionOrigenDestino = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == filaProyectoGadget.NombreCorto && prop.TipoObjeto.Equals((short)TipoObjeto.Componente) && prop.TipoPropiedad.Equals((short)TipoPropiedad.RelacionOrigenDestinoGadget));

                        if (propiedadRelacionOrigenDestino != null && propiedadRelacionOrigenDestino.ValorPropiedad == filtrosOrigenDestino && propiedadRelacionOrigenDestino.Revisada && !propiedadRelacionOrigenDestino.MismoValor)
                        {
                            filtrosOrigenDestino = propiedadRelacionOrigenDestino.ValorPropiedadDestino;
                        }
                    }

                    string ordenContexto = (string)LeerNodo(contexto, "OrdenContexto", typeof(string));
                    string nodoImagen = (string)LeerNodo(contexto, "Imagen", typeof(string));
                    short imagen = 0;
                    switch (nodoImagen)
                    {
                        case "sin imagen":
                            imagen = 0;
                            break;
                        case "imagen normal":
                            imagen = 1;
                            break;
                        case "imagen reducida":
                            imagen = 2;
                            break;
                    }

                    string nodoProyectoOrigen = (string)LeerNodo(contexto, "ProyectoOrigen", typeof(string));
                    Guid proyectoOrigenID = Guid.Empty;
                    if (!string.IsNullOrEmpty(nodoProyectoOrigen))
                    {
                        bool isGuid = Guid.TryParse(nodoProyectoOrigen, out proyectoOrigenID);
                        if (!isGuid)
                        {
                            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            proyectoOrigenID = proyectoCN.ObtenerProyectoIDPorNombre(nodoProyectoOrigen);
                            if (proyectoOrigenID.Equals(Guid.Empty))
                            {
                                throw new Exception("No se ha podido obtener el identificador del proyecto a través del nombre: " + comunidadOrigen + " (puede que no está en esta BBDD)");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            string nombrecorto = comunidadOrigen.Substring(0, comunidadOrigen.LastIndexOf("/"));
                            nombrecorto = nombrecorto.Substring(nombrecorto.LastIndexOf("/") + 1);

                            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                            proyectoOrigenID = proyectoCL.ObtenerProyectoIDPorNombreCorto(nombrecorto);

                            if (proyectoOrigenID == null || proyectoOrigenID.Equals(Guid.Empty))
                            {
                                Uri uriComunidadOrigen = new Uri(comunidadOrigen);

                                //Si no se especifica el ProyectoOrigen, se busca en comunidadOrigen

                                CallParametroConfiguracionService ParametrosConfiguracion = new CallParametroConfiguracionService();
                                ParametrosConfiguracion.Url = $"{uriComunidadOrigen.Scheme}://{uriComunidadOrigen.Host}/ParametrosConfiguracion";
                                proyectoOrigenID = ParametrosConfiguracion.ProyectoIDPorNombreCorto(nombrecorto);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("No se ha podido obtener el identificador del proyecto a través de la URL: " + comunidadOrigen + ex.ToString());
                        }
                    }

                    short numRecursos = (short)LeerNodo(contexto, "NumRecursos", typeof(short));
                    string servicioResultados = (string)LeerNodo(contexto, "ServicioResultados", typeof(string));
                    bool mostrarEnlaceOriginal = false;
                    string mostrarEnl = (string)LeerNodo(contexto, "MostrarEnlaceOriginal", typeof(string));
                    if (mostrarEnl.Equals("1"))
                    {
                        mostrarEnlaceOriginal = true;
                    }
                    bool ocultarVerMas = false;
                    string ocultar = (string)LeerNodo(contexto, "OcultarVerMas", typeof(string));
                    if (ocultar.Equals("1"))
                    {
                        ocultarVerMas = true;
                    }
                    string namespacesExtraContexto = (string)LeerNodo(contexto, "NamespacesExtra", typeof(string));
                    string itemsBusqueda = (string)LeerNodo(contexto, "ItemsBusqueda", typeof(string));
                    string resultadosEliminar = (string)LeerNodo(contexto, "ResultadosEliminar", typeof(string));
                    bool nuevaPestanya = false;
                    string nuevaPest = (string)LeerNodo(contexto, "NuevaPestanya", typeof(string));
                    if (nuevaPest.Equals("1"))
                    {
                        nuevaPestanya = true;
                    }

                    bool obtenerPrivados = false;
                    string obtenerPrivadosS = (string)LeerNodo(contexto, "ObtenerPrivados", typeof(string));
                    if (obtenerPrivadosS.Equals("1"))
                    {
                        obtenerPrivados = true;
                    }

                    //creo el diccionario para control de cambios para ProyectoGadgetContexto
                    Dictionary<string, object> listaCamposProyectoGadgetContexto = new Dictionary<string, object>();
                    ProyectoGadgetContexto proyectoGadgetContextoAux = new ProyectoGadgetContexto();
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.GadgetID), gadgetID);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.OrganizacionID), pOrganizacionID);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.ProyectoID), pProyectoID);
                    //movido a ProyectoGadget 06/04/16
                    //listaCamposProyectoGadgetContexto.Add(pProyectoDS.ProyectoGadgetContexto.NombreCortoColumn, nombreCorto);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.ComunidadOrigen), comunidadOrigen);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.ComunidadOrigenFiltros), comunidadOrigenFiltros);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.FiltrosOrigenDestino), filtrosOrigenDestino);
                    //movido a ProyectoGadget 04/04/16
                    //listaCamposProyectoGadgetContexto.Add(pProyectoDS.ProyectoGadgetContexto.ComunidadDestinoFiltrosColumn, comunidadDestinoFiltros);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.OrdenContexto), ordenContexto);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.Imagen), imagen);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.ProyectoOrigenID), proyectoOrigenID);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.NumRecursos), numRecursos);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.ServicioResultados), servicioResultados);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.MostrarEnlaceOriginal), mostrarEnlaceOriginal);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.OcultarVerMas), ocultarVerMas);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.NamespacesExtra), namespacesExtraContexto);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.ItemsBusqueda), itemsBusqueda);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.ResultadosEliminar), resultadosEliminar);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.NuevaPestanya), nuevaPestanya);
                    listaCamposProyectoGadgetContexto.Add(nameof(proyectoGadgetContextoAux.ObtenerPrivados), obtenerPrivados);

                    ProyectoGadgetContexto filaProyectoGadgetContexto = pDataWrapperProyecto.ListaProyectoGadgetContexto.Where(proyecto => proyecto.GadgetID.Equals(gadgetID) && proyecto.OrganizacionID.Equals(pOrganizacionID) && proyecto.ProyectoID.Equals(pProyectoID)).FirstOrDefault();//.FindByGadgetIDOrganizacionIDProyectoID(gadgetID, pOrganizacionID, pProyectoID);

                    if (filaProyectoGadgetContexto != null)
                    {
                        listaGadgetsContextoModificados.Add(filaProyectoGadgetContexto);
                    }

                    ObjectState state = InsertarCambiosEnEF(listaCamposProyectoGadgetContexto, mEntityContext.ProyectoGadgetContexto, proyectoGadgetContextoAux, filaProyectoGadgetContexto, listaCamposIgnorados);
                    if (state.Equals(ObjectState.Agregado))
                    {
                        mEntityContext.ProyectoGadgetContexto.Add(proyectoGadgetContextoAux);
                    }
                    if (state.Equals(ObjectState.Editado))
                    {
                        //borro la cache de ese gadget
                        DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        docCL.BorrarContextosRelacionados(gadgetID);
                    }
                }
            }

            //borro las filas que están en el DS y no en el xml     
            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoGadgetContexto).Name))
            {
                mListaOriginalesEF.Add(typeof(ProyectoGadgetContexto).Name, new List<object>(pDataWrapperProyecto.ListaProyectoGadgetContexto));
                mListaModificadasEF.Add(typeof(ProyectoGadgetContexto).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ProyectoGadgetContexto).Name].AddRange(listaGadgetsContextoModificados);

            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoGadgetIdioma).Name))
            {
                mListaOriginalesEF.Add(typeof(ProyectoGadgetIdioma).Name, new List<object>(pDataWrapperProyecto.ListaProyectoGadgetIdioma));
                mListaModificadasEF.Add(typeof(ProyectoGadgetIdioma).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ProyectoGadgetIdioma).Name].AddRange(listaGadgetsIdiomaModificados);

            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoGadget).Name))
            {
                mListaOriginalesEF.Add(typeof(ProyectoGadget).Name, new List<object>(pDataWrapperProyecto.ListaProyectoGadget));
                mListaModificadasEF.Add(typeof(ProyectoGadget).Name, new List<Object>());
            }
            mListaModificadasEF[typeof(ProyectoGadget).Name].AddRange(listaGadgetsModificados);
        }

        /// <summary>
        /// Configura las FacetasExcluidas de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pFacetaExcluida">Lista de los nodos xml de Facetas Excluidas</param>
        /// <param name="pFacetaCN">FacetaCN</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDW">Dataset de Facetas</param>
        private void ConfigurarFacetasExcluidas(XmlNodeList pFacetasExcluidas, FacetaCN pFacetaCN, Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            List<FacetaExcluida> listaModificadas = new List<FacetaExcluida>();
            pFacetaCN.CargarFacetasExcluidas(pOrganizacionID, pProyectoID, pFacetaDW);
            foreach (XmlNode facetaExcluida in pFacetasExcluidas)
            {
                string faceta = facetaExcluida.InnerText;
                //creo el diccionario para control de cambios para ProyectoGadget

                FacetaExcluida facetaExcluidaAux = new FacetaExcluida();
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                listaCampos.Add(nameof(facetaExcluidaAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(facetaExcluidaAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(facetaExcluidaAux.Faceta), faceta);

                FacetaExcluida filaFacetaExcluida = mEntityContext.FacetaExcluida.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.Faceta.Equals(faceta)).FirstOrDefault();
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.FacetaExcluida, facetaExcluidaAux, filaFacetaExcluida);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.FacetaExcluida.Add(facetaExcluidaAux);
                }
                if (filaFacetaExcluida != null)
                {
                    listaModificadas.Add(filaFacetaExcluida);
                }
            }
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pFacetaDS.FacetaExcluida, listaModificadas);
            if (!mListaModificadasEF.ContainsKey(typeof(FacetaExcluida).Name))
            {
                mListaOriginalesEF.Add(typeof(FacetaExcluida).Name, new List<object>(pFacetaDW.ListaFacetaExcluida));
                mListaModificadasEF.Add(typeof(FacetaExcluida).Name, new List<object>());
            }
            mListaModificadasEF[typeof(FacetaExcluida).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura las FacetasExternas de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pFacetasEntidadesExterna">Lista de los nodos xml de EntidadesExternas</param>
        /// <param name="pFacetaCN">FacetaCN</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDW">Dataset de Facetas</param>
        private void ConfigurarFacetaEntidadesExternas(XmlNodeList pFacetasEntidadesExternas, FacetaCN pFacetaCN, Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            List<FacetaEntidadesExternas> listaModificadas = new List<FacetaEntidadesExternas>();
            pFacetaCN.CargarFacetasEntidadesExternas(pOrganizacionID, pProyectoID, pFacetaDW);
            foreach (XmlNode facetaEntidadExterna in pFacetasEntidadesExternas)
            {
                string entidadID = (string)LeerNodo(facetaEntidadExterna, "Entidad", typeof(string));
                string grafo = (string)LeerNodo(facetaEntidadExterna, "Grafo", typeof(string));

                bool esEntidadSecundaria = false;
                string secundaria = (string)LeerNodo(facetaEntidadExterna, "EsEntidadSecundaria", typeof(string));
                if (secundaria.Equals("1"))
                {
                    esEntidadSecundaria = true;
                }

                bool buscarConRecursividad = true;
                string recursividad = (string)LeerNodo(facetaEntidadExterna, "BuscarConRecursividad", typeof(string));
                if (recursividad.Equals("0"))
                {
                    buscarConRecursividad = false;
                }

                //creo el diccionario para control de cambios

                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                FacetaEntidadesExternas facetaEntidadesExternasAux = new FacetaEntidadesExternas();

                listaCampos.Add(nameof(facetaEntidadesExternasAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(facetaEntidadesExternasAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(facetaEntidadesExternasAux.EntidadID), entidadID);
                listaCampos.Add(nameof(facetaEntidadesExternasAux.Grafo), grafo);
                listaCampos.Add(nameof(facetaEntidadesExternasAux.EsEntidadSecundaria), esEntidadSecundaria);
                listaCampos.Add(nameof(facetaEntidadesExternasAux.BuscarConRecursividad), buscarConRecursividad);

                //busco la fila para el control de cambios
                FacetaEntidadesExternas filaFacetaEntidadExterna = pFacetaDW.ListaFacetaEntidadesExternas.FirstOrDefault(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.EntidadID.Equals(entidadID));
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.FacetaEntidadesExternas, facetaEntidadesExternasAux, filaFacetaEntidadExterna);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.FacetaEntidadesExternas.Add(facetaEntidadesExternasAux);
                }
                if (filaFacetaEntidadExterna != null)
                {
                    listaModificadas.Add(filaFacetaEntidadExterna);
                }
            }
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pFacetaDS.FacetaEntidadesExternas, listaModificadas);
            if (!mListaModificadasEF.ContainsKey(typeof(FacetaEntidadesExternas).Name))
            {
                mListaOriginalesEF.Add(typeof(FacetaEntidadesExternas).Name, new List<object>(pFacetaDW.ListaFacetaEntidadesExternas));
                mListaModificadasEF.Add(typeof(FacetaEntidadesExternas).Name, new List<object>());
            }
            mListaModificadasEF[typeof(FacetaEntidadesExternas).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Carga la configuración del ámbito de búsqueda, si no se le pasa nada carga la configuración por defecto.
        /// </summary>
        /// <param name="configuracionAmbitoBusqueda">Lista de nodos de los ambitos de búsqueda</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pOrganizacionID">Identificador de la Organización</param>
        private ConfiguracionAmbitoBusquedaProyecto ConfigurarAmbitoBusqueda(GestorParametroGeneral pParamGralDS, Guid pProyectoID, Guid pOrganizacionID, XmlNode ambitoTodoGnossVisible, XmlNode ambitoTodaLaComunidadVisible)
        {
            //ParametroGeneralDSName.ConfiguracionAmbitoBusquedaProyectoRow filaConfBusqueda = pParamGralDS.ConfiguracionAmbitoBusquedaProyecto.FindByOrganizacionIDProyectoID(pOrganizacionID, pProyectoID);
            ConfiguracionAmbitoBusquedaProyecto filaConfBusqueda = pParamGralDS.ListaConfiguracionAmbitoBusquedaProyecto.Find(ambitoBusqueda => ambitoBusqueda.OrganizacionID.Equals(pOrganizacionID) && ambitoBusqueda.ProyectoID.Equals(pProyectoID));

            if (filaConfBusqueda == null)
            {
                // filaConfBusqueda = pParamGralDS.ConfiguracionAmbitoBusquedaProyecto.NewConfiguracionAmbitoBusquedaProyectoRow();
                filaConfBusqueda = new ConfiguracionAmbitoBusquedaProyecto();
                filaConfBusqueda.ProyectoID = pProyectoID;
                filaConfBusqueda.OrganizacionID = pOrganizacionID;
                filaConfBusqueda.Metabusqueda = true;
                filaConfBusqueda.TodoGnoss = true;
                //pParamGralDS.ConfiguracionAmbitoBusquedaProyecto.AddConfiguracionAmbitoBusquedaProyectoRow(filaConfBusqueda);
                pParamGralDS.ListaConfiguracionAmbitoBusquedaProyecto.Add(filaConfBusqueda);
                ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                gestorController.addAmbitoBusqueda(filaConfBusqueda);
                gestorController.SaveChanges();


            }

            if (ambitoTodaLaComunidadVisible != null)
            {
                filaConfBusqueda.Metabusqueda = ambitoTodaLaComunidadVisible.InnerText.Equals("1");
            }
            else
            {
                filaConfBusqueda.Metabusqueda = true;
            }
            if (ambitoTodoGnossVisible != null)
            {
                filaConfBusqueda.TodoGnoss = ambitoTodoGnossVisible.InnerText.Equals("1");
            }
            else
            {
                filaConfBusqueda.TodoGnoss = true;
            }
            //filaConfBusqueda.SetPestanyaDefectoIDNull();
            filaConfBusqueda.PestanyaDefectoID = null;
            return filaConfBusqueda;
        }

        /// <summary>
        /// Configura las Pestañas de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pPestanyas">Lista de nodos con las pestañas de una comunidad</param>
        /// <param name="pProyFiltroOrdenRecurso">Lista de nodos con los filtros de una pestaña</param>
        /// <param name="pDataWrapperProyecto">Dataset de un proyecto determinado</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void ConfigurarPestanyasComunidadConXML(XmlNodeList pPestanyas, DataWrapperProyecto pDataWrapperProyecto, DataWrapperCMS pCMSDW, Guid pOrganizacionID, Guid pProyectoID, Guid? pPestanyaPadreID, DataWrapperExportacionBusqueda pExportacionBusquedaDW, ref Dictionary<Guid, KeyValuePair<TipoPestanyaMenu, XmlNode>> pListaIDsPestanyas, ref List<string> pListaNombresCortosPestanyas, ref List<short> pListaUbicacionesCMS, ref ConfiguracionAmbitoBusquedaProyecto pFilaConfiguracionAmbitoBusqueda)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD(mEntityContext);
            List<Object> listaPestanyasMenuModificadas = new List<Object>();
            List<Object> listaPestanyasBusquedaModificadas = new List<Object>();
            List<Object> listaPestanyasCMSModificadas = new List<Object>();
            List<Object> listaCMSPaginaModificadas = new List<Object>();
            List<Object> listaFiltrosModificadas = new List<Object>();
            List<object> listaExportacionesModificadas = new List<object>();
            List<Object> listaPestanyasMenuRolIdentidadModificadas = new List<Object>();
            List<Object> listaPestanyasMenuRolGrupoIdentidadesModificadas = new List<Object>();
            short ordenPestanya = 0;
            foreach (XmlElement pestanya in pPestanyas)
            {
                #region Configuración genérica

                //TipoPestanya
                string strTipoPestanya = (string)LeerNodo(pestanya, "TipoPestanya", typeof(string));
                short tipoPestanya = 0;
                try
                {
                    tipoPestanya = (short)(TipoPestanyaMenu)Enum.Parse(typeof(TipoPestanyaMenu), strTipoPestanya);
                }
                catch
                {
                    throw new Exception("El tipo de pestaña " + strTipoPestanya + " no es válido");
                }
                TipoPestanyaMenu tipoPestanyaEnum = (TipoPestanyaMenu)tipoPestanya;

                //PestanyaID
                Guid pestanyaID = Guid.NewGuid();
                string nombreCortoPestanya = "";
                bool pestanyaObtenida = false;
                if (pestanya.SelectSingleNode("NombreCorto") != null)
                {
                    nombreCortoPestanya = (string)LeerNodo(pestanya, "NombreCorto", typeof(string));
                    if (string.IsNullOrEmpty(nombreCortoPestanya))
                    {
                        throw new Exception("Todas las pestañas tienen que tener NombreCorto");
                    }

                    List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> filasPestanyasMenu = pDataWrapperProyecto.ListaProyectoPestanyaMenu.Where(fila => fila.NombreCortoPestanya.Equals(nombreCortoPestanya)).ToList();

                    //Obtenemos el ID de las antiguas en función del nombrecorto
                    if (filasPestanyasMenu.Count > 0)
                    {
                        pestanyaID = filasPestanyasMenu[0].PestanyaID;
                        pestanyaObtenida = true;
                        if ((short)tipoPestanyaEnum != filasPestanyasMenu[0].TipoPestanya)
                        {
                            throw new Exception("No se le puede cambiar el tipo a una pestaña (hay que cambiarle el nombrecorto a la pestaña " + nombreCortoPestanya + ")");
                        }
                    }
                }
                else
                {
                    throw new Exception("Todas las pestañas tienen que tener NombreCorto");
                }

                //listado semantico
                XmlNodeList listadoSemantico = pestanya.SelectNodes("PresentacionListadoSemantico");
                if (listadoSemantico != null && listadoSemantico.Count > 0)
                {
                    ConfigurarPresentacionListadoSemanticoPestanya(listadoSemantico, pOrganizacionID, pProyectoID, pDataWrapperProyecto, pestanyaID);
                }
                else
                {
                    //si no viene presentación en el xml borro las filas de esa ontología del DS
                    List<PresentacionPestanyaListadoSemantico> listaPlsr = pDataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.PestanyaID.Equals(pestanyaID)).ToList();
                    //("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and PestanyaID='" + pestanyaID.ToString() + "'");
                    foreach (PresentacionPestanyaListadoSemantico plsr in listaPlsr)
                    {
                        pDataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico.Remove(plsr);

                        proyectoGBD.DeletePresentacionPestanyaListadoSemantico(plsr);
                        //plsr.Delete();
                    }
                }

                //mosaico semantico
                XmlNodeList mosaicoSemantico = pestanya.SelectNodes("PresentacionMosaicoSemantico");
                if (mosaicoSemantico != null && mosaicoSemantico.Count > 0)
                {
                    ConfigurarPresentacionMosaicoSemanticoPestanya(mosaicoSemantico, pOrganizacionID, pProyectoID, pDataWrapperProyecto, pestanyaID);
                }
                else
                {

                    //si no viene presentación en el xml borro las filas de esa ontología del DS
                    List<PresentacionPestanyaMosaicoSemantico> listaPlsr = pDataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico.Where(presentacionMosaico => presentacionMosaico.OrganizacionID.Equals(pOrganizacionID) && presentacionMosaico.ProyectoID.Equals(pProyectoID) && presentacionMosaico.PestanyaID.Equals(pestanyaID)).ToList();
                    //("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and PestanyaID='" + pestanyaID.ToString() + "'");
                    foreach (PresentacionPestanyaMosaicoSemantico plsr in listaPlsr)
                    {
                        //plsr.Delete();
                        pDataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico.Remove(plsr);
                        proyectoGBD.DeletePresentacionPestanyaMosaicoSemantico(plsr);
                    }
                }

                //mapa semantico
                XmlNodeList mapaSemantico = pestanya.SelectNodes("PresentacionMapaSemantico");
                if (mapaSemantico != null && mapaSemantico.Count > 0)
                {
                    ConfigurarPresentacionMapaSemanticoPestanya(listadoSemantico, pOrganizacionID, pProyectoID, pDataWrapperProyecto, pestanyaID);
                }
                else
                {
                    //si no viene presentación en el xml borro las filas de esa ontología del DS
                    List<PresentacionPestanyaMapaSemantico> listaPlsr = pDataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico.Where(presentacionMapa => presentacionMapa.OrganizacionID.Equals(pOrganizacionID) && presentacionMapa.ProyectoID.Equals(pProyectoID) && presentacionMapa.PestanyaID.Equals(pestanyaID)).ToList();
                    //("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and PestanyaID='" + pestanyaID.ToString() + "'");
                    foreach (PresentacionPestanyaMapaSemantico plsr in listaPlsr)
                    {
                        pDataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico.Remove(plsr);
                        proyectoGBD.DeletePresentacionPestanyaMapaSemantico(plsr);
                        //plsr.Delete();
                    }
                }


                Dictionary<string, object> listaCamposPestanyaCMS = new Dictionary<string, object>();
                Dictionary<string, object> listaCamposCMSPagina = new Dictionary<string, object>();
                short? ubicacionCMS = null;
                if (tipoPestanyaEnum == TipoPestanyaMenu.CMS)
                {
                    if (pestanya.SelectSingleNode("UbicacionCMS") != null)
                    {
                        string strUbicacionCMS = (string)LeerNodo(pestanya, "UbicacionCMS", typeof(string));
                        try
                        {
                            ubicacionCMS = short.Parse(strUbicacionCMS);
                        }
                        catch (Exception)
                        {
                            throw new Exception("La UbicacionCMS de una página del CMS tiene que ser un short");
                        }
                    }
                    else
                    {
                        throw new Exception("Las pestañas del CMS tienen que tener UbicacionCMS");
                    }

                    List<ProyectoPestanyaCMS> filasPestanyasCMS = pDataWrapperProyecto.ListaProyectoPestanyaCMS.Where(fila => fila.Ubicacion.Equals(ubicacionCMS.Value)).ToList();
                    ProyectoPestanyaCMS aux = new ProyectoPestanyaCMS();
                    if (filasPestanyasCMS.Count == 0)
                    {
                        listaCamposPestanyaCMS.Add(nameof(aux.PestanyaID), pestanyaID);
                        listaCamposPestanyaCMS.Add(nameof(aux.Ubicacion), ubicacionCMS.Value);
                    }

                    //Si se ha obtenido por el nombrecorto no se puede cambiar su ubicacion
                    List<ProyectoPestanyaCMS> filasPestanyasCMSPorID = pDataWrapperProyecto.ListaProyectoPestanyaCMS.Where(fila => fila.PestanyaID.Equals(pestanyaID)).ToList();
                    if (pestanyaObtenida && filasPestanyasCMS.Count > 0)
                    {
                        if (ubicacionCMS.Value != filasPestanyasCMSPorID[0].Ubicacion)
                        {
                            throw new Exception("No se le puede cambiar la ubicación a una pestanya del CMS");
                        }
                    }

                    if (filasPestanyasCMS.Count > 0 && !pestanyaObtenida)
                    {
                        pestanyaObtenida = true;
                        pestanyaID = filasPestanyasCMS[0].PestanyaID;
                    }

                    if (!pestanyaObtenida)
                    {
                        listaCamposCMSPagina.Add(nameof(AD.EntityModel.Models.CMS.CMSPagina.OrganizacionID), pOrganizacionID);
                        listaCamposCMSPagina.Add(nameof(AD.EntityModel.Models.CMS.CMSPagina.ProyectoID), pProyectoID);
                        listaCamposCMSPagina.Add(nameof(AD.EntityModel.Models.CMS.CMSPagina.Ubicacion), ubicacionCMS.Value);
                        listaCamposCMSPagina.Add(nameof(AD.EntityModel.Models.CMS.CMSPagina.Activa), false);
                        listaCamposCMSPagina.Add(nameof(AD.EntityModel.Models.CMS.CMSPagina.MostrarSoloCuerpo), false);
                    }
                }

                if (!pListaNombresCortosPestanyas.Contains(nombreCortoPestanya))
                {
                    pListaNombresCortosPestanyas.Add(nombreCortoPestanya);
                }
                else
                {
                    throw new Exception("No puede haber más de una pestaña con el mismo nombrecorto '" + nombreCortoPestanya + "'");
                }

                if (ubicacionCMS.HasValue)
                {
                    if (!pListaUbicacionesCMS.Contains(ubicacionCMS.Value))
                    {
                        pListaUbicacionesCMS.Add(ubicacionCMS.Value);
                    }
                    else
                    {
                        throw new Exception("No puede haber más de una pestaña del CMS con la misma ubicación '" + ubicacionCMS.Value.ToString() + "'");
                    }
                }

                foreach (Guid key in pListaIDsPestanyas.Keys)
                {
                    TipoPestanyaMenu tipo = pListaIDsPestanyas[key].Key;
                    if ((tipo == TipoPestanyaMenu.BusquedaAvanzada
                        || tipo == TipoPestanyaMenu.Debates
                        || tipo == TipoPestanyaMenu.Encuestas
                        || tipo == TipoPestanyaMenu.Home
                        || tipo == TipoPestanyaMenu.PersonasYOrganizaciones
                        || tipo == TipoPestanyaMenu.Preguntas
                        || tipo == TipoPestanyaMenu.Recursos) && tipoPestanyaEnum == tipo)
                    {
                        throw new Exception("No puede haber mas de una pestaña del tipo " + strTipoPestanya);
                    }
                }

                if (!pListaIDsPestanyas.ContainsKey(pestanyaID))
                {
                    pListaIDsPestanyas.Add(pestanyaID, new KeyValuePair<TipoPestanyaMenu, XmlNode>(tipoPestanyaEnum, pestanya));
                }
                else
                {
                    throw new Exception("No puede haber dos pestañas con el mismo ID");
                }
                //PestanyaDefecto
                if (pestanya.SelectSingleNode("PestanyaDefecto") != null)
                {
                    if (tipoPestanyaEnum != TipoPestanyaMenu.Debates
                        && tipoPestanyaEnum != TipoPestanyaMenu.Encuestas
                        && tipoPestanyaEnum != TipoPestanyaMenu.PersonasYOrganizaciones
                        && tipoPestanyaEnum != TipoPestanyaMenu.Preguntas
                        && tipoPestanyaEnum != TipoPestanyaMenu.Recursos
                        && tipoPestanyaEnum != TipoPestanyaMenu.BusquedaSemantica)
                    {
                        throw new Exception("El atributo PestanyaDefecto solo puede aplicarse a las páginas de tipos: Debates, Encuestas, PersonasYOrganizaciones, Preguntas, Recursos y Busqueda");
                    }

                    string strPestanyaDefecto = ((string)LeerNodo(pestanya, "PestanyaDefecto", typeof(string))).ToLower();
                    if (strPestanyaDefecto.Equals("1") || strPestanyaDefecto.Equals("true"))
                    {
                        //if (pFilaConfiguracionAmbitoBusqueda.IsPestanyaDefectoIDNull())
                        if (pFilaConfiguracionAmbitoBusqueda.PestanyaDefectoID == null)
                        {
                            pFilaConfiguracionAmbitoBusqueda.PestanyaDefectoID = pestanyaID;
                        }
                        else
                        {
                            throw new Exception("Sílo puede haber una pestaña de busqueda con el atributo PestanyaDefecto=1");
                        }
                    }
                    else
                    {
                        throw new Exception("El valor " + strPestanyaDefecto + " para el campo visible no es válido");
                    }
                }

                //Visible
                bool visible = false;
                if (pestanya.SelectSingleNode("Visible") != null)
                {
                    string strVisible = ((string)LeerNodo(pestanya, "Visible", typeof(string))).ToLower();
                    if (strVisible.Equals("0") || strVisible.Equals("false"))
                    {
                        visible = false;
                    }
                    else if (strVisible.Equals("1") || strVisible.Equals("true"))
                    {
                        visible = true;
                    }
                    else
                    {
                        throw new Exception("El valor " + strVisible + " para el campo visible no es válido");
                    }
                }
                else
                {
                    throw new Exception("El campo Visible en las pestañas es obligatorio");
                }

                //VisibleSinAcceso
                bool visibleSinAcceso = true;
                if (pestanya.SelectSingleNode("VisibleSinAcceso") != null)
                {
                    string strVisibleSinAcceso = ((string)LeerNodo(pestanya, "VisibleSinAcceso", typeof(string))).ToLower();
                    if (strVisibleSinAcceso.Equals("0") || strVisibleSinAcceso.Equals("false"))
                    {
                        visibleSinAcceso = false;
                    }
                    else if (strVisibleSinAcceso.Equals("1") || strVisibleSinAcceso.Equals("true"))
                    {
                        visibleSinAcceso = true;
                    }
                    else
                    {
                        throw new Exception("El valor " + strVisibleSinAcceso + " para el campo VisibleSinAcceso no es válido");
                    }
                }

                //Nombre
                string nombrePestanya = (string)LeerNodo(pestanya, "Nombre", typeof(string));
                if ((tipoPestanyaEnum.Equals(TipoPestanyaMenu.CMS) || tipoPestanyaEnum.Equals(TipoPestanyaMenu.BusquedaSemantica) || tipoPestanyaEnum.Equals(TipoPestanyaMenu.EnlaceInterno) || tipoPestanyaEnum.Equals(TipoPestanyaMenu.EnlaceExterno)) && string.IsNullOrEmpty(nombrePestanya))
                {
                    throw new Exception("El campo Nombre en las pestañas del tipo " + strTipoPestanya + " es obligatorio");
                }

                //Ruta
                string rutaPestanya = (string)LeerNodo(pestanya, "Ruta", typeof(string));
                if ((tipoPestanyaEnum.Equals(TipoPestanyaMenu.CMS) || tipoPestanyaEnum.Equals(TipoPestanyaMenu.BusquedaSemantica) || tipoPestanyaEnum.Equals(TipoPestanyaMenu.EnlaceInterno) || tipoPestanyaEnum.Equals(TipoPestanyaMenu.EnlaceExterno)) && string.IsNullOrEmpty(rutaPestanya))
                {
                    throw new Exception("El campo Ruta en las pestañas del tipo " + strTipoPestanya + " es obligatorio");
                }

                //Titulo
                string tituloPestanya = (string)LeerNodo(pestanya, "Titulo", typeof(string));

                //IdiomaDisponibles
                string idiomasDisponibles = "";
                if (pestanya.SelectSingleNode("IdiomasDisponibles") != null)
                {
                    string stIdiomasDisponibles = ((string)LeerNodo(pestanya, "IdiomasDisponibles", typeof(string))).ToLower();
                    List<string> listaIdiomas = stIdiomasDisponibles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<string> listaIdiomasBBDD = mConfigService.ObtenerListaIdiomas();
                    foreach (string idioma in listaIdiomas)
                    {
                        if (listaIdiomasBBDD.Contains(idioma))
                        {
                            idiomasDisponibles += "true@" + idioma + "|||";
                        }
                    }
                }

                //NuevaPestanya
                bool nuevaPestanya = false;
                if (pestanya.SelectSingleNode("NuevaPestanya") != null)
                {
                    string strNuevaPestanya = ((string)LeerNodo(pestanya, "NuevaPestanya", typeof(string))).ToLower();
                    if (strNuevaPestanya.Equals("0") || strNuevaPestanya.Equals("false"))
                    {
                        nuevaPestanya = false;
                    }
                    else if (strNuevaPestanya.Equals("1") || strNuevaPestanya.Equals("true"))
                    {
                        nuevaPestanya = true;
                    }
                    else
                    {
                        throw new Exception("El valor " + strNuevaPestanya + " para el campo NuevaPestanya no es válido");
                    }
                }

                //CSSBodyClass
                string claseBody = (string)LeerNodo(pestanya, "CSSBodyClass", typeof(string));

                //Activa
                bool activa = true;
                if (pestanya.SelectSingleNode("Activa") != null)
                {
                    string strActiva = ((string)LeerNodo(pestanya, "Activa", typeof(string))).ToLower();
                    if (strActiva.Equals("0") || strActiva.Equals("false"))
                    {
                        activa = false;
                    }
                    else if (strActiva.Equals("1") || strActiva.Equals("true"))
                    {
                        activa = true;
                    }
                    else
                    {
                        throw new Exception("El valor " + strActiva + " para el campo activa no es válido");
                    }
                }

                #endregion

                #region Privacidad

                short privacidadPestanya = 0;
                string htmlAlternativoPestanya = "";
                if (pestanya.SelectSingleNode("Privacidad") != null)
                {
                    //Privacidad
                    string strPrivacidadPestanya = (string)LeerNodo(pestanya, "Privacidad", typeof(string));
                    privacidadPestanya = (short)(TipoPrivacidadPagina)Enum.Parse(typeof(TipoPrivacidadPagina), strPrivacidadPestanya);

                    if (privacidadPestanya != (short)TipoPrivacidadPagina.Normal)
                    {
                        //HtmlAlternativo
                        if (pestanya.SelectSingleNode("HtmlAlternativo") != null)
                        {
                            htmlAlternativoPestanya = (string)LeerNodo(pestanya, "HtmlAlternativo", typeof(string));
                        }
                    }

                    if (privacidadPestanya == (short)TipoPrivacidadPagina.Lectores)
                    {
                        //Configuración Privacidad
                        int numLectores = 0;
                        if (pestanya.SelectSingleNode("ConfiguracionPrivacidad") != null)
                        {
                            XmlNode nodoConfiguracionPrivacidad = pestanya.SelectSingleNode("ConfiguracionPrivacidad");

                            if (nodoConfiguracionPrivacidad.SelectSingleNode("PrivacidadPerfiles") != null)
                            {
                                //perfiles
                                XmlNode nodoPerfiles = nodoConfiguracionPrivacidad.SelectSingleNode("PrivacidadPerfiles");
                                XmlNodeList perfiles = nodoPerfiles.SelectNodes("Perfil");
                                foreach (XmlNode nodoPerfil in perfiles)
                                {
                                    string nombreCortoPerfil = nodoPerfil.InnerText;
                                    Guid idPerfil = identidadCN.ObtenerPerfilIDPorNombreCorto(nombreCortoPerfil);
                                    if (idPerfil != Guid.Empty)
                                    {
                                        numLectores++;
                                        Dictionary<string, object> listaCamposPestanyaMenuRolIdentidad = new Dictionary<string, object>();
                                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad proyectoPenstanyaMenuRolIdentidadAux = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad();
                                        listaCamposPestanyaMenuRolIdentidad.Add(nameof(proyectoPenstanyaMenuRolIdentidadAux.PestanyaID), pestanyaID);
                                        listaCamposPestanyaMenuRolIdentidad.Add(nameof(proyectoPenstanyaMenuRolIdentidadAux.PerfilID), idPerfil);

                                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaPestanyaMenuRolIdentidad = pDataWrapperProyecto.ListaProyectoPestanyaMenuRolIdentidad.Find(proyectoPestanyaMenuRolId => proyectoPestanyaMenuRolId.PestanyaID.Equals(pestanyaID) && proyectoPestanyaMenuRolId.PerfilID.Equals(idPerfil));//.FindByPestanyaIDPerfilID(pestanyaID, idPerfil);

                                        ObjectState resultadoCambios = InsertarCambiosEnEF(listaCamposPestanyaMenuRolIdentidad, mEntityContext.ProyectoPestanyaMenuRolIdentidad, proyectoPenstanyaMenuRolIdentidadAux, filaPestanyaMenuRolIdentidad);
                                        if (resultadoCambios.Equals(ObjectState.Agregado))
                                        {
                                            mEntityContext.ProyectoPestanyaMenuRolIdentidad.Add(proyectoPenstanyaMenuRolIdentidadAux);
                                        }
                                        if (filaPestanyaMenuRolIdentidad != null)
                                        {
                                            listaPestanyasMenuRolIdentidadModificadas.Add(filaPestanyaMenuRolIdentidad);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("El perfil " + nombreCortoPerfil + " no existe");
                                    }
                                }

                            }
                            if (nodoConfiguracionPrivacidad.SelectSingleNode("PrivacidadGrupos") != null)
                            {
                                //Grupos
                                XmlNode nodoGrupos = nodoConfiguracionPrivacidad.SelectSingleNode("PrivacidadGrupos");

                                //GruposOrg
                                XmlNodeList gruposOrg = nodoGrupos.SelectNodes("GrupoOrg");
                                foreach (XmlNode nodoGrupoOrg in gruposOrg)
                                {
                                    string nombreCortoGrupo = (string)LeerNodo(nodoGrupoOrg, "NombreCortoGrupo", typeof(string));
                                    string nombreCortoOrg = (string)LeerNodo(nodoGrupoOrg, "NombreCortoOrg", typeof(string));

                                    OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    Guid idOrganizacion = orgCN.ObtenerOrganizacionesIDPorNombre(nombreCortoOrg);
                                    DataWrapperIdentidad identiadDSGrupo = identidadCN.ObtenerGrupoPorNombreCortoYOrganizacion(nombreCortoGrupo, idOrganizacion);

                                    if (identiadDSGrupo.ListaGrupoIdentidadesOrganizacion.Count > 0)
                                    {
                                        numLectores++;
                                        Guid idGrupo = identiadDSGrupo.ListaGrupoIdentidadesOrganizacion.First().GrupoID;
                                        Dictionary<string, object> listaCamposPestanyaMenuRolGrupoIdentidad = new Dictionary<string, object>();
                                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades proyectoPestanyaMenuRolGrupoIdentidadesAux = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades();
                                        listaCamposPestanyaMenuRolGrupoIdentidad.Add(nameof(proyectoPestanyaMenuRolGrupoIdentidadesAux.PestanyaID), pestanyaID);
                                        listaCamposPestanyaMenuRolGrupoIdentidad.Add(nameof(proyectoPestanyaMenuRolGrupoIdentidadesAux.GrupoID), idGrupo);

                                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaPestanyaMenuRolGrupoIdentidades = pDataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades.Where(pestanyaConsulta => pestanyaConsulta.PestanyaID.Equals(pestanyaID) && pestanyaConsulta.GrupoID.Equals(idGrupo)).FirstOrDefault();//FindByPestanyaIDGrupoID(pestanyaID, idGrupo);

                                        ObjectState result = InsertarCambiosEnEF(listaCamposPestanyaMenuRolGrupoIdentidad, mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades, proyectoPestanyaMenuRolGrupoIdentidadesAux, filaPestanyaMenuRolGrupoIdentidades);
                                        if (result.Equals(ObjectState.Agregado))
                                        {
                                            mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Add(proyectoPestanyaMenuRolGrupoIdentidadesAux);
                                        }
                                        if (filaPestanyaMenuRolGrupoIdentidades != null)
                                        {
                                            listaPestanyasMenuRolGrupoIdentidadesModificadas.Add(filaPestanyaMenuRolGrupoIdentidades);
                                        }
                                    }
                                    else
                                    {
                                        if (!IgnorarErroresGrupos)
                                        {
                                            throw new Exception("El grupo " + nombreCortoGrupo + " de la organizacion " + nombreCortoOrg + " no existe");
                                        }
                                    }
                                }

                                //GruposProy
                                XmlNodeList gruposProy = nodoGrupos.SelectNodes("GrupoProy");
                                foreach (XmlNode nodoGrupoProy in gruposProy)
                                {
                                    string nombreCortoGrupo = (string)LeerNodo(nodoGrupoProy, "NombreCortoGrupo", typeof(string));
                                    string nombreCortoProy = (string)LeerNodo(nodoGrupoProy, "NombreCortoProy", typeof(string));

                                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    Guid idProyecto = proyCN.ObtenerProyectoIDPorNombre(nombreCortoProy);
                                    DataWrapperIdentidad identiadDWGrupo = identidadCN.ObtenerGrupoPorNombreCortoYProyecto(nombreCortoGrupo, idProyecto);

                                    if (identiadDWGrupo.ListaGrupoIdentidadesProyecto.Count > 0)
                                    {
                                        numLectores++;
                                        Guid idGrupo = identiadDWGrupo.ListaGrupoIdentidadesProyecto.First().GrupoID;
                                        Dictionary<string, object> listaCamposPestanyaMenuRolGrupoIdentidad = new Dictionary<string, object>();
                                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades proyectoPestanyaMenuRolGrupoIdentidadesAux = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades();
                                        listaCamposPestanyaMenuRolGrupoIdentidad.Add(nameof(proyectoPestanyaMenuRolGrupoIdentidadesAux.PestanyaID), pestanyaID);
                                        listaCamposPestanyaMenuRolGrupoIdentidad.Add(nameof(proyectoPestanyaMenuRolGrupoIdentidadesAux.GrupoID), idGrupo);

                                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaPestanyaMenuRolGrupoIdentidades = pDataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades.Where(proyecto => proyecto.PestanyaID.Equals(pestanyaID) && proyecto.GrupoID.Equals(idGrupo)).FirstOrDefault();//.FindByPestanyaIDGrupoID(pestanyaID, idGrupo);

                                        ObjectState res = InsertarCambiosEnEF(listaCamposPestanyaMenuRolGrupoIdentidad, mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades, proyectoPestanyaMenuRolGrupoIdentidadesAux, filaPestanyaMenuRolGrupoIdentidades);
                                        if (res.Equals(ObjectState.Agregado))
                                        {
                                            mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Add(proyectoPestanyaMenuRolGrupoIdentidadesAux);
                                        }
                                        if (filaPestanyaMenuRolGrupoIdentidades != null)
                                        {
                                            listaPestanyasMenuRolGrupoIdentidadesModificadas.Add(filaPestanyaMenuRolGrupoIdentidades);
                                        }
                                    }
                                    else
                                    {
                                        if (!IgnorarErroresGrupos)
                                        {
                                            throw new Exception("El grupo " + nombreCortoGrupo + " del proyecto " + nombreCortoProy + " no existe");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu proyectoPestanyaMenuAux = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu();

                Dictionary<string, object> listaCamposPestanyaMenu = new Dictionary<string, object>();
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.OrganizacionID), pOrganizacionID);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.ProyectoID), pProyectoID);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.PestanyaID), pestanyaID);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.Visible), visible);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.Nombre), nombrePestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.Orden), ordenPestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.Ruta), rutaPestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.Titulo), tituloPestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.IdiomasDisponibles), idiomasDisponibles);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.NuevaPestanya), nuevaPestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.Privacidad), privacidadPestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.HtmlAlternativo), htmlAlternativoPestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.TipoPestanya), tipoPestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.NombreCortoPestanya), nombreCortoPestanya);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.VisibleSinAcceso), visibleSinAcceso);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.CSSBodyClass), claseBody);
                listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.Activa), activa);

                if (pPestanyaPadreID.HasValue)
                {
                    listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.PestanyaPadreID), pPestanyaPadreID.Value);
                }
                else
                {
                    listaCamposPestanyaMenu.Add(nameof(proyectoPestanyaMenuAux.PestanyaPadreID), null);
                }

                ProyectoPestanyaBusqueda proyectoPestanyaBusquedaAux = new ProyectoPestanyaBusqueda();
                Dictionary<string, object> listaCamposPestanyaBusqueda = new Dictionary<string, object>();

                if (tipoPestanya == (short)TipoPestanyaMenu.BusquedaSemantica
                    || tipoPestanya == (short)TipoPestanyaMenu.Recursos
                    || tipoPestanya == (short)TipoPestanyaMenu.Preguntas
                    || tipoPestanya == (short)TipoPestanyaMenu.Encuestas
                    || tipoPestanya == (short)TipoPestanyaMenu.Debates
                    || tipoPestanya == (short)TipoPestanyaMenu.PersonasYOrganizaciones
                    || tipoPestanya == (short)TipoPestanyaMenu.BusquedaAvanzada)
                {
                    XmlNode nodoConfiguracionBusqueda = pestanya.SelectNodes("ConfiguracionBusqueda")[0];

                    if (nodoConfiguracionBusqueda == null && tipoPestanya == (short)TipoPestanyaMenu.BusquedaSemantica)
                    {
                        throw new Exception("Hay que especificar ConfiguracionBusqueda en las páginas de tipo Busqueda");
                    }

                    if (nodoConfiguracionBusqueda != null)
                    {
                        bool crearFilaBusqueda = false;

                        //CampoFiltro
                        string campoFiltro = "";
                        if (nodoConfiguracionBusqueda.SelectSingleNode("CampoFiltro") != null)
                        {
                            campoFiltro = (string)LeerNodo(nodoConfiguracionBusqueda, "CampoFiltro", typeof(string));
                        }
                        if (tipoPestanya == (short)TipoPestanyaMenu.BusquedaSemantica && string.IsNullOrEmpty(campoFiltro))
                        {
                            throw new Exception("El campo CampoFiltro no puede ser vacío en las páginas de tipo Busqueda");
                        }
                        if (tipoPestanya != (short)TipoPestanyaMenu.BusquedaSemantica && !string.IsNullOrEmpty(campoFiltro))
                        {
                            throw new Exception("El campo CampoFiltro no se puede especificar en las páginas de tipo " + strTipoPestanya);
                        }
                        if (!string.IsNullOrEmpty(campoFiltro))
                        {
                            crearFilaBusqueda = true;
                        }

                        //NumeroRecursos
                        short numeroRecursos = 10;
                        string strNumeroRecursos = (string)LeerNodo(nodoConfiguracionBusqueda, "NumeroRecursos", typeof(string));
                        if (string.IsNullOrEmpty(strNumeroRecursos))
                        {
                            if (tipoPestanya == (short)TipoPestanyaMenu.BusquedaSemantica)
                            {
                                throw new Exception("El campo NumeroRecursos es obligatorio dentro de ConfiguracionBusqueda en las páginas de tipo " + strTipoPestanya);
                            }
                        }
                        try
                        {
                            if (!string.IsNullOrEmpty(strNumeroRecursos))
                            {
                                numeroRecursos = short.Parse(strNumeroRecursos);
                            }
                        }
                        catch (Exception)
                        {
                            throw new Exception("El campo NumeroRecursos tiene que ser un número");
                        }
                        if (!string.IsNullOrEmpty(strNumeroRecursos))
                        {
                            crearFilaBusqueda = true;
                        }

                        //VistaDisponible
                        string vistaDisponible = (string)LeerNodo(nodoConfiguracionBusqueda, "VistaDisponible", typeof(string));
                        if (!string.IsNullOrEmpty(vistaDisponible))
                        {
                            crearFilaBusqueda = true;
                        }

                        //MostrarFacetas
                        bool mostrarFacetas = true;
                        if (nodoConfiguracionBusqueda.SelectSingleNode("MostrarFacetas") != null)
                        {
                            string strMostrarFacetas = ((string)LeerNodo(nodoConfiguracionBusqueda, "MostrarFacetas", typeof(string))).ToLower();
                            if (strMostrarFacetas.Equals("0") || strMostrarFacetas.Equals("false"))
                            {
                                mostrarFacetas = false;
                            }
                            else if (strMostrarFacetas.Equals("1") || strMostrarFacetas.Equals("true"))
                            {
                                mostrarFacetas = true;
                            }
                            else
                            {
                                throw new Exception("El valor " + strMostrarFacetas + " para el campo MostrarFacetas no es válido");
                            }
                        }
                        if (!mostrarFacetas)
                        {
                            crearFilaBusqueda = true;
                        }

                        //MostrarCajaBusqueda
                        bool mostrarCajaBusqueda = true;
                        if (nodoConfiguracionBusqueda.SelectSingleNode("MostrarCajaBusqueda") != null)
                        {
                            string strMostrarCajaBusqueda = ((string)LeerNodo(nodoConfiguracionBusqueda, "MostrarCajaBusqueda", typeof(string))).ToLower();
                            if (strMostrarCajaBusqueda.Equals("0") || strMostrarCajaBusqueda.Equals("false"))
                            {
                                mostrarCajaBusqueda = false;
                            }
                            else if (strMostrarCajaBusqueda.Equals("1") || strMostrarCajaBusqueda.Equals("true"))
                            {
                                mostrarCajaBusqueda = true;
                            }
                            else
                            {
                                throw new Exception("El valor " + strMostrarCajaBusqueda + " para el campo MostrarCajaBusqueda no es válido");
                            }
                        }
                        if (!mostrarCajaBusqueda)
                        {
                            crearFilaBusqueda = true;
                        }

                        //ProyectoOrigenID
                        Guid proyectoOrigenID = Guid.Empty;
                        if (nodoConfiguracionBusqueda.SelectSingleNode("ProyectoOrigen") != null && !string.IsNullOrEmpty(nodoConfiguracionBusqueda.SelectSingleNode("ProyectoOrigen").InnerText))
                        {
                            if (tipoPestanya != (short)TipoPestanyaMenu.BusquedaSemantica)
                            {
                                throw new Exception("El campo ProyectoOrigen no se puede especificar en las páginas de tipo " + strTipoPestanya);
                            }

                            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            proyectoOrigenID = proyectoCN.ObtenerProyectoIDPorNombre(nodoConfiguracionBusqueda.SelectSingleNode("ProyectoOrigen").InnerText);
                        }
                        if (proyectoOrigenID != Guid.Empty)
                        {
                            crearFilaBusqueda = true;
                        }


                        //OcultarResultadosSinFiltros
                        bool ocultarResultadosSinFiltros = false;
                        if (nodoConfiguracionBusqueda.SelectSingleNode("OcultarResultadosSinFiltros") != null)
                        {
                            string strOcultarResultadosSinFiltros = ((string)LeerNodo(nodoConfiguracionBusqueda, "OcultarResultadosSinFiltros", typeof(string))).ToLower();
                            if (strOcultarResultadosSinFiltros.Equals("0") || strOcultarResultadosSinFiltros.Equals("false"))
                            {
                                ocultarResultadosSinFiltros = false;
                            }
                            else if (strOcultarResultadosSinFiltros.Equals("1") || strOcultarResultadosSinFiltros.Equals("true"))
                            {
                                ocultarResultadosSinFiltros = true;
                            }
                            else
                            {
                                throw new Exception("El valor " + strOcultarResultadosSinFiltros + " para el campo OcultarResultadosSinFiltros no es válido");
                            }
                        }
                        if (ocultarResultadosSinFiltros)
                        {
                            crearFilaBusqueda = true;
                        }

                        //PosicionCentralMapa
                        string posicionCentralMapa = "";
                        if (nodoConfiguracionBusqueda.SelectSingleNode("PosicionCentralMapa") != null && !string.IsNullOrEmpty(nodoConfiguracionBusqueda.SelectSingleNode("PosicionCentralMapa").InnerText))
                        {
                            if (tipoPestanya != (short)TipoPestanyaMenu.BusquedaSemantica)
                            {
                                throw new Exception("El campo ProyectoOrigenID no se puede especificar en las páginas de tipo " + strTipoPestanya);
                            }
                            posicionCentralMapa = nodoConfiguracionBusqueda.SelectSingleNode("PosicionCentralMapa").InnerText;
                        }
                        if (!string.IsNullOrEmpty(posicionCentralMapa))
                        {
                            crearFilaBusqueda = true;
                        }

                        //GruposPorTipo
                        bool gruposPorTipo = false;
                        if (nodoConfiguracionBusqueda.SelectSingleNode("GruposPorTipo") != null)
                        {
                            string strGruposPorTipo = ((string)LeerNodo(nodoConfiguracionBusqueda, "GruposPorTipo", typeof(string))).ToLower();
                            if (strGruposPorTipo.Equals("0") || strGruposPorTipo.Equals("false"))
                            {
                                gruposPorTipo = false;
                            }
                            else if (strGruposPorTipo.Equals("1") || strGruposPorTipo.Equals("true"))
                            {
                                gruposPorTipo = true;
                            }
                            else
                            {
                                throw new Exception("El valor " + strGruposPorTipo + " para el campo GruposPorTipo no es válido");
                            }
                        }
                        if (gruposPorTipo)
                        {
                            crearFilaBusqueda = true;
                        }

                        //GruposConfiguracion
                        string gruposConfiguracion = "";
                        if (nodoConfiguracionBusqueda.SelectSingleNode("GruposConfiguracion") != null && !string.IsNullOrEmpty(nodoConfiguracionBusqueda.SelectSingleNode("GruposConfiguracion").InnerText))
                        {
                            gruposConfiguracion = nodoConfiguracionBusqueda.SelectSingleNode("GruposConfiguracion").InnerText;
                        }
                        if (!string.IsNullOrEmpty(gruposConfiguracion))
                        {
                            crearFilaBusqueda = true;
                        }

                        //TextoBusquedaSinResultados
                        string textoBusquedaSinResultados = "";
                        if (nodoConfiguracionBusqueda.SelectSingleNode("TextoBusquedaSinResultados") != null && !string.IsNullOrEmpty(nodoConfiguracionBusqueda.SelectSingleNode("TextoBusquedaSinResultados").InnerText))
                        {
                            textoBusquedaSinResultados = nodoConfiguracionBusqueda.SelectSingleNode("TextoBusquedaSinResultados").InnerText;
                        }
                        if (!string.IsNullOrEmpty(textoBusquedaSinResultados))
                        {
                            crearFilaBusqueda = true;
                        }

                        //TextoDefectoBuscador
                        string textoDefectoBuscador = "";
                        if (nodoConfiguracionBusqueda.SelectSingleNode("TextoDefectoBuscador") != null && !string.IsNullOrEmpty(nodoConfiguracionBusqueda.SelectSingleNode("TextoDefectoBuscador").InnerText))
                        {
                            textoDefectoBuscador = nodoConfiguracionBusqueda.SelectSingleNode("TextoDefectoBuscador").InnerText;
                        }
                        if (!string.IsNullOrEmpty(textoDefectoBuscador))
                        {
                            crearFilaBusqueda = true;
                        }

                        //MostrarEnComboBusqueda
                        bool mostrarEnComboBusqueda = true;
                        if (nodoConfiguracionBusqueda.SelectSingleNode("MostrarEnComboBusqueda") != null)
                        {
                            string strMostrarEnComboBusqueda = ((string)LeerNodo(nodoConfiguracionBusqueda, "MostrarEnComboBusqueda", typeof(string))).ToLower();
                            if (strMostrarEnComboBusqueda.Equals("0") || strMostrarEnComboBusqueda.Equals("false"))
                            {
                                mostrarEnComboBusqueda = false;
                            }
                            else if (strMostrarEnComboBusqueda.Equals("1") || strMostrarEnComboBusqueda.Equals("true"))
                            {
                                mostrarEnComboBusqueda = true;
                            }
                            else
                            {
                                throw new Exception("El valor " + strMostrarEnComboBusqueda + " para el campo MostrarEnComboBusqueda no es válido");
                            }
                        }
                        if (!mostrarEnComboBusqueda)
                        {
                            crearFilaBusqueda = true;
                        }

                        //IgnorarPrivacidadEnBusqueda
                        bool ignorarPrivacidadEnBusqueda = false;
                        if (nodoConfiguracionBusqueda.SelectSingleNode("IgnorarPrivacidadEnBusqueda") != null)
                        {
                            string strIgnorarPrivacidadEnBusqueda = ((string)LeerNodo(nodoConfiguracionBusqueda, "IgnorarPrivacidadEnBusqueda", typeof(string))).ToLower();
                            if (strIgnorarPrivacidadEnBusqueda.Equals("0") || strIgnorarPrivacidadEnBusqueda.Equals("false"))
                            {
                                ignorarPrivacidadEnBusqueda = false;
                            }
                            else if (strIgnorarPrivacidadEnBusqueda.Equals("1") || strIgnorarPrivacidadEnBusqueda.Equals("true"))
                            {
                                ignorarPrivacidadEnBusqueda = true;
                            }
                            else
                            {
                                throw new Exception("El valor " + strIgnorarPrivacidadEnBusqueda + " para el campo IgnorarPrivacidadEnBusqueda no es válido");
                            }
                        }
                        if (ignorarPrivacidadEnBusqueda)
                        {
                            crearFilaBusqueda = true;
                        }

                        //OmitirCargaInicialFacetasResultados
                        bool omitirCargaInicialFacetasResultados = false;
                        if (nodoConfiguracionBusqueda.SelectSingleNode("OmitirCargaInicialFacetasResultados") != null)
                        {
                            string strOmitirCargaInicialFacetasResultados = ((string)LeerNodo(nodoConfiguracionBusqueda, "OmitirCargaInicialFacetasResultados", typeof(string))).ToLower();
                            if (strOmitirCargaInicialFacetasResultados.Equals("0") || strOmitirCargaInicialFacetasResultados.Equals("false"))
                            {
                                omitirCargaInicialFacetasResultados = false;
                            }
                            else if (strOmitirCargaInicialFacetasResultados.Equals("1") || strOmitirCargaInicialFacetasResultados.Equals("true"))
                            {
                                omitirCargaInicialFacetasResultados = true;
                            }
                            else
                            {
                                throw new Exception("El valor " + strOmitirCargaInicialFacetasResultados + " para el campo OmitirCargaInicialFacetasResultados no es válido");
                            }
                        }
                        if (omitirCargaInicialFacetasResultados)
                        {
                            crearFilaBusqueda = true;
                        }

                        if (crearFilaBusqueda)
                        {
                            if (FilasPropiedadesIntegracion != null && FilasPropiedadesIntegracion.Count > 0)
                            {
                                var propiedadCampoFiltro = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == nombreCortoPestanya && prop.TipoObjeto.Equals((short)TipoObjeto.Pagina) && prop.TipoPropiedad.Equals((short)TipoPropiedad.CampoFiltroPagina));

                                if (propiedadCampoFiltro != null && propiedadCampoFiltro.ValorPropiedad == campoFiltro && propiedadCampoFiltro.Revisada && !propiedadCampoFiltro.MismoValor)
                                {
                                    campoFiltro = propiedadCampoFiltro.ValorPropiedadDestino;
                                }
                            }

                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.PestanyaID), pestanyaID);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.CampoFiltro), campoFiltro);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.NumeroRecursos), numeroRecursos);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.VistaDisponible), vistaDisponible);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.MostrarFacetas), mostrarFacetas);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.MostrarCajaBusqueda), mostrarCajaBusqueda);
                            //si no es empty lo almaceno, si no, será null
                            if (proyectoOrigenID != null && !proyectoOrigenID.Equals(Guid.Empty))
                            {
                                listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.ProyectoOrigenID), proyectoOrigenID);
                                EliminarPermisosSobreFicherosComunidadVista();
                                //bug7608: Eliminar los permisos sobre la creación de recursos en la comunidad vista.
                            }
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.OcultarResultadosSinFiltros), ocultarResultadosSinFiltros);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.PosicionCentralMapa), posicionCentralMapa);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.GruposPorTipo), gruposPorTipo);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.GruposConfiguracion), gruposConfiguracion);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.TextoBusquedaSinResultados), textoBusquedaSinResultados);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.TextoDefectoBuscador), textoDefectoBuscador);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.MostrarEnComboBusqueda), mostrarEnComboBusqueda);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.IgnorarPrivacidadEnBusqueda), ignorarPrivacidadEnBusqueda);
                            listaCamposPestanyaBusqueda.Add(nameof(proyectoPestanyaBusquedaAux.OmitirCargaInicialFacetasResultados), omitirCargaInicialFacetasResultados);
                        }

                        #region ProyectoPestanyaFiltroOrden

                        XmlNodeList proyFiltrosOrdenRecurso = nodoConfiguracionBusqueda.SelectNodes("ProyectoFiltrosOrden/ProyectoFiltroOrden");
                        if (proyFiltrosOrdenRecurso != null && proyFiltrosOrdenRecurso.Count > 0)
                        {
                            listaFiltrosModificadas.AddRange(ConfigurarProyectoPestanyaFiltroOrdenRecursosComunidadConXML(proyFiltrosOrdenRecurso, pDataWrapperProyecto, pestanyaID));
                        }
                        else
                        {
                            //si no viene presentación en el xml borro las filas de esa pestanya del DS
                            List<ProyectoPestanyaFiltroOrdenRecursos> filasFiltro = pDataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos.Where(proyectPestFiltroOrden => proyectPestFiltroOrden.PestanyaID.Equals(pestanyaID)).ToList();//("PestanyaID='" + pestanyaID + "'");
                            //borro las filas relacionadas de las tablas que dependen de esa pestaña
                            foreach (ProyectoPestanyaFiltroOrdenRecursos filtro in filasFiltro)
                            {
                                pDataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos.Remove(filtro);
                                proyectoGBD.DeleteProyectoPestanyaFiltroOrdenRecursos(filtro);
                                //filtro.Delete();
                            }
                        }

                        #endregion ProyectoPestanyaFiltroOrden
                    }
                }


                #region ExportacionBusqueda

                XmlNodeList exportacionesBusqueda = pestanya.SelectNodes("ExportacionBusqueda");
                if (exportacionesBusqueda != null && exportacionesBusqueda.Count > 0)
                {
                    listaExportacionesModificadas.AddRange(ConfigurarExportacionBusquedaComunidadConXML(exportacionesBusqueda, pestanyaID, pExportacionBusquedaDW, pProyectoID));
                }
                else
                {
                    //si la pestanya ha dejado de tener exportaciones, no entrará en el punto anterior que es donde se marcaban 
                    //las filas de propiedades de exportación para borrarlas -> hay que marcarlas aquí si no hay exportación y 
                    //dentro de ConfigurarExportacionBusquedaComunidadConXML por si no se ha quitado toda la exportación
                    if (!mListaModificadasEF.ContainsKey(typeof(ProyectoPestanyaBusquedaExportacionPropiedad).Name))
                    {
                        mListaOriginalesEF.Add(typeof(ProyectoPestanyaBusquedaExportacionPropiedad).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaBusquedaExportacionPropiedad));
                        mListaModificadasEF.Add(typeof(ProyectoPestanyaBusquedaExportacionPropiedad).Name, new List<object>());
                    }
                    mListaModificadasEF[typeof(ProyectoPestanyaBusquedaExportacionPropiedad).Name].AddRange(new List<object>());

                    if (!mListaModificadasEF.ContainsKey(typeof(ProyectoPestanyaBusquedaExportacionExterna).Name))
                    {
                        mListaOriginalesEF.Add(typeof(ProyectoPestanyaBusquedaExportacionExterna).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaBusquedaExportacionExterna));
                        mListaModificadasEF.Add(typeof(ProyectoPestanyaBusquedaExportacionExterna).Name, new List<object>());
                    }
                    mListaModificadasEF[typeof(ProyectoPestanyaBusquedaExportacionExterna).Name].AddRange(new List<object>());
                }

                #endregion


                if (tipoPestanya != (short)TipoPestanyaMenu.EnlaceInterno && tipoPestanya != (short)TipoPestanyaMenu.EnlaceExterno && !string.IsNullOrEmpty(rutaPestanya))
                {
                    //Debemos tener cuidado en que no se repitan las rutas de las URL.
                    if (mRutasPestanyas.Contains(rutaPestanya))
                    {
                        throw new Exception(UtilIdiomas.GetText("COMADMINDISENIO", "XMLPESTANYAMISMARUTA"));
                    }
                    else
                    {
                        if (FilasPropiedadesIntegracion != null && FilasPropiedadesIntegracion.Count > 0)
                        {
                            var propiedadRuta = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == nombreCortoPestanya && prop.TipoObjeto.Equals((short)TipoObjeto.Pagina) && prop.TipoPropiedad.Equals((short)TipoPropiedad.RutaPagina));

                            if (propiedadRuta != null && propiedadRuta.ValorPropiedad == rutaPestanya && propiedadRuta.Revisada && !propiedadRuta.MismoValor)
                            {
                                rutaPestanya = propiedadRuta.ValorPropiedadDestino;
                            }
                        }

                        //Agregamos a la listas la ruta 
                        mRutasPestanyas.Add(rutaPestanya);
                    }
                }

                //voy a buscar las filas
                AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanyamenu = pDataWrapperProyecto.ListaProyectoPestanyaMenu.Find(proyectPestanyaMenu => proyectPestanyaMenu.PestanyaID.Equals(pestanyaID));//FindByPestanyaID(pestanyaID);
                ProyectoPestanyaBusqueda filaPestanyaBusqueda = pDataWrapperProyecto.ListaProyectoPestanyaBusqueda.Find(proyectPestanyaBusqueda => proyectPestanyaBusqueda.PestanyaID.Equals(pestanyaID));//FindByPestanyaID(pestanyaID);
                ProyectoPestanyaCMS filaPestanyaCMS = null;
                AD.EntityModel.Models.CMS.CMSPagina filaPaginaCMS = null;
                if (ubicacionCMS.HasValue)
                {
                    filaPestanyaCMS = pDataWrapperProyecto.ListaProyectoPestanyaCMS.Find(proyectPestanya => proyectPestanya.PestanyaID.Equals(pestanyaID) && proyectPestanya.Ubicacion.Equals(ubicacionCMS.Value));
                    filaPaginaCMS = pCMSDW.ListaCMSPagina.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.Ubicacion.Equals(ubicacionCMS.Value)).FirstOrDefault();
                }



                ObjectState resultado = InsertarCambiosEnEF(listaCamposPestanyaMenu, mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaMenuAux, filaPestanyamenu);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.ProyectoPestanyaMenu.Add(proyectoPestanyaMenuAux);
                }

                //Si el pPadre es != null, insertarlo en BD.
                XmlNodeList grupoPestanyasHijas = pestanya.SelectNodes("PestanyasHijas");
                if (grupoPestanyasHijas != null && grupoPestanyasHijas.Count > 0)
                {
                    XmlNodeList pestanyasHijas = grupoPestanyasHijas[0].SelectNodes("Pestanya");
                    ConfigurarPestanyasComunidadConXML(pestanyasHijas, pDataWrapperProyecto, pCMSDW, pOrganizacionID, pProyectoID, pestanyaID, pExportacionBusquedaDW, ref pListaIDsPestanyas, ref pListaNombresCortosPestanyas, ref pListaUbicacionesCMS, ref pFilaConfiguracionAmbitoBusqueda);
                }
                if (listaCamposPestanyaBusqueda.Count > 0)
                {
                    resultado = InsertarCambiosEnEF(listaCamposPestanyaBusqueda, mEntityContext.ProyectoPestanyaBusqueda, proyectoPestanyaBusquedaAux, filaPestanyaBusqueda);
                    if (resultado.Equals(ObjectState.Agregado))
                    {
                        mEntityContext.ProyectoPestanyaBusqueda.Add(proyectoPestanyaBusquedaAux);
                    }
                }
                if (listaCamposPestanyaCMS.Count > 0)
                {
                    ProyectoPestanyaCMS proyectoPestanyaCMSAux = new ProyectoPestanyaCMS();
                    resultado = InsertarCambiosEnEF(listaCamposPestanyaCMS, mEntityContext.ProyectoPestanyaCMS, proyectoPestanyaCMSAux, filaPestanyaCMS);
                    if (resultado.Equals(ObjectState.Agregado))
                    {
                        mEntityContext.ProyectoPestanyaCMS.Add(proyectoPestanyaCMSAux);
                    }
                }
                if (listaCamposCMSPagina.Count > 0)
                {
                    AD.EntityModel.Models.CMS.CMSPagina CMSPaginaAux = new AD.EntityModel.Models.CMS.CMSPagina();
                    resultado = InsertarCambiosEnEF(listaCamposCMSPagina, mEntityContext.CMSPagina, CMSPaginaAux, filaPaginaCMS);
                    if (resultado.Equals(ObjectState.Agregado))
                    {
                        mEntityContext.CMSPagina.Add(CMSPaginaAux);
                    }
                }

                if (filaPestanyamenu != null)
                {
                    listaPestanyasMenuModificadas.Add(filaPestanyamenu);
                }
                if (filaPestanyaBusqueda != null)
                {
                    listaPestanyasBusquedaModificadas.Add(filaPestanyaBusqueda);
                }
                if (filaPestanyaCMS != null)
                {
                    listaPestanyasCMSModificadas.Add(filaPestanyaCMS);
                }
                if (filaPaginaCMS != null)
                {
                    listaCMSPaginaModificadas.Add(filaPaginaCMS);
                }
                ordenPestanya++;
            }

            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoPestanyaCMS).Name))
            {
                mListaOriginalesEF.Add(typeof(ProyectoPestanyaCMS).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaCMS));
                mListaModificadasEF.Add(typeof(ProyectoPestanyaCMS).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ProyectoPestanyaCMS).Name].AddRange(listaPestanyasCMSModificadas);

            if (!mListaModificadasEF.ContainsKey(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad).Name))
            {
                mListaOriginalesEF.Add(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaMenuRolIdentidad));
                mListaModificadasEF.Add(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad).Name, new List<object>());
            }
            mListaModificadasEF[typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad).Name].AddRange(listaPestanyasMenuRolIdentidadModificadas);

            if (!mListaModificadasEF.ContainsKey(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades).Name))
            {
                mListaOriginalesEF.Add(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades));
                mListaModificadasEF.Add(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades).Name, new List<object>());
            }
            mListaModificadasEF[typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades).Name].AddRange(listaPestanyasMenuRolGrupoIdentidadesModificadas);

            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoPestanyaFiltroOrdenRecursos).Name))
            {
                mListaOriginalesEF.Add(typeof(ProyectoPestanyaFiltroOrdenRecursos).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos));
                mListaModificadasEF.Add(typeof(ProyectoPestanyaFiltroOrdenRecursos).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ProyectoPestanyaFiltroOrdenRecursos).Name].AddRange(listaFiltrosModificadas);

            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoPestanyaBusquedaExportacion).Name))
            {
                mListaOriginalesEF.Add(typeof(ProyectoPestanyaBusquedaExportacion).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaBusquedaExportacion));
                mListaModificadasEF.Add(typeof(ProyectoPestanyaBusquedaExportacion).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ProyectoPestanyaBusquedaExportacion).Name].AddRange(listaExportacionesModificadas);

            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoPestanyaBusqueda).Name))
            {
                mListaOriginalesEF.Add(typeof(ProyectoPestanyaBusqueda).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaBusqueda));
                mListaModificadasEF.Add(typeof(ProyectoPestanyaBusqueda).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ProyectoPestanyaBusqueda).Name].AddRange(listaPestanyasBusquedaModificadas);

            if (!mListaModificadasEF.ContainsKey(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu).Name))
            {
                mListaOriginalesEF.Add(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu).Name, new List<object>(pDataWrapperProyecto.ListaProyectoPestanyaMenu));
                mListaModificadasEF.Add(typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu).Name, new List<object>());
            }
            mListaModificadasEF[typeof(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu).Name].AddRange(listaPestanyasMenuModificadas);
        }

        /// <summary>
        /// Eliminamos los permisos para crear cualquier tipo de recurso en la comunidad vista ya que no se deben poder crear recursos, solo se van a ver las vistas de la comunidad origen.
        /// </summary>
        private void EliminarPermisosSobreFicherosComunidadVista()
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperProyecto dataWrapperProyecto = proyectoCN.ObtenerProyectoPorIDConNiveles(ProyectoSeleccionado.Clave);

            //Quitamos los permisos de creación sobre los documentos de la comunidad.
            foreach (TipoDocDispRolUsuarioProy filaDocRol in dataWrapperProyecto.ListaTipoDocDispRolUsuarioProy.ToList())
            {
                dataWrapperProyecto.ListaTipoDocDispRolUsuarioProy.Remove(filaDocRol);
                mEntityContext.EliminarElemento(filaDocRol);
            }

            foreach (TipoOntoDispRolUsuarioProy filaOntoRol in dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.ToList())
            {
                dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.Remove(filaOntoRol);
                mEntityContext.EliminarElemento(filaOntoRol);
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Inserta la fila en la tabla comprobando si hay cambios en los campos que se pasan en el diccionario
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pTabla">Tabla del dataset con la configuracion por defecto</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        /// <returns>Devuelve true en caso de cambios en el DS</returns>
        private bool InsertarCambiosEnDataSet(Dictionary<DataColumn, object> pListaCampos, DataTable pTabla, DataRow pFila)
        {
            return InsertarCambiosEnDataSet(pListaCampos, pTabla, pFila, new List<string>(), false);
        }

        /// <summary>
        /// Inserta la fila en la tabla comprobando si hay cambios en los campos que se pasan en el diccionario
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pTabla">Tabla del dataset con la configuracion por defecto</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        /// <param name="pEliminar">Verdad si hay que eliminar la fila original en vez de modificarla</param>
        /// <returns>Devuelve true en caso de cambios en el DS</returns>
        private bool InsertarCambiosEnDataSet(Dictionary<DataColumn, object> pListaCampos, DataTable pTabla, DataRow pFila, bool pEliminar)
        {
            return InsertarCambiosEnDataSet(pListaCampos, pTabla, pFila, new List<string>(), pEliminar);
        }

        /// <summary>
        /// Inserta la fila en la tabla comprobando si hay cambios en los campos que se pasan en el diccionario. Los campos ignorados se insertan pero no 
        /// retornan edición de tabla (campos autonuméricos)
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pTabla">Tabla del dataset con la configuracion por defecto</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        /// <param name="pListaCamposOmitir">Lista de campos de la tabla que no se tendran en cuenta a la hora de retornar true en caso de edicion</param>
        /// <returns>Devuelve true en caso de cambios en el DS, siempre que no se hayan producido en los campos a ignorar</returns>
        private bool InsertarCambiosEnDataSet(Dictionary<DataColumn, object> pListaCampos, DataTable pTabla, DataRow pFila, List<string> pListaCamposOmitir)
        {
            return InsertarCambiosEnDataSet(pListaCampos, pTabla, pFila, pListaCamposOmitir, false);
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        public static void SetPropValue(object src, string propName, object value)
        {
            src.GetType().GetProperty(propName).SetValue(src, value);
        }

        /// <summary>
        /// Inserta la fila en la tabla comprobando si hay cambios en los campos que se pasan en el diccionario. Los campos ignorados se insertan pero no 
        /// retornan edición de tabla (campos autonuméricos)
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pTabla">Tabla del dataset con la configuracion por defecto</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        /// <param name="pListaCamposOmitir">Lista de campos de la tabla que no se tendran en cuenta a la hora de retornar true en caso de edicion</param>
        /// <param name="pEliminar">Verdad si hay que eliminar la fila original en vez de modificarla</param>
        /// <returns>Devuelve true en caso de cambios en el DS, siempre que no se hayan producido en los campos a ignorar</returns>
        private ObjectState InsertarCambiosEnEF(Dictionary<string, object> pListaCampos, IListSource pDbSet, object pAux, object pFila, bool pEliminar = false)
        {
            return InsertarCambiosEnEF(pListaCampos, pDbSet, pAux, pFila, new List<string>(), pEliminar);
        }

        /// <summary>
        /// Inserta la fila en la tabla comprobando si hay cambios en los campos que se pasan en el diccionario. Los campos ignorados se insertan pero no 
        /// retornan edición de tabla (campos autonuméricos)
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pTabla">Tabla del dataset con la configuracion por defecto</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        /// <param name="pListaCamposOmitir">Lista de campos de la tabla que no se tendran en cuenta a la hora de retornar true en caso de edicion</param>
        /// <param name="pEliminar">Verdad si hay que eliminar la fila original en vez de modificarla</param>
        /// <returns>Devuelve true en caso de cambios en el DS, siempre que no se hayan producido en los campos a ignorar</returns>
        private ObjectState InsertarCambiosEnEF(Dictionary<string, object> pListaCampos, IListSource pDbSet, object pAux, object pFila, List<string> pListaCamposOmitir, bool pEliminar = false)
        {
            ObjectState resultado = ObjectState.SinCambios;

            if (pEliminar && pFila != null && ComprobarCambiosEF(pListaCampos, pFila))
            {
                mEntityContext.EliminarElemento(pFila);
                pFila = null;
            }

            //si la fila no existe la creo
            if (pFila == null)
            {
                pFila = pAux;
                resultado = ObjectState.Agregado;
            }
            foreach (string propiedad in pListaCampos.Keys)
            {
                object valor = GetPropValue(pFila, propiedad);
                if (pListaCampos[propiedad] == null && valor != null)
                {
                    //actualizo la fila
                    SetPropValue(pFila, propiedad, null);
                    //si la columna no es un campo a omitir
                    if (!resultado.Equals(ObjectState.Agregado) && !pListaCamposOmitir.Contains(propiedad))
                    {
                        resultado = ObjectState.Editado;
                    }
                }
                //si la columna es  != null Y no es 'NULL' O  el campo del ds != al campo del xml
                if (pListaCampos[propiedad] != null && (valor == null || !valor.Equals(pListaCampos[propiedad])))
                {
                    //actualizo la fila
                    SetPropValue(pFila, propiedad, pListaCampos[propiedad]);
                    //si la columna no es un campo a omitir
                    if (!resultado.Equals(ObjectState.Agregado) && !pListaCamposOmitir.Contains(propiedad))
                    {
                        resultado = ObjectState.Editado;
                    }
                }
            }

            //if (resultado.Equals(ObjectState.Agregado))
            //{
            //    pDbSet.Add(pAux);
            //}

            return resultado;
        }


        /// <summary>
        /// Inserta la fila en la tabla comprobando si hay cambios en los campos que se pasan en el diccionario. Los campos ignorados se insertan pero no 
        /// retornan edición de tabla (campos autonuméricos)
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pTabla">Tabla del dataset con la configuracion por defecto</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        /// <param name="pListaCamposOmitir">Lista de campos de la tabla que no se tendran en cuenta a la hora de retornar true en caso de edicion</param>
        /// <param name="pEliminar">Verdad si hay que eliminar la fila original en vez de modificarla</param>
        /// <returns>Devuelve true en caso de cambios en el DS, siempre que no se hayan producido en los campos a ignorar</returns>
        private bool InsertarCambiosEnDataSet(Dictionary<DataColumn, object> pListaCampos, DataTable pTabla, DataRow pFila, List<string> pListaCamposOmitir, bool pEliminar)
        {
            bool editada = false;
            bool agregarFila = false;

            if (pEliminar && pFila != null && ComprobarCambios(pListaCampos, pFila))
            {
                pFila.Delete();
                pFila = null;
            }

            //si la fila no existe la creo
            if (pFila == null)
            {
                pFila = pTabla.NewRow();
                agregarFila = true;
            }
            foreach (DataColumn columna in pListaCampos.Keys)
            {
                if (pListaCampos[columna] == null && !pFila.IsNull(columna))
                {
                    //actualizo la fila
                    pFila[columna] = DBNull.Value;
                    //si la columna no es un campo a omitir
                    if (!pListaCamposOmitir.Contains(columna.ColumnName))
                    {
                        editada = true;
                    }
                }
                //si la columna es  != null Y no es 'NULL' O  el campo del ds != al campo del xml
                if (pListaCampos[columna] != null && (pFila.IsNull(columna) || !pFila[columna].Equals(pListaCampos[columna])))
                {
                    //actualizo la fila
                    pFila[columna] = pListaCampos[columna];
                    //si la columna no es un campo a omitir
                    if (!pListaCamposOmitir.Contains(columna.ColumnName))
                    {
                        editada = true;
                    }
                }
            }
            if (agregarFila)
            {
                pTabla.Rows.Add(pFila);
            }
            return editada;
        }

        /// <summary>
        /// Comprueba si la lista de campos modifica la fila actual
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        /// <returns>Verdad si hay cambios</returns>
        private bool ComprobarCambiosEF(Dictionary<string, object> pListaCampos, object pFila)
        {
            foreach (string propiedad in pListaCampos.Keys)
            {
                object value = GetPropValue(pFila, propiedad);
                //si la columna es  != null Y no es 'NULL' O  el campo del ds != al campo del xml
                if (pListaCampos[propiedad] != null && (value == null || !value.Equals(pListaCampos[propiedad])))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Comprueba si la lista de campos modifica la fila actual
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        /// <returns>Verdad si hay cambios</returns>
        private bool ComprobarCambios(Dictionary<DataColumn, object> pListaCampos, DataRow pFila)
        {
            foreach (DataColumn columna in pListaCampos.Keys)
            {
                //si la columna es  != null Y no es 'NULL' O  el campo del ds != al campo del xml
                if (pListaCampos[columna] != null && (pFila.IsNull(columna) || !pFila[columna].Equals(pListaCampos[columna])))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Inserta la fila en la tabla comprobando si hay cambios en los campos que se pasan en el diccionario
        /// </summary>
        /// <param name="pListaCampos">Diccionario de los campos que llegan desde el XML</param>
        /// <param name="pTablaDefecto">Tabla del dataset con la configuracion por defecto</param>
        /// <param name="pTablaPersonalizada">Tabla del dataset con la configuracion personalizada</param>
        /// <param name="pFila">Fila modificada. Puede no existir en la tabla</param>
        private void InsertarPersonalizacionEnDataSet(Dictionary<DataColumn, object> pListaCampos, Dictionary<DataColumn, object> pListaCamposPersonalizacion, DataTable pTablaDefecto, DataTable pTablaPersonalizada, DataRow pFila, DataRow pFilaPersonalizacion)
        {
            //si existe personalización no miro nada más, compruebo cambios en esa fila
            if (pFilaPersonalizacion != null)
            {
                foreach (DataColumn columna in pListaCamposPersonalizacion.Keys)
                {
                    if (pListaCamposPersonalizacion[columna] != null && (pFilaPersonalizacion.IsNull(columna) || !pFilaPersonalizacion[columna].Equals(pListaCamposPersonalizacion[columna])))
                    {
                        pFilaPersonalizacion[columna] = pListaCamposPersonalizacion[columna];
                    }
                }
            }
            //si no existe personalización miro la configuración por defecto
            else
            {
                bool personalizada = false;
                //si existe configuración por defecto, compruebo si hay cambios
                if (pFila != null)
                {
                    foreach (DataColumn columna in pListaCampos.Keys)
                    {
                        //si la columna es  != null Y no es 'NULL' O  el campo del ds != al campo del xml
                        if (pListaCampos[columna] != null && (pFila.IsNull(columna) || !pFila[columna].Equals(pListaCampos[columna])))
                        {
                            //si hay cambios creo la fila personalizada
                            personalizada = true;
                        }
                    }
                }
                //no existe configuración personalizada ni por defecto, creo la por defecto
                else
                {
                    pFila = pTablaDefecto.NewRow();
                    foreach (DataColumn columna in pListaCampos.Keys)
                    {
                        //si la columna es  != null Y no es 'NULL' O  el campo del ds != al campo del xml
                        if (pListaCampos[columna] != null && (pFila.IsNull(columna) || !pFila[columna].Equals(pListaCampos[columna])))
                        {
                            pFila[columna] = pListaCampos[columna];
                        }
                    }
                    pTablaDefecto.Rows.Add(pFila);
                }
                //tengo que crear la fila personalizada
                if (personalizada)
                {
                    pFilaPersonalizacion = pTablaPersonalizada.NewRow();
                    foreach (DataColumn columna in pListaCamposPersonalizacion.Keys)
                    {
                        if (pListaCamposPersonalizacion[columna] != null && (pFilaPersonalizacion.IsNull(columna) || !pFilaPersonalizacion[columna].Equals(pListaCamposPersonalizacion[columna])))
                        {
                            pFilaPersonalizacion[columna] = pListaCamposPersonalizacion[columna];
                        }
                    }
                    pTablaPersonalizada.Rows.Add(pFilaPersonalizacion);
                }
            }
        }

        /// <summary>
        /// Configura los FiltrosOrdenRecurso de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pFiltrosOrdenRecurso">Lista de nodos con los filtros de una pestaña</param>
        /// <param name="pPestanya">Nodo de una pestaña</param>
        /// <param name="pDataWrapperProyecto">Dataset de un proyecto determinado</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private List<object> ConfigurarProyectoPestanyaFiltroOrdenRecursosComunidadConXML(XmlNodeList pProyFiltrosOrdenRecurso, DataWrapperProyecto pDataWrapperProyecto, Guid pPestanyaID)
        {
            List<object> listaModificadas = new List<object>();
            short i = 0;
            foreach (XmlElement filtroRecurso in pProyFiltrosOrdenRecurso)
            {
                string filtroOrden = (string)LeerNodo(filtroRecurso, "FiltroOrden", typeof(string));
                string nombreFiltro = (string)LeerNodo(filtroRecurso, "NombreFiltro", typeof(string));
                short ordenFiltro = i;
                //creo el diccionario para control de cambios
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                ProyectoPestanyaFiltroOrdenRecursos proyectoPestanyaFiltroOrdenRecursosAux = new ProyectoPestanyaFiltroOrdenRecursos();
                listaCampos.Add(nameof(proyectoPestanyaFiltroOrdenRecursosAux.FiltroOrden), filtroOrden);
                listaCampos.Add(nameof(proyectoPestanyaFiltroOrdenRecursosAux.NombreFiltro), nombreFiltro);
                listaCampos.Add(nameof(proyectoPestanyaFiltroOrdenRecursosAux.Orden), ordenFiltro);
                listaCampos.Add(nameof(proyectoPestanyaFiltroOrdenRecursosAux.PestanyaID), pPestanyaID);
                i++;

                //busco la fila para el control de cambios
                ProyectoPestanyaFiltroOrdenRecursos filaFiltroOrdenRecurso = pDataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos.Where(proyecto => proyecto.PestanyaID.Equals(pPestanyaID) && proyecto.FiltroOrden.Equals(filtroOrden)).FirstOrDefault();//.FindByPestanyaIDOrden(pPestanyaID, ordenFiltro);
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.ProyectoPestanyaFiltroOrdenRecursos, proyectoPestanyaFiltroOrdenRecursosAux, filaFiltroOrdenRecurso);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.ProyectoPestanyaFiltroOrdenRecursos.Add(proyectoPestanyaFiltroOrdenRecursosAux);
                }
                if (filaFiltroOrdenRecurso != null)
                {
                    listaModificadas.Add(filaFiltroOrdenRecurso);
                }
            }
            return listaModificadas;
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pProyectoDS.ProyectoFiltroOrdenRecursos, listaModificadas);            
        }

        private List<ProyectoPestanyaBusquedaExportacion> ConfigurarExportacionBusquedaComunidadConXML(XmlNodeList pExportacionesBusqueda, Guid pPestanyaID, DataWrapperExportacionBusqueda pExportacionBusquedaDW, Guid pProyectoID)
        {
            List<ProyectoPestanyaBusquedaExportacion> listaModificadas = new List<ProyectoPestanyaBusquedaExportacion>();
            List<ProyectoPestanyaBusquedaExportacionPropiedad> listaPropiedadesExportacionModificadas = new List<ProyectoPestanyaBusquedaExportacionPropiedad>();
            List<ProyectoPestanyaBusquedaExportacionExterna> listaExportacionesExternasModificadas = new List<ProyectoPestanyaBusquedaExportacionExterna>();

            foreach (XmlElement exportacion in pExportacionesBusqueda)
            {
                string nombreExportacion = (string)LeerNodo(exportacion, "NombreExportacion", typeof(string));
                short ordenExportacion = (short)LeerNodo(exportacion, "OrdenExportacion", typeof(short));
                string gruposExportacion = (string)LeerNodo(exportacion, "GruposExportadores", typeof(string));
                string formatosExportacion = (string)LeerNodo(exportacion, "FormatosExportacion", typeof(string));

                List<Guid> listaGrupos = null;
                string gruposExportadores = null;
                if (!string.IsNullOrEmpty(gruposExportacion))
                {
                    //NombreCortoOrganizacion|NombreCortoGrupo,NombreCortoGrupo
                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    listaGrupos = new List<Guid>();
                    foreach (string grupo in gruposExportacion.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        List<string> listaNombres = new List<string>();
                        string nombreGrupo = "";
                        if (grupo.Contains('|'))
                        {
                            nombreGrupo = grupo.Split('|')[1];
                            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            Guid organizacionID = orgCN.ObtenerOrganizacionesIDPorNombre(grupo.Split('|')[0]);
                            listaNombres.Add(nombreGrupo);
                            listaGrupos.AddRange(identCN.ObtenerGruposIDPorNombreCortoYOrganizacion(listaNombres, organizacionID));
                        }
                        else
                        {
                            nombreGrupo = grupo;
                            listaNombres.Add(nombreGrupo);
                            listaGrupos.AddRange(identCN.ObtenerGruposIDPorNombreCortoYProyecto(listaNombres, ProyectoSeleccionado.Clave));
                        }
                    }
                    string coma = "";
                    foreach (Guid grupoID in listaGrupos)
                    {
                        gruposExportadores += coma + grupoID;
                        coma = ",";
                    }
                }

                if (!string.IsNullOrEmpty(formatosExportacion))
                {
                    string[] formatos = formatosExportacion.Split(',');
                    for (int i = 0; i < formatos.Length; i++)
                    {
                        if (!formatos[i].ToLower().Equals(FormatosExportancion.CSV) && !formatos[i].ToLower().Equals(FormatosExportancion.EXCEL))
                        {
                            throw new Exception(string.Concat("El formato indicado para la exportación en ", nombreExportacion, " no es valido. Estan permitidos: ", FormatosExportancion.CSV, " y ", FormatosExportancion.EXCEL));
                        }
                    }
                }
                else
                {
                    formatosExportacion = FormatosExportancion.CSV;
                }

                Guid exportacionID = Guid.Empty;
                List<ProyectoPestanyaBusquedaExportacion> fila = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Where(item => item.PestanyaID.Equals(pPestanyaID) && item.NombreExportacion.Equals(nombreExportacion)).ToList();

                if (fila.Count > 0)
                {
                    exportacionID = fila[0].ExportacionID;
                }
                else
                {
                    exportacionID = Guid.NewGuid();
                }

                //creo el diccionario para control de cambios
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                ProyectoPestanyaBusquedaExportacion proyectoPestanyaBusquedaExportacionAux = new ProyectoPestanyaBusquedaExportacion();

                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionAux.ExportacionID), exportacionID);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionAux.PestanyaID), pPestanyaID);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionAux.NombreExportacion), nombreExportacion);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionAux.Orden), ordenExportacion);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionAux.GruposExportadores), gruposExportadores);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionAux.FormatosExportacion), formatosExportacion.ToLower());

                //busco la fila para el control de cambios
                ProyectoPestanyaBusquedaExportacion filaProyectoPestanyaExportacionBusqueda = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Where(item => item.ExportacionID.Equals(exportacionID)).FirstOrDefault();

                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.ProyectoPestanyaBusquedaExportacion, proyectoPestanyaBusquedaExportacionAux, filaProyectoPestanyaExportacionBusqueda);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.ProyectoPestanyaBusquedaExportacion.Add(proyectoPestanyaBusquedaExportacionAux);
                }
                if (filaProyectoPestanyaExportacionBusqueda != null)
                {
                    listaModificadas.Add(filaProyectoPestanyaExportacionBusqueda);
                }

                #region ExportacionBusquedaPropiedad

                XmlNodeList propiedadesExportacionBusqueda = exportacion.SelectNodes("PropiedadExportacion");
                if (propiedadesExportacionBusqueda != null && propiedadesExportacionBusqueda.Count > 0)
                {
                    listaPropiedadesExportacionModificadas.AddRange(ConfigurarPropiedadesExportacionBusquedaComunidadConXML(propiedadesExportacionBusqueda, pPestanyaID, pExportacionBusquedaDW, pProyectoID, exportacionID));
                }

                #endregion

                #region EnlaceServicioExternoExportacion

                string enlaceServicioExternoExportacionBusqueda = (string)LeerNodo(exportacion, "EnlaceServicioExternoExportacion", typeof(string));
                if (!string.IsNullOrEmpty(enlaceServicioExternoExportacionBusqueda))
                {
                    ProyectoPestanyaBusquedaExportacionExterna filaProyectoPestanyaBusquedaExportacionExterna = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionExterna.Where(item => item.ExportacionID.Equals(exportacionID)).FirstOrDefault();
                    Dictionary<string, object> listaCamposExportacionExterna = new Dictionary<string, object>();
                    ProyectoPestanyaBusquedaExportacionExterna proyectoPestanyaBusquedaExportacionExternaAux = new ProyectoPestanyaBusquedaExportacionExterna();
                    listaCamposExportacionExterna.Add(nameof(proyectoPestanyaBusquedaExportacionExternaAux.ExportacionID), exportacionID);
                    listaCamposExportacionExterna.Add(nameof(proyectoPestanyaBusquedaExportacionExternaAux.PestanyaID), pPestanyaID);
                    listaCamposExportacionExterna.Add(nameof(proyectoPestanyaBusquedaExportacionExternaAux.UrlServicioExterno), enlaceServicioExternoExportacionBusqueda);

                    ProyectoPestanyaBusquedaExportacionExterna filaProyectoPestanyaExportacionExternaBusquedaExterna = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionExterna.Where(item => item.ExportacionID.Equals(exportacionID)).FirstOrDefault();
                    ObjectState resultadoCambios = InsertarCambiosEnEF(listaCamposExportacionExterna, mEntityContext.ProyectoPestanyaBusquedaExportacionExterna, proyectoPestanyaBusquedaExportacionExternaAux, filaProyectoPestanyaExportacionExternaBusquedaExterna);
                    if (resultadoCambios.Equals(ObjectState.Agregado))
                    {
                        mEntityContext.ProyectoPestanyaBusquedaExportacionExterna.Add(proyectoPestanyaBusquedaExportacionExternaAux);
                    }
                    if (filaProyectoPestanyaExportacionExternaBusquedaExterna != null)
                    {
                        listaExportacionesExternasModificadas.Add(filaProyectoPestanyaExportacionExternaBusquedaExterna);
                    }

                    if (!mListaModificadasEF.ContainsKey(typeof(ProyectoPestanyaBusquedaExportacionExterna).Name))
                    {
                        mListaOriginalesEF.Add(typeof(ProyectoPestanyaBusquedaExportacionExterna).Name, new List<object>(pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionExterna));
                        mListaModificadasEF.Add(typeof(ProyectoPestanyaBusquedaExportacionExterna).Name, new List<object>());
                    }

                    mListaModificadasEF[typeof(ProyectoPestanyaBusquedaExportacionExterna).Name].AddRange(listaExportacionesExternasModificadas);
                }

                #endregion
            }

            if (!mListaModificadasEF.ContainsKey(typeof(ProyectoPestanyaBusquedaExportacionPropiedad).Name))
            {
                mListaModificadasEF.Add(typeof(ProyectoPestanyaBusquedaExportacionPropiedad).Name, new List<object>());
            }
            mListaModificadasEF[typeof(ProyectoPestanyaBusquedaExportacionPropiedad).Name].AddRange(listaPropiedadesExportacionModificadas);

            return listaModificadas;
        }

        private List<ProyectoPestanyaBusquedaExportacionPropiedad> ConfigurarPropiedadesExportacionBusquedaComunidadConXML(XmlNodeList pPropiedadesExportacionesBusqueda, Guid pPestanyaID, DataWrapperExportacionBusqueda pExportacionBusquedaDW, Guid pProyectoID, Guid pExportacionID)
        {
            List<ProyectoPestanyaBusquedaExportacionPropiedad> listaModificadas = new List<ProyectoPestanyaBusquedaExportacionPropiedad>();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            foreach (XmlElement propiedadExportacion in pPropiedadesExportacionesBusqueda)
            {
                short ordenPropiedad = (short)LeerNodo(propiedadExportacion, "OrdenPropiedad", typeof(short));
                string ontologiaProyecto = (string)LeerNodo(propiedadExportacion, "Ontologia", typeof(string));
                string propiedad = (string)LeerNodo(propiedadExportacion, "Propiedad", typeof(string));
                string nombrePropiedad = (string)LeerNodo(propiedadExportacion, "NombrePropiedad", typeof(string));
                string datosExtraPropiedad = (string)LeerNodo(propiedadExportacion, "DatosExtraPropiedad", typeof(string));

                //si no es semántica la ontologia será una cadena vacía
                string urlOntologia = "";
                if (propiedad.Contains(":"))
                {
                    urlOntologia = "http://gnoss.com/Ontologia/" + ontologiaProyecto + ".owl#";
                }

                //para evitar que cada vez que entre una propiedad no semántica cambie su ontologiaID busco la fila y si existe le asigno ese ID
                ProyectoPestanyaBusquedaExportacionPropiedad filaExportacionBusquedaPropiedad = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Where(item => item.ExportacionID.Equals(pExportacionID) && item.Orden.Equals(ordenPropiedad)).FirstOrDefault();

                Guid? ontologiaID = Guid.Empty;

                if (!string.IsNullOrEmpty(ontologiaProyecto))
                {
                    ontologiaID = documentacionCN.ObtenerOntologiaAPartirNombre(pProyectoID, ontologiaProyecto + ".owl");
                }

                //la propiedad no tiene ontología
                if (ontologiaID.Equals(Guid.Empty))
                {
                    //si la fila existe le asigno el ontologiaID que ya tenía asignado anteriormente para evitar cambios innecesarios en el DS
                    if (filaExportacionBusquedaPropiedad != null && filaExportacionBusquedaPropiedad.OntologiaID.HasValue && !filaExportacionBusquedaPropiedad.OntologiaID.Equals(Guid.Empty))
                    {
                        ontologiaID = filaExportacionBusquedaPropiedad.OntologiaID;
                    }
                    else
                    {
                        ontologiaID = null;
                    }
                }

                //creo el diccionario para control de cambios
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                ProyectoPestanyaBusquedaExportacionPropiedad proyectoPestanyaBusquedaExportacionPropiedadAux = new ProyectoPestanyaBusquedaExportacionPropiedad();

                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionPropiedadAux.ExportacionID), pExportacionID);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionPropiedadAux.Orden), ordenPropiedad);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionPropiedadAux.OntologiaID), ontologiaID);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionPropiedadAux.Ontologia), urlOntologia);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionPropiedadAux.Propiedad), propiedad);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionPropiedadAux.NombrePropiedad), nombrePropiedad);
                listaCampos.Add(nameof(proyectoPestanyaBusquedaExportacionPropiedadAux.DatosExtraPropiedad), datosExtraPropiedad);

                //controlo los cambios en el DS
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.ProyectoPestanyaBusquedaExportacionPropiedad, proyectoPestanyaBusquedaExportacionPropiedadAux, filaExportacionBusquedaPropiedad);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.ProyectoPestanyaBusquedaExportacionPropiedad.Add(proyectoPestanyaBusquedaExportacionPropiedadAux);
                }
                if (filaExportacionBusquedaPropiedad != null)
                {
                    listaModificadas.Add(filaExportacionBusquedaPropiedad);
                }
            }

            return listaModificadas;
        }

        /// <summary>
        /// Configura la FacetaConfigProyRangoFecha de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pConfigFacProyRangoFecha">Nodo xml con la configuracion de la FacetaConfigProyRangoFecha</param>
        /// <param name="pFacetaDW">Dataset de Facetas</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void ConfigurarFacetaConfiguracionProyectoRangoFechaConXML(XmlNodeList pConfigFacProyRangoFecha, DataWrapperFacetas pFacetaDW, Guid pOrganizacionID, Guid pProyectoID)
        {
            List<FacetaConfigProyRangoFecha> listaModificadas = new List<FacetaConfigProyRangoFecha>();
            foreach (XmlNode nodo in pConfigFacProyRangoFecha)
            {
                string propNueva = (string)LeerNodo(nodo, "PropiedadNueva", typeof(string));
                string propInicio = (string)LeerNodo(nodo, "PropioedadInicio", typeof(string));
                string propFin = (string)LeerNodo(nodo, "PropioedadFin", typeof(string));

                //creo el diccionario para control de cambios
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                FacetaConfigProyRangoFecha facetaConfigProyRangoFechaAux = new FacetaConfigProyRangoFecha();

                listaCampos.Add(nameof(facetaConfigProyRangoFechaAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(facetaConfigProyRangoFechaAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(facetaConfigProyRangoFechaAux.PropiedadNueva), propNueva);
                listaCampos.Add(nameof(facetaConfigProyRangoFechaAux.PropiedadInicio), propInicio);
                listaCampos.Add(nameof(facetaConfigProyRangoFechaAux.PropiedadFin), propFin);

                FacetaConfigProyRangoFecha filasConfigProyRangoFecha = pFacetaDW.ListaFacetaConfigProyRangoFecha.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.PropiedadNueva.Equals(propNueva) && item.PropiedadFin.Equals(propFin)).FirstOrDefault();

                FacetaConfigProyRangoFecha filaFacetaConfigProyRangoFecha = null;

                if (filasConfigProyRangoFecha != null)
                {
                    filaFacetaConfigProyRangoFecha = filasConfigProyRangoFecha;
                    listaModificadas.Add(filaFacetaConfigProyRangoFecha);
                }

                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.FacetaConfigProyRangoFecha, facetaConfigProyRangoFechaAux, filaFacetaConfigProyRangoFecha);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.FacetaConfigProyRangoFecha.Add(facetaConfigProyRangoFechaAux);
                }
            }

            if (!mListaModificadasEF.ContainsKey(typeof(FacetaConfigProyRangoFecha).Name))
            {
                mListaOriginalesEF.Add(typeof(FacetaConfigProyRangoFecha).Name, new List<object>(pFacetaDW.ListaFacetaConfigProyRangoFecha));
                mListaModificadasEF.Add(typeof(FacetaConfigProyRangoFecha).Name, new List<object>());
            }
            mListaModificadasEF[typeof(FacetaConfigProyRangoFecha).Name
                ].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura la FacetaProyectoMapa de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pConfigFacProyMapa">Nodo xml con la configuracion de la FacetaProyectoMapa</param>
        /// <param name="pFacetaDW">Dataset de Facetas</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void ConfigurarFacetaConfiguracionProyectoMapaConXML(XmlNodeList pConfigFacProyMapa, DataWrapperFacetas pFacetaDW, Guid pOrganizacionID, Guid pProyectoID)
        {
            foreach (XmlNode nodo in pConfigFacProyMapa)
            {
                string propLatitud = (string)LeerNodo(nodo, "PropLatitud", typeof(string));
                string propLongitud = (string)LeerNodo(nodo, "PropLongitud", typeof(string));
                string propRuta = (string)LeerNodo(nodo, "PropRuta", typeof(string));
                string colorRuta = (string)LeerNodo(nodo, "ColorRuta", typeof(string));

                //creo el diccionario para control de cambios
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                FacetaConfigProyMapa facetaConfigProyMapaAux = new FacetaConfigProyMapa();
                listaCampos.Add(nameof(facetaConfigProyMapaAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(facetaConfigProyMapaAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(facetaConfigProyMapaAux.PropLatitud), propLatitud);
                listaCampos.Add(nameof(facetaConfigProyMapaAux.PropLongitud), propLongitud);
                listaCampos.Add(nameof(facetaConfigProyMapaAux.PropRuta), propRuta);
                listaCampos.Add(nameof(facetaConfigProyMapaAux.ColorRuta), colorRuta);

                FacetaConfigProyMapa filaFacetaConfigProyMapa = pFacetaDW.ListaFacetaConfigProyMapa.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.FacetaConfigProyMapa, facetaConfigProyMapaAux, filaFacetaConfigProyMapa);

                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.FacetaConfigProyMapa.Add(facetaConfigProyMapaAux);
                }
            }
        }

        /// <summary>
        /// Configura la FacetaProyectoChart de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pConfigFacProyChart"></param>
        /// <param name="pFacetaDW">Dataset de Facetas</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void ConfigurarFacetaConfiguracionProyChart(XmlNodeList pListaConfigFacProyChart, DataWrapperFacetas pFacetaDW, Guid pOrganizacionID, Guid pProyectoID)
        {
            List<FacetaConfigProyChart> listaModificadas = new List<FacetaConfigProyChart>();
            foreach (XmlNode config in pListaConfigFacProyChart)
            {
                string nombre = (string)LeerNodo(config, "Nombre", typeof(string));
                Guid chartID = Guid.Empty;
                List<FacetaConfigProyChart> fila = pFacetaDW.ListaFacetaConfigProyChart.Where(item => item.Nombre.Equals(nombre)).ToList();
                if (fila.Count > 0)
                {
                    chartID = fila[0].ChartID;
                }
                else
                {
                    chartID = Guid.NewGuid();
                }
                short ordenConfProyChart = (short)LeerNodo(config, "Orden", typeof(short));
                string selectVirtuoso = (string)LeerNodo(config, "SelectConsultaVirtuoso", typeof(string));
                string filtrosVirtuoso = (string)LeerNodo(config, "FiltrosConsultaVirtuoso", typeof(string));
                string filtrosVirtuosoGroupBy = (string)LeerNodo(config, "FiltrosGroupByVirtuoso", typeof(string));
                string filtrosVirtuosoOrderBy = (string)LeerNodo(config, "FiltrosOrderByVirtuoso", typeof(string));
                string filtrosVirtuosoLimit = (string)LeerNodo(config, "FiltrosLimitVirtuoso", typeof(string));

                if (!string.IsNullOrEmpty(filtrosVirtuosoGroupBy) || !string.IsNullOrEmpty(filtrosVirtuosoOrderBy) || !string.IsNullOrEmpty(filtrosVirtuosoLimit))
                {
                    filtrosVirtuoso += "|||" + filtrosVirtuosoGroupBy + "|||" + filtrosVirtuosoOrderBy + "|||" + filtrosVirtuosoLimit;
                }

                string jsBase = (string)LeerNodo(config, "JSBase", typeof(string));
                string jsBusqueda = (string)LeerNodo(config, "JSBusqueda", typeof(string));
                XmlNodeList nodosOnto = config.SelectNodes("Ontologia");

                //creo el diccionario para control de cambios
                Dictionary<string, object> listaCampos = new Dictionary<string, object>();
                FacetaConfigProyChart facetaConfigProyChartAux = new FacetaConfigProyChart();

                listaCampos.Add(nameof(facetaConfigProyChartAux.OrganizacionID), pOrganizacionID);
                listaCampos.Add(nameof(facetaConfigProyChartAux.ProyectoID), pProyectoID);
                listaCampos.Add(nameof(facetaConfigProyChartAux.ChartID), chartID);
                listaCampos.Add(nameof(facetaConfigProyChartAux.Nombre), nombre);
                listaCampos.Add(nameof(facetaConfigProyChartAux.Orden), ordenConfProyChart);
                listaCampos.Add(nameof(facetaConfigProyChartAux.SelectConsultaVirtuoso), selectVirtuoso);
                listaCampos.Add(nameof(facetaConfigProyChartAux.FiltrosConsultaVirtuoso), filtrosVirtuoso);
                listaCampos.Add(nameof(facetaConfigProyChartAux.JSBase), jsBase);
                listaCampos.Add(nameof(facetaConfigProyChartAux.JSBusqueda), jsBusqueda);

                if (nodosOnto != null && nodosOnto.Count > 0)
                {
                    string ontologias = "";

                    foreach (XmlNode nodo in nodosOnto)
                    {
                        ontologias += nodo.InnerText + ",";
                    }

                    ontologias = ontologias.Substring(0, ontologias.Length - 1);
                    listaCampos.Add(nameof(facetaConfigProyChartAux.Ontologias), ontologias);
                }

                //busco la fila para el control de cambios
                FacetaConfigProyChart filaFacetaConfigProyChart = pFacetaDW.ListaFacetaConfigProyChart.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.ChartID.Equals(chartID)).FirstOrDefault();
                ObjectState resultado = InsertarCambiosEnEF(listaCampos, mEntityContext.FacetaConfigProyChart, facetaConfigProyChartAux, filaFacetaConfigProyChart);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.FacetaConfigProyChart.Add(facetaConfigProyChartAux);
                }
                if (filaFacetaConfigProyChart != null)
                {
                    listaModificadas.Add(filaFacetaConfigProyChart);
                }
            }
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pFacetaDS.FacetaConfigProyChart, listaModificadas);
            if (!mListaModificadasEF.ContainsKey(typeof(FacetaConfigProyChart).Name))
            {
                mListaOriginalesEF.Add(typeof(FacetaConfigProyChart).Name, new List<object>(pFacetaDW.ListaFacetaConfigProyChart));
                mListaModificadasEF.Add(typeof(FacetaConfigProyChart).Name, new List<object>());
            }
            mListaModificadasEF[typeof(FacetaConfigProyChart).Name].AddRange(listaModificadas);
        }

        /// <summary>
        /// Configura las Facetas de un proyecto determinado con los parámetros que s ehan pasado desde un XML
        /// </summary>
        /// <param name="pFac">Lista de nodos xml con la configuración de las Facetas</param>
        /// <param name="pFacetaDW">Dataset de Facetas</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void ConfigurarFacetas(XmlNodeList pListaFaceta, DataWrapperFacetas pFacetaDW, Guid pOrganizacionID, Guid pProyectoID, DataWrapperProyecto pProyectoDW)
        {
            foreach (XmlNode fac in pListaFaceta)
            {
                string faceta = (string)LeerNodo(fac, "ClaveFaceta", typeof(string));
                short reciproca = 0;

                string reciprocaString = (string)LeerNodo(fac, "Reciproca", typeof(string));
                if (!string.IsNullOrEmpty(reciprocaString))
                {
                    if (short.TryParse(reciprocaString, out reciproca))
                    {
                        throw new Exception("En un nodo 'Reciproca' no puede ir un número");
                    }
                    else
                    {
                        reciproca = (short)reciprocaString.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (string.IsNullOrEmpty(faceta))
                        {
                            faceta = reciprocaString;
                        }
                        else
                        {
                            faceta = reciprocaString + "@@@" + faceta;
                        }
                    }
                }

                short mayusculas = (short)LeerNodo(fac, "Mayusculas", typeof(short));
                if (mayusculas == -1)
                {
                    mayusculas = 1;
                }

                string objetoConocimiento = (string)LeerNodo(fac, "ObjetoConocimiento", typeof(string));

                XmlNodeList pestanyasProyecto = fac.SelectNodes("Pestanyas");

                string nombreFaceta = (string)LeerNodo(fac, "NombreFaceta", typeof(string));
                short ordenFaceta = (short)LeerNodo(fac, "OrdenFaceta", typeof(short));

                //pedimos el tipoDisenio como un entero
                //short tipoDisenio = (short)LeerNodoTipoDisenio(fac, "TipoDisenio");

                short tipoDisenio = 0;
                string disenio = (string)LeerNodo(fac, "TipoDisenio", typeof(string));

                try
                {
                    tipoDisenio = (short)((TipoDisenio)Enum.Parse(typeof(TipoDisenio), disenio));
                }
                catch { }

                short elementosVisibles = (short)LeerNodo(fac, "ElementosVisibles", typeof(short));

                //si el algoritmoTransformacion va a ser "Ninguno", no hace falta que el usuario configure ese nodo
                string nodoAlgoritmoTransformacion = (string)LeerNodo(fac, "AlgoritmoTransformacion", typeof(string));
                short algoritmoTransformacion = (short)TiposAlgoritmoTransformacion.Ninguno;
                if (!nodoAlgoritmoTransformacion.Equals(string.Empty))
                {
                    algoritmoTransformacion = ObtenerAlgoritmoTransformacion(nodoAlgoritmoTransformacion);
                }

                //Leemos el nivel semántico, solo para los Algoritmos de transformación de tesaurosemántico.
                string nivelSemantico = (string)LeerNodo(fac, "NivelSemantico", typeof(string));

                bool esSemantica = false;
                string semantica = (string)LeerNodo(fac, "EsSemantica", typeof(string));
                if (semantica.Equals("1"))
                {
                    esSemantica = true;
                }
                bool autoCompletar = false;
                string completar = (string)LeerNodo(fac, "Autocompletar", typeof(string));
                if (completar.Equals("1"))
                {
                    autoCompletar = true;
                }

                string tipoPropiedad = (string)LeerNodo(fac, "TipoPropiedad", typeof(string));

                #region switch_tipoPropiedad
                short propiedad = 0;

                try
                {
                    propiedad = (short)((TipoPropiedadFaceta)Enum.Parse(typeof(TipoPropiedadFaceta), tipoPropiedad));
                }
                catch { }
                #endregion switch_tipoPropiedad

                XmlNodeList filtrosProyecto = fac.SelectNodes("Filtros/Filtro");

                string nodoComportamiento = (string)LeerNodo(fac, "Comportamiento", typeof(string));
                #region switch_TipoMostrarSoloCaja
                short comportamiento = 0;

                try
                {
                    comportamiento = (short)((TipoMostrarSoloCaja)Enum.Parse(typeof(TipoMostrarSoloCaja), nodoComportamiento));
                }
                catch { }
                #endregion switch_TipoMostrarSoloCaja

                //Excluyente = 1 = true
                bool excluyente = true;
                string excluyenteString = (string)LeerNodo(fac, "Excluyente", typeof(string));
                if (excluyenteString.Equals("0"))
                {
                    excluyente = false;
                }

                string condicionFaceta = (string)LeerNodo(fac, "Condicion", typeof(string));

                if (string.IsNullOrEmpty(condicionFaceta))
                {
                    // Se ha cambiado el campo SubTipo por Condicion
                    // Por compatibilidad, compruebo si el campo subtipo llega
                    condicionFaceta = (string)LeerNodo(fac, "SubTipo", typeof(string));
                    if (!string.IsNullOrEmpty(condicionFaceta))
                    {
                        string[] filtrosGnossType = condicionFaceta.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        condicionFaceta = "";
                        string or = "";
                        foreach (string filtro in filtrosGnossType)
                        {
                            condicionFaceta += or + "gnoss:type=" + filtro;
                            or = "|";
                        }
                    }
                }
                string pestanyaFacetaHome = (string)LeerNodo(fac, "PestanyaFacetaHome", typeof(string));
                XmlNodeList filtrosHome = fac.SelectNodes("FiltrosHome/FiltroHome");
                short ordenHome = (short)LeerNodo(fac, "OrdenHome", typeof(short));
                bool mostrarVerMas = false;
                short verMas = (short)LeerNodo(fac, "MostrarVerMas", typeof(short));
                if (verMas == 1)
                {
                    mostrarVerMas = true;
                }

                bool mostrarInmutable = false;
                short inmutable = (short)LeerNodo(fac, "Inmutable", typeof(short));
                if (inmutable == 1)
                {
                    mostrarInmutable = true;
                }

                bool ocultaEnFacetas = false; //0
                short nodoOcultaEnFacetas = (short)LeerNodo(fac, "OcultaEnFacetas", typeof(short));
                if (nodoOcultaEnFacetas == 1)
                {
                    ocultaEnFacetas = true;
                }

                bool ocultaEnFiltros = false; //0
                short nodoOcultaEnFiltros = (short)LeerNodo(fac, "OcultaEnFiltros", typeof(short));
                if (nodoOcultaEnFiltros == 1)
                {
                    ocultaEnFiltros = true;
                }

                string facetaPrivadaParaGrupoEditores = (string)LeerNodo(fac, "FacetaPrivadaParaGrupoEditores", typeof(string));

                bool comportamientoOr = false; //0
                short nodocomportamientoOr = (short)LeerNodo(fac, "ComportamientoOr", typeof(short));
                if (nodocomportamientoOr == 1)
                {
                    comportamientoOr = true;
                }

                bool priorizarOrdenResultados = false; //0
                short nodoPriorizarOrdenResultados = (short)LeerNodo(fac, "PriorizarOrdenResultados", typeof(short));
                if (nodoPriorizarOrdenResultados == 1)
                {
                    priorizarOrdenResultados = true;
                }

                //La tabla faceta NO se edita de ninguna manera desde una comunidad
                #region TablaFaceta
                #endregion TablaFaceta

                //La tabla FacetaObjetoConocimiento NO se edita de ninguna manera desde una comunidad
                #region TablaFacetaObjetoConocimiento
                #endregion TablaFacetaObjetoConocimiento

                List<FacetaObjetoConocimientoProyecto> listaModificadasObjetoConocimientoProyecto = new List<FacetaObjetoConocimientoProyecto>();
                List<FacetaFiltroProyecto> listaModificadasFiltroProyecto = new List<FacetaFiltroProyecto>();
                List<FacetaHome> listaModificadasFacetaHome = new List<FacetaHome>();
                List<FacetaFiltroHome> listaModificadasFiltroHome = new List<FacetaFiltroHome>();
                List<FacetaObjetoConocimientoProyectoPestanya> listaFacetasObjetoConocimientoProyectoPestanya = new List<FacetaObjetoConocimientoProyectoPestanya>();

                #region TablaFacetaObjetoConocimientoProyecto

                Dictionary<string, object> listaCamposFacetaObjetoConocimientoProyecto = ObtenerDiccionarioFacetaObjetoConocimientoProyecto(pFacetaDW, pOrganizacionID, pProyectoID, objetoConocimiento, faceta, ordenFaceta, autoCompletar, propiedad, comportamiento, excluyente, condicionFaceta, nombreFaceta, tipoDisenio, elementosVisibles, algoritmoTransformacion, nivelSemantico, esSemantica, mayusculas, facetaPrivadaParaGrupoEditores, reciproca, comportamientoOr, ocultaEnFacetas, ocultaEnFiltros, priorizarOrdenResultados, mostrarInmutable);

                FacetaObjetoConocimientoProyecto filaFacetaObjetoConocimientoProyecto = pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.ObjetoConocimiento.Equals(objetoConocimiento) && item.Faceta.Equals(faceta)).FirstOrDefault();
                FacetaObjetoConocimientoProyecto facetaObjetoConocimientoProyectoAux = new FacetaObjetoConocimientoProyecto();
                ObjectState resultado = InsertarCambiosEnEF(listaCamposFacetaObjetoConocimientoProyecto, mEntityContext.FacetaObjetoConocimientoProyecto, facetaObjetoConocimientoProyectoAux, filaFacetaObjetoConocimientoProyecto);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.FacetaObjetoConocimientoProyecto.Add(facetaObjetoConocimientoProyectoAux);
                }
                listaModificadasObjetoConocimientoProyecto.Add(filaFacetaObjetoConocimientoProyecto);

                #endregion TablaFacetaObjetoConocimientoProyecto

                #region TablaFacetaFiltroProyecto
                if (filtrosProyecto != null && filtrosProyecto.Count > 0)
                {
                    listaModificadasFiltroProyecto.AddRange(ConfigurarFacetasFiltroProyecto(filtrosProyecto, pFacetaDW, pOrganizacionID, pProyectoID, objetoConocimiento, faceta));
                }
                else
                {
                    ////si no viene presentación en el xml borro las filas de esa ontología del DS
                    //FacetaDS.FacetaFiltroProyectoRow[] listaFacFilPro = (FacetaDS.FacetaFiltroProyectoRow[])pFacetaDS.FacetaFiltroProyecto.Select("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and ObjetoConocimiento='" + objetoConocimiento + "' and Faceta='" + faceta + "'");
                    //foreach (FacetaDS.FacetaFiltroProyectoRow ffpr in listaFacFilPro)
                    //{
                    //    ffpr.Delete();
                    //}
                }
                #endregion TablaFacetaFiltroProyecto

                #region TablaFacetaHome
                if (pestanyaFacetaHome != string.Empty)
                {
                    //creo el diccionario para control de cambios para FacetaHome
                    Dictionary<string, object> listaCamposFacetaHome = new Dictionary<string, object>();
                    FacetaHome facetaHomeAux = new FacetaHome();
                    listaCamposFacetaHome.Add(nameof(facetaHomeAux.OrganizacionID), pOrganizacionID);
                    listaCamposFacetaHome.Add(nameof(facetaHomeAux.ProyectoID), pProyectoID);
                    listaCamposFacetaHome.Add(nameof(facetaHomeAux.ObjetoConocimiento), objetoConocimiento);
                    listaCamposFacetaHome.Add(nameof(facetaHomeAux.Faceta), faceta);
                    listaCamposFacetaHome.Add(nameof(facetaHomeAux.PestanyaFaceta), pestanyaFacetaHome);
                    listaCamposFacetaHome.Add(nameof(facetaHomeAux.Orden), ordenHome);
                    listaCamposFacetaHome.Add(nameof(facetaHomeAux.MostrarVerMas), mostrarVerMas);

                    FacetaHome filaFacetaHome = pFacetaDW.ListaFacetaHome.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.ObjetoConocimiento.Equals(objetoConocimiento) && item.Faceta.Equals(faceta)).FirstOrDefault();
                    ObjectState resultadoCambios = InsertarCambiosEnEF(listaCamposFacetaHome, mEntityContext.FacetaHome, facetaHomeAux, filaFacetaHome);
                    if (resultado.Equals(ObjectState.Agregado))
                    {
                        mEntityContext.FacetaHome.Add(facetaHomeAux);
                    }
                    if (filaFacetaHome != null)
                    {
                        listaModificadasFacetaHome.Add(filaFacetaHome);
                    }
                }
                #endregion TablaFacetaHome

                #region TablaFacetaFiltroHome
                if (filtrosHome != null && filtrosHome.Count > 0)
                {
                    listaModificadasFiltroHome.AddRange(ConfigurarFacetasFiltroHome(filtrosHome, pFacetaDW, pOrganizacionID, objetoConocimiento, pProyectoID, faceta));
                }
                else
                {
                    ////si no viene presentación en el xml borro las filas de esa ontología del DS
                    //FacetaDS.FacetaFiltroHomeRow[] listaFacFilHome = (FacetaDS.FacetaFiltroHomeRow[])pFacetaDS.FacetaFiltroHome.Select("OrganizacionID='" + pOrganizacionID.ToString() + "' and ProyectoID='" + pProyectoID.ToString() + "' and Faceta='" + faceta + "'");
                    //foreach (FacetaDS.FacetaFiltroHomeRow ffhr in listaFacFilHome)
                    //{
                    //    ffhr.Delete();
                    //}
                }
                #endregion TablaFacetaFiltroHome

                #region TablaFacetaObjetoConocimientoProyectoPestanya

                if (pestanyasProyecto != null && pestanyasProyecto.Count > 0)
                {
                    listaFacetasObjetoConocimientoProyectoPestanya.AddRange(ConfigurarFacetasObjetoConocimientoProyectoPestanya(pestanyasProyecto, pFacetaDW, pOrganizacionID, objetoConocimiento, pProyectoID, faceta, pProyectoDW));
                }

                #endregion TablaFacetaObjetoConocimientoProyectoPestanya
                //pFacetaDW.ListaFacetaObjetoConocimientoProyecto
                if (!mListaModificadasEF.ContainsKey(typeof(FacetaFiltroProyecto).Name))
                {
                    mListaOriginalesEF.Add(typeof(FacetaFiltroProyecto).Name, new List<object>(pFacetaDW.ListaFacetaFiltroProyecto));
                    mListaModificadasEF.Add(typeof(FacetaFiltroProyecto).Name, new List<object>());
                }
                mListaModificadasEF[typeof(FacetaFiltroProyecto).Name].AddRange(listaModificadasFiltroProyecto);

                if (!mListaModificadasEF.ContainsKey(typeof(FacetaFiltroHome).Name))
                {
                    mListaOriginalesEF.Add(typeof(FacetaFiltroHome).Name, new List<object>(pFacetaDW.ListaFacetaFiltroHome));
                    mListaModificadasEF.Add(typeof(FacetaFiltroHome).Name, new List<object>());
                }
                mListaModificadasEF[typeof(FacetaFiltroHome).Name].AddRange(listaModificadasFiltroHome);

                if (!mListaModificadasEF.ContainsKey(typeof(FacetaObjetoConocimientoProyectoPestanya).Name))
                {
                    mListaOriginalesEF.Add(typeof(FacetaObjetoConocimientoProyectoPestanya).Name, new List<object>(pFacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya));
                    mListaModificadasEF.Add(typeof(FacetaObjetoConocimientoProyectoPestanya).Name, new List<object>());
                }
                mListaModificadasEF[typeof(FacetaObjetoConocimientoProyectoPestanya).Name].AddRange(listaFacetasObjetoConocimientoProyectoPestanya);

                if (!mListaModificadasEF.ContainsKey(typeof(FacetaHome).Name))
                {
                    mListaOriginalesEF.Add(typeof(FacetaHome).Name, new List<object>(pFacetaDW.ListaFacetaHome));
                    mListaModificadasEF.Add(typeof(FacetaHome).Name, new List<object>());
                }
                mListaModificadasEF[typeof(FacetaHome).Name].AddRange(listaModificadasFacetaHome);

                if (!mListaModificadasEF.ContainsKey(typeof(FacetaObjetoConocimientoProyecto).Name))
                {
                    mListaOriginalesEF.Add(typeof(FacetaObjetoConocimientoProyecto).Name, new List<object>(pFacetaDW.ListaFacetaObjetoConocimientoProyecto));
                    mListaModificadasEF.Add(typeof(FacetaObjetoConocimientoProyecto).Name, new List<object>());
                }
                mListaModificadasEF[typeof(FacetaObjetoConocimientoProyecto).Name].AddRange(listaModificadasObjetoConocimientoProyecto);
            }
        }

        private Dictionary<string, object> ObtenerDiccionarioFacetaObjetoConocimientoProyecto(DataWrapperFacetas pFacetaDW, Guid pOrganizacionID, Guid pProyectoID, string pObjetoConocimiento, string pFaceta, short pOrdenFaceta, bool pAutoCompletar, short pPropiedad, short pComportamiento, bool pExcluyente, string pCondicionFaceta, string pNombreFaceta, short pTipoDisenio, short pElementosVisibles, short pAlgoritmoTransformacion, string pNivelSemantico, bool pEsSemantica, short pMayusculas, string pFacetaPrivadaParaGrupoEditores, short pReciproca, bool pComportamientoOr, bool pOcultaEnFacetas, bool pOcultaEnFiltros, bool pPriorizarOrdenResultados, bool pMostrarInmutable)
        {
            //creo el diccionario para control de cambios para FacetaObjetoConocimientoProyecto

            Dictionary<string, object> listaCamposFacetaObjetoConocimientoProyecto = new Dictionary<string, object>();
            FacetaObjetoConocimientoProyecto aux = new FacetaObjetoConocimientoProyecto();

            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.OrganizacionID), pOrganizacionID);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.ProyectoID), pProyectoID);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.ObjetoConocimiento), pObjetoConocimiento);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Faceta), pFaceta);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Orden), pOrdenFaceta);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Autocompletar), pAutoCompletar);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.TipoPropiedad), pPropiedad);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Comportamiento), pComportamiento);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.MostrarSoloCaja), false);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Excluyente), pExcluyente);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Condicion), pCondicionFaceta);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.NombreFaceta), pNombreFaceta);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.TipoDisenio), pTipoDisenio);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.ElementosVisibles), pElementosVisibles);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.AlgoritmoTransformacion), pAlgoritmoTransformacion);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.NivelSemantico), pNivelSemantico);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.EsSemantica), pEsSemantica);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Mayusculas), pMayusculas);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.FacetaPrivadaParaGrupoEditores), pFacetaPrivadaParaGrupoEditores);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Reciproca), pReciproca);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.ComportamientoOr), pComportamientoOr);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.OcultaEnFacetas), pOcultaEnFacetas);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.OcultaEnFiltros), pOcultaEnFiltros);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.PriorizarOrdenResultados), pPriorizarOrdenResultados);
            listaCamposFacetaObjetoConocimientoProyecto.Add(nameof(aux.Inmutable), pMostrarInmutable);

            return listaCamposFacetaObjetoConocimientoProyecto;
        }

        /// <summary>
        /// Configura los filtros de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pFiltrosHome">Lista de nodos xml con la configuración de los Filtros del proyecto</param>
        /// <param name="pFacetaDW">Dataset de Facetas</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private List<FacetaFiltroProyecto> ConfigurarFacetasFiltroProyecto(XmlNodeList pFiltrosProyecto, DataWrapperFacetas pFacetaDW, Guid pOrganizacionID, Guid pProyectoID, string pObjetoConocimiento, string pFaceta)
        {
            List<FacetaFiltroProyecto> listaModificadas = new List<FacetaFiltroProyecto>();
            foreach (XmlNode filtro in pFiltrosProyecto)
            {
                string valorAcumulado = ObtenerValorAcumuladoFiltro(pProyectoID, filtro);

                short ordenFiltro = (short)LeerNodo(filtro, "OrdenFiltro", typeof(short)); //0;
                //short.TryParse(filtro.SelectSingleNode("OrdenFiltro").InnerText, out ordenFiltro);

                //creo el diccionario para control de cambios para FacetaHome

                Dictionary<string, object> listaCamposFacetaFiltroProyecto = new Dictionary<string, object>();
                FacetaFiltroProyecto facetaFiltroProyectoAux = new FacetaFiltroProyecto();

                listaCamposFacetaFiltroProyecto.Add(nameof(facetaFiltroProyectoAux.OrganizacionID), pOrganizacionID);
                listaCamposFacetaFiltroProyecto.Add(nameof(facetaFiltroProyectoAux.ProyectoID), pProyectoID);
                listaCamposFacetaFiltroProyecto.Add(nameof(facetaFiltroProyectoAux.ObjetoConocimiento), pObjetoConocimiento);
                listaCamposFacetaFiltroProyecto.Add(nameof(facetaFiltroProyectoAux.Faceta), pFaceta);
                listaCamposFacetaFiltroProyecto.Add(nameof(facetaFiltroProyectoAux.Filtro), valorAcumulado);
                listaCamposFacetaFiltroProyecto.Add(nameof(facetaFiltroProyectoAux.Orden), ordenFiltro);

                FacetaFiltroProyecto filaFacetaFiltroProyecto = pFacetaDW.ListaFacetaFiltroProyecto.FirstOrDefault(row => pOrganizacionID.Equals(row.OrganizacionID) && pProyectoID.Equals(row.ProyectoID) && pObjetoConocimiento.Equals(row.ObjetoConocimiento) && pFaceta.Equals(row.Faceta) && valorAcumulado.Equals(row.Filtro));

                //pEliminar = true porque se ha dado el caso de que están modificando un campo de la clave para formar una clave que ya existe en la tabla
                //así que primero se borra y luego se añade
                ObjectState resultado = InsertarCambiosEnEF(listaCamposFacetaFiltroProyecto, mEntityContext.FacetaFiltroProyecto, facetaFiltroProyectoAux, filaFacetaFiltroProyecto, true);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.FacetaFiltroProyecto.Add(facetaFiltroProyectoAux);
                }
                if (filaFacetaFiltroProyecto != null)
                {
                    listaModificadas.Add(filaFacetaFiltroProyecto);
                }
            }
            return listaModificadas;
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pFacetaDS.FacetaFiltroProyecto, listaModificadas);            
        }

        private string ObtenerValorAcumuladoFiltro(Guid pProyectoID, XmlNode pFiltro)
        {
            string valor;
            string valorFiltro = "";
            string valorAcumulado = "";
            XmlNodeList valores = pFiltro.SelectNodes("Valor");
            bool negada;
            int cont = 0;
            foreach (XmlNode nodoValor in valores)
            {
                cont++;
                negada = false;
                valor = "";
                //valorFiltro = (string)LeerNodo(nodoValor, "Valor", typeof(string));//puede venir un guid o un nombre del categoriatesauro
                valorFiltro = nodoValor.InnerText;
                if (valorFiltro.StartsWith("!"))
                {
                    valorFiltro = valorFiltro.Substring(1);
                    negada = true;
                }

                Guid newGuid = Guid.Empty;
                bool isGuid = Guid.TryParse(valorFiltro, out newGuid);

                if (!isGuid)
                {
                    TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> lista = tesauroCN.ObtenerTesauroYCategoria(pProyectoID, valorFiltro);
                    if (lista != null)
                    {
                        valor = lista[1].ToString();
                    }
                    else
                    {
                        //No es un GUID puede ser una categoría de tipo TesauroSemántico con los nombres y el orden que le corresponde.
                        valor = valorFiltro;
                    }
                }
                else
                {
                    valor = valorFiltro.ToUpper();
                }
                if (!valor.Equals(string.Empty))
                {
                    if (negada)
                    {
                        valor = "!" + valor;
                    }

                    valorAcumulado += valor;

                    if (cont < valores.Count)
                    {
                        valorAcumulado += "|";
                    }
                }
            }

            return valorAcumulado;
        }

        private List<FacetaObjetoConocimientoProyectoPestanya> ConfigurarFacetasObjetoConocimientoProyectoPestanya(XmlNodeList pPestanyasProyecto, DataWrapperFacetas pFacetaDW, Guid pOrganizacionID, string pObjetoConocimiento, Guid pProyectoID, string pFaceta, DataWrapperProyecto pDataWrapperproyecto)
        {
            List<FacetaObjetoConocimientoProyectoPestanya> listaModificadas = new List<FacetaObjetoConocimientoProyectoPestanya>();
            foreach (XmlNode filtro in pPestanyasProyecto)
            {
                string nombreCortoPestanya = "";
                XmlNodeList valores = filtro.SelectNodes("NombreCortoPestanya");
                foreach (XmlNode nodoValor in valores)
                {
                    nombreCortoPestanya = nodoValor.InnerText;

                    Guid pestanyaID = ObtenerPestanayaIDPorNombreCorto(pDataWrapperproyecto, nombreCortoPestanya);
                    FacetaObjetoConocimientoProyectoPestanya facetaObjetoConocimientoProyectoPestanyaAux = new FacetaObjetoConocimientoProyectoPestanya();

                    if (pestanyaID != null && pestanyaID != Guid.Empty)
                    {
                        //creo el diccionario para control de cambios para FacetaHome
                        Dictionary<string, object> listaCamposFacetaObjetoConocimientoProyectoPestanya = new Dictionary<string, object>();
                        listaCamposFacetaObjetoConocimientoProyectoPestanya.Add(nameof(facetaObjetoConocimientoProyectoPestanyaAux.OrganizacionID), pOrganizacionID);
                        listaCamposFacetaObjetoConocimientoProyectoPestanya.Add(nameof(facetaObjetoConocimientoProyectoPestanyaAux.ProyectoID), pProyectoID);
                        listaCamposFacetaObjetoConocimientoProyectoPestanya.Add(nameof(facetaObjetoConocimientoProyectoPestanyaAux.ObjetoConocimiento), pObjetoConocimiento);
                        listaCamposFacetaObjetoConocimientoProyectoPestanya.Add(nameof(facetaObjetoConocimientoProyectoPestanyaAux.Faceta), pFaceta);
                        listaCamposFacetaObjetoConocimientoProyectoPestanya.Add(nameof(facetaObjetoConocimientoProyectoPestanyaAux.PestanyaID), pestanyaID);

                        FacetaObjetoConocimientoProyectoPestanya filaFacetaObjetoConocimientoProyectoPestanya = pFacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.Faceta.Equals(pFaceta) && item.PestanyaID.Equals(pestanyaID)).FirstOrDefault();

                        //pEliminar = true porque se ha dado el caso de que están modificando un campo de la clave para formar una clave que ya existe en la tabla
                        //así que primero se borra y luego se añade
                        ObjectState resultado = InsertarCambiosEnEF(listaCamposFacetaObjetoConocimientoProyectoPestanya, mEntityContext.FacetaObjetoConocimientoProyectoPestanya, facetaObjetoConocimientoProyectoPestanyaAux, filaFacetaObjetoConocimientoProyectoPestanya, true);
                        if (resultado.Equals(ObjectState.Agregado))
                        {
                            mEntityContext.FacetaObjetoConocimientoProyectoPestanya.Add(facetaObjetoConocimientoProyectoPestanyaAux);
                        }
                        if (filaFacetaObjetoConocimientoProyectoPestanya != null)
                        {
                            listaModificadas.Add(filaFacetaObjetoConocimientoProyectoPestanya);
                        }
                    }
                }
            }
            return listaModificadas;
        }

        private Guid ObtenerPestanayaIDPorNombreCorto(DataWrapperProyecto pDataWrapperProyecto, string pNombreCortoPestanya)
        {
            Guid pestanyaID = Guid.Empty;
            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> busqueda = pDataWrapperProyecto.ListaProyectoPestanyaMenu.Where(fila => fila.NombreCortoPestanya.Equals(pNombreCortoPestanya)).ToList();
            if (busqueda.Count > 0)
            {
                AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu fila = busqueda[0];
                pestanyaID = fila.PestanyaID;
            }

            return pestanyaID;
        }

        /// <summary>
        /// Configura los filtros de la home de un proyecto determinado con los parámetros que se han pasado desde un XML
        /// </summary>
        /// <param name="pFiltrosHome">Lista de nodos xml con la configuración de los Filtros de la Home</param>
        /// <param name="pFacetasDW">DataWrapper de Facetas</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private List<FacetaFiltroHome> ConfigurarFacetasFiltroHome(XmlNodeList pFiltrosHome, DataWrapperFacetas pFacetasDW, Guid pOrganizacionID, string pObjetoConocimiento, Guid pProyectoID, string pFaceta)
        {
            List<FacetaFiltroHome> listaModificadas = new List<FacetaFiltroHome>();
            foreach (XmlNode filtro in pFiltrosHome)
            {
                #region viejo
                //string valor = "";
                //string valorFiltro = (string)LeerNodo(filtro, "Valor", typeof(string));//puede venir un guid o un nombre de categoriatesauro
                //bool isGuid = false;
                //Guid newGuid = new Guid();
                //try
                //{
                //    newGuid = new Guid(valorFiltro.ToUpper());
                //    isGuid = true;
                //    valor = valorFiltro.ToUpper();
                //}
                //catch (Exception ex)
                //{
                //    isGuid = false;
                //}
                //if (!isGuid)
                //{
                //    TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService);
                //    List<Guid> lista = tesauroCN.ObtenerTesauroYCategoria(pProyectoID, valorFiltro);
                //    if (lista != null)
                //    {
                //        valor = lista[1].ToString();
                //    }
                //}
                #endregion viejo

                string valor;
                string valorFiltro = "";
                string valorAcumulado = "";
                XmlNodeList valores = filtro.SelectNodes("Valor");
                bool negada;
                int cont = 0;
                foreach (XmlNode nodoValor in valores)
                {
                    cont++;
                    negada = false;
                    valor = "";
                    //valorFiltro = (string)LeerNodo(nodoValor, "Valor", typeof(string));//puede venir un guid o un nombre del categoriatesauro
                    valorFiltro = nodoValor.InnerText;
                    if (valorFiltro.StartsWith("!"))
                    {
                        valorFiltro = valorFiltro.Substring(1);
                        negada = true;
                    }

                    Guid newGuid = Guid.Empty;
                    bool isGuid = Guid.TryParse(valorFiltro, out newGuid);

                    if (!isGuid)
                    {
                        TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        List<Guid> lista = tesauroCN.ObtenerTesauroYCategoria(pProyectoID, valorFiltro);
                        if (lista != null)
                        {
                            valor = lista[1].ToString();
                        }
                        else
                        {
                            //No es un GUID puede ser una categoría de tipo TesauroSemántico con los nombres y el orden que le corresponde.
                            valor = valorFiltro;
                        }
                    }
                    else
                    {
                        valor = valorFiltro.ToUpper();
                    }
                    if (!valor.Equals(string.Empty))
                    {
                        if (negada)
                        {
                            valor = "!" + valor;
                        }

                        valorAcumulado += valor;

                        if (cont < valores.Count)
                        {
                            valorAcumulado += "|";
                        }
                    }
                }

                short ordenFiltro = (short)LeerNodo(filtro, "OrdenFiltro", typeof(short)); //0;

                //creo el diccionario para control de cambios para FacetaHome
                FacetaFiltroHome facetaFiltroHomeAux = new FacetaFiltroHome();
                Dictionary<string, object> listaCamposFacetaFiltroHome = new Dictionary<string, object>();
                listaCamposFacetaFiltroHome.Add(nameof(facetaFiltroHomeAux.OrganizacionID), pOrganizacionID);
                listaCamposFacetaFiltroHome.Add(nameof(facetaFiltroHomeAux.ProyectoID), pProyectoID);
                listaCamposFacetaFiltroHome.Add(nameof(facetaFiltroHomeAux.ObjetoConocimiento), pObjetoConocimiento);
                listaCamposFacetaFiltroHome.Add(nameof(facetaFiltroHomeAux.Faceta), pFaceta);
                listaCamposFacetaFiltroHome.Add(nameof(facetaFiltroHomeAux.Filtro), valorAcumulado);
                listaCamposFacetaFiltroHome.Add(nameof(facetaFiltroHomeAux.Orden), ordenFiltro);

                FacetaFiltroHome filaFacetaFiltroHome = pFacetasDW.ListaFacetaFiltroHome.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.ObjetoConocimiento.Equals(pObjetoConocimiento) && item.Faceta.Equals(pFaceta) && item.Filtro.Equals(valorAcumulado)).FirstOrDefault();

                //pEliminar = true porque se ha dado el caso de que están modificando un campo de la clave para formar una clave que ya existe en la tabla
                //así que primero se borra y luego se añade
                ObjectState resultado = InsertarCambiosEnEF(listaCamposFacetaFiltroHome, mEntityContext.FacetaFiltroHome, facetaFiltroHomeAux, filaFacetaFiltroHome, true);
                if (resultado.Equals(ObjectState.Agregado))
                {
                    mEntityContext.FacetaFiltroHome.Add(facetaFiltroHomeAux);
                }
                if (filaFacetaFiltroHome != null)
                {
                    listaModificadas.Add(filaFacetaFiltroHome);
                }
            }
            return listaModificadas;
            ////borro las que estan en el DS pero no en el xml
            //BorrarFilasDS(pFacetaDS.FacetaFiltroHome, listaModificadas);            
        }

        /// <summary>
        /// Lee los nodos en los que podemos recibir un valor de tipo string o short
        /// </summary>
        /// <param name="nodo">Nodo del XML</param>
        /// <param name="nom">Nombre del nodo</param>
        /// <returns>Salida contendrá un short con el tipo de diseño de la faceta</returns>
        private short LeerNodoTipoDisenio(XmlNode nodo, string nom)
        {
            short salida = 0;
            string tipoDisenio = nodo.SelectSingleNode(nom).InnerText;
            switch (tipoDisenio)
            {
                case "DesdeHastaDiasMesAño":
                    salida = 0;
                    break;
                case "ListaOrdCantidad":
                    salida = 1;
                    break;
                case "EnCreacion":
                    salida = 2;
                    break;
                case "ListaMayorAMenor":
                    salida = 3;
                    break;
                case "ListaMenorAMayor":
                    salida = 4;
                    break;
                case "Calendario":
                    salida = 5;
                    break;
                case "RangoSoloDesde":
                    salida = 6;
                    break;
                case "RangoSoloHasta":
                    salida = 7;
                    break;
                case "CalendarioConRangos":
                    salida = 8;
                    break;
                case "ListaOrdCantidadTesauro":
                    salida = 9;
                    break;
            }
            return salida;
        }

        /// <summary>
        /// Lee los nodos en los que podemos recibir un valor de tipo string o short
        /// </summary>
        /// <param name="nodo">Nodo del XML</param>
        /// <param name="nom">Nombre del nodo</param>
        /// <param name="pTipo">Tipo que se espera recibir</param>
        /// <returns>Object que contendrá una string o un short</returns>
        private object LeerNodo(XmlNode nodo, string nom, Type pTipo)
        {
            Object salida = null;
            if (nodo != null)
            {
                if (nodo.SelectSingleNode(nom) != null)
                {
                    if (pTipo.Equals(typeof(short)))
                    {
                        short enteroCorto = 0;
                        short.TryParse(nodo.SelectSingleNode(nom).InnerText, out enteroCorto);
                        salida = enteroCorto;
                    }
                    if (pTipo.Equals(typeof(string)))
                    {
                        salida = nodo.SelectSingleNode(nom).InnerText;
                    }
                }
                else if (pTipo.Equals(typeof(short)))
                {
                    //es nulo
                    salida = (short)-1;
                }
                else
                {
                    salida = string.Empty;
                }
            }
            else if (pTipo.Equals(typeof(short)))
            {
                //es nulo
                salida = (short)-1;
            }
            else
            {
                salida = string.Empty;
            }
            return salida;
        }

        /// <summary>
        /// Borra de la tabla todas las filas salvo las que están en la lista pasada como parámetro
        /// </summary>
        private void BorrarFilasSobrantes()
        {
            foreach (string tabla in mListaModificadasEF.Keys)
            {
                BorrarFilas(tabla, mListaModificadasEF[tabla]);
            }
        }

        /// <summary>
        /// Borra de la tabla todas las filas salvo las que están en la lista pasada como parámetro
        /// </summary>
        /// <param name="pTabla"></param>
        /// <param name="pListaFilas"></param>
        private void BorrarFilas(string pNombreLista, List<object> pListaFilas)
        {
            List<DataRow> listaFilasABorrar = new List<DataRow>();
            //busco las que no están en la lista y no son las añadidas nuevas
            foreach (object fila in mListaOriginalesEF[pNombreLista])
            {
                if (PuedeBorrarFila(fila, pListaFilas))
                {
                    mEntityContext.EliminarElemento(fila);
                }
            }
        }

        /// <summary>
        /// Comprueba si la fila se puede borrar (no está en las modificadas y no es añadida y no es gadget o gadgetcontexto y no es ni comparticionautomatica ni comparticionautomaticareglas ni comparticionautomaticamapping)
        /// </summary>
        /// <param name="fila">Fila a comprobar</param>
        /// <param name="pListaFilas">Lista de filas a excluir del borrado</param>
        /// <returns>Devuelve true si la fila se puede borrar</returns>
        private bool PuedeBorrarFila(object fila, List<object> pListaFilas)
        {
            bool salida = false;
            if (!pListaFilas.Contains(fila) && !mEntityContext.EsElementoModified(fila) /*&&
                (!(fila is ProyectoDS.ProyectoGadgetRow) || (((ProyectoDS.ProyectoGadgetRow)fila).Tipo.Equals((short)TipoGadget.RecursosContextos)))*/
                && (!(fila is ComparticionAutomatica) && !(fila is ComparticionAutomaticaReglas) && !(fila is ComparticionAutomaticaMapping))
               )
            {
                salida = true;
            }

            return salida;
        }

        /// <summary>
        /// Obtiene el short TipoAlgoritmoTransformacion correspondiente
        /// </summary>
        /// <param name="pAlgoritmoTransformacion">Cadena de AlgortimoTransformacion de la que se quiere obtener su short</param>
        /// <returns>short con el AlgoritmoTransformacion, 8 (Ninguno) por defecto</returns>
        private short ObtenerAlgoritmoTransformacion(string pAlgoritmoTransformacion)
        {
            short salida = 8; //equivale a AlgoritmoTransformacion.Ninguno

            try
            {
                salida = (short)((TiposAlgoritmoTransformacion)Enum.Parse(typeof(TiposAlgoritmoTransformacion), pAlgoritmoTransformacion));
            }
            catch (Exception ex)
            {
                throw;
            }

            return salida;
        }

        #endregion Metodos Generales


    }
}