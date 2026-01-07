using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Blog;
using Es.Riam.Gnoss.AD.EntityModel.Models.Carga;
using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Comentario;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.MVC;
using Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Es.Riam.Gnoss.AD.EntityModel
{
    public class EntityContextOracle : EntityContext
    {
        public EntityContextOracle(UtilPeticion utilPeticion, LoggingService loggingService, ILogger<EntityContextOracle> logger, ILoggerFactory loggerFactory, DbContextOptions<EntityContext> dbContextOptions, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, bool pCache = false)
            : base(utilPeticion, loggingService, loggerFactory, dbContextOptions, configService, servicesUtilVirtuosoAndReplication, pCache)
        {

        }

        public EntityContextOracle(UtilPeticion utilPeticion, LoggingService loggingService, ILogger<EntityContextOracle> logger, ILoggerFactory loggerFactory, DbContextOptions<EntityContext> dbContextOptions, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, string pDefaultSchema = null, bool pCache = false)
            : base(utilPeticion, loggingService, loggerFactory, dbContextOptions, configService, servicesUtilVirtuosoAndReplication, pDefaultSchema, pCache)
        {            
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Proyecto>()
                .Property(e => e.EsProyectoDestacado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Proyecto>()
               .Property(e => e.TieneTwitter)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Proyecto>()
               .Property(e => e.EnviarTwitterComentario)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Proyecto>()
               .Property(e => e.EnviarTwitterNuevaCat)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Proyecto>()
               .Property(e => e.EnviarTwitterNuevoAdmin)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Proyecto>()
               .Property(e => e.EnviarTwitterNuevaPolitCert)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Proyecto>()
               .Property(e => e.EnviarTwitterNuevoTipoDoc)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Proyecto>()
               .Property(e => e.TagTwitterGnoss)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Organizacion>()
               .Property(e => e.EsBuscable)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Organizacion>()
               .Property(e => e.EsBuscableExternos)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Organizacion>()
               .Property(e => e.ModoPersonal)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Organizacion>()
               .Property(e => e.Eliminada)
               .HasPrecision(1)
               .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Amigo>()
                .Property(e => e.EsFanMutuo)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Blog>()
                .Property(e => e.PermitirComentarios)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Blog>()
                .Property(e => e.PermitirTrackbacks)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Blog>()
                .Property(e => e.CrearFuentesWeb)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Blog>()
                .Property(e => e.VisibilidadListadosBusquedas)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Blog>()
                .Property(e => e.VisibilidadBuscadoresWeb)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Blog>()
                .Property(e => e.Eliminado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Blog>()
                .Property(e => e.PermiteActualizarTwitter)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CMSComponente>()
                .Property(e => e.Activo)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CMSComponente>()
                .Property(e => e.AccesoPublico)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CMSPagina>()
                .Property(e => e.Activa)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CMSPagina>()
                .Property(e => e.MostrarSoloCuerpo)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Models.Comentario.Comentario>()
                .Property(e => e.Eliminado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Models.ComparticionAutomatica.ComparticionAutomatica>()
                .Property(e => e.Eliminada)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Models.ComparticionAutomatica.ComparticionAutomatica>()
                .Property(e => e.ActualizarHome)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionAmbitoBusqueda>()
                .Property(e => e.Metabusqueda)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionAmbitoBusqueda>()
                .Property(e => e.TodoGnoss)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionAmbitoBusquedaProyecto>()
                .Property(e => e.Metabusqueda)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionAmbitoBusquedaProyecto>()
                .Property(e => e.TodoGnoss)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionBBDD>()
                .Property(e => e.EsMaster)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionBBDD>()
                .Property(e => e.LecturaPermitida)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionEnvioCorreo>()
                .Property(e => e.SSL)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CorreoInterno>()
                .Property(e => e.Eliminado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CorreoInterno>()
                .Property(e => e.EnPapelera)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Curriculum>()
                .Property(e => e.UsaDatosPersonalesPerfil)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Curriculum>()
                .Property(e => e.Publicado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraEcosistema>()
                .Property(e => e.Obligatorio)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraEcosistema>()
                .Property(e => e.Paso1Registro)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraEcosistemaVirtuoso>()
                .Property(e => e.Obligatorio)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraEcosistemaVirtuoso>()
                .Property(e => e.Paso1Registro)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraEcosistemaVirtuoso>()
                .Property(e => e.VisibilidadFichaPerfil)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraProyecto>()
                .Property(e => e.Obligatorio)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraProyecto>()
                .Property(e => e.Paso1Registro)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraProyectoVirtuoso>()
                .Property(e => e.Obligatorio)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraProyectoVirtuoso>()
                .Property(e => e.Paso1Registro)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DatoExtraProyectoVirtuoso>()
                .Property(e => e.VisibilidadFichaPerfil)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Documento>()
                .Property(e => e.CompartirPermitido)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Documento>()
                .Property(e => e.Publico)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Documento>()
                .Property(e => e.Borrador)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Documento>()
                .Property(e => e.CreadorEsAutor)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Documento>()
                .Property(e => e.Protegido)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Documento>()
                .Property(e => e.PrivadoEditores)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Documento>()
                .Property(e => e.UltimaVersion)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Documento>()
                .Property(e => e.Eliminado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoEnvioNewsLetter>()
                .Property(e => e.EnvioSolicitado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoEnvioNewsLetter>()
                .Property(e => e.EnvioRealizado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoGrupoUsuario>()
                .Property(e => e.Editor)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<EntradaBlog>()
                .Property(e => e.Eliminado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<EntradaBlog>()
                .Property(e => e.Borrador)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaEntidadesExternas>()
                .Property(e => e.EsEntidadSecundaria)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaEntidadesExternas>()
                .Property(e => e.BuscarConRecursividad)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaHome>()
                .Property(e => e.MostrarVerMas)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimiento>()
                .Property(e => e.Autocompletar)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimiento>()
                .Property(e => e.EsSemantica)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimiento>()
                .Property(e => e.EsPorDefecto)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimiento>()
                .Property(e => e.ComportamientoOr)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<GrupoAmigos>()
                .Property(e => e.Automatico)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<GrupoIdentidades>()
                .Property(e => e.Publico)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<GrupoIdentidades>()
                .Property(e => e.PermitirEnviarMensajes)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<IntegracionContinuaPropiedad>()
                .Property(e => e.MismoValor)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<IntegracionContinuaPropiedad>()
                .Property(e => e.Revisada)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<OntologiaProyecto>()
                .Property(e => e.CachearDatosSemanticos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<OntologiaProyecto>()
                .Property(e => e.EsBuscable)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.WikiDisponible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.BaseRecursosDisponible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.CompartirRecursosPermitido)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.InvitacionesDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.ServicioSuscripcionDisp)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.BlogsDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.ForosDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.EncuestasDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.VotacionesDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.ComentariosDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PreguntasDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.DebatesDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.BrightcoveDisponible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.EntidadRevisadaObligatoria)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PermitirRevisionManualPro)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PermitirRevisionManualGF)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PermitirRevisionManualObj)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PermitirRevisionManualComp)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PermitirCertificacionRec)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.DafoDisponible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PlantillaDisponible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.VerVotaciones)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.ClausulasRegistro)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.OcultarPersonalizacion)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PestanyaRecursosVisible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.ImagenRelacionadosMini)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.EsBeta)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.GadgetsPieDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.GadgetsCabeceraDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.BiosCortas)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.RssDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.RdfDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.RegDidactalia)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.HomeVisible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.CargasMasivasDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.ComunidadGNOSS)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.IdiomasDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.SupervisoresAdminGrupos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.FechaNacimientoObligatoria)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PrivacidadObligatoria)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.EventosDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.SolicitarCoockieLogin)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.CMSDisponible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.InvitacionesPorContactoDisponibles)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PermitirUsuNoLoginDescargDoc)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.AvisoCookie)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.MostrarPersonasEnCatalogo)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.EnvioMensajesPermitido)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.TieneSitemapComunidad)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PermitirVotacionesNegativas)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.MostrarAccionesEnListados)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PalcoActivado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.PermitirRecursosPrivados)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ParametroGeneral>()
                .Property(e => e.ChatDisponible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<PermisoGrupoAmigoOrg>()
                .Property(e => e.PermisoEdicion)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<PersonaOcupacionFigura>()
                .Property(e => e.EsPropietarioFigura)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoConfigExtraSem>()
                .Property(e => e.Editable)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoElementoHtml>()
                .Property(e => e.CargarSinAceptarCookies)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoElementoHtml>()
                .Property(e => e.Privado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoEvento>()
                .Property(e => e.Activo)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoEvento>()
                .Property(e => e.Interno)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoGadgetContexto>()
                .Property(e => e.MostrarEnlaceOriginal)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoGadgetContexto>()
                .Property(e => e.OcultarVerMas)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoGadgetContexto>()
                .Property(e => e.NuevaPestanya)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoGadgetContexto>()
                .Property(e => e.ObtenerPrivados)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.EsRutaInterna)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.EsSemantica)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.NuevaPestanya)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.MostrarFacetas)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.MostrarCajaBusqueda)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.Visible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.CMS)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.OcultarResultadosSinFiltros)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanya>()
                .Property(e => e.GruposPorTipo)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ResultadoSuscripcion>()
                .Property(e => e.Leido)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ResultadoSuscripcion>()
                .Property(e => e.Sincaducidad)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Models.Suscripcion.Suscripcion>()
                .Property(e => e.Bloqueada)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Usuario>()
                .Property(e => e.EstaBloqueado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Usuario>()
                .Property(e => e.TwoFactorAuthentication)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<BlogComunidad>()
                .Property(e => e.Compartido)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CargaPaquete>()
                .Property(e => e.Comprimido)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CMSBloque>()
                .Property(e => e.Borrador)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoLecturaAumentada>()
                .Property(e => e.Validada)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoRolGrupoIdentidades>()
                .Property(e => e.Editor)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoRolIdentidad>()
                .Property(e => e.Editor)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.Autocompletar)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.MostrarSoloCaja)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.Oculta)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.EsSemantica)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.Excluyente)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.ComportamientoOr)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.OcultaEnFacetas)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.OcultaEnFiltros)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.PriorizarOrdenResultados)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.Inmutable)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<FacetaObjetoConocimientoProyecto>()
                .Property(e => e.MostrarContador)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<NotificacionCorreoPersona>()
                .Property(e => e.EnviadoRabbit)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossOrg>()
                .Property(e => e.VerRecursos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossOrg>()
                .Property(e => e.VerRecursosExterno)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<PersonaVinculoOrganizacion>()
                .Property(e => e.UsarFotoPersonal)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CategoriaProyectoCookie>()
                .Property(e => e.EsCategoriaTecnica)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoGadget>()
                .Property(e => e.Visible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoGadget>()
                .Property(e => e.MultiIdioma)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoGadget>()
                .Property(e => e.CargarPorAjax)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPasoRegistro>()
                .Property(e => e.Obligatorio)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .Property(e => e.NuevaPestanya)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .Property(e => e.Visible)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .Property(e => e.VisibleSinAcceso)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .Property(e => e.Activa)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoSearchPersonalizado>()
                .Property(e => e.OmitirRdfType)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<RedireccionValorParametro>()
                .Property(e => e.MantenerFiltros)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<SolicitudNuevaOrganizacion>()
                .Property(e => e.EsBuscable)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<SolicitudNuevaOrganizacion>()
                .Property(e => e.EsBuscableExternos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<SolicitudNuevaOrganizacion>()
                .Property(e => e.ModoPersonal)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<SolicitudNuevoUsuario>()
                .Property(e => e.EsBuscable)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<SolicitudNuevoUsuario>()
                .Property(e => e.EsBuscableExterno)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<SolicitudNuevoUsuario>()
                .Property(e => e.CrearClase)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<SolicitudNuevoUsuario>()
                .Property(e => e.CambioPassword)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<SolicitudNuevoUsuario>()
                .Property(e => e.FaltanDatos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<CategoriaTesauro>()
                .Property(e => e.TieneFoto)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Persona>()
                .Property(e => e.EsBuscable)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Persona>()
                .Property(e => e.EsBuscableExternos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Persona>()
                .Property(e => e.Eliminado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoRolUsuario>()
                .Property(e => e.EstaBloqueado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoCookie>()
                .Property(e => e.EsEditable)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
                .Property(e => e.Compartido)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
                .Property(e => e.Eliminado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
                .Property(e => e.IdentPubVisibleExt)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
                .Property(e => e.PermiteComentarios)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
                .Property(e => e.IndexarRecurso)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
                .Property(e => e.PrivadoEditores)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<DocumentoWebVinBaseRecursos>()
                .Property(e => e.LinkAComunidadOrigen)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .Property(e => e.MostrarFacetas)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .Property(e => e.MostrarCajaBusqueda)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .Property(e => e.OcultarResultadosSinFiltros)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .Property(e => e.GruposPorTipo)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .Property(e => e.MostrarEnComboBusqueda)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .Property(e => e.IgnorarPrivacidadEnBusqueda)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyectoPestanyaBusqueda>()
                .Property(e => e.OmitirCargaInicialFacetasResultados)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.SolicitudesContacto)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.MensajesGnoss)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.ComentariosRecursos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.InvitacionComunidad)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.InvitacionOrganizacion)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.VerAmigos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.VerAmigosExterno)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.VerRecursos)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.VerRecursosExterno)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ConfiguracionGnossPersona>()
                .Property(e => e.EnviarEnlaces)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Perfil>()
                .Property(e => e.Eliminado)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Perfil>()
                .Property(e => e.TieneTwitter)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<ProyRolUsuClausulaReg>()
                .Property(e => e.Valor)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Models.IdentidadDS.Identidad>()
                .Property(e => e.RecibirNewsLetter)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Models.IdentidadDS.Identidad>()
                .Property(e => e.MostrarBienvenida)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Models.IdentidadDS.Identidad>()
                .Property(e => e.ActivoEnComunidad)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Models.IdentidadDS.Identidad>()
                .Property(e => e.ActualizaHome)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<OrganizacionParticipaProy>()
                .Property(e => e.EstaBloqueada)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<VersionDocumento>()
                .Property(e => e.EsMejora)
                .HasPrecision(1)
                .HasColumnType("NUMBER(1)");

            modelBuilder.Entity<Estado>()
                .Property(e => e.PermiteMejora)
                .HasPrecision(1)
                .HasColumnType("NUMBER(2)");
        }
    }
}
