using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Amigos;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Solicitudes
{
    public class ControladorDeSolicitudes : ControladorBase
    {
        private EntityContextBASE mEntityContextBASE;

        public ControladorDeSolicitudes(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mEntityContextBASE = entityContextBASE;
        }

        public Guid GuardarRegistroOrganizacion(string pNombreCorto, string pNombre, Guid pUsuarioID)
        {
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GeneralCN generalCN = new GeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DateTime fechaHoy = generalCN.HoraServidor;

            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
            dataWrapperUsuario.ListaUsuario.Add(usuarioCN.ObtenerUsuarioPorID(pUsuarioID));
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            DataWrapperDocumentacion docDS = new DataWrapperDocumentacion();
            DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();
            DataWrapperTesauro tesauroDW = new DataWrapperTesauro();
            DataWrapperOrganizacion organizacionDW = new DataWrapperOrganizacion();
            DataWrapperProyecto proyectoDw = new DataWrapperProyecto();
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();

            GestionUsuarios gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);
            GestionOrganizaciones gestorOrg = new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext);
            GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
            GestionNotificaciones gestorNotificaciones =  new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            GestionTesauro gestorTesauro = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);
            GestorDocumental gestorDoc = new GestorDocumental(docDW, mLoggingService, mEntityContext);

            gestorUsuarios.GestorDocumental = gestorDoc;
            gestorUsuarios.GestorTesauro = gestorTesauro;

            gestorOrg.GestorTesauro = gestorTesauro;
            gestorOrg.GestorDocumental = gestorDoc;

            gestorPersonas.GestorUsuarios = gestorUsuarios;
            gestorPersonas.GestorOrganizaciones = gestorOrg;

            GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, gestorPersonas, gestorOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionAmigos gestorAmigos = new GestionAmigos(new DataWrapperAmigos(), gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionCorreo gestorCorreoInterno = new GestionCorreo(new CorreoDS(), gestorPersonas, gestorIdentidades, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            gestorIdentidades.GestorAmigos = gestorAmigos;
            gestorUsuarios.GestorIdentidades = gestorIdentidades;
            gestorIdentidades.GestorPersonas = gestorPersonas;
            gestorIdentidades.GestorOrganizaciones = gestorOrg;
            gestorIdentidades.GestorUsuarios = gestorUsuarios;

            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = dataWrapperUsuario.ListaUsuario.FirstOrDefault();
            SolicitudNuevoUsuario filaSU = null;
            Persona persona = null;
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = null;
            AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrg = null;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool existeFAQ = proyCN.ExisteProyectoFAQ();
            bool existeNoticias = proyCN.ExisteProyectoNoticias();
            bool existeDidactalia = proyCN.ExisteProyectoDidactalia();

            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();

            proyCN.Dispose();

            bool subirFoto = false;
            short tipoProyecto = (short)TipoProyecto.Comunidad;

            LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            LiveDS liveDS = new LiveDS();

            List<Guid> listaPerfilesEnvioCorreo = new List<Guid>();

            List<Guid> listaOrganizacions = new List<Guid>();
            string nombrePersona = "";
            string apellidosPersona = "";
            string emailPersona = "";
            string idiomaPersona = "";

            Elementos.ServiciosGenerales.Organizacion org = gestorOrg.AgregarOrganizacion();

            filaOrg = org.FilaOrganizacion;

            filaOrg.Nombre = pNombre;
            filaOrg.ModoPersonal = true;

            filaOrg.EsBuscable = true;
            filaOrg.EsBuscableExternos = false;
                    
            filaOrg.NombreCorto = pNombreCorto;
            filaOrg.Alias = pNombre;
            mEntityContext.SaveChanges();
            AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa filaEmpr = new AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa();

            filaEmpr.OrganizacionID = filaOrg.OrganizacionID;
            //filaOrg.OrganizacionEmpresa = filaEmpr;

            filaEmpr.Organizacion = filaOrg;
            organizacionDW.ListaOrganizacionEmpresa.Add(filaEmpr);
            mEntityContext.OrganizacionEmpresa.Add(filaEmpr);
            //GuardarLogo(filaOrg, filaSOrg.SolicitudID);
            mEntityContext.SaveChanges();
            ConfiguracionGnossOrg filaConfigOrg = new ConfiguracionGnossOrg();
            filaConfigOrg.OrganizacionID = filaOrg.OrganizacionID;

            if (filaOrg.EsBuscable)
            {
                filaConfigOrg.VerRecursos = true;
            }
            else
            {
                filaConfigOrg.VerRecursos = false;
            }
            filaConfigOrg.VerRecursosExterno = false;
            filaConfigOrg.VisibilidadContactos = 0;
            organizacionDW.ListaConfiguracionGnossOrg.Add(filaConfigOrg);
            mEntityContext.ConfiguracionGnossOrg.Add(filaConfigOrg);

            Perfil perfilOrg = gestorIdentidades.AgregarPerfilOrganizacion(org, recibirNewsletterDefectoProyectos);

            

            gestorOrg.AgregarOrganizacionAProyecto(org.Clave, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, perfilOrg.IdentidadMyGNOSS.Clave);

            Perfil perfil = null; // David: Variable utilizada para la creación del perfil personal del usuario

            List<Guid> listaProyectos = new List<Guid>();

              
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            dataWrapperPersona.Merge(personaCN.ObtenerPersonaPorUsuario(filaUsuario.UsuarioID, true, false));
            personaCN.Dispose();

            gestorPersonas.RecargarPersonas();

            persona = gestorPersonas.ListaPersonas[dataWrapperPersona.ListaPersona.FirstOrDefault().PersonaID];

            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestorOrg.OrganizacionDW.Merge(orgCN.ObtenerOrganizacionesVinculadasAPersona(persona.Clave));
            gestorOrg.CargarOrganizaciones();
            orgCN.Dispose();

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesDePersonaDeMyGNOSS(persona.Clave));
            identCN.Dispose();
            gestorIdentidades.RecargarHijos();
                
            persona = gestorPersonas.ListaPersonas.Values[0];
            AD.EntityModel.Models.PersonaDS.Persona filaPersGuardada = dataWrapperPersona.ListaPersona.FirstOrDefault();
            nombrePersona = filaPersGuardada.Nombre;
            idiomaPersona = filaPersGuardada.Idioma;
            apellidosPersona = filaPersGuardada.Apellidos;
            
            List<AD.EntityModel.Models.IdentidadDS.GrupoAmigos> filasGrupoAmigos = gestorIdentidades.GestorAmigos.AmigosDW.ListaGrupoAmigos.Where(item => item.Tipo == (int)TipoGrupoAmigos.AutomaticoOrganizacion && item.Automatico && item.IdentidadID.Equals(perfilOrg.IdentidadMyGNOSS.Clave)).ToList();

            UtilIdiomas utilIdiomas = new UtilIdiomas("", null, persona.FilaPersona.Idioma, ProyectoSeleccionado.Clave, Guid.Empty, Guid.Empty, mLoggingService, mEntityContext, mConfigService);

            gestorOrg.AgregarAdministradorDeOrganizacion(filaUsuario.UsuarioID, org.Clave, gestorUsuarios);
            mEntityContext.SaveChanges();
            Perfil perfilPersOrg = null;

            #region Vincular persona a organización o clase

            if (!persona.UsuarioCargado)
            {
                DataWrapperUsuario usuDW = new DataWrapperUsuario();
                usuDW.ListaUsuario.Add(usuarioCN.ObtenerUsuarioPorID(persona.UsuarioID));

                if (usuDW.ListaUsuario.Count.Equals(1))
                {
                    persona.GestorPersonas.GestorUsuarios.DataWrapperUsuario.Merge(usuDW);
                    persona.GestorPersonas.GestorUsuarios.RecargarUsuarios();
                }
            }



            #endregion

            DatosTrabajoPersonaOrganizacion perfilPersonaOrganizacion = gestorOrg.VincularPersonaOrganizacion(org, persona);
            PersonaVinculoOrganizacion filaOrgPersona = perfilPersonaOrganizacion.FilaVinculo;
            ControladorIdentidades controladorIdentidades = new ControladorIdentidades(gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            controladorIdentidades.CrearPerfilPersonaOrganizacion(persona, org, true, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, liveDS, recibirNewsletterDefectoProyectos);
            mEntityContext.SaveChanges();
            perfilPersOrg = gestorIdentidades.ObtenerPerfilDePersonaEnOrganizacion(org.Clave, persona.Clave);
            gestorUsuarios.AgregarUsuarioAProyecto(filaUsuario, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, perfilPersOrg.IdentidadMyGNOSS.Clave, false);
            mEntityContext.SaveChanges();
            gestorIdentidades.RecargarHijos();

            AD.EntityModel.Models.PersonaDS.DatosTrabajoPersonaLibre filaDatosTrabajoLibre = null;


            gestorTesauro.AgregarTesauroOrganizacion(org.Clave, utilIdiomas.GetText("TESAURO", "RECURSOSPUBLICOS"), utilIdiomas.GetText("TESAURO", "RECURSOSPRIVADOS"));
            gestorDoc.AgregarBRDeOrganizacion(org.Clave);
            mEntityContext.SaveChanges();
            AD.EntityModel.Models.IdentidadDS.Perfil filaPerfilEnOrg = perfilPersOrg.FilaPerfil;

            string asunto = "", mensaje = "";

            asunto = utilIdiomas.GetText("ACEPTARINVITACION", "ASUNTOMENSAJEBIENVENIDA");
            mensaje = utilIdiomas.GetText("ACEPTARINVITACION", "CUERPOMENSAJEBIENVENIDA");

            gestorCorreoInterno.GestorNotificaciones = gestorNotificaciones;

            List<Guid> listaDestinatarios = new List<Guid>();

            listaDestinatarios.Add(perfilOrg.IdentidadMyGNOSS.Clave);
            listaPerfilesEnvioCorreo.Add(perfilOrg.Clave);


            Guid correoID = Guid.Empty;
            List<ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EcosistemaSinMetaProyecto.ToString())).ToList();
            //bool ecosistemaSinMetaproyecto = ParametroAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinMetaProyecto.ToString() + "'").Length > 0 && bool.Parse((string)ParametroAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinMetaProyecto.ToString() + "'")[0]["Valor"]);
            bool ecosistemaSinMetaproyecto = busqueda.Count > 0 && bool.Parse(busqueda.First().Valor);

            if (!ecosistemaSinMetaproyecto)
            {
                correoID = gestorCorreoInterno.AgregarCorreo(Guid.Empty, listaDestinatarios, asunto, mensaje, this.BaseURL, TipoEnvioCorreoBienvenida.CorreoInterno, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                mEntityContext.SaveChanges();
            }

            if (correoID != Guid.Empty)
            {
                foreach (Guid destinatario in listaDestinatarios)
                {
                    new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarMensajeFacModeloBaseSimple(correoID, Guid.Empty, ProyectoSeleccionado.Clave, "base", destinatario.ToString(), null, PrioridadBase.Alta);
                    mEntityContext.SaveChanges();
                }
            }

            #region Agregar como amigos de la organización al propio administrador

            AmigosCN AmigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (!AmigosCN.EsAmigoDeIdentidad(perfilOrg.IdentidadMyGNOSS.Clave, perfilPersOrg.IdentidadMyGNOSS.Clave))
            {
                gestorAmigos.CrearFilaAmigo(perfilOrg.IdentidadMyGNOSS.Clave, perfilPersOrg.IdentidadMyGNOSS.Clave);
            }
            AmigosCN.Dispose();

                #endregion

                
            

            DataWrapperSuscripcion suscripcionDW = null;
            if (persona != null && persona.GestorPersonas != null && persona.GestorPersonas.GestorUsuarios != null && persona.GestorPersonas.GestorUsuarios.GestorIdentidades != null && persona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorSuscripciones != null && persona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorSuscripciones.SuscripcionDW.ListaSuscripcion.Count > 0)
            {
                //lo hago así porque este dataset puede no rellenarse en ControladorIdentidades.CrearPerfilPersonaOrganizacion
                suscripcionDW = persona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorSuscripciones.SuscripcionDW;
            }

            CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            mEntityContext.SaveChanges();
            correoCN.ActualizarCorreo(gestorCorreoInterno.CorreoDS);
            liveCN.ActualizarBD(liveDS);
            ControladorOrganizaciones controladorOrg = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            controladorOrg.ActualizarModeloBASE(perfilOrg.IdentidadMyGNOSS, ProyectoAD.MyGnoss, true, true, PrioridadBase.Alta);
            //ControladorCorreo.AgregarNotificacionCorreoNuevoAPerfiles(listaPerfilesEnvioCorreo);

            Elementos.ServiciosGenerales.Proyecto proyPrivClase = null;

            if (listaOrganizacions.Count > 0)
            {
                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionIdentidades gestorIdent = new GestionIdentidades(idenCN.ObtenerIdentidadesDeOrganizaciones(listaOrganizacions, ProyectoAD.MyGnoss), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                foreach (Identidad iden in gestorIdent.ListaIdentidades.Values)
                {
                    AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = gestorIdent.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(iden.PerfilID));
                    AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion filaPerfilOrg = gestorIdent.DataWrapperIdentidad.ListaPerfilOrganizacion.FirstOrDefault(perfilOrg => perfilOrg.PerfilID.Equals(iden.PerfilID));

                }
            }

            
            return filaOrg.OrganizacionID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSolicitudDW"></param>
        /// <param name="pSolicitudIDTexto"></param>
        /// <param name="pSolicitudesDeClase"></param>
        public Guid aceptarSolicitudOrganizacion(DataWrapperSolicitud pSolicitudDW, string pSolicitudIDTexto, bool pSolicitudesDeClase, Guid pUsuarioAdminID)
        {
            return aceptarSolicitudOrganizacion(pSolicitudDW, pSolicitudIDTexto, pSolicitudesDeClase, "", "", "", null, null, null, pUsuarioAdminID, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSolicitudDW"></param>
        /// <param name="pSolicitudIDTexto"></param>
        /// <param name="pSolicitudesDeClase"></param>
        public Guid aceptarSolicitudOrganizacion(DataWrapperSolicitud pSolicitudDW, string pSolicitudIDTexto, bool pSolicitudesDeClase, string pNombreComunidad, string pNombreCortoComunidad, string pCategoriasComunidad, List<string> pTagsComunidad, List<CategoriaTesauro> pCategoriasMyGnoss, SortedList<int, Elementos.ServiciosGenerales.Proyecto> pProyectosRel, Guid pUsuarioAdminID, List<Guid> pListaProyectosMiembro, GestionNotificaciones pGestorNotificaciones)
        {
            return aceptarSolicitudOrganizacion(pSolicitudDW, pSolicitudIDTexto, pSolicitudesDeClase, pNombreComunidad, pNombreCortoComunidad, pCategoriasComunidad, pTagsComunidad, pCategoriasMyGnoss, pProyectosRel, pUsuarioAdminID, pListaProyectosMiembro, pGestorNotificaciones, Guid.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSolicitudDW"></param>
        /// <param name="pSolicitudIDTexto"></param>
        /// <param name="pSolicitudesDeClase"></param>
        public Guid aceptarSolicitudOrganizacion(DataWrapperSolicitud pSolicitudDW, string pSolicitudIDTexto, bool pSolicitudesDeClase, string pNombreComunidad, string pNombreCortoComunidad, string pCategoriasComunidad, List<string> pTagsComunidad, List<CategoriaTesauro> pCategoriasMyGnoss, SortedList<int, Elementos.ServiciosGenerales.Proyecto> pProyectosRel, Guid pUsuarioAdminID, List<Guid> pListaProyectosMiembro, GestionNotificaciones pGestorNotificaciones, Guid pPerfilProfesorID)
        {
            ControladorPersonas controladorPers = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GeneralCN generalCN = new GeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DateTime fechaHoy = generalCN.HoraServidor;

            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
            dataWrapperUsuario.ListaUsuario.Add(usuarioCN.ObtenerUsuarioPorID(pUsuarioAdminID));
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            DataWrapperDocumentacion docDS = new DataWrapperDocumentacion();
            DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();
            DataWrapperTesauro tesauroDW = new DataWrapperTesauro();
            DataWrapperOrganizacion organizacionDW = new DataWrapperOrganizacion();
            DataWrapperProyecto proyectoDw = new DataWrapperProyecto();
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();

            if (pSolicitudesDeClase)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                dataWrapperIdentidad = identidadCN.ObtenerPerfilProfesorDeUsuario(pUsuarioAdminID);
                identidadCN.Dispose();
            }
            GestionUsuarios gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);
            GestionOrganizaciones gestorOrg = new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext);
            GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
            GestionNotificaciones gestorNotificaciones = null;

            if (pGestorNotificaciones == null)
            {
                gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                gestorNotificaciones = pGestorNotificaciones;
            }
            GestionTesauro gestorTesauro = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);
            GestorDocumental gestorDoc = new GestorDocumental(docDW, mLoggingService, mEntityContext);

            gestorUsuarios.GestorDocumental = gestorDoc;
            gestorUsuarios.GestorTesauro = gestorTesauro;

            gestorOrg.GestorTesauro = gestorTesauro;
            gestorOrg.GestorDocumental = gestorDoc;

            gestorPersonas.GestorUsuarios = gestorUsuarios;
            gestorPersonas.GestorOrganizaciones = gestorOrg;

            GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, gestorPersonas, gestorOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionAmigos gestorAmigos = new GestionAmigos(new DataWrapperAmigos(), gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionCorreo gestorCorreoInterno = new GestionCorreo(new CorreoDS(), gestorPersonas, gestorIdentidades, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            gestorIdentidades.GestorAmigos = gestorAmigos;
            gestorUsuarios.GestorIdentidades = gestorIdentidades;
            gestorIdentidades.GestorPersonas = gestorPersonas;
            gestorIdentidades.GestorOrganizaciones = gestorOrg;
            gestorIdentidades.GestorUsuarios = gestorUsuarios;

            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = dataWrapperUsuario.ListaUsuario.FirstOrDefault();
            SolicitudNuevoUsuario filaSU = null;
            Persona persona = null;
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = null;
            AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrg = null;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool existeFAQ = proyCN.ExisteProyectoFAQ();
            bool existeNoticias = proyCN.ExisteProyectoNoticias();
            bool existeDidactalia = proyCN.ExisteProyectoDidactalia();

            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();

            proyCN.Dispose();

            bool subirFoto = false;
            short tipoProyecto = (short)TipoProyecto.Comunidad;

            LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            LiveDS liveDS = new LiveDS();

            List<Guid> listaPerfilesEnvioCorreo = new List<Guid>();

            List<Guid> listaOrganizacions = new List<Guid>();

            foreach (SolicitudNuevaOrganizacion filaSOrg in pSolicitudDW.ListaSolicitudNuevaOrganizacion)
            {
                if ((string.IsNullOrEmpty(pSolicitudIDTexto)) || (filaSOrg.SolicitudID.ToString().Equals(pSolicitudIDTexto)))
                {
                    string nombrePersona = "";
                    string apellidosPersona = "";
                    string emailPersona = "";
                    string idiomaPersona = "";

                    filaSOrg.Solicitud.Estado = (short)AD.Usuarios.EstadoSolicitud.Aceptada;
                    filaSOrg.Solicitud.FechaProcesado = fechaHoy;

                    Elementos.ServiciosGenerales.Organizacion org = gestorOrg.AgregarOrganizacion();

                    filaOrg = org.FilaOrganizacion;
                    filaOrg.CP = filaSOrg.CP;
                    filaOrg.Direccion = filaSOrg.Direccion;
                    filaOrg.Localidad = filaSOrg.Poblacion;
                    filaOrg.PaisID = filaSOrg.PaisID;

                    if (!filaSOrg.ProvinciaID.HasValue)
                    {
                        filaOrg.Provincia = filaSOrg.Provincia;
                    }
                    else
                    {
                        filaOrg.ProvinciaID = filaSOrg.ProvinciaID;
                    }
                    filaOrg.Nombre = filaSOrg.Nombre;
                    filaOrg.Web = filaSOrg.PaginaWeb;
                    filaOrg.ModoPersonal = filaSOrg.ModoPersonal;
                    if (!pSolicitudesDeClase && filaSOrg.EsBuscable.HasValue)
                    {
                        filaOrg.EsBuscable = filaSOrg.EsBuscable.Value;
                        filaOrg.EsBuscableExternos = filaSOrg.EsBuscableExternos.Value;
                    }
                    else
                    {
                        filaOrg.EsBuscable = true;
                        filaOrg.EsBuscableExternos = false;
                    }
                    filaOrg.NombreCorto = filaSOrg.NombreCorto;
                    filaOrg.Alias = filaSOrg.Alias;


                    SolicitudNuevaOrgEmp filaSEmpr = filaSOrg.SolicitudNuevaOrgEmp;
                    AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa filaEmpr = new AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa();

                    filaEmpr.OrganizacionID = filaOrg.OrganizacionID;
                    filaEmpr.Empleados = filaSEmpr.Empleados;
                    filaEmpr.CIF = filaSEmpr.CIF;
                    filaEmpr.TipoOrganizacion = filaSEmpr.Tipo;
                    filaEmpr.SectorOrganizacion = filaSEmpr.Sector;
                    filaOrg.OrganizacionEmpresa = filaEmpr;
                    organizacionDW.ListaOrganizacionEmpresa.Add(filaEmpr);
                    mEntityContext.OrganizacionEmpresa.Add(filaEmpr);
                    GuardarLogo(filaOrg, filaSOrg.SolicitudID);

                    ConfiguracionGnossOrg filaConfigOrg = new ConfiguracionGnossOrg();
                    filaConfigOrg.OrganizacionID = filaOrg.OrganizacionID;

                    if (filaOrg.EsBuscable)
                    {
                        filaConfigOrg.VerRecursos = true;
                    }
                    else
                    {
                        filaConfigOrg.VerRecursos = false;
                    }
                    filaConfigOrg.VerRecursosExterno = false;
                    filaConfigOrg.VisibilidadContactos = 0;
                    organizacionDW.ListaConfiguracionGnossOrg.Add(filaConfigOrg);
                    mEntityContext.ConfiguracionGnossOrg.Add(filaConfigOrg);

                    Perfil perfilOrg = gestorIdentidades.AgregarPerfilOrganizacion(org, recibirNewsletterDefectoProyectos);

                    ControladorOrganizaciones controladorOrg = new ControladorOrganizaciones(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    controladorOrg.ActualizarModeloBASE(perfilOrg.IdentidadMyGNOSS, ProyectoAD.MyGnoss, true, true, PrioridadBase.Alta);

                    gestorOrg.AgregarOrganizacionAProyecto(org.Clave, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, perfilOrg.IdentidadMyGNOSS.Clave);
                    List<SolicitudNuevoUsuario> filasSU = pSolicitudDW.ListaSolicitudNuevoUsuario.Where(item => item.UsuarioID.Equals(filaSOrg.UsuarioAdminID)).ToList();
                    Perfil perfil = null; // David: Variable utilizada para la creación del perfil personal del usuario

                    List<Guid> listaProyectos = new List<Guid>();

                    if (filasSU.Count > 0)
                    {
                        filaSU = filasSU[0];
                        List<Solicitud> filas = pSolicitudDW.ListaSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudIDTexto)).ToList();
                        Solicitud fila = filaSU.Solicitud;

                        filaUsuario.EstaBloqueado = false;

                        fila.Estado = (short)AD.Usuarios.EstadoSolicitud.Aceptada;
                        fila.FechaProcesado = fechaHoy;

                        persona = gestorPersonas.AgregarPersona();
                        gestorPersonas.CrearDatosTrabajoPersonaLibre(persona);

                        AD.EntityModel.Models.PersonaDS.Persona filaPersona = persona.FilaPersona;
                        filaPersona.UsuarioID = filaSU.UsuarioID;
                        filaPersona.Apellidos = filaSU.Apellidos;
                        filaPersona.CPPersonal = filaSU.CP;
                        filaPersona.Email = filaSU.Email;
                        filaPersona.EsBuscable = filaSU.EsBuscable;
                        filaPersona.EsBuscableExternos = filaSU.EsBuscableExterno;
                        filaPersona.FechaNacimiento = filaSU.FechaNacimiento;
                        filaPersona.LocalidadPersonal = filaSU.Poblacion;
                        filaPersona.Nombre = filaSU.Nombre;
                        filaPersona.PaisPersonalID = filaSU.PaisID;
                        filaPersona.ProvinciaPersonal = filaSU.Provincia;
                        filaPersona.Idioma = filaSU.Idioma;
                        filaPersona.Sexo = filaSU.Sexo;
                        filaPersona.EstadoCorreccion = (short)EstadoCorreccion.NoCorreccion;

                        subirFoto = true;

                        //Guardar foto en el servidor y en la BD
                        GuardarFoto(filaPersona, filaSU.SolicitudID);

                        nombrePersona = filaPersona.Nombre;
                        idiomaPersona = filaPersona.Idioma;
                        apellidosPersona = filaPersona.Apellidos;

                        AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona filaConfigPers = gestorPersonas.AgregarConfiguracionGnossPersona(filaPersona.PersonaID);
                        filaPersona.ConfiguracionGnossPersona = filaConfigPers;

                        mEntityContext.Persona.Add(filaPersona);
                        if (persona.Usuario == null)
                        {
                            persona.GestorPersonas.GestorUsuarios = gestorUsuarios;
                        }
                        persona.UsuarioCargado = true;

                        bool IncluirDidactalia = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).ProyectoDebeRegistrarEnDidactalia(filaSU.Solicitud.ProyectoID);

                        perfil = gestorIdentidades.AgregarPerfilPersonal(filaPersona, true, ProyectoAD.MetaProyecto, ProyectoAD.MetaProyecto, recibirNewsletterDefectoProyectos);
                        filaIdentidad = ((Identidad)perfil.Hijos[0]).FilaIdentidad;

                        gestorUsuarios.CompletarUsuarioNuevo(filaUsuario);
                        gestorUsuarios.AgregarUsuarioAProyecto(filaUsuario, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, filaIdentidad.IdentidadID);
                        gestorIdentidades.RecargarHijos();

                        controladorPers.ActualizarModeloBASE(perfil.IdentidadMyGNOSS, ProyectoAD.MyGnoss, true, true, PrioridadBase.Alta);

                        Guid organizacionRegistroUsuario = filaSU.Solicitud.OrganizacionID;
                        Guid proyectoRegistroUsuario = filaSU.Solicitud.ProyectoID;

                        RegistrarUsuarioEnProyectosObligatorios(organizacionRegistroUsuario, proyectoRegistroUsuario, filaPersona.PersonaID, perfil, filaUsuario, gestorUsuarios, gestorIdentidades);
                        gestorIdentidades.RecargarHijos();
                    }
                    else
                    {
                        if (dataWrapperPersona.ListaPersona.Count == 0)
                        {
                            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            dataWrapperPersona.Merge(personaCN.ObtenerPersonaPorUsuario(filaUsuario.UsuarioID, true, false));
                            personaCN.Dispose();

                            gestorPersonas.RecargarPersonas();

                            persona = gestorPersonas.ListaPersonas[dataWrapperPersona.ListaPersona.FirstOrDefault().PersonaID];

                            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            gestorOrg.OrganizacionDW.Merge(orgCN.ObtenerOrganizacionesVinculadasAPersona(persona.Clave));
                            gestorOrg.CargarOrganizaciones();
                            orgCN.Dispose();

                            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            gestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadesDePersonaDeMyGNOSS(persona.Clave));
                            identCN.Dispose();
                            gestorIdentidades.RecargarHijos();
                        }
                        persona = gestorPersonas.ListaPersonas.Values[0];
                        AD.EntityModel.Models.PersonaDS.Persona filaPersGuardada = dataWrapperPersona.ListaPersona.FirstOrDefault();
                        nombrePersona = filaPersGuardada.Nombre;
                        idiomaPersona = filaPersGuardada.Idioma;
                        apellidosPersona = filaPersGuardada.Apellidos;
                    }
                    List<AD.EntityModel.Models.IdentidadDS.GrupoAmigos> filasGrupoAmigos = gestorIdentidades.GestorAmigos.AmigosDW.ListaGrupoAmigos.Where(item => item.Tipo == (int)TipoGrupoAmigos.AutomaticoOrganizacion && item.Automatico && item.IdentidadID.Equals(perfilOrg.IdentidadMyGNOSS.Clave)).ToList();

                    UtilIdiomas utilIdiomas = new UtilIdiomas("", null, persona.FilaPersona.Idioma, ProyectoSeleccionado.Clave, Guid.Empty, Guid.Empty, mLoggingService, mEntityContext, mConfigService);

                    gestorOrg.AgregarAdministradorDeOrganizacion(filaUsuario.UsuarioID, org.Clave, gestorUsuarios);

                    Perfil perfilPersOrg = null;

                    #region Vincular persona a organización o clase

                    if (!persona.UsuarioCargado)
                    {
                        DataWrapperUsuario usuDW = new DataWrapperUsuario();
                        usuDW.ListaUsuario.Add(usuarioCN.ObtenerUsuarioPorID(persona.UsuarioID));

                        if (usuDW.ListaUsuario.Count.Equals(1))
                        {
                            persona.GestorPersonas.GestorUsuarios.DataWrapperUsuario.Merge(usuDW);
                            persona.GestorPersonas.GestorUsuarios.RecargarUsuarios();
                        }
                    }

                    if (!pSolicitudesDeClase)
                    {
                        //OrganizacionDS.PersonaVinculoOrganizacionRow filaOrgPersona = gestorOrg.VincularPersonaOrganizacion(org, persona);
                        DatosTrabajoPersonaOrganizacion perfilPersonaOrganizacion = gestorOrg.VincularPersonaOrganizacion(org, persona);
                        PersonaVinculoOrganizacion filaOrgPersona = perfilPersonaOrganizacion.FilaVinculo;

                        filaOrgPersona.Cargo = filaSOrg.CargoContactoPrincipal;
                        filaOrgPersona.EmailTrabajo = filaSOrg.EmailContactoPrincipal;
                        emailPersona = filaSOrg.EmailContactoPrincipal;

                        new ControladorIdentidades(gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).CrearPerfilPersonaOrganizacion(persona, org, true, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, liveDS, recibirNewsletterDefectoProyectos);

                        perfilPersOrg = gestorIdentidades.ObtenerPerfilDePersonaEnOrganizacion(org.Clave, persona.Clave);
                        gestorUsuarios.AgregarUsuarioAProyecto(filaUsuario, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, perfilPersOrg.IdentidadMyGNOSS.Clave, false);

                        gestorIdentidades.RecargarHijos();

                        AD.EntityModel.Models.PersonaDS.DatosTrabajoPersonaLibre filaDatosTrabajoLibre = null;

                        if (persona.DatosTrabajoPersonaLibre != null)
                        {
                            filaDatosTrabajoLibre = persona.DatosTrabajoPersonaLibre;
                        }
                        ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        DataWrapperProyecto dataWrapperProyecto = null;

                        if (listaProyectos.Count > 0)
                        {
                            dataWrapperProyecto = proyectoCN.ObtenerProyectosPorIDsCargaLigera(listaProyectos);
                        }
                        else
                        {
                            dataWrapperProyecto = proyectoCN.ObtenerProyectosParticipaUsuarioConSuPerfilPersonal(pUsuarioAdminID);
                        }
                        proyectoCN.Dispose();

                        string pais = controladorPers.ObtenerNombrePaisPersona(persona);
                        string provincia = controladorPers.ObtenerNombreProvinciaPersona(persona);
                    }
                    else
                    {
                        foreach (Identidad identidad in gestorIdentidades.ListaIdentidades.Values)
                        {
                            if (identidad.Tipo == TiposIdentidad.Profesor)
                            {
                                perfilPersOrg = gestorIdentidades.ObtenerPerfilDePersonaEnOrganizacion(org.Clave, persona.Clave);

                                if (perfilPersOrg == null)
                                {
                                    gestorIdentidades.AgregarFilaPerfilPersonaOrganizacion(persona.Clave, org.Clave, identidad.FilaIdentidad.PerfilID);
                                    perfilPersOrg = gestorIdentidades.ObtenerPerfilDePersonaEnOrganizacion(org.Clave, persona.Clave);
                                }
                                break;
                            }
                        }
                    }
                    #endregion

                    gestorTesauro.AgregarTesauroOrganizacion(org.Clave, utilIdiomas.GetText("TESAURO", "RECURSOSPUBLICOS"), utilIdiomas.GetText("TESAURO", "RECURSOSPRIVADOS"));
                    gestorDoc.AgregarBRDeOrganizacion(org.Clave);

                    AD.EntityModel.Models.IdentidadDS.Perfil filaPerfilEnOrg = perfilPersOrg.FilaPerfil;

                    string asunto = "", mensaje = "";

                    asunto = utilIdiomas.GetText("ACEPTARINVITACION", "ASUNTOMENSAJEBIENVENIDA");
                    mensaje = utilIdiomas.GetText("ACEPTARINVITACION", "CUERPOMENSAJEBIENVENIDA");

                    gestorCorreoInterno.GestorNotificaciones = gestorNotificaciones;

                    List<Guid> listaDestinatarios = new List<Guid>();

                    listaDestinatarios.Add(perfilOrg.IdentidadMyGNOSS.Clave);
                    listaPerfilesEnvioCorreo.Add(perfilOrg.Clave);

                    // David: Si además de la organización, se ha registrado un usuario, se envia el mensaje a la bandeja de correo de este
                    if (filasSU.Count > 0 && perfil != null)
                    {
                        listaDestinatarios.Add(perfil.IdentidadMyGNOSS.Clave);
                        listaPerfilesEnvioCorreo.Add(perfil.Clave);
                    }


                    Guid correoID = Guid.Empty;
                    List<ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EcosistemaSinMetaProyecto.ToString())).ToList();
                    //bool ecosistemaSinMetaproyecto = ParametroAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinMetaProyecto.ToString() + "'").Length > 0 && bool.Parse((string)ParametroAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinMetaProyecto.ToString() + "'")[0]["Valor"]);
                    bool ecosistemaSinMetaproyecto = busqueda.Count > 0 && bool.Parse(busqueda.First().Valor);

                    if (!ecosistemaSinMetaproyecto)
                    {
                        correoID = gestorCorreoInterno.AgregarCorreo(Guid.Empty, listaDestinatarios, asunto, mensaje, this.BaseURL, TipoEnvioCorreoBienvenida.CorreoInterno, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                    }

                    if (correoID != Guid.Empty)
                    {
                        foreach (Guid destinatario in listaDestinatarios)
                        {
                            new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarMensajeFacModeloBaseSimple(correoID, Guid.Empty, ProyectoSeleccionado.Clave, "base", destinatario.ToString(), null, PrioridadBase.Alta);
                        }
                    }

                    #region Agregar como amigos de la organización al propio administrador

                    AmigosCN AmigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                    if (!AmigosCN.EsAmigoDeIdentidad(perfilOrg.IdentidadMyGNOSS.Clave, perfilPersOrg.IdentidadMyGNOSS.Clave))
                    {
                        gestorAmigos.CrearFilaAmigo(perfilOrg.IdentidadMyGNOSS.Clave, perfilPersOrg.IdentidadMyGNOSS.Clave);
                    }
                    AmigosCN.Dispose();

                    #endregion

                    #region Grupo de miembros de la clase/Organización

                    if (filasGrupoAmigos.Count > 0)
                    {
                        if (pSolicitudesDeClase)
                        {
                            filasGrupoAmigos[0].Nombre = utilIdiomas.GetText("CONTACTOS", "GRUPOMIEMBROSCLASE", filaOrg.Alias);
                        }
                        else
                        {
                            filasGrupoAmigos[0].Nombre = utilIdiomas.GetText("CONTACTOS", "GRUPOMIEMBROSORGANIZACION", filaOrg.Alias);
                        }

                        //Grupo automático de alumnos de una clase.
                        FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                        //Obtenemos los datos del DS porque aún la primera vez no se han cargado en la BD.
                        Guid profesorID = Guid.Empty;
                        List<AD.EntityModel.Models.IdentidadDS.Identidad> identidadRow = dataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(pPerfilProfesorID) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && identidad.Tipo == 4).ToList();
                        if (identidadRow.Count > 0)
                        {
                            Guid.TryParse(identidadRow[0].IdentidadID.ToString(), out profesorID);
                        }
                        else
                        {
                            //Si no lo encuentra en el DS, lo intentamos en la BD.
                            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            Guid? profID = identCN.ObtenerIdentidadProfesor(pPerfilProfesorID);
                            if (profID != null)
                            {
                                profesorID = profID.Value;
                            }
                            identCN.Dispose();
                        }


                        if (profesorID != null && profesorID != Guid.Empty)
                        {
                            facetadoCN.InsertarNuevoGrupoContactos(filasGrupoAmigos[0].IdentidadID.ToString(), filasGrupoAmigos[0].GrupoID.ToString(), filasGrupoAmigos[0].Nombre, profesorID.ToString());

                            //Limpiamos la caché de amigos de la identidad para que se muestre el grupo de clase una vez que se ha creado la clase.
                            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                            amigosCL.InvalidarAmigos(profesorID);
                            amigosCL.Dispose();
                        }
                        else
                        {
                            facetadoCN.InsertarNuevoGrupoContactos(filasGrupoAmigos[0].IdentidadID.ToString(), filasGrupoAmigos[0].GrupoID.ToString(), filasGrupoAmigos[0].Nombre, null);
                        }
                        facetadoCN.Dispose();

                    }


                    #endregion

                    SubirLogo(filaOrg, filaSOrg.SolicitudID);

                    if (subirFoto)
                    {
                        //Guardar foto en el servidor y en la BD
                        SubirFoto(persona.FilaPersona, filaSU.SolicitudID);
                    }

                }
                liveCN.ActualizarBD(liveDS);
                liveCN.Dispose();

                DataWrapperSuscripcion suscripcionDW = null;
                if (persona != null && persona.GestorPersonas != null && persona.GestorPersonas.GestorUsuarios != null && persona.GestorPersonas.GestorUsuarios.GestorIdentidades != null && persona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorSuscripciones != null && persona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorSuscripciones.SuscripcionDW.ListaSuscripcion.Count > 0)
                {
                    //lo hago así porque este dataset puede no rellenarse en ControladorIdentidades.CrearPerfilPersonaOrganizacion
                    suscripcionDW = persona.GestorPersonas.GestorUsuarios.GestorIdentidades.GestorSuscripciones.SuscripcionDW;
                }

                CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                mEntityContext.SaveChanges();
                correoCN.ActualizarCorreo(gestorCorreoInterno.CorreoDS);

                //ControladorCorreo.AgregarNotificacionCorreoNuevoAPerfiles(listaPerfilesEnvioCorreo);

                Elementos.ServiciosGenerales.Proyecto proyPrivClase = null;

                if (!string.IsNullOrEmpty(pNombreComunidad))
                {
                    try
                    {
                        ControladorProyecto controladorProyectos = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                        DataWrapperOrganizacion orgProyDW = null;
                        DataWrapperProyecto proyDeClaseDS = null;
                        GestorParametroGeneral paramGeneralDS = null;
                        DataWrapperTesauro tesDW = null;
                        DataWrapperDocumentacion documentoDW = null;
                        DataWrapperUsuario usuDW = null;
                        DataWrapperIdentidad identDS = null;

                        proyPrivClase = controladorProyectos.CrearNuevoProyecto(pNombreComunidad, pNombreCortoComunidad, pNombreComunidad, pTagsComunidad.ToArray(), (short)TipoAcceso.Privado, tipoProyecto, pUsuarioAdminID, pPerfilProfesorID, ProyectoAD.MetaOrganizacion, Guid.Empty, false, true, true, true, false, null, out orgProyDW, out proyDeClaseDS, out paramGeneralDS, out tesDW, out documentoDW, out usuDW, out identDS);
                        proyPrivClase.Estado = (short)EstadoProyecto.Abierto;

                        proyPrivClase.Tags = UtilCadenas.CadenaFormatoTexto(pTagsComunidad);

                        string[] categorias = pCategoriasComunidad.Split(',');

                        //nombre, categorias, tags
                        //string tagsModeloBase = UtilCadenas.CadenaFormatoTexto(pTagsComunidad);
                        //tagsModeloBase += "," + BaseProyectosAD.NOMBRE_PROY + pNombreComunidad + BaseProyectosAD.NOMBRE_PROY;

                        proyPrivClase.GestorProyectos.GestionTesauro = new GestionTesauro(tesDW, mLoggingService, mEntityContext);

                        foreach (string categoria in categorias)
                        {
                            proyPrivClase.GestorProyectos.GestionTesauro.AgregarCategoria(categoria.Trim());
                        }

                        foreach (CategoriaTesauro catTesauro in pCategoriasMyGnoss)
                        {
                            ProyectoAgCatTesauro proyectoAgCatTesauro = new ProyectoAgCatTesauro();
                            proyectoAgCatTesauro.OrganizacionID = proyPrivClase.FilaProyecto.OrganizacionID;
                            proyectoAgCatTesauro.ProyectoID = proyPrivClase.FilaProyecto.ProyectoID;
                            proyectoAgCatTesauro.TesauroID = catTesauro.FilaCategoria.TesauroID;
                            proyectoAgCatTesauro.CategoriaTesauroID = catTesauro.Clave;
                            proyPrivClase.GestorProyectos.DataWrapperProyectos.ListaProyectoAgCatTesauro.Add(proyectoAgCatTesauro);
                            if (mEntityContext.ProyectoAgCatTesauro.FirstOrDefault(proy => proy.OrganizacionID.Equals(proyectoAgCatTesauro.OrganizacionID) && proy.ProyectoID.Equals(proyectoAgCatTesauro.ProyectoID) && proy.TesauroID.Equals(proyectoAgCatTesauro.TesauroID) && proy.CategoriaTesauroID.Equals(proyectoAgCatTesauro.CategoriaTesauroID)) == null)
                            {
                                mEntityContext.ProyectoAgCatTesauro.Add(proyectoAgCatTesauro);
                            }


                            //tagsModeloBase += "," + BaseProyectosAD.CAT_PROY + catTesauro.Nombre + BaseProyectosAD.CAT_PROY;
                        }

                        foreach (short orden in pProyectosRel.Keys)
                        {
                            ProyectoRelacionado proyectoRelacionado = new ProyectoRelacionado();
                            proyectoRelacionado.OrganizacionID = proyPrivClase.FilaProyecto.OrganizacionID;
                            proyectoRelacionado.ProyectoID = proyPrivClase.FilaProyecto.ProyectoID;
                            proyectoRelacionado.OrganizacionRelacionadaID = pProyectosRel[orden].FilaProyecto.OrganizacionID;
                            proyectoRelacionado.ProyectoRelacionadoID = pProyectosRel[orden].FilaProyecto.ProyectoID;
                            proyectoRelacionado.Orden = orden;
                            proyPrivClase.GestorProyectos.DataWrapperProyectos.ListaProyectoRelacionado.Add(proyectoRelacionado);
                        }
                        mEntityContext.SaveChanges();
                    }
                    catch (Exception) { }
                }
                if (listaOrganizacions.Count > 0)
                {
                    IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestionIdentidades gestorIdent = new GestionIdentidades(idenCN.ObtenerIdentidadesDeOrganizaciones(listaOrganizacions, ProyectoAD.MyGnoss), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    foreach (Identidad iden in gestorIdent.ListaIdentidades.Values)
                    {
                        AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = gestorIdent.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(iden.PerfilID));
                        AD.EntityModel.Models.IdentidadDS.PerfilOrganizacion filaPerfilOrg = gestorIdent.DataWrapperIdentidad.ListaPerfilOrganizacion.FirstOrDefault(perfilOrg => perfilOrg.PerfilID.Equals(iden.PerfilID));

                        if (proyPrivClase != null)
                        {
                            int solicitudDeClase = 0;
                            if (pSolicitudesDeClase)
                            {
                                solicitudDeClase = 1;
                            }
                            new ControladorIdentidades(gestorIdent, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).RegistrarOrganizacionEnProyecto(filaPerfil, filaPerfilOrg, proyPrivClase, solicitudDeClase);

                            //Actualizo el modelo base:
                            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                            controladorPersonas.ActualizarModeloBASE(proyPrivClase.IdentidadCreadoraProyecto, proyPrivClase.Clave, true, true, PrioridadBase.Alta);

                            ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                            controladorProyecto.ActualizarModeloBase(proyPrivClase.Clave, PrioridadBase.Alta);
                        }

                        if (pListaProyectosMiembro.Count > 0)
                        {
                            ProyectoCN proyecCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            GestionProyecto gestorProy = new GestionProyecto(proyecCN.ObtenerProyectosPorID(pListaProyectosMiembro), mLoggingService, mEntityContext);
                            proyecCN.Dispose();
                            foreach (Guid proyecto in pListaProyectosMiembro)
                            {
                                int solicitudDeClase = 0;
                                if (pSolicitudesDeClase)
                                {
                                    solicitudDeClase = 1;
                                }
                                new ControladorIdentidades(gestorIdent, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).RegistrarOrganizacionEnProyecto(filaPerfil, filaPerfilOrg, gestorProy.ListaProyectos[proyecto], solicitudDeClase);
                            }
                        }
                    }
                }

            }
            return filaOrg.OrganizacionID;
        }

        /// <summary>
        /// Acepta una solicitud de nuevo usuario.
        /// </summary>
        /// <param name="pSolicitudDW">DataSet de solicitud</param>
        /// <param name="pPeticionDW">DataSet de petición</param>
        /// <param name="pSolicitudID"></param>
        /// <returns>Identidad creada en el proceso</returns>
        public Identidad AceptarSolicitudNuevoUsuario(DataWrapperSolicitud pSolicitudDW, DataWrapperPeticion pPeticionDW, Guid pSolicitudID, Dictionary<string, string> pParametroProyecto)
        {
            Solicitud fila = pSolicitudDW.ListaSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).FirstOrDefault();

            List<SolicitudNuevoUsuario> filasNuevoUsuario = pSolicitudDW.ListaSolicitudNuevoUsuario.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            Persona per = new Persona(mLoggingService);
            Guid identidadId = new Guid();
            Guid identidadProfesorId = new Guid();

            GestionOrganizaciones gestorOrganizaciones = null;
            DataWrapperUsuario dataWrapperUsuario = null;
            GestionPersonas gestorPersonas = null;
            GestionUsuarios gestorUsuarios = null;
            GestionNotificaciones gestorNotificaciones = null;
            GestionCorreo gestorCorreoInterno = null;
            GestionPeticiones gestorPeticiones = null;

            if (pPeticionDW != null)
            {
                gestorPeticiones = new GestionPeticiones(pPeticionDW, mLoggingService, mEntityContext);
            }
            GestionIdentidades gestorIdentidades = null;
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();
            DataWrapperAmigos amigosDW = new DataWrapperAmigos();

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyectoCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();
            proyectoCN.Dispose();

            if (filasNuevoUsuario.Count > 0)
            {
                #region Guardamos el usuario nuevo
                dataWrapperUsuario = new DataWrapperUsuario();
                SolicitudNuevoUsuario filaNuevoUsuario = filasNuevoUsuario[0];

                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = usuarioCN.ObtenerUsuarioPorID(filaNuevoUsuario.UsuarioID);
                dataWrapperUsuario.ListaUsuario.Add(filaUsuario);
                filaUsuario.EstaBloqueado = false;

                GeneralCN generalCN = new GeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DateTime fechaHoy = generalCN.HoraServidor;

                fila.Estado = (short)EstadoSolicitud.Aceptada;
                fila.FechaProcesado = fechaHoy;

                //Persona
                dataWrapperPersona = new DataWrapperPersona();
                gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
                Persona persona = gestorPersonas.AgregarPersona();
                gestorPersonas.CrearDatosTrabajoPersonaLibre(persona);

                AD.EntityModel.Models.PersonaDS.Persona filaPersona = persona.FilaPersona;
                filaPersona.UsuarioID = filaNuevoUsuario.UsuarioID;
                filaPersona.Apellidos = filaNuevoUsuario.Apellidos;
                filaPersona.CPPersonal = filaNuevoUsuario.CP;
                filaPersona.Email = filaNuevoUsuario.Email;
                filaPersona.EsBuscable = filaNuevoUsuario.EsBuscable;
                filaPersona.EsBuscableExternos = filaNuevoUsuario.EsBuscableExterno;
                filaPersona.FechaNacimiento = filaNuevoUsuario.FechaNacimiento;
                filaPersona.LocalidadPersonal = filaNuevoUsuario.Poblacion;
                filaPersona.Nombre = filaNuevoUsuario.Nombre;
                filaPersona.PaisPersonalID = filaNuevoUsuario.PaisID;
                filaPersona.Idioma = filaNuevoUsuario.Idioma;
                filaPersona.EstadoCorreccion = (short)EstadoCorreccion.NoCorreccion;

                if (!filaNuevoUsuario.ProvinciaID.HasValue)
                {
                    filaPersona.ProvinciaPersonal = filaNuevoUsuario.Provincia;
                }
                else
                {
                    filaPersona.ProvinciaPersonalID = filaNuevoUsuario.ProvinciaID;
                }
                filaPersona.Sexo = filaNuevoUsuario.Sexo;
                mEntityContext.Persona.Add(filaPersona);
                dataWrapperPersona.ListaPersona.Add(filaPersona);
                AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona filaConfigPers = gestorPersonas.AgregarConfiguracionGnossPersona(filaPersona.PersonaID);

                gestorIdentidades = new GestionIdentidades(identidadDW, gestorPersonas, new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);
                gestorPersonas.GestorUsuarios = gestorUsuarios;
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                bool existeFAQ = proyCN.ExisteProyectoFAQ();
                bool existeNoticias = proyCN.ExisteProyectoNoticias();
                bool existeDidactalia = proyCN.ExisteProyectoDidactalia();
                proyCN.Dispose();

                //bool IncluirDidactalia = ControladorProyecto.ProyectoDebeRegistrarEnDidactalia(filaNuevoUsuario.SolicitudRow.ProyectoID);
                Perfil perfilPersona = null;
                Identidad objetoIdentidad = null;
                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = null;


                if (PerfilPersonalDisponible)
                {
                    perfilPersona = gestorIdentidades.AgregarPerfilPersonal(filaPersona, true, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, recibirNewsletterDefectoProyectos);
                    objetoIdentidad = (Identidad)perfilPersona.Hijos[0];
                    filaIdentidad = objetoIdentidad.FilaIdentidad;
                }

                List<Guid> listaProyectosParticipaUsuario = new List<Guid>();

                //Guardar foto en el servidor y en la BD
                GuardarFotoPersona(filaPersona, filaNuevoUsuario.SolicitudID);

                AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrganizacion = null;
                gestorOrganizaciones = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);

                if (PerfilPersonalDisponible)
                {
                    gestorUsuarios.AgregarUsuarioAProyecto(filaUsuario, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, filaIdentidad.IdentidadID);
                    ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                    controladorPersonas.ActualizarModeloBASE(objetoIdentidad.IdentidadMyGNOSS, ProyectoAD.MyGnoss, true, false, PrioridadBase.Alta);


                    gestorIdentidades.RecargarHijos();

                    Guid organizacionRegistroUsuario = filaNuevoUsuario.Solicitud.OrganizacionID;
                    Guid proyectoRegistroUsuario = filaNuevoUsuario.Solicitud.ProyectoID;

                    new ControladorDeSolicitudes(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).RegistrarUsuarioEnProyectosObligatorios(organizacionRegistroUsuario, proyectoRegistroUsuario, filaPersona.PersonaID, perfilPersona, filaUsuario, gestorUsuarios, gestorIdentidades);
                    gestorIdentidades.RecargarHijos();

                    if (!filaNuevoUsuario.Solicitud.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !filaNuevoUsuario.Solicitud.ProyectoID.Equals(ProyectoAD.ProyectoFAQ) && !filaNuevoUsuario.Solicitud.ProyectoID.Equals(ProyectoAD.ProyectoNoticias) && !filaNuevoUsuario.Solicitud.ProyectoID.Equals(ProyectoAD.ProyectoDidactalia))
                    {
                        Guid organizacionID = filaNuevoUsuario.Solicitud.OrganizacionID;
                        Guid proyectoID = filaNuevoUsuario.Solicitud.ProyectoID;
                        Identidad ObjetoIdentidadProy = new ControladorIdentidades(gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarIdentidadPerfilYUsuarioAProyecto(gestorIdentidades, gestorUsuarios, organizacionID, proyectoID, filaUsuario, perfilPersona, recibirNewsletterDefectoProyectos);
                        gestorIdentidades.RecargarHijos();
                        listaProyectosParticipaUsuario.Add(proyectoID);
                    }
                }
                else
                {
                    gestorUsuarios.AgregarProyectoRolUsuario(filaUsuario.UsuarioID, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto);
                }
                PeticionCN peticionCN = new PeticionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (pPeticionDW == null)
                {
                    gestorPeticiones = new GestionPeticiones(peticionCN.ObtenerPeticionInvitacionesOrganizacionesAceptadasUsuario(filaUsuario.UsuarioID), mLoggingService, mEntityContext);
                }

                if (gestorPeticiones.ListaPeticiones.Count > 0)
                {
                    //El usuario tiene alguna invitación de organización aceptada, hay que hacerle miembro de esas organizaciones
                    foreach (PeticionInvOrganizacion peticion in gestorPeticiones.ListaPeticiones.Values)
                    {
                        AceptarInvitacionOrganizacion(peticion, gestorOrganizaciones, gestorIdentidades, gestorUsuarios, gestorPersonas);
                    }
                }

                if (pPeticionDW == null)
                {
                    gestorPeticiones.Dispose();
                    gestorPeticiones = new GestionPeticiones(peticionCN.ObtenerPeticionInvitacionesComunidadesAceptadasUsuario(filaUsuario.UsuarioID), mLoggingService, mEntityContext);
                }

                if (gestorPeticiones.ListaPeticiones.Count > 0)
                {
                    //El usuario tiene alguna invitación de comunidad aceptada, hay que hacerle miembro de esas comunidades, y si hay dafo le hacemos miembro del dafo
                    foreach (Peticion peticion in gestorPeticiones.ListaPeticiones.Values)
                    {
                        if (peticion is PeticionInvComunidad)
                        {
                            PeticionInvComunidad petCom = (PeticionInvComunidad)peticion;
                            AceptarInvitacionComunidad(petCom, gestorUsuarios, filaUsuario, gestorIdentidades, objetoIdentidad.PerfilUsuario);

                            listaProyectosParticipaUsuario.Add(petCom.FilaInvitacionComunidad.ProyectoID);
                        }
                    }
                }

                gestorUsuarios.GestorTesauro = new GestionTesauro(new DataWrapperTesauro(), mLoggingService, mEntityContext);
                gestorUsuarios.GestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext);
                gestorUsuarios.CompletarUsuarioNuevo(filaUsuario, UtilIdiomas.GetText("TESAURO", "RECURSOSPUBLICOS"), UtilIdiomas.GetText("TESAURO", "RECURSOSPRIVADOS"));

                if ((gestorIdentidades.GestorAmigos != null) && (gestorIdentidades.GestorAmigos.AmigosDW != null))
                {
                    amigosDW = gestorIdentidades.GestorAmigos.AmigosDW;
                }
                identidadId = filaIdentidad.IdentidadID;
                per = persona;
                if (filaPersona.FechaNacimiento.HasValue)
                {
                    if (ComprobarMenorDe16Anios(filaPersona.FechaNacimiento.Value))
                    {
                        //Es menor de 16 años, elimino su perfil persona para que solo pueda acceder con su perfil de organización
                        AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad filaProyIdent = gestorUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.FirstOrDefault(item => item.IdentidadID.Equals(filaIdentidad.IdentidadID));

                        gestorUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Remove(filaProyIdent);
                        mEntityContext.Entry(filaProyIdent);

                        foreach (AD.EntityModel.Models.IdentidadDS.Identidad identidadEliminar in objetoIdentidad.PerfilUsuario.FilaPerfil.Identidad)
                        {
                            identidadEliminar.FechaBaja = DateTime.Now;
                        }
                        objetoIdentidad.PerfilUsuario.FilaPerfil.Eliminado = true;
                    }
                }


                #endregion
            }

            if (filasNuevoUsuario.Count > 0)
            {
                #region Generamos el correo interno para el nuevo usuario

                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                string asunto = "", mensaje = "";
                gestorCorreoInterno = new GestionCorreo(new CorreoDS(), gestorPersonas, gestorIdentidades, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                List<Guid> listaDestinatarios = new List<Guid>();
                listaDestinatarios.Add(identidadId);

                asunto = UtilIdiomas.GetText("ACEPTARINVITACION", "ASUNTOMENSAJEBIENVENIDA");
                mensaje = UtilIdiomas.GetText("ACEPTARINVITACION", "CUERPOMENSAJEBIENVENIDA");

                Guid correoID = Guid.Empty;

                TipoEnvioCorreoBienvenida tipoEnvioCorreo = gestorCorreoInterno.ObtenerTipoEnvioCorreoBienvenida(EsEcosistemaSinMetaProyecto, pParametroProyecto);

                if (!tipoEnvioCorreo.Equals(TipoEnvioCorreoBienvenida.Ninguno))
                {
                    correoID = gestorCorreoInterno.AgregarCorreo(Guid.Empty, listaDestinatarios, asunto, mensaje, this.BaseURL, tipoEnvioCorreo, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                }

                if (correoID != Guid.Empty)
                {
                    foreach (Guid destinatario in listaDestinatarios)
                    {
                        new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarMensajeFacModeloBaseSimple(correoID, Guid.Empty, ProyectoSeleccionado.Clave, "base", destinatario.ToString(), null, PrioridadBase.Alta);
                    }
                }

                #endregion

                DataWrapperSuscripcion suscripcionDW = null;
                if (gestorIdentidades.GestorSuscripciones != null && gestorIdentidades.GestorSuscripciones.SuscripcionDW.ListaSuscripcion.Count > 0)
                {
                    suscripcionDW = gestorIdentidades.GestorSuscripciones.SuscripcionDW;
                }

                mEntityContext.SaveChanges();
                //ControladorCorreo.AgregarNotificacionCorreoNuevoAIdentidades(listaDestinatarios);
                //ControladorCorreo.AgregarNotificacionCorreoNuevoAIdentidades(listaDestinatarios2);

                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Identidad identidadnuevo = new GestionIdentidades(idenCN.ObtenerIdentidadPorID(identidadId, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication).ListaIdentidades[identidadId];

                //Actualizo el modelo base:
                ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                controladorPersonas.ActualizarModeloBASE(identidadnuevo, ProyectoAD.MyGnoss, true, false, null, false, PrioridadBase.Alta);
            }
            else
            {
                #region Generamos el correo interno para el nuevo profesor

                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                string asunto = "", mensaje = "";

                asunto = UtilIdiomas.GetText("ACEPTARINVITACION", "ASUNTOMENSAJEBIENVENIDAPROF");

                gestorCorreoInterno = new GestionCorreo(new CorreoDS(), gestorPersonas, gestorIdentidades, null, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<Guid> listaDestinatarios = new List<Guid>();
                listaDestinatarios.Add(identidadProfesorId);

                Guid correoID = Guid.Empty;

                if (!EsEcosistemaSinMetaProyecto)
                {
                    correoID = gestorCorreoInterno.AgregarCorreo(Guid.Empty, listaDestinatarios, asunto, mensaje, this.BaseURL, ProyectoSeleccionado, TiposNotificacion.AvisoCorreoNuevoContacto, UtilIdiomas.LanguageCode);
                }

                if (correoID != Guid.Empty)
                {
                    foreach (Guid destinatario in listaDestinatarios)
                    {
                        new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarMensajeFacModeloBaseSimple(correoID, Guid.Empty, ProyectoSeleccionado.Clave, "base", destinatario.ToString(), null, PrioridadBase.Alta);
                    }
                }

                #endregion

                mEntityContext.SaveChanges();
                identidadId = identidadProfesorId;

                //ControladorCorreo.AgregarNotificacionCorreoNuevoAIdentidades(listaDestinatarios);
            }
            return gestorIdentidades.ListaIdentidades[identidadId]; ;
        }

        /// <summary>
        /// Acepta la invitación a participar en una comunidad
        /// </summary>
        /// <param name="pPeticion">Peticion</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        /// <param name="pFilaUsuario">Fila de usuario</param>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        /// <param name="pPerfilPublico">Perfil de MyGnoss del usuario que acepta la petición</param>
        /// <param name="pDafoProyectoDS">DataSet de Dafo</param>
        private void AceptarInvitacionComunidad(PeticionInvComunidad pPeticion, GestionUsuarios pGestorUsuarios, AD.EntityModel.Models.UsuarioDS.Usuario pFilaUsuario, GestionIdentidades pGestorIdentidades, Perfil pPerfilPublico)
        {
            pPeticion.FilaPeticion.FechaProcesado = DateTime.Now;
            pPeticion.FilaPeticion.Estado = (short)EstadoPeticion.Aceptada;
            pPeticion.FilaPeticion.UsuarioID = pFilaUsuario.UsuarioID;

            Guid identidadID = Guid.Empty;

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();
            proyCN.Dispose();

            Identidad ObjetoIdentidadProy = new ControladorIdentidades(pGestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarIdentidadPerfilYUsuarioAProyecto(pGestorIdentidades, pGestorUsuarios, pPeticion.FilaInvitacionComunidad.OrganizacionID, pPeticion.FilaInvitacionComunidad.ProyectoID, pFilaUsuario, pPerfilPublico, recibirNewsletterDefectoProyectos);
            identidadID = ObjetoIdentidadProy.Clave;

            //Actualizo el modelo Base:
            ControladorPersonas controladorPersonas = new ControladorPersonas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            controladorPersonas.ActualizarModeloBASE(ObjetoIdentidadProy, pPeticion.FilaInvitacionComunidad.ProyectoID, true, true, PrioridadBase.Alta);

            new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).ActualizarGnossLive(pPeticion.FilaInvitacionComunidad.ProyectoID, pPerfilPublico.Clave, AccionLive.Agregado, (int)TipoLive.Miembro, false, PrioridadLive.Alta);

            //Invalidamos la cache de Mis comunidades
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarMisProyectos(ObjetoIdentidadProy.PerfilID);
            proyCL.Dispose();

            pGestorIdentidades.RecargarHijos();
        }

        /// <summary>
        /// Acepta la invitación a participar con una organización
        /// </summary>
        /// <param name="pPeticion">Petición</param>
        /// <param name="pGestorOrganizaciones">Gestor de organizaciones</param>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        /// <param name="pGestorPersonas">Gestor de personas</param>
        private void AceptarInvitacionOrganizacion(PeticionInvOrganizacion pPeticion, GestionOrganizaciones pGestorOrganizaciones, GestionIdentidades pGestorIdentidades, GestionUsuarios pGestorUsuarios, GestionPersonas pGestorPersonas)
        {
            //Acepto la petición
            pPeticion.FilaPeticion.FechaProcesado = DateTime.Now;
            pPeticion.FilaPeticion.Estado = (short)EstadoPeticion.Aceptada;

            UsuarioGnoss usuario = pGestorUsuarios.ListaUsuarios[pGestorUsuarios.ListaUsuarios.Keys[0]];

            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            pGestorOrganizaciones.OrganizacionDW.Merge(orgCN.ObtenerOrganizacionPorID(pPeticion.FilaInvitacionOrganizacion.OrganizacionID));
            pGestorOrganizaciones.CargarOrganizaciones();
            Elementos.ServiciosGenerales.Organizacion org = (Elementos.ServiciosGenerales.Organizacion)pGestorOrganizaciones.ListaOrganizaciones[pPeticion.FilaInvitacionOrganizacion.OrganizacionID];

            pGestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(pGestorOrganizaciones.OrganizacionDW);
            pGestorIdentidades.GestorOrganizaciones.CargarOrganizaciones();

            Persona persona = pGestorPersonas.ListaPersonas[pGestorPersonas.ListaPersonas.Keys[0]];

            if (pGestorIdentidades.GestorUsuarios == null)
            {
                pGestorIdentidades.GestorUsuarios = pGestorUsuarios;
            }

            if (persona.Usuario == null)
            {
                persona.GestorPersonas.GestorUsuarios = pGestorUsuarios;
            }
            persona.UsuarioCargado = true;
            DatosTrabajoPersonaOrganizacion perfilPersonaOrganizacion = pGestorOrganizaciones.VincularPersonaOrganizacion(org, persona);
            PersonaVinculoOrganizacion filaOrgPersona = perfilPersonaOrganizacion.FilaVinculo;
            filaOrgPersona.Cargo = pPeticion.FilaInvitacionOrganizacion.Cargo;
            filaOrgPersona.EmailTrabajo = pPeticion.FilaInvitacionOrganizacion.Email;

            //Creo el perfil persona+organizacion
            LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            LiveDS liveDS = new LiveDS();
            IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            pGestorIdentidades.DataWrapperIdentidad.Merge(idenCN.ObtenerIdentidadesDeOrganizacion(org.Clave, ProyectoAD.MetaProyecto));
            idenCN.Dispose();
            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            pGestorIdentidades.GestorAmigos = new GestionAmigos(new DataWrapperAmigos(), pGestorIdentidades, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            amigosCN.Dispose();

            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyectoCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();
            proyectoCN.Dispose();

            Perfil perfil = new ControladorIdentidades(pGestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarPerfilPersonaOrganizacion(pGestorIdentidades, pGestorIdentidades.GestorOrganizaciones, pGestorIdentidades.GestorUsuarios, persona, org, true, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, liveDS, recibirNewsletterDefectoProyectos);

            liveCN.ActualizarBD(liveDS);
            liveCN.Dispose();

            pGestorUsuarios.AgregarUsuarioAProyecto(usuario.FilaUsuario, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, perfil.IdentidadMyGNOSS.Clave, false);

            pGestorIdentidades.RecargarHijos();

            pGestorUsuarios.AgregarOrganizacionRolUsuario(usuario.Clave, org.Clave);

            AgregarContactosOrganizacion(pGestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.FirstOrDefault(perfilPersonaOrg => perfilPersonaOrg.PersonaID.Equals(persona.Clave) && perfilPersonaOrg.Equals(org.Clave)), pGestorIdentidades);
        }

        /// <summary>
        /// Agrega a la organización el usuario actual como contacto
        /// </summary>
        /// <param name="filaPerOrg"></param>
        /// <param name="pGestorIdentidades"></param>
        private void AgregarContactosOrganizacion(AD.EntityModel.Models.IdentidadDS.PerfilPersonaOrg filaPerOrg, GestionIdentidades pGestorIdentidades)
        {
            ControladorAmigos controlador = new ControladorAmigos(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            if (filaPerOrg != null)
            {
                controlador.AgregarContactosOrganizacion(filaPerOrg.OrganizacionID, filaPerOrg.Perfil.Identidad.First().IdentidadID, pGestorIdentidades);
            }

        }

        /// <summary>
        /// Comprueba si la fecha de nacimiento corresponde a un menor de edad
        /// </summary>
        /// <param name="pFechaNacimiento">Fecha de nacimiento de la persona</param>
        /// <returns></returns>
        private bool ComprobarMenorDe16Anios(DateTime pFechaNacimiento)
        {
            DateTime fechaActual = DateTime.Now.AddYears(-16);
            if (pFechaNacimiento.Year > fechaActual.Year)
            {
                return true;
            }
            else if (pFechaNacimiento.Year == fechaActual.Year)
            {
                if (pFechaNacimiento.Month > fechaActual.Month)
                {
                    return true;
                }
                else if (pFechaNacimiento.Month == fechaActual.Month)
                {
                    if (pFechaNacimiento.Day > fechaActual.Day)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Guarda la foto del perfil
        /// </summary>
        /// <param name="pFila">Fila de la persona o de la solicitud</param>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        private void GuardarFotoPersona(AD.EntityModel.Models.PersonaDS.Persona pFila, Guid pSolicitudID)
        {
            Stopwatch sw = null;
            try
            {
                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                string url = UrlIntragnossServicios.Replace("https://", "http://");
                servicioImagenes.Url = url;
                byte[] resultado = servicioImagenes.ObtenerImagen(UtilArchivos.ContentImagenesSolicitudes + "/" + pSolicitudID.ToString(), ".png");

                if (resultado != null)
                {
                    if (pFila is AD.EntityModel.Models.PersonaDS.Persona)
                    {
                        pFila.Foto = resultado;
                    }

                    sw = LoggingService.IniciarRelojTelemetria();
                    servicioImagenes.AgregarImagen(resultado, UtilArchivos.ContentImagenesPersonas + "/" + pFila.PersonaID.ToString(), ".png");
                    mLoggingService.AgregarEntradaDependencia("Agregar imagen desde servicio imagenes", false, "GuardarFotoPersona", sw, true);

                    sw.Restart();
                    servicioImagenes.BorrarImagen(UtilArchivos.ContentImagenesSolicitudes + "/" + pSolicitudID.ToString() + ".png");
                    mLoggingService.AgregarEntradaDependencia("Borrar imagen desde servicio imagenes", false, "GuardarFotoPersona", sw, true);

                    SixLabors.ImageSharp.Image image = UtilImages.ConvertirArrayBytesEnImagen(resultado);

                    //Ajusto su tamaño
                    SixLabors.ImageSharp.SizeF tamanioProporcional = UtilImages.CalcularTamanioProporcionado(image, 54, 54);
                    image = UtilImages.AjustarImagen(image, tamanioProporcional.Width, tamanioProporcional.Height);

                    //Guardo la imagen en un archivo temporal
                    MemoryStream ms = new MemoryStream();
                    image.SaveAsPng(ms);

                    servicioImagenes.AgregarImagen(ms.ToArray(), UtilArchivos.ContentImagenesPersonas + "/" + pFila.PersonaID.ToString() + "_peque", ".png");
                }
            }
            catch (Exception)
            {
                mLoggingService.AgregarEntradaDependencia("Error guardar la foto del perfil", false, "GuardarFotoPersona", sw, true);
                throw;
            }
        }


        /// <summary>
        /// Guarda la foto de la organización
        /// </summary>
        /// <param name="pFila">Fila de organización</param>
        /// <param name="pSolicitudID">Identificador de solicitud</param>
        private void SubirLogo(AD.EntityModel.Models.OrganizacionDS.Organizacion pFila, Guid pSolicitudID)
        {
            Stopwatch sw = null;
            try
            {
                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);

                string url = UrlIntragnossServicios.Replace("https://", "http://");
                servicioImagenes.Url = url;

                if (pFila.Logotipo != null)
                {
                    sw = LoggingService.IniciarRelojTelemetria();
                    servicioImagenes.AgregarImagen(pFila.Logotipo, UtilArchivos.ContentImagenesOrganizaciones + "/" + pFila.OrganizacionID.ToString(), ".png");
                    mLoggingService.AgregarEntradaDependencia("Agregar imagen desde servicio imagenes", false, "GuardarFotoPersona", sw, true);

                    sw.Restart();
                    servicioImagenes.BorrarImagen(UtilArchivos.ContentImagenesSolicitudes + "/" + pSolicitudID.ToString() + ".png");
                    mLoggingService.AgregarEntradaDependencia("Borrar imagen desde servicio imagenes", false, "GuardarFotoPersona", sw, true);

                    SixLabors.ImageSharp.Image image = UtilImages.ConvertirArrayBytesEnImagen(pFila.Logotipo);

                    //Ajusto su tamaño
                    SixLabors.ImageSharp.SizeF tamanioProporcional = UtilImages.CalcularTamanioProporcionado(image, 54, 54);
                    image = UtilImages.AjustarImagen(image, tamanioProporcional.Width, tamanioProporcional.Height);

                    //Guardo la imagen en un archivo temporal
                    MemoryStream ms = new MemoryStream();
                    image.SaveAsPng(ms);
                    servicioImagenes.AgregarImagen(ms.ToArray(), UtilArchivos.ContentImagenesOrganizaciones + "/" + pFila.OrganizacionID.ToString() + "_peque", ".png");
                }
            }
            catch (Exception)
            {
                mLoggingService.AgregarEntradaDependencia("Error guardar la foto de la organizacion", false, "SubirLogo", sw, true);
                throw;
            }
        }


        /// <summary>
        /// Guarda la foto de la organización
        /// </summary>
        /// <param name="pFila">Fila de organizacion</param>
        /// <param name="pSolicitudID">Identificador de solicitud</param>
        private void GuardarLogo(AD.EntityModel.Models.OrganizacionDS.Organizacion pFila, Guid pSolicitudID)
        {
            Stopwatch sw = null;
            try
            {
                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                sw = LoggingService.IniciarRelojTelemetria();

                string url = UrlIntragnossServicios.Replace("https://", "http://");
                servicioImagenes.Url = url;
                byte[] resultado = servicioImagenes.ObtenerImagen(UtilArchivos.ContentImagenesSolicitudes + "/" + pSolicitudID.ToString(), ".png");
                mLoggingService.AgregarEntradaDependencia("Obtener imagen desde servicio imagenes", false, "GuardarLogo", sw, true);
                if (resultado != null)
                {
                    pFila.Logotipo = resultado;
                }
            }
            catch (Exception)
            {
                mLoggingService.AgregarEntradaDependencia("Error obtener imagen desde servicio imagenes", false, "GuardarLogo", sw, false);
                throw;
            }
        }

        /// <summary>
        /// Guarda la foto del perfil
        /// </summary>
        /// <param name="pFila">Fila de persona</param>
        /// <param name="pSolicitudID">Identificador de solicitud</param>
        private void SubirFoto(AD.EntityModel.Models.PersonaDS.Persona pFila, Guid pSolicitudID)
        {
            Stopwatch sw = null;
            try
            {
                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                string url = UrlIntragnossServicios.Replace("https://", "http://");
                servicioImagenes.Url = url;

                if (pFila.Foto != null)
                {
                    sw = LoggingService.IniciarRelojTelemetria();
                    servicioImagenes.AgregarImagen(pFila.Foto, UtilArchivos.ContentImagenesPersonas + "/" + pFila.PersonaID.ToString(), ".png");
                    mLoggingService.AgregarEntradaDependencia("Agregar imagen desde servicio imagenes", false, "SubirFoto", sw, true);

                    sw.Restart();
                    servicioImagenes.BorrarImagen(UtilArchivos.ContentImagenesSolicitudes + "/" + pSolicitudID.ToString() + ".png");
                    mLoggingService.AgregarEntradaDependencia("Borrar imagen desde servicio imagenes", false, "SubirFoto", sw, true);

                    SixLabors.ImageSharp.Image image = UtilImages.ConvertirArrayBytesEnImagen(pFila.Foto);

                    //Ajusto su tamaño
                    SixLabors.ImageSharp.SizeF tamanioProporcional = UtilImages.CalcularTamanioProporcionado(image, 54, 54);
                    image = UtilImages.AjustarImagen(image, tamanioProporcional.Width, tamanioProporcional.Height);

                    //Guardo la imagen en un archivo temporal
                    MemoryStream ms = new MemoryStream();
                    image.SaveAsPng(ms);
                    servicioImagenes.AgregarImagen(ms.ToArray(), UtilArchivos.ContentImagenesPersonas + "/" + pFila.PersonaID.ToString() + "_peque", ".png");
                }
            }
            catch (Exception)
            {
                mLoggingService.AgregarEntradaDependencia("Error guardar la foto del perfil", false, "SubirFoto", sw, false);
                throw;
            }
        }

        /// <summary>
        /// Guarda la foto del perfil
        /// </summary>
        /// <param name="pFila">Fila de persona</param>
        /// <param name="pSolicitudID">Identificador de solicitud</param>
        private void GuardarFoto(AD.EntityModel.Models.PersonaDS.Persona pFila, Guid pSolicitudID)
        {
            Stopwatch sw = null;
            try
            {
                ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                string url = UrlIntragnossServicios.Replace("https://", "http://");
                servicioImagenes.Url = url;
                sw = LoggingService.IniciarRelojTelemetria();
                byte[] resultado = servicioImagenes.ObtenerImagen(UtilArchivos.ContentImagenesSolicitudes + "/" + pSolicitudID.ToString(), ".png");
                mLoggingService.AgregarEntradaDependencia("Obtener imagen desde servicio imagenes", false, "GuardarFoto", sw, false);
                if (resultado != null)
                {
                    pFila.Foto = resultado;
                }
            }
            catch (Exception)
            {
                mLoggingService.AgregarEntradaDependencia("Error guardar la foto del perfil", false, "GuardarFoto", sw, false);
                throw;
            }
        }

        /// <summary>
        /// Agrega al usuario en los proyectos en los que es obligatorio que esté registrado cuando se registra
        /// </summary>
        /// <param name="pOrganizacionRegistroUsuario">Identificador de la organización donde se está registrando</param>
        /// <param name="pProyectoRegistroUsuario">Identificador de la comunidad donde se está registrando</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="filaUsuario">Fila Usuario</param>
        /// <param name="gestorUsuarios">Gestor de usuarios con el usuario ya cargado</param>
        /// <param name="gestorIdentidades">Gestor de identidades con la identidad ya cargada</param>
        public void RegistrarUsuarioEnProyectosObligatorios(Guid pOrganizacionRegistroUsuario, Guid pProyectoRegistroUsuario, Guid pPersonaID, Perfil pPerfilPersona, AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario, GestionUsuarios gestorUsuarios, GestionIdentidades gestorIdentidades)
        {
            ParametroAplicacionCN paramApliCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            //List<ParametroAplicacionDS.ProyectoRegistroObligatorioRow> filasProyectosARegistrarUsuario = paramApliCN.ObtenerFilasProyectosARegistrarUsuario(pOrganizacionRegistroUsuario, pProyectoRegistroUsuario);

            List<ProyectoRegistroObligatorio> filasProyectosARegistrarUsuario = paramApliCN.ObtenerFilasProyectosARegistrarUsuario(pOrganizacionRegistroUsuario, pProyectoRegistroUsuario);
            Dictionary<Guid, bool> recibirNewsletterDefectoProyectos = proyCN.ObtenerProyectosConConfiguracionNewsletterPorDefecto();

            foreach (ProyectoRegistroObligatorio fila in filasProyectosARegistrarUsuario)
            {
                Guid organizacionID = fila.OrganizacionID;
                Guid proyectoID = fila.ProyectoID;
                bool visibleUsuariosActivos = fila.VisibilidadUsuariosActivos == 1 && ComprobarPersonaEsMayor14Anios(pPerfilPersona.PersonaPerfil);

                if (gestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Count(identidad => identidad.ProyectoID.Equals(proyectoID)) == 0)
                {
                    //se agrega la identidad al perfil
                    Identidad identidad = new ControladorIdentidades(gestorIdentidades, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarIdentidadPerfilYUsuarioAProyecto(gestorIdentidades, gestorUsuarios, organizacionID, proyectoID, filaUsuario, pPerfilPersona, recibirNewsletterDefectoProyectos);
                    identidad.FilaIdentidad.ActivoEnComunidad = visibleUsuariosActivos;
                }
            }

            proyCN.Dispose();
            paramApliCN.Dispose();
        }

        private static bool ComprobarPersonaEsMayor14Anios(Persona pPersona)
        {
            bool esMayor = true;

            if (pPersona != null)
            {
                DateTime zeroTime = new DateTime(1, 1, 1);

                TimeSpan span = DateTime.Now.Subtract(pPersona.Fecha);
                // Because we start at year 1 for the Gregorian
                // calendar, we must subtract a year here.
                int years = (zeroTime + span).Year - 1;

                esMayor = years >= 14;
            }

            return esMayor;
        }


        #region Propiedades

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
                    //ParametroAplicacionDS.ParametroAplicacionRow[] filasParametro = (ParametroAplicacionDS.ParametroAplicacionRow[])ParametroAplicacionDS.ParametroAplicacion.Select("parametro='PerfilPersonalDisponible'");
                    List<AD.EntityModel.ParametroAplicacion> filasParametro = ParametroAplicacionDS.Where(parametro => parametro.Parametro.Equals("PerfilPersonalDisponible")).ToList();

                    if (filasParametro.Count > 0 && filasParametro.First().Valor.ToLower().Equals("false"))
                    {
                        perfilPersonalDisponible = false;
                    }
                }

                return perfilPersonalDisponible.Value;
            }
        }

        #endregion

    }
}
