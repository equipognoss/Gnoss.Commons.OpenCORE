using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cache;
using Es.Riam.Gnoss.AD.EntityModel.Models.Carga;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;


namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    #region Enumeraciones

    /// <summary>
    /// Enumeraci?n para distinguir tipos de proyectos
    /// </summary>
    public enum TipoProyecto
    {
        /// <summary>
        /// Proyecto de organizaci?n
        /// </summary>
        DeOrganizacion = 0,
        /// <summary>
        /// Comunidad web
        /// </summary>
        Comunidad = 1,
        /// <summary>
        /// Metacomunidad
        /// </summary>
        MetaComunidad = 2,
        /// <summary>
        /// Universidad 2.0
        /// </summary>
        Universidad20 = 3,
        /// <summary>
        /// Educacion Expandida
        /// </summary>
        EducacionExpandida = 4,
        /// <summary>
        /// Educacion Expandida
        /// </summary>
        Catalogo = 5,
        /// <summary>
        /// Educaci?n primaria
        /// </summary>
        EducacionPrimaria = 6,
        /// <summary>
        /// Catalogo no social publico con un unico tipo de recurso
        /// </summary>
        CatalogoNoSocialConUnTipoDeRecurso = 7,
        /// <summary>
        /// Catalogo no social
        /// </summary>
        CatalogoNoSocial = 8
    }

    public class SectionAttribute : Attribute
    {
        public string Nombre { get; set; }
		public SectionAttribute(string name)
		{
			this.Nombre = name;
		}
	}

    public static class TipoProyectoExt
    {
        public static string ToFriendlyString(this TipoProyecto tipoProyecto)
        {
            switch (tipoProyecto)
            {
                case TipoProyecto.Catalogo:
                    return "catalog";
                case TipoProyecto.CatalogoNoSocial:
                    return "not social catalog";
                case TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso:
                    return "one type resource not social catalog";
                case TipoProyecto.Comunidad:
                    return "community";
                case TipoProyecto.DeOrganizacion:
                    return "gnoss project";
                case TipoProyecto.EducacionExpandida:
                    return "secondary education";
                case TipoProyecto.EducacionPrimaria:
                    return "primary education";
                case TipoProyecto.MetaComunidad:
                    return "meta community";
                case TipoProyecto.Universidad20:
                    return "university";
                default:
                    return tipoProyecto.ToString();
            }
        }
    }

    /// <summary>
    /// Enumeraci?n para distinguir tipos de acceso
    /// </summary>
    public enum TipoAcceso
    {
        /// <summary>
        /// Proyecto p?blico
        /// </summary>
        Publico = 0,
        /// <summary>
        /// Proyecto privado
        /// </summary>
        Privado = 1,
        /// <summary>
        /// Proyecto restringido
        /// </summary>
        Restringido = 2,
        /// <summary>
        /// Proyecto reservado
        /// </summary>
        Reservado = 3
    }

    /// <summary>
    /// Enumeraci?n para distinguir estados de un proyecto
    /// </summary>
    public enum EstadoProyecto
    {
        /// <summary>
        /// Proyecto bloqueado/cerrado
        /// </summary>
        Cerrado = 0,
        /// <summary>
        /// Proyecto bloqueado temporalmente x mantenimiento
        /// </summary>
        CerradoTemporalmente = 1,
        /// <summary>
        /// Proyecto en definicion
        /// </summary>
        Definicion = 2,
        /// <summary>
        /// Proyecto abierto
        /// </summary>
        Abierto = 3,
        /// <summary>
        /// Proyecto que se esta cerrando y esta unos dias en estado de gracia
        /// </summary>
        Cerrandose = 4
    }

    /// <summary>
    /// Enumeraci?n para distinguir tipos administradores/supervisores/usuarios del proyecto
    /// </summary>
    public enum TipoRolUsuario
    {
        /// <summary>
        /// Administrador
        /// </summary>
        Administrador = 0,
        /// <summary>
        /// Supervisor
        /// </summary>
        Supervisor = 1,
        /// <summary>
        /// Usuario
        /// </summary>
        Usuario = 2,
        /// <summary>
        /// Dise?ador
        /// </summary>
        Diseniador = 3
    }

    public enum TipoDePermiso
    {
        Comunidad,
        Contenidos,
        Recursos,
        Ecosistema
    }

	public enum PermisoComunidad : ulong
	{
		[Description("DESCPERMISOINFOGENERAL")]
        [Section("COMUNIDAD")]
		GestionarInformacionGeneral = 1,
        [Description("DESCPERMISOFLUJOS")]
        [Section("COMUNIDAD")]
        GestionarFlujos = 2,
        [Description("DESCPERMISOINTERACCIONESSOCIALES")]
		[Section("COMUNIDAD")]
		GestionarInteraccionesSociales = 4,
		[Description("DESCPERMISOMIEMBROS")]
		[Section("COMUNIDAD")]
		GestionarMiembros = 8,
		[Description("DESCPERMISOSOLICITUDESGRUPO")]
		[Section("COMUNIDAD")]
		GestionarSolicitudesDeAccesoAGrupo = 16,
		[Description("DESCPERMISONIVELESCERTIFICACION")]
		[Section("COMUNIDAD")]
		GestionarNivelesDeCertificacion = 32,
		[Description("DESCPERMISOPESOSAUTOCOMPLETAR")]
		[Section("ESTRUCTURA")]
		GestionarPesosAutocompletado = 64,
		[Description("DESCPERMISOREDIRECCIONES")]
		[Section("ESTRUCTURA")]
		GestionarRedirecciones = 128,
		[Description("DESCPERMISOOAUTH")]
		[Section("CONFIGURACION")]
		DescargarConfiguracionOAuth = 256,
		[Description("DESCPERMISOCOOKIES")]
		[Section("CONFIGURACION")]
		GestionarCookies = 512,
		[Description("DESCPERMISOFTP")]
		[Section("CONFIGURACION")]
		AccederAlFTP = 1024,
		[Description("DESCPERMISOTRADUCCIONES")]
		[Section("CONFIGURACION")]
		GestionarTraducciones = 2048,
		[Description("DESCPERMISODATOSEXTRA")]
		[Section("CONFIGURACION")]
		GestionarDatosExtraRegistro = 4096,
		[Description("DESCPERMISOTRAZAS")]
		[Section("CONFIGURACION")]
		GestionarTrazas = 8192,
		[Description("DESCPERMISOCONFIGURACIONES")]
		[Section("CONFIGURACION")]
		GestionarConfiguraciones = 16384,
		[Description("DESCPERMISOCACHE")]
		[Section("CONFIGURACION")]
		GestionarCache = 32768,
		[Description("DESCPERMISOSEO")]
		[Section("CONFIGURACION")]
		AdministrarSEOYGoogleAnalytics = 65536,
		[Description("DESCPERMISOESTADISTICAS")]
		[Section("CONFIGURACION")]
		AccederAEstadisticasDeLaComunidad = 131072,
		[Description("DESCPERMISOCLAUSULAS")]
		[Section("CONFIGURACION")]
		GestionarClausulasDeRegistro = 262144,
		[Description("DESCPERMISOCORREO")]
		[Section("CONFIGURACION")]
		GestionarBuzonDeCorreo = 524288,
		[Description("DESCPERMISOSERVICIOSEXTERNOS")]
		[Section("CONFIGURACION")]
		GestionarServiciosExternos = 1048576,
		[Description("DESCPERMISOESTADOSERVICIOS")]
		[Section("CONFIGURACION")]
		AccederAlEstadoDeLosServicios = 2097152,
		[Description("DESCPERMISOOPCIONESMETA")]
		[Section("CONFIGURACION")]
		GestionarOpcionesDelMetaadministrador = 4194304,
		[Description("DESCPERMISOEVENTOS")]
		[Section("CONFIGURACION")]
		GestionarEventosExternos = 8388608,
		[Description("DESCPERMISOSPARQL")]
		[Section("GRAFO")]
		AccesoSparqlEndpoint = 16777216,
		[Description("DESCPERMISOCARGAMASIVA")]
		[Section("GRAFO")]
		ConsultarCargasMasivas = 33554432,
		[Description("DESCPERMISOBORRADOMASIVO")]
		[Section("GRAFO")]
		EjecutarBorradoMasivo = 67108864,
		[Description("DESCPERMISOSUGERENCIASBUSQUEDA")]
		[Section("GRAFO")]
		GestionarSugerenciasDeBusqueda = 134217728,
		[Description("DESCPERMISOCONTEXTOS")]
		[Section("GRAFO")]
		GestionarInformacionContextual = 268435456,
		[Description("DESCPERMISOSEARCHPERSONALIZADO")]
		[Section("DESCUBRIMIENTO")]
		GestionarParametrosDeBusquedaPersonalizados = 536870912,
		[Description("DESCPERMISOMAPA")]
		[Section("DESCUBRIMIENTO")]
		GestionarMapa = 1073741824,
		[Description("DESCPERMISOGRAFICOS")]
		[Section("DESCUBRIMIENTO")]
		AdministrarGraficos = 2147483648,
		[Description("DESCPERMISOVISTAS")]
		[Section("APARIENCIA")]
		GestionarVistas = 4294967296,
		[Description("DESCPERMISOIC")]
		[Section("IC")]
		GestionarIntegracionContinua = 8589934592,
		[Description("DESCPERMISOREPROCESAR")]
		[Section("MANTENIMIENTO")]
		EjecutarReprocesadosDeRecursos = 17179869184,
		[Description("DESCPERMISOAPLICACIONES")]
		[Section("APLICACIONES")]
		GestionarAplicacionesEspecificas = 34359738368,
		[Description("DESCPERMISOROLES")]
		[Section("COMUNIDAD")]
		GestionarRolesYPermisos = 68719476736
    }

	public enum PermisoContenidos : ulong
	{
		[Description("DESCPERMISOVERCATEGORIA")]
		[Section("COMUNIDAD")]
		VerCategorias = 1,
		[Description("DESCPERMISOANYADIRCATEGORIA")]
		[Section("COMUNIDAD")]
		AnyadirCategoria = 2,
		[Description("DESCPERMISOEDITARCATEGORIA")]
		[Section("COMUNIDAD")]
		ModificarCategoria = 4,
		[Description("DESCPERMISOELIMINARCATEGORIA")]
		[Section("COMUNIDAD")]
		EliminarCategoria = 8,

		[Description("DESCPERMISOVERPAGINA")]
		[Section("ESTRUCTURA")]
		VerPagina = 16,
		[Description("DESCPERMISOCREARPAGINA")]
		[Section("ESTRUCTURA")]
		CrearPagina = 32,
		[Description("DESCPERMISOPUBLICARPAGINA")]
		[Section("ESTRUCTURA")]
		PublicarPagina = 64,
		[Description("DESCPERMISOEDITARPAGINA")]
		[Section("ESTRUCTURA")]
		EditarPagina = 128,
		[Description("DESCPERMISOELIMINARPAGINA")]
		[Section("ESTRUCTURA")]
		EliminarPagina = 256,

		[Description("DESCPERMISOVERCMS")]
		[Section("ESTRUCTURA")]
		VerComponenteCMS = 512,
		[Description("DESCPERMISOCREARCMS")]
		[Section("ESTRUCTURA")]
		CrearComponenteCMS = 1024,
		[Description("DESCPERMISOEDITARCMS")]
		[Section("ESTRUCTURA")]
		EditarComponenteCMS = 2048,
		[Description("DESCPERMISOELIMINARCMS")]
		[Section("ESTRUCTURA")]
		EliminarComponenteCMS = 4096,
		[Description("DESCPERMISOMULTIMEDIACMS")]
		[Section("ESTRUCTURA")]
		GestionarMultimediaCMS = 8192,

		[Description("DESCPERMISOGESTIONAROC")]
		[Section("GRAFO")]
		GestionarOC = 16384,
		[Description("DESCPERMISOANYADIRSECUNDARIA")]
		[Section("GRAFO")]
		AnyadirValorEntidadSecundaria = 32768,
		[Description("DESCPERMISOMODIFICARSECUNDARIA")]
		[Section("GRAFO")]
		ModificarValorEntidadSecundaria = 65536,
		[Description("DESCPERMISOELIMINARSECUNDARIA")]
		[Section("GRAFO")]
		EliminarValorEntidadSecundaria = 131072,

		[Description("DESCPERMISOVERTESAURO")]
		[Section("GRAFO")]
		VerTesauroSemantico = 262144,
		[Description("DESCPERMISOANYADIRTESAURO")]
		[Section("GRAFO")]
		AnyadirValorTesauro = 524288,
		[Description("DESCPERMISOMODIFICARTESAURO")]
		[Section("GRAFO")]
		ModificarValorTesauro = 1048576,
		[Description("DESCPERMISOELIMINARTESAURO")]
		[Section("GRAFO")]
		EliminarValorTesauro = 2097152,

		[Description("DESCPERMISOVERFACETA")]
		[Section("DESCUBRIMIENTO")]
		VerFaceta = 4194304,
		[Description("DESCPERMISOCREARFACETA")]
		[Section("DESCUBRIMIENTO")]
		CrearFaceta = 8388608,
		[Description("DESCPERMISOMODIFICARFACETA")]
		[Section("DESCUBRIMIENTO")]
		ModificarFaceta = 16777216,
		[Description("DESCPERMISOELIMINARFACETA")]
		[Section("DESCUBRIMIENTO")]
		EliminarFaceta = 33554432,

		[Description("DESCPERMISORESTAURARVERSIONCMS")]
		[Section("ESTRUCTURA")]
		RestaurarVersionCMS = 67108864,
		[Description("DESCPERMISOELIMINARVERSIONCMS")]
		[Section("ESTRUCTURA")]
		EliminarVersionCMS = 134217728,

		[Description("DESCPERMISORESTAURARVERSIONPAGINA")]
		[Section("ESTRUCTURA")]
		RestaurarVersionPagina = 268435456,
		[Description("DESCPERMISOELIMINARVERSIONPAGINA")]
		[Section("ESTRUCTURA")]
		EliminarVersionPagina = 536870912
	}

    public enum PermisoEcosistema : ulong
    {
		[Description("DESCPERMISOECOSISTEMATRADUCCIONES")]
		[Section("ECOSISTEMA")]
		GestionarTraduccionesEcosistema = 1,
		[Description("DESCPERMISOECOSISTEMADATOSEXTRA")]
		[Section("ECOSISTEMA")]
		GestionarDatosExtraRegistroEcosistema = 2,
		[Description("DESCPERMISOECOSISTEMACORREO")]
		[Section("ECOSISTEMA")]
		GestionarBuzonDeCorreoEcosistema = 4,
		[Description("DESCPERMISOECOSISTEMAEVENTOS")]
		[Section("ECOSISTEMA")]
		GestionarEventosExternosEcosistema = 8,
		[Description("DESCPERMISOECOSISTEMACATEGORIAS")]
		[Section("ECOSISTEMA")]
		GestionarCategoriasDePlataforma = 16,
		[Description("DESCPERMISOECOSISTEMACONFIGURACION")]
		[Section("ECOSISTEMA")]
		GestionarLaConfiguracionPlataforma = 32,
		[Description("DESCPERMISOECOSISTEMASHAREPOINT")]
		[Section("ECOSISTEMA")]
		ConfiguracionDeSharePoint = 64,
		[Description("DESCPERMISOECOSISTEMAVISTAS")]
		[Section("ECOSISTEMA")]
		GestionarVistasEcosistema = 128,
		[Description("DESCPERMISOECOSISTEMAIC")]
		[Section("ECOSISTEMA")]
		AdministrarIntegracionContinua = 256,
		[Description("DESCPERMISOECOSISTEMASOLICITUDES")]
		[Section("ECOSISTEMA")]
		AdministrarSolicitudesComunidad = 512,
		[Description("DESCPERMISOECOSISTEMAROLES")]
		[Section("ECOSISTEMA")]
		GestionarRolesYPermisosEcosistema = 1024,
		[Description("DESCPERMISOECOSISTEMAMIEMBROS")]
		[Section("ECOSISTEMA")]
		AdministrarMiembrosEcosistema = 2048
	}

    public enum PermisoRecursos : ulong
    {
		[Description("DESCPERMISOCREARADJUNTO")]
		[Section("RECURSOS")]
		CrearRecursoTipoAdjunto = 1,
		[Description("DESCPERMISOEDITARADJUNTO")]
		[Section("RECURSOS")]
		EditarRecursoTipoAdjunto = 2,
		[Description("DESCPERMISOELIMINARADJUNTO")]
		[Section("RECURSOS")]
		EliminarRecursoTipoAdjunto = 4,

		[Description("DESCPERMISOCREARREFERENCIA")]
		[Section("RECURSOS")]
		CrearRecursoTipoReferenciaADocumentoFisico = 8,
		[Description("DESCPERMISOEDITARREFERENCIA")]
		[Section("RECURSOS")]
		EditarRecursoTipoReferenciaADocumentoFisico = 16,
		[Description("DESCPERMISOELIMINARREFERNCIA")]
		[Section("RECURSOS")]
		EliminarRecursoTipoReferenciaADocumentoFisico = 32,

		[Description("DESCPERMISOCREARENLACE")]
		[Section("RECURSOS")]
		CrearRecursoTipoEnlace = 64,
		[Description("DESCPERMISOEDITARENLACE")]
		[Section("RECURSOS")]
		EditarRecursoTipoEnlace = 128,
		[Description("DESCPERMISOELIMINARENLACE")]
		[Section("RECURSOS")]
		EliminarRecursoTipoEnlace = 256,

		[Description("DESCPERMISOCREARNOTA")]
		[Section("RECURSOS")]
		CrearNota = 512,
		[Description("DESCPERMISOEDITARNOTA")]
		[Section("RECURSOS")]
		EditarNota = 1024,
		[Description("DESCPERMISOELIMINARNOTA")]
		[Section("RECURSOS")]
		EliminarNota = 2048,

		[Description("DESCPERMISOCREARPREGUNTA")]
		[Section("RECURSOS")]
		CrearPregunta = 4096,
		[Description("DESCPERMISOEDITARPREGUNTA")]
		[Section("RECURSOS")]
		EditarPregunta = 8192,
		[Description("DESCPERMISOELIMINARPREGUNTA")]
		[Section("RECURSOS")]
		EliminarPregunta = 16384,

		[Description("DESCPERMISOCREARENCUESTA")]
		[Section("RECURSOS")]
		CrearEncuesta = 32768,
		[Description("DESCPERMISOEDITARENCUESTA")]
		[Section("RECURSOS")]
		EditarEncuesta = 65536,
		[Description("DESCPERMISOELIMINARENCUESTA")]
		[Section("RECURSOS")]
		EliminarEncuesta = 131072,

		[Description("DESCPERMISOCREARDEBATE")]
		[Section("RECURSOS")]
		CrearDebate = 262144,
		[Description("DESCPERMISOEDITARDEBATE")]
		[Section("RECURSOS")]
		EditarDebate = 524288,
		[Description("DESCPERMISOELIMINARDEBATE")]
		[Section("RECURSOS")]
		EliminarDebate = 1048576,

		[Description("DESCPERMISOCREARSEMANTICO")]
		[Section("RECURSOS")]
		CrearRecursoSemantico = 2097152,
		[Description("DESCPERMISOEDITARSEMANTICO")]
		[Section("RECURSOS")]
		EditarRecursoSemantico = 4194304,
		[Description("DESCPERMISOELIMINARSEMANTICO")]
		[Section("RECURSOS")]
		EliminarRecursoSemantico = 8388608,

		[Description("DESCPERMISORESTAURARVERSIONENLACE")]
		[Section("RECURSOS")]
		RestaurarVersionEnlace = 16777216,
		[Description("DESCPERMISOELIMINARVERSIONENLACE")]
		[Section("RECURSOS")]
		EliminarVersionEnlace = 33554432,

		[Description("DESCPERMISORESTAURARVERSIONADJUNTO")]
		[Section("RECURSOS")]
		RestaurarVersionAdjunto = 67108864,
		[Description("DESCPERMISOELIMINARVERSIONADJUNTO")]
		[Section("RECURSOS")]
		EliminarVersionAdjunto = 134217728,

		[Description("DESCPERMISORESTAURARVERSIONREFERNCIA")]
		[Section("RECURSOS")]
		RestaurarVersionReferencia = 268435456,
		[Description("DESCPERMISOELIMINARVERSIONREFERNCIA")]
		[Section("RECURSOS")]
		EliminarVersionReferencia = 536870912,

		[Description("DESCPERMISORESTAURARVERSIONNOTA")]
		[Section("RECURSOS")]
		RestaurarVersionNota = 1073741824,
		[Description("DESCPERMISORESELIMINARVERSIONNOTA")]
		[Section("RECURSOS")]
		EliminarVersionNota = 2147483648,

		[Description("DESCPERMISORESTAURARVERSIONPREGUNTA")]
		[Section("RECURSOS")]
		RestaurarVersionPregunta = 4294967296,
		[Description("DESCPERMISOELIMINARVERSIONPREGUNTA")]
		[Section("RECURSOS")]
		EliminarVersionPregunta = 8589934592,

		[Description("DESCPERMISORESTAURARVERSIONENCUESTA")]
		[Section("RECURSOS")]
		RestaurarVersionEncuesta = 17179869184,
		[Description("DESCPERMISOELIMINARVERSIONENCUESTA")]
		[Section("RECURSOS")]
		EliminarVersionEncuesta = 34359738368,
        
		[Description("DESCPERMISORESTAURARVERSIONDEBATE")]
		[Section("RECURSOS")]
		RestaurarVersionDebate = 68719476736,
		[Description("DESCPERMISOELIMINARVERSIONDEBATE")]
		[Section("RECURSOS")]
		EliminarVersionDebate = 137438953472,

		[Description("DESCPERMISOCERTIFICARRECURSO")]
		[Section("RECURSOS")]
		CertificarRecurso = 274877906944
	}

	public enum TipoPermisoRecursosSemanticos : ulong
    {
        Crear = 1,
        Modificar = 2,
        Eliminar = 4,
        RestaurarVersion = 8,
        EliminarVersion = 16
    }

    public enum AmbitoRol
    {
        Comunidad,
        Ecosistema
    }

    /// <summary>
    /// Vistas que pueden tener los recursos en la home de tipo cat?logo
    /// </summary>
    public enum TipoVistaHomeCatalogo
    {
        /// <summary>
        /// Listado
        /// </summary>
        Listado = 0,

        /// <summary>
        /// Mosaicp
        /// </summary>
        Mosaico = 1,
    }

    public enum TipoUbicacionGadget
    {
        /// <summary>
        /// Gadgets en el lateral de la Home de la comunidad(No cat?logos)
        /// </summary>
        LateralHomeComunidad = 0,
        /// <summary>
        /// Gadgets en la ficha de un recurso de la comunidad
        /// </summary>
        FichaRecursoComunidad = 1,
        /// <summary>
        /// Gadgets en el pie de la home de la comunidad
        /// </summary>
        PieHomeComunidad = 2,
        /// <summary>
        /// Gadgets en el cuerpo de la home de la comunidad
        /// </summary>
        CuerpoHomeComunidad = 3,
        /// <summary>
        /// Gadgets en la cabecera cuerpo de de la home de la comunidad
        /// </summary>
        CabeceraHomeComunidad = 4,
        /// <summary>
        /// Gadgets en la cabecera del ?ndice de la comunidad
        /// </summary>
        CabeceraIndiceComunidad = 5,
        /// <summary>
        /// Gadgets en la cabecera de la p?gina de recursos
        /// </summary>
        CabeceraRecursosComunidad = 6,
        /// <summary>
        /// Gadgets en la cabecera de las pesta?as de la comunidad
        /// </summary>
        CabeceraPestanyasComunidad = 7,
        /// <summary>
        /// Gadgets en la cabecera de la p?gina de debates
        /// </summary>
        CabeceraDebatesComunidad = 8,
        /// <summary>
        /// Gadgets en la cabecera de la p?gina de dafos
        /// </summary>
        CabeceraDafosComunidad = 9,
        /// <summary>
        /// Gadgets en la cabecera de la p?gina de preguntas
        /// </summary>
        CabeceraPreguntasComunidad = 10,
        /// <summary>
        /// Gadgets en la cabecera de la p?gina de encuestas
        /// </summary>
        CabeceraEncuestasComunidad = 11,
        /// <summary>
        /// Gadgets en la cabecera de la p?gina de personas y organizaciones
        /// </summary>
        CabeceraPersonasYOrgDeComunidad = 12,
        /// <summary>
        /// Gadgets en la cabecera de la p?gina de acerca-de
        /// </summary>
        CabeceraAcercaDeComunidad = 13,
    }

    public enum TipoEventoProyecto
    {
        /// <summary>
        /// Evento disponible
        /// </summary>
        SinRestriccion = 0,
        /// <summary>
        /// Evento solo disponible para nuevos miembros en la comunidad
        /// </summary>
        NuevoEnComunidad = 1,
        /// <summary>
        /// Evento solo disponible para nuevos miembros en el ecosistema
        /// </summary>
        NuevoEnEcosistema = 2,
    }

    /// <summary>
    /// Enumeraci?n para distinguir los tipos de cabeceras
    /// </summary>
    public enum TipoCabeceraProyecto
    {
        /// <summary>
        /// Cabecera normal
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Cabecera simplificada
        /// </summary>
        Simplificada = 1,
    }

    /// <summary>
    /// Enumeraci?n para distinguir los tipos de fichas
    /// </summary>
    public enum TipoFichaRecursoProyecto
    {
        /// <summary>
        /// Ficha normal
        /// </summary>
        Normal = 0,
        /// <summary>
        /// FIcha de inevery
        /// </summary>
        Inevery = 1,
    }

    /// <summary>
    /// Enumeracion para identificar los tipos de eventos en las acciones de un proyecto
    /// </summary>
    public enum TipoProyectoEventoAccion
    {
        Registro = 0,
        Login = 1,
        LecturaArticulo = 3,

        // No usar el setTimeout!!!
        CompartirContenidoGoogle = 4,
        CompartirContenidoTwitter = 5,
        CompartirContenidoFacebook = 6,
        CompartirContenidoDelicious = 7,
        CompartirContenidoLinkedIn = 8,
        CompartirContenidoReddit = 9,
        CompartirContenidoBlogger = 10,
        CompartirContenidoDiigo = 11,

        PublicarEnalce = 20,
        PublicarVideoAudio = 21,
        PublicarAdjunto = 22,
        PublicarNota = 23,
        PublicarWiki = 24,
        PublicarPregunta = 25,
        PublicarDebate = 26,

        EnviarEnlace = 40,

        RegistroGoogle = 51,
        RegistroFacebook = 52,
        RegistroTwitter = 53,
        RegistroSantillana = 54,
        InicioRegistroGoogle = 55,
        InicioRegistroFacebook = 56,
        InicioRegistroTwitter = 57,
        InicioRegistroSantillana = 58,
        LoginGoogle = 61,
        LoginFacebook = 62,
        LoginTwitter = 63,
        LoginSantillana = 64,
        LoginKeycloak = 65,

        ClickGoogle = 71,
        ClickFacebook = 72,
        ClickTwitter = 73,
        ClickSantillana = 74,

        LoginConToken = 80,
        RegistroConToken = 81
    }

    /// <summary>
    /// Enumeraci?n para distinguir los pasos del registro
    /// </summary>
    public enum PasosRegistro
    {
        /// <summary>
        /// P?gina de Preferencias
        /// </summary>
        Preferencias = 0,
        /// <summary>
        /// P?gina de Datos
        /// </summary>
        Datos = 1,
        /// <summary>
        /// P?gina de Conecta
        /// </summary>
        Conecta = 2
    }

    /// <summary>
    /// Enumeraci?n para distinguir los campos genericos del registro
    /// </summary>
    public enum TipoCampoGenericoRegistro
    {
        /// <summary>
        /// Pa?s
        /// </summary>
        Pais = 0,
        /// <summary>
        /// Provincia
        /// </summary>
        Provincia = 1,
        /// <summary>
        /// Localidad
        /// </summary>
        Localidad = 2,
        /// <summary>
        /// Sexo
        /// </summary>
        Sexo = 3
    }

    /// <summary>
    /// Enumeraci?n para distinguir tipos de privacidad de una pagina
    /// </summary>
    public enum TipoPrivacidadPagina
    {
        /// <summary>
        /// Hereda la privacidad de la comunidad (publica, privada..)
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Tiene la privacidad contraria a la comunidad (si la comunidad es public es visible solo para miembros y si la comunidad es privada es publico)
        /// </summary>
        Especial = 1,
        /// <summary>
        /// Solo tienen acceso los perfiles y grupos seleccionados
        /// </summary>
        Lectores = 2,
    }

    /// <summary>
    /// Tipos de configuraci?n extra para los elementos sem?nticos de una comunidad.
    /// </summary>
    public enum TipoConfigExtraSemantica
    {
        /// <summary>
        /// Configuraci?n para tesauro sem?ntico.
        /// </summary>
        TesauroSemantico = 0,
        /// <summary>
        /// Configuraci?n para una entidad secundaria.
        /// </summary>
        EntidadSecundaria = 1,
        /// <summary>
        /// Configuraci?n para un grafo simple.
        /// </summary>
        GrafoSimple = 2
    }

    #endregion

    #region Clases Join

    public class JoinProyectoPestanyaBusquedaProyectoPestanyaMenu
    {
        public ProyectoPestanyaBusqueda ProyectoPestanyaBusqueda { get; set; }
        public ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }

    #endregion

    #region Joins

    public static class JoinsProyecto
    {
        public static IQueryable<JoinProyectoPestanyaBusquedaProyectoPestanyaMenu> JoinProyectoPestanyaMenu(this IQueryable<ProyectoPestanyaBusqueda> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);

            return pQuery.Join(entityContext.ProyectoPestanyaMenu, pestanyaBusqueda => pestanyaBusqueda.PestanyaID, pestanyaMenu => pestanyaMenu.PestanyaID, (pestanyaBusqueda, pestanyaMenu) => new JoinProyectoPestanyaBusquedaProyectoPestanyaMenu
            {
                ProyectoPestanyaBusqueda = pestanyaBusqueda,
                ProyectoPestanyaMenu = pestanyaMenu
            });
        }
    }

    #endregion



    /// <summary>
    /// DataAdapter de proyecto
    /// </summary>
    public class ProyectoAD : BaseAD
    {
        #region Miembros

        /// <summary>
        /// Id del proyecto myGnoss
        /// </summary>
        private static Guid mMyGnoss = new Guid("11111111-1111-1111-1111-111111111111");

        /// <summary>
        /// Id de la meta-organizacion
        /// </summary>
        private static Guid mMetaOrganizacion = new Guid("11111111-1111-1111-1111-111111111111");

        /// <summary>
        /// Id del meta-proyecto
        /// </summary>
        private static Guid mMetaProyecto = new Guid("11111111-1111-1111-1111-111111111111");

        /// <summary>
        /// Tabla base del metaproyecto
        /// </summary>
        private static int mTablaBaseIdMetaProyecto = int.MinValue;

        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor por defecto, sin par?metros, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public ProyectoAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ProyectoAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Constructor a partir del fichero de configuraci?n de conexi?n a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci?n de la conexi?n a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se est?n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ProyectoAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ProyectoAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string sqlSelectExisteProyectoFAQ;
        private string sqlSelectExisteProyectoNoticias;
        private string sqlSelectExisteProyectoDidactalia;
        private string sqlSelectEmailsMiembrosDeEventoDeProyecto;
        private string sqlSelectTipoDocImagenPorDefecto;
        private string sqlUpdateAumentarNumeroMiembrosDelProyecto;
        private string sqlSelectEventosProyectoPorIdentidadID;

        #region Twitter

        private string sqlUpdateTokenTwitterProyecto;

        #endregion

        #endregion

        #region P?blicos

        public Guid ObtenerGuidPestanyaPorTipo(Guid pProyectoId, short pTipo)
        {
            var proyectoPestanyaMenu = mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID == pProyectoId && item.TipoPestanya == pTipo).FirstOrDefault();
            if (proyectoPestanyaMenu!=null)
            {
                return proyectoPestanyaMenu.PestanyaID;
            }

            return Guid.Empty;
                
        }


        /// <summary>
        /// Obtiene la fecha de alta del grupo de organizaci?n en un proyecto
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organizaci?n</param>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DateTime con la fecha de alta del grupo en el proyecto. Null en caso de no encontrarlo</returns>
        public DateTime? ObtenerFechaAltaGrupoOrganizacionEnProyecto(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID)
        {
            DateTime? fechaAlta = null;

            GrupoOrgParticipaProy filaGrupoProyecto = mEntityContext.GrupoOrgParticipaProy.Find(pGrupoID, pOrganizacionID, pProyectoID);

            if (filaGrupoProyecto != null)
            {
                fechaAlta = filaGrupoProyecto.FechaAlta;
            }

            return fechaAlta;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un determinado grupo de organizaci?n
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organizaci?n</param>
        /// <returns>Lista de filas de GrupoOrgParticipaProy con los proyectos del grupo</returns>
        public List<GrupoOrgParticipaProy> ObtenerProyectosParticipaGrupoOrganizacion(Guid pGrupoID)
        {
            return mEntityContext.GrupoOrgParticipaProy.Where(fila => fila.GrupoID == pGrupoID).ToList();
        }

        /// <summary>
        /// Obtiene los gadget por idioma asociados al identificador del gadget pasado por par?metro
        /// </summary>
        /// <param name="pGadgetID">Identificador del gadget</param>
        /// <returns></returns>
        public List<ProyectoGadgetIdioma> ObtenerProyectoGadgetIdiomaDeGadget(Guid pGadgetID)
        {
            return mEntityContext.ProyectoGadgetIdioma.Where(item => item.GadgetID.Equals(pGadgetID)).ToList();
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un determinado grupo de organizaci?n
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista de filas de GrupoOrgParticipaProy con los grupos del proyecto</returns>
        public List<GrupoOrgParticipaProy> ObtenerGruposOrganizacionParticipanProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.GrupoOrgParticipaProy.Where(fila => fila.OrganizacionID == pOrganizacionID && fila.ProyectoID == pProyectoID).ToList();
        }

        /// <summary>
        /// Elimina la fila de GrupoOrgParticipaProy.
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organizaci?n</param>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void BorrarFilaGrupoOrgParticipaProy(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID)
        {
            GrupoOrgParticipaProy filaGrupo = new GrupoOrgParticipaProy { GrupoID = pGrupoID, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID };
            mEntityContext.Entry(filaGrupo).State = EntityState.Deleted;
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Crea una fila de GrupoOrgParticipaProy.
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organizaci?n</param>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoIdentidad">Tipo de perfil con el que participan los miembros del grupo</param>
        public void AddFilaGrupoOrgParticipaProy(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID, TiposIdentidad pTipoIdentidad)
        {
            GrupoOrgParticipaProy filaGrupo = new GrupoOrgParticipaProy { GrupoID = pGrupoID, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, FechaAlta = DateTime.Now, TipoPerfil = (short)pTipoIdentidad };
            mEntityContext.GrupoOrgParticipaProy.Add(filaGrupo);
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataSet"></param>
        /// <param name="orden"></param>
        /// <param name="IDProy"></param>
        /// <returns></returns>
        public string ObtieneDescripciondeNivelCertificacion(string pOrden, Guid pProyectoID)
        {
            return mEntityContext.NivelCertificacion.Where(item => item.Orden.ToString().Equals(pOrden) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.Descripcion).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los permisos de p?ginas de los usuarios de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista de filas de PermisosPaginasUsuarios</returns>
        public List<PermisosPaginasUsuarios> ObtenerPermisosPaginasProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.PermisosPaginasUsuarios.Where(fila => fila.OrganizacionID == pOrganizacionID && fila.ProyectoID == pProyectoID).ToList();
        }

        /// <summary>
        /// Obtiene los permisos de p?ginas del usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Lista de filas de PermisosPaginasUsuarios del usuario en el proyecto</returns>
        public List<PermisosPaginasUsuarios> ObtenerPermisosPaginasProyectoUsuarioID(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID)
        {
            return mEntityContext.PermisosPaginasUsuarios.Where(fila => fila.OrganizacionID == pOrganizacionID && fila.ProyectoID == pProyectoID && fila.UsuarioID == pUsuarioID).ToList();
        }

        /// <summary>
        /// Obtiene los permisos de p?ginas del usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoPagina">P?gina sobre la que se comprueba el permiso</param>
        /// <returns>True si el usuario tiene permiso sobre el tipo de p?gina en el proyecto</returns>
        public bool TienePermisoUsuarioEnPagina(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID, TipoPaginaAdministracion pTipoPagina)
        {
            return mEntityContext.PermisosPaginasUsuarios.Any(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.Pagina == (short)pTipoPagina);
        }

        /// <summary>
        /// Obtiene una RedireccionRegistroRuta por su id y sus RedireccionValorParametro asociadas 
        /// </summary>
        /// <param name="pDominio"></param>
        /// <returns></returns>
        public RedireccionRegistroRuta ObtenerRedireccionRegistroRutaPorRedireccionID(Guid pRedireccionID)
        {
            return mEntityContext.RedireccionRegistroRuta.Include("RedireccionValorParametro").FirstOrDefault(r => r.RedireccionID == pRedireccionID);
        }

        /// <summary>
        /// Obtiene una lista con registros de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con los registros de un proyecto</returns>
        public List<string> ObtenerProyectoPasoRegistro(Guid pProyectoID)
        {
            return mEntityContext.ProyectoPasoRegistro.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.PasoRegistro).ToList();
        }

        /// <summary>
        /// Obtiene una lista con las pestanyas para los pasos del registro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con las Pestanyas del proyecto</returns>
        public List<ProyectoPestanyaMenu> ListaPestanyasMenuRegistro(Guid pProyectoID)
        {
            return mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(pProyectoID) && item.TipoPestanya.Equals((short)TipoPestanyaMenu.CMS)).ToList();
        }

        /// <summary>
        /// Obtiene una lista con la obligatoriedad de los registros
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con los la obligatoriedad de los registros</returns>
        public List<bool> ObtenerListaObligatoriedadRegistros(Guid pProyectoID)
        {
            return mEntityContext.ProyectoPasoRegistro.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.Obligatorio).ToList();
        }

        /// <summary>
        /// Actualiza la tabla PreferenciaProyecto con la preferencia de los proyectos seleccionados
        /// </summary>
        /// <param name="listaCategoriasSeleccionadas">Lista de IDs de las categorias seleccionadas</param>
        /// <param name="pProyectoID">ID del proyecto seleccionado</param>
        public void ActualizarTablaPreferenciaProyecto(List<Guid> listaCategoriasSeleccionadas, Guid pProyectoID)
        {
            List<PreferenciaProyecto> yaSeleccionadas = mEntityContext.PreferenciaProyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            foreach (PreferenciaProyecto pref in yaSeleccionadas)
            {
                mEntityContext.Entry(pref).State = EntityState.Deleted;
            }
            foreach (Guid id in listaCategoriasSeleccionadas)
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ProyectoAD>(),mLoggerFactory);
                AD.EntityModel.Models.Tesauro.CategoriaTesauro cateTesauro = mEntityContext.CategoriaTesauro.Where(proy => proy.CategoriaTesauroID.Equals(id)).FirstOrDefault();
                PreferenciaProyecto pref = new PreferenciaProyecto();
                pref.TesauroID = cateTesauro.TesauroID;
                pref.CategoriaTesauroID = id;
                pref.OrganizacionID = proyAD.ObtenerOrganizacionIDAPartirDeProyectoID(pProyectoID);
                pref.ProyectoID = pProyectoID;
                pref.Orden = cateTesauro.Orden;
                mEntityContext.PreferenciaProyecto.Add(pref);
            }
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Devuelve los IDs de las categorias seleccionadas
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista de los IDs de las categorias seleccionadas</returns>
        public List<Guid> ObtenerCategoriasSeleccionadas(Guid pProyectoID)
        {
            return mEntityContext.PreferenciaProyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.CategoriaTesauroID).ToList();
        }

        /// <summary>
        /// Guarda en la base de datos los registros por pasos en la tabla ProyectoPasoRegistro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="listaPasos">Lista con los registros a guardar</param>
        public void GuardarRegistroPorPasos(Guid pProyectoID, List<PasoRegistroModel> listaPasos)
        {
            List<ProyectoPasoRegistro> listaPPR = mEntityContext.ProyectoPasoRegistro.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            int numListaPPR = listaPPR.Count;
            int i = 0;
            if (numListaPPR >= 1)
            {
                foreach (ProyectoPasoRegistro proy in listaPPR)
                {
                    mEntityContext.EliminarElemento(proy);
                }
                foreach (PasoRegistroModel registro in listaPasos)
                {
                    if (!registro.Deleted)
                    {
                        ProyectoPasoRegistro pPasoRegistro = new ProyectoPasoRegistro();
                        pPasoRegistro.OrganizacionID = MetaOrganizacion;
                        pPasoRegistro.ProyectoID = pProyectoID;
                        pPasoRegistro.Orden = (short)i;
                        pPasoRegistro.PasoRegistro = registro.NombrePasoRegistro;
                        pPasoRegistro.Obligatorio = registro.Obligatorio;
                        mEntityContext.ProyectoPasoRegistro.Add(pPasoRegistro);
                        i++;
                    }
                }
            }
            else
            {
                foreach (PasoRegistroModel registro in listaPasos)
                {
                    if (!registro.Deleted)
                    {
                        ProyectoPasoRegistro pPasoRegistro = new ProyectoPasoRegistro();
                        pPasoRegistro.OrganizacionID = MetaOrganizacion;
                        pPasoRegistro.ProyectoID = pProyectoID;
                        pPasoRegistro.Orden = (short)i;
                        pPasoRegistro.PasoRegistro = registro.NombrePasoRegistro;
                        pPasoRegistro.Obligatorio = registro.Obligatorio;
                        mEntityContext.ProyectoPasoRegistro.Add(pPasoRegistro);
                        i++;
                    }
                }
            }
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene las redirecciones de un dominio
        /// </summary>
        /// <param name="pDominio"></param>
        /// <returns></returns>
        public List<RedireccionRegistroRuta> ObtenerRedireccionRegistroRutaPorDominio(string pDominio, bool pCargarRelaciones)
        {
            mLoggingService.AgregarEntrada("ObtenerRedireccionRegistroRutaPorDominio - INICIO");

            List<RedireccionRegistroRuta> filasRedirecciones;

            if (!pCargarRelaciones)
            {
                filasRedirecciones = mEntityContext.RedireccionRegistroRuta.Where(fila => fila.Dominio == pDominio).OrderByDescending(r => r.FechaCreacion).ToList();
            }
            else
            {
                filasRedirecciones = mEntityContext.RedireccionRegistroRuta.Include("RedireccionValorParametro").Where(r => r.Dominio == pDominio).OrderByDescending(r => r.FechaCreacion).ToList();
            }

            mLoggingService.AgregarEntrada("ObtenerRedireccionRegistroRutaPorDominio - FIN");

            return filasRedirecciones;
        }

        /// <summary>
        /// A?ade a la base de datos la redirecci?n indicada y su valor con par?metro de tener
        /// </summary>
        /// <param name="pRedireccionRegistroRuta"></param>
        public void AniadirRedireccionRegistroRuta(RedireccionRegistroRuta pRedireccionRegistroRuta)
        {
            mEntityContext.RedireccionRegistroRuta.Add(pRedireccionRegistroRuta);
        }

        /// <summary>
        /// A?ade la el valor par?metro de la redirecci?n indicado a la base de datos
        /// </summary>
        /// <param name="pRedireccionValorParametro"></param>
        public void AniadirRedireccionValorParametro(RedireccionValorParametro pRedireccionValorParametro)
        {
            mEntityContext.RedireccionValorParametro.Add(pRedireccionValorParametro);
        }

        public List<RedireccionValorParametro> ObtenerRedireccionValorParametroPorRedireccionID(Guid pRedireccionID)
        {
            return mEntityContext.RedireccionValorParametro.Where(item => item.RedireccionID.Equals(pRedireccionID)).ToList();
        }

        /// <summary>
        /// Crea/Modifica una lista de filas de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pDicFilasRedireccion"></param>
        /// <param name="pRetrasarGuardado">Indica si hay que guardar los cambios inmediatamente. Pasarlo a TRUE si no se mantiene el mismo contexto para todas las operaciones</param>
        public void GuardarFilaRedireccionRegistroRuta(Dictionary<RedireccionRegistroRuta, bool> pDicFilasRedireccion, bool pRetrasarGuardado)
        {
            foreach (KeyValuePair<RedireccionRegistroRuta, bool> redireccion in pDicFilasRedireccion)
            {
                if (redireccion.Value) //es nueva
                {
                    mEntityContext.RedireccionRegistroRuta.Add(redireccion.Key);
                }
                else
                {
                    var entidadRedireccion = mEntityContext.RedireccionRegistroRuta.First(item => item.RedireccionID.Equals(redireccion.Key.RedireccionID));
                    entidadRedireccion.NombreParametro = redireccion.Key.NombreParametro;
                    entidadRedireccion.UrlOrigen = redireccion.Key.UrlOrigen;
                    if (redireccion.Key.RedireccionValorParametro != null)
                    {
                        foreach (RedireccionValorParametro filaValor in redireccion.Key.RedireccionValorParametro.ToList())
                        {
                            RedireccionValorParametro redireccionValorParametro = mEntityContext.RedireccionValorParametro.Where(item => item.RedireccionID.Equals(filaValor.RedireccionID) && item.ValorParametro.Equals(filaValor.ValorParametro)).FirstOrDefault();
                            if (redireccionValorParametro != null)
                            {
                                redireccionValorParametro.UrlRedireccion = filaValor.UrlRedireccion;
                                redireccionValorParametro.RedireccionRegistroRuta = filaValor.RedireccionRegistroRuta;
                            }
                            else
                            {
                                redireccionValorParametro = new RedireccionValorParametro();
                                redireccionValorParametro.ValorParametro = filaValor.ValorParametro;
                                redireccionValorParametro.OrdenPresentacion = filaValor.OrdenPresentacion;
                                redireccionValorParametro.UrlRedireccion = filaValor.UrlRedireccion;
                                redireccionValorParametro.RedireccionRegistroRuta = filaValor.RedireccionRegistroRuta;
                                redireccionValorParametro.RedireccionID = filaValor.RedireccionID;
                                redireccionValorParametro.MantenerFiltros = filaValor.MantenerFiltros;

                                mEntityContext.RedireccionValorParametro.Add(redireccionValorParametro);
                            }
                        }
                    }
                }
            }

            if (!pRetrasarGuardado)
            {
                ActualizarBaseDeDatosEntityContext();
            }
        }

        public List<IntegracionContinuaPropiedad> ObtenerFilasIntegracionContinuaParametro(Guid pProyectoID)
        {
            return mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public void BorrarConfiguracionSemanticaExtraDeProyecto(Guid pClave, string pNombreOntologia)
        {
            List<ProyectoConfigExtraSem> listaBorrar = mEntityContext.ProyectoConfigExtraSem.Where(proyConfig => proyConfig.ProyectoID.Equals(pClave) && proyConfig.UrlOntologia.Equals(pNombreOntologia)).ToList();
            foreach (ProyectoConfigExtraSem proyBorrar in listaBorrar)
            {
                mEntityContext.ProyectoConfigExtraSem.Remove(proyBorrar);
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Crea una nueva carga masiva en la base de datos
        /// </summary>
        /// <param name="idCarga">Id de la carga</param>
        /// <param name="estado">Estado de la carga</param>
        /// <param name="fechaAlta">Fecha de creaci?n de la carga</param>
        /// <param name="proyectoId">Id el proyecto al que pertenece la carga</param>
        /// <param name="identidadId">Id de la identidad del sujeto de la carga</param>
        /// <param name="nombre">Nombre de la carga</param>
        /// <param name="organizacionId">Id de la organizacion de la carga</param>
        /// <returns>Devuelve cierto si se ha a?adido una nueva carga</returns>
        public bool CrearNuevaCargaMasiva(Guid idCarga, int estado, DateTime fechaAlta, Guid proyectoId, Guid identidadId, string ontologia, string nombre = null, Guid? organizacionId = null)
        {
            bool creada = false;

            bool existe = mEntityContext.Carga.Any(item => item.CargaID.Equals(idCarga));
            if (!existe)
            {
                Carga nuevaCarga = new Carga();
                nuevaCarga.CargaID = idCarga;
                nuevaCarga.Nombre = nombre;
                nuevaCarga.Estado = (short)estado;
                nuevaCarga.FechaAlta = fechaAlta;
                nuevaCarga.ProyectoID = proyectoId;
                nuevaCarga.OrganizacionID = organizacionId;
                nuevaCarga.IdentidadID = identidadId;
                nuevaCarga.Ontologia = ontologia;

                mEntityContext.Carga.Add(nuevaCarga);
                mEntityContext.SaveChanges();
                creada = true;
            }

            return creada;
        }

        public List<string> ObtenerPropiedadesSearch(Guid pProyectoID)
        {
            List<string> listaPropiedades = new List<string>();

            List<ConfigSearchProy> listaConfigSearchProy = mEntityContext.ConfigSearchProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            List<ConfigAutocompletarProy> listaConfigAutocompletarProy = mEntityContext.ConfigAutocompletarProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (ConfigSearchProy configSearchProy in listaConfigSearchProy)
            {
                listaPropiedades = listaPropiedades.Union(configSearchProy.Valor.Split('|').ToList()).ToList();
            }
            foreach (ConfigAutocompletarProy configAutocompletarProy in listaConfigAutocompletarProy)
            {
                listaPropiedades = listaPropiedades.Union(configAutocompletarProy.Valor.Split('|').ToList()).ToList();
            }

            return listaPropiedades;
        }

        /// <summary>
        /// Devuelve la lista de cargas de una identidadID
        /// </summary>
        /// <param name="identidadId">Id de la identidad de la carga</param>
        /// <returns>Lista de cargas correspondientes a la identidad</returns>
        public List<Carga> ObtenerCargasMasivasPorIdentidadID(Guid identidadId)
        {
            return mEntityContext.Carga.Where(x => x.IdentidadID.Value.Equals(identidadId)).OrderByDescending(x => x.FechaAlta).ToList();
        }

        /// <summary>
        /// Devuelve la lista de paquetes de una carga
        /// </summary>
        /// <param name="cargaID">Id de la carga</param>
        /// <returns>Lista de los paquetes de la carga</returns>
        public List<CargaPaquete> ObtenerPaquetesPorIDCarga(Guid cargaID)
        {
            return mEntityContext.CargaPaquete.Where(x => x.CargaID.Equals(cargaID)).ToList();
        }

        /// <summary>
        /// Crea un nuevo paquete asociado a una carga
        /// </summary>
        /// <param name="paqueteID">Id del paquete</param>
        /// <param name="cargaId">Id de la carga</param>
        /// <param name="rutaOnto">Ruta del archivo de triples de la ontologia</param>
        /// <param name="rutaSearch">Ruta del archivo de triples busqueda</param>
        /// <param name="rutaSql">Ruta del archivo del sql</param>
        /// <param name="estado">Estado del paquete</param>
        /// <param name="error">Error del paquete</param>
        /// <param name="fechaAlta">Fecha en la que se crea el paquete</param>
        /// <param name="ontologia">Ontologia a la que pertenece el paquete</param>
        /// <param name="comprimido">Los archivos estan comprimidos</param>
        /// <param name="fechaProcesado">Fecha en la que se ha procesado el paquete</param>
        /// <returns>Devuelve cierto si se ha creado el paquete</returns>
        public bool CrearNuevoPaqueteCargaMasiva(Guid paqueteID, Guid cargaId, string rutaOnto, string rutaSearch, string rutaSql, int estado, string error, DateTime? fechaAlta, bool comprimido = false, DateTime? fechaProcesado = null)
        {
            bool creado = false;

            bool existe = mEntityContext.CargaPaquete.Any(item => item.PaqueteID.Equals(paqueteID));
            if (!existe)
            {
                CargaPaquete nuevoPaquete = new CargaPaquete();
                nuevoPaquete.PaqueteID = paqueteID;
                nuevoPaquete.CargaID = cargaId;
                nuevoPaquete.RutaOnto = rutaOnto;
                nuevoPaquete.RutaBusqueda = rutaSearch;
                nuevoPaquete.RutaSQL = rutaSql;
                nuevoPaquete.Estado = (short)estado;
                nuevoPaquete.Error = string.Empty;
                nuevoPaquete.FechaAlta = fechaAlta;
                nuevoPaquete.FechaProcesado = fechaProcesado;
                nuevoPaquete.Comprimido = comprimido;
                mEntityContext.CargaPaquete.Add(nuevoPaquete);
                mEntityContext.SaveChanges();
                creado = true;
            }

            return creado;
        }

        public void CrearFilasIntegracionContinuaParametro(List<IntegracionContinuaPropiedad> pListaPropiedades, Guid pProyectoID, TipoObjeto pTipoObjeto, string pID)
        {
            try
            {
                List<IntegracionContinuaPropiedad> propiedadesAnteriores;
                if (string.IsNullOrEmpty(pID))
                {
                    propiedadesAnteriores = mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID.Equals(pProyectoID) && propiedad.TipoObjeto.Equals((short)pTipoObjeto)).ToList();
                }
                else
                {
                    propiedadesAnteriores = mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID.Equals(pProyectoID) && propiedad.TipoObjeto.Equals((short)pTipoObjeto) && propiedad.ObjetoPropiedad.Equals(pID)).ToList();
                }

                bool transaccionIniciada = IniciarTransaccionEntityContext();
                try
                {
                    foreach (IntegracionContinuaPropiedad propiedad in propiedadesAnteriores)
                    {
                        if (!pListaPropiedades.Exists(propiedadfind => propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ObjetoPropiedad == propiedad.ObjetoPropiedad && propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ValorPropiedad == propiedad.ValorPropiedad))
                        {
                            mEntityContext.IntegracionContinuaPropiedad.Remove(propiedad);
                        }
                    }

                    ActualizarBaseDeDatosEntityContext();
                    foreach (IntegracionContinuaPropiedad propiedad in pListaPropiedades)
                    {
                        if (!propiedadesAnteriores.Exists(propiedadfind => propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ObjetoPropiedad == propiedad.ObjetoPropiedad && propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ValorPropiedad == propiedad.ValorPropiedad))
                        {
                            mEntityContext.IntegracionContinuaPropiedad.Add(propiedad);
                        }
                    }

                    ActualizarBaseDeDatosEntityContext();

                    if (transaccionIniciada)
                    {
                        TerminarTransaccion(true);
                    }
                }
                catch
                {
                    TerminarTransaccion(false);
                    throw;
                }
            }
            catch
            {
                //No interrumpimos la ejecuci?n por este error
            }
        }

        public void GuardarFilasIntegracionContinuaParametro(List<IntegracionContinuaPropiedad> pListaPropiedades, Guid pProyectoID)
        {
            //Propiedades del entorno actual
            List<IntegracionContinuaPropiedad> propiedadesAnteriores = mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID == pProyectoID).ToList();

            foreach (IntegracionContinuaPropiedad propiedad in propiedadesAnteriores)
            {
                if (!pListaPropiedades.Exists(propiedadfind => propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ObjetoPropiedad == propiedad.ObjetoPropiedad && propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ValorPropiedad == propiedad.ValorPropiedad))
                {
                    mEntityContext.IntegracionContinuaPropiedad.Remove(propiedad);
                }
            }

            foreach (IntegracionContinuaPropiedad propiedad in pListaPropiedades)
            {
                IntegracionContinuaPropiedad propiedadAnterior = propiedadesAnteriores.FirstOrDefault(propiedadfind => propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ObjetoPropiedad == propiedad.ObjetoPropiedad && propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ValorPropiedad == propiedad.ValorPropiedad);

                if (propiedadAnterior == null)
                {
                    propiedadAnterior = propiedad;
                    propiedadAnterior.ProyectoID = pProyectoID;
                    mEntityContext.IntegracionContinuaPropiedad.Add(propiedadAnterior);
                }

                propiedadAnterior.ValorPropiedadDestino = propiedad.ValorPropiedadDestino;
                propiedadAnterior.MismoValor = propiedad.MismoValor;
                propiedadAnterior.Revisada = propiedad.Revisada;
            }

            ActualizarBaseDeDatosEntityContext();
        }

        public Dictionary<string, Guid> ObtenerOntologiasConIDPorNombreCortoProyIncluyendoSecundarias(string pNombreCortoProyecto)
        {
            Dictionary<string, Guid> resultado = new Dictionary<string, Guid>();

            var resultadoConsulta = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docWebVin => docWebVin.DocumentoID, (documento, docWebVin) => new
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (objeto, baseRecursosProyecto) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursosProyecto
            }).Join(mEntityContext.Proyecto, objeto => objeto.BaseRecursosProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (objeto, proyecto) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = objeto.BaseRecursosProyecto,
                Proyecto = proyecto
            }).Where(objeto => (objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) || objeto.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)) && objeto.Documento.Eliminado == false && objeto.DocumentoWebVinBaseRecursos.Eliminado == false && objeto.Proyecto.NombreCorto.Equals(pNombreCortoProyecto)).Select(objeto => new { objeto.Documento.DocumentoID, objeto.Documento.Enlace }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid idOnt = fila.DocumentoID;
                string nombreOnt = fila.Enlace;
                if (!resultado.ContainsKey(nombreOnt))
                {
                    resultado.Add(nombreOnt.ToLower(), idOnt);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Elimina la fila de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pListaRedireccionesID">Lista de identificadores de redirecci?n</param>
        /// <param name="pRetrasarGuardado">Indica si hay que guardar los cambios inmediatamente. Pasarlo a TRUE si no se mantiene el mismo contexto para todas las operaciones</param>
        public void BorrarFilaRedireccionRegistroRuta(List<Guid> pListaRedireccionesID, bool pRetrasarGuardado)
        {
            foreach (Guid redireccionID in pListaRedireccionesID)
            {
                RedireccionRegistroRuta filaRedireccion = mEntityContext.RedireccionRegistroRuta.Find(redireccionID);
                if (filaRedireccion != null)
                {
                    mEntityContext.RedireccionValorParametro.RemoveRange(filaRedireccion.RedireccionValorParametro);
                    mEntityContext.RedireccionRegistroRuta.Remove(filaRedireccion);
                }
            }

            if (!pRetrasarGuardado)
            {
                ActualizarBaseDeDatosEntityContext();
            }
        }

        /// <summary>
        /// Elimina la fila de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pRedireccionID">Lista de identificadores de redirecci?n</param>
        public void BorrarRedireccionRegistroRuta(Guid pRedireccionID)
        {
            RedireccionRegistroRuta redireccionRegistroRuta = mEntityContext.RedireccionRegistroRuta.Where(item => item.RedireccionID.Equals(pRedireccionID)).FirstOrDefault();
            List<RedireccionValorParametro> listaRedireccionValorParametro = mEntityContext.RedireccionValorParametro.Where(item => item.RedireccionID.Equals(pRedireccionID)).ToList();

            mEntityContext.RedireccionValorParametro.RemoveRange(listaRedireccionValorParametro);
            mEntityContext.RedireccionRegistroRuta.Remove(redireccionRegistroRuta);
        }

        /// <summary>
        /// Elimina la fila de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pFilasValores">Diccionario de identificadores de redirecci?n y lista de valores de par?metros</param>
        /// <param name="pRetrasarGuardado">Indica si hay que guardar los cambios inmediatamente. Pasarlo a TRUE si no se mantiene el mismo contexto para todas las operaciones</param>
        public void BorrarFilasRedireccionValorParametro(List<RedireccionValorParametro> pFilasValores, bool pRetrasarGuardado)
        {
            if (pFilasValores != null && pFilasValores.Count > 0)
            {
                mEntityContext.RedireccionValorParametro.RemoveRange(pFilasValores);
            }

            if (!pRetrasarGuardado)
            {
                ActualizarBaseDeDatosEntityContext();
            }
        }

        public string ObtenerExcepcionesMovil(Guid pProyectoID)
        {
            return mEntityContext.ParametroProyecto.Where(parametroProyecto => parametroProyecto.ProyectoID.Equals(pProyectoID) && parametroProyecto.Parametro.Equals("ExcepcionBusquedaMovil")).Select(parametroProyecto => parametroProyecto.Valor).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la lista de proyectos con acciones externas
        /// </summary>
        /// <returns>Lista de Identificadores de proyecto con acciones externas configuradas</returns>
        public List<Guid> ObtenerListaIDsProyectosConAccionesExternas()
        {
            return mEntityContext.AccionesExternasProyecto.Select(item => item.ProyectoID).Distinct().ToList();
        }

        /// <summary>
        /// Carga los proyectos en estado (cerrandose)
        /// </summary>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosCerrandose()
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            //Proyecto
            List<Proyecto> listaProyectos = mEntityContext.Proyecto.Where(proyecto => proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose)).ToList();
            dataWrapperProyecto.ListaProyecto = listaProyectos;

            //ProyectoCerrandose
            List<ProyectoCerrandose> listaProyectoCerrandose = mEntityContext.ProyectoCerrandose.Join(mEntityContext.Proyecto, proyCerrandose => new { proyCerrandose.OrganizacionID, proyCerrandose.ProyectoID }, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, (proyCerrandose, proyecto) => new
            {
                ProyectoCerrandose = proyCerrandose,
                Proyecto = proyecto
            }).Where(objeto => objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose)).Select(objeto => objeto.ProyectoCerrandose).ToList();
            dataWrapperProyecto.ListaProyectoCerrandose = listaProyectoCerrandose;

            //AdministradorProyecto
            List<AdministradorProyecto> listaAdministradorProyecto = mEntityContext.AdministradorProyecto.Join(mEntityContext.Proyecto, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID }, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, (adminProy, proyecto) => new
            {
                AdministradorProyecto = adminProy,
                Proyecto = proyecto
            }).Where(objeto => objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) && objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador)).Select(objeto => objeto.AdministradorProyecto).ToList();
            dataWrapperProyecto.ListaAdministradorProyecto = listaAdministradorProyecto;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Carga una lista con el identificador de todos los proyectos abiertos
        /// </summary>
        /// <returns>ProyectoDS</returns>
        public List<Guid> ObtenerTodosIDProyectosAbiertos()
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.Estado.Equals((short)EstadoProyecto.Abierto)).Select(proyecto => proyecto.ProyectoID).ToList();
        }

        /// <summary>
        /// Comprueba si existen documentos que su nivel de certificaci?n sea el pasado por par?metro
        /// </summary>
        /// <param name="pNivelCertificacionID">Identificador del nivel de certificaci?n</param>
        /// <returns>TRUE si existe alg?n documento, FALSE en caso contrario</returns>
        public bool ExisteDocAsociadoANivelCertif(Guid pNivelCertificacionID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Any(item => item.NivelCertificacionID.Equals(pNivelCertificacionID));
        }

        /// <summary>
        /// Comprueba si existen documentos que su nivel de certificaci?n sea el pasado por par?metro
        /// </summary>
        /// <param name="pNivelesCertificacionID">Identificador de los niveles de certificaci?n</param>
        /// <returns>Lista con los niveles de certificacion y un booleano que indica si tiene documentos</returns>
        public Dictionary<Guid, bool> ExisteDocAsociadoANivelCertif(List<Guid> pNivelesCertificacionID)
        {
            Dictionary<Guid, bool> resultado = new Dictionary<Guid, bool>();

            if (pNivelesCertificacionID.Count > 0)
            {
                var resultadoConsulta = mEntityContext.DocumentoWebVinBaseRecursos.Where(documentoWebVin => pNivelesCertificacionID.Contains((Guid)documentoWebVin.NivelCertificacionID)).Where(item => item.NivelCertificacionID.HasValue).GroupBy(documento => documento.NivelCertificacionID.Value).Select(item => new { NivelCertificacionID = item.Key, Documentos = item.Count() }).ToList();

                foreach (var fila in resultadoConsulta)
                {
                    Guid nivelCert = fila.NivelCertificacionID;
                    int numDocsAsociados = fila.Documentos;

                    resultado.Add(nivelCert, numDocsAsociados > 0);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la URL propia de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>URL propia de proyecto</returns>
        public string ObtenerURLPropiaProyecto(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.URLPropia).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene si el proyecto tienen un dominio con proyectros multiples
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public int NumeroProyectosConMismosDominio(string pDominio)
        {
            int resultado = 0;

            int num = mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia.Equals(pDominio)).ToList().Count;

            if (num > 0)
            {
                //Se trata de una web
                resultado += num;
            }
            else
            {
                int numProy = mEntityContext.ConfiguracionServiciosProyecto.Where(conf => conf.Url.Equals(pDominio)).Select(conf => conf.ProyectoID).Distinct().ToList().Count;

                if (numProy > 0)
                {
                    //ConfiguracionServiciosProyecto
                    resultado += numProy;
                }

                List<Guid> listaProyectoID = mEntityContext.ConfiguracionServiciosProyecto.Where(config => !config.Url.Equals(pDominio)).Select(config => config.ProyectoID).ToList();
                int numServ = mEntityContext.Proyecto.Where(proyecto => mEntityContext.ConfiguracionServiciosDominio.Any(config => config.Url.Equals(pDominio) && config.Dominio.Contains(proyecto.URLPropia)) && !listaProyectoID.Contains(proyecto.ProyectoID)).ToList().Count;

                if (numServ > 0)
                {
                    //ConfiguracionServiciosdominio
                    resultado += numServ;
                }
                if (resultado > 0)
                {
                    return resultado;
                }
                else
                {
                    List<Guid> listaConfigProyectoID = mEntityContext.ConfiguracionServiciosProyecto.Select(proyecto => proyecto.ProyectoID).ToList();
                    List<Guid> listaProyectoProyectoID = mEntityContext.Proyecto.Where(proyecto => mEntityContext.ConfiguracionServiciosDominio.Any(config => config.Dominio.Contains(proyecto.URLPropia)) && !listaConfigProyectoID.Contains(proyecto.ProyectoID)).Select(proyecto => proyecto.ProyectoID).ToList();
                    List<Guid> listaGuidCompleta = mEntityContext.ConfiguracionServiciosProyecto.Select(config => config.ProyectoID).Union(listaProyectoProyectoID).ToList();

                    int numGen = mEntityContext.Proyecto.Where(proyecto => !listaGuidCompleta.Contains(proyecto.ProyectoID) && mEntityContext.ConfiguracionServicios.Any(conf => conf.Url.Equals(pDominio))).Select(proy => proy.ProyectoID).Distinct().ToList().Count;
                    if (numGen > 0)
                    {
                        //ConfiguracionServiciosGenerico
                        resultado = numGen;
                    }
                }
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene los proyectos por id del dominio
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public List<Guid> ObtenerProyectosIdDeDominio(string pDominio)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia.Equals(pDominio)).Select(proyecto => proyecto.ProyectoID).ToList();
        }

        public DataWrapperProyecto ObtenerProyectoLoginConfiguracion(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoLoginConfiguracion = mEntityContext.ProyectoLoginConfiguracion.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene la URL propia de un proyecto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>URL propia de proyecto</returns>
        public string ObtenerURLPropiaProyectoPorNombreCorto(string pNombreCorto)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.NombreCorto.Equals(pNombreCorto)).Select(proyecto => proyecto.URLPropia).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las URLs propias de proyectos cuyos nombres cortos se pasan por par?metro
        /// </summary>
        /// <param name="pNombresCortos">Nombres cortos de los proyectos</param>
        /// <returns>URLS propias de los proyectos</returns>
        public Dictionary<string, string> ObtenerURLSPropiasProyectosPorNombresCortos(List<string> pNombresCortos)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>();

            if (pNombresCortos.Count > 0)
            {
                var resultadoConsulta = mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia != null && pNombresCortos.Contains(proyecto.NombreCorto)).Select(proyecto => new { proyecto.NombreCorto, proyecto.URLPropia }).Distinct().ToList();

                foreach (var fila in resultadoConsulta)
                {
                    if (!string.IsNullOrEmpty(fila.NombreCorto.ToString()) && !string.IsNullOrEmpty(fila.URLPropia.ToString()))
                    {
                        resultado.Add(fila.NombreCorto, fila.URLPropia);
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el identificador de la organizaci?n de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>identificador de la organizaci?n del proyecto</returns>
        public Guid ObtenerOrganizacionIDProyecto(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.OrganizacionID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre corto de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Nombre corto del proyecto</returns>
        public string ObtenerNombreCortoProyecto(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.NombreCorto).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los nombres cortos de los proyectos
        /// </summary>
        /// <param name="pProyectosID">Identificador de los proyectos</param>
        /// <returns>Nombres cortos de los proyectos</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectos(List<Guid> pProyectosID)
        {
            Dictionary<Guid, string> resultado = new Dictionary<Guid, string>();

            if (pProyectosID.Count > 0)
            {
                var resultadoConsulta = mEntityContext.Proyecto.Where(proyecto => pProyectosID.Contains(proyecto.ProyectoID)).Select(proyecto => new { proyecto.ProyectoID, proyecto.NombreCorto }).ToList();
                foreach (var linea in resultadoConsulta)
                {
                    Guid proyID = linea.ProyectoID;
                    string nombreCorto = linea.NombreCorto;

                    if (!resultado.ContainsKey(proyID))
                    {
                        resultado.Add(proyID, nombreCorto);
                    }
                }
            }
            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<Guid, string>> ObtenerNombresCortosProyectosConNombresCortosOntologias(Guid? pProyectoID, string pNombreCortoProyecto)
        {
            Dictionary<string, Dictionary<Guid, string>> resultado = new Dictionary<string, Dictionary<Guid, string>>();

            var resultadoConsulta = mEntityContext.Proyecto.Join(mEntityContext.OntologiaProyecto, proyecto => proyecto.ProyectoID, ontologiaProy => ontologiaProy.ProyectoID, (proyecto, ontologiaProy) => new
            {
                Proyecto = proyecto,
                OntologiaProyecto = ontologiaProy
            }).Join(mEntityContext.Documento, objeto => new { Enlace = objeto.OntologiaProyecto.OntologiaProyecto1 + ".owl", objeto.OntologiaProyecto.ProyectoID }, documento => new { documento.Enlace, ProyectoID = documento.ProyectoID.Value }, (objeto, documento) => new
            {
                Proyecto = objeto.Proyecto,
                OntologiaProyecto = objeto.OntologiaProyecto,
                Documento = documento
            }).Where(x => !string.IsNullOrEmpty(x.OntologiaProyecto.NombreCortoOnt) && !x.Documento.Eliminado && x.Documento.Tipo == 7);            

            if (pProyectoID.HasValue)
            {
                resultadoConsulta = resultadoConsulta.Where(objeto => objeto.Proyecto.ProyectoID.Equals(pProyectoID.Value));

            }
            else if (!string.IsNullOrEmpty(pNombreCortoProyecto))
            {
                resultadoConsulta = resultadoConsulta.Where(objeto => objeto.Proyecto.NombreCorto.Equals(pNombreCortoProyecto));
            }

            var resultadoConsulta2 = resultadoConsulta.Select(objeto => new { objeto.Proyecto.NombreCorto, objeto.Documento.DocumentoID, objeto.OntologiaProyecto.NombreCortoOnt }).ToList();

            foreach (var fila in resultadoConsulta2)
            {
                string nombreCorto = fila.NombreCorto;
                Guid idOnt = fila.DocumentoID;
                string nombreOnt = fila.NombreCortoOnt;
                Dictionary<Guid, string> listaOntologias = new Dictionary<Guid, string>();
                if (!resultado.ContainsKey(nombreCorto))
                {
                    resultado.Add(nombreCorto, listaOntologias);
                }
                else
                {
                    listaOntologias = resultado[nombreCorto];
                }
                listaOntologias.Add(idOnt, nombreOnt);
            }

            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPerfil"></param>
        /// <param name="pTipo"></param>
        /// <returns></returns>
        public string ObtenerNombreOntologiaProyectoUsuario(Guid pPerfilID, string pTipo)
        {
            return mEntityContext.OntologiaProyecto.Join(mEntityContext.Identidad, ontologia => ontologia.ProyectoID, identidad => identidad.ProyectoID, (ontologia, identidad) => new
            {
                OntologiaProyecto = ontologia,
                PerfilID = identidad.PerfilID
            }).Where(objeto => objeto.PerfilID.Equals(pPerfilID) && objeto.OntologiaProyecto.OntologiaProyecto1.Equals(pTipo)).Select(objeto => objeto.OntologiaProyecto.NombreOnt).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las ontologias con su id por el nombre corto del proyecto
        /// </summary>
        /// <param name="pNombreCortoProyecto">Nombre corto del proyecto</param>
        /// <returns></returns>
        public Dictionary<string, Guid> ObtenerOntologiasConIDPorNombreCortoProy(string pNombreCortoProyecto)
        {
            Dictionary<string, Guid> resultado = new Dictionary<string, Guid>();

            var resultadoConsulta = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docWebVin => docWebVin.DocumentoID, (documento, docWebVin) => new
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (objeto, baseRecursosProyecto) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursosProyecto
            }).Join(mEntityContext.Proyecto, objeto => objeto.BaseRecursosProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (objeto, proyecto) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = objeto.BaseRecursosProyecto,
                Proyecto = proyecto
            }).Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && !objeto.Documento.Eliminado && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Proyecto.NombreCorto.Equals(pNombreCortoProyecto)).Select(objeto => new { objeto.Documento.DocumentoID, objeto.Documento.Enlace }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid idOnt = fila.DocumentoID;
                string nombreOnt = fila.Enlace;
                if (!resultado.ContainsKey(nombreOnt))
                {
                    resultado.Add(nombreOnt.ToLower(), idOnt);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene las ontolog?as de los proyectos en los que participa un determinado perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Dataset de proyecto con la tabla OntologiaProyecto cargada para el perfil</returns>
        public List<OntologiaProyecto> ObtenerOntologiasPorPerfilID(Guid pPerfilID)
        {
            return mEntityContext.OntologiaProyecto.Join(mEntityContext.Identidad, ontologia => ontologia.ProyectoID, identidad => identidad.ProyectoID, (ontologia, identidad) => new
            {
                OntologiaProyecto = ontologia,
                PerfilID = identidad.PerfilID
            }).Where(objeto => objeto.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.OntologiaProyecto).ToList();
        }

        /// <summary>
        /// Obtiene el nombre de la ontolog?a a partir de su identificador pasado por par?metro
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontolog?a</param>
        /// <returns>Nombre de la ontolog?a</returns>
        public string ObtenerNombreOntologiaProyectoPorOntologiaID(Guid pOntologiaID)
        {
            string enlace = mEntityContext.Documento.Where(item => item.DocumentoID.Equals(pOntologiaID)).Select(item => item.Enlace).FirstOrDefault();

            return enlace.Replace(".owl", string.Empty);
        }

        /// <summary>
        /// Obtiene el id autonum?rico que se le asigna a cada proyecto para crear la tabla BASE
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public int ObtenerTablaBaseProyectoIDProyectoPorID(Guid pProyectoID)
        {
            int resultado = -1;

            if (pProyectoID != Guid.Empty)
            {
                object id = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proy => proy.TablaBaseProyectoID).FirstOrDefault();

                if ((id is int) && (((int)id) > 0))
                {
                    resultado = (int)id;
                }
            }
            return resultado;
        }

        public int ObtenerNumeroDeProyectos()
        {
            return mEntityContext.Proyecto.Select(proy => proy.ProyectoID).Count();
        }

		/// <summary>
		/// Cambia el tipo de autocompletado que se usa en un proyecto
		/// </summary>
		/// <param name="pProyectoID"></param>
		/// <param name="pPestanyaID">Pesta?a de b?squeda a cambiar el tipo de autocompletado</param>
		/// <param name="pTipoAutocompletar">Tipo del autocompletado que se va a usar en la pesta?a</param>
		/// <exception cref="NotImplementedException"></exception>
		public void CambiarTipoAutocompletadoProyecto(Guid pProyectoID, Guid pPestanyaID, TipoAutocompletar pTipoAutocompletar)
		{
			ProyectoPestanyaBusqueda proyectoPestanyaBusqueda = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPestBusqueda => proyPestBusqueda.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestBusqueda, proyPestMenu) => new
			{
				ProyectPestanyaBusqueda = proyPestBusqueda,
				ProyectoID = proyPestMenu.ProyectoID
			}).Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID) && proyecto.ProyectPestanyaBusqueda.PestanyaID.Equals(pPestanyaID)).Select(proyecto => proyecto.ProyectPestanyaBusqueda).FirstOrDefault();
            if(proyectoPestanyaBusqueda != null && proyectoPestanyaBusqueda.TipoAutocompletar != pTipoAutocompletar)
            {
                proyectoPestanyaBusqueda.TipoAutocompletar = pTipoAutocompletar;
                mEntityContext.SaveChanges();
			}
		}
		/// <summary>
		/// Elimina las entradas de ProyectoPestanyaBusquedaPesoOC para un proyecto y una pesta?a determinada
		/// </summary>
		/// <param name="pProyectoID"></param>
		/// <param name="pPestanyaID"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void EliminarProyectoPestanyaPesosBusqueda(Guid pProyectoID, Guid pPestanyaID)
		{
            List<ProyectoPestanyaBusquedaPesoOC> listaProyectoPestanyaBusquedaPesoOC = mEntityContext.ProyectoPestanyaBusquedaPesoOC.Where(item => item.PestanyaID.Equals(pPestanyaID) && item.ProyectoID.Equals(pProyectoID)).ToList();
            foreach(ProyectoPestanyaBusquedaPesoOC proyectoPestanyaBusquedaPesoOC in listaProyectoPestanyaBusquedaPesoOC)
            {
                mEntityContext.EliminarElemento(proyectoPestanyaBusquedaPesoOC);
            }
            mEntityContext.SaveChanges();
		}
		/// <summary>
		/// Inserta las filas en la tabla ProyectoPestanyaBusquedaPesoOC para la configuraci?n del tipo de autocompletado
		/// </summary>
		/// <param name="pListaProyectoPestanyaBusquedaPesoOC"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void GuardarProyectoPestanyaBusquedaPeso(List<ProyectoPestanyaBusquedaPesoOC> pListaProyectoPestanyaBusquedaPesoOC)
		{
            foreach (ProyectoPestanyaBusquedaPesoOC proyectoPestanyaBusquedaPesoOC in pListaProyectoPestanyaBusquedaPesoOC)
            {
                ProyectoPestanyaBusquedaPesoOC filaBD = mEntityContext.ProyectoPestanyaBusquedaPesoOC.FirstOrDefault(item => item.ProyectoID.Equals(proyectoPestanyaBusquedaPesoOC.ProyectoID) && item.OntologiaProyecto1.Equals(proyectoPestanyaBusquedaPesoOC.OntologiaProyecto1) && item.Tipo.Equals(proyectoPestanyaBusquedaPesoOC.Tipo) && item.PestanyaID.Equals(proyectoPestanyaBusquedaPesoOC.PestanyaID));
                if (filaBD != null)
                {
                    if (filaBD.Peso != proyectoPestanyaBusquedaPesoOC.Peso) 
                    {
                        filaBD.Peso = proyectoPestanyaBusquedaPesoOC.Peso;
                    }
				}
                else
                {
                    mEntityContext.ProyectoPestanyaBusquedaPesoOC.Add(proyectoPestanyaBusquedaPesoOC);
                }
			}
            mEntityContext.SaveChanges();
		}
		/// <summary>
		/// Obtiene el id autonum?rico que se le asigna a cada proyecto para crear la tabla BASE
		/// </summary>
		/// <param name="pListaProyectosID">Identificadores de los proyectos</param>
		/// <returns></returns>
		public Dictionary<Guid, int> ObtenerTablasBaseProyectoIDProyectoPorID(List<Guid> pListaProyectosID)
        {
            Dictionary<Guid, int> diccionarioProyectoBaseProyectoID = new Dictionary<Guid, int>();

            if (pListaProyectosID.Count > 0)
            {
                var resultadoConsulta = mEntityContext.Proyecto.Where(proyecto => pListaProyectosID.Contains(proyecto.ProyectoID)).Select(proyecto => new { proyecto.ProyectoID, proyecto.TablaBaseProyectoID }).ToList();
                foreach (var fila in resultadoConsulta)
                {
                    Guid proyectoID = fila.ProyectoID;
                    int baseProyectoID = fila.TablaBaseProyectoID;
                    if (!diccionarioProyectoBaseProyectoID.ContainsKey(proyectoID))
                    {
                        diccionarioProyectoBaseProyectoID.Add(proyectoID, baseProyectoID);
                    }
                }
            }
            return diccionarioProyectoBaseProyectoID;
        }

        /// <summary>
        /// Obtiene el proyecto a partir del id autonum?rico que se le asigna al crear la tabla BASE.
        /// </summary>
        /// <param name="pTablaBaseProyectoID">Identificador de la tabla base del proyecto</param>
        /// <returns>DataSet del proyecto con el proyecto cargado</returns>
        public List<Proyecto> ObtenerProyectoPorTablaBaseProyectoID(int pTablaBaseProyectoID)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.TablaBaseProyectoID.Equals(pTablaBaseProyectoID) && proyecto.Estado > 0).ToList();
        }

        /// <summary>
        /// Obtiene el identificador proyecto a partir del id autonum?rico que se le asigna al crear la tabla BASE.
        /// </summary>
        /// <param name="pTablaBaseProyectoID">Identificador de la tabla base del proyecto</param>
        /// <returns>Identificador del proyecto con el proyecto cargado</returns>
        public Guid ObtenerProyectoIDPorTablaBaseProyectoID(int pTablaBaseProyectoID)
        {
            Guid resultado = mEntityContext.Proyecto.Where(proyecto => proyecto.TablaBaseProyectoID.Equals(pTablaBaseProyectoID)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();

            if (!resultado.Equals(Guid.Empty))
            {
                return resultado;
            }
            else
            {
                return MetaProyecto;
            }
        }

        /// <summary>
        /// Obtiene los proyectos con ontologias que administra el usuario
        /// </summary>
        /// <param name="pUsuarioID"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerProyectosConOntologiasAdministraUsuario(Guid pUsuarioID)
        {
            Dictionary<Guid, string> listadoProyectos = new Dictionary<Guid, string>();

            var resultadoConsulta = mEntityContext.OntologiaProyecto.Join(mEntityContext.Proyecto, ontologiaProy => ontologiaProy.ProyectoID, proyecto => proyecto.ProyectoID, (ontologiaProy, proyecto) => new
            {
                OntologiaProyecto = ontologiaProy,
                Proyecto = proyecto
            }).Join(mEntityContext.AdministradorProyecto, objeto => objeto.Proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (objeto, adminProy) => new
            {
                OntologiaProyecto = objeto.OntologiaProyecto,
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID)).Select(objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.Nombre }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid idProyecto = fila.ProyectoID;
                string nombreProyecto = fila.Nombre;
                if (!listadoProyectos.ContainsKey(idProyecto))
                {
                    listadoProyectos.Add(idProyecto, nombreProyecto);
                }
            }

            return listadoProyectos;
        }

        /// <summary>
        /// Obtiene los NombreFiltro de los ProyectosSearchPersonalizados del proyecto
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista con los NombreFiltro del proyecto</returns>
        public List<ProyectoSearchPersonalizado> ObtenerProyectosSearchPersonalizado(Guid pProyectoID)
        {
            return mEntityContext.ProyectoSearchPersonalizado.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Obtiene los valores de la consulta SPARQL asociada al filtro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="nombreFiltro">Nombre del filtro de la consulta</param>
        /// <returns>Lista con los de la consulta de SPARQL</returns>
        public List<string> ObtenerValoresConsultaSPARQL(Guid pProyectoID, string nombreFiltro)
        {
            List<string> consultaSPARQL = new List<string>();

            var datosConsulta = mEntityContext.ProyectoSearchPersonalizado.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.NombreFiltro.Equals(nombreFiltro)).FirstOrDefault();

            consultaSPARQL.Add(datosConsulta.WhereSPARQL);
            consultaSPARQL.Add(datosConsulta.OrderBySPARQL);
            consultaSPARQL.Add(datosConsulta.WhereFacetasSPARQL);

            return consultaSPARQL;
        }

        /// <summary>
        /// Actualiza los valores de la consulta de SPARQL, en caso de no existir lo a?ade 
        /// </summary>
        /// <param name="organizacionID">Id de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="nombreFiltro">Nombre del filtro de las consultas</param>
        /// <param name="listaValores">Lista de los valores de las consultas de SPARQL</param>
        public void ActualizarValoresConsultaSPARQL(Guid organizacionID, Guid pProyectoID, string nombreFiltro, List<string> listaValores)
        {
            ProyectoSearchPersonalizado proySeleccionado = mEntityContext.ProyectoSearchPersonalizado.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.OrganizacionID.Equals(organizacionID) && proy.NombreFiltro.Equals(nombreFiltro)).FirstOrDefault();

            if (proySeleccionado == null)
            {
                ProyectoSearchPersonalizado proyPersonalizado = new ProyectoSearchPersonalizado();
                proyPersonalizado.OrganizacionID = organizacionID;
                proyPersonalizado.ProyectoID = pProyectoID;
                proyPersonalizado.NombreFiltro = nombreFiltro;
                proyPersonalizado.WhereSPARQL = listaValores[0];
                proyPersonalizado.OrderBySPARQL = listaValores[1];
                proyPersonalizado.WhereFacetasSPARQL = listaValores[2];
                proyPersonalizado.OmitirRdfType = false;

                mEntityContext.ProyectoSearchPersonalizado.Add(proyPersonalizado);
            }
            else
            {
                proySeleccionado.WhereSPARQL = listaValores[0];
                proySeleccionado.OrderBySPARQL = listaValores[1];
                proySeleccionado.WhereFacetasSPARQL = listaValores[2];
            }

            ActualizarBaseDeDatosEntityContext();
        }
        /// <summary>
        /// Actualiza los valores de la tabla ProyectoSearchPersonalizado con los nuevos valores de b?squeda personalizada
        /// </summary>
        /// <param name="organizacionID">Id de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="listaParametros">Lista de los par?metros de b?squeda personalizados</param>
        public void ActualizarParametrosBusquedaPersonalizados(Guid organizacionID, Guid pProyectoID, List<ParametroBusquedaPersonalizadoModel> listaParametros)
        {
            foreach (ParametroBusquedaPersonalizadoModel param in listaParametros)
            {
                ProyectoSearchPersonalizado proyecto = mEntityContext.ProyectoSearchPersonalizado.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.NombreFiltro.Equals(param.NombreParametro)).FirstOrDefault();
                if (proyecto == null)
                {
                    if (param.NombreParametro != null)
                    {
                        ProyectoSearchPersonalizado nuevoProyecto = new ProyectoSearchPersonalizado();
                        nuevoProyecto.OrganizacionID = organizacionID;
                        nuevoProyecto.ProyectoID = pProyectoID;
                        nuevoProyecto.NombreFiltro = param.NombreParametro;
                        nuevoProyecto.WhereSPARQL = param.WhereParametro == null ? string.Empty : param.WhereParametro;
                        nuevoProyecto.OrderBySPARQL = param.OrderByParametro == null ? string.Empty : param.OrderByParametro;
                        nuevoProyecto.WhereFacetasSPARQL = param.WhereFacetaParametro == null ? string.Empty : param.WhereFacetaParametro;
                        nuevoProyecto.OmitirRdfType = false;

                        mEntityContext.ProyectoSearchPersonalizado.Add(nuevoProyecto);
                    }

                }
                else
                {
                    if (param.Deleted)
                    {
                        mEntityContext.Entry(proyecto).State = EntityState.Deleted;
                        List<ProyectoPestanyaBusqueda> listaProyectoPestanyaBusqueda = mEntityContext.ProyectoPestanyaBusqueda.JoinProyectoPestanyaMenu().Where(x => x.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID) && x.ProyectoPestanyaBusqueda.SearchPersonalizado.Equals(param.NombreParametro)).Select(item=>item.ProyectoPestanyaBusqueda).ToList();
                        foreach(ProyectoPestanyaBusqueda proyectoPestanyaBusqueda in listaProyectoPestanyaBusqueda) 
                        {
                            proyectoPestanyaBusqueda.SearchPersonalizado = "";
                        }
                    }
                    else
                    {
                        if (param.NombreParametro != null)
                        {
                            proyecto.NombreFiltro = param.NombreParametro;
                            proyecto.WhereSPARQL = param.WhereParametro == null ? string.Empty : param.WhereParametro;
                            proyecto.OrderBySPARQL = param.OrderByParametro == null ? string.Empty : param.OrderByParametro;
                            proyecto.WhereFacetasSPARQL = param.WhereFacetaParametro == null ? string.Empty : param.WhereFacetaParametro;
                        }
                    }
                }

            }
            ActualizarBaseDeDatosEntityContext();

        }
        /// <summary>
        /// Comprueba si existe el proyecto FAQ
        /// </summary>
        /// <returns>TRUE si existe el proyecto FAQ</returns>
        public bool ExisteProyectoFAQ()
        {
            DbCommand comandoExiste = ObtenerComando(this.sqlSelectExisteProyectoFAQ);

            object resultado = EjecutarEscalar(comandoExiste);

            if ((resultado != null) && (resultado.Equals(1)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Comprueba si existe el proyecto Noticias de Gnoss
        /// </summary>
        /// <returns>TRUE si existe el proyecto Noticias de Gnoss</returns>
        public bool ExisteProyectoNoticias()
        {
            DbCommand comandoExiste = ObtenerComando(this.sqlSelectExisteProyectoNoticias);

            object resultado = EjecutarEscalar(comandoExiste);

            if ((resultado != null) && (resultado.Equals(1)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Comprueba si existe el proyecto Didactalia de Gnoss
        /// </summary>
        /// <returns>TRUE si existe el proyecto Didactalia de Gnoss</returns>
        public bool ExisteProyectoDidactalia()
        {
            DbCommand comandoExiste = ObtenerComando(this.sqlSelectExisteProyectoDidactalia);

            object resultado = EjecutarEscalar(comandoExiste);

            if ((resultado != null) && (resultado.Equals(1)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Obtiene los proyectos de una organizaci?n a partir de una lista de proyectos pasados por par?metro
        /// </summary>
        /// <param name="pListaProyectoID">Lista de identificadores de proyecto</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosPorID(List<Guid> pListaProyectoID)
        {
            DataWrapperProyecto dataWrapperproyectoDS = new DataWrapperProyecto();

            if (pListaProyectoID.Count > 0)
            {
                List<Proyecto> listaProyectos = mEntityContext.Proyecto.Where(proyecto => pListaProyectoID.Contains(proyecto.ProyectoID)).ToList();
                List<ProyectoAgCatTesauro> listaProyectosAgCatTesauro = mEntityContext.ProyectoAgCatTesauro.Where(proyectoAgCatTesauro => pListaProyectoID.Contains(proyectoAgCatTesauro.ProyectoID)).ToList();
                List<AdministradorProyecto> listaAdministradoresProyecto = mEntityContext.AdministradorProyecto.Where(adminProyecto => pListaProyectoID.Contains(adminProyecto.ProyectoID)).ToList();
                dataWrapperproyectoDS.ListaProyecto = listaProyectos;
                dataWrapperproyectoDS.ListaProyectoAgCatTesauro = listaProyectosAgCatTesauro;
                dataWrapperproyectoDS.ListaAdministradorProyecto = listaAdministradoresProyecto;
            }
            return (dataWrapperproyectoDS);
        }

        /// <summary>
        /// Obtiene proyectos (carga ligera) a partir de la lista de sus identificadores pasada como par?metro
        /// </summary>
        /// <param name="pListaProyectoID">Lista de identificadores de proyecto</param>
        /// <param name="pTraerSoloAbiertos">True si s?lo se quieren obtener los proyectos de la lista que est?n abiertos (False por defecto)</param>
        /// <param name="pLimite">N?mero de proyectos m?ximo a obtener</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosPorIDsCargaLigera(List<Guid> pListaProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proyecto => pListaProyectoID.Contains(proyecto.ProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene (carga ligera) los datos de los proyectos mas populares a los que no pertenece el usuario
        /// </summary>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <param name="pNumeroProyectos">N?mero de proyectos</param>
        /// <returns>Dataset de proyecto</returns>
        public List<Proyecto> ObtenerProyectosRecomendadosPorPersona(Guid pPersonaID, int pNumeroProyectos)
        {
            List<Proyecto> listaProyectos = new List<Proyecto>();
            if (pNumeroProyectos > 0)
            {
                var varGuids = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
                {
                    ProyectoID = identidad.ProyectoID,
                    PersonaID = perfil.PersonaID
                }).Where(selecccion => selecccion.PersonaID.Value.Equals(pPersonaID));

                List<Guid> listaGuids = new List<Guid>();
                foreach (var id in varGuids.ToList())
                {
                    listaGuids.Add(id.ProyectoID);
                }

                var lista = mEntityContext.ProyectosMasActivos.Join(mEntityContext.Proyecto, proyectosMasActivos => proyectosMasActivos.ProyectoID, proyecto => proyecto.ProyectoID, (proyectosMasActivos, proyecto) => new
                {
                    ProyectoID = proyectosMasActivos.ProyectoID,
                    TipoAcceso = proyecto.TipoAcceso,
                    Estado = proyecto.Estado,
                    Peso = proyectosMasActivos.Peso
                }).Where(proyectoMasActivo => proyectoMasActivo.TipoAcceso.Equals((short)TipoAcceso.Publico) && proyectoMasActivo.Estado.Equals((short)EstadoProyecto.Abierto) && !listaGuids.Contains(proyectoMasActivo.ProyectoID)).OrderByDescending(proyectoMasActivo => proyectoMasActivo.Peso).Take(pNumeroProyectos);

                List<Guid> listaIdsProyectosMasActivos = new List<Guid>();
                foreach (var id in lista.ToList())
                {
                    listaIdsProyectosMasActivos.Add(id.ProyectoID);
                }

                if (listaIdsProyectosMasActivos.Count > 0)
                {
                    listaProyectos = mEntityContext.Proyecto.Where(proyecto => listaIdsProyectosMasActivos.Contains(proyecto.ProyectoID)).ToList();
                }
            }
            return listaProyectos;
        }

        public DataWrapperProyecto ObtenerProyectoDashboardPorID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoPestanyaMenu = mEntityContext.ProyectoPestanyaMenu.Where(proyectoPestanyaMenu => proyectoPestanyaMenu.ProyectoID.Equals(pProyectoID)).OrderBy(proyecto => proyecto.Orden).ToList();
            dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistente = mEntityContext.ProyectoPestanyaDashboardAsistente.Join(mEntityContext.ProyectoPestanyaMenu, proyPestDashAsis => proyPestDashAsis.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestDashAsis, proyPestMenu) => new
            {
                proyectoPestanyaDashboardAsistente = proyPestDashAsis,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectoPestanyaDashboardAsistente).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistenteDataset = mEntityContext.ProyectoPestanyaDashboardAsistenteDataset.Join(mEntityContext.ProyectoPestanyaDashboardAsistente, proyPestDashAsisData => proyPestDashAsisData.AsisID, proyPestDashAsis => proyPestDashAsis.AsisID, (proyPestDashAsisData, proyPestDashAsis) => new
            {
                proyectoPestanyaDashboardAsistenteData = proyPestDashAsisData,
                proyectoPestanyaDashboardAsistente = proyPestDashAsis
            }).Join(mEntityContext.ProyectoPestanyaMenu, proyPestDashAsis => proyPestDashAsis.proyectoPestanyaDashboardAsistente.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestDashAsis, proyPestMenu) => new
            {
                proyectoPestanyaDashboardAsistenteData = proyPestDashAsis.proyectoPestanyaDashboardAsistenteData,
                proyectoPestanyaDashboardAsistente = proyPestDashAsis.proyectoPestanyaDashboardAsistente,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectoPestanyaDashboardAsistenteData).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene un proyecto (Proyecto, AdministradorProyecto,ProyectoAgCatTesauro, ProyectoCerradoTmp, ProyectoCerrandose, DatoExtraProyecto, DatoExtraProyectoOpcion, DatoExtraProyectoVirtuoso, ProyectoPasoRegistro, CamposRegistroProyectoGenericos,ProyectoPestanyaMenu, ProyectoPestanyaCMS, ProyectoPestanyaBusqueda, ProyectoPestanyaMenuRolIdetndidad, ProyectoPestanyaMenuRolGrupoIdentidades) a partir de su identificador
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de proyecto con el proyecto buscado</returns>
        public DataWrapperProyecto ObtenerProyectoPorID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaAdministradorGrupoProyecto = mEntityContext.AdministradorGrupoProyecto.Where(adminGroupProy => adminGroupProy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoAgCatTesauro = mEntityContext.ProyectoAgCatTesauro.Where(proyectoAgCatTesauro => proyectoAgCatTesauro.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoCerradoTmp = mEntityContext.ProyectoCerradoTmp.Where(proyectoCerradoTmp => proyectoCerradoTmp.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoCerrandose = mEntityContext.ProyectoCerrandose.Where(proyectoCerrandose => proyectoCerrandose.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyectoOpcion = mEntityContext.DatoExtraProyectoOpcion.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoPasoRegistro = mEntityContext.ProyectoPasoRegistro.Where(proyectoPasoRegistro => proyectoPasoRegistro.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaFacetaObjetoConocimientoProyectoPestanya = mEntityContext.FacetaObjetoConocimientoProyectoPestanya.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaCamposRegistroProyectoGenericos = mEntityContext.CamposRegistroProyectoGenericos.Where(campos => campos.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoPestanyaMenu = mEntityContext.ProyectoPestanyaMenu.Where(proyectoPestanyaMenu => proyectoPestanyaMenu.ProyectoID.Equals(pProyectoID)).OrderBy(proyecto => proyecto.Orden).ToList();
            dataWrapperProyecto.ListaProyectoPestanyaCMS = mEntityContext.ProyectoPestanyaCMS.Join(mEntityContext.ProyectoPestanyaMenu, proyPestanyaCMS => proyPestanyaCMS.PestanyaID, proyPestanyaMenu => proyPestanyaMenu.PestanyaID, (proyPestanyaCMS, proyPestanyaMenu) => new
            {
                ProyectoPestanyaCMS = proyPestanyaCMS,
                ProyectPestanyaMenuID = proyPestanyaMenu.ProyectoID
            }).Where(proyPestanyaCMS => proyPestanyaCMS.ProyectPestanyaMenuID.Equals(pProyectoID)).Select(proyPestanyaCMS => proyPestanyaCMS.ProyectoPestanyaCMS).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaBusqueda = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPestBusqueda => proyPestBusqueda.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestBusqueda, proyPestMenu) => new
            {
                ProyectPestanyaBusqueda = proyPestBusqueda,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.ProyectPestanyaBusqueda).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaBusquedaPesoOC = mEntityContext.ProyectoPestanyaBusquedaPesoOC.Where(proyPestBusquedaPesoOC => proyPestBusquedaPesoOC.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades = mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Join(mEntityContext.ProyectoPestanyaMenu, proyPestMenuRogrupolId => proyPestMenuRogrupolId.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestMenuRogrupolId, proyPestMenu) => new
            {
                proyPestMenuRogrupolId = proyPestMenuRogrupolId,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyPestMenuRogrupolId).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenuRolIdentidad = mEntityContext.ProyectoPestanyaMenuRolIdentidad.Join(mEntityContext.ProyectoPestanyaMenu, proyPestRolId => proyPestRolId.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestRolId, proyPestMenu) => new
            {
                proyectPestanyaRolIdentidad = proyPestRolId,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectPestanyaRolIdentidad).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos = mEntityContext.ProyectoPestanyaFiltroOrdenRecursos.Join(mEntityContext.ProyectoPestanyaMenu, proyPestFiltro => proyPestFiltro.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestFiltro, proyPestMenu) => new
            {
                proyectoPestanyaFiltroOrdenRecursos = proyPestFiltro,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectoPestanyaFiltroOrdenRecursos).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistente = mEntityContext.ProyectoPestanyaDashboardAsistente.Join(mEntityContext.ProyectoPestanyaMenu, proyPestDashAsis => proyPestDashAsis.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestDashAsis, proyPestMenu) => new
            {
                proyectoPestanyaDashboardAsistente = proyPestDashAsis,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectoPestanyaDashboardAsistente).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistenteDataset = mEntityContext.ProyectoPestanyaDashboardAsistenteDataset.Join(mEntityContext.ProyectoPestanyaDashboardAsistente, proyPestDashAsisData => proyPestDashAsisData.AsisID, proyPestDashAsis => proyPestDashAsis.AsisID, (proyPestDashAsisData, proyPestDashAsis) => new
            {
                proyectoPestanyaDashboardAsistenteData = proyPestDashAsisData,
                proyectoPestanyaDashboardAsistente = proyPestDashAsis
            }).Join(mEntityContext.ProyectoPestanyaMenu, proyPestDashAsis => proyPestDashAsis.proyectoPestanyaDashboardAsistente.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestDashAsis, proyPestMenu) => new
            {
                proyectoPestanyaDashboardAsistenteData = proyPestDashAsis.proyectoPestanyaDashboardAsistenteData,
                proyectoPestanyaDashboardAsistente = proyPestDashAsis.proyectoPestanyaDashboardAsistente,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectoPestanyaDashboardAsistenteData).ToList();

            dataWrapperProyecto.ListaProyectoEventoAccion = mEntityContext.ProyectoEventoAccion.Where(proyectoEvento => proyectoEvento.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaAccionesExternasProyecto = mEntityContext.AccionesExternasProyecto.Where(acciones => acciones.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaProyectoSearchPersonalizado = mEntityContext.ProyectoSearchPersonalizado.Where(proyectoSearchPersonalizado => proyectoSearchPersonalizado.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaOntologiaProyecto = mEntityContext.OntologiaProyecto.Where(ontologiaProy => ontologiaProy.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaVistaVirtualProyecto = mEntityContext.VistaVirtualProyecto.Where(vistaVirtualProy => vistaVirtualProy.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaConfigAutocompletarProy = mEntityContext.ConfigAutocompletarProy.Where(config => config.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoPestanyaBusquedaPesoOC = mEntityContext.ProyectoPestanyaBusquedaPesoOC.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        public List<Guid> ObtenerCategoriasProyecto(Guid pProyectoID)
        {
            List<Guid> categoriasDeComunidad = mEntityContext.TesauroProyecto.Join(mEntityContext.CategoriaTesauro, item => item.TesauroID, item => item.TesauroID, (tesauroProyecto, categoriaTesauro) => new
            {
                TesauroProyecto = tesauroProyecto,
                CategoriaTesauro = categoriaTesauro
            }).Where(item => item.TesauroProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.CategoriaTesauro.CategoriaTesauroID).ToList();
            return categoriasDeComunidad;
        }


        /// <summary>
        /// Obtiene los datos Extra de un proyecto (incluidos los del ecosistema) (para los regisrtros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperProyecto ObtenerDatosExtraProyectoPorID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaDatoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(dato => dato.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraProyectoOpcion = mEntityContext.DatoExtraProyectoOpcion.Where(dato => dato.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(dato => dato.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistema = mEntityContext.DatoExtraEcosistema.ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistemaOpcion = mEntityContext.DatoExtraEcosistemaOpcion.ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuoso.ToList();

            dataWrapperProyecto.ListaCamposRegistroProyectoGenericos = mEntityContext.CamposRegistroProyectoGenericos.Where(campos => campos.ProyectoID.Equals(pProyectoID)).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los datos Extra de un proyecto (incluidos los del ecosistema) (para los registros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperProyecto ObtenerDatosExtraProyectoPorListaIDs(List<Guid> pListaProyectosID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaDatoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(dato => pListaProyectosID.Contains(dato.ProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraProyectoOpcion = mEntityContext.DatoExtraProyectoOpcion.Where(dato => pListaProyectosID.Contains(dato.ProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(dato => pListaProyectosID.Contains(dato.ProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistema = mEntityContext.DatoExtraEcosistema.ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistemaOpcion = mEntityContext.DatoExtraEcosistemaOpcion.ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuoso.ToList();

            dataWrapperProyecto.ListaCamposRegistroProyectoGenericos = mEntityContext.CamposRegistroProyectoGenericos.Where(campo => pListaProyectosID.Contains(campo.ProyectoID)).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene las preferencias disponibles en un proyecto (para los registros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla PreferenciasProyecto</returns>
        public DataWrapperProyecto ObtenerPreferenciasProyectoPorID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaPreferenciaProyecto = mEntityContext.PreferenciaProyecto.Where(preferencia => preferencia.ProyectoID.Equals(pProyectoID)).OrderBy(preferencia => preferencia.Orden).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene las acciones externas en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla AccionesExternasProyecto</returns>
        public DataWrapperProyecto ObtenerAccionesExternasProyectoPorProyectoID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaAccionesExternasProyecto = mEntityContext.AccionesExternasProyecto.Where(acciones => acciones.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene las acciones externas en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla AccionesExternasProyecto</returns>
        public DataWrapperProyecto ObtenerAccionesExternasProyectoPorListaIDs(List<Guid> pListaProyectosID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaAccionesExternasProyecto = mEntityContext.AccionesExternasProyecto.Where(acciones => pListaProyectosID.Contains(acciones.ProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los eventos disponibles en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public DataWrapperProyecto ObtenerEventosProyectoPorProyectoID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyectoEvento = mEntityContext.ProyectoEvento.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los eventos disponibles en un proyecto para una identidad
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pIdentidadID">Identidad de la que se quieren obtener los eventos a los que est? suscrita</param>
        /// <returns>DataSet de los eventos a los que est? suscrita la identidad</returns>
        public DataSet ObtenerEventoProyectoIdentidadID(Guid pProyectoID, Guid pIdentidadID)
        {
            DataSet ds = new DataSet();

            DbCommand commandsqlSelectEventosProyectoPorIdentidadID = ObtenerComando(sqlSelectEventosProyectoPorIdentidadID);
            AgregarParametro(commandsqlSelectEventosProyectoPorIdentidadID, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            AgregarParametro(commandsqlSelectEventosProyectoPorIdentidadID, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            CargarDataSet(commandsqlSelectEventosProyectoPorIdentidadID, ds, "EventosProyectoIdentidad");

            return ds;
        }


        /// <summary>
        /// Obtiene el evento con ese ID
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public DataWrapperProyecto ObtenerEventoProyectoPorEventoID(Guid pEventoID, bool pSoloActivos)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoEvento> listaProyectoEvento = mEntityContext.ProyectoEvento.Where(proyecto => proyecto.EventoID.Equals(pEventoID)).ToList();
            if (pSoloActivos)
            {
                listaProyectoEvento = listaProyectoEvento.Where(proyecto => proyecto.Activo).ToList();
            }
            dataWrapperProyecto.ListaProyectoEvento = listaProyectoEvento;

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los participantes de un evento con ese ID
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyectoParticipante</returns>
        public DataWrapperProyecto ObtenerEventoProyectoParticipantesPorEventoID(Guid pEventoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoEventoParticipante = mEntityContext.ProyectoEventoParticipante.Where(proyecto => proyecto.EventoID.Equals(pEventoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene el numero participantes de eventos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Diccionario con clave evento y valor numero de participantes</returns>
        public Dictionary<Guid, int> ObtenerNumeroParticipantesEventosPorProyectoID(Guid pProyectoID)
        {
            Dictionary<Guid, int> EventosParticipantes = new Dictionary<Guid, int>();

            var resultadoConsulta = mEntityContext.ProyectoEventoParticipante.Join(mEntityContext.ProyectoEvento, proyectEvenParti => proyectEvenParti.EventoID, proyectoEvento => proyectoEvento.EventoID, (proyectEvenParti, proyectoEvento) => new
            {
                ProyectoEventoParticipante = proyectEvenParti,
                ProyectoEvento = proyectoEvento
            }).Where(objeto => objeto.ProyectoEvento.ProyectoID.Equals(pProyectoID)).GroupBy(objeto => objeto.ProyectoEventoParticipante.EventoID, objeto => objeto.ProyectoEventoParticipante.IdentidadID, (agrupacion, g) => new
            {
                EventoID = agrupacion,
                NumParticipantes = g.ToList().Count
            }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid idEvento = fila.EventoID;
                int numParticipantes = fila.NumParticipantes;
                EventosParticipantes.Add(idEvento, numParticipantes);
            }


            return EventosParticipantes;
        }

        /// <summary>
        /// Obtiene si una identidad participa en un evento
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <param name="pIdentidad">Clave de la identidad</param>
        /// <returns>TRUE si participa en el evento</returns>
        public bool ObtenerSiIdentidadParticipaEnEvento(Guid pEventoID, Guid pIdentidad)
        {
            List<ProyectoEventoParticipante> lista = mEntityContext.ProyectoEventoParticipante.Where(proyecto => proyecto.EventoID.Equals(pEventoID) && proyecto.IdentidadID.Equals(pIdentidad)).ToList();
            bool participa = (lista.Count > 0);

            return (participa);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por par?metro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuario(Guid pUsuarioID)
        {
            return ObtenerProyectosParticipaUsuario(pUsuarioID, false);
        }

        /// <summary>
        /// Obtiene el valor de un texto traducido a un idioma especifico
        /// </summary>
        /// <param name="pCommunityShortName">Nombre corto de la comuidad</param>
        /// <param name="pLanguage">Idioma del texto</param>
        /// <param name="pTextoID">Clave del texto</param>
        /// <returns>Texto en el idioma especificado</returns>
        public string ObtenerValorTraduccionDeTexto(string pCommunityShortName, string pLanguage, string pTextoID)
        {
            Guid proyectoID = mEntityContext.Proyecto.Where(item => item.NombreCorto.Equals(pCommunityShortName)).Select(item => item.ProyectoID).FirstOrDefault();
            Guid personalizacionID = mEntityContext.VistaVirtualProyecto.Where(item => item.ProyectoID.Equals(proyectoID)).Select(item => item.PersonalizacionID).FirstOrDefault();
            if (!personalizacionID.Equals(Guid.Empty))
            {
                return mEntityContext.TextosPersonalizadosPersonalizacion.Where(item => item.PersonalizacionID.Equals(personalizacionID) && item.TextoID.Equals(pTextoID) && item.Language.Equals(pLanguage)).Select(item => item.Texto).FirstOrDefault();
            }
            else
            {
                throw new ExcepcionWeb($"El proyecto {pCommunityShortName} no tiene personalizaci?n");
            }
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por par?metro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoDocumentoCompartido">Tipo del recurso que va a compartir el usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosUsuarioPuedeCompartirRecurso(Guid pUsuarioID, TiposDocumentacion pTipoDocumentoCompartido)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            if (pTipoDocumentoCompartido != TiposDocumentacion.EntradaBlog)
            {
                dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
                {
                    Proyecto = proyecto,
                    ProyectoUsuarioIdentidad = proyUserIden
                }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.Proyecto.OrganizacionID, objeto.Proyecto.ProyectoID, objeto.ProyectoUsuarioIdentidad.UsuarioID }, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID, adminProy.UsuarioID }, (objeto, adminProy) => new
                {
                    Proyecto = objeto.Proyecto,
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    AdministradorProyecto = adminProy
                }).Join(mEntityContext.TipoDocDispRolUsuarioProy, objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.OrganizacionID }, tipo => new { tipo.ProyectoID, tipo.OrganizacionID }, (objeto, tipo) => new
                {
                    Proyecto = objeto.Proyecto,
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    AdministradorProyecto = objeto.AdministradorProyecto,
                    TipoDocDispRolUsuarioProy = tipo
                }).Where(objeto => objeto.TipoDocDispRolUsuarioProy.RolUsuario >= objeto.AdministradorProyecto.Tipo && objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) && objeto.TipoDocDispRolUsuarioProy.TipoDocumento.Equals((short)pTipoDocumentoCompartido)).Select(objeto => objeto.Proyecto).Concat(
                    mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, usuarioIdent => new { OrganizacionID = usuarioIdent.OrganizacionGnossID, usuarioIdent.ProyectoID }, (proyecto, usuarioIden) => new
                    {
                        Proyecto = proyecto,
                        ProyectoUsuarioIdentidad = usuarioIden
                    }).Join(mEntityContext.TipoDocDispRolUsuarioProy, objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.OrganizacionID }, tipoDoc => new { tipoDoc.ProyectoID, tipoDoc.OrganizacionID }, (objeto, tipoDoc) => new
                    {
                        Proyecto = objeto.Proyecto,
                        ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                        TipoDocDispRolUsuarioProy = tipoDoc
                    }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) && objeto.TipoDocDispRolUsuarioProy.RolUsuario.Equals((short)TipoRolUsuario.Usuario) && objeto.TipoDocDispRolUsuarioProy.TipoDocumento.Equals((short)pTipoDocumentoCompartido)).Select(objeto => objeto.Proyecto)
                    ).Concat(
                        mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (proyecto, adminProy) => new
                        {
                            Proyecto = proyecto,
                            AdministradorProyecto = adminProy
                        }).Join(mEntityContext.TesauroProyecto, objeto => objeto.Proyecto.ProyectoID, tesauro => tesauro.ProyectoID, (objeto, tesauro) => new
                        {
                            Proyecto = objeto.Proyecto,
                            AdministradorProyecto = objeto.AdministradorProyecto,
                            TesauroProyecto = tesauro
                        }).Join(mEntityContext.CategoriaTesauro, objeto => objeto.TesauroProyecto.TesauroID, catTes => catTes.TesauroID, (objeto, catTes) => new
                        {
                            Proyecto = objeto.Proyecto,
                            AdministradorProyecto = objeto.AdministradorProyecto,
                            TesauroProyecto = objeto.TesauroProyecto,
                            CategoriaTesauro = catTes
                        }).Where(objeto => objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID)).Select(objeto => objeto.Proyecto)
                    ).OrderBy(objeto => objeto.Nombre).Distinct().ToList();
            }
            else
            {
                dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
                {
                    Proyecto = proyecto,
                    ProyectoUsuarioIdentidad = proyUserIden
                }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto)).Select(objeto => objeto.Proyecto).Concat(

                    mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (Proyecto, adminProy) => new
                    {
                        Proyecto = Proyecto,
                        AdministradorProyecto = adminProy
                    }).Join(mEntityContext.TesauroProyecto, objeto => objeto.Proyecto.ProyectoID, tesauroProy => tesauroProy.ProyectoID, (objeto, tesuaroProy) => new
                    {
                        Proyecto = objeto.Proyecto,
                        AdministradorProyecto = objeto.AdministradorProyecto,
                        TesauroProyecto = tesuaroProy
                    }).Join(mEntityContext.CategoriaTesauro, objeto => objeto.TesauroProyecto.TesauroID, catTes => catTes.TesauroID, (objeto, catTes) => new
                    {
                        Proyecto = objeto.Proyecto,
                        AdministradorProyecto = objeto.AdministradorProyecto,
                        TesauroProyecto = objeto.TesauroProyecto,
                        CategoriaTesauro = catTes
                    }).Where(objeto => objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID)).Select(objeto => objeto.Proyecto)
                    ).OrderBy(objeto => objeto.Nombre).Distinct().ToList();
            }

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario en modo personal o profesional personal
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioEnModoPersonal(Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            mEntityContext.Database.SetCommandTimeout(1);

            var resultadoConsulta = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyectoUsuarioIden => new { OrganizacionID = proyectoUsuarioIden.OrganizacionGnossID, proyectoUsuarioIden.ProyectoID }, (proyecto, proyectoUsuarioIden) => new
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentidad = proyectoUsuarioIden
            }).Join(mEntityContext.ProyectoRolUsuario, objeto => new { objeto.Proyecto.OrganizacionID, objeto.Proyecto.ProyectoID, objeto.ProyectoUsuarioIdentidad.UsuarioID }, proyRolUser => new { OrganizacionID = proyRolUser.OrganizacionGnossID, proyRolUser.ProyectoID, proyRolUser.UsuarioID }, (objeto, proyRolUser) => new
            {
                Proyecto = objeto.Proyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                ProyectoRolUsuario = proyRolUser
            }).Join(mEntityContext.Identidad, objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                ProyectoRolUsuario = objeto.ProyectoRolUsuario,
                Identidad = identidad
            })
            .Where(objeto => objeto.ProyectoRolUsuario.UsuarioID.Equals(pUsuarioID) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose)) && !objeto.ProyectoRolUsuario.EstaBloqueado && (objeto.Identidad.Tipo < 2 || objeto.Identidad.Tipo == 4) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue).OrderBy(objeto => objeto.Proyecto.Nombre);

            dataWrapperProyecto.ListaProyecto = resultadoConsulta.Select(objeto => objeto.Proyecto).Distinct().ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por par?metro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pSoloUsuariosSinBloquear">Indique si deben traerse solo los usuarios sin bloquear o todos</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuario(Guid pUsuarioID, bool pSoloUsuariosSinBloquear)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            var lista = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentidad = proyUserIden
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID)).OrderBy(objeto => objeto.Proyecto.Nombre).Select(objeto => objeto.Proyecto).ToList();
            if (pSoloUsuariosSinBloquear)
            {
                lista = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
                {
                    Proyecto = proyecto,
                    ProyectoUsuarioIdentidad = proyUserIden
                }).Join(mEntityContext.ProyectoRolUsuario, objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.OrganizacionID }, proyectoRolIden => new { proyectoRolIden.ProyectoID, OrganizacionID = proyectoRolIden.OrganizacionGnossID }, (objeto, proyectoRolIden) => new
                {
                    Proyecto = objeto.Proyecto,
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    ProyectoRolUsuario = proyectoRolIden
                }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && !objeto.ProyectoRolUsuario.EstaBloqueado).OrderBy(objeto => objeto.Proyecto.Nombre).Select(objeto => objeto.Proyecto).ToList();
            }

            dataWrapperProyecto.ListaProyecto = lista;

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por par?metro.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosParticipaUsuarioSinBloquearNiAbandonar(Guid pUsuarioID)
        {
            var resParaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID.Value, persona => persona.PersonaID, (objeto, persona) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID) && objeto.Perfil.PersonaID.HasValue && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue).Select(objeto => new { ProyectoID = objeto.Proyecto.ProyectoID, NombreCorto = objeto.Proyecto.NombreCorto }).ToList();

            Dictionary<Guid, string> proys = new Dictionary<Guid, string>();

            foreach (var fila in resParaProyecto)
            {
                if (!proys.ContainsKey(fila.ProyectoID))
                {
                    proys.Add(fila.ProyectoID, fila.NombreCorto);
                }
            }

            return proys;
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por par?metro.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoDoc">Tipo de documento que se va a cargar</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosParticipaUsuarioSinBloquearNiAbandonarYConfigurables(Guid pUsuarioID)
        {
            DataSet proyectoDS = new DataSet();

            //Consulta que devuelve los proyectos a los que pertenece el usuario y puede cargar recursos a trav?s del programa de carga masiva
            string sqlCargasMasivas = "SELECT Proyecto.ProyectoID, NombreCorto FROM Proyecto INNER JOIN Identidad ON (Proyecto.ProyectoID=Identidad.ProyectoID AND Proyecto.OrganizacionID=Identidad.OrganizacionID) INNER JOIN Perfil ON (Identidad.PerfilID=Perfil.PerfilID) INNER JOIN Persona ON (Perfil.PersonaID=Persona.PersonaID) INNER JOIN TipoDocDispRolUsuarioProy ON (Proyecto.ProyectoID=TipoDocDispRolUsuarioProy.ProyectoID) WHERE Persona.UsuarioID = " + IBD.GuidValor(pUsuarioID) + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL AND TipoDocDispRolUsuarioProy.TipoDocumento = " + (int)TiposDocumentacion.CargasMasivas + " AND TipoDocDispRolUsuarioProy.RolUsuario >= (";

            //Subconsulta que devuelve el rol del usuario del proyecto o pro defecto siempre 2.
            string subConsulta = "(SELECT coalesce(AdministradorProyecto.Tipo , 2) FROM AdministradorProyecto WHERE AdministradorProyecto.ProyectoID = Proyecto.ProyectoID AND AdministradorProyecto.UsuarioID =" + IBD.GuidValor(pUsuarioID) + ") UNION (SELECT 2 FROM AdministradorProyecto WHERE not exists(SELECT coalesce(AdministradorProyecto.Tipo , 2) FROM AdministradorProyecto WHERE AdministradorProyecto.ProyectoID = Proyecto.ProyectoID AND AdministradorProyecto.UsuarioID =" + IBD.GuidValor(pUsuarioID) + "))";

            //Agnadimos la subconsulta que devuelve 2 o el rol de usuario en un determinado proyecto.
            sqlCargasMasivas += subConsulta + ") Group by Proyecto.ProyectoID, NombreCorto";

            //Consulta que devuelve los proyectos a los que pertenece el usuario y se pueden cargar recursos de cualquir tipo (imagenes/v?deos/archivos... etc)
            string sqlTipoDoc = "SELECT Proyecto.ProyectoID, NombreCorto FROM Proyecto INNER JOIN Identidad ON (Proyecto.ProyectoID=Identidad.ProyectoID AND Proyecto.OrganizacionID=Identidad.OrganizacionID) INNER JOIN Perfil ON (Identidad.PerfilID=Perfil.PerfilID) INNER JOIN Persona ON (Perfil.PersonaID=Persona.PersonaID) INNER JOIN TipoDocDispRolUsuarioProy ON (Proyecto.ProyectoID=TipoDocDispRolUsuarioProy.ProyectoID) WHERE Persona.UsuarioID = " + IBD.GuidValor(pUsuarioID) + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL AND TipoDocDispRolUsuarioProy.TipoDocumento = " + (int)TiposDocumentacion.FicheroServidor + " AND TipoDocDispRolUsuarioProy.RolUsuario >= (" + subConsulta + ") Group by Proyecto.ProyectoID, NombreCorto";

            //Consulta que devuelve los proyectos en los que el usuario es el administrador
            string sqlAdministra = "SELECT Proyecto.ProyectoID, NombreCorto FROM Proyecto INNER JOIN AdministradorProyecto ON Proyecto.ProyectoID = AdministradorProyecto.ProyectoID INNER JOIN Identidad ON (Proyecto.ProyectoID=Identidad.ProyectoID AND Proyecto.OrganizacionID=Identidad.OrganizacionID) INNER JOIN Perfil ON (Identidad.PerfilID=Perfil.PerfilID) INNER JOIN Persona ON (Perfil.PersonaID=Persona.PersonaID AND Persona.UsuarioID=AdministradorProyecto.UsuarioID)  WHERE AdministradorProyecto.Tipo = " + (short)TipoRolUsuario.Administrador + " AND AdministradorProyecto.UsuarioID = " + IBD.GuidValor(pUsuarioID) + " AND Proyecto.ProyectoID != " + IBD.GuidValor(ProyectoAD.MetaProyecto) + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL ";

            //INTERSECT entre los proyectos a los que el usuario puede subir recursos y a los que se pueden subir cualquier tipo de recursos UNION con los proyectos que administra el usuario.
            string sqlQuery = "(" + sqlCargasMasivas + " INTERSECT " + sqlTipoDoc + ") UNION " + sqlAdministra;

            DbCommand commandsqlSelectProyectosParticipaUsuario = ObtenerComando(sqlQuery);
            CargarDataSet(commandsqlSelectProyectosParticipaUsuario, proyectoDS, "Proyecto");

            Dictionary<Guid, string> proys = new Dictionary<Guid, string>();

            foreach (DataRow fila in proyectoDS.Tables[0].Rows)
            {
                if (!proys.ContainsKey((Guid)fila["ProyectoID"]))
                {
                    proys.Add((Guid)fila["ProyectoID"], (string)fila["NombreCorto"]);
                }
            }

            proyectoDS.Dispose();
            return proys;
        }


        /// <summary>
        /// Obtiene el UsuarioID de todos los miembros de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Lista de UsuarioID</returns>
        public List<Guid> ObtenerUsuarioIDMiembrosProyecto(Guid pProyectoID)
        {
            List<Guid> resultados = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioID => proyectoUsuarioID.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioID, identidad) => new { ProyectoUsuarioIdentidad = proyectoUsuarioID, Identidad = identidad }).Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.FechaBaja == null && item.Identidad.FechaExpulsion == null).Select(proyecto => proyecto.ProyectoUsuarioIdentidad.UsuarioID).ToList();

            return resultados;
        }

        /// <summary>
        /// Obtiene los proyectos administrados por el perfil pasado por par?metro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosAdministraUsuarioSinBloquearNiAbandonar(Guid pUsuarioID, bool ecosistema)
        {            
            //Proyecto           
            var resultadoConsulta = mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (proyecto, adminProy) => new
            {
                Proyecto = proyecto,
                AdministradorProyecto = adminProy
            }).Join(mEntityContext.Identidad, objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => new { PersonaID = objeto.Perfil.PersonaID.Value, objeto.AdministradorProyecto.UsuarioID }, persona => new { persona.PersonaID, UsuarioID = persona.UsuarioID.Value }, (objeto, persona) => new
            {
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                persona = persona,
                UsuarioID = persona.UsuarioID
            });

            var resultadoFinal = resultadoConsulta.Where(objeto => objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Perfil.PersonaID.HasValue && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID) && objeto.UsuarioID.HasValue && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue).Select(objeto => new
            {
                ProyectoID = objeto.Proyecto.ProyectoID,
                NombreCorto = objeto.Proyecto.NombreCorto
            }).ToList();

            if (!ecosistema)
            {
                resultadoFinal = resultadoConsulta.Where(objeto => objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Perfil.PersonaID.HasValue && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID) && objeto.UsuarioID.HasValue && !objeto.Proyecto.ProyectoID.Equals(MetaProyecto) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue).Select(objeto => new
                {
                    ProyectoID = objeto.Proyecto.ProyectoID,
                    NombreCorto = objeto.Proyecto.NombreCorto
                }).ToList();
            }


            Dictionary<Guid, string> proys = new Dictionary<Guid, string>();

            foreach (var fila in resultadoFinal)
            {
                if (!proys.ContainsKey(fila.ProyectoID))
                {
                    proys.Add(fila.ProyectoID, fila.NombreCorto);
                }
            }
            return proys;
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por par?metro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioConPerfil(Guid pUsuarioID, Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyuserIden => new { OrganizacionID = proyuserIden.OrganizacionGnossID, proyuserIden.ProyectoID }, (proyecto, proyuserIden) => new
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentidad = proyuserIden
            }).Join(mEntityContext.Identidad, objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = identidad
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Identidad.PerfilID.Equals(pPerfilID) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose))).OrderBy(objeto => objeto.Proyecto.Nombre).Select(objeto => objeto.Proyecto).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los proyectos en los que el administrador es un usuario pasado como par?metro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que el usuario es administrador</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDeUsuario(Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(pUsuarioID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los registros de AdministradorProyecto de una persona pasado como par?metro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDePersona(Guid pPersonaID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Join(mEntityContext.Persona, adminProy => new { UsuarioID = adminProy.UsuarioID }, persona => new { UsuarioID = persona.UsuarioID.Value }, (adminProy, persona) => new
            {
                AdministradorProyecto = adminProy,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.PersonaID.Equals(pPersonaID)).Select(objeto => objeto.AdministradorProyecto).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene el UsuarioID y PerfilID de los administradores de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Diccionario con perejas de UsuarioID, PerfilID</returns>
        public Dictionary<Guid, Guid> ObtenerUsuarioIDPerfilIDAdministradoresDeProyecto(Guid pProyectoID)
        {
            Dictionary<Guid, Guid> resultados = new Dictionary<Guid, Guid>();

            var resultado = mEntityContext.AdministradorProyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID, adminProy.UsuarioID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID, proyUserIden.UsuarioID }, (adminProy, proyUserIden) => new
            {
                AdministradorProyecto = adminProy,
                ProyectoUsuarioIdentidad = proyUserIden
            }).Join(mEntityContext.Identidad, objeto => new { objeto.ProyectoUsuarioIdentidad.IdentidadID, OrganizacionID = objeto.ProyectoUsuarioIdentidad.OrganizacionGnossID, objeto.ProyectoUsuarioIdentidad.ProyectoID }, identidad => new { identidad.IdentidadID, identidad.OrganizacionID, identidad.ProyectoID }, (objeto, identidad) => new
            {
                AdministradorProyecto = objeto.AdministradorProyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = identidad
            }).Where(objeto => objeto.AdministradorProyecto.Tipo == 0 && objeto.AdministradorProyecto.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (var fila in resultado)
            {
                Guid id = fila.AdministradorProyecto.UsuarioID;
                if (!resultados.ContainsKey(id))
                {
                    resultados.Add(id, fila.Identidad.PerfilID);
                }
            }
            return resultados;
        }

        /// <summary>
        /// Obtiene los datos de la tabla AdministradorProyecto de un proyecto dado
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyecto con la tabla AdministradorProyecto cargada</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDeProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperProyecto;
        }


        /// <summary>
        /// Obtiene los proyectos hijos de los proyectos que se pasan por parametros
        /// </summary>
        /// <param name="pListaProyectosID">Lista de identificadores de los proyectos</param>
        /// <param name="pUsuarioID">Identificador del usuario actual</param>
        /// <returns>Dataset de proyecto con los proyectos hijos cargados</returns>
        public DataWrapperProyecto ObtenerProyectosHijosDeProyectos(List<Guid> pListaProyectosID, Guid pUsuarioID)
        {
            List<Proyecto> listaProyecto = mEntityContext.Proyecto.Where(proyecto => (!proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado) || mEntityContext.ProyectoRolUsuario.Any(rolUsuario => !rolUsuario.EstaBloqueado && rolUsuario.OrganizacionGnossID.Equals(proyecto.OrganizacionID) && proyecto.ProyectoID.Equals(rolUsuario.ProyectoID) && rolUsuario.UsuarioID.Equals(pUsuarioID))) && pListaProyectosID.Contains((Guid)proyecto.ProyectoSuperiorID)).ToList();

            List<Guid> listaProyectos = new List<Guid>();

            foreach (Proyecto filaProy in listaProyecto)
            {
                listaProyectos.Add(filaProy.ProyectoID);
            }

            if (listaProyectos.Count > 0)
            {
                return ObtenerProyectosPorID(listaProyectos);
            }
            else
            {
                DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
                dataWrapperProyecto.ListaProyecto = new List<Proyecto>();
                return dataWrapperProyecto;
            }
        }

        /// <summary>
        /// Obtiene los proyectos padres de los proyectos que se pasan por parametros
        /// </summary>
        /// <param name="pListaProyectosID">Lista de identificadores de los proyectos</param>
        /// <returns>Dataset de proyecto con los proyectos padres cargados</returns>
        public DataWrapperProyecto ObtenerProyectosPadresDeProyectos(List<Guid> pListaProyectosID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            //Hijos            
            List<Guid> subconsulta = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoSuperiorID.HasValue && pListaProyectosID.Contains(proyecto
                  .ProyectoID)).Select(proyecto => proyecto.ProyectoSuperiorID.Value).ToList();
            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proy => subconsulta.Contains(proy.ProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene el id del proyecto padre del proyecto pasado por par?metro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del cual se quiere obtener el padre</param>
        /// <returns>Id del proyecto padre</returns>
        public Guid ObtenerProyectoPadreIDDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Where(item => item.ProyectoSuperiorID.HasValue && item.ProyectoID.Equals(pProyectoID)).Select(item => item.ProyectoSuperiorID.Value).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una identidad (no muestra los que est?n cerrados) pasada por par?metro
        /// </summary>
        /// <param name="pIdentidad">Identificador de la identidad</param>
        /// <returns>Dataset de proyectos con el proyecto en el que participa la identidad</returns>
        public DataWrapperProyecto ObtenerProyectoParticipaIdentidad(Guid pIdentidad)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentidad = proyUserIden
            }).Join(mEntityContext.Identidad, objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = identidad
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID.Equals(pIdentidad) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose)) && !objeto.Identidad.FechaBaja.HasValue).Select(objeto => objeto.Proyecto).Distinct().ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Comprueba si el usuario es administrador del metaproyecto "MYGNOSS"
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorProyectoMYGnoss(Guid pUsuarioID)
        {
            return mEntityContext.AdministradorProyecto.Any(administradorProy => administradorProy.ProyectoID.Equals(MetaProyecto) && administradorProy.UsuarioID.Equals(pUsuarioID) && administradorProy.Tipo.Equals((short)TipoRolUsuario.Administrador));
        }


        /// <summary>
        /// Comprueba si el usuario esta bloqueado en el proyecto proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EstaUsuarioBloqueadoEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return mEntityContext.ProyectoRolUsuario.Any(proyecto => proyecto.ProyectoID.Equals(pProyectoID) && proyecto.UsuarioID.Equals(pUsuarioID) && proyecto.EstaBloqueado);
        }

        /// <summary>
        /// Obtiene los proyectos (carga ligera de "Proyecto" y sus administradores "AdministradorProyecto") 
        /// en los que un usuario de organizaci?n participa con el perfil de la organizaci?n pasada por par?metro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n</param>
        /// <param name="pUsuarioID">Identificador del usuario de la organizaci?n</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioDeLaOrganizacion(Guid pOrganizacionID, Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Guid> listaProyectoID = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyUserIden => proyUserIden.IdentidadID, identidad => identidad.IdentidadID, (proyUserIden, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyUserIden,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersonaOrg, objeto => objeto.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (objeto, perfilPersonaOrg) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                PerfilPersonaOrg = perfilPersonaOrg
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(MetaProyecto)).Select(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID).ToList();
            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proyecto => listaProyectoID.Contains(proyecto.ProyectoID)).Distinct().ToList();

            //AdministradorProyecto            
            List<Guid> listaAdminProyectoID = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIden => proyectoUsuarioIden.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIden, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIden,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersonaOrg, objeto => objeto.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (objeto, perfilPersonaOrg) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                PerfilPersonaOrg = perfilPersonaOrg
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(MetaProyecto)).Select(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID).ToList();
            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => listaAdminProyectoID.Contains(adminProy.ProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Comprueba si el usuario es administrador del proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipo">Tipo del rol que se quiere comprobar</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorProyecto(Guid pUsuarioID, Guid pProyectoID, TipoRolUsuario pTipo)
        {
			bool esAdministrador = false;

			if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
			{
				RolEcosistemaUsuario rolEcosistema = mEntityContext.RolEcosistemaUsuario.Where(x => x.UsuarioID.Equals(pUsuarioID) && x.RolID.Equals(ProyectoAD.RolAdministradorEcosistema)).FirstOrDefault();
				if (rolEcosistema != null)
				{
					esAdministrador = true;
				}
			}
			else
			{
				Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS.Persona persona = mEntityContext.Persona.Where(x => x.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();
				if (persona != null)
				{
					Guid perfilID = mEntityContext.PerfilPersona.Where(x => x.PersonaID.Equals(persona.PersonaID)).Select(x => x.PerfilID).FirstOrDefault();
					Guid identidadID = mEntityContext.Identidad.Where(x => x.PerfilID.Equals(perfilID) && x.ProyectoID.Equals(pProyectoID)).Select(x => x.IdentidadID).FirstOrDefault();
					RolIdentidad rolIdentidad = mEntityContext.RolIdentidad.Where(x => x.IdentidadID.Equals(identidadID) && x.RolID.Equals(ProyectoAD.RolAdministrador)).FirstOrDefault();
					if (rolIdentidad != null)
					{
						esAdministrador = true;
					}
				}
			}

			return esAdministrador;
			/*List<AdministradorProyecto> listaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(pUsuarioID) && adminProy.Tipo.Equals((short)pTipo)).ToList();
            bool EsAdministrador = (listaAdministradorProyecto.Count > 0);
            return (EsAdministrador);*/
		}

        /// <summary>
        /// Comprueba si la identidad es administrador del proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipo">Tipo del rol que se quiere comprobar</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsIdentidadAdministradorProyecto(Guid pIdentidadID, Guid pProyectoID, TipoRolUsuario pTipo)
        {
			bool esAdministrador = false;

			if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
			{
                Guid usuarioID = mEntityContext.ProyectoUsuarioIdentidad.Where(x => x.IdentidadID.Equals(pIdentidadID) && x.ProyectoID.Equals(pProyectoID)).Select(x => x.UsuarioID).FirstOrDefault();
				RolEcosistemaUsuario rolEcosistema = mEntityContext.RolEcosistemaUsuario.Where(x => x.UsuarioID.Equals(usuarioID) && x.RolID.Equals(ProyectoAD.RolAdministradorEcosistema)).FirstOrDefault();
				if (rolEcosistema != null)
				{
					esAdministrador = true;
				}
			}
			else
			{
			    RolIdentidad rolIdentidad = mEntityContext.RolIdentidad.Where(x => x.IdentidadID.Equals(pIdentidadID) && x.RolID.Equals(ProyectoAD.RolAdministrador)).FirstOrDefault();
				if (rolIdentidad != null)
				{
				    esAdministrador = true;
				}				
			}

			return esAdministrador;
			/*var listaAdministradorProyectoVar = mEntityContext.AdministradorProyecto.Join(mEntityContext.Persona, adminProy => adminProy.UsuarioID, persona => persona.UsuarioID, (adminProy, persona) => new
            {
                ProyectoID = adminProy.ProyectoID,
                PersonaID = persona.PersonaID,
                UsuarioID = adminProy.UsuarioID,
                Proyecto = adminProy.Proyecto,
                Tipo = adminProy.Tipo
            }).Join(mEntityContext.Perfil, adminProyPer => adminProyPer.PersonaID, perfil => perfil.PersonaID, (adminProyPer, perfil) => new
            {
                ProyectoID = adminProyPer.ProyectoID,
                Proyecto = adminProyPer.Proyecto,
                PersonaID = adminProyPer.PersonaID,
                PerfilID = perfil.PerfilID,
                Tipo = adminProyPer.Tipo
            }).Join(mEntityContext.Identidad, adminProyPerPer => adminProyPerPer.PerfilID, identidad => identidad.PerfilID, (adminProyPerPer, identidad) => new
            {
                ProyectoID = adminProyPerPer.ProyectoID,
                Proyecto = adminProyPerPer.Proyecto,
                PersonaID = adminProyPerPer.PersonaID,
                PerfilID = adminProyPerPer.PerfilID,
                Tipo = adminProyPerPer.Tipo,
                IdentidadID = identidad.IdentidadID
            }).Where(adminProyPerPerIden => adminProyPerPerIden.IdentidadID.Equals(pIdentidadID) && adminProyPerPerIden.ProyectoID.Equals(pProyectoID) && adminProyPerPerIden.Tipo <= (short)pTipo).ToList();
            bool EsAdministrador = (listaAdministradorProyectoVar.ToList().Count > 0);

            return (EsAdministrador);*/
		}

        /// <summary>
        /// Obtiene nombre completo de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public string ObtenerNombreDeProyectoID(Guid pProyectoID)
        {
            string resultado = "";

            string nombre = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.Nombre).FirstOrDefault();

            if (nombre != null)
            {
                resultado = nombre;
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene nombre completo de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public Guid ObtenerProyectoSuperiorIDDeProyectoID(Guid pProyectoID)
        {

            Guid? proyectoSuperiorID = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.ProyectoSuperiorID).FirstOrDefault();

            if (proyectoSuperiorID != null)
            {
                return proyectoSuperiorID.Value;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una organizaci?n
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n</param>
        /// <param name="pExcluirMyGNOSS">TRUE si se debe de excluir de la consulta el proyecto de MyGnoss, FALSE en caso contrario</param>
        /// <returns>Dataset de proyectos</returns>
        //public DataWrapperProyecto ObtenerProyectosParticipaOrganizacion(Guid pOrganizacionID, bool pExcluirMyGNOSS)
        public DataWrapperProyecto ObtenerProyectosParticipaOrganizacion(Guid pOrganizacionID, bool pExcluirMyGNOSS)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Proyecto> proyectos = mEntityContext.Proyecto.Join(mEntityContext.OrganizacionParticipaProy, proyecto => new { ProyectoID = proyecto.ProyectoID, OrganizcionID = proyecto.OrganizacionID }, orgParticipaProy => new { ProyectoID = orgParticipaProy.ProyectoID, OrganizcionID = orgParticipaProy.OrganizacionProyectoID }, (proyecto, orgParticipaProy) => new
            {
                Proyecto = proyecto,
                OrganizacionID = orgParticipaProy.OrganizacionID
            }).Where(proyectoOrg => proyectoOrg.OrganizacionID.Equals(pOrganizacionID) && !proyectoOrg.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrado) && !proyectoOrg.Proyecto.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente)).Select(proyectoOrg => proyectoOrg.Proyecto).ToList();
            if (pExcluirMyGNOSS)
            {
                proyectos = mEntityContext.Proyecto.Join(mEntityContext.OrganizacionParticipaProy, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, orgParticipaProy => new { orgParticipaProy.ProyectoID, orgParticipaProy.OrganizacionID }, (proyecto, orgParticipaProy) => new
                {
                    Proyecto = proyecto,
                    OrganizacionID = orgParticipaProy.OrganizacionID
                }).Where(proyectoOrg => proyectoOrg.OrganizacionID.Equals(pOrganizacionID) && !proyectoOrg.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !proyectoOrg.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrado) && !proyectoOrg.Proyecto.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente)).Select(proyectoOrg => proyectoOrg.Proyecto).ToList();
            }

            dataWrapperProyecto.ListaProyecto = proyectos;
            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una organizaci?n ordenados por relevancia (N?mero de visitas en GNOSS)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaOrganizacionPorRelevancia(Guid pOrganizacionID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Proyecto> listaProyectos = mEntityContext.Proyecto.Join(mEntityContext.OrganizacionParticipaProy, proyecto => new { proyecto.ProyectoID, ORganizacionID = proyecto.OrganizacionID }, orgPartProy => new { orgPartProy.ProyectoID, ORganizacionID = orgPartProy.OrganizacionProyectoID }, (proyecto, orgPartProy) => new
            {
                Proyecto = proyecto,
                OrganizacionParticipaProy = orgPartProy
            }).Where(objeto => objeto.OrganizacionParticipaProy.OrganizacionID.Equals(pOrganizacionID) && !objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrado) && !objeto.Proyecto.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente)).Select(objeto => objeto.Proyecto).ToList();
            dataWrapperProyecto.ListaProyecto = listaProyectos.Join(mEntityContext.ProyectosMasActivos, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, proyectoMasActivo => new { proyectoMasActivo.ProyectoID, proyectoMasActivo.OrganizacionID }, (proyecto, proyectoMasActivo) => new
            {
                Proyecto = proyecto,
                ProyectosMasActivos = proyectoMasActivo
            }).OrderByDescending(objeto => objeto.ProyectosMasActivos.Peso).Select(objeto => objeto.Proyecto).ToList();

            return dataWrapperProyecto;
        }


        /// <summary>
        /// Obtiene una lista con los IDs de los proyectos en los que participa la organizacion
        /// </summary>
        /// <param name="pOrganizacion">Identificador de la organizacion</param>
        /// <param name="pObtenerSoloActivos">Indica si se debe traer los proyectos en los que ya no participa</param>
        /// <returns>Lista con los IDs de los proyectos en los que participa la organizacion</returns>
        public List<Guid> ObtenerListaProyectoIDDeOrganizacion(Guid pOrganizacion, bool pObtenerSoloActivos)
        {
            var listaProyectosVAR = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
            {
                Perfil = perfil,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion) && objeto.Perfil.OrganizacionID.Value.Equals(pOrganizacion)).ToList();

            if (pObtenerSoloActivos)
            {
                listaProyectosVAR = listaProyectosVAR.Where(objeto => objeto.Identidad.FechaBaja == null && objeto.Identidad.FechaExpulsion == null).ToList();
            }

            return listaProyectosVAR.Select(objeto => objeto.Identidad.ProyectoID).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un usuario ordenados por relevancia (N?mero de visitas del usuario)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilUsuarioPorRelevancia(Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(MetaProyecto)).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => objeto.Proyecto).ToList();

            return dataWrapperProyecto;
        }

        public DataSet ObtenerDatosProyectosDesplegarAcciones(List<Guid> pListaProyectos)
        {
            StringBuilder listaProyectos = new StringBuilder();
            foreach (Guid proyectoid in pListaProyectos)
            {
                listaProyectos.Append($"{IBD.GuidValor(proyectoid)}, ");
            }

            string select = $"SELECT Proyecto.ProyectoID, Proyecto.TipoProyecto, Proyecto.NombreCorto, ParametroGeneral.MostrarPersonasEnCatalogo, ParametroGeneral.VotacionesDisponibles, ParametroGeneral.PermitirVotacionesNegativas, ParametroGeneral.ComentariosDisponibles, ParametroGeneral.CompartirRecursosPermitido FROM ParametroGeneral INNER JOIN Proyecto ON Proyecto.ProyectoID = ParametroGeneral.ProyectoID WHERE Proyecto.ProyectoID IN ({listaProyectos.ToString().Substring(0, listaProyectos.Length - 2)})";
            DbCommand commandsql = ObtenerComando(select);

            DataSet dataset = new DataSet();
            CargarDataSet(commandsql, dataset, "DatosDesplegarAcciones");

            return dataset;
        }

        /// <summary>
        /// Obtiene los proyectos (carga ligera de "Proyecto" y sus administradores "AdministradorProyecto")
        /// en los que el usuario participa con el perfil personal
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioConSuPerfilPersonal(Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Guid> listaProyectoID = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyecto => proyecto.IdentidadID, identidad => identidad.IdentidadID, (proyecto, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersona, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                PerfilPersona = perfil
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && !objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(MetaProyecto)).Select(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID).ToList();

            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proyecto => listaProyectoID.Contains(proyecto.ProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene nombre completo de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombreDeProyectosID(List<Guid> pListaProyectosID)
        {
            Dictionary<Guid, string> listaProyectos = new Dictionary<Guid, string>();
            var proyectos = mEntityContext.Proyecto.Where(proyecto => pListaProyectosID.Contains(proyecto.ProyectoID)).Select(proyecto => new { proyecto.ProyectoID, proyecto.Nombre }).ToList();
            foreach (var proyecto in proyectos)
            {
                listaProyectos.Add(proyecto.ProyectoID, proyecto.Nombre);
            }
            return listaProyectos;
        }

        /// <summary>
        /// Obtiene si el usuario participa en el proyecto con alguna de sus identidades
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>TRUE si el usuario participa con alguna de sus identidadez</returns>
        public bool ParticipaUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            return mEntityContext.Identidad.JoinPerfil().JoinPersona().Any(objeto => objeto.Perfil.PersonaID.HasValue && objeto.Identidad.ProyectoID.Equals(pProyectoID) && objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID));
        }

        /// <summary>
        /// Obtiene el la URL del API de Integracion Continua
        /// </summary>
        /// <param name="pNombre">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public string ObtenerURLApiIntegracionContinua()
        {
            return mEntityContext.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals("UrlServicioIntegracionEntornos")).Select(parametroApp => parametroApp.Valor).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el la URL del API de Integracion Continua
        /// </summary>
        /// <param name="pNombre">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerEntornoID()
        {
            object resultado = mEntityContext.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals("EntornoIntegracionContinua")).Select(parametroApp => parametroApp.Valor).FirstOrDefault();

            if (resultado != null)
            {
                return Guid.Parse(resultado.ToString());
            }
            else
            {
                return Guid.Empty;
            }

        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre CORTO pasado por par?metro
        /// </summary>
        /// <param name="pNombre">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerProyectoIDPorNombre(string pNombre)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.NombreCorto.Equals(pNombre)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el proyecto a trav?s de su nombre corto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto a obtener</param>
        /// <returns></returns>
        public Proyecto ObtenerProyectoPorNombreCorto(string pNombreCorto)
        {
            return mEntityContext.Proyecto.Where(item => item.NombreCorto.Equals(pNombreCorto)).FirstOrDefault();
        }

        /// <summary>
        /// Nos indica si existe alg?n proyecto con el nombre corto indicado
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto a comprobar</param>
        /// <returns></returns>
        public bool ExisteNombreCortoProyecto(string pNombreCorto)
        {
            return mEntityContext.Proyecto.Any(item => item.Nombre.Equals(pNombreCorto));
        }

        /// Obtiene el identificador de un proyecto y del proyecto del que hereda en caso de hacerlo
        /// </summary>
        /// <param name="pNombre">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public List<Guid> ObtenerProyectoYProyectoSuperiorIDs(string pNombreCorto)
        {
            List<Guid> listaIDs = new List<Guid>();
            Proyecto proyecto = mEntityContext.Proyecto.Where(item => item.NombreCorto.Equals(pNombreCorto)).FirstOrDefault();
            listaIDs.Add(proyecto.ProyectoID);
            if (proyecto.ProyectoSuperiorID.HasValue && !proyecto.ProyectoSuperiorID.Value.Equals(Guid.Empty))
            {
                listaIDs.Add(proyecto.ProyectoSuperiorID.Value);
            }
            return listaIDs;
        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre CORTO pasado por par?metro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public List<Guid> ObtenerProyectoIDOrganizacionIDPorNombreCorto(string pNombreCorto)
        {
            var resultadoConsulta = mEntityContext.Proyecto.Where(proyecto => proyecto.NombreCorto.Equals(pNombreCorto)).Select(proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }).FirstOrDefault();

            List<Guid> resultado = null;

            if (resultadoConsulta != null)
            {
                resultado = new List<Guid>();
                resultado.Add(resultadoConsulta.OrganizacionID);
                resultado.Add(resultadoConsulta.ProyectoID);
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre pasado por par?metro
        /// </summary>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerProyectoIDPorNombreLargo(string pNombre)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.Nombre.Equals(pNombre)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pTipoIdentidad">Tipo de identidad del perfil</param>
        /// <param name="pExcluirMyGNOSS">TRUE si se quiere excluir la metacomunidad, FALSE en caso contrario</param>
        /// <returns>Dataset de proyecto con los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilUsuario(Guid pPerfilID, short pTipoIdentidad, bool pExcluirMyGNOSS, Guid pUsuarioID)
        {
            var listaProyectosIDParte1Var = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(proyIden => proyIden.Identidad.PerfilID.Equals(pPerfilID) && proyIden.Proyecto.Estado != (short)EstadoProyecto.Cerrado && proyIden.Proyecto.Estado != (short)EstadoProyecto.CerradoTemporalmente && proyIden.Proyecto.Estado != (short)EstadoProyecto.Definicion && !proyIden.Identidad.FechaBaja.HasValue).ToList();

            var listaProyectosIDParte2Var = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.AdministradorProyecto, proyIden => new { proyIden.Proyecto.OrganizacionID, proyIden.Proyecto.ProyectoID }, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID }, (proyIden, adminProy) => new
            {
                Proyecto = proyIden.Proyecto,
                Identidad = proyIden.Identidad,
                AdminProyecto = adminProy
            }).Where(proyIdentAdmin => proyIdentAdmin.Identidad.PerfilID.Equals(pPerfilID) && !proyIdentAdmin.Identidad.FechaBaja.HasValue && !proyIdentAdmin.Proyecto.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && proyIdentAdmin.AdminProyecto.UsuarioID.Equals(pUsuarioID) && proyIdentAdmin.AdminProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador)).ToList();

            List<Guid> listaProyectosIDParte2 = listaProyectosIDParte2Var.Select(objeto => objeto.Proyecto.ProyectoID).ToList();
            List<Guid> listaProyectosIDParte1 = listaProyectosIDParte1Var.Select(objeto => objeto.Proyecto.ProyectoID).ToList();

            if (pTipoIdentidad >= 0)
            {
                listaProyectosIDParte1 = listaProyectosIDParte1Var.Where(objeto => objeto.Identidad.Tipo.Equals(pTipoIdentidad)).Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                listaProyectosIDParte2 = listaProyectosIDParte2Var.Where(objeto => objeto.Identidad.Tipo.Equals(pTipoIdentidad)).Select(objeto => objeto.Proyecto.ProyectoID).ToList();
            }
            List<Guid> listaProyectosID = listaProyectosIDParte1.Union(listaProyectosIDParte2).ToList();

            List<Proyecto> listaProyectosParticipaPerfilUsuario = mEntityContext.Proyecto.Where(proy => listaProyectosID.Contains(proy.ProyectoID)).OrderBy(proy => proy.Nombre).ToList();
            if (pExcluirMyGNOSS)
            {
                var listaProyectosIDSinMyGNOSSParte1Var = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
                {
                    Proyecto = proyecto,
                    Identidad = identidad
                }).Where(proyIden => proyIden.Identidad.PerfilID.Equals(pPerfilID) && proyIden.Proyecto.Estado != (short)EstadoProyecto.Cerrado && proyIden.Proyecto.Estado != (short)EstadoProyecto.CerradoTemporalmente && proyIden.Proyecto.Estado != (short)EstadoProyecto.Definicion && !proyIden.Identidad.FechaBaja.HasValue).ToList();

                var listaProyectosIDSinMyGNOSSParte2Var = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
                {
                    Proyecto = proyecto,
                    Identidad = identidad
                }).Join(mEntityContext.AdministradorProyecto, proyIden => new { proyIden.Proyecto.OrganizacionID, proyIden.Proyecto.ProyectoID }, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID }, (proyIden, adminProy) => new
                {
                    Proyecto = proyIden.Proyecto,
                    Identidad = proyIden.Identidad,
                    AdminProyecto = adminProy
                }).Where(proyIdentAdmin => proyIdentAdmin.Identidad.PerfilID.Equals(pPerfilID) && !proyIdentAdmin.Identidad.FechaBaja.HasValue && proyIdentAdmin.Proyecto.TipoProyecto != (short)TipoProyecto.MetaComunidad && proyIdentAdmin.AdminProyecto.UsuarioID.Equals(pUsuarioID) && proyIdentAdmin.AdminProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador)).ToList();
                List<Guid> listaProyectosIDSinMyGNOSSParte2 = listaProyectosIDSinMyGNOSSParte2Var.Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                List<Guid> listaProyectosIDSinMyGNOSSParte1 = listaProyectosIDSinMyGNOSSParte1Var.Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                if (pTipoIdentidad >= 0)
                {
                    listaProyectosIDSinMyGNOSSParte1 = listaProyectosIDSinMyGNOSSParte1Var.Where(objeto => objeto.Identidad.Tipo.Equals(pTipoIdentidad)).Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                    listaProyectosIDSinMyGNOSSParte2 = listaProyectosIDSinMyGNOSSParte2Var.Where(objeto => objeto.Identidad.Tipo.Equals(pTipoIdentidad)).Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                }
                List<Guid> listaProyectosSinMyGNOSSID = listaProyectosIDSinMyGNOSSParte1.Union(listaProyectosIDSinMyGNOSSParte2).ToList();

                listaProyectosParticipaPerfilUsuario = mEntityContext.Proyecto.Where(proy => listaProyectosSinMyGNOSSID.Contains(proy.ProyectoID)).OrderBy(proy => proy.Nombre).ToList();
            }

            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyecto = listaProyectosParticipaPerfilUsuario;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil pasandole una de las identidades del perfil(NO incluye myGnoss)
        /// </summary>
        /// /// <param name="listaProyectos">Lista con los proyectos obtenidos de Virtuoso</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilIdentidad(string listaProyectos, Guid pIdentidadID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            List<int> tiposDeAcceso = new List<int>();
            tiposDeAcceso.Add(0); tiposDeAcceso.Add(1); tiposDeAcceso.Add(2);
            List<Guid> listaPerfilID = mEntityContext.Identidad.Where(identidad => identidad.IdentidadID.Equals(pIdentidadID)).Select(identidad => identidad.PerfilID).ToList();
            List<Guid> listaProyectoID = mEntityContext.Identidad.Where(identidad => listaPerfilID.Contains(identidad.PerfilID)).Select(identidad => identidad.ProyectoID).ToList();
            List<Proyecto> proyectos = mEntityContext.Proyecto.Where(proyecto => listaProyectos.Contains(proyecto.ProyectoID.ToString()) && (tiposDeAcceso.Contains(proyecto.TipoAcceso) || listaProyectoID.Contains(proyecto.ProyectoID)) && proyecto.ProyectoID != ProyectoAD.MetaProyecto && (proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose))).ToList();

            dataWrapperProyecto.ListaProyecto = proyectos;
            return dataWrapperProyecto;
        }


        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil(NO incluye myGnoss)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfil(Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProyecto = adminProy,
                Usuario = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.Usuario.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProyecto = objeto.AdminProyecto,
                Persona = persona
            }).Join(mEntityContext.Perfil, persona => persona.Persona.PersonaID, perfil => perfil.PersonaID.Value, (persona, perfil) => new
            {
                AdminProyecto = persona.AdminProyecto,
                Perfil = perfil
            }).Where(perfil => perfil.Perfil.PerfilID == pPerfilID).Select(objeto => objeto.AdminProyecto.ProyectoID).ToList();

            var listaProyectosVar = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && objeto.Proyecto.ProyectoID != MetaProyecto && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID)))).Select(objeto => new ProyectoNumConexiones
            {
                ProyectoID = objeto.Proyecto.ProyectoID,
                NombreCorto = objeto.Proyecto.NombreCorto,
                Nombre = objeto.Proyecto.Nombre,
                TipoAcceso = objeto.Proyecto.TipoAcceso,
                NumConexiones = objeto.Identidad.NumConnexiones,
                TipoProyecto = objeto.Proyecto.TipoProyecto
            }).ToList();

            dataWrapperProyecto.ListaProyectoNumConexiones = listaProyectosVar;
            return dataWrapperProyecto;
        }


        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil(NO incluye myGnoss)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilLimite10(Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProyecto = adminProy,
                Usuario = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.Usuario.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProyecto = objeto.AdminProyecto,
                Persona = persona
            }).Join(mEntityContext.Perfil, persona => persona.Persona.PersonaID, perfil => perfil.PersonaID.Value, (persona, perfil) => new
            {
                AdminProyecto = persona.AdminProyecto,
                Perfil = perfil
            }).Where(perfil => perfil.Perfil.PerfilID == pPerfilID).Select(objeto => objeto.AdminProyecto.ProyectoID).ToList();

            var listaProyectosVar = mEntityContext.Proyecto.
                Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
                {
                    Proyecto = proyecto,
                    Identidad = identidad
                }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && objeto.Proyecto.ProyectoID != MetaProyecto && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID)))).OrderByDescending(identidad => identidad.Identidad.NumConnexiones).Take(10).Select(objeto => new ProyectoNumConexiones
                {
                    ProyectoID = objeto.Proyecto.ProyectoID,
                    NombreCorto = objeto.Proyecto.NombreCorto,
                    Nombre = objeto.Proyecto.Nombre,
                    TipoAcceso = objeto.Proyecto.TipoAcceso,
                    NumConexiones = objeto.Identidad.NumConnexiones,
                    TipoProyecto = objeto.Proyecto.TipoProyecto
                }).ToList();

            dataWrapperProyecto.ListaProyectoNumConexiones = listaProyectosVar;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene una lista con los proyectos que no son de registro obligatorio.
        /// </summary>
        /// <returns>Lista de los proyectos que no son de registro obligatorio</returns>
        public List<Guid> ObtenerListaIDsProyectosSinRegistroObligatorio()
        {
            return mEntityContext.ProyectoSinRegistroObligatorio.Select(proyecto => proyecto.ProyectoID).Distinct().ToList();
        }



        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil(NO incluye myGnoss)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public Dictionary<string, KeyValuePair<string, short>> ObtenerListaProyectosParticipaPerfilUsuario(Guid pPerfilID)
        {
            Dictionary<string, KeyValuePair<string, short>> listaProyectos = new Dictionary<string, KeyValuePair<string, short>>();

            List<Guid> listaGuid = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID, (objeto, perfil) => new
            {
                AdminProy = objeto.AdminProy,
                PerfilID = perfil.PerfilID
            }).Where(objeto => objeto.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.AdminProy.ProyectoID).ToList();

            var consulta = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaGuid.Contains(objeto.Proyecto.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => new { objeto.Proyecto.NombreCorto, objeto.Proyecto.Nombre, objeto.Proyecto.TipoAcceso, objeto.Identidad.NumConnexiones });

            foreach (var objeto in consulta.ToList())
            {
                if (!listaProyectos.ContainsKey(objeto.NombreCorto))
                {
                    listaProyectos.Add(objeto.NombreCorto, new KeyValuePair<string, short>(objeto.Nombre, objeto.TipoAcceso));
                }
            }
            return listaProyectos;
        }
        /// <summary>
        /// Obtiene la lista de proyectos en las que participa una persona pasada por par?metro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>lista de guids con to
        public List<Guid> ObtenerIdProyectosParticipaPersona(Guid pPersonaID)
        {
            return mEntityContext.Identidad.JoinPerfil().Where(item => !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Perfil.Eliminado && item.Perfil.PersonaID.Equals(pPersonaID)).Select(item => item.Identidad.ProyectoID).ToList();
        }

        /// <summary>
        /// Obtiene una lista con los identificadores de los proyectos en los que participa el usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del Usuario</param>
        /// <returns>Lista con los identificadores de los proyectos en los que participa el Usuario</returns>
        public List<Guid> ObtenerListaIDsProyectosParticipaUsuario(Guid pUsuarioID)
        {
            List<Guid> listaProyecto;

            List<Guid> listaPerfilID = mEntityContext.Perfil.Join(mEntityContext.Persona, perfil => perfil.PersonaID.Value, persona => persona.PersonaID, (perfil, persona) => new
            {
                Perfil = perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.Perfil.PerfilID).ToList();

            List<Guid?> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID.Value, (objeto, perfil) => new
            {
                UsurioID = objeto.Persona.UsuarioID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Where(objeto => objeto.UsurioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.UsurioID).ToList();

            listaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => listaPerfilID.Contains(objeto.Identidad.PerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion)) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID))).OrderBy(objeto => objeto.Identidad.NumConnexiones).Select(objeto => objeto.Proyecto.ProyectoID).Distinct().ToList();

            return listaProyecto;
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa el usuarioID
        /// </summary>
        /// <param name="pUsuarioID">Identificado del Usuario</param>
        /// <returns>Lista de los proyectos en los que participa el Usuario</returns>
        public Dictionary<string, KeyValuePair<string, short>> ObtenerListaProyectosParticipaUsuario(Guid pUsuarioID)
        {
            Dictionary<string, KeyValuePair<string, short>> listaProyectos = new Dictionary<string, KeyValuePair<string, short>>();

            List<Guid> listaPerfilID = mEntityContext.Perfil.Join(mEntityContext.Persona, perfil => perfil.PersonaID.Value, persona => persona.PersonaID, (perfil, persona) => new
            {
                Perfil = perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.Perfil.PerfilID).ToList();

            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID.Value, (objeto, perfil) => new
            {
                UsurioID = objeto.Persona.UsuarioID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Where(objeto => objeto.UsurioID.HasValue && objeto.UsurioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.UsurioID.Value).ToList();

            var listaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => listaPerfilID.Contains(objeto.Identidad.PerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => new
            {
                objeto.Proyecto.NombreCorto,
                objeto.Proyecto.Nombre,
                objeto.Proyecto.TipoAcceso,
                objeto.Identidad.NumConnexiones
            }).ToList();


            foreach (var proyecto in listaProyecto)
            {
                if (!listaProyectos.ContainsKey(proyecto.NombreCorto))
                {
                    listaProyectos.Add(proyecto.NombreCorto, new KeyValuePair<string, short>(proyecto.Nombre, proyecto.TipoAcceso));
                }
            }

            return listaProyectos;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa el usuario
        /// </summary>
        /// <param name="pUsuarioID">Id del usuario</param>
        /// <returns>Devuelve lista con los Id de los proyectos que participa el usuario</returns>
        public List<Guid> ObtenerProyectoIdParticipaUsuario(Guid pUsuarioID)
        {
            List<Guid> proyectosUsuario = new List<Guid>();

            List<Guid> listaPerfilID = mEntityContext.Perfil.Join(mEntityContext.Persona, perfil => perfil.PersonaID.Value, persona => persona.PersonaID, (perfil, persona) => new
            {
                Perfil = perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.Perfil.PerfilID).ToList();

            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID.Value, (objeto, perfil) => new
            {
                UsurioID = objeto.Persona.UsuarioID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Where(objeto => objeto.UsurioID.HasValue && objeto.UsurioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.UsurioID.Value).ToList();

            var listaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => listaPerfilID.Contains(objeto.Identidad.PerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => new
            {
                objeto.Proyecto.ProyectoID
            }).ToList();

            foreach (var proyectoID in listaProyecto.Select(item => item.ProyectoID))
            {
                if (!proyectosUsuario.Contains(proyectoID))
                {
                    proyectosUsuario.Add(proyectoID);
                }
            }
            return proyectosUsuario;
        }

        /// <summary>
        /// Obtiene un n?mero espec?fico de proyectos en los que participa el usuario
        /// </summary>
        /// <param name="pUsuarioID">Id del usuario</param>
        /// <param name="numeroResultados">Numero de proyectos que se van a devolver</param>
        /// <returns>Devuelve lista con los Id de los proyectos que participa el usuario</returns>
        public List<Guid> ObtenerProyectosIDParticipaUsuario(Guid pUsuarioID, int pNumeroResultados)
        {
            List<Guid> proyectosUsuario = new List<Guid>();

            List<Guid> listaPerfilID = mEntityContext.Perfil.Join(mEntityContext.Persona, perfil => perfil.PersonaID.Value, persona => persona.PersonaID, (perfil, persona) => new
            {
                Perfil = perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.Perfil.PerfilID).ToList();

            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID.Value, (objeto, perfil) => new
            {
                UsurioID = objeto.Persona.UsuarioID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Where(objeto => objeto.UsurioID.HasValue && objeto.UsurioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.UsurioID.Value).ToList();

            var listaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => listaPerfilID.Contains(objeto.Identidad.PerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => new
            {
                objeto.Proyecto.ProyectoID
            }).ToList();

            foreach (var proyectoID in listaProyecto.Select(item => item.ProyectoID))
            {
                if (!proyectosUsuario.Contains(proyectoID))
                {
                    proyectosUsuario.Add(proyectoID);
                }
                if (pNumeroResultados.Equals(proyectosUsuario.Count))
                {
                    break;
                }
            }

            return proyectosUsuario;
        }

        /// <summary>
        /// Devuvle los usuarios que no pertenecen al proyecto
        /// </summary>
        /// <param name="listaUsuarios">Lista de los usuarios de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista de los usuarios que no pertenecen a la organizacion</returns>
        public List<Guid> ObtenerUsuariosNoParticipanEnComunidad(List<Guid> listaUsuarios, Guid pProyectoID)
        {
            List<Guid> usuariosNoPertenecen = new List<Guid>();
            foreach (Guid usuario in listaUsuarios)
            {
                if (!mEntityContext.ProyectoRolUsuario.Any(item => item.UsuarioID.Equals(usuario) && item.ProyectoID.Equals(pProyectoID)))
                {
                    usuariosNoPertenecen.Add(usuario);
                }
            }
            return usuariosNoPertenecen;
        }

        /// <summary>
        /// Obtiene las urls de la busqueda
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista con las Urls de la busqueda</returns>
        public List<string> ObtenerUrlsComunidadCajaBusqueda(Guid pProyectoID)
        {
            Guid componenteId = mEntityContext.CMSComponente.Where(tipo => tipo.TipoComponente.Equals((short)TipoComponenteCMS.CajaBuscador) && tipo.ProyectoID.Equals(pProyectoID)).Select(tipo => tipo.ComponenteID).FirstOrDefault();
            List<string> listaUrl = mEntityContext.CMSPropiedadComponente.Where(tipo => tipo.TipoPropiedadComponente.Equals((short)TipoPropiedadCMS.URLBusqueda) && tipo.ComponenteID.Equals(componenteId)).Select(tipo => tipo.ValorPropiedad).ToList();
            return listaUrl;
        }
        /// <summary>
        /// Elimina la comunidad de la url de b?squeda
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="url">Url a la que se le quiere quitar la comunidad</param>
        public void QuitarUrlComunidadCajaBusqueda(Guid pProyectoID, string url)
        {
            string urlNueva = string.Empty;
            string[] trozos = url.Split('/');
            if (trozos[3].Length == 2)
            {
                urlNueva = $"{trozos[0]}//{trozos[2]}/{trozos[3]}";
                for (int i = 6; i < trozos.Length; i++)
                {
                    urlNueva = $"{urlNueva}/{trozos[i]}";
                }
            }
            else
            {
                urlNueva = $"{trozos[0]}//{trozos[2]}";
                for (int i = 5; i < trozos.Length; i++)
                {
                    urlNueva = $"{urlNueva}/{trozos[i]}";
                }
            }
            Guid 
                componenteId = mEntityContext.CMSComponente.Where(tipo => tipo.TipoComponente.Equals((short)TipoComponenteCMS.CajaBuscador) && tipo.ProyectoID.Equals(pProyectoID)).Select(tipo => tipo.ComponenteID).FirstOrDefault();
            mEntityContext.CMSPropiedadComponente.Where(tipo => tipo.TipoPropiedadComponente.Equals((short)TipoPropiedadCMS.URLBusqueda) && tipo.ComponenteID.Equals(componenteId) && tipo.ValorPropiedad.Equals(url)).FirstOrDefault().ValorPropiedad = urlNueva;
            mEntityContext.SaveChanges();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public Guid ObtenerProyectoIDMasActivoPerfil(Guid pPerfilID)
        {
            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                Usuario = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.Usuario.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new
            {
                PersonaID = persona.PersonaID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Join(mEntityContext.Perfil, objeto => objeto.PersonaID, perfil => perfil.PersonaID, (objeto, perfil) => new
            {
                ProyectoID = objeto.ProyectoID,
                PerfilID = perfil.PerfilID
            }).Where(objeto => objeto.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.ProyectoID).ToList();

            Guid proyectoMasActivo = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyect = proyecto,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyect.ProyectoID.Equals(MetaProyecto) && (objeto.Proyect.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyect.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyect.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyect.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => objeto.Proyect.ProyectoID).FirstOrDefault();

            return proyectoMasActivo;
        }

        /// <summary>
        /// Obtiene una lista con los proyectos comunes en los que participan dos perfiles
        /// </summary>
        /// <param name="pPerfilID1">Identificador del perfil 1</param>
        /// <param name="pTipoIdentidad1">Tipo de identidad del perfil 1</param>
        /// <param name="pPerfilID2">Identificador del perfil 2</param>
        /// <param name="pTipoIdentidad2">Tipo de identidad del perfil 2</param>
        /// <param name="pIncluirMyGNOSS">Indica si se debe de buscar my gnoss</param>
        /// <returns>DataSet de Proyectos</returns>
        public DataWrapperProyecto ObtenerListaProyectosComunesParticipanPerfilesUsuarios(Guid pPerfilID1, TiposIdentidad pTipoIdentidad1, Guid pPerfilID2, TiposIdentidad pTipoIdentidad2, bool pIncluirMyGNOSS)
        {
            List<Guid> listaProyectos = new List<Guid>();

            var resultadoSQL1 = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proy => proy.ProyectoID, identidad => identidad.ProyectoID, (proy, identidad) => new
            {
                ProyectoID = proy.ProyectoID,
                Nombre = proy.Nombre,
                PerfilID = identidad.PerfilID,
                FechaBaja = identidad.FechaBaja,
                FechaExpulsion = identidad.FechaExpulsion,
                Tipo = identidad.Tipo
            }).Where(res => res.PerfilID.Equals(pPerfilID1) && !res.ProyectoID.Equals(ProyectoAD.MyGnoss) && !res.FechaBaja.HasValue && !res.FechaExpulsion.HasValue && res.Tipo.Equals((short)pTipoIdentidad1)).Select(res => new { res.ProyectoID, res.Nombre });

            var resultadoSQL2 = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proy => proy.ProyectoID, identidad => identidad.ProyectoID, (proy, identidad) => new
            {
                ProyectoID = proy.ProyectoID,
                Nombre = proy.Nombre,
                PerfilID = identidad.PerfilID,
                FechaBaja = identidad.FechaBaja,
                FechaExpulsion = identidad.FechaExpulsion,
                Tipo = identidad.Tipo
            }).Where(res => res.PerfilID.Equals(pPerfilID2) && !res.ProyectoID.Equals(ProyectoAD.MyGnoss) && !res.FechaBaja.HasValue && !res.FechaExpulsion.HasValue && res.Tipo.Equals((short)pTipoIdentidad2)).Select(res => new { res.ProyectoID, res.Nombre });

            if (pIncluirMyGNOSS)
            {
                resultadoSQL1 = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proy => proy.ProyectoID, identidad => identidad.ProyectoID, (proy, identidad) => new
                {
                    ProyectoID = proy.ProyectoID,
                    Nombre = proy.Nombre,
                    PerfilID = identidad.PerfilID,
                    FechaBaja = identidad.FechaBaja,
                    FechaExpulsion = identidad.FechaExpulsion,
                    Tipo = identidad.Tipo
                }).Where(res => res.PerfilID.Equals(pPerfilID1) && !res.FechaBaja.HasValue && !res.FechaExpulsion.HasValue && res.Tipo.Equals((short)pTipoIdentidad1)).Select(res => new { res.ProyectoID, res.Nombre });

                resultadoSQL2 = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proy => proy.ProyectoID, identidad => identidad.ProyectoID, (proy, identidad) => new
                {
                    ProyectoID = proy.ProyectoID,
                    Nombre = proy.Nombre,
                    PerfilID = identidad.PerfilID,
                    FechaBaja = identidad.FechaBaja,
                    FechaExpulsion = identidad.FechaExpulsion,
                    Tipo = identidad.Tipo
                }).Where(res => res.PerfilID.Equals(pPerfilID2) && !res.FechaBaja.HasValue && !res.FechaExpulsion.HasValue && res.Tipo.Equals((short)pTipoIdentidad2)).Select(res => new { res.ProyectoID, res.Nombre });
            }

            var resultadoFinal = resultadoSQL1.Intersect(resultadoSQL2).OrderBy(res => res.Nombre);

            foreach (var proyecto in resultadoFinal.ToList())
            {
                listaProyectos.Add(proyecto.ProyectoID);
            }

            return ObtenerProyectosPorIDsCargaLigera(listaProyectos);
        }

        /// <summary>
        /// Obtiene una DataSet sin tipar con una tabla "Emails" que contiene los campos (IdentidadID,PersonaID,Nombre,Email) de cada uno de los miembros que participan en un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyectos</param>
        /// <returns>DataSet</returns>
        public List<EmailsMiembrosDeProyecto> ObtenerEmailsMiembrosDeProyecto(Guid pProyectoID)
        {
            List<EmailsMiembrosDeProyecto> lista = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID, persona => persona.PersonaID, (objeto, persona) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && objeto.Identidad.FechaBaja == null && !objeto.Perfil.Eliminado && !objeto.Perfil.OrganizacionID.HasValue && objeto.Persona.Email != null).Select(objeto => new EmailsMiembrosDeProyecto { IdentidadID = objeto.ProyectoUsuarioIdentidad.IdentidadID, PersonaID = objeto.Perfil.PersonaID, Nombre = objeto.Persona.Nombre, Email = objeto.Persona.Email })
            .Union(mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID, persona => persona.PersonaID, (objeto, persona) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Join(mEntityContext.PersonaVinculoOrganizacion, objeto => new { objeto.Persona.PersonaID, OrganizacionID = objeto.Perfil.OrganizacionID.Value }, personaVinculoOrganizacion => new { personaVinculoOrganizacion.PersonaID, personaVinculoOrganizacion.OrganizacionID }, (objeto, personaVinculoOrganizacion) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                PersonaVinculoOrganizacion = personaVinculoOrganizacion
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Perfil.Eliminado && objeto.Perfil.PersonaID.HasValue && objeto.Perfil.OrganizacionID.HasValue && objeto.PersonaVinculoOrganizacion.EmailTrabajo != null).Select(objeto => new EmailsMiembrosDeProyecto { IdentidadID = objeto.ProyectoUsuarioIdentidad.IdentidadID, PersonaID = objeto.Perfil.PersonaID, Nombre = objeto.Persona.Nombre, Email = objeto.PersonaVinculoOrganizacion.EmailTrabajo })).ToList();

            return lista;
        }

        /// <summary>
        /// Obtiene una lista con los servicios externos si es MetaProyecto
        /// </summary>
        /// <returns>Lista de los servicios externos</returns>
        public List<EcosistemaServicioExterno> ObtenerEcosistemaServicioExterno()
        {
            return mEntityContext.EcosistemaServicioExterno.ToList();
        }

        /// <summary>
        /// Obtiene una lista con los servicios externos si no es MetaProyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista de los servicios externos</returns>
        public List<ProyectoServicioExterno> ObtenerProyectoServicioExterno(Guid pProyectoID)
        {
            return mEntityContext.ProyectoServicioExterno.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Obtiene el valor de un parametro de aplicacion 
        /// </summary>
        /// <param name="parametro">Parametro de aplicacion</param>
        /// <returns>Valor correspondiente al parametro pasado</returns>
        public string ObtenerParametroAplicacion(string parametro)
        {
            return mEntityContext.ParametroAplicacion.Where(param => param.Parametro.Equals(parametro)).Select(param => param.Valor).FirstOrDefault();
        }

        /// <summary>
        /// Actualiza la tabla ParametroAplicacion con el parametro y valor dado. Si el paramerto no existe en la base de datos lo a?ade.
        /// </summary>
        /// <param name="pParametro">Nombre del parametro a guardar. Clave</param>
        /// <param name="pValor">Valor del paramentro</param>       
        public void ActualizarParametroAplicacion(string pParametro, string pValor)
        {
            AD.EntityModel.ParametroAplicacion parametroAplicacion = mEntityContext.ParametroAplicacion.Where(item => item.Parametro.Equals(pParametro)).FirstOrDefault();

            if (parametroAplicacion != null)
            {
                parametroAplicacion.Valor = pValor;
            }
            else
            {
                parametroAplicacion = new AD.EntityModel.ParametroAplicacion() { Parametro = pParametro, Valor = pValor };
                mEntityContext.ParametroAplicacion.Add(parametroAplicacion);
            }

            mEntityContext.SaveChanges();
        }
        /// <summary>
        /// Guarda en la tabla EcosistemaServicioExterno una nueva fila
        /// </summary>
        /// <param name="eco">Objeto de tipo EcosistemaServicioExterno</param> 
        public void AgregarServicioExternoEcosistema(EcosistemaServicioExterno eco)
        {
            mEntityContext.EcosistemaServicioExterno.Add(eco);
        }

        /// <summary>
        ///  Guarda en la tabla ProyectoServicioExterno una nueva fila
        /// </summary>
        /// <param name="proy">Objeto de tipo ProyectoServicioExterno</param> 
        public void AgregarProyectoServicioExterno(ProyectoServicioExterno proy)
        {
            mEntityContext.ProyectoServicioExterno.Add(proy);
        }

        /// <summary>
        ///  Elimina de la tabla EcosistemaServicioExterno una fila
        /// </summary>
        /// <param name="eco">Objeto de tipo EcosistemaServicioExterno</param> 
        public void EliminarEcosistemaServicioExterno(EcosistemaServicioExterno eco)
        {
            mEntityContext.Entry(eco).State = EntityState.Deleted;
        }

        /// <summary>
        ///  Elimina de la tabla ProyectoServicioExterno una fila
        /// </summary>
        /// <param name="proy">Objeto de tipo ProyectoServicioExterno</param>
        public void EliminarProyectoServicioExterno(ProyectoServicioExterno proy)
        {
            mEntityContext.Entry(proy).State = EntityState.Deleted;
        }

        /// <summary>
        ///  Obtiene la OrganizacionID a la que corresponde un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>ID de la organizacion</returns>
        public Guid ObtenerOrganizacionIDAPartirDeProyectoID(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.OrganizacionID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene una DataSet sin tipar con una tabla "Emails" que contiene los campos (IdentidadID,PersonaID,Nombre,Email) de cada uno de los miembros que participan en un determinado evento
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet</returns>
        public DataSet ObtenerEmailsMiembrosDeEventoDeProyecto(Guid pEventoID)
        {
            DataSet dataSet = new DataSet();

            DbCommand commandsqlSelectEmailsMiembrosDeEventoDeProyecto = ObtenerComando(sqlSelectEmailsMiembrosDeEventoDeProyecto);
            AgregarParametro(commandsqlSelectEmailsMiembrosDeEventoDeProyecto, IBD.ToParam("eventoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pEventoID));
            CargarDataSet(commandsqlSelectEmailsMiembrosDeEventoDeProyecto, dataSet, "Emails");

            return dataSet;
        }

        /// <summary>
        /// Devuelve una lista con los emails de los administradores del proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>DataSet</returns>
        public List<string> ObtenerEmailsAdministradoresDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID.Value, persona => persona.PersonaID, (objeto, persona) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.UsuarioID, objeto.ProyectoUsuarioIdentidad.ProyectoID }, adminProy => new { adminProy.UsuarioID, adminProy.ProyectoID }, (objeto, adminProy) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.Perfil.PersonaID.HasValue && objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Perfil.Eliminado && !objeto.Perfil.OrganizacionID.HasValue && objeto.Persona.Email != null && objeto.AdministradorProyecto.Tipo == 0).Select(item => item.Persona.Email).Distinct().Union(
                mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) => new
                {
                    ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                    Identidad = identidad,
                }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
                {
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = perfil
                }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID, persona => persona.PersonaID, (objeto, persona) => new
                {
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = objeto.Perfil,
                    Persona = persona
                }).Join(mEntityContext.PersonaVinculoOrganizacion, objeto => new { objeto.Persona.PersonaID, OrganizacionID = objeto.Perfil.OrganizacionID.Value }, personaVinculoOrganizacion => new { personaVinculoOrganizacion.PersonaID, personaVinculoOrganizacion.OrganizacionID }, (objeto, personaVinculoOrganizacion) => new
                {
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = objeto.Perfil,
                    Persona = objeto.Persona,
                    PersonaVinculoOrganizacion = personaVinculoOrganizacion
                }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.UsuarioID, objeto.ProyectoUsuarioIdentidad.ProyectoID }, adminProyect => new { adminProyect.UsuarioID, adminProyect.ProyectoID }, (objeto, adminProyect) => new
                {
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = objeto.Perfil,
                    Persona = objeto.Persona,
                    PersonaVinculoOrganizacion = objeto.PersonaVinculoOrganizacion,
                    AdministradorProyecto = adminProyect
                }).Where(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Perfil.Eliminado && objeto.Perfil.PersonaID.HasValue && objeto.Perfil.OrganizacionID.HasValue && objeto.PersonaVinculoOrganizacion.EmailTrabajo != null && objeto.AdministradorProyecto.Tipo == 0).Select(item => item.Persona.Email).Distinct()
                ).ToList();
        }

        /// <summary>
        /// Obtiene el n?mero de elementos de un perfil en unos proyectos.
        /// </summary>
        /// <param name="pProyectosID">IDs de los proyectos</param>
        /// <param name="pPerfilID">ID del perfil</param>
        /// <returns>DataSet con la tabla 'ProyectoPerfilNumElem' cargada para un perfil en uno proyectos</returns>
        public DataWrapperProyecto ObtenerNumeroElementosPerfilEnProyectos(List<Guid> pProyectosID, Guid pPerfilID)
        {
            DataWrapperProyecto proyectoDataWrapper = new DataWrapperProyecto();
            if (pProyectosID.Count > 0)
            {
                List<ProyectoPerfilNumElem> listaProyectoPerfilNumElem = mEntityContext.ProyectoPerfilNumElem.Where(proyectoPerfil => proyectoPerfil.PerfilID.Equals(pPerfilID) && pProyectosID.Contains(proyectoPerfil.ProyectoID)).ToList();
                proyectoDataWrapper.ListaProyectoPerfilNumElem = listaProyectoPerfilNumElem;
            }

            return proyectoDataWrapper;
        }

        /// <summary>
        /// Actualiza los contadores del proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto a actualizar</param>
        /// <param name="pNumOrg">N?mero de miembros de organizaci?n</param>
        /// <param name="pNumIden">N?mero de miembros normales</param>
        /// <param name="pNumRec">N?mero de recursos</param>
        /// <param name="pNumDafos">N?mero de dafos</param>
        /// <param name="pNumDebates">N?mero Debates</param>
        /// <param name="pNumPreg">N?mero Preguntas</param>
        public void ActualizarContadoresProyecto(Guid pProyectoID, int pNumOrg, int pNumIden, int pNumRec, int pNumDafos, int pNumDebates, int pNumPreg)
        {
            Proyecto proyecto = mEntityContext.Proyecto.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).FirstOrDefault();

            if (pNumOrg != -1)
            {
                proyecto.NumeroOrgRegistradas = pNumOrg;
            }
            if (pNumIden != -1)
            {
                proyecto.NumeroMiembros = pNumIden;
            }
            if (pNumRec != -1)
            {
                proyecto.NumeroRecursos = pNumRec;
            }
            if (pNumDafos != -1)
            {
                proyecto.NumeroDafos = pNumDafos;
            }
            if (pNumDebates != -1)
            {
                proyecto.NumeroDebates = pNumDebates;
            }
            if (pNumPreg != -1)
            {
                proyecto.NumeroPreguntas = pNumPreg;
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Obtiene el n?mero de recursos del proyectoID publicados en los ?ltimos 30 d?as
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se quiere obtener el n?mero de recursos</param>
        /// <param name="pNumDias">D?as hasta hoy</param>
        /// <returns>N?mero de recursos</returns>
        public int ObtenerNumRecursosProyecto30Dias(Guid pProyectoID, int pNumDias)
        {
            var query = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !item.Documento.Borrador && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.PrivadoEditores && !item.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && !item.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria));

            if (pNumDias != -1)
            {
                query = query.Where(item => item.Documento.FechaCreacion > DateTime.Now.AddDays(pNumDias));
            }

            return query.Count();
        }

        public Proyecto ObtenerProyectoDeBaseRecursos(Guid pBaseRecursosID)
        {
            Guid proyectoID = mEntityContext.BaseRecursosProyecto.Where(x => x.BaseRecursosID.Equals(pBaseRecursosID)).Select(x => x.ProyectoID).FirstOrDefault();
            return mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(proyectoID)).FirstOrDefault();
		}

        /// <summary>
        /// Obtiene las secciones de la home de un proyecto tipo cat?logo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperProyecto ObtenerSeccionesHomeCatalogoDeProyecto(Guid pProyectoID)
        {
            List<SeccionProyCatalogo> listaSeccionProyCatalogo = mEntityContext.SeccionProyCatalogo.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();

            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaSeccionProyCatalogo = listaSeccionProyCatalogo;

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Aumenta en 1 el "NumeroMiembros" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void AumentarNumeroMiembrosDelProyecto(Guid pProyectoID)
        {
            DbCommand commandsqlUpdateAumentarNumeroMiembrosDelProyecto = ObtenerComando(sqlUpdateAumentarNumeroMiembrosDelProyecto);
            AgregarParametro(commandsqlUpdateAumentarNumeroMiembrosDelProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            ActualizarBaseDeDatos(commandsqlUpdateAumentarNumeroMiembrosDelProyecto);
        }

        /// <summary>
        /// Disminuye en 1 el "NumeroMiembros" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void DisminuirNumeroMiembrosDelProyecto(Guid pProyectoID)
        {
            Proyecto proyecto = mEntityContext.Proyecto.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (proyecto != null)
            {
                proyecto.NumeroMiembros = proyecto.NumeroMiembros - 1;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza el "NumeroMiembros" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void ActualizarNumeroMiembrosDelProyecto(Guid pProyectoID)
        {
            DbCommand commandsqlUpdateActualizar = ObtenerComando("UPDATE Proyecto SET NumeroMiembros = (SELECT COUNT(*) FROM Usuario inner join Persona ON (Usuario.UsuarioID=Persona.UsuarioID) inner join Perfil ON (Persona.PersonaID=Perfil.PersonaID) inner join Identidad on (Perfil.PerfilID = Identidad.PerfilID) where proyectoid=" + IBD.ToParam("proyectoID") + " AND Fechabaja IS NULL and Fechaexpulsion IS NULL and Perfil.Eliminado = 0 and Persona.Eliminado = 0 and Tipo != " + (short)TiposIdentidad.ProfesionalCorporativo + ")");
            AgregarParametro(commandsqlUpdateActualizar, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            ActualizarBaseDeDatos(commandsqlUpdateActualizar);
        }

        /// <summary>
        ///  Disminuye en 1 el "NumeroOrgRegistradas" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void DisminuirNumeroOrParticipanEnProyecto(Guid pProyectoID)
        {
            Proyecto proyecto = mEntityContext.Proyecto.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (proyecto != null)
            {
                proyecto.NumeroOrgRegistradas = proyecto.NumeroOrgRegistradas - 1;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Aumenta en 1 el "NumeroOrgRegistradas" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void AumentarNumeroOrgParticipanEnProyecto(Guid pProyectoID)
        {
            Proyecto proyecto = mEntityContext.Proyecto.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (proyecto != null)
            {
                proyecto.NumeroOrgRegistradas = proyecto.NumeroOrgRegistradas + 1;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene un lista con los nombres de determinados proyectos
        /// </summary>
        /// <param name="pListaProyectos">Lista con los identificadores de los proyectos a nombrar</param>
        /// <returns>Lista (identificador, nombre) de proyectos</returns>
        public Dictionary<Guid, Proyecto> ObtenerNombreProyectos(List<Guid> pListaProyectos)
        {
            Dictionary<Guid, Proyecto> listaNombres = new Dictionary<Guid, Proyecto>();

            if (pListaProyectos.Count > 0)
            {
                List<Proyecto> listaProyectos = mEntityContext.Proyecto.Where(proyecto => pListaProyectos.Contains(proyecto.ProyectoID)).ToList();

                foreach (Proyecto filaProy in listaProyectos)
                {
                    listaNombres.Add(filaProy.ProyectoID, filaProy);
                }
            }
            return listaNombres;
        }

        /// <summary>
        /// Comprueba si en el proyecto existen usuarios que no sean los administradores del mismo
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si existen, FALSE en caso contrario</returns>
        public bool TieneUsuariosExceptoLosAdministradores(Guid pProyectoID)
        {
            List<Guid> listaID = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID) && adminProy.Tipo.Equals((short)TipoRolUsuario.Administrador)).Select(adminProy => adminProy.UsuarioID).ToList();

            List<Guid> ListaIdUsuarios = mEntityContext.ProyectoUsuarioIdentidad.Where(proyectoUser => proyectoUser.ProyectoID.Equals(pProyectoID) && !listaID.Contains(proyectoUser.UsuarioID)).Select(res => res.UsuarioID).ToList();

            return (ListaIdUsuarios.Count > 0);
        }

        /// <summary>
        /// Comprueba si existe alguna categor?a de tesauro en el proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si existen, FALSE en caso contrario</returns>
        public bool TienecategoriasDeTesauro(Guid pProyectoID)
        {
            List<Guid> categoriasTesauro = mEntityContext.CategoriaTesauro.Join(mEntityContext.TesauroProyecto, categoriaTes => categoriaTes.TesauroID, tesProyecto => tesProyecto.TesauroID, (categoriaTes, tesProyecto) => new
            {
                CategoriaTesauroID = categoriaTes.CategoriaTesauroID,
                ProyectoID = tesProyecto.ProyectoID
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.CategoriaTesauroID).ToList();

            List<Guid> listaSugerenciasID = mEntityContext.CategoriaTesauroSugerencia.Join(mEntityContext.TesauroProyecto, categoriaTesSug => categoriaTesSug.TesauroSugerenciaID, tesProyecto => tesProyecto.TesauroID, (categoriaTesSug, tesProyecto) => new
            {
                SugerenciaID = categoriaTesSug.SugerenciaID,
                ProyectoID = tesProyecto.ProyectoID
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).Select(objecto => objecto.SugerenciaID).ToList();

            return ((categoriasTesauro != null) || (listaSugerenciasID != null));
        }

        /// <summary>
        /// Obtienen los proyectos a los que acceden las identidades que tienen acceso a un proyecto pasado como par?metro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosDeIdentidadesAccedenAProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProycto = new DataWrapperProyecto();

            List<Guid> listaPerfilID = mEntityContext.Identidad.Where(identidad => identidad.FechaBaja == null && identidad.ProyectoID.Equals(pProyectoID)).Select(identidad => identidad.PerfilID).ToList();

            var proyectos = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyect => proyect.ProyectoID, identidad => identidad.ProyectoID, (proyect, identidad) => new
            {
                Proyecto = proyect,
                Identidad = identidad
            }).Where(proyectoIdentidad => proyectoIdentidad.Proyecto.TipoAcceso != (short)TipoAcceso.Privado && proyectoIdentidad.Proyecto.TipoAcceso != (short)TipoAcceso.Reservado && proyectoIdentidad.Proyecto.Estado != (short)EstadoProyecto.Cerrado && proyectoIdentidad.Proyecto.Estado != (short)EstadoProyecto.CerradoTemporalmente && proyectoIdentidad.Proyecto.TipoProyecto != (short)TipoProyecto.MetaComunidad && proyectoIdentidad.Identidad.FechaBaja == null && listaPerfilID.Contains(proyectoIdentidad.Identidad.PerfilID)).Select(objeto => new
            {
                OrganizacionID = objeto.Proyecto.OrganizacionID,
                ProyectoID = objeto.Proyecto.ProyectoID,
                Nombre = objeto.Proyecto.Nombre,
                TipoProyecto = objeto.Proyecto.TipoProyecto,
                TipoAcceso = objeto.Proyecto.TipoAcceso,
                NumeroRecursos = objeto.Proyecto.NumeroRecursos,
                NumeroPreguntas = objeto.Proyecto.NumeroPreguntas,
                NumeroDebates = objeto.Proyecto.NumeroDebates,
                NumeroMiembros = objeto.Proyecto.NumeroMiembros,
                NumeroOrgRegistradas = objeto.Proyecto.NumeroOrgRegistradas,
                EsProyectoDestacado = objeto.Proyecto.EsProyectoDestacado,
                Estado = objeto.Proyecto.Estado,
                URLPropia = objeto.Proyecto.URLPropia,
                NombreCorto = objeto.Proyecto.NombreCorto,
                Descripcion = objeto.Proyecto.Descripcion,
                ProyectoSuperiorID = objeto.Proyecto.ProyectoSuperiorID,
                TieneTwitter = objeto.Proyecto.TieneTwitter,
                TagTwitter = objeto.Proyecto.TagTwitter,
                UsuarioTwitter = objeto.Proyecto.UsuarioTwitter,
                TokenTwitter = objeto.Proyecto.TokenTwitter,
                TokenSecretoTwitter = objeto.Proyecto.TokenSecretoTwitter,
                EnviarTwitterComentario = objeto.Proyecto.EnviarTwitterComentario,
                EnviarTwitterNuevaCat = objeto.Proyecto.EnviarTwitterNuevaCat,
                EnviarTwitterNuevoAdmin = objeto.Proyecto.EnviarTwitterNuevoAdmin,
                EnviarTwitterNuevaPolitCert = objeto.Proyecto.EnviarTwitterNuevaPolitCert,
                EnviarTwitterNuevoTipoDoc = objeto.Proyecto.EnviarTwitterNuevoTipoDoc,
                TablaBaseProyectoID = objeto.Proyecto.TablaBaseProyectoID,
                ProcesoVinculadoID = objeto.Proyecto.ProcesoVinculadoID,
                Tags = objeto.Proyecto.Tags,
                TagTwitterGnoss = objeto.Proyecto.TagTwitterGnoss,
                NombrePresentacion = objeto.Proyecto.NombrePresentacion
            }).OrderBy(objeto => objeto.Nombre).Distinct().ToList();

            List<Proyecto> listaProyectos = new List<Proyecto>();
            foreach (var proyectoCorto in proyectos)
            {
                Proyecto proyect = new Proyecto();
                proyect.OrganizacionID = proyectoCorto.OrganizacionID;
                proyect.ProyectoID = proyectoCorto.ProyectoID;
                proyect.Nombre = proyectoCorto.Nombre;
                proyect.TipoProyecto = proyectoCorto.TipoProyecto;
                proyect.TipoAcceso = proyectoCorto.TipoAcceso;
                proyect.NumeroRecursos = proyectoCorto.NumeroRecursos;
                proyect.NumeroPreguntas = proyectoCorto.NumeroPreguntas;
                proyect.NumeroDebates = proyectoCorto.NumeroDebates;
                proyect.NumeroMiembros = proyectoCorto.NumeroMiembros;
                proyect.NumeroOrgRegistradas = proyectoCorto.NumeroOrgRegistradas;
                proyect.EsProyectoDestacado = proyectoCorto.EsProyectoDestacado;
                proyect.Estado = proyectoCorto.Estado;
                proyect.URLPropia = proyectoCorto.URLPropia;
                proyect.NombreCorto = proyectoCorto.NombreCorto;
                proyect.Descripcion = proyectoCorto.Descripcion;
                proyect.ProyectoSuperiorID = proyectoCorto.ProyectoSuperiorID;
                proyect.TieneTwitter = proyectoCorto.TieneTwitter;
                proyect.TagTwitter = proyectoCorto.TagTwitter;
                proyect.UsuarioTwitter = proyectoCorto.UsuarioTwitter;
                proyect.TokenTwitter = proyectoCorto.TokenTwitter;
                proyect.TokenSecretoTwitter = proyectoCorto.TokenSecretoTwitter;
                proyect.EnviarTwitterComentario = proyectoCorto.EnviarTwitterComentario;
                proyect.EnviarTwitterNuevaCat = proyectoCorto.EnviarTwitterNuevaCat;
                proyect.EnviarTwitterNuevoAdmin = proyectoCorto.EnviarTwitterNuevoAdmin;
                proyect.EnviarTwitterNuevaPolitCert = proyectoCorto.EnviarTwitterNuevaPolitCert;
                proyect.EnviarTwitterNuevoTipoDoc = proyectoCorto.EnviarTwitterNuevoTipoDoc;
                proyect.TablaBaseProyectoID = proyectoCorto.TablaBaseProyectoID;
                proyect.ProcesoVinculadoID = proyectoCorto.ProcesoVinculadoID;
                proyect.Tags = proyectoCorto.Tags;
                proyect.TagTwitterGnoss = proyectoCorto.TagTwitterGnoss;
                proyect.NombrePresentacion = proyectoCorto.NombrePresentacion;
                listaProyectos.Add(proyect);
            }
            dataWrapperProycto.ListaProyecto = listaProyectos;

            return dataWrapperProycto;
        }

        /// <summary>
        /// Obtiene los niveles de certificaci?n de un proyecto pasado como par?metro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerNivelesCertificacionRecursosProyecto(Guid pProyectoID)
        {
            List<NivelCertificacion> listNivelcertificacion = mEntityContext.NivelCertificacion.Where(nivelCertificacion => nivelCertificacion.ProyectoID.Equals(pProyectoID)).ToList();

            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaNivelCertificacion = listNivelcertificacion;

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los gadgets de un proyecto que se le pasa como par?metro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetsProyecto(Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoGadget> listaProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.ProyectoID.Equals(pProyectoID)).ToList();

            List<ProyectoGadgetContexto> listaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Where(proyectoGadget => proyectoGadget.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (ProyectoGadget gadget in listaProyectoGadget)
            {
                ProyectoGadgetContexto contexto = listaProyectoGadgetContexto.FirstOrDefault(x => x.GadgetID.Equals(gadget.GadgetID));
                gadget.ProyectoGadgetContexto = contexto;
            }

            List<ProyectoGadgetIdioma> listaProyectoGadgetIdioma = mEntityContext.ProyectoGadgetIdioma.Where(proyectoGadgetIdioma => proyectoGadgetIdioma.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaProyectoGadget = listaProyectoGadget;
            dataWrapperProyecto.ListaProyectoGadgetContexto = listaProyectoGadgetContexto;
            dataWrapperProyecto.ListaProyectoGadgetIdioma = listaProyectoGadgetIdioma;

            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los gadgets del tipo indicado de un proyecto que se le pasa como par?metro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        /// <param name="pTipoUbicacionGadget">Indica la ubicaci?n de los gadgets que se van a cargar(0-home, 1-ficha recursos)</param>
        /// <returns>DataSet con los gadgets del tipo indicado del proyecto que se pasa por par?metro</returns>
        public void ObtenerGadgetsProyectoUbicacion(Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, TipoUbicacionGadget pTipoUbicacionGadget)
        {
            pDataWrapperProyecto.ListaProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.ProyectoID.Equals(pProyectoID) && proyectoGadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget)).ToList();

            pDataWrapperProyecto.ListaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Join(mEntityContext.ProyectoGadget, proyectoGadgetContexto => proyectoGadgetContexto.GadgetID, proyectoGadget => proyectoGadget.GadgetID, (proyectoGadgetContexto, proyectoGadget) => new
            {
                ProyectoGadgetContexto = proyectoGadgetContexto,
                TipoUbicacion = proyectoGadget.TipoUbicacion
            }).Where(gadget => gadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget) && gadget.ProyectoGadgetContexto.ProyectoID.Equals(pProyectoID)).Select(gadget => gadget.ProyectoGadgetContexto).ToList();

            pDataWrapperProyecto.ListaProyectoGadgetIdioma = mEntityContext.ProyectoGadgetIdioma.Join(mEntityContext.ProyectoGadget, proyGadgetIdioma => proyGadgetIdioma.GadgetID, proyGadget => proyGadget.GadgetID, (proyGadgetIdioma, proyGadget) => new
            {
                ProyectGadgetIdioma = proyGadgetIdioma,
                TipoUbicacion = proyGadget.TipoUbicacion
            }).Where(gadget => gadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget) && gadget.ProyectGadgetIdioma.ProyectoID.Equals(pProyectoID)).Select(gadget => gadget.ProyectGadgetIdioma).ToList();
        }

        /// <summary>
        /// Obtiene los gadgets y gadgets contexto de un proyecto que se le pasa como par?metro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetsProyectoOrigen(Guid pProyectoOrigenID, DataWrapperProyecto pDataWrapperProyecto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoGadgetContexto> listaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Where(proyectoGadgetContexto => proyectoGadgetContexto.ProyectoOrigenID.Equals(pProyectoOrigenID)).ToList();
            dataWrapperProyecto.ListaProyectoGadgetContexto = listaProyectoGadgetContexto;

            List<ProyectoGadget> listaProyectoGadget = mEntityContext.ProyectoGadget.Join(mEntityContext.ProyectoGadgetContexto, gadget => gadget.GadgetID, contexto => contexto.GadgetID, (gadget, contexto) => new
            {
                ProyectoGadget = gadget,
                ProyectoOrigenID = contexto.ProyectoOrigenID
            }).Where(proyectoGadgetContexto => proyectoGadgetContexto.ProyectoOrigenID.Equals(pProyectoOrigenID)).Select(objeto => objeto.ProyectoGadget).ToList();

            dataWrapperProyecto.ListaProyectoGadget = listaProyectoGadget;
            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }

        /// <summary>
        /// Comprueba la existencia de gadgets de tipo Recursos Relacionados
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto origen</param>
        /// <returns>True si existen gadgets de tipo Recursos Relacionados</returns>
        public bool TieneGadgetRecursosRelacionados(Guid pProyectoID)
        {
            int numeroProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.ProyectoID.Equals(pProyectoID) && proyectoGadget.Tipo.Equals((short)TipoGadget.RecursosRelacionados)).ToList().Count;

            return numeroProyectoGadget > 0;
        }

        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pGadgetID">Clave del gadget</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadget(Guid pGadgetID, DataWrapperProyecto pDataWrapperProyecto, Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoGadget> listaProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.GadgetID.Equals(pGadgetID) && proyectoGadget.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoGadget = listaProyectoGadget;

            List<ProyectoGadgetContexto> listaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Where(proyectoGadgetContexto => proyectoGadgetContexto.GadgetID.Equals(pGadgetID) && proyectoGadgetContexto.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoGadgetContexto = listaProyectoGadgetContexto;

            List<ProyectoGadgetIdioma> listaProyectoGadgetIdioma = mEntityContext.ProyectoGadgetIdioma.Where(proyectoGadgetIdioma => proyectoGadgetIdioma.GadgetID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoGadgetIdioma = listaProyectoGadgetIdioma;

            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }
        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pGadgetID">Clave del gadget</param>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerDataSetGadget(Guid pGadgetID, Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            ObtenerGadget(pGadgetID, dataWrapperProyecto, pProyectoID);
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pNombreCorto">Nombrecorto del gadget</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetContextoPorNombreCorto(string pNombreCorto, DataWrapperProyecto pDataWrapperProyecto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoGadgetContexto> listaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Join(mEntityContext.ProyectoGadget, proyGadgetContext => proyGadgetContext.GadgetID, proyGadget => proyGadget.GadgetID, (proyGadgetContext, proyGadget) => new
            {
                ProyectoGadgetContexto = proyGadgetContext,
                NombreCorto = proyGadget.NombreCorto
            }).Where(objeto => objeto.NombreCorto.Equals(pNombreCorto)).Select(objeto => objeto.ProyectoGadgetContexto).ToList();
            dataWrapperProyecto.ListaProyectoGadgetContexto = listaProyectoGadgetContexto;

            List<ProyectoGadget> listaProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.NombreCorto.Equals(pNombreCorto)).ToList();
            dataWrapperProyecto.ListaProyectoGadget = listaProyectoGadget;
            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }

        /// <summary>
        /// Comprueba si existe un nombre corto de ProyectoGadget para el proyecto
        /// </summary>
        /// <param name="pNombreCortoGadget">Nombre corto del gadget</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si alg?n gadget tiene ese nombre corto</returns>
        public bool ExisteNombreCortoProyectoGadget(string pNombreCortoGadget, Guid pProyectoID)
        {
            List<Guid> encontrado = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.NombreCorto.ToUpper().Equals(pNombreCortoGadget.ToUpper()) && proyectoGadget.GadgetID.Equals(pProyectoID)).Select(proyectoGadget => proyectoGadget.GadgetID).ToList();

            return (encontrado.Count > 0);
        }
        /// <summary>
        /// Obtiene las pesta?as de men? de un proyecto que se le pasa por parametro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>Lista con las pesta?as del proyecto que se pasa por parametros</returns>
        public Dictionary<Guid, string> ObtenerPestanyasProyectoNombre(Guid pProyectoID)
        {
            List<ProyectoPestanyaMenu> listaProyectoPestanyaMenu = mEntityContext.ProyectoPestanyaMenu.Where(proyPestanya => proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Recursos) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Preguntas) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Debates) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Encuestas) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.PersonasYOrganizaciones) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.BusquedaSemantica) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.BusquedaAvanzada)).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).ToList();

            Dictionary<Guid, string> dicNombres = new Dictionary<Guid, string>();
            foreach (ProyectoPestanyaMenu pes in listaProyectoPestanyaMenu)
            {
                if (string.IsNullOrEmpty(pes.Nombre))
                {
                    dicNombres.Add(pes.PestanyaID, Enum.GetName(typeof(TipoPestanyaMenu), pes.TipoPestanya));
                }
                else
                {
                    dicNombres.Add(pes.PestanyaID, pes.Nombre);
                }
            }
            return dicNombres;
        }
		/// <summary>
		/// Obtiene la informaci?n de las tablas ProyectoPestanyaBusqueda con autocompletado enriquecido, ProyectoPestanyaBusquedaPesoOC, OntologiaProyecto y FacetaObjetoConocimientoProyectoPestanya para la configuraci?n del autocotocompletado enriquecido para un proyecto dado
		/// </summary>
		/// <param name="pProyectoID">identificador del proyecto</param>
		/// <returns>Data wraper del proyecto</returns>
		public DataWrapperProyecto ObtenerInformacionAutocompletadoEnriquecidoProyecto(Guid pProyectoID)
		{
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
			dataWrapperProyecto.ListaProyectoPestanyaBusqueda = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPestBusqueda => proyPestBusqueda.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestBusqueda, proyPestMenu) => new
			{
				ProyectPestanyaBusqueda = proyPestBusqueda,
				ProyectoID = proyPestMenu.ProyectoID
			}).Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID) /*&& proyecto.ProyectPestanyaBusqueda.TipoAutocompletar != 0*/).Select(proyecto => proyecto.ProyectPestanyaBusqueda).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenu = mEntityContext.ProyectoPestanyaMenu.Where(item => dataWrapperProyecto.ListaProyectoPestanyaBusqueda.Select(pestanya => pestanya.PestanyaID).Contains(item.PestanyaID)).ToList();

            dataWrapperProyecto.ListaFacetaObjetoConocimientoProyectoPestanya = mEntityContext.FacetaObjetoConocimientoProyectoPestanya.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

			dataWrapperProyecto.ListaProyectoPestanyaBusquedaPesoOC = mEntityContext.ProyectoPestanyaBusquedaPesoOC.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaOntologiaProyecto = mEntityContext.OntologiaProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
			return dataWrapperProyecto;
		}

		/// <summary>
		/// Obtiene las pesta?as de un proyecto que se le pasa por parametros
		/// </summary>
		/// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
		/// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
		/// <param name="pOmitirGenericas"></param>
		/// <returns>DataSet con las pesta?as del proyecto que se pasa por parametros</returns>
		public void ObtenerPestanyasProyecto(Guid? pProyectoID, DataWrapperProyecto pDataWrapperProyecto, bool pOmitirGenericas)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoPestanyaMenu> listaProyectoPestanyaMenu;
            var listaProyectoPestanyaMenuQuery = mEntityContext.ProyectoPestanyaMenu.AsQueryable();

            List<ProyectoPestanyaCMS> listaProyectoPestanyaCMS;
            var varProyectoPestanyaCMSQuery = mEntityContext.ProyectoPestanyaCMS.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaCMS => proyectoPestanyaCMS.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaCMS, proyectoPestanyaMenu) => new
            {
                ProyectoPestanyaCMS = proyectoPestanyaCMS,
                ProyectoPestanyaMenu = proyectoPestanyaMenu
            });

            List<ProyectoPestanyaBusqueda> listaProyectoPestanyaBusqueda;
            var varProyectoPestanyaBusquedaQuery = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaBusqueda => proyectoPestanyaBusqueda.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaBusqueda, proyectoPestanyaMenu) => new
            {
                ProyectoPestanyaBusqueda = proyectoPestanyaBusqueda,
                ProyectoPestanyaMenu = proyectoPestanyaMenu
            });

            List<ProyectoPestanyaMenuRolGrupoIdentidades> listProyectoPestanyaMenuRolGrupoIdentidades;
            var varProyectoPestanyaMenuRolGrupoIdentidadesQuery = mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaMenuRolGrupoIdentidades => proyectoPestanyaMenuRolGrupoIdentidades.PestanyaID, proyectoPestanya => proyectoPestanya.PestanyaID, (proyectoPestanyaMenuRolGrupoIdentidades, proyectoPestanya) => new
            {
                ProyectoPestanyaMenuRolGrupoIdentidades = proyectoPestanyaMenuRolGrupoIdentidades,
                ProyectoPestanyaMenu = proyectoPestanya
            });

            List<ProyectoPestanyaMenuRolIdentidad> listProyectoPestanyaMenuRolIdentidad;
            var varProyectoPestanyaMenuRolIdentidadQuery = mEntityContext.ProyectoPestanyaMenuRolIdentidad.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaMenuRolIdentidad => proyectoPestanyaMenuRolIdentidad.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaMenuRolIdentidad, proyectoPestanyaMenu) => new
            {
                ProyectoPestanyaMenuRolIdentidad = proyectoPestanyaMenuRolIdentidad,
                ProyectoPestanyaMenu = proyectoPestanyaMenu
            });

            if (pProyectoID.HasValue)
            {
                listaProyectoPestanyaMenuQuery = listaProyectoPestanyaMenuQuery.Where(objeto => objeto.ProyectoID.Equals(pProyectoID.Value));
                varProyectoPestanyaCMSQuery = varProyectoPestanyaCMSQuery.Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID.Value));
                varProyectoPestanyaBusquedaQuery = varProyectoPestanyaBusquedaQuery.Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID.Value));
                varProyectoPestanyaMenuRolGrupoIdentidadesQuery = varProyectoPestanyaMenuRolGrupoIdentidadesQuery.Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID.Value));
                varProyectoPestanyaMenuRolIdentidadQuery = varProyectoPestanyaMenuRolIdentidadQuery.Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID.Value));
            }
            if (pOmitirGenericas)
            {
                listaProyectoPestanyaMenuQuery = listaProyectoPestanyaMenuQuery.Where(objeto => !string.IsNullOrEmpty(objeto.Ruta));
                varProyectoPestanyaCMSQuery = varProyectoPestanyaCMSQuery.Where(objeto => !string.IsNullOrEmpty(objeto.ProyectoPestanyaMenu.Ruta));
                varProyectoPestanyaBusquedaQuery = varProyectoPestanyaBusquedaQuery.Where(objeto => !string.IsNullOrEmpty(objeto.ProyectoPestanyaMenu.Ruta));
                varProyectoPestanyaMenuRolGrupoIdentidadesQuery = varProyectoPestanyaMenuRolGrupoIdentidadesQuery.Where(objeto => !string.IsNullOrEmpty(objeto.ProyectoPestanyaMenu.Ruta));
                varProyectoPestanyaMenuRolIdentidadQuery = varProyectoPestanyaMenuRolIdentidadQuery.Where(objeto => !string.IsNullOrEmpty(objeto.ProyectoPestanyaMenu.Ruta));
            }

            listaProyectoPestanyaMenu = listaProyectoPestanyaMenuQuery.OrderBy(proyectoPestMenu => proyectoPestMenu.Orden).ToList();
            listaProyectoPestanyaCMS = varProyectoPestanyaCMSQuery.Select(objeto => objeto.ProyectoPestanyaCMS).ToList();
            listaProyectoPestanyaBusqueda = varProyectoPestanyaBusquedaQuery.Select(objeto => objeto.ProyectoPestanyaBusqueda).Include(item => item.ProyectoPestanyaMenu).ToList();
            listProyectoPestanyaMenuRolGrupoIdentidades = varProyectoPestanyaMenuRolGrupoIdentidadesQuery.Select(objeto => objeto.ProyectoPestanyaMenuRolGrupoIdentidades).ToList();
            listProyectoPestanyaMenuRolIdentidad = varProyectoPestanyaMenuRolIdentidadQuery.Select(objeto => objeto.ProyectoPestanyaMenuRolIdentidad).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenu = listaProyectoPestanyaMenu;
            dataWrapperProyecto.ListaProyectoPestanyaCMS = listaProyectoPestanyaCMS;
            dataWrapperProyecto.ListaProyectoPestanyaBusqueda = listaProyectoPestanyaBusqueda;
            dataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades = listProyectoPestanyaMenuRolGrupoIdentidades;
            dataWrapperProyecto.ListaProyectoPestanyaMenuRolIdentidad = listProyectoPestanyaMenuRolIdentidad;
            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }

        public List<ProyectoPestanyaMenu> ObtenerProyectoPestanyaMenuPorProyectoID(Guid pProyectoID)
        {
            return mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(pProyectoID)).OrderBy(item => item.Orden).ToList();
        }

        /// <summary>
        /// Nos indica si actualmente existen permisos para administrar los documentos sem?nticos
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Si existe o no permisos para que se puedan administrar los documentos sem?nticos</returns>
        public bool ExisteTipoDocDispRolUsuarioProySemantico(Guid pProyectoID)
        {
            return mEntityContext.TipoDocDispRolUsuarioProy.Any(item => item.ProyectoID.Equals(pProyectoID) && item.TipoDocumento == (short)TiposDocumentacion.Semantico);
        }

        /// <summary>
        /// Nos indica si actualmente existen permisos para administrar la ontolog?a indicada
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento de la ontologia</param>
        /// <returns>Si existe o no permisos para que se puedan administrar los documentos sem?nticos</returns>
        public bool ExisteTipoOntoDispRolUsuarioProy(Guid pProyectoID, Guid pDocumentoID)
        {
            return mEntityContext.TipoOntoDispRolUsuarioProy.Any(item => item.ProyectoID.Equals(pProyectoID) && item.OntologiaID.Equals(pDocumentoID));
        }

        public List<ProyectoPestanyaMenu> ObtenerPestanyasDeProyectoSegunPrivacidadDeIdentidad(Guid pProyectoID, Guid pIdentidadID)
        {
            Guid perfilID = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID).FirstOrDefault();

            List<Guid> listaIdentidadesPerfil = mEntityContext.Identidad.Where(item => item.PerfilID.Equals(perfilID)).Select(item => item.IdentidadID).ToList();

            List<Guid> listaGruposPerteneceIdentidad = mEntityContext.GrupoIdentidadesParticipacion.Where(item => listaIdentidadesPerfil.Contains(item.IdentidadID)).Select(item => item.GrupoID).ToList();


            var subconsultaPestanyasGrupos = mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Where(item => listaGruposPerteneceIdentidad.Contains(item.GrupoID)).Select(item => item.PestanyaID);
            var subconsultaPestanyasPerfil = mEntityContext.ProyectoPestanyaMenuRolIdentidad.Where(item => item.PerfilID.Equals(perfilID)).Select(item => item.PestanyaID);

            var conjuntoPestanyasIDs = subconsultaPestanyasGrupos.Union(subconsultaPestanyasPerfil).Distinct();

            return mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(pProyectoID) && (conjuntoPestanyasIDs.Contains(item.PestanyaID) && item.Privacidad == 2 || item.Privacidad == 0)).OrderBy(item => item.Orden).ToList();
        }

        /// <summary>
        /// Obtiene las pesta?as de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>DataSet con las p?ginas html del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerPaginasHtmlProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoPaginaHtml = mEntityContext.ProyectoPaginaHtml.Where(proyectoPaginaHTML => proyectoPaginaHTML.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene la tabla RecursosRelacionadosPresentacion del proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del gadget</param>
        /// <returns>DataSet con RecursosRelacionadosPresentacion</returns>
        public DataWrapperProyecto ObtenerRecursosRelacionadosPresentacion(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaRecursosRelacionadosPresentacion = mEntityContext.RecursosRelacionadosPresentacion.Where(recurso => recurso.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los proyectos relacionados de un proyecto que se le pasa como par?metro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los proyectos</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los proyectos del proyecto que se pasa por parametros</returns>
        public void ObtenerProyectosRelacionados(Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoRelacionado = mEntityContext.ProyectoRelacionado.Where(proyectoRelacionado => proyectoRelacionado.ProyectoID.Equals(pProyectoID)).ToList();
            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }

        /// <summary>
        /// Actualiza los proyectos
        /// </summary>
        /// <param name="pProyectoDS">Dataset de proyectos para actualizar</param>
        /// <param name="pRecalculandoProyectosMasActivos">TRUE si se est?n actualizando los proyectos m?s activos</param>
        public void ActualizarProyectos(bool pPeticionIntegracionContinua = false)
        {
            mEntityContext.SaveChanges();
        }

        private List<DataRow> OrdenarFilasPestanyasmenu(DataRow[] filasPestanyaMenu, DataTable pTablaProyectoPestanyaMenu)
        {
            List<DataRow> listaOrdenada = new List<DataRow>();
            foreach (DataRow fila in filasPestanyaMenu)
            {
                listaOrdenada.Add(fila);
                if (pTablaProyectoPestanyaMenu.Select("PestanyaPadreID='" + fila["PestanyaID"] + "'").Length > 0)
                {
                    listaOrdenada.AddRange(OrdenarFilasPestanyasmenu(pTablaProyectoPestanyaMenu.Select("PestanyaPadreID='" + fila["PestanyaID"] + "'"), pTablaProyectoPestanyaMenu));
                }
            }

            return listaOrdenada;
        }

        /// <summary>
        /// Comprueba si existe un nombre corto de proyecto en BD
        /// </summary>
        /// <param name="pNombreCortoProyecto"></param>
        /// <returns>TRUE si existe un nombroCorto en BD igual al pasado por par?metro</returns>
        public bool ExisteNombreCortoEnBD(string pNombreCortoProyecto)
        {
            return mEntityContext.Proyecto.Any(proyecto => proyecto.NombreCorto == pNombreCortoProyecto);
        }

        /// <summary>
        /// Comprueba si existe un nombre de proyecto en BD
        /// </summary>
        /// <param name="pNombreProyecto">Nombre de proyecto</param>
        /// <returns>TRUE si existe un nombre en BD igual al pasado por par?metro</returns>
        public bool ExisteNombreEnBD(string pNombreProyecto)
        {
            return mEntityContext.Proyecto.Any(proyecto => proyecto.Nombre.ToUpper() == pNombreProyecto);
        }

        /// <summary>
        /// Comprueba si existe un proyecto con el ID
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public bool ExisteProyectoConID(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Any(proyecto => proyecto.ProyectoID.Equals(pProyectoID));
        }

        /// <summary>
        /// Comprueba si alg?n usuario de la organizaci?n (personas con usuario vinculadas con la organizaci?n) 
        /// es administrador del proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci?n</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si se encuentra algun usuario de la organizaci?n que sea administrador del proyecto</returns>
        public bool EsAlguienDeLAOrganizacionAdministradorProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            List<Guid> listaIdentidadID = mEntityContext.AdministradorProyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, adminProy => adminProy.UsuarioID, proyUsuarioIdentidad => proyUsuarioIdentidad.UsuarioID, (adminProy, proyUsuarioIdentidad) => new
            {
                AdministradorProyecto = adminProy,
                ProyectoUsuarioIdentidad = proyUsuarioIdentidad
            }).Where(objeto => objeto.AdministradorProyecto.ProyectoID.Equals(pProyectoID) && objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador)).Select(objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID).ToList();

            return mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
            {
                Idenitdad = identidad,
                Perfil = perfil
            }).Any(objeto => listaIdentidadID.Contains(objeto.Idenitdad.IdentidadID) && objeto.Perfil.OrganizacionID != null);
        }

        /// <summary>
        /// Carga la presentaci?n de todos los documentos sem?nticos en una comunidad 
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionSemantico(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            #region Listado

            dataWrapperProyecto.ListaPresentacionListadoSemantico = mEntityContext.PresentacionListadoSemantico.Where(presentacionListSemantico => presentacionListSemantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacionListSemantico => presentacionListSemantico.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico = mEntityContext.PresentacionPestanyaListadoSemantico.Where(presentacionPestListadoSeman => presentacionPestListadoSeman.ProyectoID.Equals(pProyectoID)).OrderBy(presentacionPestListadoSeman => presentacionPestListadoSeman.Orden).ToList();
            
            #endregion

            #region Mosaico

            dataWrapperProyecto.ListaPresentacionMosaicoSemantico = mEntityContext.PresentacionMosaicoSemantico.Where(presentacionMosaicoSmenatico => presentacionMosaicoSmenatico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico = mEntityContext.PresentacionPestanyaMosaicoSemantico.Where(presentacionPestanyaMosaicoSemantico => presentacionPestanyaMosaicoSemantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();
            #endregion

            #region Mapa

            dataWrapperProyecto.ListaPresentacionMapaSemantico = mEntityContext.PresentacionMapaSemantico.Where(presentacionMapa => presentacionMapa.ProyectoID.Equals(pProyectoID)).OrderBy(presentacionMapa => presentacionMapa.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico = mEntityContext.PresentacionPestanyaMapaSemantico.Where(presentacionPestanyaMapa => presentacionPestanyaMapa.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            #endregion

            #region Dataset

            dataWrapperProyecto.ListaPresentacionPersonalizadoSemantico = mEntityContext.PresentacionPersonalizadoSemantico.Where(presentacionPersonalizadoSemantico => presentacionPersonalizadoSemantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacionPersonalizadoSemantico => presentacionPersonalizadoSemantico.Orden).ToList();

            #endregion

            dataWrapperProyecto.ListaProyectoPestanyaBusqueda = mEntityContext.ProyectoPestanyaBusqueda.JoinProyectoPestanyaMenu().Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.ProyectoPestanyaBusqueda).ToList();

            #region Contextos

            dataWrapperProyecto.ListaRecursosRelacionadosPresentacion = mEntityContext.RecursosRelacionadosPresentacion.Where(recursosRelacionados => recursosRelacionados.ProyectoID.Equals(pProyectoID)).OrderBy(recursosRelacionados => recursosRelacionados.Orden).ToList();

            #endregion

            return dataWrapperProyecto;
        }

        public List<PresentacionMapaSemantico> ObtenerListaPresentacionMapaSemantico(Guid pProyectoID, string pNombreOnto = "")
        {
            if (string.IsNullOrEmpty(pNombreOnto))
            {
                return mEntityContext.PresentacionMapaSemantico.Where(item => item.ProyectoID.Equals(pProyectoID)).OrderBy(item => item.Orden).ToList();
            }
            else
            {
                return mEntityContext.PresentacionMapaSemantico.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Ontologia.ToLower().Contains($"{pNombreOnto.ToLower()}.owl")).OrderBy(item => item.Orden).ToList();
            }
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobaci?n</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyectoPorUsuID(Guid pProyectoID, Guid pUsuarioID)
        {
            TipoRolUsuario tipoRolUsuario;
            List<AdministradorProyecto> listaAdministradoresProyecto = mEntityContext.AdministradorProyecto.Where(admin => admin.ProyectoID.Equals(pProyectoID) && admin.UsuarioID.Equals(pUsuarioID)).ToList();

            if (listaAdministradoresProyecto.Count == 0)
            {
                tipoRolUsuario = TipoRolUsuario.Usuario;
            }
            else
            {
                tipoRolUsuario = (TipoRolUsuario)listaAdministradoresProyecto[0].Tipo;
            }

            return ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(pProyectoID, tipoRolUsuario);
        }
        /// <summary>
        /// Carga la presentaci?n de todos los documentos sem?nticos en una comunidad 
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionListadoSemantico(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            #region Listado
            
            dataWrapperProyecto.ListaPresentacionListadoSemantico = mEntityContext.PresentacionListadoSemantico.Where(presentacionListadoSmenantico => presentacionListadoSmenantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();
            
            dataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico = mEntityContext.PresentacionPestanyaListadoSemantico.Where(presentacionPestanya => presentacionPestanya.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            #endregion

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Carga la presentaci?n de todos los documentos sem?nticos en una comunidad 
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionMosaicoSemantico(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            #region Mosaico

            dataWrapperProyecto.ListaPresentacionMosaicoSemantico = mEntityContext.PresentacionMosaicoSemantico.Where(presentacionMosaicoSmenantico => presentacionMosaicoSmenantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico = mEntityContext.PresentacionPestanyaMosaicoSemantico.Where(presentacionPestanya => presentacionPestanya.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();
            
            #endregion
            
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Carga la presentaci?n de todos los documentos sem?nticos en una comunidad 
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionMapaSemantico(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            #region Mapa

            dataWrapperProyecto.ListaPresentacionMapaSemantico = mEntityContext.PresentacionMapaSemantico.Where(presentacionMapaSmenantico => presentacionMapaSmenantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico = mEntityContext.PresentacionPestanyaMapaSemantico.Where(presentacionPestanya => presentacionPestanya.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            #endregion

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Elimina los registros de proyectos del dataset pasado como par?metro
        /// </summary>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        public void EliminarProyectos()
        {
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Obtiene el identificador del metaproyecto
        /// </summary>
        /// <returns>Identificador del metaproyecto</returns>
        public Guid ObtenerMetaProyectoID()
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el identificador de la metaorganizaci?n
        /// </summary>
        /// <returns>Identificador de la metaorganizaci?n</returns>
        public Guid ObtenerMetaOrganizacionID()
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad)).Select(proyecto => proyecto.OrganizacionID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene un listado con todas las urls propias de los proyectos
        /// </summary>
        /// <returns>Lista de strings</returns>
        public List<string> ObtenerUrlPropiasProyectos()
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia != null).Select(proyecto => proyecto.URLPropia).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene un listado con todas las urls propias de los proyectos
        /// </summary>
        /// <returns>Lista de strings</returns>
        public List<string> ObtenerUrlPropiasProyectosPublicos()
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia != null && (proyecto.TipoAcceso.Equals((short)TipoAcceso.Publico) || proyecto.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proyecto => proyecto.URLPropia).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene los contadores de reucursos, personas y organizaciones de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de la comunidad</param>
        /// <returns>DataSet con Proyecto con los contadores de reucursos, personas y organizaciones de una comunidad</returns>
        public DataWrapperProyecto ObtenerContadoresProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto proyectoDataWrapper = new DataWrapperProyecto();
            
            proyectoDataWrapper.ListaProyecto = mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy).ToList();
            
            return proyectoDataWrapper;
        }

        /// <summary>
        /// Obtiene los tags de varios proyectos
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerTagsDeProyectos(List<Guid> pListaProyectoID)
        {
            Dictionary<Guid, string> tags = new Dictionary<Guid, string>();

            foreach (Guid id in pListaProyectoID)
            {
                tags.Add(id, "");
            }

            if (pListaProyectoID.Count > 0)
            {
                var consulta = mEntityContext.Proyecto.Where(proyecto => pListaProyectoID.Contains(proyecto.ProyectoID)).Select(proyecto => new { proyecto.ProyectoID, proyecto.Tags }).ToList();

                foreach (var fila in consulta)
                {
                    if (!string.IsNullOrEmpty(fila.Tags))
                    {
                        Guid idDoc = fila.ProyectoID;
                        string tagss = fila.Tags;
                        tags[idDoc] = tagss;
                    }
                }

            }
            return tags;
        }

        /// <summary>
        /// Obtiene los datos de una carga a partir de su ID
        /// </summary>
        /// <param name="pCargaID">Identificador de la carga</param>
        /// <returns>Datos de la carga masiva</returns>
        public Carga ObtenerDatosCargaPorID(Guid pCargaID)
        {
            Carga carga = mEntityContext.Carga.Where(item => item.CargaID.Equals(pCargaID)).FirstOrDefault();
            return carga;
        }

        /// <summary>
        /// Obtener los datos del paquete de una carga masiva
        /// </summary>
        /// <param name="pPaqueteID"></param>
        /// <returns></returns>
        public CargaPaquete ObtenerDatosPaquete(Guid pPaqueteID)
        {
            return mEntityContext.CargaPaquete.Where(item => item.PaqueteID.Equals(pPaqueteID)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la ontolog?a a la que pertenece una carga masiva a partir
        /// del id de la carga
        /// </summary>
        /// <param name="pCargaId">Identificador de la carga de la cual queremos obtener la ontolog?a</param>
        /// <returns>La ontolog?a a la que pertenece la carga</returns>
        public string ObtenerOntologiaCarga(Guid pCargaId)
        {
            return mEntityContext.Carga.Where(item => item.CargaID.Equals(pCargaId)).Select(item => item.Ontologia).FirstOrDefault();
        }

        /// <summary>
        /// Devuelve el proyectoID a partir de la base de recursos de un proyecto
        /// </summary>
        /// <param name="pBaseRecursosID"></param>
        /// <returns>Si encuentra el proyectoID sino Guid.empty</returns>
        public Guid ObtenerProyectoIDPorBaseRecursos(Guid pBaseRecursosID)
        {
            return mEntityContext.BaseRecursosProyecto.Where(baseRecursoProyecto => baseRecursoProyecto.BaseRecursosID.Equals(pBaseRecursosID)).Select(baseRecursoProyecto => baseRecursoProyecto.ProyectoID).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ObtenerDatosPorBaseRecursosPersona(Guid pBaseRecursosID, Guid pPersonaID, out Guid pProyectoID, out Guid pIdentidadID, out Guid pOrganizacionID)
        {
            var datos = mEntityContext.BaseRecursosProyecto.Join(mEntityContext.Identidad, baseRecursoProy => baseRecursoProy.ProyectoID, identidad => identidad.ProyectoID, (baseRecursoProy, identidad) => new
            {
                BaseRecursoProyecto = baseRecursoProy,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                BaseRecursoProyecto = objeto.BaseRecursoProyecto,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Where(objeto => !objeto.Identidad.FechaBaja.HasValue && objeto.Perfil.PersonaID.HasValue && objeto.Perfil.PersonaID.Value.Equals(pPersonaID) && objeto.BaseRecursoProyecto.BaseRecursosID.Equals(pBaseRecursosID)).Select(objeto => new
            {
                ProyectoID = objeto.Identidad.ProyectoID,
                IdentidadID = objeto.Identidad.IdentidadID,
                OrganizacionID = objeto.Perfil.OrganizacionID
            }).ToList();

            pProyectoID = Guid.Empty;
            pIdentidadID = Guid.Empty;
            pOrganizacionID = Guid.Empty;

            if (datos.Count > 0)
            {
                pProyectoID = datos[0].ProyectoID;
                pIdentidadID = datos[0].IdentidadID;

                if (datos[0].OrganizacionID.HasValue)
                {
                    pOrganizacionID = datos[0].OrganizacionID.Value;
                }
            }
        }

        /// <summary>
        /// Devuelve la Base de Recursos de un ProyectoID
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que queremos la Base de recursos</param>
        /// <returns>Base de recursos ID</returns>
        public Guid ObtenerBaseRecursosProyectoPorProyectoID(Guid pProyectoID)
        {
           return mEntityContext.BaseRecursosProyecto.Where(baseRecurso => baseRecurso.ProyectoID.Equals(pProyectoID)).Select(baseRecurso => baseRecurso.BaseRecursosID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los grafos gr?ficos configurados en un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>DataSet la tabla 'ProyectoGrafoFichaRec' con los grafos gr?ficos configurados en un proyecto</returns>
        public DataWrapperProyecto ObtenerGrafosProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyectoGrafoFichaRec = mEntityContext.ProyectoGrafoFichaRec.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los nosmbres cortos de todas las comunidades de los tipos especificados
        /// </summary>
        /// <param name="pListaTipos">Lista de tipos de comunidades</param>
        /// <returns>Nombres cortos</returns>
        public List<string> ObtenerNombresCortosProyectosPorTipo(List<TipoProyecto> pListaTipos)
        {
            return mEntityContext.Proyecto.Where(proyecto => pListaTipos.Contains((TipoProyecto)proyecto.TipoProyecto)).Select(proyecto => proyecto.NombreCorto).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene un listado con los proyectos que tienen configuracion de newasletter por defecto (Guid.Empty especifica que es confgiracion del ecosistema)
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, bool> ObtenerProyectosConConfiguracionNewsletterPorDefecto()
        {
            Dictionary<Guid, bool> NewsletterProyecto = new Dictionary<Guid, bool>();

            //Proyecto            
            var resultsProyecto = mEntityContext.ParametroProyecto.Where(parametroProyecto => parametroProyecto.Parametro.Equals(Parametro.ParametroAD.RecibirNewsletterDefecto)).Select(parametroProyecto => new { parametroProyecto.ProyectoID, parametroProyecto.Valor }).ToList();

            foreach (var resultProyecto in resultsProyecto)
            {
                Guid idproyecto = resultProyecto.ProyectoID;
                string valor = resultProyecto.Valor;
                NewsletterProyecto.Add(idproyecto, valor.Trim().ToLower() == "1" || valor.Trim().ToLower() == "true");
            }

            //Ecosistema
            List<string> resultsEcosistema = mEntityContext.ParametroAplicacion.Where(parametroAplicacion => parametroAplicacion.Parametro.Equals(ParametroAplicacion.TiposParametrosAplicacion.RecibirNewsletterDefecto)).Select(parametro => parametro.Valor).ToList();

            foreach (string resultEcosistema in resultsEcosistema)
            {
                string valor = resultEcosistema;
                NewsletterProyecto.Add(Guid.Empty, valor.Trim().ToLower() == "1" || valor.Trim().ToLower() == "true");
            }

            return NewsletterProyecto;
        }

        #region Datos para Twitter

        /// <summary>
        /// Actualiza los tokens para Twitter del proyecto pasado por par?metro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pTokenTwitter">Token para Twitter</param>
        /// <param name="pTokenSecretoTwitter">Token secreto para Twitter</param>
        public void ActualizarTokenTwitterProyecto(Guid pProyectoID, string pTokenTwitter, string pTokenSecretoTwitter)
        {
            DbCommand commandsqlActualizarTokensTwitterProyecto = ObtenerComando(sqlUpdateTokenTwitterProyecto);

            AgregarParametro(commandsqlActualizarTokensTwitterProyecto, IBD.ToParam("ProyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            AgregarParametro(commandsqlActualizarTokensTwitterProyecto, IBD.ToParam("TokenTwitter"), DbType.String, pTokenTwitter);
            AgregarParametro(commandsqlActualizarTokensTwitterProyecto, IBD.ToParam("TokenSecretoTwitter"), DbType.String, pTokenSecretoTwitter);

            ActualizarBaseDeDatos(commandsqlActualizarTokensTwitterProyecto);
        }

        #endregion

        #region Documentaci?n

        /// <summary>
        /// Actualiza el n?mero de recursos, preguntas y debates de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void ActulizarNumeroDocumentacion(Guid pProyectoID)
        {            
            try
            {
                var comun = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docWebVin => docWebVin.DocumentoID, (documento, docWebVin) => new
                {
                    Documento = documento,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (objeto, baseRecursoProy) => new
                {
                    Documento = objeto.Documento,
                    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                    BaseRecursosProyecto = baseRecursoProy
                }).Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores);

                int numRec = comun.Where(objeto => objeto.Documento.Tipo != (short)TiposDocumentacion.Debate && objeto.Documento.Tipo != (short)TiposDocumentacion.Pregunta && objeto.Documento.Tipo != (short)TiposDocumentacion.Ontologia).Select(objeto => objeto.Documento.DocumentoID).ToList().Count;

                int numPre = comun.Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta)).Select(objeto => objeto.Documento.DocumentoID).ToList().Count;

                int numDeb = comun.Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate)).Select(objeto => objeto.Documento.DocumentoID).ToList().Count;

                List<Proyecto> listaProyectos = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();

                foreach (Proyecto proyecto in listaProyectos)
                {
                    proyecto.NumeroRecursos = numRec;
                    proyecto.NumeroPreguntas = numPre;
                    proyecto.NumeroDebates = numDeb;
                }

                #region ProyectoPerfilNumElem

                List<ProyectoPerfilNumElem> listaEliminar = mEntityContext.ProyectoPerfilNumElem.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
                foreach (ProyectoPerfilNumElem proy in listaEliminar)
                {
                    mEntityContext.ProyectoPerfilNumElem.Remove(proy);
                }

                var elementoInsertar = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docWebBinBase => docWebBinBase.DocumentoID, (documento, docWebBinBase) => new
                {
                    Documento = documento,
                    DocumentoWebVinBaseRecursos = docWebBinBase
                }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecurso => baseRecurso.BaseRecursosID, (objeto, baseRecurso) => new
                {
                    Documento = objeto.Documento,
                    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                    BaseRecursoProyecto = baseRecurso
                }).Join(mEntityContext.DocumentoRolIdentidad, objeto => objeto.Documento.DocumentoID, documentoRolIdenitdad => documentoRolIdenitdad.DocumentoID, (objeto, documentoRolIdentidad) => new
                {
                    Documento = objeto.Documento,
                    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                    BaseRecursoProyecto = objeto.BaseRecursoProyecto,
                    DocumentoRolIdentidad = documentoRolIdentidad
                }).Where(objeto => objeto.BaseRecursoProyecto.ProyectoID.Equals(pProyectoID) && !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores).GroupBy(objeto => new { objeto.BaseRecursoProyecto.ProyectoID, objeto.DocumentoRolIdentidad.PerfilID }).Select(objeto => new { ProyectoID = objeto.Key.ProyectoID, PerfilID = objeto.Key.PerfilID, Documentos = objeto.Count() }).FirstOrDefault();
                ProyectoPerfilNumElem newProyectoPerfilNumElem = new ProyectoPerfilNumElem();
                if (elementoInsertar != null)
                {
                    newProyectoPerfilNumElem.PerfilID = elementoInsertar.PerfilID;
                    newProyectoPerfilNumElem.ProyectoID = elementoInsertar.ProyectoID;
                    newProyectoPerfilNumElem.NumRecursos = elementoInsertar.Documentos;

                    mEntityContext.ProyectoPerfilNumElem.Add(newProyectoPerfilNumElem);
                    mEntityContext.SaveChanges();
                }

                #endregion
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e,mlogger);
            }
        }

        /// <summary>
        /// Obtiene el tipo de proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public TipoProyecto ObtenerTipoProyecto(Guid pProyectoID)
        {

            short tipoProyecto = 0;

            object resultado = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.TipoProyecto).FirstOrDefault();

            if (resultado != null)
            {
                tipoProyecto = (short)resultado;
            }
            return (TipoProyecto)tipoProyecto;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperProyecto CargarProyectosDeOrganizacionCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoConAdministrado> proyectos = mEntityContext.Proyecto.Join(mEntityContext.OrganizacionParticipaProy, proyecto => proyecto.ProyectoID, orgParticipaProy => orgParticipaProy.ProyectoID, (proyecto, orgParticipaProy) => new
            {
                Proyecto = proyecto,
                OrganizacionParticipaProy = orgParticipaProy
            }).Join(mEntityContext.AdministradorProyecto, objeto => objeto.Proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (objeto, adminProy) => new
            {
                Proyecto = objeto.Proyecto,
                OrganizacionParticipaProy = objeto.OrganizacionParticipaProy,
                AdministradorProyecto = adminProy
            }).Join(mEntityContext.Persona, objeto => objeto.AdministradorProyecto.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new
            {
                Proyecto = objeto.Proyecto,
                OrganizacionParticipaProy = objeto.OrganizacionParticipaProy,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Persona = persona
            }).GroupJoin(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                Proyecto = objeto.Proyecto,
                OrganizacionParticipaProy = objeto.OrganizacionParticipaProy,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Persona = objeto.Persona,
                Perfil = perfil
            }).SelectMany(x => x.Perfil.DefaultIfEmpty(), (x, y) => new
            {
                Proyecto = x.Proyecto,
                OrganizacionParticipaProy = x.OrganizacionParticipaProy,
                AdministradorProyecto = x.AdministradorProyecto,
                Persona = x.Persona,
                Perfil = y
            }).Where(objeto => objeto.OrganizacionParticipaProy.OrganizacionID.Equals(pOrganizacionID) && objeto.Proyecto.Estado != 0 && objeto.Proyecto.Estado != 1 && objeto.Proyecto.ProyectoID != ProyectoAD.MetaProyecto).ToList()
            .GroupBy(objeto => new { objeto.Proyecto.OrganizacionID, objeto.Proyecto.ProyectoID, objeto.Proyecto.Nombre }, objeto => objeto.Perfil?.OrganizacionID, (objetoAgrup, g) => new
            {
                OrganizacionID = objetoAgrup.OrganizacionID,
                ProyectoID = objetoAgrup.ProyectoID,
                Nombre = objetoAgrup.Nombre,
                Administrado = g.ToList().Count > 0 ? 0 : 1
            }).Select(objeto => new ProyectoConAdministrado { OrganizacionID = objeto.OrganizacionID, ProyectoID = objeto.ProyectoID, Nombre = objeto.Nombre, Administrado = objeto.Administrado }).Distinct().ToList();

            dataWrapperProyecto.ListaProyectoConAdministrado = proyectos;
            
            return dataWrapperProyecto;
        }



        public List<UsuarioAdministradorComunidad> CargarProyectosParticipaPersonaOrg(Guid pOrganizacionID, Guid pPersonaID)
        {

            var administradoresProyectoSQL = mEntityContext.Proyecto.JoinOrganizacionParticipaProy().JoinPerfil().LeftJoinIdentidad().LeftJoinPerfil().LeftJoinPersona().LeftJoinAdministradorProyecto().Where(x => !x.Identidad.FechaBaja.HasValue && !x.Identidad.FechaExpulsion.HasValue && x.OrganizacionParticipaProy.OrganizacionID.Equals(pOrganizacionID) && x.Perfil.PersonaID.HasValue && x.Perfil.PersonaID.Value.Equals(pPersonaID) && x.Proyecto.Estado != 0 && x.Proyecto.Estado != 1 && !x.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto));

            var administradoresProyectoSQL2 = administradoresProyectoSQL.GroupBy(x => new { OrganizacionID = x.Proyecto.OrganizacionID, ProyectoID = x.Proyecto.ProyectoID, Nombre = x.Proyecto.Nombre, Tipo = x.Identidad.Tipo });

            var administradoresProyectoSQL3 = administradoresProyectoSQL2.Select(x => new
            {
                OrganizacionID = x.Key.OrganizacionID,
                ProyectoID = x.Key.ProyectoID,
                Nombre = x.Key.Nombre,
                Tipo = ((short?)x.Key.Tipo).HasValue ? (short?)x.Key.Tipo : null,
                Administrador = (x.Count(y => ((Guid?)y.AdministradorProyecto.UsuarioID).HasValue)) > 0 ? 1 : 0
            }).Distinct();


            List<UsuarioAdministradorComunidad> administradoresProyecto = administradoresProyectoSQL3.Select(x => new UsuarioAdministradorComunidad
            {
                OrganizacionID = x.OrganizacionID,
                ProyectoID = x.ProyectoID,
                Nombre = x.Nombre,
                Tipo = x.Tipo,
                Administrador = x.Administrador
            }).OrderBy(item => item.Nombre).ToList();


            return administradoresProyecto;
        }


        /// <summary>
        /// Obtiene el estado de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public EstadoProyecto ObtenerEstadoProyecto(Guid pProyectoID)
        {
            short estado = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.Estado).FirstOrDefault();

            return (EstadoProyecto)estado;
        }

        /// <summary>
        /// Obtiene el tipo de acceso a un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public TipoAcceso ObtenerTipoAccesoProyecto(Guid pProyectoID)
        {
            short tipoAcceso = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.TipoAcceso).FirstOrDefault();
            
            return (TipoAcceso)tipoAcceso;
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobaci?n</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(Guid pProyectoID, TipoRolUsuario pTipoRol)
        {
            List<TiposDocumentacion> listaTiposDoc = new List<TiposDocumentacion>();

            List<TipoDocDispRolUsuarioProy> listaTipoDocRolUsuario = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipoDocRolUs => tipoDocRolUs.ProyectoID.Equals(pProyectoID) && tipoDocRolUs.RolUsuario >= (short)pTipoRol).ToList();

            foreach (TipoDocDispRolUsuarioProy filaTipoDosRolUsu in listaTipoDocRolUsuario)
            {
                listaTiposDoc.Add((TiposDocumentacion)filaTipoDosRolUsu.TipoDocumento);
            }

            return listaTiposDoc;
        }

        /// <summary>
        /// Obtiene el rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobaci?n</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>El rol de usuario en un determinado proyecto</returns>
        public TipoRolUsuario ObtenerRolUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            TipoRolUsuario tipoRolUsuario;

            var adminProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(pUsuarioID)).Select(adminProy => new
            {
                OrganizacionID = adminProy.OrganizacionID,
                ProyectoID = adminProy.ProyectoID,
                UsuarioID = adminProy.UsuarioID,
                Tipo = adminProy.Tipo
            });


            if (adminProyecto.ToList().Count == 0)
            {
                tipoRolUsuario = TipoRolUsuario.Usuario;
            }
            else
            {
                tipoRolUsuario = (TipoRolUsuario)adminProyecto.First().Tipo;
            }

            return tipoRolUsuario;
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobaci?n</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyectoPorSuID(Guid pProyectoID, Guid pUsuarioID)
        {
            TipoRolUsuario tipoRolUsuario;

            var adminProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(pUsuarioID)).Select(adminProy => new
            {
                OrganizacionID = adminProy.OrganizacionID,
                ProyectoID = adminProy.ProyectoID,
                UsuarioID = adminProy.UsuarioID,
                Tipo = adminProy.Tipo
            });

            if (adminProyecto.ToList().Count == 0)
            {
                tipoRolUsuario = TipoRolUsuario.Usuario;
            }
            else
            {
                tipoRolUsuario = (TipoRolUsuario)adminProyecto.First().Tipo;
            }


            return ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(pProyectoID, tipoRolUsuario);
        }

        /// <summary>
        /// Obtiene los filtros de ordenes disponibles para un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoPestanyaFiltroOrdenRecursos' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerFiltrosOrdenesDeProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyDS = new DataWrapperProyecto();

            List<ProyectoPestanyaFiltroOrdenRecursos> listaProyectoPestanyaFiltroOrdenRecursos = mEntityContext.ProyectoPestanyaFiltroOrdenRecursos.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaFiltro => proyectoPestanyaFiltro.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaFiltro, proyectoPestanyaMenu) => new
            {
                ProyectoPestanyaFiltro = proyectoPestanyaFiltro,
                ProyectoID = proyectoPestanyaMenu.ProyectoID
            }).Where(busqueda => busqueda.ProyectoID.Equals(pProyectoID)).Select(busqueda => busqueda.ProyectoPestanyaFiltro).ToList();

            dataWrapperProyDS.ListaProyectoPestanyaFiltroOrdenRecursos = listaProyectoPestanyaFiltroOrdenRecursos;
            return dataWrapperProyDS;
        }

        /// <summary>
        /// Obtiene los tesauros sem?nticos configurados para edici?n.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoConfigExtraSem' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerTesaurosSemanticosConfigEdicionDeProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto DataWrapperProyDS = new DataWrapperProyecto();
            List<ProyectoConfigExtraSem> listaProyectoConfigExtraSem = mEntityContext.ProyectoConfigExtraSem.Where(proyectoConfigExtra => proyectoConfigExtra.ProyectoID.Equals(pProyectoID) && proyectoConfigExtra.Tipo.Equals((short)TipoConfigExtraSemantica.TesauroSemantico)).ToList();
            DataWrapperProyDS.ListaProyectoConfigExtraSem = listaProyectoConfigExtraSem;
            return DataWrapperProyDS;
        }

        /// <summary>
        /// Obtiene los tesauros sem?nticos configurados para edici?n.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoConfigExtraSem' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerConfiguracionSemanticaExtraDeProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyDS = new DataWrapperProyecto();
            List<ProyectoConfigExtraSem> listProyectoConfigExtraSem = mEntityContext.ProyectoConfigExtraSem.Where(proyectoConfigExtr => proyectoConfigExtr.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyDS.ListaProyectoConfigExtraSem = listProyectoConfigExtraSem;
            return dataWrapperProyDS;
        }

		/// <summary>
		/// Obtiene los tesauros sem?nticos configurados para edici?n.
		/// </summary>
		/// <returns>DataSet de ProyectoDS con la tabla 'ProyectoConfigExtraSem' cargada para un proyecto</returns>
		public DataWrapperProyecto ObtenerConfiguracionSemanticaExtraDeProyectos()
		{
			DataWrapperProyecto dataWrapperProyDS = new DataWrapperProyecto();
			List<ProyectoConfigExtraSem> listProyectoConfigExtraSem = mEntityContext.ProyectoConfigExtraSem.ToList();
			dataWrapperProyDS.ListaProyectoConfigExtraSem = listProyectoConfigExtraSem;
			return dataWrapperProyDS;
		}

		/// <summary>
		/// Indica si el recurso es de un Tipo de recursos que se encuentra en la lista de recursos que no se publican en la actividad reciente
		/// </summary>
		/// <param name="pRecursoID">ID del recurso</param>
		/// <param name="pProyectoID">ID del proyecto</param>
		/// <returns></returns>
		public bool ComprobarSiRecursoSePublicaEnActividadReciente(Guid pRecursoID, Guid pProyectoID)
        {
            Guid? ontologiaID = mEntityContext.Documento.Where(Documento => Documento.DocumentoID == pRecursoID && Documento.ElementoVinculadoID.HasValue).Select(Documento => Documento.ElementoVinculadoID).FirstOrDefault();

            if (ontologiaID.HasValue)
            {
                string ontoID = ontologiaID.Value.ToString();

                //Si encuentra alguna fila que cumpla las condiciones, devuelve false
                return !mEntityContext.ProyTipoRecNoActivReciente.Where(proyTipoNoRec => proyTipoNoRec.TipoRecurso == 5 && proyTipoNoRec.OntologiasID.Contains(ontoID)).Any();
            }

            return true;
        }

        /// <summary>
        /// Obtiene los tipos de recursos que no deben ir a la actividad reciente de la comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet con la tabla ProyTipoRecNoActivReciente cargada para el proyecto</returns>
        public DataWrapperProyecto ObtenerTiposRecursosNoActividadReciente(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyectoDS = new DataWrapperProyecto();
           
            List<ProyTipoRecNoActivReciente> listaProyTipoRecNoActivReciente = mEntityContext.ProyTipoRecNoActivReciente.Where(proyTipoNoRec => proyTipoNoRec.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyectoDS.ListaProyTipoRecNoActivReciente = listaProyTipoRecNoActivReciente;
            return dataWrapperProyectoDS;
        }

        /// <summary>
        /// Devuelve las im?genes por defecto seg?n el tipo de imagen por defecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Imagen por defecto seg?n el tipo de imagen por defecto</returns>
        public Dictionary<short, Dictionary<Guid, string>> ObtenerTipoDocImagenPorDefecto(Guid pProyectoID)
        {
            DataSet dataSet = new DataSet();

            DbCommand comandoSel = ObtenerComando(sqlSelectTipoDocImagenPorDefecto + " WHERE ProyectoID=" + IBD.GuidValor(pProyectoID));
            CargarDataSet(comandoSel, dataSet, "TipoDocImagenPorDefecto");
            var seleccion = mEntityContext.TipoDocImagenPorDefecto.Where(tipoDocImagen => tipoDocImagen.ProyectoID.Equals(pProyectoID)).Select(tipoDocImagen => new { tipoDocImagen.ProyectoID, tipoDocImagen.TipoRecurso, tipoDocImagen.OntologiaID, tipoDocImagen.UrlImagen }).ToList();
            Dictionary<short, Dictionary<Guid, string>> listaTipo = new Dictionary<short, Dictionary<Guid, string>>();

            foreach (var fila in seleccion)
            {
                if (!listaTipo.ContainsKey(fila.TipoRecurso))
                {
                    listaTipo.Add(fila.TipoRecurso, new Dictionary<Guid, string>());
                }

                listaTipo[fila.TipoRecurso].Add(fila.OntologiaID, fila.UrlImagen);
            }

            dataSet.Dispose();
            return listaTipo;
        }

        #endregion

        #region Administraci?n proyecto

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que administra el grupo
        /// </summary>
        /// <param name="pUsuarioID">Clave del grupo</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectosAdministradosPorGrupo(Guid pGrupoID)
        {
            List<Guid> listaGuidProyectosAdministra = mEntityContext.AdministradorGrupoProyecto.Where(adminGrupoProy => adminGrupoProy.GrupoID.Equals(pGrupoID)).Select(adminGrupoProy => adminGrupoProy.ProyectoID).ToList();
            return listaGuidProyectosAdministra;
        }

        /// <summary>
        /// Obtiene una lista con los administradores de un proyecto 
        /// (S?lo los administradores --> Tipo = 0 )
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que queremos obtener sus administradores</param>
        /// <returns>Lista con las identidades de los administradores de un proyecto</returns>
        ///            
        public List<Guid> ObtenerListaIdentidadesAdministradoresPorProyecto(Guid pProyectoID)
        {
            List<Guid> listaAdministradores = mEntityContext.Identidad.Join(mEntityContext.ProyectoUsuarioIdentidad, identidad => identidad.IdentidadID, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, (identidad, proyectoUsuarioIdentidad) => new
            {
                Identidad = identidad,
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad
            }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.UsuarioID, objeto.Identidad.ProyectoID }, adminProy => new { adminProy.UsuarioID, adminProy.ProyectoID }, (objeto, adminProy) => new
            {
                Identidad = objeto.Identidad,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.Identidad.ProyectoID.Equals(pProyectoID) && objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Identidad.FechaBaja == null && objeto.Identidad.FechaExpulsion == null)
             .Select(objeto => objeto.Identidad.IdentidadID).ToList();

            return listaAdministradores;
        }

        /// <summary>
        /// Obtiene una lista con los supervisores de un proyecto 
        /// (S?lo los administradores --> Tipo = 1 )
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que queremos obtener sus supervisores</param>
        /// <returns>Lista con las identidades de los supervisores de un proyecto</returns>
        public List<Guid> ObtenerListaIdentidadesSupervisoresPorProyecto(Guid pProyectoID)
        {
            List<Guid> listaSupervisores = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
            {
                Idenitdad = identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID, persona => persona.PersonaID, (objeto, persona) => new
            {
                Idenitdad = objeto.Idenitdad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Join(mEntityContext.Usuario, objeto => objeto.Persona.UsuarioID, usuario => usuario.UsuarioID, (objeto, usuario) => new
            {
                Idenitdad = objeto.Idenitdad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                Usuario = usuario
            }).Join(mEntityContext.AdministradorProyecto, objeto => objeto.Usuario.UsuarioID, adminProy => adminProy.UsuarioID, (objeto, adminProy) => new
            {
                Idenitdad = objeto.Idenitdad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                Usuario = objeto.Usuario,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.AdministradorProyecto.ProyectoID.Equals(pProyectoID) && objeto.Idenitdad.ProyectoID.Equals(pProyectoID) && objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Supervisor)).Select(objeto => objeto.Idenitdad.IdentidadID).ToList();

            return listaSupervisores;
        }

        /// <summary>
        /// Obtiene los nombres de los proyectos administrados por el usuario pasado por par?metro con el perfil dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerNombresProyectosAdministradosPorUsuarioID(Guid pUsuarioID, Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            
            List<Proyecto> listaProyectos = mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proy => proy.ProyectoID, adminProy => adminProy.ProyectoID, (proy, adminProy) => new
            {
                NombreProy = proy.Nombre,
                ProyectoID = proy.ProyectoID,
                TipoAcceso = proy.TipoAcceso,
                UsuarioID = adminProy.UsuarioID
            }).Join(
                mEntityContext.Identidad, proyAdminProy => proyAdminProy.ProyectoID, identidad => identidad.ProyectoID, (proyAdminProy, identidad) => new
                {
                    NombreProy = proyAdminProy.NombreProy,
                    ProyectoID = proyAdminProy.ProyectoID,
                    TipoAcceso = proyAdminProy.TipoAcceso,
                    UsuarioID = proyAdminProy.UsuarioID,
                    PerfilID = identidad.PerfilID
                }
                ).Where(proyAdminProy => ((proyAdminProy.TipoAcceso.Equals((short)TipoAcceso.Privado) || proyAdminProy.TipoAcceso.Equals((short)TipoAcceso.Reservado)) && proyAdminProy.UsuarioID.Equals(pUsuarioID)) && proyAdminProy.PerfilID.Equals(pPerfilID)).Select(proyAdminProyIden => new Proyecto { Nombre = proyAdminProyIden.NombreProy }).ToList();

            dataWrapperProyecto.ListaProyecto = listaProyectos;

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los nombres de los proyectos administrados por el usuario pasado por par?metro con el perfil dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public Dictionary<Guid, string> ObtenerNombresProyectosPrivadosAdministradosPorUsuario(Guid pUsuarioID, Guid pPerfilID)
        {
            Dictionary<Guid, string> listaComunidades = new Dictionary<Guid, string>();

            var objetosConsulta = mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (proyecto, adminProy) => new
            {
                Proyecto = proyecto,
                AdministradorProyecto = adminProy
            }).Join(mEntityContext.Identidad, objeto => objeto.Proyecto.ProyectoID, idenitdad => idenitdad.ProyectoID, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Identidad = identidad
            }).Where(objeto => ((objeto.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) || objeto.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado)) && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID)) && objeto.Identidad.PerfilID.Equals(pPerfilID)).Select(objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.Nombre }).ToList();

            //Proyecto
            foreach (var objetoConsulta in objetosConsulta)
            {
                Guid proyectoID = objetoConsulta.ProyectoID;
                if (!listaComunidades.ContainsKey(proyectoID))
                {
                    listaComunidades.Add(proyectoID, objetoConsulta.Nombre);
                }
            }

            return listaComunidades;
        }

        /// <summary>
        /// Obtiene los proyectos administrados por el perfil pasado por par?metro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosAdministradosPorPerfilID(Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Proyecto> listaProyectos = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.ProyectoUsuarioIdentidad, objeto => new { objeto.Identidad.IdentidadID, objeto.Identidad.ProyectoID }, proyUserIden => new { proyUserIden.IdentidadID, proyUserIden.ProyectoID }, (objeto, proyUserIden) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                ProyectoUsuarioIdentidad = proyUserIden
            }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.ProyectoID, objeto.ProyectoUsuarioIdentidad.UsuarioID }, adminProy => new { adminProy.ProyectoID, adminProy.UsuarioID }, (objeto, adminProy) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(objeto => objeto.Proyecto).ToList();

            dataWrapperProyecto.ListaProyecto = listaProyectos;

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Indica si el usuario ers el ?nico administrador de un proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>True si es el ?nico administrador de algun proyecto</returns>
        public bool EsUsuarioAdministradorUnicoDeProyecto(Guid pUsuarioID)
        {
            return EsUsuarioAdministradorUnicoDeProyecto(pUsuarioID, Guid.Empty);
        }

        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>True si es el ?nico administrador del proyecto</returns>
        public bool EsUsuarioAdministradorUnicoDeProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            List<Guid> listaGuidAdmin = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(pUsuarioID) && adminProy.Tipo.Equals((short)TipoRolUsuario.Administrador)).Select(adminProy => adminProy.ProyectoID).ToList();

            var listaAdministradoresProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => listaGuidAdmin.Contains(adminProy.ProyectoID)).GroupBy(adminProy => adminProy.ProyectoID, adminProy => adminProy.ProyectoID, (agrupacion, g) => new
            {
                ProyectoID = agrupacion,
                NumAdmin = g.ToList().Count
            }).Select(adminProy => new
            {
                adminProy.NumAdmin,
                adminProy.ProyectoID
            }).ToList();


            if (listaAdministradoresProyecto != null && listaAdministradoresProyecto.Count == 1)
            {
                return listaAdministradoresProyecto[0].NumAdmin == 1;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene los proyectos administrados por la organizacion pasado por par?metro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosAdministradosPorOrganizacionID(Guid pOrganizacionID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Proyecto> listaProyectosConsulta = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.ProyectoUsuarioIdentidad, objeto => new { objeto.Identidad.IdentidadID, objeto.Identidad.ProyectoID }, proyUsuarioIden => new { proyUsuarioIden.IdentidadID, proyUsuarioIden.ProyectoID }, (objeto, proyUsuarioIden) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                ProyectoUsuarioIdentidad = proyUsuarioIden
            }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.ProyectoID, objeto.ProyectoUsuarioIdentidad.UsuarioID }, adminProy => new { adminProy.ProyectoID, adminProy.UsuarioID }, (objeto, adminProy) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).Select(objeto => objeto.Proyecto).Distinct().ToList();

            dataWrapperProyecto.ListaProyecto = listaProyectosConsulta;

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene el proyecto cuyo identificador se pasa por par?metro, adem?s de sus niveles de certificaci?n 
        /// y los permisos de los roles de usuario sobre los tipos de recursos del proyecto
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public DataWrapperProyecto ObtenerProyectoPorIDConNiveles(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            //Proyecto
            List<Proyecto> listaProyectosConsulta = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            //NivelCertificacion
            List<NivelCertificacion> listaNivelCertificacion = mEntityContext.NivelCertificacion.Where(nivelCertificacion => nivelCertificacion.ProyectoID.Equals(pProyectoID)).ToList();

            //TipoDocDispRolUsuarioProy
            List<TipoDocDispRolUsuarioProy> listaTipoDocDispRolUsuarioProy = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipoDoc => tipoDoc.ProyectoID.Equals(pProyectoID)).ToList();

            //TipoOntoDispRolUsuarioProy
            List<TipoOntoDispRolUsuarioProy> listaTipoOntoDispRolUsuarioProy = mEntityContext.TipoOntoDispRolUsuarioProy.Where(tipoOnto => tipoOnto.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaProyecto = listaProyectosConsulta;
            dataWrapperProyecto.ListaNivelCertificacion = listaNivelCertificacion;
            dataWrapperProyecto.ListaTipoDocDispRolUsuarioProy = listaTipoDocDispRolUsuarioProy;
            dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy = listaTipoOntoDispRolUsuarioProy;
            return dataWrapperProyecto;
        }



        /// <summary>
        /// Obtiene los grupos que tienen permisos sobre una ontolog?a en un determinado proyecto
        /// </summary>
        /// <param name="pListaOntologiasID">Lista de identificadores de ontolog?a</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobaci?n</param>
        /// <returns>Diccionario con los grupos de comunidad y organizaci?n que tienen permiso sobre la ontolog?a</returns>
        public Dictionary<Guid, List<Guid>> ObtenerGruposPermitidosOntologiasEnProyecto(List<Guid> pListaOntologiasID, Guid pProyectoID)
        {
            Dictionary<Guid, List<Guid>> dicGruposOntologias = new Dictionary<Guid, List<Guid>>();

            if (pListaOntologiasID.Count > 0)
            {
                var objetoConsulta = mEntityContext.DocumentoRolGrupoIdentidades.Where(documento => pListaOntologiasID.Contains(documento.DocumentoID)).ToList();

                foreach (DocumentoRolGrupoIdentidades fila in objetoConsulta)
                {
                    if (!dicGruposOntologias.ContainsKey(fila.DocumentoID))
                    {
                        dicGruposOntologias.Add(fila.DocumentoID, new List<Guid>());
                    }

                    if (!dicGruposOntologias[fila.DocumentoID].Contains(fila.GrupoID))
                    {
                        dicGruposOntologias[fila.DocumentoID].Add(fila.GrupoID);
                    }
                }
            }

            return dicGruposOntologias;
        }

        /// <summary>
        /// Obtiene las ontolog?as permitidas para un rol de usuario en un determinado proyecto
        /// </summary>
        /// <param name="pIdentidadEnProyID">Identificador de la identidad del usuario en el proyecto</param>
        /// <param name="pIdentidadEnMyGnossID">Identificador de la identidad del usuario en mygnoss</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobaci?n</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <param name="pIdentidadDeOtroProyecto">Verdad si la identidad pertenece a otro proyecto distinto a pProyectoID</param>
        /// <param name="pOntologiasEcosistema">Ontolog?as del ecosistema. Si no se pasan, se obtienen de base de datos</param>
        /// <returns>Lista con las ontolog?as permitidas para la identidad</returns>
        public List<Guid> ObtenerOntologiasPermitidasIdentidadEnProyecto(Guid pIdentidadEnProyID, Guid pIdentidadEnMyGnossID, Guid pProyectoID, TipoRolUsuario pTipoRol, bool pIdentidadDeOtroProyecto, Dictionary<Guid, Guid> pOntologiasEcosistema = null, Guid? pDocumentoID = null)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            HashSet<Guid> listaOntologiasDisponibles = new HashSet<Guid>();
            bool primera = true;

            //TipoOntoDispRolUsuarioProy
            List<TipoOntoDispRolUsuarioProy> listaTipoOntoDispRolUsuarioProy = mEntityContext.TipoOntoDispRolUsuarioProy.Where(tipoOnto => tipoOnto.ProyectoID.Equals(pProyectoID) && tipoOnto.RolUsuario >= (short)pTipoRol).ToList();
            dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy = listaTipoOntoDispRolUsuarioProy;
            if (pOntologiasEcosistema == null)
            {
                pOntologiasEcosistema = ObtenerOntologiasEcosistema();
            }

            if (pOntologiasEcosistema != null && pOntologiasEcosistema.Count > 0)
            {
                foreach (Guid ontologiaID in pOntologiasEcosistema.Keys)
                {
                    if (!pOntologiasEcosistema[ontologiaID].Equals(pProyectoID))
                    {
                        if (primera)
                        {
                            listaTipoOntoDispRolUsuarioProy = mEntityContext.TipoOntoDispRolUsuarioProy.Where(tipoOnto => tipoOnto.ProyectoID.Equals(pOntologiasEcosistema[ontologiaID]) && tipoOnto.OntologiaID.Equals(ontologiaID) && tipoOnto.RolUsuario >= (short)pTipoRol).ToList();
                            primera = false;
                        }
                        else
                        {
                            listaTipoOntoDispRolUsuarioProy = listaTipoOntoDispRolUsuarioProy.Union(mEntityContext.TipoOntoDispRolUsuarioProy.Where(tipoOnto => tipoOnto.ProyectoID.Equals(pOntologiasEcosistema[ontologiaID]) && tipoOnto.OntologiaID.Equals(ontologiaID) && tipoOnto.RolUsuario >= (short)pTipoRol)).ToList();
                        }
                    }
                }

                dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy = dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.Union(listaTipoOntoDispRolUsuarioProy).ToList();
            }

            //rellenar lista
            foreach (TipoOntoDispRolUsuarioProy fila in dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy)
            {
                listaOntologiasDisponibles.Add(fila.OntologiaID);
            }
            
            StringBuilder sb = new StringBuilder();
            string whereDocumento = "";
            List<Guid> listaGuids;
            if (pDocumentoID.HasValue)
            {
                List<Guid> listaDocumentosTotalesDisponibles = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.Tipo == 7 && !item.Documento.Eliminado && item.Documento.DocumentoID.Equals(pDocumentoID.Value)).Select(item => item.Documento.DocumentoID).ToList();

                listaGuids = mEntityContext.DocumentoRolGrupoIdentidades.Join(mEntityContext.GrupoIdentidadesParticipacion, documentoRolGrupoIdentidades => documentoRolGrupoIdentidades.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (documentoRolGrupoIdentidades, grupoIdentidadesParticipacion) => new
                {
                    DocumentoRolGrupoIdentidades = documentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                }).Where(item => (item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnProyID) || item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnMyGnossID)) && listaDocumentosTotalesDisponibles.Contains(item.DocumentoRolGrupoIdentidades.DocumentoID)).Select(item => item.DocumentoRolGrupoIdentidades.DocumentoID).ToList();
            }
            else
            {
                List<Guid> listaDocumentosTotalesDisponibles = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Documento.Tipo == 7 && !item.Documento.Eliminado).Select(item => item.Documento.DocumentoID).ToList();

                listaGuids = mEntityContext.DocumentoRolGrupoIdentidades.Join(mEntityContext.GrupoIdentidadesParticipacion, documentoRolGrupoIdentidades => documentoRolGrupoIdentidades.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (documentoRolGrupoIdentidades, grupoIdentidadesParticipacion) => new
                {
                    DocumentoRolGrupoIdentidades = documentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                }).Where(item => (item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnProyID) || item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnMyGnossID)) && listaDocumentosTotalesDisponibles.Contains(item.DocumentoRolGrupoIdentidades.DocumentoID)).Select(item => item.DocumentoRolGrupoIdentidades.DocumentoID).ToList();
            }

            if (pIdentidadDeOtroProyecto)
            {
                // La identidad es de otro proyecto (proviene del parametro ProyectoIDPatronOntologias)
                // Hago join a partir del nombrecorto de los grupos en los que participa el usuario en la comunidad de origen
                sb.AppendLine("UNION ALL");
                sb.AppendLine("select Documento.DocumentoID FROM Documento");
                sb.AppendLine("inner join DocumentoRolGrupoIdentidades on Documento.DocumentoID = DocumentoRolGrupoIdentidades.DocumentoID");
                sb.AppendLine("inner join GrupoIdentidades GrupoIdentidadesPatron on GrupoIdentidadesPatron.GrupoID = DocumentoRolGrupoIdentidades.GrupoID");
                sb.AppendLine("inner join GrupoidentidadesProyecto GrupoidentidadesProyectoPatron on GrupoIdentidadesPatron.Grupoid = GrupoidentidadesProyectoPatron.grupoid");
                sb.AppendLine("inner join GrupoIdentidades GurpoIdentidadesHija on GurpoIdentidadesHija.nombrecorto = GrupoIdentidadesPatron.nombrecorto");
                sb.AppendLine("inner join GrupoIdentidadesParticipacion on GrupoIdentidadesParticipacion.GrupoID = GurpoIdentidadesHija.GrupoID");
                sb.AppendLine($"where Documento.ProyectoID = {IBD.GuidValor(pProyectoID)}");
                sb.AppendLine($"  {whereDocumento} and GrupoIdentidadesParticipacion.IdentidadID = {IBD.GuidValor(pIdentidadEnProyID)}");

                if (pDocumentoID.HasValue)
                {
                    var primeraConsulta = mEntityContext.Documento.Join(mEntityContext.DocumentoRolGrupoIdentidades, documento => documento.DocumentoID, documentoRolIdentidad => documentoRolIdentidad.DocumentoID, (documento, documentoRolGrupoIdentidades) => new
                    {
                        Documento = documento,
                        DocumentoRolGrupoIdentidades = documentoRolGrupoIdentidades
                    }).Join(mEntityContext.GrupoIdentidades, item => item.DocumentoRolGrupoIdentidades.GrupoID, grupoIdentidades => grupoIdentidades.GrupoID, (item, grupoIdentidades) => new
                    {
                        Documento = item.Documento,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidades = grupoIdentidades
                    }).Join(mEntityContext.GrupoIdentidadesProyecto, item => item.GrupoIdentidades.GrupoID, grupoIdentidadesProyecto => grupoIdentidadesProyecto.GrupoID, (item, grupoIdentidadesProyecto) => new
                    {
                        Documento = item.Documento,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidades = item.GrupoIdentidades,
                        GrupoIdentidadesProyecto = grupoIdentidadesProyecto
                    }).Join(mEntityContext.GrupoIdentidades, item => item.GrupoIdentidades.NombreCorto, grupoIdentidadesHija => grupoIdentidadesHija.NombreCorto, (item, grupoIdentidadesHija) => new
                    {
                        Documento = item.Documento,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidades = item.GrupoIdentidades,
                        GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                        GrupoIdentidadesHija = grupoIdentidadesHija
                    }).Join(mEntityContext.GrupoIdentidadesParticipacion, item => item.GrupoIdentidadesHija.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (item, grupoIdentidadesParticipacion) => new
                    {
                        Documento = item.Documento,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidades = item.GrupoIdentidades,
                        GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                        GrupoIdentidadesHija = item.GrupoIdentidadesHija,
                        GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                    }).Where(item => item.Documento.ProyectoID.HasValue && item.Documento.ProyectoID.Value.Equals(pProyectoID) && item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnProyID) && item.Documento.DocumentoID.Equals(pDocumentoID.Value)).Select(item => item.Documento.DocumentoID);

                    listaGuids = listaGuids.Union(primeraConsulta).ToList();

                }
                else
                {
                    listaGuids = listaGuids.Union(mEntityContext.Documento.Join(mEntityContext.DocumentoRolGrupoIdentidades, documento => documento.DocumentoID, docRolGrupoIden => docRolGrupoIden.DocumentoID, (documento, docRolGrupoIden) => new
                    {
                        Documento = documento,
                        DocumentoRolGrupoIdentidades = docRolGrupoIden
                    }).Join(mEntityContext.GrupoIdentidades, objeto => objeto.DocumentoRolGrupoIdentidades.GrupoID, grupoIdentidadesPatron => grupoIdentidadesPatron.GrupoID, (objeto, grupoIdentidadesPatron) => new
                    {
                        Documento = objeto.Documento,
                        DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesPatron = grupoIdentidadesPatron
                    }).Join(mEntityContext.GrupoIdentidadesProyecto, objeto => objeto.GrupoIdentidadesPatron.GrupoID, grupoidentidadesProyectoPatron => grupoidentidadesProyectoPatron.GrupoID, (objeto, grupoidentidadesProyectoPatron) => new
                    {
                        Documento = objeto.Documento,
                        DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesPatron = objeto.GrupoIdentidadesPatron,
                        GrupoidentidadesProyectoPatron = grupoidentidadesProyectoPatron
                    }).Join(mEntityContext.GrupoIdentidades, objeto => objeto.GrupoIdentidadesPatron.NombreCorto, grupoIdentidadesHija => grupoIdentidadesHija.NombreCorto, (objeto, grupoIdentidadesHija) => new
                    {
                        Documento = objeto.Documento,
                        DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesPatron = objeto.GrupoIdentidadesPatron,
                        GrupoidentidadesProyectoPatron = objeto.GrupoidentidadesProyectoPatron,
                        GrupoIdentidadesHija = grupoIdentidadesHija
                    }).Join(mEntityContext.GrupoIdentidadesParticipacion, objeto => objeto.GrupoIdentidadesHija.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (objeto, grupoIdentidadesParticipacion) => new
                    {
                        Documento = objeto.Documento,
                        DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesPatron = objeto.GrupoIdentidadesPatron,
                        GrupoidentidadesProyectoPatron = objeto.GrupoidentidadesProyectoPatron,
                        GrupoIdentidadesHija = objeto.GrupoIdentidadesHija,
                        GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                    }).Where(objeto => objeto.Documento.ProyectoID.Value.Equals(pProyectoID) && objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnProyID)).Select(objeto => objeto.Documento.DocumentoID)
                    ).ToList();
                }
            }
            
            foreach (Guid id in listaGuids)
            {
                listaOntologiasDisponibles.Add(id);
            }

            return listaOntologiasDisponibles.ToList();
        }

        /// <summary>
        /// Obtiene las ontolog?as del ecosistema
        /// </summary>
        /// <returns>Lista con los DocumentoID de todas las ontolog?as con su ProyectoID como valor del diccionario</returns>
        public Dictionary<Guid, Guid> ObtenerOntologiasEcosistema()
        {
            Dictionary<Guid, Guid> listaOntologias = new Dictionary<Guid, Guid>();
            
            //Obtengo las ontolog?as disponibles:
            var resultadoConsulta = mEntityContext.Documento.Where(documento => documento.Visibilidad == 1 && documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && documento.Eliminado.Equals(false)).Select(documento => new { documento.DocumentoID, documento.ProyectoID }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid documentoID = fila.DocumentoID;
                Guid proyectoID = fila.ProyectoID.Value;

                listaOntologias.Add(documentoID, proyectoID);
            }

            return listaOntologias;
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID)
        {
            List<int> l;
            return ObtenerListaProyectoRelacionados(pProyectoID, out l);
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <param name="pManual">Indica si los proyectos son manuales o automaticos</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID, out bool pManual)
        {
            List<int> l;
            return ObtenerListaProyectoRelacionados(pProyectoID, out l, out pManual);
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <param name="pManual">Indica si los proyectos son manuales o automaticos</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID, out List<int> listaIdNumerico)
        {
            bool manual = true;
            return ObtenerListaProyectoRelacionados(pProyectoID, out listaIdNumerico, out manual);
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <param name="pManual">Indica si los proyectos son manuales o automaticos</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID, out List<int> listaIdNumerico, out bool pManual)
        {
            pManual = true;

            listaIdNumerico = new List<int>();
            List<Guid> listaProyectosRelacionados = new List<Guid>();
            DataWrapperProyecto proyectoDS = new DataWrapperProyecto();

            var listaProyectoTablaRelacion = mEntityContext.Proyecto.Join(mEntityContext.ProyectoRelacionado, proy => new { proy.ProyectoID, proy.OrganizacionID }, proyRelacionado => new { proyRelacionado.ProyectoID, proyRelacionado.OrganizacionID }, (proy, proyRelacionado) => new
            {
                ProyectoRelacionadoID = proyRelacionado.ProyectoRelacionadoID,
                ProyectoID = proy.ProyectoID,
                Tipo = 0,
                Orden = proyRelacionado.Orden,
                TablaBaseProyectoId = proy.TablaBaseProyectoID
            }).Where(proyectosTablaRelacion => proyectosTablaRelacion.ProyectoID.Equals(pProyectoID));

            var listaGuidsProyectoSuperiorID = mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.ProyectoSuperiorID.HasValue).Select(proyect => proyect.ProyectoSuperiorID.Value);

            var resultadoProyectoPadre = mEntityContext.Proyecto.Where(proy => listaGuidsProyectoSuperiorID.Contains(proy.ProyectoID) && !proy.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && !proy.Estado.Equals((short)EstadoProyecto.Cerrado) && !proy.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente) && !proy.Estado.Equals((short)EstadoProyecto.Definicion) && (proy.TipoAcceso.Equals((short)TipoAcceso.Publico) || proy.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proy => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = 2,
                Orden = 0,
                TablaBaseProyectoID = proy.TablaBaseProyectoID
            });
            
            var listaProyectosHijos = mEntityContext.Proyecto.Where(proy => proy.ProyectoSuperiorID.HasValue && proy.ProyectoSuperiorID.Value.Equals(pProyectoID) && !proy.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && !proy.Estado.Equals((short)EstadoProyecto.Cerrado) && !proy.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente) && !proy.Estado.Equals((short)EstadoProyecto.Definicion) && (proy.TipoAcceso.Equals((short)TipoAcceso.Publico) || proy.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proy => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = 2,
                Orden = 0,
                TablaBaseProyectoID = proy.TablaBaseProyectoID
            });
        
            var listaProyectosHermanos = mEntityContext.Proyecto.Where(proy => listaGuidsProyectoSuperiorID.Contains(proy.ProyectoID) && !proy.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && !proy.Estado.Equals((short)EstadoProyecto.Cerrado) && !proy.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente) && !proy.Estado.Equals((short)EstadoProyecto.Definicion) && (proy.TipoAcceso.Equals((short)TipoAcceso.Publico) || proy.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proy => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = 3,
                Orden = 0,
                TablaBaseProyectoID = proy.TablaBaseProyectoID
            });

            var listaGuidProyectoAgCategoriaTesauro = mEntityContext.Proyecto.Join(mEntityContext.ProyectoAgCatTesauro, proy => proy.ProyectoID, proyAgCatTes => proyAgCatTes.ProyectoID, (proy, ProyectoAgCatTesauro) => new
            {
                CategoriaTesauroID = ProyectoAgCatTesauro.CategoriaTesauroID,
                ProyectoID = proy.ProyectoID
            }).Where(proyCatTes => proyCatTes.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.CategoriaTesauroID);

            var listaProyectosRelacionadosRes = mEntityContext.Proyecto.Join(mEntityContext.ProyectoAgCatTesauro, proy => proy.ProyectoID, proyAgCatTesauro => proyAgCatTesauro.ProyectoID, (proy, proyAgCatTesauro) => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = 4,
                Orden = 0,
                CategoriaTesauroID = proyAgCatTesauro.CategoriaTesauroID,
                TablaBaseProyectoID = proy.TablaBaseProyectoID,
                TipoProyecto = proy.TipoProyecto,
                Estado = proy.Estado,
                TipoAcceso = proy.TipoAcceso
            }).Where(proyRel => listaGuidProyectoAgCategoriaTesauro.Contains(proyRel.CategoriaTesauroID) && !proyRel.ProyectoID.Equals(ProyectoAD.ProyectoFAQ) && !proyRel.ProyectoID.Equals(ProyectoAD.ProyectoNoticias) && !proyRel.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && !proyRel.Estado.Equals((short)EstadoProyecto.Cerrado) && !proyRel.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente) && !proyRel.Estado.Equals((short)EstadoProyecto.Definicion) && (proyRel.TipoAcceso.Equals((short)TipoAcceso.Publico) || proyRel.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proyRel => proyRel);
          
            var resManual = listaProyectoTablaRelacion.Join(mEntityContext.ProyectosMasActivos, tablaProyectos => new { ProyectoID = tablaProyectos.ProyectoRelacionadoID }, masActivos => new { ProyectoID = masActivos.ProyectoID }, (tablaProyectos, masActivos) => new
            {
                ProyectoID = tablaProyectos.ProyectoRelacionadoID,
                Tipo = tablaProyectos.Tipo,
                Orden = tablaProyectos.Orden,
                Peso = masActivos.Peso,
                TablaBaseProyectoID = tablaProyectos.TablaBaseProyectoId
            }).OrderByDescending(tablaProyectosMasActivos => tablaProyectosMasActivos.Orden).ThenByDescending(tablaProyectosMasActivos => tablaProyectosMasActivos.Peso);

            var listaProyectosRelacionadosResForUnion = listaProyectosRelacionadosRes.Select(proy => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = proy.Tipo,
                Orden = proy.Orden,
                TablaBaseProyectoID = proy.TablaBaseProyectoID
            });
            var ProyHijosHermanosRelUnionAll = resultadoProyectoPadre.Concat(listaProyectosHijos).Concat(listaProyectosHermanos).Concat(listaProyectosRelacionadosResForUnion);
            var auto = ProyHijosHermanosRelUnionAll.Join(mEntityContext.ProyectosMasActivos, tablaProyectos => tablaProyectos.ProyectoID, masActivos => masActivos.ProyectoID, (tablaProyectos, masActivos) => new
            {
                ProyectoId = tablaProyectos.ProyectoID,
                Tipo = tablaProyectos.Tipo,
                Orden = tablaProyectos.Orden,
                Peso = masActivos.Peso,
                TablaBaseProyectoID = tablaProyectos.TablaBaseProyectoID
            }).OrderByDescending(resultado => resultado.Tipo).ThenByDescending(resultado => resultado.Orden).ThenByDescending(resultado => resultado.Peso);


            foreach (var proyect in resManual.ToList())
            {
                Proyecto proyecto = new Proyecto();
                proyecto.ProyectoID = proyect.ProyectoID;
                proyecto.TablaBaseProyectoID = proyect.TablaBaseProyectoID;
                proyectoDS.ListaProyecto.Add(proyecto);
            }

            foreach (Proyecto filaProyecto in proyectoDS.ListaProyecto)
            {
                if (!listaProyectosRelacionados.Contains(filaProyecto.ProyectoID) && filaProyecto.ProyectoID != pProyectoID)
                {
                    listaIdNumerico.Add(filaProyecto.TablaBaseProyectoID);
                    listaProyectosRelacionados.Add(filaProyecto.ProyectoID);
                }
            }

            if (listaProyectosRelacionados.Count == 0)
            {
                pManual = false;

                foreach (var proyect in auto.ToList())
                {
                    Proyecto proyecto = new Proyecto();
                    proyecto.ProyectoID = proyect.ProyectoId;
                    proyecto.TablaBaseProyectoID = proyect.TablaBaseProyectoID;
                    proyectoDS.ListaProyecto.Add(proyecto);
                }

                foreach (Proyecto filaProyecto in proyectoDS.ListaProyecto)
                {
                    if (!listaProyectosRelacionados.Contains(filaProyecto.ProyectoID) && filaProyecto.ProyectoID != pProyectoID)
                    {
                        listaIdNumerico.Add(filaProyecto.TablaBaseProyectoID);
                        listaProyectosRelacionados.Add(filaProyecto.ProyectoID);
                    }
                }
            }

            return listaProyectosRelacionados;
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que administra el usuario
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <param name="pPerfilID">Clave del perfil</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectosAdministraUsuarioYPerfil(Guid pUsuarioID, Guid pPerfilID)
        {
            return mEntityContext.AdministradorProyecto.Join(mEntityContext.Identidad, adminProy => adminProy.ProyectoID, identidad => identidad.ProyectoID, (adminProy, identidad) => new
            {
                AdministradorProyecto = adminProy,
                Identidad = identidad
            }).Where(objeto => objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID) && objeto.Identidad.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.AdministradorProyecto.ProyectoID).ToList();
        }

        #endregion

        #region Administraci?n del tesauro

        /// <summary>
        /// Obtiene todos los documentos que est?n vinculados a un serie de categorias.
        /// </summary>
        /// <param name="pListaCategorias">Lista con las categorias a las que est?n agregados los documentos</param>
        /// <param name="pTesauroID">Identificador del tesauro al que pertenecen las categor?as</param>
        /// <returns>DataSet de documentaci?n con los documentos</returns>
        public DataWrapperProyecto ObtenerVinculacionProyectosDeCategoriasTesauro(List<Guid> pListaCategorias, Guid pTesauroID)
        {
            DataWrapperProyecto proyectoDS = new DataWrapperProyecto();

            if (pListaCategorias.Count > 0)
            {

                List<ProyectoAgCatTesauro> listaProyectoAgCatTesauro = mEntityContext.ProyectoAgCatTesauro.Where(proyAgCatTesauro => pListaCategorias.Contains(proyAgCatTesauro.CategoriaTesauroID) && proyAgCatTesauro.TesauroID.Equals(pTesauroID)).ToList();
                proyectoDS.ListaProyectoAgCatTesauro = listaProyectoAgCatTesauro;
            }
            return proyectoDS;
        }

        #endregion

        #region Proyectos hijos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerProyectosHijosDeProyecto(Guid pProyectoID)
        {
            var resultadoComando = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPesBus => proyPesBus.PestanyaID, proyPesMenu => proyPesMenu.PestanyaID, (proyPesBus, proyPesMenu) => new
            {
                ProyectoID = proyPesMenu.ProyectoID,
                proyPesBus.ProyectoOrigenID
            }).Where(proyJoin => proyJoin.ProyectoOrigenID.Equals(pProyectoID)).GroupBy(proyJoin => proyJoin.ProyectoID).Select(proyJoin => proyJoin.Key);
            List<Guid> listaHijos = new List<Guid>();

            foreach (var fila in resultadoComando.ToList())
            {
                listaHijos.Add(fila);
            }
            return listaHijos;
        }

        /// <summary>
        /// Obtiene los proyectos hijos del proyecto indicado. 
        /// </summary>
        /// <param name="pProyectoSuperiorID">Proyecto superior de los proyectos ids que quiero obtener</param>
        /// <returns>Todos los proyectos cuyo proyecto superior sea el dado por parámetro</returns>
        public List<Guid> ObtenerProyectosIdsDeProyectoSuperiorID(Guid pProyectoSuperiorID, bool pCargarProyectosCerrados)
        {
            if (pCargarProyectosCerrados)
            {
                return mEntityContext.Proyecto.Where(item => item.ProyectoSuperiorID.Equals(pProyectoSuperiorID)).Select(item => item.ProyectoID).ToList();
            }
            else
            {
                return mEntityContext.Proyecto.Where(item => item.ProyectoSuperiorID.Equals(pProyectoSuperiorID) && item.Estado != (short)EstadoProyecto.Cerrado && !item.FechaFin.HasValue).Select(item => item.ProyectoID).ToList();
            }
        }

        /// <summary>
        /// Obtiene el ID del proyecto origen del actual, si lo tiene o GUID.Empty si no.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>ID del proyecto origen del actual, si lo tiene o GUID.Empty si no</returns>
        public Guid ObtenerProyectoOrigenDeProyecto(Guid pProyectoID)
        {
             return mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPestBus => proyPestBus.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestBus, proyPestMenu) => new
            {
                ProyectoOrigenID = proyPestBus.ProyectoOrigenID,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proyJoin => proyJoin.ProyectoID.Equals(pProyectoID) && proyJoin.ProyectoOrigenID.HasValue).Select(proyJoin => proyJoin.ProyectoOrigenID.Value).FirstOrDefault();
        }

        #endregion

        /// <summary>
        /// Obtiene la URL de un servicio externo de un proyecto y en caso de no existir lo busca en el ecosistema
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNombre">Nomrbe del servicio</param>
        /// <returns></returns>
        public string ObtenerUrlServicioExterno(Guid pProyectoID, string pNombre)
        {
            var proyectoServicioExterno = mEntityContext.ProyectoServicioExterno.Where(proyServicioExterno => proyServicioExterno.ProyectoID.Equals(pProyectoID) && proyServicioExterno.NombreServicio.Equals(pNombre)).FirstOrDefault();
            string url = null;
            if (proyectoServicioExterno == null || string.IsNullOrEmpty(proyectoServicioExterno.UrlServicio))
            {
                var ecosistemaServicioExterno = mEntityContext.EcosistemaServicioExterno.Where(item => item.NombreServicio.Equals(pNombre)).FirstOrDefault();

                if (ecosistemaServicioExterno != null)
                {
                    url = ecosistemaServicioExterno.UrlServicio;
                }
            }
            else
            {
                url = proyectoServicioExterno.UrlServicio;
            }

            return url;
        }

        /// <summary>
        /// Obtiene el peso de una ontología para una pestaña concreta para poder crear el autocompletar con dicho peso
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto donde se encuentra la pesta�a</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pValorOnto">Ontología del proyecto</param>
        /// <param name="pPestanyaID">Identificador de la pestaña</param>
        /// <param name="pSubtipo">subtipo de la ontologia</param>
        /// <returns>peso del subtipo para ese OC</returns>
        public int ObtenerPesoPestanyaBusquedaOC(Guid pProyectoID, Guid pOrganizacionID, string pValorOnto, Guid pPestanyaID, string pSubtipo)
        {
            int peso = mEntityContext.ProyectoPestanyaBusquedaPesoOC.Where(item => item.ProyectoID.Equals(pProyectoID) && item.OrganizacionID.Equals(pOrganizacionID) && item.PestanyaID.Equals(pPestanyaID) && item.Tipo.Equals(pSubtipo)).Select(item => item.Peso).FirstOrDefault();          
            return peso;
        }

        public List<string> ObtenerNombresDeProyectoPorBusquedaAutocompletar(string pQuery)
        {
            List<string> resultados = mEntityContext.Proyecto.Where(fila => fila.Nombre.ToLower().Contains(pQuery.ToLower()) || fila.NombreCorto.ToLower().Contains(pQuery.ToLower())).Select(fila => fila.NombreCorto).ToList();

            return resultados;
		}

        public string ObtenerIdiomaPrincipalDominio(string pDominio)
        {
            string idioma = string.Empty;

            if (!string.IsNullOrEmpty(pDominio))
            {
                string url = mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia.Contains(pDominio + "@")).Select(proyecto => proyecto.URLPropia).FirstOrDefault();

                if (url != null && url.IndexOf(pDominio) >= 0)
                {
                    string urlAux = url;

                    urlAux = urlAux.Substring(urlAux.IndexOf(pDominio) + pDominio.Length + 1);

                    if (urlAux.Contains("|||"))
                    {
                        idioma = urlAux.Substring(0, urlAux.IndexOf("|||"));
                    }
                    else
                    {
                        idioma = urlAux;
                    }
                }
            }

            return idioma;
        }

        public ConfiguracionCachesCostosas ObtenerConfiguracionCachesCostosasDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.ConfiguracionCachesCostosas.Where(x => x.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        public bool EstanCachesDeBusquedasActivas(Guid pProyectoID)
        {
            return mEntityContext.ConfiguracionCachesCostosas.Where(x => x.ProyectoID.Equals(pProyectoID)).Select(config => config.CachesDeBusquedasActivas).FirstOrDefault();
        }

        public List<Rol> ObtenerRolesDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.Rol.Where(x => x.ProyectoID.Equals(pProyectoID) || x.ProyectoID.Equals(MetaProyecto) || x.Tipo.Equals(AmbitoRol.Ecosistema)).OrderBy(r => r.Nombre).ToList();
        }

        public List<RolEcosistema> ObtenerRolesAdministracionEcosistema()
        {
            return mEntityContext.RolEcosistema.ToList();
        }

        public List<RolEcosistema> ObtenerRolesAdministracionEcosistemaDeUsuario(Guid pUsuarioID)
        {
			return mEntityContext.RolEcosistema.Join(mEntityContext.RolEcosistemaUsuario, rol => rol.RolID, rolUsuario => rolUsuario.RolID, (rol, rolUsuario) => new
			{
				RolEcosistema = rol,
				RolEcosistemaUsuario = rolUsuario
			}).Where(objeto => objeto.RolEcosistemaUsuario.UsuarioID.Equals(pUsuarioID)).Select(objeto => objeto.RolEcosistema).ToList();
		}

        public List<Guid> ObtenerIdentidadesAdministradorasDeProyecto(Guid pProyectoID)
        {
			List<Guid> identidadesAdministradoras = mEntityContext.RolIdentidad.Join(mEntityContext.Identidad, rolIdentidad => rolIdentidad.IdentidadID, identidad => identidad.IdentidadID, (rolIdentidad, identidad) => new { identidad.IdentidadID, rolIdentidad.RolID, identidad.ProyectoID }).Where(x => x.RolID == ProyectoAD.RolAdministrador && x.ProyectoID == pProyectoID).Select(x => x.IdentidadID).ToList();
            return identidadesAdministradoras;
		}

        public List<Guid> ObtenerIdentidadesDeRol(Guid pRolID, Guid pProyectoID)
        {
			List<Guid> identidadesAdministradoras = mEntityContext.RolIdentidad.Join(mEntityContext.Identidad, rolIdentidad => rolIdentidad.IdentidadID, identidad => identidad.IdentidadID, (rolIdentidad, identidad) => new { identidad.IdentidadID, rolIdentidad.RolID, identidad.ProyectoID }).Where(x => x.RolID == pRolID && x.ProyectoID == pProyectoID).Select(x => x.IdentidadID).ToList();
			return identidadesAdministradoras;
		}

        public void GuardarRolProyecto(Rol pRol)
        {
            Rol rol = mEntityContext.Rol.Where(x => x.RolID.Equals(pRol.RolID)).FirstOrDefault();
            if (rol != null)
            {
                rol.Nombre = pRol.Nombre;
                rol.FechaModificacion = pRol.FechaModificacion;
                rol.Descripcion = pRol.Descripcion;
                rol.PermisosRecursos = pRol.PermisosRecursos;
                rol.PermisosAdministracion = pRol.PermisosAdministracion;
                rol.PermisosContenidos = pRol.PermisosContenidos;
				// si cambia el tipo, cambia el proyecto
				rol.Tipo = pRol.Tipo;
				rol.ProyectoID = pRol.ProyectoID;               
				rol.OrganizacionID = pRol.OrganizacionID;               
            }
            else
            {
                mEntityContext.Rol.Add(pRol);
            }

            mEntityContext.SaveChanges();
        }

        public void GuardarRolEcosistema(RolEcosistema pRol)
        {
			RolEcosistema rol = mEntityContext.RolEcosistema.Where(x => x.RolID.Equals(pRol.RolID)).FirstOrDefault();
			if (rol != null)
			{
				rol.Nombre = pRol.Nombre;
				rol.FechaModificacion = pRol.FechaModificacion;
				rol.Descripcion = pRol.Descripcion;
                rol.Permisos = pRol.Permisos;
                rol.FechaModificacion = pRol.FechaModificacion;
			}
			else
			{
				mEntityContext.RolEcosistema.Add(pRol);
			}

			mEntityContext.SaveChanges();
		}

        public void EliminarRolDeProyecto(Guid pRolID)
        {
            Rol rol = mEntityContext.Rol.Where(x => x.RolID.Equals(pRolID)).FirstOrDefault();
            if (rol != null)
            {
                mEntityContext.EliminarElemento(rol);
                mEntityContext.SaveChanges();
            }
		}

        public void EliminarRolDeAdministracionEcosistema(Guid pRolID)
        {
			RolEcosistema rol = mEntityContext.RolEcosistema.Where(x => x.RolID.Equals(pRolID)).FirstOrDefault();
			if (rol != null)
			{
				mEntityContext.EliminarElemento(rol);
				mEntityContext.SaveChanges();
			}
		}

        public List<Rol> ObtenerRolesDeGrupo(Guid pGrupoID)
        {
			return mEntityContext.Rol.Join(mEntityContext.RolGrupoIdentidades, rol => rol.RolID, rolGrupo => rolGrupo.RolID, (rol, rolGrupo) => new
			{
				Rol = rol,
				RolGrupoIdentidades = rolGrupo
			}).Where(objeto => objeto.RolGrupoIdentidades.GrupoID.Equals(pGrupoID)).Select(objeto => objeto.Rol).ToList();
		}

		public List<Rol> ObtenerRolesDeGrupos(List<Guid> pListaGrupos)
		{
            List<Rol> listaRoles = new List<Rol>();
            foreach (Guid grupoID in pListaGrupos)
            {
				listaRoles = listaRoles.Union(ObtenerRolesDeGrupo(grupoID)).ToList();
            }

			return listaRoles;
		}



		#endregion


        public void GuardarDetallesDocumentacion(DetallesDocumentacion detallesDocumentacion)
        {

            if(mEntityContext.DetallesDocumentacion.Where(x => x.ProyectoID.Equals(detallesDocumentacion.ProyectoID)).FirstOrDefault() == null)
            {
                mEntityContext.DetallesDocumentacion.Add(detallesDocumentacion);
            }
            else
            {
                DetallesDocumentacion existente = mEntityContext.DetallesDocumentacion.Find(detallesDocumentacion.ProyectoID);
                if (existente != null)
                {
                    mEntityContext.Entry(existente).CurrentValues.SetValues(detallesDocumentacion);
                    mEntityContext.SaveChanges();
                }


            }
        }

        public DetallesDocumentacion ObtenerDetallesDocumentacion(Guid proyectoID)
        {
            return mEntityContext.DetallesDocumentacion.Where(x => x.ProyectoID.Equals(proyectoID)).FirstOrDefault();
        }

        public Dictionary<String, String> ObtenerTitulosOntologias(Guid proyectoID)
        {
            return mEntityContext.Documento.Where(x => x.ProyectoID.Equals(proyectoID) && x.Tipo.Equals(7)).GroupBy(d => d.Titulo).ToDictionary(g => g.Key, g => g.First().Enlace);
        }

        /// <summary>
        /// Obtenemos el número de recursos por proyecto en un diccionario donde la clave 
        /// es el proyectoID y el valor el número de recursos que hay en el proyecto
        /// </summary>
        /// <returns>Un diccionario donde la clave es el identificador del proyecto y el valor es el número de recursos</returns>
        public Dictionary<Guid, int> ObtenerContadoresRecursoProyecto()
        {            
            return mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => !item.Documento.Eliminado && !item.Documento.Borrador && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.ProyectoID.HasValue).GroupBy(item => item.Documento.ProyectoID.Value).Select(group => new
            {
                ProyectoID = group.Key,
                NumeroRecursos = group.Count()
            }).ToDictionary(item => item.ProyectoID, item => item.NumeroRecursos);
        }

        /// <summary>
        /// Obtenemos el número de miembros por proyecto en un diccionario donde la clave 
        /// es el proyectoID y el valor el número de usuarios dados de alta en el proyecto
        /// </summary>
        /// <returns>Un diccionario donde la clave es el identificador del proyecto y el valor es el número de usuarios dados de alta en el proyecto</returns>
        public Dictionary<Guid, int> ObtenerContadoresMiembrosProyecto()
        {
            return mEntityContext.Identidad.Where(item => !item.FechaBaja.HasValue && !item.FechaExpulsion.HasValue).GroupBy(item => item.ProyectoID).Select(group => new
            {
                ProyectoID = group.Key,
                NumeroMiembros = group.Count()
            }).ToDictionary(item => item.ProyectoID, item => item.NumeroMiembros);
        }


        #region Privados

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD est?tico
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como par?metro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas
           
            //Consultas
            sqlSelectExisteProyectoFAQ = "SELECT 1 FROM Proyecto WHERE ProyectoID = " + IBD.GuidValor(ProyectoFAQ);

            sqlSelectExisteProyectoNoticias = "SELECT 1 FROM Proyecto WHERE ProyectoID = " + IBD.GuidValor(ProyectoNoticias);

            sqlSelectExisteProyectoDidactalia = "SELECT 1 FROM Proyecto WHERE ProyectoID = " + IBD.GuidValor(ProyectoDidactalia);

            sqlSelectEmailsMiembrosDeEventoDeProyecto = "SELECT ProyectoUsuarioIdentidad.IdentidadID,Perfil.PersonaID,Persona.Nombre,Persona.Apellidos, Persona.Email FROM proyectoUsuarioIdentidad INNER JOIN Identidad ON proyectoUsuarioIdentidad.IdentidadID = Identidad.identidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.PersonaID = Perfil.PersonaID INNER JOIN ProyectoeventoParticipante on ProyectoeventoParticipante.identidadid=Identidad.identidadid WHERE ProyectoeventoParticipante.EventoID = " + IBD.GuidParamValor("eventoID") + " AND Perfil.OrganizacionID IS NULL AND Persona.Email IS NOT NULL  UNION  Select proyectoUsuarioIdentidad.IdentidadID,Perfil.PersonaID,Persona.Nombre,Persona.Apellidos, PersonaVinculoOrganizacion.EmailTrabajo as Email FROM proyectoUsuarioIdentidad INNER JOIN Identidad ON proyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.personaID = Perfil.PersonaID INNER JOIN PersonaVinculoOrganizacion ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID AND PersonaVinculoOrganizacion.OrganizacionID = Perfil.OrganizacionID INNER JOIN ProyectoeventoParticipante on ProyectoeventoParticipante.identidadid=Identidad.identidadid WHERE ProyectoeventoParticipante.EventoID = " + IBD.GuidParamValor("eventoID") + " AND Perfil.PersonaID IS NOT NULL AND Perfil.OrganizacionID IS NOT NULL AND PersonaVinculoOrganizacion.EmailTrabajo IS NOT NULL ";

            sqlUpdateAumentarNumeroMiembrosDelProyecto = "UPDATE Proyecto SET NumeroMiembros = NumeroMiembros + 1 WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            sqlSelectTipoDocImagenPorDefecto = "SELECT " + IBD.CargarGuid("ProyectoID") + ", TipoRecurso, " + IBD.CargarGuid("OntologiaID") + ", UrlImagen FROM TipoDocImagenPorDefecto";

            #region Documentacion

            sqlSelectEventosProyectoPorIdentidadID = "SELECT " + IBD.CargarGuid("ProyectoEvento.EventoID") + ", ProyectoEvento.Nombre, ProyectoEvento.InfoExtra, ProyectoEventoParticipante.Fecha FROM ProyectoEvento inner join ProyectoEventoParticipante on ProyectoEventoParticipante.EventoID = ProyectoEvento.EventoID WHERE proyectoID = " + IBD.GuidParamValor("proyectoID") + " AND ProyectoEventoParticipante.IdentidadID = " + IBD.GuidParamValor("identidadID");

            #endregion

            #region Datos Twitter

            sqlUpdateTokenTwitterProyecto = IBD.ReplaceParam("UPDATE Proyecto SET TokenTwitter = @TokenTwitter, TokenSecretoTwitter = @TokenSecretoTwitter WHERE Proyecto.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " ");

            #endregion

            #endregion
        }

        public void CargarFacetaObjetoConocimientoProyectoPestanya(DataWrapperProyecto pProyectoDW, Guid pProyectoID, Guid? pOrganizacionID)
        {
            throw new NotImplementedException();
        }

        public ProyectoPestanyaBusqueda ObtenerProyectoPestanyasBusqueda(Guid pPestanyaID)
        {
            return mEntityContext.ProyectoPestanyaBusqueda.Where(item => item.PestanyaID.Equals(pPestanyaID)).FirstOrDefault();
        }

        public List<Guid> ObtenerPestanyasBusquedaProyectoSinAutocompletarFaceta(Guid pProyectoID, string pFaceta, string pObjetoConocimiento, Guid pPestanyaID)
        {
            List<Guid> listaPestanyasBusquedaSinAutocompletar = new List<Guid>();
            var consultaPestanyasSinAutocompletar = mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(pProyectoID) && item.PestanyaID.Equals(pPestanyaID)).Select(item => item.PestanyaID)
                .Except(mEntityContext.FacetaObjetoConocimientoProyectoPestanya.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Faceta.Equals(pFaceta) && item.ObjetoConocimiento.Equals(pObjetoConocimiento) && item.AutocompletarEnriquecido).Select(item => item.PestanyaID));
            listaPestanyasBusquedaSinAutocompletar = consultaPestanyasSinAutocompletar.ToList();
            return listaPestanyasBusquedaSinAutocompletar;
        }

		public Rol ObtenerRolUsuarioEnProyecto(Guid pProyectoID)
		{            
            return mEntityContext.Rol.FirstOrDefault(x => x.ProyectoID.Equals(pProyectoID) && x.EsRolUsuario);
		}
        public List<Guid> ObtenerPestanyasConAutocompletarEnriquecidoPorProyectoFacetaObjetoConocimiento(Guid pProyectoID, string pFaceta, string pObjetoConocimiento)
        {
            return mEntityContext.FacetaObjetoConocimientoProyectoPestanya.Where(item => item.AutocompletarEnriquecido && item.Faceta.Equals(pFaceta) && item.ObjetoConocimiento.Equals(pObjetoConocimiento)).Select(item => item.PestanyaID).Distinct().ToList();
        }


		#endregion

		#region Propiedades

		/// <summary>
		/// Obtiene el identificador de myGnoss (todo unos)
		/// </summary>
		public static Guid MyGnoss
        {
            get
            {
                return mMyGnoss;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la metaorganizaci?n
        /// </summary>
        public static Guid MetaOrganizacion
        {
            get
            {
                return mMetaOrganizacion;
            }
            set
            {
                mMetaOrganizacion = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del metaproyecto
        /// </summary>
        public static Guid MetaProyecto
        {
            get
            {
                return mMetaProyecto;
            }
            set
            {
                mMetaProyecto = value;
            }
        }

        public static Guid RolAdministrador
        {
            get
            {
                return new Guid("B56237B6-19D1-493D-8558-07E12B8A2B31");
			}
        }

        public static Guid RolAdministradorEcosistema
        {
            get
            {
				return new Guid("20896DFD-283B-465A-AFCE-D6C2AB96E6EC");
			}
        }

		/// <summary>
		/// Obtiene el identificador de la tabla base del metaproyecto
		/// </summary>
		public static int TablaBaseIdMetaProyecto
        {
            get
            {
                return mTablaBaseIdMetaProyecto;
            }
            set
            {
                mTablaBaseIdMetaProyecto = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto MetaGNOSS
        /// </summary>
        public static Guid MetaGNOSS
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111114");
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto FAQ
        /// </summary>
        public static Guid ProyectoFAQ
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111112");
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto Didactalia
        /// </summary>
        public static Guid ProyectoDidactalia
        {
            get
            {
                return new Guid("f22e757b-8116-4496-bec4-ae93a4792c28");
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto Noticias
        /// </summary>
        public static Guid ProyectoNoticias
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111113");
            }
        }

        /// <summary>
        /// Obtiene el color azul para los logos peque?os cuando seleccionen im?genes gen?ricas para el proyecto
        /// </summary>
        public static Color COLOR_AZUL
        {
            get
            {
                return Color.FromArgb(38, 153, 182);
            }
        }

        /// <summary>
        /// Obtiene el color lila para los logos peque?os cuando seleccionen im?genes gen?ricas para el proyecto
        /// </summary>
        public static Color COLOR_LILA
        {
            get
            {
                return Color.FromArgb(116, 51, 131);
            }
        }

        /// <summary>
        /// Obtiene el color marr?n para los logos peque?os cuando seleccionen im?genes gen?ricas para el proyecto
        /// </summary>
        public static Color COLOR_MARRON
        {
            get
            {
                return Color.FromArgb(196, 135, 54);
            }
        }

        /// <summary>
        /// Obtiene el color morado para los logos peque?os cuando seleccionen im?genes gen?ricas para el proyecto
        /// </summary>
        public static Color COLOR_MORADO
        {
            get
            {
                return Color.FromArgb(126, 135, 190);
            }
        }

        /// <summary>
        /// Obtiene el color rosa para los logos peque?os cuando seleccionen im?genes gen?ricas para el proyecto
        /// </summary>
        public static Color COLOR_ROSA
        {
            get
            {
                return Color.FromArgb(242, 120, 143);
            }
        }

        /// <summary>
        /// Obtiene el color salm?n para los logos peque?os cuando seleccionen im?genes gen?ricas para el proyecto
        /// </summary>
        public static Color COLOR_SALMON
        {
            get
            {
                return Color.FromArgb(236, 129, 77);
            }
        }

        #endregion


    }

    public class JoinProyectoOrganizacionParticipaProy
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
    }

    public class JoinProyectoAdministradorProyecto
    {
        public Proyecto Proyecto { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
    }

    public class JoinProyectoAdministradorProyectoPersona
    {
        public Proyecto Proyecto { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public AD.EntityModel.Models.PersonaDS.Persona Persona { get; set; }
    }

    public class JoinProyectoAdministradorProyectoPersonaPerfil
    {
        public Proyecto Proyecto { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public AD.EntityModel.Models.PersonaDS.Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinProyectoAdministradorProyectoPersonaPerfilIdentidad
    {
        public Proyecto Proyecto { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public AD.EntityModel.Models.PersonaDS.Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyOrganizacion
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Organizacion Organizacion { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyOrganizacionPais
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.Pais.Pais Pais { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfil
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.Pais.Pais Pais { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfilIdentidad
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.Pais.Pais Pais { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfil
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfilIdentidad
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }

        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad
        {
            get; set;
        }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfil
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }

        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad
        {
            get; set;
        }

        public Perfil Perfil2
        {
            get; set;
        }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersona
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }

        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad
        {
            get; set;
        }

        public Perfil Perfil2
        {
            get; set;
        }

        public AD.EntityModel.Models.PersonaDS.Persona Persona
        {
            get; set;
        }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersonaAdministradorProyecto
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }

        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad
        {
            get; set;
        }

        public Perfil Perfil2
        {
            get; set;
        }

        public AD.EntityModel.Models.PersonaDS.Persona Persona
        {
            get; set;
        }

        public AdministradorProyecto AdministradorProyecto
        {
            get; set;
        }
    }
}