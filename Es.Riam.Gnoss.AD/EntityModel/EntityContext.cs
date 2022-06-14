namespace Es.Riam.Gnoss.AD.EntityModel
{
    using Elementos.ParametroGeneralDSName;
    using Es.Riam.Gnoss.Util.Configuracion;
    using Es.Riam.Gnoss.Web.MVC.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
    using Models;
    using Models.Blog;
    using Models.Carga;
    using Models.CMS;
    using Models.Comentario;
    using Models.ComparticionAutomatica;
    using Models.Documentacion;
    using Models.Faceta;
    using Models.IdentidadDS;
    using Models.MVC;
    using Models.Notificacion;
    using Models.OrganizacionDS;
    using Models.Pais;
    using Models.ParametroGeneralDS;
    using Models.PersonaDS;
    using Models.Peticion;
    using Models.ProyectoDS;
    using Models.Solicitud;
    using Models.Suscripcion;
    using Models.Tesauro;
    using Models.UsuarioDS;
    using Models.VistaVirtualDS;
    using Models.Voto;
    using Oracle.ManagedDataAccess.Client;
    using Riam.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using Util.General;
    using Models.Sitemaps;
    using Npgsql;
    using System.Reflection;
    using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
    using Es.Riam.AbstractsOpen;

    public partial class EntityContext : DbContext
    {
        private string mDefaultSchema;
        private bool mCache;

        private UtilPeticion mUtilPeticion;

        private LoggingService mLoggingService;
        private ILoggerFactory mLoggerFactory;
        private DbContextOptions<EntityContext> mDbContextOptions;
        private readonly ConfigService _configService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        /// <summary>
        /// Constructor internal, para obtener un objeto EntityContext, llamar al m�todo ObtenerEntityContext del BaseAD
        /// </summary>
        public EntityContext(UtilPeticion utilPeticion, LoggingService loggingService, ILoggerFactory loggerFactory, DbContextOptions<EntityContext> dbContextOptions, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, bool pCache = false)
            : base(dbContextOptions)
        {
            loggingService.AgregarEntrada("Tiempos_construir_entityContext");
            _configService = configService;
            mCache = pCache;
            mLoggerFactory = loggerFactory;
            mUtilPeticion = utilPeticion;
            //if (_configService.ObtenerTipoBD().Equals("2"))
            //{
            //    mDefaultSchema = "dbo";
            //}

            mLoggingService = loggingService;
            mDbContextOptions = dbContextOptions;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        public EntityContext(UtilPeticion utilPeticion, LoggingService loggingService, ILoggerFactory loggerFactory, DbContextOptions<EntityContext> dbContextOptions, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, string pDefaultSchema = null, bool pCache = false)
            : base(dbContextOptions)
        {
            loggingService.AgregarEntrada("Tiempos_construir_entityContext");
            _configService = configService;
            mDefaultSchema = pDefaultSchema;
            //if (_configService.ObtenerTipoBD().Equals("2") && mDefaultSchema == null)
            //{
            //    mDefaultSchema = "dbo";
            //}
            mCache = pCache;
            mLoggerFactory = loggerFactory;
            mUtilPeticion = utilPeticion;
            mLoggingService = loggingService;
            mDbContextOptions = dbContextOptions;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        public void Migrate()
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configService.ObtenerTipoBD().Equals("2"))
            {
                optionsBuilder.UseNpgsql(_configService.ObtenerSqlConnectionString());
            }
            else if (_configService.ObtenerTipoBD().Equals("1"))
            {
                optionsBuilder.UseOracle(_configService.ObtenerSqlConnectionString());
            }
            optionsBuilder.UseLoggerFactory(mLoggerFactory);
        }

        public void EliminarInstance()
        {
            EntityContext instance = mUtilPeticion.ObtenerObjetoDePeticion("EntityContext") as EntityContext;
            if (instance != null)
            {
                instance.Database.GetDbConnection().Close();
                instance.Database.GetDbConnection().Dispose();
                instance.Dispose();
                mUtilPeticion.EliminarObjetoDePeticion("EntityContext");
                mUtilPeticion.EliminarObjetoDePeticion("Conexion");
            }
            EntityContext instanceCache = mUtilPeticion.ObtenerObjetoDePeticion("EntityContextSinProxy") as EntityContext;
            if (instanceCache != null)
            {
                instanceCache.Database.GetDbConnection().Close();
                instanceCache.Database.GetDbConnection().Dispose();
                instanceCache.Dispose();
                mUtilPeticion.EliminarObjetoDePeticion("EntityContextSinProxy");
            }

        }

        public bool UsarEntityCache
        {
            get
            {
                bool? usarEntityCache = mUtilPeticion.ObtenerObjetoDePeticion("EntityContextCache") as bool?;

                return usarEntityCache.HasValue && usarEntityCache.Value;
            }
            set
            {
                mUtilPeticion.AgregarObjetoAPeticionActual("EntityContextCache", value);
            }
        }
        public virtual DbSet<ConfiguracionEnvioCorreo> ConfiguracionEnvioCorreo { get; set; }
        public virtual DbSet<PermisosPaginasUsuarios> PermisosPaginasUsuarios { get; set; }
        public virtual DbSet<GrupoOrgParticipaProy> GrupoOrgParticipaProy { get; set; }
        public virtual DbSet<RedireccionRegistroRuta> RedireccionRegistroRuta { get; set; }
        public virtual DbSet<RedireccionValorParametro> RedireccionValorParametro { get; set; }
        public virtual DbSet<IntegracionContinuaPropiedad> IntegracionContinuaPropiedad { get; set; }
        public virtual DbSet<ParametroAplicacion> ParametroAplicacion { get; set; }
        public virtual DbSet<ConfiguracionServiciosDominio> ConfiguracionServiciosDominio { get; set; }
        public virtual DbSet<ConfiguracionServiciosProyecto> ConfiguracionServiciosProyecto { get; set; }
        public virtual DbSet<ConfigApplicationInsightsDominio> ConfigApplicationInsightsDominio { get; set; }
        public virtual DbSet<ConfiguracionServicios> ConfiguracionServicios { get; set; }
        public virtual DbSet<ConfiguracionBBDD> ConfiguracionBBDD { get; set; }
        public virtual DbSet<AccionesExternas> AccionesExternas { get; set; }
        public virtual DbSet<TextosPersonalizadosPlataforma> TextosPersonalizadosPlataforma { get; set; }
        public virtual DbSet<ProyectoSinRegistroObligatorio> ProyectoSinRegistroObligatorio { get; set; }
        public virtual DbSet<ProyectoRegistroObligatorio> ProyectoRegistroObligatorio { get; set; }
        public virtual DbSet<ProyectoServicioWeb> ProyectoServicioWeb { get; set; }

        //Dato Extra

        //Blog
        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<BlogAgCatTesauro> BlogAgCatTesauro { get; set; }
        public virtual DbSet<BlogComunidad> BlogComunidad { get; set; }
        public virtual DbSet<EntradaBlog> EntradaBlog { get; set; }

        //Suscripcion
        public virtual DbSet<CategoriaTesVinSuscrip> CategoriaTesVinSuscrip { get; set; }
        public virtual DbSet<Suscripcion> Suscripcion { get; set; }
        public virtual DbSet<SuscripcionBlog> SuscripcionBlog { get; set; }
        public virtual DbSet<SuscripcionIdentidadProyecto> SuscripcionIdentidadProyecto { get; set; }
        public virtual DbSet<SuscripcionTesauroOrganizacion> SuscripcionTesauroOrganizacion { get; set; }
        public virtual DbSet<SuscripcionTesauroProyecto> SuscripcionTesauroProyecto { get; set; }
        public virtual DbSet<SuscripcionTesauroUsuario> SuscripcionTesauroUsuario { get; set; }

        //ParametroGeneralDS
        public virtual DbSet<ConfiguracionAmbitoBusquedaProyecto> ConfiguracionAmbitoBusquedaProyecto { get; set; }
        public virtual DbSet<ParametroGeneral> ParametroGeneral { get; set; }
        public virtual DbSet<ParametroProyecto> ParametroProyecto { get; set; }
        public virtual DbSet<ProyectoElementoHtml> ProyectoElementoHtml { get; set; }
        public virtual DbSet<ProyectoElementoHTMLRol> ProyectoElementoHTMLRol { get; set; }
        public virtual DbSet<ProyectoMetaRobots> ProyectoMetaRobots { get; set; }
        public virtual DbSet<ProyectoRDFType> ProyectoRDFType { get; set; }
        public virtual DbSet<TextosPersonalizadosPersonalizacion> TextosPersonalizadosPersonalizacion { get; set; }
        public virtual DbSet<TextosPersonalizadosProyecto> TextosPersonalizadosProyecto { get; set; }
        public virtual DbSet<ConfiguracionAmbitoBusqueda> ConfiguracionAmbitoBusqueda { get; set; }

        //Pais
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Provincia> Provincia { get; set; }


        //ProyectoDS
        public virtual DbSet<AdministradorGrupoProyecto> AdministradorGrupoProyecto { get; set; }
        public virtual DbSet<AdministradorProyecto> AdministradorProyecto { get; set; }
        public virtual DbSet<NivelCertificacion> NivelCertificacion { get; set; }
        public virtual DbSet<PresentacionListadoSemantico> PresentacionListadoSemantico { get; set; }
        public virtual DbSet<PresentacionMapaSemantico> PresentacionMapaSemantico { get; set; }
        public virtual DbSet<PresentacionMosaicoSemantico> PresentacionMosaicoSemantico { get; set; }
        public virtual DbSet<PresentacionPersonalizadoSemantico> PresentacionPersonalizadoSemantico { get; set; }
        public virtual DbSet<PresentacionPestanyaListadoSemantico> PresentacionPestanyaListadoSemantico { get; set; }
        public virtual DbSet<PresentacionPestanyaMapaSemantico> PresentacionPestanyaMapaSemantico { get; set; }
        public virtual DbSet<PresentacionPestanyaMosaicoSemantico> PresentacionPestanyaMosaicoSemantico { get; set; }
        public virtual DbSet<Proyecto> Proyecto { get; set; }
        public virtual DbSet<ProyectoAgCatTesauro> ProyectoAgCatTesauro { get; set; }
        public virtual DbSet<ProyectoCerradoTmp> ProyectoCerradoTmp { get; set; }
        public virtual DbSet<ProyectoCerrandose> ProyectoCerrandose { get; set; }
        public virtual DbSet<ProyectoConfigExtraSem> ProyectoConfigExtraSem { get; set; }
        public virtual DbSet<ProyectoGadget> ProyectoGadget { get; set; }
        public virtual DbSet<ProyectoGadgetContexto> ProyectoGadgetContexto { get; set; }
        public virtual DbSet<ProyectoGadgetContextoHTMLplano> ProyectoGadgetContextoHTMLplano { get; set; }
        public virtual DbSet<ProyectoGadgetIdioma> ProyectoGadgetIdioma { get; set; }
        public virtual DbSet<ProyectoGrafoFichaRec> ProyectoGrafoFichaRec { get; set; }
        public virtual DbSet<ProyectoLoginConfiguracion> ProyectoLoginConfiguracion { get; set; }
        public virtual DbSet<ProyectoPaginaHtml> ProyectoPaginaHtml { get; set; }
        public virtual DbSet<ProyectoPasoRegistro> ProyectoPasoRegistro { get; set; }
        public virtual DbSet<ProyectoPerfilNumElem> ProyectoPerfilNumElem { get; set; }
        public virtual DbSet<ProyectoPestanya> ProyectoPestanya { get; set; }
        public virtual DbSet<ProyectoPestanyaBusqueda> ProyectoPestanyaBusqueda { get; set; }
        public virtual DbSet<ProyectoPestanyaBusquedaExportacion> ProyectoPestanyaBusquedaExportacion { get; set; }
        public virtual DbSet<ProyectoPestanyaBusquedaExportacionExterna> ProyectoPestanyaBusquedaExportacionExterna { get; set; }
        public virtual DbSet<ProyectoPestanyaBusquedaExportacionPropiedad> ProyectoPestanyaBusquedaExportacionPropiedad { get; set; }
        public virtual DbSet<ProyectoPestanyaCMS> ProyectoPestanyaCMS { get; set; }
        public virtual DbSet<ProyectoPestanyaExportacionBusqueda> ProyectoPestanyaExportacionBusqueda { get; set; }
        public virtual DbSet<ProyectoPestanyaFiltroOrdenRecursos> ProyectoPestanyaFiltroOrdenRecursos { get; set; }
        public virtual DbSet<ProyectoPestanyaMenu> ProyectoPestanyaMenu { get; set; }
        public virtual DbSet<ProyectoPestanyaMenuRolGrupoIdentidades> ProyectoPestanyaMenuRolGrupoIdentidades { get; set; }
        public virtual DbSet<ProyectoPestanyaMenuRolIdentidad> ProyectoPestanyaMenuRolIdentidad { get; set; }
        public virtual DbSet<ProyectoPestanyaRolGrupoIdentidades> ProyectoPestanyaRolGrupoIdentidades { get; set; }
        public virtual DbSet<ProyectoPestanyaRolIdentidad> ProyectoPestanyaRolIdentidad { get; set; }
        public virtual DbSet<ProyectoRelacionado> ProyectoRelacionado { get; set; }
        public virtual DbSet<ProyectoSearchPersonalizado> ProyectoSearchPersonalizado { get; set; }
        public virtual DbSet<ProyectoServicioExterno> ProyectoServicioExterno { get; set; }
        public virtual DbSet<EcosistemaServicioExterno> EcosistemaServicioExterno { get; set; }
        public virtual DbSet<ProyectosMasActivos> ProyectosMasActivos { get; set; }
        public virtual DbSet<ProyTipoRecNoActivReciente> ProyTipoRecNoActivReciente { get; set; }
        public virtual DbSet<CamposRegistroProyectoGenericos> CamposRegistroProyectoGenericos { get; set; }
        public virtual DbSet<DatoExtraEcosistema> DatoExtraEcosistema { get; set; }
        public virtual DbSet<DatoExtraEcosistemaOpcion> DatoExtraEcosistemaOpcion { get; set; }
        public virtual DbSet<DatoExtraEcosistemaVirtuoso> DatoExtraEcosistemaVirtuoso { get; set; }
        public virtual DbSet<DatoExtraProyecto> DatoExtraProyecto { get; set; }
        public virtual DbSet<DatoExtraProyectoOpcion> DatoExtraProyectoOpcion { get; set; }
        public virtual DbSet<DatoExtraProyectoVirtuoso> DatoExtraProyectoVirtuoso { get; set; }
        public virtual DbSet<TipoOntoDispRolUsuarioProy> TipoOntoDispRolUsuarioProy { get; set; }

        public virtual DbSet<SeccionProyCatalogo> SeccionProyCatalogo { get; set; }

        public virtual DbSet<TipoDocImagenPorDefecto> TipoDocImagenPorDefecto { get; set; }
        public virtual DbSet<GrupoIdentidades> GrupoIdentidades { get; set; }
        public virtual DbSet<GrupoIdentidadesOrganizacion> GrupoIdentidadesOrganizacion { get; set; }
        public virtual DbSet<GrupoIdentidadesParticipacion> GrupoIdentidadesParticipacion { get; set; }
        public virtual DbSet<GrupoIdentidadesProyecto> GrupoIdentidadesProyecto { get; set; }
        public virtual DbSet<Persona> Persona { get; set; }
        public virtual DbSet<Perfil> Perfil { get; set; }
        public virtual DbSet<Identidad> Identidad { get; set; }

        //Voto
        public virtual DbSet<VotoEntradaBlog> VotoEntradaBlog { get; set; }
        public virtual DbSet<VotoMensajeForo> VotoMensajeForo { get; set; }
        public virtual DbSet<Voto> Voto { get; set; }

        //ComparticionAutomatica
        public virtual DbSet<ComparticionAutomatica> ComparticionAutomatica { get; set; }
        public virtual DbSet<ComparticionAutomaticaMapping> ComparticionAutomaticaMapping { get; set; }
        public virtual DbSet<ComparticionAutomaticaReglas> ComparticionAutomaticaReglas { get; set; }

        //Identidad
        public virtual DbSet<PerfilGadget> PerfilGadget { get; set; }
        public virtual DbSet<PerfilOrganizacion> PerfilOrganizacion { get; set; }
        public virtual DbSet<PerfilRedesSociales> PerfilRedesSociales { get; set; }
        public virtual DbSet<Profesor> Profesor { get; set; }
        public virtual DbSet<OrganizacionClase> OrganizacionClase { get; set; }
        public virtual DbSet<Models.OrganizacionDS.OrganizacionEmpresa> OrganizacionEmpresa { get; set; }
        public virtual DbSet<AmigoAgGrupo> AmigoAgGrupo { get; set; }
        public virtual DbSet<DatoExtraEcosistemaVirtuosoPerfil> DatoExtraEcosistemaVirtuosoPerfil { get; set; }
        public virtual DbSet<DatoExtraProyectoVirtuosoIdentidad> DatoExtraProyectoVirtuosoIdentidad { get; set; }
        public virtual DbSet<GrupoAmigos> GrupoAmigos { get; set; }
        public virtual DbSet<DatoExtraEcosistemaOpcionPerfil> DatoExtraEcosistemaOpcionPerfil { get; set; }
        public virtual DbSet<PermisoAmigoOrg> PermisoAmigoOrg { get; set; }
        public virtual DbSet<Amigo> Amigo { get; set; }
        public virtual DbSet<PermisoGrupoAmigoOrg> PermisoGrupoAmigoOrg { get; set; }
        public virtual DbSet<IdentidadContadores> IdentidadContadores { get; set; }
        public virtual DbSet<IdentidadContadoresRecursos> IdentidadContadoresRecursos { get; set; }

        public virtual DbSet<ProyectoRolGrupoUsuario> ProyectoRolGrupoUsuario { get; set; }
        public virtual DbSet<Organizacion> Organizacion { get; set; }
        public virtual DbSet<PersonaVinculoOrganizacion> PersonaVinculoOrganizacion { get; set; }
        public virtual DbSet<OrganizacionParticipaProy> OrganizacionParticipaProy { get; set; }
        public virtual DbSet<TipoDocDispRolUsuarioProy> TipoDocDispRolUsuarioProy { get; set; }
        public virtual DbSet<ContadorPerfil> ContadorPerfil { get; set; }

        //VistaVirtual
        public virtual DbSet<VistaVirtual> VistaVirtual { get; set; }
        public virtual DbSet<VistaVirtualCMS> VistaVirtualCMS { get; set; }
        public virtual DbSet<VistaVirtualGadgetRecursos> VistaVirtualGadgetRecursos { get; set; }
        public virtual DbSet<VistaVirtualPersonalizacion> VistaVirtualPersonalizacion { get; set; }
        public virtual DbSet<VistaVirtualProyecto> VistaVirtualProyecto { get; set; }
        public virtual DbSet<VistaVirtualRecursos> VistaVirtualRecursos { get; set; }


        public virtual DbSet<RecursosRelacionadosPresentacion> RecursosRelacionadosPresentacion { get; set; }
        public virtual DbSet<PersonaVisibleEnOrg> PersonaVisibleEnOrg { get; set; }
        public virtual DbSet<PersonaOcupacionFigura> PersonaOcupacionFigura { get; set; }
        public virtual DbSet<PersonaOcupacionFormaSec> PersonaOcupacionFormaSec { get; set; }
        public virtual DbSet<OrganizacionGnoss> OrganizacionGnoss { get; set; }
        public virtual DbSet<ConfiguracionGnossOrg> ConfiguracionGnossOrg { get; set; }

        public virtual DbSet<AdministradorGeneral> AdministradorGeneral { get; set; }
        public virtual DbSet<GeneralRolUsuario> GeneralRolUsuario { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<UsuarioContadores> UsuarioContadores { get; set; }
        public virtual DbSet<HistoricoProyectoUsuario> HistoricoProyectoUsuario { get; set; }
        public virtual DbSet<UsuarioVinculadoLoginRedesSociales> UsuarioVinculadoLoginRedesSociales { get; set; }
        public virtual DbSet<InicioSesion> InicioSesion { get; set; }
        public virtual DbSet<GrupoUsuarioUsuario> GrupoUsuarioUsuario { get; set; }
        public virtual DbSet<OrganizacionRolUsuario> OrganizacionRolUsuario { get; set; }
        public virtual DbSet<ProyectoUsuarioIdentidad> ProyectoUsuarioIdentidad { get; set; }
        public virtual DbSet<AccionesExternasProyecto> AccionesExternasProyecto { get; set; }
        public virtual DbSet<PreferenciaProyecto> PreferenciaProyecto { get; set; }
        public virtual DbSet<ProyectoEvento> ProyectoEvento { get; set; }
        public virtual DbSet<ProyectoEventoParticipante> ProyectoEventoParticipante { get; set; }
        public virtual DbSet<PerfilPersonaOrg> PerfilPersonaOrg { get; set; }
        public virtual DbSet<PerfilPersona> PerfilPersona { get; set; }
        public virtual DbSet<ProyectoEventoAccion> ProyectoEventoAccion { get; set; }
        public virtual DbSet<ConfigAutocompletarProy> ConfigAutocompletarProy { get; set; }
        public virtual DbSet<ConfigSearchProy> ConfigSearchProy { get; set; }
        public virtual DbSet<DatoExtraProyectoOpcionIdentidad> DatoExtraProyectoOpcionIdentidad { get; set; }
        public virtual DbSet<ProyectoPalabrasInapropiadas> ProyectoPalabrasInapropiadas { get; set; }
        public virtual DbSet<UsuarioRedirect> UsuarioRedirect { get; set; }

        //MVC
        public virtual DbSet<CorreoInterno> CorreoInterno { get; set; }

        //Documento
        public virtual DbSet<AtributoFichaBibliografica> AtributoFichaBibliografica { get; set; }
        public virtual DbSet<DocumentoEntidadGnoss> DocumentoEntidadGnoss { get; set; }
        public virtual DbSet<BaseRecursosUsuario> BaseRecursosUsuario { get; set; }
        public virtual DbSet<ColaCargaRecursos> ColaCargaRecursos { get; set; }
        public virtual DbSet<DocumentoAtributoBiblio> DocumentoAtributoBiblio { get; set; }
        public virtual DbSet<DocumentoComentario> DocumentoComentario { get; set; }
        public virtual DbSet<DocumentoEnEdicion> DocumentoEnEdicion { get; set; }
        public virtual DbSet<DocumentoEnvioNewsLetter> DocumentoEnvioNewsLetter { get; set; }
        public virtual DbSet<DocumentoGrupoUsuario> DocumentoGrupoUsuario { get; set; }
        public virtual DbSet<DocumentoNewsletter> DocumentoNewsletter { get; set; }
        public virtual DbSet<DocumentoRespuesta> DocumentoRespuesta { get; set; }
        public virtual DbSet<DocumentoRespuestaVoto> DocumentoRespuestaVoto { get; set; }
        public virtual DbSet<DocumentoRolGrupoIdentidades> DocumentoRolGrupoIdentidades { get; set; }
        public virtual DbSet<DocumentoTipologia> DocumentoTipologia { get; set; }
        public virtual DbSet<DocumentoTokenBrightcove> DocumentoTokenBrightcove { get; set; }
        public virtual DbSet<DocumentoTokenTOP> DocumentoTokenTOP { get; set; }
        public virtual DbSet<DocumentoUrlCanonica> DocumentoUrlCanonica { get; set; }
        public virtual DbSet<DocumentoVincDoc> DocumentoVincDoc { get; set; }
        public virtual DbSet<FichaBibliografica> FichaBibliografica { get; set; }
        public virtual DbSet<Tipologia> Tipologia { get; set; }
        public virtual DbSet<VotoDocumento> VotoDocumento { get; set; }
        public virtual DbSet<BaseRecursos> BaseRecursos { get; set; }
        public virtual DbSet<BaseRecursosProyecto> BaseRecursosProyecto { get; set; }
        public virtual DbSet<ColaDocumento> ColaDocumento { get; set; }
        public virtual DbSet<Documento> Documento { get; set; }
        public virtual DbSet<DocumentoRolIdentidad> DocumentoRolIdentidad { get; set; }
        public virtual DbSet<DocumentoWebAgCatTesauro> DocumentoWebAgCatTesauro { get; set; }
        public virtual DbSet<DocumentoWebVinBaseRecursos> DocumentoWebVinBaseRecursos { get; set; }
        public virtual DbSet<DocumentoWebVinBaseRecursosExtra> DocumentoWebVinBaseRecursosExtra { get; set; }
        public virtual DbSet<HistorialDocumento> HistorialDocumento { get; set; }
        public virtual DbSet<VersionDocumento> VersionDocumento { get; set; }
        public virtual DbSet<ResultadoSuscripcion> ResultadoSuscripcion { get; set; }
        public virtual DbSet<DocumentoLecturaAumentada> DocumentoLecturaAumentada { get; set; }

        //Tesauro
        public virtual DbSet<CategoriaTesauro> CategoriaTesauro { get; set; }
        public virtual DbSet<CategoriaTesauroPropiedades> CategoriaTesauroPropiedades { get; set; }
        public virtual DbSet<CategoriaTesauroSugerencia> CategoriaTesauroSugerencia { get; set; }
        public virtual DbSet<CatTesauroAgCatTesauro> CatTesauroAgCatTesauro { get; set; }
        public virtual DbSet<CatTesauroCompartida> CatTesauroCompartida { get; set; }
        public virtual DbSet<CatTesauroPermiteTipoRec> CatTesauroPermiteTipoRec { get; set; }
        public virtual DbSet<Tesauro> Tesauro { get; set; }
        public virtual DbSet<TesauroOrganizacion> TesauroOrganizacion { get; set; }
        public virtual DbSet<TesauroProyecto> TesauroProyecto { get; set; }
        public virtual DbSet<TesauroUsuario> TesauroUsuario { get; set; }

        //Comentario

        public virtual DbSet<Comentario> Comentario { get; set; }
        public virtual DbSet<VotoComentario> VotoComentario { get; set; }
        public virtual DbSet<ComentarioBlog> ComentarioBlog { get; set; }
        public virtual DbSet<ComentarioCuestion> ComentarioCuestion { get; set; }

        //Documentacion

        public virtual DbSet<BaseRecursosOrganizacion> BaseRecursosOrganizacion { get; set; }


        //FACETA
        public virtual DbSet<ConfiguracionConexionGrafo> ConfiguracionConexionGrafo { get; set; }
        public virtual DbSet<OntologiaProyecto> OntologiaProyecto { get; set; }
        public virtual DbSet<FacetaObjetoConocimientoProyecto> FacetaObjetoConocimientoProyecto { get; set; }
        public virtual DbSet<FacetaEntidadesExternas> FacetaEntidadesExternas { get; set; }
        public virtual DbSet<FacetaFiltroProyecto> FacetaFiltroProyecto { get; set; }
        public virtual DbSet<FacetaObjetoConocimiento> FacetaObjetoConocimiento { get; set; }
        public virtual DbSet<FacetaExcluida> FacetaExcluida { get; set; }
        public virtual DbSet<FacetaObjetoConocimientoProyectoPestanya> FacetaObjetoConocimientoProyectoPestanya { get; set; }
        public virtual DbSet<FacetaConfigProyChart> FacetaConfigProyChart { get; set; }
        public virtual DbSet<FacetaConfigProyMapa> FacetaConfigProyMapa { get; set; }
        public virtual DbSet<FacetaFiltroHome> FacetaFiltroHome { get; set; }
        public virtual DbSet<FacetaMultiple> FacetaMultiple { get; set; }
        public virtual DbSet<FacetaRedireccion> FacetaRedireccion { get; set; }
        public virtual DbSet<FacetaConfigProyRangoFecha> FacetaConfigProyRangoFecha { get; set; }
        public virtual DbSet<FacetaHome> FacetaHome { get; set; }

        //Peticion
        public virtual DbSet<Peticion> Peticion { get; set; }
        public virtual DbSet<PeticionInvitacionComunidad> PeticionInvitacionComunidad { get; set; }
        public virtual DbSet<PeticionInvitacionGrupo> PeticionInvitacionGrupo { get; set; }
        public virtual DbSet<PeticionInvitaContacto> PeticionInvitaContacto { get; set; }
        public virtual DbSet<PeticionNuevoProyecto> PeticionNuevoProyecto { get; set; }
        public virtual DbSet<PeticionOrgInvitaPers> PeticionOrgInvitaPers { get; set; }

        //CMS        
        public virtual DbSet<CMSBloque> CMSBloque { get; set; }
        public virtual DbSet<CMSBloqueComponente> CMSBloqueComponente { get; set; }
        public virtual DbSet<CMSBloqueComponentePropiedadComponente> CMSBloqueComponentePropiedadComponente { get; set; }
        public virtual DbSet<CMSComponente> CMSComponente { get; set; }
        public virtual DbSet<CMSComponentePrivadoProyecto> CMSComponentePrivadoProyecto { get; set; }
        public virtual DbSet<CMSComponenteRolGrupoIdentidades> CMSComponenteRolGrupoIdentidades { get; set; }
        public virtual DbSet<CMSComponenteRolIdentidad> CMSComponenteRolIdentidad { get; set; }
        public virtual DbSet<CMSPagina> CMSPagina { get; set; }
        public virtual DbSet<CMSPropiedadComponente> CMSPropiedadComponente { get; set; }
        public virtual DbSet<CMSRolGrupoIdentidades> CMSRolGrupoIdentidades { get; set; }
        public virtual DbSet<CMSRolIdentidad> CMSRolIdentidad { get; set; }


        //UsuarioDS
        public virtual DbSet<ClausulaRegistro> ClausulaRegistro { get; set; }
        public virtual DbSet<ProyectoRolUsuario> ProyectoRolUsuario { get; set; }
        public virtual DbSet<ProyRolUsuClausulaReg> ProyRolUsuClausulaReg { get; set; }
        public virtual DbSet<AdministradorOrganizacion> AdministradorOrganizacion { get; set; }
        public virtual DbSet<GrupoUsuario> GrupoUsuario { get; set; }
        public virtual DbSet<GeneralRolGrupoUsuario> GeneralRolGrupoUsuario { get; set; }


        //Curriculum
        public virtual DbSet<Curriculum> Curriculum { get; set; }

        // Solicitud
        public virtual DbSet<Solicitud> Solicitud { get; set; }
        public virtual DbSet<ConfiguracionGnossPersona> ConfiguracionGnossPersona { get; set; }
        public virtual DbSet<DatosTrabajoPersonaLibre> DatosTrabajoPersonaLibre { get; set; }
        public virtual DbSet<SolicitudNuevaOrganizacion> SolicitudNuevaOrganizacion { get; set; }
        public virtual DbSet<SolicitudNuevoProfesor> SolicitudNuevoProfesor { get; set; }
        public virtual DbSet<SolicitudNuevoUsuario> SolicitudNuevoUsuario { get; set; }
        public virtual DbSet<SolicitudOrganizacion> SolicitudOrganizacion { get; set; }
        public virtual DbSet<SolicitudUsuario> SolicitudUsuario { get; set; }
        public virtual DbSet<DatoExtraEcosistemaOpcionSolicitud> DatoExtraEcosistemaOpcionSolicitud { get; set; }
        public virtual DbSet<DatoExtraEcosistemaVirtuosoSolicitud> DatoExtraEcosistemaVirtuosoSolicitud { get; set; }
        public virtual DbSet<DatoExtraProyectoOpcionSolicitud> DatoExtraProyectoOpcionSolicitud { get; set; }
        public virtual DbSet<SolicitudGrupo> SolicitudGrupo { get; set; }
        public virtual DbSet<SolicitudNuevaOrgEmp> SolicitudNuevaOrgEmp { get; set; }
        public virtual DbSet<DatoExtraProyectoVirtuosoSolicitud> DatoExtraProyectoVirtuosoSolicitud { get; set; }

        //Sitemaps
        public virtual DbSet<Sitemaps> Sitemaps { get; set; }
        public virtual DbSet<SitemapsIndex> SitemapsIndex { get; set; }


        // Notificacion

        public virtual DbSet<ColaTwitter> ColaTwitter { get; set; }
        public virtual DbSet<EmailIncorrecto> EmailIncorrecto { get; set; }
        public virtual DbSet<Invitacion> Invitacion { get; set; }
        public virtual DbSet<Notificacion> Notificacion { get; set; }
        public virtual DbSet<NotificacionAlertaPersona> NotificacionAlertaPersona { get; set; }
        public virtual DbSet<NotificacionCorreoPersona> NotificacionCorreoPersona { get; set; }
        public virtual DbSet<NotificacionEnvioMasivo> NotificacionEnvioMasivo { get; set; }
        public virtual DbSet<NotificacionParametro> NotificacionParametro { get; set; }
        public virtual DbSet<NotificacionParametroPersona> NotificacionParametroPersona { get; set; }
        public virtual DbSet<NotificacionSolicitud> NotificacionSolicitud { get; set; }
        public virtual DbSet<NotificacionSuscripcion> NotificacionSuscripcion { get; set; }

        public virtual DbSet<Carga> Carga { get; set; }
        public virtual DbSet<CargaPaquete> CargaPaquete { get; set; }

        //Cookie
        public virtual DbSet<CategoriaProyectoCookie> CategoriaProyectoCookie { get; set; }
        public virtual DbSet<ProyectoCookie> ProyectoCookie { get; set; }

        public virtual DbSet<TokenApiLogin> TokenApiLogin { get; set; }
        //new tables
        public virtual DbSet<UltimosDocumentosVisitados> UltimosDocumentosVisitados { get; set; }



        public bool NoConfirmarTransacciones
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.NoConfirmarTransacciones;
            }
            set
            {
                mServicesUtilVirtuosoAndReplication.NoConfirmarTransacciones = value;
            }
        }
        public Dictionary<string, DbTransaction> TransaccionesPendientes
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.TransaccionesPendientes;
            }
        }

        /// <summary>
        /// Termina las transacciones pendientes que no se han confirmado
        /// IMPORTANTE: Usar sólo si se ha establecido NoConfirmarTransacciones a TRUE. En caso contrario, usar TerminarTransaccion
        /// </summary>
        /// <param name="pExito">Verdad si se deba hacer commit. Falso si se debe deshacer la transacción</param>
        public void TerminarTransaccionesPendientes(bool pExito)
        {
            if (mServicesUtilVirtuosoAndReplication.NoConfirmarTransacciones)
            {
                foreach (string nombreTransaccion in TransaccionesPendientes.Keys)
                {
                    DbTransaction transaccion = TransaccionesPendientes[nombreTransaccion];
                    try
                    {
                        TransaccionesPendientes.Remove(nombreTransaccion);
                        if (pExito)
                        {
                            transaccion.Commit();
                        }
                        else
                        {
                            if (transaccion.Connection != null)
                            {
                                transaccion.Rollback();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex);

                        if (pExito)
                        {
                            throw;
                        }
                    }
                }
                TransaccionesPendientes.Clear();
                Database.UseTransaction(null);
                mServicesUtilVirtuosoAndReplication.NoConfirmarTransacciones = false;
                //mFacetadoAD.InsertarInstruccionesEnReplica();
            }
        }
        private static readonly MethodInfo IsNumericMethodInfo = typeof(EntityContext)
       .GetRuntimeMethod(nameof(IsNumeric), new[] { typeof(string) });

        public bool IsNumeric(string s)
        {
            int num = 0;
            return int.TryParse(s, out num);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(IsNumericMethodInfo)
            .HasName("ISNUMERIC")
            .IsBuiltIn();
            modelBuilder.Entity<BlogAgCatTesauro>()
                .HasKey(c => new { c.TesauroID, c.CategoriaTesauroID, c.BlogID });
            modelBuilder.Entity<PermisosPaginasUsuarios>()
                .HasKey(c => new { c.UsuarioID, c.OrganizacionID, c.ProyectoID, c.Pagina });
            modelBuilder.Entity<GrupoOrgParticipaProy>()
                .HasKey(c => new { c.GrupoID, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<RedireccionValorParametro>()
                .HasKey(c => new { c.RedireccionID, c.ValorParametro });
            modelBuilder.Entity<IntegracionContinuaPropiedad>()
                .HasKey(c => new { c.ProyectoID, c.TipoObjeto, c.ObjetoPropiedad, c.TipoPropiedad });
            modelBuilder.Entity<TextosPersonalizadosPlataforma>()
               .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.TextoID, c.Language });
            modelBuilder.Entity<ProyectoSinRegistroObligatorio>()
               .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OrganizacionSinRegistroID, c.ProyectoSinRegistroID });
            modelBuilder.Entity<ProyectoRegistroObligatorio>()
               .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<BlogComunidad>()
                .HasKey(c => new { c.BlogID, c.ProyectoID, c.OrganizacionID });
            modelBuilder.Entity<EntradaBlog>()
                .HasKey(c => new { c.BlogID, c.EntradaBlogID });
            modelBuilder.Entity<CategoriaTesVinSuscrip>()
                .HasKey(c => new { c.SuscripcionID, c.TesauroID, c.CategoriaTesauroID });
            modelBuilder.Entity<SuscripcionBlog>()
                .HasKey(c => new { c.BlogID, c.SuscripcionID });
            modelBuilder.Entity<SuscripcionIdentidadProyecto>()
                .HasKey(c => new { c.IdentidadID, c.SuscripcionID });
            modelBuilder.Entity<ConfiguracionAmbitoBusquedaProyecto>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ParametroGeneral>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ParametroProyecto>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Parametro });
            modelBuilder.Entity<ProyectoElementoHtml>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ElementoHeadID });
            modelBuilder.Entity<ProyectoElementoHTMLRol>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ElementoHeadID, c.GrupoID });
            modelBuilder.Entity<ProyectoMetaRobots>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Tipo });
            modelBuilder.Entity<ProyectoRDFType>()
               .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.TipoDocumento });
            modelBuilder.Entity<TextosPersonalizadosPersonalizacion>()
               .HasKey(c => new { c.PersonalizacionID, c.TextoID, c.Language });
            modelBuilder.Entity<TextosPersonalizadosProyecto>()
               .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.TextoID, c.Language });
            modelBuilder.Entity<ConfiguracionAmbitoBusqueda>()
               .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<AdministradorGrupoProyecto>()
               .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.GrupoID });
            modelBuilder.Entity<AdministradorProyecto>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.UsuarioID, c.Tipo });
            modelBuilder.Entity<PresentacionListadoSemantico>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OntologiaID, c.Orden });
            modelBuilder.Entity<PresentacionMapaSemantico>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OntologiaID, c.Orden });
            modelBuilder.Entity<PresentacionMosaicoSemantico>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OntologiaID, c.Orden });
            modelBuilder.Entity<PresentacionPersonalizadoSemantico>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OntologiaID, c.Orden });
            modelBuilder.Entity<PresentacionPestanyaListadoSemantico>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.PestanyaID, c.OntologiaID, c.Orden });
            modelBuilder.Entity<PresentacionPestanyaMapaSemantico>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.PestanyaID, c.OntologiaID, c.Orden });
            modelBuilder.Entity<PresentacionPestanyaMosaicoSemantico>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.PestanyaID, c.OntologiaID, c.Orden });
            modelBuilder.Entity<Proyecto>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ProyectoAgCatTesauro>()
               .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.TesauroID, c.CategoriaTesauroID });
            modelBuilder.Entity<ProyectoCerradoTmp>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ProyectoCerrandose>()
                .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ProyectoConfigExtraSem>()
               .HasKey(c => new { c.ProyectoID, c.UrlOntologia, c.SourceTesSem });
            modelBuilder.Entity<ProyectoGadget>()
              .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.GadgetID });
            modelBuilder.Entity<ProyectoGadgetContexto>()
              .HasKey(c => new { c.GadgetID, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ProyectoGadgetContextoHTMLplano>()
              .HasKey(c => new { c.GadgetID, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ProyectoGadgetIdioma>()
              .HasKey(c => new { c.GadgetID, c.OrganizacionID, c.ProyectoID, c.Idioma });
            modelBuilder.Entity<ProyectoGrafoFichaRec>()
              .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.GrafoID });
            modelBuilder.Entity<ProyectoLoginConfiguracion>()
              .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ProyectoPaginaHtml>()
              .HasKey(c => new { c.ProyectoID, c.Nombre });
            modelBuilder.Entity<ProyectoPasoRegistro>()
              .HasKey(c => new { c.ProyectoID, c.OrganizacionID, c.Orden });
            modelBuilder.Entity<ProyectoPerfilNumElem>()
              .HasKey(c => new { c.ProyectoID, c.PerfilID });
            modelBuilder.Entity<ProyectoPestanya>()
              .HasKey(c => new { c.ProyectoID, c.Nombre });
            modelBuilder.Entity<ProyectoPestanyaBusquedaExportacionPropiedad>()
              .HasKey(c => new { c.ExportacionID, c.Orden });
            modelBuilder.Entity<ProyectoPestanyaCMS>()
              .HasKey(c => new { c.PestanyaID, c.Ubicacion });
            modelBuilder.Entity<ProyectoPestanyaFiltroOrdenRecursos>()
              .HasKey(c => new { c.PestanyaID, c.Orden });
            modelBuilder.Entity<ProyectoPestanyaMenuRolGrupoIdentidades>()
             .HasKey(c => new { c.PestanyaID, c.GrupoID });
            modelBuilder.Entity<ProyectoPestanyaMenu>()
             .HasKey(c => new { c.PestanyaID });
            modelBuilder.Entity<ProyectoPestanyaMenuRolIdentidad>()
             .HasKey(c => new { c.PestanyaID, c.PerfilID });
            modelBuilder.Entity<ProyectoPestanyaRolGrupoIdentidades>()
             .HasKey(c => new { c.ProyectoID, c.Nombre, c.GrupoID });
            modelBuilder.Entity<ProyectoPestanyaRolIdentidad>()
             .HasKey(c => new { c.ProyectoID, c.Nombre, c.PerfilID });
            modelBuilder.Entity<ProyectoRelacionado>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OrganizacionRelacionadaID, c.ProyectoRelacionadoID });
            modelBuilder.Entity<ProyectoSearchPersonalizado>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.NombreFiltro });
            modelBuilder.Entity<ProyectoServicioExterno>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.NombreServicio });
            modelBuilder.Entity<ProyectosMasActivos>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ProyTipoRecNoActivReciente>()
             .HasKey(c => new { c.ProyectoID, c.TipoRecurso });
            modelBuilder.Entity<CamposRegistroProyectoGenericos>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Orden });
            modelBuilder.Entity<DatoExtraEcosistemaOpcion>()
             .HasKey(c => new { c.DatoExtraID, c.OpcionID });
            modelBuilder.Entity<DatoExtraProyecto>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.DatoExtraID });
            modelBuilder.Entity<DatoExtraProyectoOpcion>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.DatoExtraID, c.OpcionID });
            modelBuilder.Entity<DatoExtraProyectoVirtuoso>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.DatoExtraID });
            modelBuilder.Entity<TipoOntoDispRolUsuarioProy>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OntologiaID, c.RolUsuario });
            modelBuilder.Entity<SeccionProyCatalogo>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OrganizacionBusquedaID, c.ProyectoBusquedaID, c.Orden });
            modelBuilder.Entity<TipoDocImagenPorDefecto>()
             .HasKey(c => new { c.ProyectoID, c.TipoRecurso, c.OntologiaID });
            modelBuilder.Entity<GrupoIdentidadesOrganizacion>()
             .HasKey(c => new { c.GrupoID, c.OrganizacionID });
            modelBuilder.Entity<GrupoIdentidadesParticipacion>()
             .HasKey(c => new { c.GrupoID, c.IdentidadID });
            modelBuilder.Entity<GrupoIdentidadesProyecto>()
             .HasKey(c => new { c.GrupoID, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<VotoEntradaBlog>()
             .HasKey(c => new { c.BlogID, c.EntradaBlogID, c.VotoID });
            modelBuilder.Entity<VotoMensajeForo>()
             .HasKey(c => new { c.VotoID, c.ForoID, c.CategoriaForoID, c.TemaID, c.MensajeForoID });
            modelBuilder.Entity<ComparticionAutomaticaMapping>()
             .HasKey(c => new { c.ComparticionID, c.ReglaMapping, c.TesauroID, c.CategoriaTesauroID });
            modelBuilder.Entity<ComparticionAutomaticaReglas>()
             .HasKey(c => new { c.ComparticionID, c.Regla });
            modelBuilder.Entity<PerfilGadget>()
             .HasKey(c => new { c.PerfilID, c.GadgetID });
            modelBuilder.Entity<PerfilOrganizacion>()
             .HasKey(c => new { c.PerfilID, c.OrganizacionID });
            modelBuilder.Entity<PerfilRedesSociales>()
             .HasKey(c => new { c.PerfilID, c.NombreRedSocial });
            modelBuilder.Entity<Profesor>()
             .HasKey(c => new { c.ProfesorID, c.PerfilID });
            modelBuilder.Entity<AmigoAgGrupo>()
             .HasKey(c => new { c.GrupoID, c.IdentidadID, c.IdentidadAmigoID });
            modelBuilder.Entity<DatoExtraEcosistemaVirtuosoPerfil>()
             .HasKey(c => new { c.DatoExtraID, c.PerfilID });
            modelBuilder.Entity<DatoExtraProyectoVirtuosoIdentidad>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.DatoExtraID, c.IdentidadID });
            modelBuilder.Entity<DatoExtraEcosistemaOpcionPerfil>()
             .HasKey(c => new { c.DatoExtraID, c.OpcionID, c.PerfilID });
            modelBuilder.Entity<PermisoAmigoOrg>()
             .HasKey(c => new { c.IdentidadOrganizacionID, c.IdentidadUsuarioID, c.IdentidadAmigoID });
            modelBuilder.Entity<Amigo>()
             .HasKey(c => new { c.IdentidadAmigoID, c.IdentidadID });
            modelBuilder.Entity<PermisoGrupoAmigoOrg>()
             .HasKey(c => new { c.IdentidadOrganizacionID, c.IdentidadUsuarioID, c.GrupoID });
            modelBuilder.Entity<IdentidadContadoresRecursos>()
             .HasKey(c => new { c.IdentidadID, c.Tipo, c.NombreSem });
            modelBuilder.Entity<ProyectoRolGrupoUsuario>()
             .HasKey(c => new { c.OrganizacionGnossID, c.ProyectoID, c.GrupoUsuarioID });
            modelBuilder.Entity<PersonaVinculoOrganizacion>()
             .HasKey(c => new { c.PersonaID, c.OrganizacionID });
            modelBuilder.Entity<OrganizacionParticipaProy>()
             .HasKey(c => new { c.OrganizacionID, c.OrganizacionProyectoID, c.ProyectoID });
            modelBuilder.Entity<TipoDocDispRolUsuarioProy>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.TipoDocumento, c.RolUsuario });
            modelBuilder.Entity<VistaVirtual>()
             .HasKey(c => new { c.PersonalizacionID, c.TipoPagina });
            modelBuilder.Entity<VistaVirtualCMS>()
             .HasKey(c => new { c.PersonalizacionID, c.TipoComponente, c.PersonalizacionComponenteID });
            modelBuilder.Entity<VistaVirtualGadgetRecursos>()
             .HasKey(c => new { c.PersonalizacionID, c.PersonalizacionComponenteID });
            modelBuilder.Entity<VistaVirtualProyecto>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.PersonalizacionID });
            modelBuilder.Entity<VistaVirtualRecursos>()
             .HasKey(c => new { c.PersonalizacionID, c.RdfType });
            modelBuilder.Entity<RecursosRelacionadosPresentacion>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Orden, c.OntologiaID });
            modelBuilder.Entity<PersonaVisibleEnOrg>()
             .HasKey(c => new { c.PersonaID, c.OrganizacionID });
            modelBuilder.Entity<PersonaOcupacionFigura>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.EstructuraID, c.ElementoEstructuraID, c.PersonaID });
            modelBuilder.Entity<PersonaOcupacionFormaSec>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.EstructuraID, c.ElementoEstructuraID, c.PersonaID });
            modelBuilder.Entity<HistoricoProyectoUsuario>()
             .HasKey(c => new { c.UsuarioID, c.OrganizacionGnossID, c.ProyectoID, c.IdentidadID, c.FechaEntrada });
            modelBuilder.Entity<UsuarioVinculadoLoginRedesSociales>()
             .HasKey(c => new { c.UsuarioID, c.TipoRedSocial });
            modelBuilder.Entity<GrupoUsuarioUsuario>()
             .HasKey(c => new { c.UsuarioID, c.GrupoUsuarioID });
            modelBuilder.Entity<OrganizacionRolUsuario>()
             .HasKey(c => new { c.UsuarioID, c.OrganizacionID });
            modelBuilder.Entity<ProyectoUsuarioIdentidad>()
             .HasKey(c => new { c.IdentidadID, c.UsuarioID, c.OrganizacionGnossID, c.ProyectoID });
            modelBuilder.Entity<AccionesExternasProyecto>()
             .HasKey(c => new { c.TipoAccion, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<PreferenciaProyecto>()
             .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.TesauroID, c.CategoriaTesauroID });
            modelBuilder.Entity<ProyectoEventoParticipante>()
             .HasKey(c => new { c.EventoID, c.IdentidadID });
            modelBuilder.Entity<ProyectoEventoAccion>()
            .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Evento });
            modelBuilder.Entity<ConfigAutocompletarProy>()
            .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Clave });
            modelBuilder.Entity<ConfigSearchProy>()
            .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Clave });
            modelBuilder.Entity<DatoExtraProyectoOpcionIdentidad>()
            .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.DatoExtraID, c.OpcionID, c.IdentidadID });
            modelBuilder.Entity<ProyectoPalabrasInapropiadas>()
            .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Tag });
            modelBuilder.Entity<AtributoFichaBibliografica>()
            .HasKey(c => new { c.AtributoID, c.FichaBibliograficaID });
            modelBuilder.Entity<DocumentoEntidadGnoss>()
            .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.EntidadGnossID, c.DocumentoID, c.CategoriaDocumentacionID });
            modelBuilder.Entity<BaseRecursosUsuario>()
            .HasKey(c => new { c.BaseRecursosID, c.UsuarioID });
            modelBuilder.Entity<DocumentoAtributoBiblio>()
            .HasKey(c => new { c.DocumentoID, c.AtributoID, c.FichaBibliograficaID });
            modelBuilder.Entity<DocumentoComentario>()
            .HasKey(c => new { c.ComentarioID, c.DocumentoID });
            modelBuilder.Entity<DocumentoEnvioNewsLetter>()
            .HasKey(c => new { c.DocumentoID, c.IdentidadID, c.Fecha });
            modelBuilder.Entity<DocumentoGrupoUsuario>()
            .HasKey(c => new { c.DocumentoID, c.GrupoUsuarioID });
            modelBuilder.Entity<DocumentoRespuestaVoto>()
            .HasKey(c => new { c.DocumentoID, c.IdentidadID });
            modelBuilder.Entity<DocumentoRolGrupoIdentidades>()
            .HasKey(c => new { c.DocumentoID, c.GrupoID });
            modelBuilder.Entity<DocumentoTipologia>()
            .HasKey(c => new { c.DocumentoID, c.TipologiaID, c.AtributoID });
            modelBuilder.Entity<DocumentoVincDoc>()
            .HasKey(c => new { c.DocumentoID, c.DocumentoVincID });
            modelBuilder.Entity<Tipologia>()
           .HasKey(c => new { c.TipologiaID, c.AtributoID });
            modelBuilder.Entity<VotoDocumento>()
           .HasKey(c => new { c.DocumentoID, c.VotoID });
            modelBuilder.Entity<BaseRecursosProyecto>()
           .HasKey(c => new { c.BaseRecursosID, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<DocumentoRolIdentidad>()
           .HasKey(c => new { c.DocumentoID, c.PerfilID });
            modelBuilder.Entity<DocumentoWebAgCatTesauro>()
            .HasKey(c => new { c.TesauroID, c.CategoriaTesauroID, c.BaseRecursosID, c.DocumentoID });
            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
           .HasKey(c => new { c.DocumentoID, c.BaseRecursosID });
            modelBuilder.Entity<DocumentoWebVinBaseRecursosExtra>()
           .HasKey(c => new { c.DocumentoID, c.BaseRecursosID });
            modelBuilder.Entity<ResultadoSuscripcion>()
           .HasKey(c => new { c.SuscripcionID, c.RecursoID });
            modelBuilder.Entity<CategoriaTesauro>()
           .HasKey(c => new { c.TesauroID, c.CategoriaTesauroID });
            modelBuilder.Entity<CategoriaTesauroPropiedades>()
           .HasKey(c => new { c.TesauroID, c.CategoriaTesauroID });
            modelBuilder.Entity<CatTesauroAgCatTesauro>()
           .HasKey(c => new { c.TesauroID, c.CategoriaSuperiorID, c.CategoriaInferiorID });
            modelBuilder.Entity<CatTesauroCompartida>()
           .HasKey(c => new { c.TesauroOrigenID, c.CategoriaOrigenID, c.TesauroDestinoID });
            modelBuilder.Entity<CatTesauroPermiteTipoRec>()
           .HasKey(c => new { c.TesauroID, c.CategoriaTesauroID, c.TipoRecurso });
            modelBuilder.Entity<TesauroOrganizacion>()
          .HasKey(c => new { c.TesauroID, c.OrganizacionID });
            modelBuilder.Entity<TesauroProyecto>()
          .HasKey(c => new { c.TesauroID, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<TesauroUsuario>()
          .HasKey(c => new { c.TesauroID, c.UsuarioID });
            modelBuilder.Entity<VotoComentario>()
          .HasKey(c => new { c.VotoID, c.ComentarioID });
            modelBuilder.Entity<ComentarioBlog>()
          .HasKey(c => new { c.ComentarioID, c.BlogID, c.EntradaBlogID });
            modelBuilder.Entity<ComentarioCuestion>()
          .HasKey(c => new { c.ComentarioID, c.CuestionID });
            modelBuilder.Entity<BaseRecursosOrganizacion>()
         .HasKey(c => new { c.BaseRecursosID, c.OrganizacionID });
            modelBuilder.Entity<OntologiaProyecto>()
         .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.OntologiaProyecto1 });
            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
         .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ObjetoConocimiento, c.Faceta });
            modelBuilder.Entity<FacetaEntidadesExternas>()
         .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.EntidadID });
            modelBuilder.Entity<FacetaFiltroProyecto>()
         .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ObjetoConocimiento, c.Faceta, c.Orden });
            modelBuilder.Entity<FacetaObjetoConocimiento>()
         .HasKey(c => new { c.ObjetoConocimiento, c.Faceta });
            modelBuilder.Entity<FacetaExcluida>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Faceta });
            modelBuilder.Entity<FacetaObjetoConocimientoProyectoPestanya>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ObjetoConocimiento, c.Faceta, c.PestanyaID });
            modelBuilder.Entity<FacetaConfigProyChart>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ChartID });
            modelBuilder.Entity<FacetaConfigProyMapa>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<FacetaFiltroHome>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ObjetoConocimiento, c.Faceta, c.Orden });
            modelBuilder.Entity<FacetaMultiple>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ObjetoConocimiento, c.Faceta });
            modelBuilder.Entity<FacetaConfigProyRangoFecha>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.PropiedadNueva, c.PropiedadInicio, c.PropiedadFin });
            modelBuilder.Entity<FacetaHome>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.ObjetoConocimiento, c.Faceta });
            modelBuilder.Entity<CMSBloqueComponente>()
        .HasKey(c => new { c.BloqueID, c.ComponenteID });
            modelBuilder.Entity<CMSBloqueComponentePropiedadComponente>()
        .HasKey(c => new { c.BloqueID, c.ComponenteID, c.TipoPropiedadComponente });
            modelBuilder.Entity<CMSComponentePrivadoProyecto>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.TipoComponente });
            modelBuilder.Entity<CMSComponenteRolGrupoIdentidades>()
        .HasKey(c => new { c.ComponenteID, c.GrupoID });
            modelBuilder.Entity<CMSComponenteRolIdentidad>()
        .HasKey(c => new { c.ComponenteID, c.PerfilID });
            modelBuilder.Entity<CMSPagina>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Ubicacion });
            modelBuilder.Entity<CMSPropiedadComponente>()
        .HasKey(c => new { c.ComponenteID, c.TipoPropiedadComponente });
            modelBuilder.Entity<CMSRolGrupoIdentidades>()
        .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Ubicacion, c.GrupoID });
            modelBuilder.Entity<ClausulaRegistro>()
        .HasKey(c => new { c.ClausulaID, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<ProyectoRolUsuario>()
        .HasKey(c => new { c.OrganizacionGnossID, c.ProyectoID, c.UsuarioID });
            modelBuilder.Entity<ProyRolUsuClausulaReg>()
        .HasKey(c => new { c.ClausulaID, c.OrganizacionID, c.OrganizacionGnossID, c.ProyectoID, c.UsuarioID });
            modelBuilder.Entity<AdministradorOrganizacion>()
       .HasKey(c => new { c.UsuarioID, c.OrganizacionID, c.Tipo });
            modelBuilder.Entity<SolicitudNuevaOrganizacion>()
       .HasKey(c => new { c.SolicitudID, c.UsuarioAdminID });
            modelBuilder.Entity<SolicitudNuevoProfesor>()
       .HasKey(c => new { c.SolicitudID, c.UsuarioID });
            modelBuilder.Entity<SolicitudNuevoUsuario>()
       .HasKey(c => new { c.UsuarioID, c.SolicitudID });
            modelBuilder.Entity<SolicitudOrganizacion>()
       .HasKey(c => new { c.SolicitudID, c.OrganizacionID });
            modelBuilder.Entity<SolicitudUsuario>()
       .HasKey(c => new { c.SolicitudID, c.UsuarioID, c.PersonaID });
            modelBuilder.Entity<DatoExtraEcosistemaOpcionSolicitud>()
      .HasKey(c => new { c.DatoExtraID, c.OpcionID, c.SolicitudID });
            modelBuilder.Entity<DatoExtraEcosistemaVirtuosoSolicitud>()
     .HasKey(c => new { c.DatoExtraID, c.Opcion, c.SolicitudID });
            modelBuilder.Entity<DatoExtraProyectoOpcionSolicitud>()
     .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.DatoExtraID, c.OpcionID, c.SolicitudID });
            modelBuilder.Entity<SolicitudGrupo>()
     .HasKey(c => new { c.SolicitudID, c.GrupoID, c.IdentidadID });
            modelBuilder.Entity<SolicitudNuevaOrgEmp>()
     .HasKey(c => new { c.SolicitudID, c.UsuarioAdminID });
            modelBuilder.Entity<DatoExtraProyectoVirtuosoSolicitud>()
     .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.DatoExtraID, c.SolicitudID });
            modelBuilder.Entity<NotificacionAlertaPersona>()
     .HasKey(c => new { c.NotificacionID, c.PersonaID });
            modelBuilder.Entity<NotificacionCorreoPersona>()
     .HasKey(c => new { c.NotificacionID, c.EmailEnvio });
            modelBuilder.Entity<NotificacionParametro>()
     .HasKey(c => new { c.NotificacionID, c.ParametroID });
            modelBuilder.Entity<NotificacionParametroPersona>()
     .HasKey(c => new { c.NotificacionID, c.ParametroID, c.PersonaID });
            modelBuilder.Entity<NotificacionSolicitud>()
     .HasKey(c => new { c.NotificacionID, c.SolicitudID, c.OrganizacionID, c.ProyectoID });
            modelBuilder.Entity<NotificacionSuscripcion>()
     .HasKey(c => new { c.NotificacionID, c.SuscripcionID });
            modelBuilder.Entity<CMSRolIdentidad>()
     .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.Ubicacion, c.PerfilID });
            modelBuilder.Entity<ProyectoServicioWeb>()
     .HasKey(c => new { c.OrganizacionID, c.ProyectoID, c.AplicacionWeb });
            modelBuilder.Entity<PerfilPersonaOrg>()
     .HasKey(c => new { c.PersonaID, c.OrganizacionID, c.PerfilID });

            modelBuilder.Entity<Models.Sitemaps.Sitemaps>()
          .HasKey(c => new { c.Dominio, c.SitemapIndexName });

            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
       .HasOne(a => a.DocumentoWebVinBaseRecursosExtra)
       .WithOne(b => b.DocumentoWebVinBaseRecursos)
       .HasForeignKey<DocumentoWebVinBaseRecursosExtra>(b => new { b.DocumentoID, b.BaseRecursosID });
            modelBuilder.Entity<FacetaHome>()
       .HasOne(a => a.FacetaObjetoConocimientoProyecto)
       .WithOne(b => b.FacetaHome)
       .HasForeignKey<FacetaObjetoConocimientoProyecto>(b => new { b.OrganizacionID, b.ProyectoID, b.ObjetoConocimiento, b.Faceta });
            modelBuilder.Entity<ProyectoCerradoTmp>()
       .HasOne(a => a.Proyecto)
       .WithOne(b => b.ProyectoCerradoTmp)
       .HasForeignKey<ProyectoCerradoTmp>(b => new { b.OrganizacionID, b.ProyectoID });
            modelBuilder.Entity<ProyectoCerrandose>()
       .HasOne(a => a.Proyecto)
       .WithOne(b => b.ProyectoCerrandose)
       .HasForeignKey<ProyectoCerrandose>(b => new { b.OrganizacionID, b.ProyectoID });
            modelBuilder.Entity<ProyectoGadget>()
       .HasOne(a => a.ProyectoGadgetContextoHTMLplano)
       .WithOne(b => b.ProyectoGadget)
       .HasForeignKey<ProyectoGadgetContextoHTMLplano>(b => new { b.OrganizacionID, b.ProyectoID, b.GadgetID });
            modelBuilder.Entity<ProyectoLoginConfiguracion>()
      .HasOne(a => a.Proyecto)
      .WithOne(b => b.ProyectoLoginConfiguracion)
      .HasForeignKey<ProyectoLoginConfiguracion>(b => new { b.OrganizacionID, b.ProyectoID });

            modelBuilder.Entity<ProyectoPestanyaMenu>()
      .HasOne(a => a.ProyectoPestanyaBusqueda)
      .WithOne(b => b.ProyectoPestanyaMenu)
      .HasForeignKey<ProyectoPestanyaBusqueda>(b => new { b.PestanyaID });

            modelBuilder.Entity<ProyectoPestanyaBusquedaExportacion>()
      .HasOne(a => a.ProyectoPestanyaBusquedaExportacionExterna)
      .WithOne(b => b.ProyectoPestanyaBusquedaExportacion)
      .HasForeignKey<ProyectoPestanyaBusquedaExportacionExterna>(b => new { b.ExportacionID });
            modelBuilder.Entity<ProyectosMasActivos>()
      .HasOne(a => a.Proyecto)
      .WithOne(b => b.ProyectosMasActivos)
      .HasForeignKey<ProyectosMasActivos>(b => new { b.OrganizacionID, b.ProyectoID });
            modelBuilder.Entity<SolicitudNuevaOrganizacion>()
      .HasOne(a => a.SolicitudNuevaOrgEmp)
      .WithOne(b => b.SolicitudNuevaOrganizacion)
      .HasForeignKey<SolicitudNuevaOrgEmp>(b => new { b.SolicitudID, b.UsuarioAdminID });
            modelBuilder.Entity<ConfigAutocompletarProy>()
      .HasOne(a => a.ProyectoPestanyaMenu)
      .WithMany(b => b.ConfigAutocompletarProy)
      .HasForeignKey(b => b.PestanyaID);
            modelBuilder.Entity<RedireccionValorParametro>()
      .HasOne(a => a.RedireccionRegistroRuta)
      .WithMany(b => b.RedireccionValorParametro)
      .HasForeignKey(b => b.RedireccionID);
            modelBuilder.Entity<GrupoIdentidades>()
                .HasMany(e => e.GrupoIdentidadesParticipacion)
                .WithOne(e => e.GrupoIdentidades)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            //.OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GrupoIdentidades>()
                .HasMany(e => e.GrupoIdentidadesProyecto)
                .WithOne(e => e.GrupoIdentidades)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<ClausulaRegistro>()
            //    .HasMany(e => e.ProyRolUsuClausulaReg)
            //    .WithRequired(e => e.ClausulaRegistro)
            //    .HasForeignKey(e => new { e.ClausulaID, e.OrganizacionID, e.ProyectoID })
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Perfil>()
                .HasMany(e => e.DatoExtraEcosistemaOpcionPerfil)
                .WithOne(e => e.Perfil)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GeneralRolUsuario>()
                .Property(e => e.RolPermitido)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<GeneralRolUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<OrganizacionRolUsuario>()
                .Property(e => e.RolPermitido)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<OrganizacionRolUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoRolUsuario>()
                .Property(e => e.RolPermitido)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoRolUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoRolGrupoUsuario>()
                .Property(e => e.RolPermitido)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoRolGrupoUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoRolUsuario>()
                .HasMany(e => e.ProyRolUsuClausulaReg)
                .WithOne(e => e.ProyectoRolUsuario)
                .IsRequired()
                .HasForeignKey(e => new { OrganizacionGnossID = e.OrganizacionGnossID, ProyectoID = e.ProyectoID, UsuarioID = e.UsuarioID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(e => e.AdministradorGeneral)
                .WithOne(e => e.Usuario)
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.AdministradorOrganizacion)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(e => e.GeneralRolUsuario)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .HasOne(e => e.InicioSesion)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.OrganizacionRolUsuario)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.ProyectoUsuarioIdentidad)
                .WithOne(e => e.Usuario)
                .HasForeignKey(e => e.UsuarioID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Identidad>()
                .HasMany(e => e.ProyectoUsuarioIdentidad)
                .WithOne(e => e.Identidad)
                .HasForeignKey(e => e.IdentidadID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoUsuarioIdentidad)
                .WithOne(e => e.Proyecto)
                .HasForeignKey(e => new { e.OrganizacionGnossID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.UsuarioVinculadoLoginRedesSociales)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(e => e.UsuarioContadores)
                .WithOne(e => e.Usuario)
                .IsRequired();

            modelBuilder.Entity<GeneralRolGrupoUsuario>()
                    .Property(e => e.RolPermitido)
                    .IsFixedLength()
                    .IsUnicode(false);

            modelBuilder.Entity<GeneralRolGrupoUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ConfiguracionEnvioCorreo>()
                .Property(e => e.tipo)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoRegistroObligatorio>()
                .HasMany(e => e.ListaProyectoSinRegistroObligatorio)
                .WithOne(e => e.ProyectoRegistroObligatorio)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionSinRegistroID, e.ProyectoSinRegistroID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.CoordenadasHome)
                .IsFixedLength();

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.CoordenadasMosaico)
                .IsFixedLength();

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.CoordenadasSup)
                .IsFixedLength();

            modelBuilder.Entity<ProyectoElementoHtml>()
                .HasMany(e => e.ProyectoElementoHTMLRol)
                .WithOne(e => e.ProyectoElementoHtml)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID, e.ElementoHeadID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.EnlaceContactoPiePagina)
                .IsUnicode(false);

            //Documento
            modelBuilder.Entity<AtributoFichaBibliografica>()
                .HasMany(e => e.DocumentoAtributoBiblio)
                .WithOne(e => e.AtributoFichaBibliografica)
                .IsRequired()
                .HasForeignKey(e => new { e.AtributoID, e.FichaBibliograficaID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GrupoIdentidades>()
                .HasMany(e => e.GrupoIdentidadesOrganizacion)
                .WithOne(e => e.GrupoIdentidades)
                .IsRequired()
                .HasForeignKey(e => e.GrupoID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<DocumentoRespuesta>()
                .HasMany(e => e.DocumentoRespuestaVoto)
                .WithOne(e => e.DocumentoRespuesta)
                .IsRequired()
                .HasForeignKey(e => e.RespuestaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FichaBibliografica>()
                .HasMany(e => e.AtributoFichaBibliografica)
                .WithOne(e => e.FichaBibliografica)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tipologia>()
                .HasMany(e => e.DocumentoTipologia)
                .WithOne(e => e.Tipologia)
                .IsRequired()
                .HasForeignKey(e => new { e.TipologiaID, e.AtributoID })
                .OnDelete(DeleteBehavior.Restrict);

            //Blog
            modelBuilder.Entity<Blog>()
                .HasMany(e => e.BlogAgCatTesauro)
                .WithOne(e => e.Blog)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blog>()
                .HasMany(e => e.BlogComunidad)
                .WithOne(e => e.Blog)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //Peticion
            modelBuilder.Entity<Peticion>()
                .HasOne(e => e.PeticionInvitaContacto)
                .WithOne(e => e.Peticion)
                .IsRequired();

            modelBuilder.Entity<Peticion>()
                .HasOne(e => e.PeticionInvitacionComunidad)
                .WithOne(e => e.Peticion)
                .IsRequired();

            modelBuilder.Entity<Peticion>()
                .HasOne(e => e.PeticionOrgInvitaPers)
                .WithOne(e => e.Peticion)
                .IsRequired();

            modelBuilder.Entity<Peticion>()
                .HasOne(e => e.PeticionInvitacionGrupo)
                .WithOne(e => e.Peticion)
                .IsRequired();

            modelBuilder.Entity<Peticion>()
                .HasOne(e => e.PeticionNuevoProyecto)
                .WithOne(e => e.Peticion)
                .IsRequired();


            //ProyectoDS
            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.AdministradorGrupoProyecto)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.AdministradorProyecto)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID });

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.NivelCertificacion)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.PresentacionListadoSemantico)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.PresentacionMapaSemantico)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.PresentacionMosaicoSemantico)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
               .HasMany(e => e.PresentacionPersonalizadoSemantico)
               .WithOne(e => e.Proyecto)
               .IsRequired()
               .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.PresentacionPestanyaListadoSemantico)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.PresentacionPestanyaMapaSemantico)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.PresentacionPestanyaMosaicoSemantico)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoRelacionado)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoAgCatTesauro)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoCerradoTmp>()
                .HasOne(e => e.Proyecto)
                .WithOne(e => e.ProyectoCerradoTmp)
                .IsRequired();

            modelBuilder.Entity<ProyectoCerrandose>()
                .HasOne(e => e.Proyecto)
                .WithOne(e => e.ProyectoCerrandose)
                .IsRequired();

            modelBuilder.Entity<ProyectoLoginConfiguracion>()
                .HasOne(e => e.Proyecto)
                .WithOne(e => e.ProyectoLoginConfiguracion)
                .IsRequired();

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.Proyecto1)
                .WithOne(e => e.Proyecto2)
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoSuperiorID });

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoGrafoFichaRec)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoPasoRegistro)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoPestanyaMenu)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoSearchPersonalizado)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoServicioExterno)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectosMasActivos>()
                .HasOne(e => e.Proyecto)
                .WithOne(e => e.ProyectosMasActivos).
                IsRequired();

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoRelacionado1)
                .WithOne(e => e.Proyecto1)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionRelacionadaID, e.ProyectoRelacionadoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoGadget)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoGadget>()
                .HasOne(e => e.ProyectoGadgetContextoHTMLplano)
                .WithOne(e => e.ProyectoGadget)
                .IsRequired();

            modelBuilder.Entity<ProyectoGadget>()
                .HasMany(e => e.ProyectoGadgetIdioma)
                .WithOne(e => e.ProyectoGadget)
                .IsRequired()
                .HasForeignKey(e => new { e.GadgetID, e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.VistaDisponible)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoPestanya>()
                .HasMany(e => e.ProyectoPestanyaRolGrupoIdentidades)
                .WithOne(e => e.ProyectoPestanya)
                .IsRequired()
                .HasForeignKey(e => new { e.ProyectoID, e.Nombre })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanya>()
                .HasMany(e => e.ProyectoPestanyaRolIdentidad)
                .WithOne(e => e.ProyectoPestanya)
                .IsRequired()
                .HasForeignKey(e => new { e.ProyectoID, e.Nombre })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .HasMany(e => e.ProyectoPestanyaBusquedaExportacion)
                .WithOne(e => e.ProyectoPestanyaBusqueda)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaBusquedaExportacion>()
                .HasOne(e => e.ProyectoPestanyaBusquedaExportacionExterna)
                .WithOne(e => e.ProyectoPestanyaBusquedaExportacion)
                .IsRequired();

            modelBuilder.Entity<ProyectoPestanyaBusquedaExportacion>()
                .HasMany(e => e.ProyectoPestanyaBusquedaExportacionPropiedad)
                .WithOne(e => e.ProyectoPestanyaBusquedaExportacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaBusquedaExportacionPropiedad>()
                .Property(e => e.DatosExtraPropiedad)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .Property(e => e.NombreCortoPestanya)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.PresentacionPestanyaListadoSemantico)
                .WithOne(e => e.ProyectoPestanyaMenu)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.PresentacionPestanyaMapaSemantico)
                .WithOne(e => e.ProyectoPestanyaMenu)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.PresentacionPestanyaMosaicoSemantico)
                .WithOne(e => e.ProyectoPestanyaMenu)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .HasOne(e => e.ProyectoPestanyaMenu)
                .WithOne(e => e.ProyectoPestanyaBusqueda)
                .IsRequired();

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.ProyectoPestanyaCMS)
                .WithOne(e => e.ProyectoPestanyaMenu)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.ProyectoPestanyaFiltroOrdenRecursos)
                .WithOne(e => e.ProyectoPestanyaMenu)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.ProyectoPestanyaMenu1)
                .WithOne(e => e.ProyectoPestanyaMenu2)
                .HasForeignKey(e => e.PestanyaPadreID);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.ProyectoPestanyaMenuRolGrupoIdentidades)
                .WithOne(e => e.ProyectoPestanyaMenu)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.ProyectoPestanyaMenuRolIdentidad)
                .WithOne(e => e.ProyectoPestanyaMenu)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.FacetaObjetoConocimientoProyectoPestanya)
                .WithOne(e => e.ProyectoPestanyaMenu)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EcosistemaServicioExterno>()
                .Property(e => e.NombreServicio)
                .IsUnicode(false);

            modelBuilder.Entity<EcosistemaServicioExterno>()
                .Property(e => e.UrlServicio)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoServicioExterno>()
                .Property(e => e.NombreServicio)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoServicioExterno>()
                .Property(e => e.UrlServicio)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoRolUsuario>()
                .Property(e => e.RolPermitido)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoRolUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<DatoExtraEcosistema>()
                .HasMany(e => e.DatoExtraEcosistemaOpcion)
                .WithOne(e => e.DatoExtraEcosistema)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatoExtraProyecto>()
               .HasMany(e => e.DatoExtraProyectoOpcion)
               .WithOne(e => e.DatoExtraProyecto)
               .IsRequired()
               .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID, e.DatoExtraID })
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoEvento>()
                .HasMany(e => e.ProyectoEventoParticipante)
                .WithOne(e => e.ProyectoEvento)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(e => e.UsuarioRedirect)
                .WithOne(e => e.Usuario)
                .IsRequired();

            modelBuilder.Entity<Persona>()
               .Property(e => e.Sexo)
               .IsFixedLength()
               .IsUnicode(false);

            modelBuilder.Entity<DatoExtraProyectoVirtuoso>()
                .HasMany(e => e.DatoExtraProyectoVirtuosoIdentidad)
                .WithOne(e => e.DatoExtraProyectoVirtuoso)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID, e.DatoExtraID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatoExtraProyectoVirtuoso>()
                .HasMany(e => e.DatoExtraProyectoVirtuosoSolicitud)
                .WithOne(e => e.DatoExtraProyectoVirtuoso)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID, e.DatoExtraID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Identidad>()
                .HasMany(e => e.DatoExtraProyectoVirtuosoIdentidad)
                .WithOne(e => e.Identidad)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //ComparticionAutomatica
            modelBuilder.Entity<ComparticionAutomatica>()
               .HasMany(e => e.ComparticionAutomaticaMapping)
               .WithOne(e => e.ComparticionAutomatica)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComparticionAutomatica>()
                .HasMany(e => e.ComparticionAutomaticaReglas)
                .WithOne(e => e.ComparticionAutomatica)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComparticionAutomaticaReglas>()
                .Property(e => e.Navegacion)
                .IsUnicode(false);

            //Comentario
            modelBuilder.Entity<Comentario>()
                .HasMany(e => e.ComentarioBlog)
                .WithOne(e => e.Comentario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comentario>()
                .HasMany(e => e.ComentarioCuestion)
                .WithOne(e => e.Comentario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comentario>()
                .HasMany(e => e.Comentario1)
                .WithOne(e => e.Comentario2)
                .HasForeignKey(e => e.ComentarioSuperiorID);

            modelBuilder.Entity<Comentario>()
                .HasMany(e => e.VotoComentario)
                .WithOne(e => e.Comentario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            //Organizacion
            modelBuilder.Entity<Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa>()
                .HasOne(e => e.Organizacion)
                .WithOne(e => e.OrganizacionEmpresa).IsRequired();

            modelBuilder.Entity<Organizacion>()
                .HasMany(e => e.PersonaVinculoOrganizacion)
                .WithOne(e => e.Organizacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Organizacion>()
                .HasMany(e => e.OrganizacionParticipaProy)
                .WithOne(e => e.Organizacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Organizacion>()
                .HasOne(e => e.ConfiguracionGnossOrg)
                .WithOne(e => e.Organizacion)
                .IsRequired();

            modelBuilder.Entity<OrganizacionGnoss>()
                .HasOne(e => e.Organizacion)
                .WithOne(e => e.OrganizacionGnoss).
                IsRequired();

            modelBuilder.Entity<Organizacion>()
                .HasMany(e => e.Organizacion1)
                .WithOne(e => e.Organizacion2)
                .HasForeignKey(e => e.OrganizacionPadreID);

            modelBuilder.Entity<Organizacion>()
                .HasMany(e => e.AdministradorOrganizacion)
                .WithOne(e => e.Organizacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BaseRecursos>()
               .HasMany(e => e.BaseRecursosOrganizacion)
               .WithOne(e => e.BaseRecursos)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BaseRecursos>()
                .HasMany(e => e.BaseRecursosProyecto)
                .WithOne(e => e.BaseRecursos)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BaseRecursos>()
                .HasMany(e => e.BaseRecursosUsuario)
                .WithOne(e => e.BaseRecursos)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BaseRecursos>()
                .HasMany(e => e.DocumentoWebVinBaseRecursos)
                .WithOne(e => e.BaseRecursos)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasMany(e => e.DocumentoRolIdentidad)
                .WithOne(e => e.Documento)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasMany(e => e.DocumentoWebVinBaseRecursos)
                .WithOne(e => e.Documento)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasOne(e => e.VersionDocumento)
                .WithOne(e => e.Documento)
                .IsRequired();

            modelBuilder.Entity<Documento>()
                .HasMany(e => e.HistorialDocumento)
                .WithOne(e => e.Documento)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Documento>()
                .HasMany(e => e.DocumentoRolGrupoIdentidades)
                .WithOne(e => e.Documento)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasMany(e => e.DocumentoVincDoc)
                .WithOne(e => e.Documento)
                .IsRequired()
                .HasForeignKey(e => e.DocumentoID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasMany(e => e.DocumentoVincDoc1)
                .WithOne(e => e.Documento1)
                .IsRequired()
                .HasForeignKey(e => e.DocumentoVincID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
               .HasMany(e => e.DocumentoRespuestaVoto)
               .WithOne(e => e.Documento)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasMany(e => e.VotoDocumento)
                .WithOne(e => e.Documento)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //Voto

            modelBuilder.Entity<Voto>()
                .HasMany(e => e.VotoDocumento)
                .WithOne(e => e.Voto)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voto>()
                .HasMany(e => e.VotoComentario)
                .WithOne(e => e.Voto)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voto>()
                .HasMany(e => e.VotoEntradaBlog)
                .WithOne(e => e.Voto)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voto>()
                .HasMany(e => e.VotoMensajeForo)
                .WithOne(e => e.Voto)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //Pais
            modelBuilder.Entity<Pais>()
                .HasMany(e => e.Provincia)
                .WithOne(e => e.Pais)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //Tesauro

            modelBuilder.Entity<CategoriaTesauro>()
                .HasMany(e => e.DocumentoWebAgCatTesauro)
                .WithOne(e => e.CategoriaTesauro)
                .IsRequired()
                .HasForeignKey(e => new { e.TesauroID, e.CategoriaTesauroID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CategoriaTesauro>()
                .HasMany(e => e.CategoriaTesauroSugerencia)
                .WithOne(e => e.CategoriaTesauro)
                .HasForeignKey(e => new { e.TesauroCatPadreID, e.CategoriaTesauroPadreID });

            modelBuilder.Entity<CategoriaTesauro>()
                .HasMany(e => e.CatTesauroAgCatTesauroInferior)
                .WithOne(e => e.CategoriaTesauro)
                .IsRequired()
                .HasForeignKey(e => new { e.TesauroID, e.CategoriaInferiorID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CategoriaTesauro>()
                .HasMany(e => e.CatTesauroAgCatTesauroSuperior)
                .WithOne(e => e.CategoriaTesuaro1)
                .IsRequired()
                .HasForeignKey(e => new { e.TesauroID, e.CategoriaSuperiorID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CategoriaTesauro>()
                .HasMany(e => e.CatTesauroCompartida)
                .WithOne(e => e.CategoriaTesauro)
                .HasForeignKey(e => new { e.TesauroDestinoID, e.CategoriaSupDestinoID });

            modelBuilder.Entity<CategoriaTesauro>()
                .HasMany(e => e.CatTesauroCompartida1)
                .WithOne(e => e.CategoriaTesauro1)
                .IsRequired()
                .HasForeignKey(e => new { e.TesauroOrigenID, e.CategoriaOrigenID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CategoriaTesauro>()
               .HasMany(e => e.CategoriaTesauroSugerencia)
               .WithOne(e => e.CategoriaTesauro)
               .HasForeignKey(e => new { e.TesauroCatPadreID, e.CategoriaTesauroPadreID });

            modelBuilder.Entity<Tesauro>()
                .HasMany(e => e.CategoriaTesauro)
                .WithOne(e => e.Tesauro)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tesauro>()
                .HasMany(e => e.CategoriaTesauroSugerencia)
                .WithOne(e => e.Tesauro)
                .IsRequired()
                .HasForeignKey(e => e.TesauroSugerenciaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tesauro>()
                .HasMany(e => e.TesauroProyecto)
                .WithOne(e => e.Tesauro)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tesauro>()
                .HasMany(e => e.TesauroUsuario)
                .WithOne(e => e.Tesauro)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tesauro>()
                .HasMany(e => e.TesauroOrganizacion)
                .WithOne(e => e.Tesauro)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
               .HasMany(e => e.ProyectoPestanyaBusquedaExportacion)
               .WithOne(e => e.ProyectoPestanyaBusqueda)
               .IsRequired()
               .HasForeignKey(e => e.PestanyaID)
               .OnDelete(DeleteBehavior.Restrict);

            //FACETA

            modelBuilder.Entity<OntologiaProyecto>()
                .Property(e => e.NombreCortoOnt)
                .IsUnicode(false);

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .HasMany(e => e.FacetaFiltroProyecto)
                .WithOne(e => e.FacetaObjetoConocimientoProyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID, e.ObjetoConocimiento, e.Faceta })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .HasOne(e => e.FacetaHome)
                .WithOne(e => e.FacetaObjetoConocimientoProyecto)
                .IsRequired();

            modelBuilder.Entity<FacetaHome>()
                .HasMany(e => e.FacetaFiltroHome)
                .WithOne(e => e.FacetaHome)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID, e.ObjetoConocimiento, e.Faceta })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .HasMany(e => e.FacetaObjetoConocimientoProyectoPestanya)
                .WithOne(e => e.FacetaObjetoConocimientoProyecto)
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID, e.ObjetoConocimiento, e.Faceta })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.FacetaObjetoConocimientoProyectoPestanya)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID })
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario
            modelBuilder.Entity<ProyectoRolUsuario>()
               .Property(e => e.RolPermitido)
               .IsFixedLength()
               .IsUnicode(false);

            modelBuilder.Entity<ProyectoRolUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<OrganizacionRolUsuario>()
                .Property(e => e.RolPermitido)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<OrganizacionRolUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.OrganizacionRolUsuario)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.UsuarioVinculadoLoginRedesSociales)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Usuario>()
               .HasMany(e => e.BaseRecursosUsuario)
               .WithOne(e => e.Usuario)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Usuario>()
                .HasOne(e => e.GeneralRolUsuario)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GeneralRolUsuario>()
                .Property(e => e.RolPermitido)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.TesauroUsuario)
                .WithOne(e => e.Usuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(e => e.UsuarioContadores)
                .WithOne(e => e.Usuario)
                .IsRequired();

            modelBuilder.Entity<GeneralRolUsuario>()
                .Property(e => e.RolDenegado)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.Proyecto1)
                .WithOne(e => e.Proyecto2)
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoSuperiorID });

            //VistaVirtual
            modelBuilder.Entity<VistaVirtualCMS>()
               .Property(e => e.DatosExtra)
               .IsUnicode(false);

            modelBuilder.Entity<VistaVirtualPersonalizacion>()
                .HasMany(e => e.VistaVirtual)
                .WithOne(e => e.VistaVirtualPersonalizacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VistaVirtualPersonalizacion>()
                .HasMany(e => e.VistaVirtualCMS)
                .WithOne(e => e.VistaVirtualPersonalizacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VistaVirtualPersonalizacion>()
                .HasMany(e => e.VistaVirtualGadgetRecursos)
                .WithOne(e => e.VistaVirtualPersonalizacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VistaVirtualPersonalizacion>()
                .HasMany(e => e.VistaVirtualProyecto)
                .WithOne(e => e.VistaVirtualPersonalizacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VistaVirtualPersonalizacion>()
                .HasMany(e => e.VistaVirtualRecursos)
                .WithOne(e => e.VistaVirtualPersonalizacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            // Solicitud
            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.SolicitudOrganizacion)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.SolicitudUsuario)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.SolicitudNuevaOrganizacion)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitudNuevoUsuario>()
                .Property(e => e.Sexo)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.DatoExtraEcosistemaOpcionSolicitud)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.DatoExtraEcosistemaVirtuosoSolicitud)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.DatoExtraProyectoOpcionSolicitud)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.SolicitudOrganizacion)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.SolicitudGrupo)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.SolicitudUsuario)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.SolicitudNuevaOrganizacion)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitudNuevaOrganizacion>()
                .HasOne(e => e.SolicitudNuevaOrgEmp)
                .WithOne(e => e.SolicitudNuevaOrganizacion)
                .IsRequired();

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.SolicitudNuevoProfesor)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitudNuevoUsuario>()
                .Property(e => e.Sexo)
                .IsFixedLength()
                .IsUnicode(false);

            //Suscripcion

            modelBuilder.Entity<Suscripcion>()
                .HasMany(e => e.CategoriaTesVinSuscrip)
                .WithOne(e => e.Suscripcion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Suscripcion>()
                .HasMany(e => e.SuscripcionIdentidadProyecto)
                .WithOne(e => e.Suscripcion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Suscripcion>()
                .HasOne(e => e.SuscripcionTesauroOrganizacion)
                .WithOne(e => e.Suscripcion)
                .IsRequired();

            modelBuilder.Entity<Suscripcion>()
                .HasOne(e => e.SuscripcionTesauroProyecto)
                .WithOne(e => e.Suscripcion)
                .IsRequired();

            modelBuilder.Entity<Suscripcion>()
                .HasMany(e => e.SuscripcionBlog)
                .WithOne(e => e.Suscripcion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Suscripcion>()
                .HasOne(e => e.SuscripcionTesauroUsuario)
                .WithOne(e => e.Suscripcion)
                .IsRequired();

            // Identidad

            modelBuilder.Entity<Perfil>()
                .HasOne(e => e.PerfilPersona)
                .WithOne(e => e.Perfil)
                .IsRequired();

            modelBuilder.Entity<Identidad>()
                .HasMany(e => e.GrupoIdentidadesParticipacion)
                .WithOne(e => e.Identidad)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Identidad>()
                .HasOne(e => e.IdentidadContadores)
                .WithOne(e => e.Identidad)
                .IsRequired();

            modelBuilder.Entity<Identidad>()
                .HasMany(e => e.OrganizacionParticipaProyecto)
                .WithOne(e => e.Identidad)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Perfil>()
                .HasMany(e => e.Profesor)
                .WithOne(e => e.Perfil)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Identidad>()
                .HasMany(e => e.DatoExtraProyectoOpcionIdentidad)
                .WithOne(e => e.Identidad)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Perfil>()
                .HasMany(e => e.Identidad)
                .WithOne(e => e.Perfil)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Perfil>()
                .HasMany(e => e.PerfilPersonaOrg)
                .WithOne(e => e.Perfil)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //CMS
            modelBuilder.Entity<CMSBloque>()
                .HasMany(e => e.CMSBloqueComponente)
                .WithOne(e => e.CMSBloque)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CMSBloqueComponente>()
                .HasMany(e => e.CMSBloqueComponentePropiedadComponente)
                .WithOne(e => e.CMSBloqueComponente)
                .IsRequired()
                .HasForeignKey(e => new { e.BloqueID, e.ComponenteID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CMSComponente>()
                .Property(e => e.IdiomasDisponibles)
                .IsUnicode(false);

            modelBuilder.Entity<CMSComponente>()
                .Property(e => e.NombreCortoComponente)
                .IsUnicode(false);

            modelBuilder.Entity<CMSComponente>()
                .HasMany(e => e.CMSBloqueComponente)
                .WithOne(e => e.CMSComponente)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CMSComponente>()
                .HasMany(e => e.CMSPropiedadComponente)
                .WithOne(e => e.CMSComponente)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CMSComponente>()
                .HasMany(e => e.CMSComponenteRolGrupoIdentidades)
                .WithOne(e => e.CMSComponente)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CMSComponente>()
                .HasMany(e => e.CMSComponenteRolIdentidad)
                .WithOne(e => e.CMSComponente)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CMSPagina>()
                .HasMany(e => e.CMSBloque)
                .WithOne(e => e.CMSPagina)
                .IsRequired()
                .HasForeignKey(e => new { e.OrganizacionID, e.ProyectoID, e.Ubicacion })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CMSBloque>()
               .HasMany(e => e.CMSBloqueComponente)
               .WithOne(e => e.CMSBloque)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            //Sitemaps

            modelBuilder.Entity<SitemapsIndex>()
                .HasMany(e => e.Indexlist)
                .WithOne(e => e.SitemapIndex)
                .IsRequired()
                .HasForeignKey(e => new { e.Dominio });

            //Notifiacion

            modelBuilder.Entity<Notificacion>()
               .HasMany(e => e.Invitacion)
               .WithOne(e => e.Notificacion)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacion>()
                .HasOne(e => e.NotificacionEnvioMasivo)
                .WithOne(e => e.Notificacion)
                .IsRequired();

            modelBuilder.Entity<Notificacion>()
                .HasMany(e => e.NotificacionAlertaPersona)
                .WithOne(e => e.Notificacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacion>()
                .HasMany(e => e.NotificacionCorreoPersona)
                .WithOne(e => e.Notificacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacion>()
                .HasMany(e => e.NotificacionSuscripcion)
                .WithOne(e => e.Notificacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacion>()
                .HasMany(e => e.NotificacionParametro)
                .WithOne(e => e.Notificacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacion>()
                .HasMany(e => e.NotificacionParametroPersona)
                .WithOne(e => e.Notificacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacion>()
                .HasMany(e => e.NotificacionSolicitud)
                .WithOne(e => e.Notificacion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasMany(e => e.NotificacionSolicitud)
                .WithOne(e => e.Solicitud)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //SUSCRIPCION

            modelBuilder.Entity<Suscripcion>()
                .HasMany(e => e.CategoriaTesVinSuscrip)
                .WithOne(e => e.Suscripcion)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Carga>()
                .HasMany(e => e.CargaPaquete)
                .WithOne(e => e.Carga)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasOne(e => e.DocumentoLecturaAumentada)
                .WithOne(e => e.Documento)
                .IsRequired();

            modelBuilder.Entity<DocumentoLecturaAumentada>()
                .Property(e => e.TituloAumentado)
                .IsUnicode(false);

            //Cookie
            modelBuilder.Entity<CategoriaProyectoCookie>()
                .HasMany(e => e.ProyectoCookie)
                .WithOne(e => e.CategoriaProyectoCookie)
                .IsRequired()
                .HasForeignKey(e => e.CategoriaID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.CategoriaProyectoCookie)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.ProyectoID, e.OrganizacionID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasMany(e => e.ProyectoCookie)
                .WithOne(e => e.Proyecto)
                .IsRequired()
                .HasForeignKey(e => new { e.ProyectoID, e.OrganizacionID })
                .OnDelete(DeleteBehavior.Restrict);

            //string ficheroConexion = (string)UtilPeticion.ObtenerObjetoDePeticion("FicheroConexion");
            //string tipoBD = Conexion.ObtenerParametro(ficheroConexion, "config/acid/tipoBD", false);
            //if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("1"))
            //{

            //    modelBuilder.Entity<ConfiguracionBBDD>().ToTable("CONFIGURACIONBBDD");
            //    modelBuilder.Entity<ConfiguracionBBDD>().Property(config => config.NumConexion).HasColumnName("NUMCONEXION");
            //    modelBuilder.Entity<ConfiguracionBBDD>().Property(config => config.ConectionTimeout).HasColumnName("CONECTIONTIMEOUT");
            //    modelBuilder.Entity<ConfiguracionBBDD>().Property(config => config.NombreConexion).HasColumnName("NOMBRECONEXION");
            //    modelBuilder.Entity<ConfiguracionBBDD>().Property(config => config.TipoConexion).HasColumnName("TIPOCONEXION");
            //    modelBuilder.Entity<ConfiguracionBBDD>().Property(config => config.DatosExtra).HasColumnName("DATOSEXTRA");
            //    modelBuilder.Entity<ConfiguracionBBDD>().Property(config => config.EsMaster).HasColumnName("ESMASTER");
            //    modelBuilder.Entity<ConfiguracionBBDD>().Property(config => config.LecturaPermitida).HasColumnName("LECTURAPERMITIDA");
            //    modelBuilder.Entity<ConfiguracionBBDD>().Property(config => config.Conexion).HasColumnName("CONEXION");
            //}

            if (mDefaultSchema != null && mDefaultSchema.Equals("dbo"))
            {
                if (_configService.ObtenerTipoBD().Equals("2"))
                {
                    //modelBuilder.HasDefaultSchema(mDefaultSchema);
                }
            }
            else if (mDefaultSchema != null && !mDefaultSchema.Equals("dbo"))
            {
                modelBuilder.HasDefaultSchema(mDefaultSchema);
            }

        }

        public bool ContextoInicializado
        {
            get
            {
                bool? ini = (bool?)mUtilPeticion.ObtenerObjetoDePeticion("ContextoInicializado");
                if (!ini.HasValue)
                {
                    ini = false;
                }
                return ini.Value;
            }
            set
            {
                mUtilPeticion.AgregarObjetoAPeticionActual("ContextoInicializado", value);
            }
        }

        private void InicializarEntityContext()
        {
            var conexion = ObtenerConexion();
            //EntityContext [System.Runtime.CompilerServices.CallerFilePath] string memberName = ""
            string schemaDefecto = GetDafaultSchema(conexion);

            //UtilPeticion.AgregarObjetoAPeticionActual("EntityContextSinProxy", new EntityContext(conexion, schemaDefecto));
            //UsarEntityCache = true;

            EntityContext context = new EntityContext(mUtilPeticion, mLoggingService, mLoggerFactory, mDbContextOptions, _configService, mServicesUtilVirtuosoAndReplication, schemaDefecto);

            mUtilPeticion.AgregarObjetoAPeticionActual("EntityContext", context);
            //UsarEntityCache = false;
            ContextoInicializado = true;
        }

        private void InicializarEntityContextCache()
        {
            var conexion = ObtenerConexion();
            //EntityContext [System.Runtime.CompilerServices.CallerFilePath] string memberName = ""
            string schemaDefecto = GetDafaultSchema(conexion);

            //UtilPeticion.AgregarObjetoAPeticionActual("EntityContextSinProxy", new EntityContext(conexion, schemaDefecto));
            //UsarEntityCache = true;
            EntityContext context = new EntityContext(mUtilPeticion, mLoggingService, mLoggerFactory, mDbContextOptions, _configService, mServicesUtilVirtuosoAndReplication, schemaDefecto);

            //context.Configuration.LazyLoadingEnabled = false;

            mUtilPeticion.AgregarObjetoAPeticionActual("EntityContextSinProxy", context);

        }

        private string GetDafaultSchema(DbConnection pConexionMaster)
        {
            string schemaDefecto = null;

            if (BaseAD.ListaDefaultSchemaPorConexion == null)
            {
                BaseAD.ListaDefaultSchemaPorConexion = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
            }

            if (BaseAD.ListaDefaultSchemaPorConexion.ContainsKey(pConexionMaster.ConnectionString))
            {
                schemaDefecto = BaseAD.ListaDefaultSchemaPorConexion[pConexionMaster.ConnectionString];
            }
            else if (pConexionMaster is SqlConnection)
            {
                try
                {
                    DbCommand dbCommand = new SqlCommand("select SCHEMA_NAME()", (SqlConnection)pConexionMaster);

                    schemaDefecto = (string)dbCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Error al obtener el contexto por defecto de la base de datos: " + pConexionMaster.ConnectionString);
                }
                BaseAD.ListaDefaultSchemaPorConexion.TryAdd(pConexionMaster.ConnectionString, schemaDefecto);
            }
            else if (pConexionMaster is OracleConnection)
            {
                try
                {
                    DbCommand dbCommand = new OracleCommand("SELECT SYS_CONTEXT('USERENV','CURRENT_SCHEMA') FROM DUAL", (OracleConnection)pConexionMaster);
                    schemaDefecto = (string)dbCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Error al obtener el contexto por defecto de la base de datos: " + pConexionMaster.ConnectionString);
                }
                BaseAD.ListaDefaultSchemaPorConexion.TryAdd(pConexionMaster.ConnectionString, schemaDefecto);

            }

            return schemaDefecto;
        }

        /// <summary>
        /// Obtiene la conexi�n a la base de datos
        /// </summary>
        protected DbConnection ObtenerConexion()
        {
            DbConnection conexion = null;
            if (conexion == null)
            {
                conexion = (DbConnection)mUtilPeticion.ObtenerObjetoDePeticion("Conexion");

                if (conexion == null || !conexion.State.Equals(System.Data.ConnectionState.Open))
                {
                    string ficheroConexion = (string)mUtilPeticion.ObtenerObjetoDePeticion("FicheroConexion");

                    string tipoBD = _configService.ObtenerSqlConnectionString();

                    Microsoft.Practices.EnterpriseLibrary.Data.Database database = null;
                    if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("1"))
                    {
                        string connectionString = _configService.ObtenerSqlConnectionString();
                        //string connectionString = BaseAD.ObtenerCadenaConexion("", new List<string>());
                        string[] partesConexion = connectionString.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                        if (partesConexion.Length > 1 && partesConexion[1].StartsWith("TimeOut="))
                        {
                            connectionString = partesConexion[0];
                        }
                        conexion = new OracleConnection(connectionString);
                        conexion.Open();
                    }
                    else if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("2"))
                    {
                        string connectionString = _configService.ObtenerSqlConnectionString();
                        string[] partesConexion = connectionString.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                        if (partesConexion.Length > 1 && partesConexion[1].StartsWith("TimeOut="))
                        {
                            connectionString = partesConexion[0];
                        }

                        conexion = new NpgsqlConnection(connectionString);
                        conexion.Open();
                    }
                    else
                    {
                        string connectionString = _configService.ObtenerSqlConnectionString();
                        string[] partesConexion = connectionString.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                        if (partesConexion.Length > 1 && partesConexion[1].StartsWith("TimeOut="))
                        {
                            connectionString = partesConexion[0];
                        }
                        database = new SqlDatabase(connectionString);
                        conexion = database.CreateConnection();
                        conexion.Open();
                    }

                    mUtilPeticion.AgregarObjetoAPeticionActual("Conexion", conexion);
                }
            }
            return conexion;
        }

        public List<object> ObtenerElementosModificados(Type pObjeto)
        {
            return ObtenerElementosPorEstado(pObjeto, EntityState.Added, EntityState.Modified);
        }

        public bool EsElementoAniadido(object pElemento)
        {
            return Entry(pElemento).State == EntityState.Added;
        }

        public bool EsElementoModified(object pElemento)
        {
            return Entry(pElemento).State == EntityState.Modified;
        }

        public void DesvincularElemento(object pElemento)
        {
            Entry(pElemento).State = EntityState.Detached;
        }

        public void EliminarElemento(object pElemento, bool pPeticionIntegracionContinua = false)
        {
            if (EsElementoAniadido(pElemento))
            {
                DesvincularElemento(pElemento);
            }
            else if (EsElementoModified(pElemento) && !pPeticionIntegracionContinua)
            {
                string mensaje = $"Se está intentando eliminar un elemento que se ha modificado, elemento de tipo: {pElemento.GetType()}";
                throw new Exception(mensaje);
            }
            else
            {
                Entry(pElemento).State = EntityState.Deleted;
            }
        }

        public List<object> ObtenerElementosEliminados(Type pObjeto)
        {
            return ObtenerElementosPorEstado(pObjeto, EntityState.Deleted);
        }

        private List<object> ObtenerElementosPorEstado(Type pObjeto, params EntityState[] pEstados)
        {
            return ChangeTracker.Entries().Where(x => pEstados.Contains(x.State) && x.Entity.GetType().Equals(pObjeto)).Select(x => x.Entity).ToList();
        }

        public T ObtenerValorOriginalDeObjeto<T>(object pObjeto, string pPropiedad)
        {
            if (Entry(pObjeto).State == EntityState.Modified)
            {
                return Entry(pObjeto).OriginalValues.GetValue<T>(pPropiedad);
            }
            else if (Entry(pObjeto).State == EntityState.Added)
            {
                return Entry(pObjeto).CurrentValues.GetValue<T>(pPropiedad);
            }
            else
            {
                return default(T);
            }
        }

        public void AceptarCambios(object entrada)
        {
            Entry(entrada).State = EntityState.Unchanged;
        }
    }
    [Serializable]
    [Table("TokenApiLogin")]
    public partial class TokenApiLogin
    {
        [Key]
        public Guid Token { get; set; }

        [Required]
        [StringLength(255)]
        public string Login { get; set; }

        public DateTime Fecha { get; set; }
    }

    [Serializable]
    [Table("ParametroAplicacion")]
    public class ParametroAplicacion
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string Parametro { get; set; }

        [Column(Order = 1)]
        [StringLength(1000)]
        public string Valor { get; set; }

        public ParametroAplicacion()
        {

        }
        public ParametroAplicacion(string parametro, string valor)
        {
            this.Parametro = parametro;
            this.Valor = valor;
        }
    }
    [Serializable]
    [Table("ConfiguracionServiciosDominio")]
    public class ConfiguracionServiciosDominio
    {
        [Key]
        [Column(Order = 0)]
        public Int16 NumServicio { get; set; }

        [Column(Order = 1)]
        [Required]
        [StringLength(100)]
        public string Dominio { get; set; }

        [Column(Order = 2)]
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Column(Order = 3)]
        [Required]
        [StringLength(1000)]
        public string Url { get; set; }
        public ConfiguracionServiciosDominio()
        {

        }

    }
    [Serializable]
    [Table("ConfiguracionServicios")]
    public class ConfiguracionServicios
    {
        [Key]
        [Column(Order = 0)]
        public Int16 NumServicio { get; set; }

        [Column(Order = 2)]
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Column(Order = 3)]
        [Required]
        [StringLength(1000)]
        public string Url { get; set; }
        public ConfiguracionServicios()
        {

        }

    }
    [Serializable]
    [Table("ConfiguracionServiciosProyecto")]
    public class ConfiguracionServiciosProyecto
    {
        [Key]
        [Column(Order = 0)]
        public Int16 NumServicio { get; set; }

        [Column(Order = 1)]
        [Required]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Column(Order = 3)]
        [Required]
        [StringLength(1000)]
        public string Url { get; set; }

        public ConfiguracionServiciosProyecto()
        {

        }

    }

    [Serializable]
    [Table("ConfiguracionBBDD")]
    public class ConfiguracionBBDD
    {
        [Key]
        [Column(Order = 0)]
        public short NumConexion { get; set; }

        [Column(Order = 1)]
        [Required]
        [StringLength(100)]
        public string NombreConexion { get; set; }

        [Column(Order = 2)]
        [Required]
        public short TipoConexion { get; set; }

        [Column(Order = 3)]
        [Required]
        [StringLength(1000)]
        public string Conexion { get; set; }

        [Column(Order = 4)]
        [StringLength(100)]
        public string DatosExtra { get; set; }

        [Column(Order = 5)]
        [Required]
        public bool EsMaster { get; set; }

        [Column(Order = 6)]
        [Required]
        public bool LecturaPermitida { get; set; }

        [Column(Order = 7)]
        public int? ConectionTimeout { get; set; }

        public ConfiguracionBBDD()
        {

        }


    }
    [Serializable]
    [Table("AccionesExternas")]
    public class AccionesExternas
    {
        [Key]
        [Column(Order = 0)]
        public short TipoAccion { get; set; }

        [Column(Order = 1)]
        [Required]
        public string URL { get; set; }

        public AccionesExternas()
        {

        }

    }
    [Serializable]
    [Table("ConfigApplicationInsightsDominio")]
    public class ConfigApplicationInsightsDominio
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(250)]
        public string Dominio { get; set; }

        [Column(Order = 1)]

        public Guid ImplementationKey { get; set; }

        [Column(Order = 2)]
        public int UbicacionLogs { get; set; }

        [Column(Order = 3)]
        public int UbicacionTrazas { get; set; }

        public ConfigApplicationInsightsDominio()
        {

        }

    }
    [Serializable]
    [Table("TextosPersonalizadosPlataforma")]
    public class TextosPersonalizadosPlataforma
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        [Required]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(100)]
        [Required]
        public string TextoID { get; set; }

        [Column(Order = 3)]
        [StringLength(10)]
        public string Language { get; set; }

        [Column(Order = 4)]
        public string Texto { get; set; }

        public TextosPersonalizadosPlataforma()
        {
        }

    }
    [Serializable]
    [Table("ProyectoSinRegistroObligatorio")]
    public class ProyectoSinRegistroObligatorio
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid OrganizacionSinRegistroID { get; set; }

        [Column(Order = 3)]
        public Guid ProyectoSinRegistroID { get; set; }

        public virtual ProyectoRegistroObligatorio ProyectoRegistroObligatorio { get; set; }
        public ProyectoSinRegistroObligatorio()
        {

        }
        public ProyectoSinRegistroObligatorio(Guid organizacionSinRegistroID, Guid proyectoSinRegistroID, Guid organizacionRegistroID, Guid proyectoRegistroID)
        {
            this.OrganizacionSinRegistroID = organizacionRegistroID;
            this.ProyectoSinRegistroID = proyectoSinRegistroID;
            this.ProyectoRegistroObligatorio.OrganizacionID = organizacionRegistroID;
            this.ProyectoRegistroObligatorio.ProyectoID = proyectoRegistroID;

        }

    }
    [Serializable]
    [Table("ProyectoRegistroObligatorio")]
    public class ProyectoRegistroObligatorio
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        [Required]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [Required]
        public short VisibilidadUsuariosActivos { get; set; }

        public ICollection<ProyectoSinRegistroObligatorio> ListaProyectoSinRegistroObligatorio { get; set; }

        public ProyectoRegistroObligatorio()
        {
            ListaProyectoSinRegistroObligatorio = new HashSet<ProyectoSinRegistroObligatorio>();
        }
    }


}
