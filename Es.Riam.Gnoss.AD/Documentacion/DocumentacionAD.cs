using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Comparadores;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using static Es.Riam.Gnoss.Util.Seguridad.Capacidad;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;


namespace Es.Riam.Gnoss.AD.Documentacion
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración que contiene los tipos de entidad que pueden estar relacionados con un documento
    /// </summary>
    public enum TipoEntidadVinculadaDocumento
    {
        /// <summary>
        /// Documento web
        /// </summary>
        Web = 0,
        /// <summary>
        /// Currículum
        /// </summary>
        Curriculum,
        /// <summary>
        /// Entidad temporal
        /// </summary>
        Temporal
    }

    /// <summary>
    /// Contiene los tipo de atributos bibliográficos y tipológicos.
    /// </summary>
    public enum TipoAtributosCampos
    {
        /// <summary>
        /// Texto
        /// </summary>
        Texto = 0,
        /// <summary>
        /// Fecha
        /// </summary>
        Fecha = 1,
        /// <summary>
        /// Numérico
        /// </summary>
        Numerico = 2,
        /// <summary>
        /// Título
        /// </summary>
        Titulo = 3,
        /// <summary>
        /// Autor
        /// </summary>
        Autor = 4,
        /// <summary>
        /// Apellidos
        /// </summary>
        Apellidos = 5,
        /// <summary>
        /// Año múltiple
        /// </summary>
        AnioMultiple = 6,
        /// <summary>
        /// Autor múltiple
        /// </summary>
        AutorMultiple = 7
    }

    /// <summary>
    /// Tipo de entradas posibles en el historial.
    /// </summary>
    public enum TipoEntradaHistorialDocumento
    {
        /// <summary>
        /// Tag
        /// </summary>
        Tag = 0,
        /// <summary>
        /// Categoría de tesauro
        /// </summary>
        CategoriaTesauro = 1,
        /// <summary>
        /// Comentario
        /// </summary>
        Comentario = 2
    }

    /// <summary>
    /// Acciones que se almacenan en el historial de documento.
    /// </summary>
    public enum AccionHistorialDocumento
    {
        /// <summary>
        /// Documento añadido
        /// </summary>
        Agregar = 0,
        /// <summary>
        /// Documento eliminado
        /// </summary>
        Eliminar = 1,
        /// <summary>
        /// Documento compartido
        /// </summary>
        CompartirDoc = 2,
        /// <summary>
        /// Nueva versión de documento
        /// </summary>
        CrearVersion = 3,
        /// <summary>
        /// Modificación de documento
        /// </summary>
        GuardarDocumento = 4,
        /// <summary>
        /// Documento descompartido
        /// </summary>
        DesCompartirDoc = 5,
        /// <summary>
        /// Versión de documento restaurada
        /// </summary>
        RestaurarVersion = 6,
        /// <summary>
        /// Certificación de documento
        /// </summary>
        CertificarDoc = 7,
        /// <summary>
        /// Quitar la certificación de documento
        /// </summary>
        EliminarCertificacionDoc = 8
    }

    /// <summary>
    /// Tipo de base de recursos
    /// </summary>
    public enum TipoBaseRecursos
    {
        /// <summary>
        /// Base de recursos de usuario
        /// </summary>
        BRUsuario = 0,
        /// <summary>
        /// Base de recursos de organización
        /// </summary>
        BROrganizacion = 1,
        /// <summary>
        /// Base de recursos de proyecto
        /// </summary>
        BRProyectos = 2,
        /// <summary>
        /// Base de recursos no definida
        /// </summary>
        BRNoDefinida = 3
    }

    /// <summary>
    /// Tipos de errores de concurrencia
    /// </summary>
    public enum ErroresConcurrencia
    {
        /// <summary>
        /// No hay error de concurrencia
        /// </summary>
        NoConcurrencia = 0,

        /// <summary>
        /// Error de concurrencia: Esta versión del documento la a modificado otra persona
        /// </summary>
        ConcurrenciaMismaVersion = 1,

        /// <summary>
        /// Error de concurrencia: Esta versión del documento está obsoleta porque se ha creado una nueva
        /// </summary>
        VersionObsoleta = 2
    }

    /// <summary>
    /// Estado en los que se puede encontrar un elemento de una cola.
    /// </summary>
    public enum EstadoElementoCola
    {
        /// <summary>
        /// El elemento todavía no ha sido tratado.
        /// </summary>
        Espera = 0,
        /// <summary>
        /// Se ha producido 1 error al intentar procesar el elemento.
        /// </summary>
        Error1 = 1,
        /// <summary>
        /// Se han producido 2 errores al intentar procesar el elemento.
        /// </summary>
        Error2 = 2,
        /// <summary>
        /// Se han producido 3 errores al intentar procesar el elemento.
        /// </summary>
        Error3 = 3,
        /// <summary>
        /// Se han producido 4 errores al intentar procesar el elemento.
        /// </summary>
        Error4 = 4,
        /// <summary>
        /// Se han producido 5 errores al intentar procesar el elemento y se dejará de procesar.
        /// </summary>
        Fallido = 5,
        /// <summary>
        /// Se ha prodesado el elemento satisfactoriamente.
        /// </summary>
        Procesado = 6
    }

    /// <summary>
    /// Tipos de resultado de un documento pregunta
    /// </summary>
    public enum TiposResultadosDocPregunta
    {
        /// <summary>
        /// Empty
        /// </summary>
        Empty = 0,
        /// <summary>
        /// Pregunta SoyCreador
        /// </summary>
        PreguntaSoyCreador,
        /// <summary>
        /// Dafo SoyParticipante
        /// </summary>
        PreguntaSoyParticipante,
        /// <summary>
        /// Pregunta SoyLector
        /// </summary>
        PreguntaSoyLector
    }

    /// <summary>
    /// Tipos de resultado de un documento pregunta
    /// </summary>
    public enum EstadoPregunta
    {
        /// <summary>
        /// Pregunta Sin Respuesta
        /// </summary>
        SinRespuesta = 0,
        /// <summary>
        /// Pregunta Abierta
        /// </summary>
        Abierta,
        /// <summary>
        /// Pregunta Contestada (cerrada)
        /// </summary>
        Contestada,
    }


    /// <summary>
    /// 
    /// </summary>
    public enum EstadoSubidaVideo
    {
        /// <summary>
        /// Solo se ha creado el token
        /// </summary>
        Espera = 0,
        /// <summary>
        /// Subiendo a GNOSS
        /// </summary>
        SubiendoGNOSS = 1,
        /// <summary>
        /// Subido a GNOSS
        /// </summary>
        SubidoaGNOSS = 2,
        /// <summary>
        /// Subiendo a la plataforma
        /// </summary>
        SubiendoPlataforma = 3,
        /// <summary>
        /// Finalizado
        /// </summary>
        Finalizado = 4,
        /// <summary>
        /// Fallido
        /// </summary>
        Fallido = 5,
        /// <summary>
        /// Finalizado y correcto
        /// </summary>
        FinalizadoYCorrecto = 6,
        /// <summary>
        /// Subida
        /// </summary>
        SubidoaGNOSSEditado = 7,
    }

    /// <summary>
    /// Priridad de las capturas
    /// </summary>
    public enum PrioridadColaDocumento
    {
        /// <summary>
        /// Prioridad alta
        /// </summary>
        Alta = 0,
        /// <summary>
        /// Prioridad media
        /// </summary>
        Media = 1,
        /// <summary>
        /// Prioridad baja
        /// </summary>
        Baja = 2
    }

    public enum VisibilidadDocumento
    {
        /// <summary>
        /// Visible para todos.
        /// </summary>
        Todos = 0,
        /// <summary>
        /// Visible solo para los miembros de la comunidad (en buscadores si que sale para el usuario invitado).
        /// </summary>
        MiembrosComunidad = 1,
        /// <summary>
        /// Visible solo para los miembros de la comunidad (no sale ni siquiera en los buscadores para el usuario invitado).
        /// </summary>
        PrivadoMiembrosComunidad = 2
    }

    public enum TipoPublicacion
    {
        Publicado = 0,
        Compartido = 1,
        CompartidoAutomatico = 2
    }

    public enum PropiedadesOntologia
    {
        cargasmultiples,
        urlservicio,
        urlservicioElim,
        xmlTroceado,
        urlserviciocomplementario,
        permitirUsuNoLogueado,
        urlserviciocomplementarioSincrono,
        enviarRdfAntiguo
    }

    #endregion

    public class JoinVotoVotoDocumento
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoDocumento VotoDocumento { get; set; }
    }

    public class JoinVotoVotoDocumentoDocumento
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoDocumento VotoDocumento { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoDocumentoEntidadGnoss
    {
        public Documento Documento { get; set; }
        public DocumentoEntidadGnoss DocumentoEntidadGnoss { get; set; }
    }

    public class JoinDocumentoAtributoBiblioDocumento
    {
        public DocumentoAtributoBiblio DocumentoAtributoBiblio { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoAtributoBiblioDocumentoDocumentoEntidadGnoss
    {
        public DocumentoAtributoBiblio DocumentoAtributoBiblio { get; set; }
        public Documento Documento { get; set; }
        public DocumentoEntidadGnoss DocumentoEntidadGnoss { get; set; }
    }

    public class JoinVersionDocumentoDocumento
    {
        public VersionDocumento VersionDocumento { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinVersionDocumentoDocumentoDocumentoEntidadGnoss
    {
        public VersionDocumento VersionDocumento { get; set; }
        public Documento Documento { get; set; }
        public DocumentoEntidadGnoss DocumentoEntidadGnoss { get; set; }
    }

    public class JoinDocumentoWebAgCatTesauroDocumento
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoWebAgCatTesauroDocumentoDocumentoEntidadGnoss
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public Documento Documento { get; set; }
        public DocumentoEntidadGnoss DocumentoEntidadGnoss { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumento
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumentoDocumentoEntidadGnoss
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public DocumentoEntidadGnoss DocumentoEntidadGnoss { get; set; }
    }

    public class JoinBaseRecursosOrganizacionDocumentoWebVinBaseRecursos
    {
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinBaseRecursosUsuarioDocumentoWebVinBaseRecursos
    {
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoNivelCertificacion
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public NivelCertificacion NivelCertificacion { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoProyecto
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.ProyectoDS.Proyecto Proyecto { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoIdentidad
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoIdentidadPerfil
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDocumentoEnvioNewsLetterDocumento
    {
        public DocumentoEnvioNewsLetter DocumentoEnvioNewsLetter { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoEnvioNewsLetterDocumentoDocumentoNewsLetter
    {
        public DocumentoEnvioNewsLetter DocumentoEnvioNewsLetter { get; set; }
        public Documento Documento { get; set; }
        public DocumentoNewsletter DocumentoNewsLetter { get; set; }
    }

    public class JoinDocumentoVinDocDocumento
    {
        public DocumentoVincDoc DocumentoVinDoc { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoVinDocDocumentoDocumentoWebVinBaseRecursos
    {
        public DocumentoVincDoc DocumentoVinDoc { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinDocumentoVinDocDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public DocumentoVincDoc DocumentoVinDoc { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoVinDocDocumentoDocumento
    {
        public DocumentoVincDoc DocumentoVinDoc { get; set; }
        public Documento Documento { get; set; }
        public Documento Documento2 { get; set; }
    }

    public class JoinDocumentoVinDocDocumentoDocumentoDocumentoWebVinBaseRecursos
    {
        public DocumentoVincDoc DocumentoVinDoc { get; set; }
        public Documento Documento { get; set; }
        public Documento Documento2 { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinDocumentoVinDocDocumentoDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public DocumentoVincDoc DocumentoVinDoc { get; set; }
        public Documento Documento { get; set; }
        public Documento Documento2 { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosNivelCertificacion
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public NivelCertificacion NivelCertificacion { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersona
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaDocumento
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public Persona Persona { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaDocumentoPerfil
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public Persona Persona { get; set; }
        public Documento Documento { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
    {
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinBaseRecursosUsuarioDocumentoWebVinBaseRecursosDocumento
    {
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinBaseRecursosOrganizacionDocumentoWebVinBaseRecursosDocumento
    {
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoWebAgCatTesauroCategoriaTesauro
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesaruo { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class JoinBaseRecursosBaseRecursosUsuario
    {
        public BaseRecursos BaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class JoinBaseRecursosBaseRecursosProyecto
    {
        public BaseRecursos BaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinBaseRecursosBaseRecursosOrganizacion
    {
        public BaseRecursos BaseRecursos { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauro
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscrip
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcion
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidad
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidadBaseRecursosProyecto
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidadBaseRecursosProyectoDocumento
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyecto
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcion
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcionIdentidad
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcionIdentidadDocumento
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoComentarioVersionDocumento
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public VersionDocumento VersionDocumento { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosDocumentoDocumentoBaseRecursosProyecto
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinComentarioDocumentoComentario
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
    }

    public class JoinComentarioDocumentoComentarioDocumento
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinComentarioDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinComentarioDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinVotoDocumentoVotoDocumento
    {
        public VotoDocumento VotoDocumento { get; set; }
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinVotoDocumentoVotoDocumentoDocumentoWebVinBaseRecursos
    {
        public VotoDocumento VotoDocumento { get; set; }
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinVotoDocumentoVotoDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public VotoDocumento VotoDocumento { get; set; }
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoIdentidad
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoIdentidadPerfil
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursos
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
    }

    public class JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursosBaseRecursosUsuario
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class JoinDocumentoIdentidadPerfilPersona
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinDocumentoIdentidadPerfilPersonaProyecto
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public EntityModel.Models.ProyectoDS.Proyecto Proyecto { get; set; }
    }

    //proyecto on proyecto.proyectoid = documento.proyectoid

    public static class Joins
    {
        public static IQueryable<JoinDocumentoIdentidadPerfilPersonaProyecto> JoinProyecto(this IQueryable<JoinDocumentoIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.Documento.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinDocumentoIdentidadPerfilPersonaProyecto
            {
                Proyecto = proyecto,
                Documento = item.Documento,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona
            });
        }

        public static IQueryable<JoinDocumentoIdentidadPerfilPersona> JoinPersona(this IQueryable<JoinDocumentoIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinDocumentoIdentidadPerfilPersona
            {
                Persona = persona,
                Documento = item.Documento,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinDocumentoIdentidadPerfil> JoinPerfil(this IQueryable<JoinDocumentoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinDocumentoIdentidadPerfil
            {
                Perfil = perfil,
                Documento = item.Documento,
                Identidad = item.Identidad
            });
        }


        public static IQueryable<JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinDocumentoIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.Documento.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebBinBaseRecursos) => new JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursos
            {
                Perfil = item.Perfil,
                Documento = item.Documento,
                Identidad = item.Identidad,
                DocumentoWebVinBaseRecursos = documentoWebBinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursosBaseRecursosOrganizacion> JoinBaseRecursosOrganizacion(this IQueryable<JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosOrganizacion, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosOrganizacion => baseRecursosOrganizacion.BaseRecursosID, (item, baseRecursosOrganizacion) => new JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
            {
                Perfil = item.Perfil,
                Documento = item.Documento,
                Identidad = item.Identidad,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosOrganizacion = baseRecursosOrganizacion
            });
        }
        public static IQueryable<JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursosBaseRecursosUsuario> JoinBaseRecursosUsuario(this IQueryable<JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosUsuario, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosUsuario => baseRecursosUsuario.BaseRecursosID, (item, baseRecursosUsuario) => new JoinDocumentoIdentidadPerfilDocumentoWebVinBaseRecursosBaseRecursosUsuario
            {
                Perfil = item.Perfil,
                Documento = item.Documento,
                Identidad = item.Identidad,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosUsuario = baseRecursosUsuario
            });
        }

        public static IQueryable<JoinDocumentoIdentidad> JoinIdentidad(this IQueryable<Documento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, documento => documento.CreadorID, identidad => identidad.IdentidadID, (documento, identidad) => new JoinDocumentoIdentidad
            {
                Identidad = identidad,
                Documento = documento
            });
        }

        public static IQueryable<JoinVotoDocumentoVotoDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinVotoDocumentoVotoDocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) => new JoinVotoDocumentoVotoDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Voto = item.Voto,
                VotoDocumento = item.VotoDocumento
            });
        }

        public static IQueryable<JoinVotoDocumentoVotoDocumentoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinVotoDocumentoVotoDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.Documento.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecuros) => new JoinVotoDocumentoVotoDocumentoDocumentoWebVinBaseRecursos
            {
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecuros,
                Documento = item.Documento,
                Voto = item.Voto,
                VotoDocumento = item.VotoDocumento
            });
        }

        public static IQueryable<JoinVotoDocumentoVotoDocumento> JoinDocumento(this IQueryable<JoinVotoDocumentoVoto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.VotoDocumento.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinVotoDocumentoVotoDocumento
            {
                Documento = documento,
                Voto = item.Voto,
                VotoDocumento = item.VotoDocumento
            });
        }

        public static IQueryable<JoinComentarioDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinComentarioDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) => new JoinComentarioDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                Comentario = item.Comentario,
                Documento = item.Documento,
                DocumentoComentario = item.DocumentoComentario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinComentarioDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinComentarioDocumentoComentarioDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.Documento.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecursos) => new JoinComentarioDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos
            {
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos,
                DocumentoComentario = item.DocumentoComentario,
                Comentario = item.Comentario,
                Documento = item.Documento
            });
        }

        public static IQueryable<JoinComentarioDocumentoComentarioDocumento> JoinDocumento(this IQueryable<JoinComentarioDocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoComentario.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinComentarioDocumentoComentarioDocumento
            {
                Documento = documento,
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario
            });
        }

        public static IQueryable<JoinComentarioDocumentoComentario> JoinDocumentoComentario(this IQueryable<EntityModel.Models.Comentario.Comentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoComentario, comentario => comentario.ComentarioID, documentoComentario => documentoComentario.ComentarioID, (comentario, documentoComentario) => new JoinComentarioDocumentoComentario
            {
                DocumentoComentario = documentoComentario,
                Comentario = comentario
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoDocumentoBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinDocumentoWebVinBaseRecursosDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) => new JoinDocumentoWebVinBaseRecursosDocumentoDocumentoBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Documento = item.Documento
            });
        }

        public static IQueryable<JoinDocumentoComentarioVersionDocumento> JoinVersionDocumento(this IQueryable<DocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.VersionDocumento, documentoComentario => documentoComentario.DocumentoID, versionDocumento => versionDocumento.DocumentoID, (documentoComentario, versionDocumento) => new JoinDocumentoComentarioVersionDocumento
            {
                VersionDocumento = versionDocumento,
                DocumentoComentario = documentoComentario
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcionIdentidadDocumento> JoinDocumento(this IQueryable<JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcionIdentidadDocumento
            {
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Suscripcion = item.Suscripcion,
                Identidad = item.Identidad,
                Documento = documento
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcionIdentidad
            {
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Suscripcion = item.Suscripcion,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcion> JoinSuscripcion(this IQueryable<JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, item => item.SuscripcionIdentidadProyecto.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (item, suscripcion) => new JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyectoSuscripcion
            {
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Suscripcion = suscripcion
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyecto> JoinSuscripcionIdentidadProyecto(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SuscripcionIdentidadProyecto, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.IdentidadPublicacionID, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.IdentidadID, (documentoWebVinBaseRecursos, suscripcionIdentidadProyecto) => new JoinDocumentoWebVinBaseRecursosSuscripcionIdentidadProyecto
            {
                SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidadBaseRecursosProyectoDocumento> JoinDocumento(this IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidadBaseRecursosProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidadBaseRecursosProyectoDocumento
            {
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Suscripcion = item.Suscripcion,
                Identidad = item.Identidad,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = documento
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidadBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => new { item.DocumentoWebVinBaseRecursos.BaseRecursosID, item.Identidad.ProyectoID }, baseRecursosProyecto => new { baseRecursosProyecto.BaseRecursosID, baseRecursosProyecto.ProyectoID }, (item, baseRecursosProyecto) => new JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidadBaseRecursosProyecto
            {
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Suscripcion = item.Suscripcion,
                Identidad = item.Identidad,
                BaseRecursosProyecto = baseRecursosProyecto
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcionIdentidad
            {
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Suscripcion = item.Suscripcion,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcion> JoinSuscripcion(this IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscrip> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, item => item.CategoriaTesVinSuscrip.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (item, suscripcion) => new JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscripSuscripcion
            {
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Suscripcion = suscripcion
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscrip> JoinCategoriaTesVinSuscrip(this IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesVinSuscrip, item => new { item.DocumentoWebAgCatTesauro.CategoriaTesauroID, item.DocumentoWebAgCatTesauro.TesauroID }, categoriaTesVinSuscrip => new { categoriaTesVinSuscrip.CategoriaTesauroID, categoriaTesVinSuscrip.TesauroID }, (item, categoriaTesVinSuscrip) => new JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauroCategoriaTesVinSuscrip
            {
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                CategoriaTesVinSuscrip = categoriaTesVinSuscrip
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauro> JoinDocumentoWebAgCatTesauro(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebAgCatTesauro, documentoWebVinBaseRecursos => new { documentoWebVinBaseRecursos.DocumentoID, documentoWebVinBaseRecursos.BaseRecursosID }, documentoWebAgCatTesauro => new { documentoWebAgCatTesauro.DocumentoID, documentoWebAgCatTesauro.BaseRecursosID }, (documentoWebVinBaseRecursos, documentoWebAgCatTesauro) => new JoinDocumentoWebVinBaseRecursosDocumentoWebAgCatTesauro
            {
                DocumentoWebAgCatTesauro = documentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinBaseRecursosBaseRecursosOrganizacion> JoinBaseRecursosOrganizacion(this IQueryable<BaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosOrganizacion, baseRecursos => baseRecursos.BaseRecursosID, baseRecursosOrganizacion => baseRecursosOrganizacion.BaseRecursosID, (baseRecursos, baseRecursosOrganizacion) => new JoinBaseRecursosBaseRecursosOrganizacion
            {
                BaseRecursosOrganizacion = baseRecursosOrganizacion,
                BaseRecursos = baseRecursos
            });
        }


        public static IQueryable<JoinBaseRecursosBaseRecursosUsuario> JoinBaseRecursosUsuario(this IQueryable<BaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosUsuario, baseRecursos => baseRecursos.BaseRecursosID, baseRecursosUsuario => baseRecursosUsuario.BaseRecursosID, (baseRecursos, baseRecursosUsuario) => new JoinBaseRecursosBaseRecursosUsuario
            {
                BaseRecursosUsuario = baseRecursosUsuario,
                BaseRecursos = baseRecursos
            });
        }

        public static IQueryable<JoinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<BaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, baseRecursos => baseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (baseRecursos, baseRecursosProyecto) => new JoinBaseRecursosBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                BaseRecursos = baseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebAgCatTesauroCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<DocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, documentoWebAgCatTesauro => documentoWebAgCatTesauro.CategoriaTesauroID, categoriaTesauro => categoriaTesauro.CategoriaTesauroID, (documentoWebAgCatTesauro, categoriaTesauro) => new JoinDocumentoWebAgCatTesauroCategoriaTesauro
            {
                CategoriaTesauro = categoriaTesauro,
                DocumentoWebAgCatTesaruo = documentoWebAgCatTesauro
            });
        }

        public static IQueryable<JoinBaseRecursosOrganizacionDocumentoWebVinBaseRecursosDocumento> JoinDocumento(this IQueryable<JoinBaseRecursosOrganizacionDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinBaseRecursosOrganizacionDocumentoWebVinBaseRecursosDocumento
            {
                Documento = documento,
                BaseRecursosOrganizacion = item.BaseRecursosOrganizacion,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinBaseRecursosUsuarioDocumentoWebVinBaseRecursosDocumento> JoinDocumento(this IQueryable<JoinBaseRecursosUsuarioDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinBaseRecursosUsuarioDocumentoWebVinBaseRecursosDocumento
            {
                Documento = documento,
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosOrganizacion> JoinBaseRecursosOrganizacion(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosOrganizacion, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.BaseRecursosID, baseRecursosOrganizacion => baseRecursosOrganizacion.BaseRecursosID, (documentoWebVinBaseRecursos, baseRecursosOrganizacion) => new JoinDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
            {
                BaseRecursosOrganizacion = baseRecursosOrganizacion,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario> JoinBaseRecursosUsuario(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosUsuario, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.BaseRecursosID, baseRecursosUsuario => baseRecursosUsuario.BaseRecursosID, (documentoWebVinBaseRecursos, baseRecursosUsuario) => new JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario
            {
                BaseRecursosUsuario = baseRecursosUsuario,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersona> JoinPersona(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.BaseRecursosUsuario.UsuarioID, persona => persona.UsuarioID, (item, persona) => new JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersona
            {
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Persona = persona
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaDocumento> JoinDocumento(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaDocumento
            {
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Persona = item.Persona,
                Documento = documento
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaDocumentoPerfil> JoinPerfil(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PerfilID, (item, perfil) => new JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaDocumentoPerfil
            {
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Persona = item.Persona,
                Documento = item.Documento,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosNivelCertificacion> JoinNivelCertificacion(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NivelCertificacion, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.NivelCertificacionID, nivelCertificacion => nivelCertificacion.NivelCertificacionID, (documentoWebVinBaseRecursos, nivelCertificacion) => new JoinDocumentoWebVinBaseRecursosNivelCertificacion
            {
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos,
                NivelCertificacion = nivelCertificacion
            });
        }

        public static IQueryable<JoinDocumentoVinDocDocumentoDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinDocumentoVinDocDocumentoDocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) => new JoinDocumentoVinDocDocumentoDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                DocumentoVinDoc = item.DocumentoVinDoc,
                Documento = item.Documento,
                Documento2 = item.Documento2,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursosProyecto
            });
        }

        public static IQueryable<JoinDocumentoVinDocDocumentoDocumentoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinDocumentoVinDocDocumentoDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.Documento2.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecursos) => new JoinDocumentoVinDocDocumentoDocumentoDocumentoWebVinBaseRecursos
            {
                DocumentoVinDoc = item.DocumentoVinDoc,
                Documento = item.Documento,
                Documento2 = item.Documento2,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoVinDocDocumentoDocumento> JoinDocumento(this IQueryable<JoinDocumentoVinDocDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoVinDoc.DocumentoVincID, documento2 => documento2.DocumentoID, (item, documento2) => new JoinDocumentoVinDocDocumentoDocumento
            {
                DocumentoVinDoc = item.DocumentoVinDoc,
                Documento = item.Documento,
                Documento2 = documento2
            });
        }

        public static IQueryable<JoinDocumentoVinDocDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinDocumentoVinDocDocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) => new JoinDocumentoVinDocDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                Documento = item.Documento,
                DocumentoVinDoc = item.DocumentoVinDoc,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoVinDocDocumentoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinDocumentoVinDocDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.Documento.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecursos) => new JoinDocumentoVinDocDocumentoDocumentoWebVinBaseRecursos
            {
                Documento = item.Documento,
                DocumentoVinDoc = item.DocumentoVinDoc,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoVinDocDocumento> JoinDocumento(this IQueryable<DocumentoVincDoc> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoVinDoc => documentoVinDoc.DocumentoID, documento => documento.DocumentoID, (documentoVinDoc, documento) => new JoinDocumentoVinDocDocumento
            {
                Documento = documento,
                DocumentoVinDoc = documentoVinDoc
            });
        }

        public static IQueryable<JoinDocumentoEnvioNewsLetterDocumentoDocumentoNewsLetter> JoinDocumentoNewsLetter(this IQueryable<JoinDocumentoEnvioNewsLetterDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoNewsletter, item => item.DocumentoEnvioNewsLetter.DocumentoID, documentoNewsLetter => documentoNewsLetter.DocumentoID, (item, documentoNewsLetter) => new JoinDocumentoEnvioNewsLetterDocumentoDocumentoNewsLetter
            {
                Documento = item.Documento,
                DocumentoEnvioNewsLetter = item.DocumentoEnvioNewsLetter,
                DocumentoNewsLetter = documentoNewsLetter
            });
        }

        public static IQueryable<JoinDocumentoEnvioNewsLetterDocumento> JoinDocumento(this IQueryable<DocumentoEnvioNewsLetter> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoEnvioNewsLetter => documentoEnvioNewsLetter.DocumentoID, documento => documento.DocumentoID, (documentoEnvioNewsLetter, documento) => new JoinDocumentoEnvioNewsLetterDocumento
            {
                Documento = documento,
                DocumentoEnvioNewsLetter = documentoEnvioNewsLetter
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoNivelCertificacion> JoinNivelCertificacion(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NivelCertificacion, item => item.DocumentoWebVinBaseRecursos.NivelCertificacionID, nivelCertificacion => nivelCertificacion.NivelCertificacionID, (item, nivelCertificacion) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoNivelCertificacion
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                NivelCertificacion = nivelCertificacion
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento> JoinDocumento(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Documento = documento
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoProyecto> JoinProyecto(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.Documento.ProyectoID.Value, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoProyecto
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Documento = item.Documento,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoIdentidad
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Documento = item.Documento,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoIdentidadPerfil> JoinPerfil(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoIdentidadPerfil
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Documento = item.Documento,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (documentoWebVinBaseRecursos, baseRecursosProyecto) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinBaseRecursosUsuarioDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<BaseRecursosUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, baseRecursosUsuario => baseRecursosUsuario.BaseRecursosID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.BaseRecursosID, (baseRecursosUsuario, documentoWebVinBaseRecursos) => new JoinBaseRecursosUsuarioDocumentoWebVinBaseRecursos
            {
                BaseRecursosUsuario = baseRecursosUsuario,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinBaseRecursosOrganizacionDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<BaseRecursosOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, baseRecursosOrganizacion => baseRecursosOrganizacion.BaseRecursosID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.BaseRecursosID, (baseRecursosOrganizacion, documentoWebVinBaseRecursos) => new JoinBaseRecursosOrganizacionDocumentoWebVinBaseRecursos
            {
                BaseRecursosOrganizacion = baseRecursosOrganizacion,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumentoDocumentoEntidadGnoss> JoinDocumentoEntidadGnoss(this IQueryable<JoinDocumentoWebVinBaseRecursosDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoEntidadGnoss, item => item.Documento.DocumentoID, documentoEntidadGnoss => documentoEntidadGnoss.DocumentoID, (item, documentoEntidadGnoss) => new JoinDocumentoWebVinBaseRecursosDocumentoDocumentoEntidadGnoss
            {
                Documento = item.Documento,
                DocumentoEntidadGnoss = documentoEntidadGnoss,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosDocumento> JoinDocumento(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (documentoWebVinBaseRecursos, documento) => new JoinDocumentoWebVinBaseRecursosDocumento
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebAgCatTesauroDocumentoDocumentoEntidadGnoss> JoinDocumentoEntidadGnoss(this IQueryable<JoinDocumentoWebAgCatTesauroDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoEntidadGnoss, item => item.Documento.DocumentoID, documentoEntidadGnoss => documentoEntidadGnoss.DocumentoID, (item, documentoEntidadGnoss) => new JoinDocumentoWebAgCatTesauroDocumentoDocumentoEntidadGnoss
            {
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoEntidadGnoss = documentoEntidadGnoss
            });
        }

        public static IQueryable<JoinDocumentoWebAgCatTesauroDocumento> JoinDocumento(this IQueryable<DocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoWebAgCatTesauro => documentoWebAgCatTesauro.DocumentoID, documento => documento.DocumentoID, (documentoWebAgCatTesauro, documento) => new JoinDocumentoWebAgCatTesauroDocumento
            {
                Documento = documento,
                DocumentoWebAgCatTesauro = documentoWebAgCatTesauro
            });
        }

        public static IQueryable<JoinVersionDocumentoDocumentoDocumentoEntidadGnoss> JoinDocumentoEntidadGnoss(this IQueryable<JoinVersionDocumentoDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoEntidadGnoss, item => item.Documento.DocumentoID, documentoEntidadGnoss => documentoEntidadGnoss.DocumentoID, (item, documentoEntidadGnoss) => new JoinVersionDocumentoDocumentoDocumentoEntidadGnoss
            {
                Documento = item.Documento,
                VersionDocumento = item.VersionDocumento,
                DocumentoEntidadGnoss = documentoEntidadGnoss
            });
        }

        public static IQueryable<JoinVersionDocumentoDocumento> JoinDocumento(this IQueryable<VersionDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, versionDocumento => versionDocumento.DocumentoID, documento => documento.DocumentoID, (versionDocumento, documento) => new JoinVersionDocumentoDocumento
            {
                Documento = documento,
                VersionDocumento = versionDocumento
            });
        }

        public static IQueryable<JoinDocumentoAtributoBiblioDocumentoDocumentoEntidadGnoss> JoinDocumentoEntidadGnoss(this IQueryable<JoinDocumentoAtributoBiblioDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoEntidadGnoss, item => item.Documento.DocumentoID, documentoEntidadGnoss => documentoEntidadGnoss.DocumentoID, (item, documentoEntidadGnoss) => new JoinDocumentoAtributoBiblioDocumentoDocumentoEntidadGnoss
            {
                DocumentoEntidadGnoss = documentoEntidadGnoss,
                Documento = item.Documento,
                DocumentoAtributoBiblio = item.DocumentoAtributoBiblio
            });
        }

        public static IQueryable<JoinDocumentoAtributoBiblioDocumento> JoinDocumento(this IQueryable<DocumentoAtributoBiblio> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoAtributoBiblio => documentoAtributoBiblio.DocumentoID, documento => documento.DocumentoID, (documentoAtributoBiblio, documento) => new JoinDocumentoAtributoBiblioDocumento
            {
                Documento = documento,
                DocumentoAtributoBiblio = documentoAtributoBiblio
            });
        }

        public static IQueryable<JoinDocumentoDocumentoEntidadGnoss> JoinDocumentoEntidadGnoss(this IQueryable<Documento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoEntidadGnoss, documento => documento.DocumentoID, documentoEntidadGnoss => documentoEntidadGnoss.DocumentoID, (documento, documentoEntidadGnoss) => new JoinDocumentoDocumentoEntidadGnoss
            {
                DocumentoEntidadGnoss = documentoEntidadGnoss,
                Documento = documento
            });
        }
        public static IQueryable<JoinVotoVotoDocumentoDocumento> JoinDocumento(this IQueryable<JoinVotoVotoDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.VotoDocumento.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinVotoVotoDocumentoDocumento
            {
                VotoDocumento = item.VotoDocumento,
                Voto = item.Voto,
                Documento = documento
            });
        }

        public static IQueryable<JoinVotoVotoDocumento> JoinVotoDocumento(this IQueryable<EntityModel.Models.Voto.Voto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.VotoDocumento, voto => voto.VotoID, votoDocumento => votoDocumento.VotoID, (voto, votoDocumento) => new JoinVotoVotoDocumento
            {
                VotoDocumento = votoDocumento,
                Voto = voto
            });
        }
    }

    /// <summary>
    /// DadaAdapter de documentación
    /// </summary>
    public class DocumentacionAD : BaseAD
    {
        #region Constantes

        public const string COLA_MINIATURA = "ColaMiniatura";
        public const string EXCHANGE = "";
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public DocumentacionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<DocumentacionAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public DocumentacionAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<DocumentacionAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        #region Select simples

        private string selectDocumento;
        private string selectBaseRecursos;
        private string selectBaseRecursosIDProyecto;
        private string selectTipologia;
        private string selectDocumentoTipologia;
        private string selectDocumentoRolIdentidad;
        private string selectDocumentoRolGrupoIdentidades;
        private string selectDocumentoGrupoUsuario;
        private string selectHistorialDocumento;
        private string selectVotoDocumento;
        private string selectVersionDocumento;
        private string selectTagDocumento;
        private string selectFichaBibliografica;
        private string selectAtributoFichaBibliografica;
        private string selectDocumentoAtributoBiblio;
        private string selectDocumentoComentario;
        private string selectDocumentoWebVinBaseRecursos;
        private string selectDocumentoWebVinBaseRecursosExtra;
        private string selectDocumentoWebVinBaseRecursosDistinct;
        private string selectDocumentoWebAgCatTesauro;
        private string selectBaseRecursosOrganizacion;
        private string selectBaseRecursosProyecto;
        private string selectBaseRecursosUsuario;
        private string selectDocumentoEnvioNewsLetter;
        private string selectDocumentoRespuesta;
        private string selectDocumentoRespuestaVoto;
        private string selectDocumentoTokenBrightcove;
        private string selectDocumentoTokenTOP;
        private string selectDocumentoNewsletter;
        private string selectColaCargaRecursos;


        #endregion

        #region Consultas sencillas

        private string sqlSelectDocumento;
        private string sqlSelectDocumentoSimple;
        private string sqlSelectBaseRecursos;
        private string sqlSelectBaseRecursosDocumentoWebVinBaseRecursos;
        private string sqlSelectTipologia;
        private string sqlSelectDocumentoTipologia;
        private string sqlSelectDocumentoRolIdentidad;
        private string sqlSelectDocumentoRolGrupoIdentidades;
        private string sqlSelectDocumentoRolIdentidadPorDocumentoID;
        private string sqlSelectDocumentoRolGrupoIdentidadesPorDocumentoID;
        private string sqlSelectDocumentoGrupoUsuario;
        private string sqlSelectHistorialDocumento;
        private string sqlSelectVotoDocumento;
        private string sqlSelectVersionDocumento;
        private string sqlSelectTagDocumento;
        private string sqlSelectFichaBibliografica;
        private string sqlSelectAtributoFichaBibliografica;
        private string sqlSelectDocumentoAtributoBiblio;
        private string sqlSelectDocumentoComentario;
        private string sqlSelectDocumentoWebVinBaseRecursos;
        private string sqlSelectDocumentoWebVinBaseRecursosExtra;
        private string sqlSelectDocumentoWebAgCatTesauro;
        private string sqlSelectDocumentoWebAgCatTesauroSimple;
        private string sqlSelectBaseRecursosOrganizacion;
        private string sqlSelectBaseRecursosProyecto;
        private string sqlSelectBaseRecursosUsuario;
        private string sqlSelectComprobarConcurrenciaDocumento;
        private string sqlSelectDocumentoEnvioNewsLetter;
        private string sqlSelectColaDocumento;
        private string sqlSelectDocumentoVincDoc;
        private string sqlSelectDocumentoRespuesta;
        private string sqlSelectDocumentoRespuestaVoto;
        private string sqlSelectDocumentoTokenBrightcove;
        private string sqlSelectDocumentoTokenTOP;
        private string sqlSelectDocumentoNewsletter;
        private string sqlSelectColaCargaRecursos;

        #region Distincts

        private string sqlSelectDistinctBaseRecursosProyecto;
        private string sqlSelectDistinctDocumentoWebAgCatTesauro;
        private string sqlSelectDistinctBaseRecursosUsuario;
        private string sqlSelectDistinctDocumentoComentario;

        private string selectDistinctDocumento;

        private string selectDistinctTagDocumento;

        #endregion

        #endregion

        #region Consultas Gordas

        #region Generales

        private string sqlSelectTagsDocumentosDeOrganizacion;
        private string sqlSelectTagsDocumentosDePersona;
        private string sqlSelectDocumentosDeOrganizacion;

        private string sqlSelectDocumentosDeEntidades;
        private string sqlSelectTagDocumentosDeEntidades;
        private string sqlSelectDocumentoAtributoBiblioDeEntidades;
        private string sqlSelectVersionDocumentoDeEntidades;
        private string sqlSelectDocumentoWebAgCatTesauroDeEntidades;
        private string sqlSelectDocumentoWebVinBaseRecursosDeEntidades;

        private string sqlSelectDocumentoPorID;
        private string sqlSelectDocumentoPorIDConNombrePerfil;
        private string sqlSelectTagsDocumentoPorID;
        private string sqlSelectTieneArticulosWikiProyecto;

        private string sqlSelectDocumentoTokenBrightcovePorID;
        private string sqlSelectDocumentoTokenBrightcovePorTokenID;

        private string sqlSelectDocumentoTokenTOPPorID;
        private string sqlSelectDocumentoTokenTOPPorTokenID;

        private string sqlSelectDocumentoNewsletterPorDocumentoID;
        private string sqlSelectDocumentoEnvioNewsletterPendienteEnvio;

        /// <summary>
        /// Actualiza los nombres de las categorías de documentos asociados a entidades cuando estas cambien
        /// </summary>
        private string sqlActualizarNombreCategoriaDocumentalEntidadGnoss;

        /// <summary>
        /// Actualiza los tags de documentos con los nombres de categorias a la que pertenecen
        /// </summary>
        private string sqlActualizarTagNombreCategoriaDocumentacionEntidadGnoss;

        private string sqlSelectTagDocumentoCurriculum;
        private string sqlDeleteTagDocumentoCurriculum;
        private string sqlInsertTagDocumentoCurriculum;
        private string sqlUpdateDocumentoNombreEntidadCurriculum;
        private string sqlSelectDocumentosSuscripcion;
        private string sqlSelectTagsDocumentosSuscripcion;
        private string sqlSelectDocumentosWebDePersonaCompartidos;

        private string sqlRepeticionTituloDocumento;
        private string sqlRepeticionEnlaceDocumento;
        private string sqlRepeticionTituloDocumentoDeBR;
        private string sqlRepeticionEnlaceDocumentoDeBR;
        private string sqlRepeticionTituloDocumentoDeVariasBRs;
        private string sqlRepeticionEnlaceDocumentoDeVariasBRs;
        private string sqlSelectHistorialDocumentoPorDocumentoID;

        private string sqlSelectOntologiasProyecto;
        private string sqlSelectOntologiasPrincYSecundariasProyecto;

        #endregion

        #region Documentos Web

        private string sqlSelectDocumentoWebAgCatTesauroDeBR;
        private string sqlSelectDocumentoWebAgCatTesauroDeBRDeDoc;

        private string sqlComunidadesDePersonaPorDocumentoWEB;
        private string sqlComunidadesDePersonaPorDocumentoBRWEB;
        private string sqlComunidadesDePersonaPorDocumentoBRUsuarioWEB;
        private string sqlComunidadesDePersonaPorDocumentoBROrganizacionWEB;

        private string sqlSelectDocumentosVinculadosAProyecto;
        private string sqlSelectDocumentosWebVinDeProyecto;
        private string sqlSelectDocumentosBaseRecursosUsuario;

        private string sqlSelectNivelCertificacion;
        private string sqlSelectNivelesCertificacionProyecto;
        private string sqlSelectDocumentosBaseRecursosOrganizacion;

        private string sqlActualizarNumerosDocumentacion;
        private string sqlActualizarContadoresTotalesDocumento;
        private string sqlActualizarUltimaVersionDocumento;

        #endregion

        #region Base de Recursos

        private string sqlSelectBaseRecursosDEProyecto;
        private string sqlSelectBaseRecursosProyectoDEProyecto;

        private string sqlSelectBaseRecursosDEUsuario;
        private string sqlSelectBaseRecursosUsuarioDEUsuario;

        private string sqlSelectBaseRecursosDEOrganizacion;
        private string sqlSelectBaseRecursosOrganizacionDEOrganizacion;

        private string selectDistinctBaseRecursosProyecto;
        private string selectDistinctBaseRecursosUsuario;
        private string selectDistinctBaseRecursosOrganizacion;
        private string sqlSelectDocumentoWebVinBaseRecursosDEOrganizacion;
        private string sqlSelectDocumentoWebAgCatTesauroDEOrganizacion;

        #endregion

        #region VersionDocumento

        private string sqlSelectVersionDocPorOriginalDoc;

        #endregion

        #region Comentarios y Votos

        private string sqlSelectTodosComentariosDocumento;

        private string sqlSelectTodosVotosDocumento;

        #endregion

        #region Documentos Temporales

        private string sqlSelectDocumentosTemporales;
        private string sqlSelectDocumentosTemporalesEspecificandoTipo;
        private string sqlSelectDocumentosTemporalesPorSoloCreador;
        private string sqlSelectDocumentosTemporalesEspecificandoTipoPorSoloCreador;
        private string sqlSelectDocumentosTemporalesPorNombre;
        private string sqlSelectDocumentosTemporalesEspecificandoTipoPorNombre;

        #endregion

        #region Documento Newsletter

        private string sqlSelectDocumentoEnvioNewsLetterPorDocID;

        #endregion

        #region Cola de documentos

        private string sqlInsertEnColaDocumento;

        #endregion

        #endregion

        #endregion

        #region DataAdapter
        #region Documento
        private string sqlDocumentoInsert;
        private string sqlDocumentoDelete;
        private string sqlDocumentoModify;
        #endregion

        #region BaseRecursos
        private string sqlBaseRecursosInsert;
        private string sqlBaseRecursosDelete;
        private string sqlBaseRecursosModify;
        #endregion

        #region Tipologia
        private string sqlTipologiaInsert;
        private string sqlTipologiaDelete;
        private string sqlTipologiaModify;
        #endregion

        #region DocumentoTipologia
        private string sqlDocumentoTipologiaInsert;
        private string sqlDocumentoTipologiaDelete;
        private string sqlDocumentoTipologiaModify;
        #endregion

        #region DocumentoRolIdentidad
        private string sqlDocumentoRolIdentidadInsert;
        private string sqlDocumentoRolIdentidadDelete;
        private string sqlDocumentoRolIdentidadModify;
        #endregion

        #region DocumentoRolGrupoIdentidades
        private string sqlDocumentoRolGrupoIdentidadesInsert;
        private string sqlDocumentoRolGrupoIdentidadesDelete;
        private string sqlDocumentoRolGrupoIdentidadesModify;
        #endregion

        #region DocumentoGrupoUsuario
        private string sqlDocumentoGrupoUsuarioInsert;
        private string sqlDocumentoGrupoUsuarioDelete;
        private string sqlDocumentoGrupoUsuarioModify;
        #endregion

        #region HistorialDocumento
        private string sqlHistorialDocumentoInsert;
        private string sqlHistorialDocumentoDelete;
        private string sqlHistorialDocumentoModify;
        #endregion

        #region VotoDocumento
        private string sqlVotoDocumentoInsert;
        private string sqlVotoDocumentoDelete;
        private string sqlVotoDocumentoModify;
        #endregion

        #region VersionDocumento
        private string sqlVersionDocumentoInsert;
        private string sqlVersionDocumentoDelete;
        private string sqlVersionDocumentoModify;
        #endregion

        #region TagDocumento
        private string sqlTagDocumentoInsert;
        private string sqlTagDocumentoDelete;
        private string sqlTagDocumentoModify;
        #endregion

        #region FichaBibliografica
        private string sqlFichaBibliograficaInsert;
        private string sqlFichaBibliograficaDelete;
        private string sqlFichaBibliograficaModify;
        #endregion

        #region AtributoFichaBibliografica
        private string sqlAtributoFichaBibliograficaInsert;
        private string sqlAtributoFichaBibliograficaDelete;
        private string sqlAtributoFichaBibliograficaModify;
        #endregion

        #region DocumentoAtributoBiblio
        private string sqlDocumentoAtributoBiblioInsert;
        private string sqlDocumentoAtributoBiblioDelete;
        private string sqlDocumentoAtributoBiblioModify;
        #endregion

        #region DocumentoComentario
        private string sqlDocumentoComentarioInsert;
        private string sqlDocumentoComentarioDelete;
        private string sqlDocumentoComentarioModify;
        #endregion

        #region DocumentoWebVinBaseRecursos
        private string sqlDocumentoWebVinBaseRecursosInsert;
        private string sqlDocumentoWebVinBaseRecursosDelete;
        private string sqlDocumentoWebVinBaseRecursosModify;
        #endregion

        #region DocumentoWebVinBaseRecursosExtra
        private string sqlDocumentoWebVinBaseRecursosExtraInsert;
        private string sqlDocumentoWebVinBaseRecursosExtraDelete;
        private string sqlDocumentoWebVinBaseRecursosExtraModify;
        #endregion

        #region DocumentoWebAgCatTesauro
        private string sqlDocumentoWebAgCatTesauroInsert;
        private string sqlDocumentoWebAgCatTesauroDelete;
        private string sqlDocumentoWebAgCatTesauroModify;
        #endregion

        #region BaseRecursosOrganizacion
        private string sqlBaseRecursosOrganizacionInsert;
        private string sqlBaseRecursosOrganizacionDelete;
        private string sqlBaseRecursosOrganizacionModify;
        #endregion

        #region BaseRecursosProyecto
        private string sqlBaseRecursosProyectoInsert;
        private string sqlBaseRecursosProyectoDelete;
        private string sqlBaseRecursosProyectoModify;
        #endregion

        #region BaseRecursosUsuario
        private string sqlBaseRecursosUsuarioInsert;
        private string sqlBaseRecursosUsuarioDelete;
        private string sqlBaseRecursosUsuarioModify;
        #endregion

        #region DocumentoEnvioNewsLetter
        private string sqlDocumentoEnvioNewsLetterInsert;
        private string sqlDocumentoEnvioNewsLetterDelete;
        private string sqlDocumentoEnvioNewsLetterModify;
        #endregion

        #region ColaDocumento
        private string sqlColaDocumentoInsert;
        private string sqlColaDocumentoDelete;
        private string sqlColaDocumentoModify;
        #endregion

        #region DocumentoVincDoc
        private string sqlDocumentoVincDocInsert;
        private string sqlDocumentoVincDocDelete;
        private string sqlDocumentoVincDocModify;
        #endregion

        #region DocumentoRespuesta
        private string sqlDocumentoRespuestaInsert;
        private string sqlDocumentoRespuestaDelete;
        private string sqlDocumentoRespuestaModify;
        #endregion

        #region DocumentoRespuestaVoto
        private string sqlDocumentoRespuestaVotoInsert;
        private string sqlDocumentoRespuestaVotoDelete;
        private string sqlDocumentoRespuestaVotoModify;
        #endregion

        #region DocumentoTokenBrightcove
        private string sqlDocumentoTokenBrightcoveInsert;
        private string sqlDocumentoTokenBrightcoveDelete;
        private string sqlDocumentoTokenBrightcoveModify;
        #endregion

        #region DocumentoTokenTOP
        private string sqlDocumentoTokenTOPInsert;
        private string sqlDocumentoTokenTOPDelete;
        private string sqlDocumentoTokenTOPModify;
        #endregion

        #region DocumentoNewsletter
        private string sqlDocumentoNewsletterInsert;
        private string sqlDocumentoNewsletterDelete;
        private string sqlDocumentoNewsletterModify;
        #endregion

        #region ColaCargaRecursos
        private string sqlColaCargaRecursosInsert;
        private string sqlColaCargaRecursosDelete;
        private string sqlColaCargaRecursosModify;
        #endregion

        #region ColaCrawler
        private string sqlColaCrawlerInsert;
        private string sqlColaCrawlerDelete;
        private string sqlColaCrawlerModify;


        #endregion

        #region DocumentoUrlCanonica
        private string sqlDocumentoUrlCanonicaInsert;
        private string sqlDocumentoUrlCanonicaDelete;
        private string sqlDocumentoUrlCanonicaModify;
        #endregion

        #endregion

        #region Métodos Generales

        #region Muy generales

        /// <summary>
        /// Guarda la documentación agregada y modificada.
        /// </summary>
        /// <param name="pDataSet">Dataset de documentación</param>
        public void GuardarDocumentacionActualizada(DataSet pDataSet)
        {
            StringBuilder errorMessage = new StringBuilder();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                DataSet addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);

                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    errorMessage.AppendLine($"Actualizo tabla Documento");

                    #region Actualizar tabla Documento (TOCAR CON CUIDADO)


                    DbCommand InsertDocumentoCommand = ObtenerComando(sqlDocumentoInsert);

                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("CompartirPermitido"), DbType.Boolean, "CompartirPermitido", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("ElementoVinculadoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoVinculadoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Titulo"), DbType.String, "Titulo", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Enlace"), DbType.String, "Enlace", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("FechaCreacion"), DbType.DateTime, "FechaCreacion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("CreadorID"), IBD.TipoGuidToObject(DbType.Guid), "CreadorID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("TipoEntidad"), DbType.Int16, "TipoEntidad", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("NombreCategoriaDoc"), DbType.String, "NombreCategoriaDoc", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("NombreElementoVinculado"), DbType.String, "NombreElementoVinculado", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Publico"), DbType.Boolean, "Publico", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Borrador"), DbType.Boolean, "Borrador", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("CreadorEsAutor"), DbType.Boolean, "CreadorEsAutor", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Valoracion"), DbType.Double, "Valoracion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Autor"), DbType.String, "Autor", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("FechaModificacion"), DbType.DateTime, "FechaModificacion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("IdentidadProteccionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadProteccionID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("FechaProteccion"), DbType.DateTime, "FechaProteccion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("UltimaVersion"), DbType.Boolean, "UltimaVersion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Protegido"), DbType.Boolean, "Protegido", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("NumeroComentariosPublicos"), DbType.Int32, "NumeroComentariosPublicos", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("NumeroTotalVotos"), DbType.Int32, "NumeroTotalVotos", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("NumeroTotalConsultas"), DbType.Int32, "NumeroTotalConsultas", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("NumeroTotalDescargas"), DbType.Int32, "NumeroTotalDescargas", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("VersionFotoDocumento"), DbType.Int32, "VersionFotoDocumento", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Rank"), DbType.Int32, "Rank", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Rank_Tiempo"), DbType.Double, "Rank_Tiempo", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Licencia"), DbType.String, "Licencia", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoCommand, IBD.ToParam("Visibilidad"), DbType.Int16, "Visibilidad", DataRowVersion.Current);

                    DbCommand ModifyDocumentoCommand = ObtenerComando(sqlDocumentoModify);

                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("CompartirPermitido"), DbType.Boolean, "CompartirPermitido", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("ElementoVinculadoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoVinculadoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Titulo"), DbType.String, "Titulo", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Enlace"), DbType.String, "Enlace", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("FechaCreacion"), DbType.DateTime, "FechaCreacion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("CreadorID"), IBD.TipoGuidToObject(DbType.Guid), "CreadorID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("TipoEntidad"), DbType.Int16, "TipoEntidad", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("NombreCategoriaDoc"), DbType.String, "NombreCategoriaDoc", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("NombreElementoVinculado"), DbType.String, "NombreElementoVinculado", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Publico"), DbType.Boolean, "Publico", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Borrador"), DbType.Boolean, "Borrador", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("CreadorEsAutor"), DbType.Boolean, "CreadorEsAutor", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Valoracion"), DbType.Double, "Valoracion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Autor"), DbType.String, "Autor", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("FechaModificacion"), DbType.DateTime, "FechaModificacion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("IdentidadProteccionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadProteccionID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("FechaProteccion"), DbType.DateTime, "FechaProteccion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("UltimaVersion"), DbType.Boolean, "UltimaVersion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Protegido"), DbType.Boolean, "Protegido", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("VersionFotoDocumento"), DbType.Int32, "VersionFotoDocumento", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("NumeroTotalVotos"), DbType.Int32, "NumeroTotalVotos", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Rank"), DbType.Int32, "Rank", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Rank_Tiempo"), DbType.Double, "Rank_Tiempo", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Licencia"), DbType.String, "Licencia", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoCommand, IBD.ToParam("Visibilidad"), DbType.Int16, "Visibilidad", DataRowVersion.Current);

                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "Documento", InsertDocumentoCommand, ModifyDocumentoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla Documento actualizada. Siguiente: BaseRecursos");
                    timer.Restart();


                    #region Actualizar tabla BaseRecursos
                    //DbCommand InsertBaseRecursosCommand = ObtenerComando(sqlBaseRecursosInsert);
                    //AgregarParametro(InsertBaseRecursosCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);

                    //DbCommand ModifyBaseRecursosCommand = ObtenerComando(sqlBaseRecursosModify);
                    //AgregarParametro(ModifyBaseRecursosCommand, IBD.ToParam("O_BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Original);

                    //AgregarParametro(ModifyBaseRecursosCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "BaseRecursos", InsertBaseRecursosCommand, ModifyBaseRecursosCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla BaseRecursos actualizada. Siguiente: Tipologia");
                    timer.Restart();

                    #region Actualizar tabla Tipologia
                    DbCommand InsertTipologiaCommand = ObtenerComando(sqlTipologiaInsert);
                    AgregarParametro(InsertTipologiaCommand, IBD.ToParam("TipologiaID"), IBD.TipoGuidToObject(DbType.Guid), "TipologiaID", DataRowVersion.Current);
                    AgregarParametro(InsertTipologiaCommand, IBD.ToParam("AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Current);
                    AgregarParametro(InsertTipologiaCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertTipologiaCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertTipologiaCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(InsertTipologiaCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(InsertTipologiaCommand, IBD.ToParam("Tipo"), DbType.Int32, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertTipologiaCommand, IBD.ToParam("Orden"), DbType.Int32, "Orden", DataRowVersion.Current);

                    DbCommand ModifyTipologiaCommand = ObtenerComando(sqlTipologiaModify);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("O_TipologiaID"), IBD.TipoGuidToObject(DbType.Guid), "TipologiaID", DataRowVersion.Original);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("O_AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    //AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    //AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    //AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("O_Tipo"), DbType.Int32, "Tipo", DataRowVersion.Original);
                    //AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("O_Orden"), DbType.Int32, "Orden", DataRowVersion.Original);

                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("TipologiaID"), IBD.TipoGuidToObject(DbType.Guid), "TipologiaID", DataRowVersion.Current);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Current);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("Tipo"), DbType.Int32, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyTipologiaCommand, IBD.ToParam("Orden"), DbType.Int32, "Orden", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "Tipologia", InsertTipologiaCommand, ModifyTipologiaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla Tipologia actualizada. Siguiente: DocumentoTipologia");
                    timer.Restart();

                    #region Actualizar tabla DocumentoTipologia
                    DbCommand InsertDocumentoTipologiaCommand = ObtenerComando(sqlDocumentoTipologiaInsert);
                    AgregarParametro(InsertDocumentoTipologiaCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTipologiaCommand, IBD.ToParam("TipologiaID"), IBD.TipoGuidToObject(DbType.Guid), "TipologiaID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTipologiaCommand, IBD.ToParam("AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTipologiaCommand, IBD.ToParam("Valor"), DbType.String, "Valor", DataRowVersion.Current);

                    DbCommand ModifyDocumentoTipologiaCommand = ObtenerComando(sqlDocumentoTipologiaModify);
                    AgregarParametro(ModifyDocumentoTipologiaCommand, IBD.ToParam("O_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTipologiaCommand, IBD.ToParam("O_TipologiaID"), IBD.TipoGuidToObject(DbType.Guid), "TipologiaID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTipologiaCommand, IBD.ToParam("O_AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoTipologiaCommand, IBD.ToParam("O_Valor"), DbType.String, "Valor", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoTipologiaCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTipologiaCommand, IBD.ToParam("TipologiaID"), IBD.TipoGuidToObject(DbType.Guid), "TipologiaID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTipologiaCommand, IBD.ToParam("AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTipologiaCommand, IBD.ToParam("Valor"), DbType.String, "Valor", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoTipologia", InsertDocumentoTipologiaCommand, ModifyDocumentoTipologiaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoTipologia actualizada. Siguiente: DocumentoRolIdentidad");
                    timer.Restart();

                    #region Actualizar tabla DocumentoRolIdentidad
                    DbCommand InsertDocumentoRolIdentidadCommand = ObtenerComando(sqlDocumentoRolIdentidadInsert);
                    AgregarParametro(InsertDocumentoRolIdentidadCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRolIdentidadCommand, IBD.ToParam("PerfilID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRolIdentidadCommand, IBD.ToParam("Editor"), DbType.Boolean, "Editor", DataRowVersion.Current);

                    DbCommand ModifyDocumentoRolIdentidadCommand = ObtenerComando(sqlDocumentoRolIdentidadModify);
                    AgregarParametro(ModifyDocumentoRolIdentidadCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoRolIdentidadCommand, IBD.ToParam("Original_PerfilID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoRolIdentidadCommand, IBD.ToParam("Original_Editor"), DbType.Boolean, "Editor", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoRolIdentidadCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoRolIdentidadCommand, IBD.ToParam("PerfilID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoRolIdentidadCommand, IBD.ToParam("Editor"), DbType.Boolean, "Editor", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoRolIdentidad", InsertDocumentoRolIdentidadCommand, ModifyDocumentoRolIdentidadCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoRolIdentidad actualizada. Siguiente: DocumentoRolGrupoIdentidades");
                    timer.Restart();

                    #region Actualizar tabla DocumentoRolGrupoIdentidades
                    DbCommand InsertDocumentoRolGrupoIdentidadesCommand = ObtenerComando(sqlDocumentoRolGrupoIdentidadesInsert);
                    AgregarParametro(InsertDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("Editor"), DbType.Boolean, "Editor", DataRowVersion.Current);

                    DbCommand ModifyDocumentoRolGrupoIdentidadesCommand = ObtenerComando(sqlDocumentoRolGrupoIdentidadesModify);
                    AgregarParametro(ModifyDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("Original_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("Original_Editor"), DbType.Boolean, "Editor", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoRolGrupoIdentidadesCommand, IBD.ToParam("Editor"), DbType.Boolean, "Editor", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoRolGrupoIdentidades", InsertDocumentoRolGrupoIdentidadesCommand, ModifyDocumentoRolGrupoIdentidadesCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoRolGrupoIdentidades actualizada. Siguiente: DocumentoGrupoUsuario");
                    timer.Restart();

                    #region Actualizar tabla DocumentoGrupoUsuario
                    DbCommand InsertDocumentoGrupoUsuarioCommand = ObtenerComando(sqlDocumentoGrupoUsuarioInsert);
                    AgregarParametro(InsertDocumentoGrupoUsuarioCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoGrupoUsuarioCommand, IBD.ToParam("GrupoUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoUsuarioID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoGrupoUsuarioCommand, IBD.ToParam("Editor"), DbType.Boolean, "Editor", DataRowVersion.Current);

                    DbCommand ModifyDocumentoGrupoUsuarioCommand = ObtenerComando(sqlDocumentoGrupoUsuarioModify);
                    AgregarParametro(ModifyDocumentoGrupoUsuarioCommand, IBD.ToParam("O_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoGrupoUsuarioCommand, IBD.ToParam("O_GrupoUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoUsuarioID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoGrupoUsuarioCommand, IBD.ToParam("O_Editor"), DbType.Boolean, "Editor", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoGrupoUsuarioCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoGrupoUsuarioCommand, IBD.ToParam("GrupoUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoUsuarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoGrupoUsuarioCommand, IBD.ToParam("Editor"), DbType.Boolean, "Editor", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoGrupoUsuario", InsertDocumentoGrupoUsuarioCommand, ModifyDocumentoGrupoUsuarioCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoGrupoUsuario actualizada. Siguiente: HistorialDocumento");
                    timer.Restart();

                    #region Actualizar tabla HistorialDocumento
                    //DbCommand InsertHistorialDocumentoCommand = ObtenerComando(sqlHistorialDocumentoInsert);
                    //AgregarParametro(InsertHistorialDocumentoCommand, IBD.ToParam("HistorialDocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "HistorialDocumentoID", DataRowVersion.Current);
                    //AgregarParametro(InsertHistorialDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //AgregarParametro(InsertHistorialDocumentoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    //AgregarParametro(InsertHistorialDocumentoCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    //AgregarParametro(InsertHistorialDocumentoCommand, IBD.ToParam("TagNombre"), DbType.String, "TagNombre", DataRowVersion.Current);
                    //AgregarParametro(InsertHistorialDocumentoCommand, IBD.ToParam("CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Current);
                    //AgregarParametro(InsertHistorialDocumentoCommand, IBD.ToParam("Accion"), DbType.Int32, "Accion", DataRowVersion.Current);
                    //AgregarParametro(InsertHistorialDocumentoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);

                    //DbCommand ModifyHistorialDocumentoCommand = ObtenerComando(sqlHistorialDocumentoModify);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("O_HistorialDocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "HistorialDocumentoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("O_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("O_TagNombre"), DbType.String, "TagNombre", DataRowVersion.Original);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("O_CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Original);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("O_Accion"), DbType.Int32, "Accion", DataRowVersion.Original);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);

                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("HistorialDocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "HistorialDocumentoID", DataRowVersion.Current);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("TagNombre"), DbType.String, "TagNombre", DataRowVersion.Current);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Current);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("Accion"), DbType.Int32, "Accion", DataRowVersion.Current);
                    //AgregarParametro(ModifyHistorialDocumentoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "HistorialDocumento", InsertHistorialDocumentoCommand, ModifyHistorialDocumentoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla HistorialDocumento actualizada. Siguiente: VotoDocumento");
                    timer.Restart();

                    #region Actualizar tabla VotoDocumento

                    DbCommand InsertVotoDocumentoCommand = ObtenerComando(sqlVotoDocumentoInsert);

                    AgregarParametro(InsertVotoDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertVotoDocumentoCommand, IBD.ToParam("VotoID"), IBD.TipoGuidToObject(DbType.Guid), "VotoID", DataRowVersion.Current);
                    AgregarParametro(InsertVotoDocumentoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);

                    DbCommand ModifyVotoDocumentoCommand = ObtenerComando(sqlVotoDocumentoModify);

                    AgregarParametro(ModifyVotoDocumentoCommand, IBD.ToParam("O_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyVotoDocumentoCommand, IBD.ToParam("O_VotoID"), IBD.TipoGuidToObject(DbType.Guid), "VotoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyVotoDocumentoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);

                    AgregarParametro(ModifyVotoDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyVotoDocumentoCommand, IBD.ToParam("VotoID"), IBD.TipoGuidToObject(DbType.Guid), "VotoID", DataRowVersion.Current);
                    AgregarParametro(ModifyVotoDocumentoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "VotoDocumento", InsertVotoDocumentoCommand, ModifyVotoDocumentoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla VotoDocumento actualizada. Siguiente: VersionDocumento");
                    timer.Restart();

                    #region Actualizar tabla VersionDocumento
                    //DbCommand InsertVersionDocumentoCommand = ObtenerComando(sqlVersionDocumentoInsert);
                    //AgregarParametro(InsertVersionDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //AgregarParametro(InsertVersionDocumentoCommand, IBD.ToParam("Version"), DbType.Int32, "Version", DataRowVersion.Current);
                    //AgregarParametro(InsertVersionDocumentoCommand, IBD.ToParam("DocumentoOriginalID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoOriginalID", DataRowVersion.Current);
                    //AgregarParametro(InsertVersionDocumentoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);

                    //DbCommand ModifyVersionDocumentoCommand = ObtenerComando(sqlVersionDocumentoModify);
                    //AgregarParametro(ModifyVersionDocumentoCommand, IBD.ToParam("O_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    ////AgregarParametro(ModifyVersionDocumentoCommand, IBD.ToParam("O_Version"), DbType.Int32, "Version", DataRowVersion.Original);
                    ////AgregarParametro(ModifyVersionDocumentoCommand, IBD.ToParam("O_DocumentoOriginalID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoOriginalID", DataRowVersion.Original);
                    ////AgregarParametro(ModifyVersionDocumentoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);

                    //AgregarParametro(ModifyVersionDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //AgregarParametro(ModifyVersionDocumentoCommand, IBD.ToParam("Version"), DbType.Int32, "Version", DataRowVersion.Current);
                    //AgregarParametro(ModifyVersionDocumentoCommand, IBD.ToParam("DocumentoOriginalID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoOriginalID", DataRowVersion.Current);
                    //AgregarParametro(ModifyVersionDocumentoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "VersionDocumento", InsertVersionDocumentoCommand, ModifyVersionDocumentoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla VersionDocumento actualizada. Siguiente: FichaBibliografica");
                    timer.Restart();

                    #region Actualizar tabla FichaBibliografica
                    DbCommand InsertFichaBibliograficaCommand = ObtenerComando(sqlFichaBibliograficaInsert);
                    AgregarParametro(InsertFichaBibliograficaCommand, IBD.ToParam("FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Current);
                    AgregarParametro(InsertFichaBibliograficaCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(InsertFichaBibliograficaCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);

                    DbCommand ModifyFichaBibliograficaCommand = ObtenerComando(sqlFichaBibliograficaModify);
                    AgregarParametro(ModifyFichaBibliograficaCommand, IBD.ToParam("O_FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Original);
                    //AgregarParametro(ModifyFichaBibliograficaCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    //AgregarParametro(ModifyFichaBibliograficaCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);

                    AgregarParametro(ModifyFichaBibliograficaCommand, IBD.ToParam("FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Current);
                    AgregarParametro(ModifyFichaBibliograficaCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(ModifyFichaBibliograficaCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "FichaBibliografica", InsertFichaBibliograficaCommand, ModifyFichaBibliograficaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla FichaBibliografica actualizada. Siguiente: AtributoFichaBibliografica");
                    timer.Restart();

                    #region Actualizar tabla AtributoFichaBibliografica
                    DbCommand InsertAtributoFichaBibliograficaCommand = ObtenerComando(sqlAtributoFichaBibliograficaInsert);
                    AgregarParametro(InsertAtributoFichaBibliograficaCommand, IBD.ToParam("AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Current);
                    AgregarParametro(InsertAtributoFichaBibliograficaCommand, IBD.ToParam("FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Current);
                    AgregarParametro(InsertAtributoFichaBibliograficaCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(InsertAtributoFichaBibliograficaCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(InsertAtributoFichaBibliograficaCommand, IBD.ToParam("Tipo"), DbType.Int32, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertAtributoFichaBibliograficaCommand, IBD.ToParam("Orden"), DbType.Int32, "Orden", DataRowVersion.Current);
                    AgregarParametro(InsertAtributoFichaBibliograficaCommand, IBD.ToParam("Longitud"), DbType.Int32, "Longitud", DataRowVersion.Current);

                    DbCommand ModifyAtributoFichaBibliograficaCommand = ObtenerComando(sqlAtributoFichaBibliograficaModify);
                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("O_AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Original);
                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("O_FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Original);
                    //AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    //AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    //AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("O_Tipo"), DbType.Int32, "Tipo", DataRowVersion.Original);
                    //AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("O_Orden"), DbType.Int32, "Orden", DataRowVersion.Original);
                    //AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("O_Longitud"), DbType.Int32, "Longitud", DataRowVersion.Original);

                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Current);
                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Current);
                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("Tipo"), DbType.Int32, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("Orden"), DbType.Int32, "Orden", DataRowVersion.Current);
                    AgregarParametro(ModifyAtributoFichaBibliograficaCommand, IBD.ToParam("Longitud"), DbType.Int32, "Longitud", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "AtributoFichaBibliografica", InsertAtributoFichaBibliograficaCommand, ModifyAtributoFichaBibliograficaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla AtributoFichaBibliografica actualizada. Siguiente: DocumentoAtributoBiblio");
                    timer.Restart();

                    #region Actualizar tabla DocumentoAtributoBiblio
                    DbCommand InsertDocumentoAtributoBiblioCommand = ObtenerComando(sqlDocumentoAtributoBiblioInsert);
                    AgregarParametro(InsertDocumentoAtributoBiblioCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoAtributoBiblioCommand, IBD.ToParam("AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoAtributoBiblioCommand, IBD.ToParam("Valor"), DbType.String, "Valor", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoAtributoBiblioCommand, IBD.ToParam("FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Current);

                    DbCommand ModifyDocumentoAtributoBiblioCommand = ObtenerComando(sqlDocumentoAtributoBiblioModify);
                    AgregarParametro(ModifyDocumentoAtributoBiblioCommand, IBD.ToParam("O_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoAtributoBiblioCommand, IBD.ToParam("O_AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoAtributoBiblioCommand, IBD.ToParam("O_Valor"), DbType.String, "Valor", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoAtributoBiblioCommand, IBD.ToParam("O_FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoAtributoBiblioCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoAtributoBiblioCommand, IBD.ToParam("AtributoID"), IBD.TipoGuidToObject(DbType.Guid), "AtributoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoAtributoBiblioCommand, IBD.ToParam("Valor"), DbType.String, "Valor", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoAtributoBiblioCommand, IBD.ToParam("FichaBibliograficaID"), IBD.TipoGuidToObject(DbType.Guid), "FichaBibliograficaID", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoAtributoBiblio", InsertDocumentoAtributoBiblioCommand, ModifyDocumentoAtributoBiblioCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoAtributoBiblio actualizada. Siguiente: DocumentoComentario");
                    timer.Restart();

                    #region Actualizar tabla DocumentoComentario
                    DbCommand InsertDocumentoComentarioCommand = ObtenerComando(sqlDocumentoComentarioInsert);
                    AgregarParametro(InsertDocumentoComentarioCommand, IBD.ToParam("ComentarioID"), IBD.TipoGuidToObject(DbType.Guid), "ComentarioID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoComentarioCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoComentarioCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);

                    DbCommand ModifyDocumentoComentarioCommand = ObtenerComando(sqlDocumentoComentarioModify);
                    AgregarParametro(ModifyDocumentoComentarioCommand, IBD.ToParam("O_ComentarioID"), IBD.TipoGuidToObject(DbType.Guid), "ComentarioID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoComentarioCommand, IBD.ToParam("O_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoComentarioCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoComentarioCommand, IBD.ToParam("ComentarioID"), IBD.TipoGuidToObject(DbType.Guid), "ComentarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoComentarioCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoComentarioCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoComentario", InsertDocumentoComentarioCommand, ModifyDocumentoComentarioCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoComentario actualizada. Siguiente: DocumentoWebVinBaseRecursos");
                    timer.Restart();

                    #region Actualizar tabla DocumentoWebVinBaseRecursos

                    //DbCommand InsertDocumentoWebVinBaseRecursosCommand = ObtenerComando(sqlDocumentoWebVinBaseRecursosInsert);

                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("IdentidadPublicacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadPublicacionID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("FechaPublicacion"), DbType.DateTime, "FechaPublicacion", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("NumeroComentarios"), DbType.Int32, "NumeroComentarios", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("NumeroVotos"), DbType.Int32, "NumeroVotos", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("PublicadorOrgID"), IBD.TipoGuidToObject(DbType.Guid), "PublicadorOrgID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("PermiteComentarios"), DbType.Boolean, "PermiteComentarios", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("NivelCertificacionID"), IBD.TipoGuidToObject(DbType.Guid), "NivelCertificacionID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("Rank"), DbType.Int32, "Rank", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("Rank_Tiempo"), DbType.Double, "Rank_Tiempo", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("IndexarRecurso"), DbType.Boolean, "IndexarRecurso", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("PrivadoEditores"), DbType.Boolean, "PrivadoEditores", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("TipoPublicacion"), DbType.Int16, "TipoPublicacion", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("LinkAComunidadOrigen"), DbType.Boolean, "LinkAComunidadOrigen", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosCommand, IBD.ToParam("FechaCertificacion"), DbType.DateTime, "FechaCertificacion", DataRowVersion.Current);

                    //DbCommand ModifyDocumentoWebVinBaseRecursosCommand = ObtenerComando(sqlDocumentoWebVinBaseRecursosModify);

                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("Original_BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Original);

                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("FechaPublicacion"), DbType.DateTime, "FechaPublicacion", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("NumeroComentarios"), DbType.Int32, "NumeroComentarios", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("NumeroVotos"), DbType.Int32, "NumeroVotos", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("PublicadorOrgID"), IBD.TipoGuidToObject(DbType.Guid), "PublicadorOrgID", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("PermiteComentarios"), DbType.Boolean, "PermiteComentarios", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("NivelCertificacionID"), IBD.TipoGuidToObject(DbType.Guid), "NivelCertificacionID", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("Rank"), DbType.Int32, "Rank", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("Rank_Tiempo"), DbType.Double, "Rank_Tiempo", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("IndexarRecurso"), DbType.Boolean, "IndexarRecurso", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("PrivadoEditores"), DbType.Boolean, "PrivadoEditores", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("TipoPublicacion"), DbType.Int16, "TipoPublicacion", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("LinkAComunidadOrigen"), DbType.Boolean, "LinkAComunidadOrigen", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosCommand, IBD.ToParam("FechaCertificacion"), DbType.DateTime, "FechaCertificacion", DataRowVersion.Current);

                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoWebVinBaseRecursos", InsertDocumentoWebVinBaseRecursosCommand, ModifyDocumentoWebVinBaseRecursosCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoWebVinBaseRecursos actualizada. Siguiente: DocumentoWebVinBaseRecursosExtra");
                    timer.Restart();

                    #region Actualizar tabla DocumentoWebVinBaseRecursosExtra

                    //DbCommand InsertDocumentoWebVinBaseRecursosExtraCommand = ObtenerComando(sqlDocumentoWebVinBaseRecursosExtraInsert);

                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("NumeroDescargas"), DbType.Int32, "NumeroDescargas", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("NumeroConsultas"), DbType.Int32, "NumeroConsultas", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("FechaUltimaVisita"), DbType.DateTime, "FechaUltimaVisita", DataRowVersion.Current);

                    //DbCommand ModifyDocumentoWebVinBaseRecursosExtraCommand = ObtenerComando(sqlDocumentoWebVinBaseRecursosExtraModify);

                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("Original_BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Original);

                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("NumeroDescargas"), DbType.Int32, "NumeroDescargas", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("NumeroConsultas"), DbType.Int32, "NumeroConsultas", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebVinBaseRecursosExtraCommand, IBD.ToParam("FechaUltimaVisita"), DbType.DateTime, "FechaUltimaVisita", DataRowVersion.Current);

                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoWebVinBaseRecursosExtra", InsertDocumentoWebVinBaseRecursosExtraCommand, ModifyDocumentoWebVinBaseRecursosExtraCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoWebVinBaseRecursosExtra actualizada. Siguiente: DocumentoWebAgCatTesauro");
                    timer.Restart();

                    #region Actualizar tabla DocumentoWebAgCatTesauro
                    //DbCommand InsertDocumentoWebAgCatTesauroCommand = ObtenerComando(sqlDocumentoWebAgCatTesauroInsert);
                    //AgregarParametro(InsertDocumentoWebAgCatTesauroCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebAgCatTesauroCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebAgCatTesauroCommand, IBD.ToParam("CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebAgCatTesauroCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    //AgregarParametro(InsertDocumentoWebAgCatTesauroCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);

                    //DbCommand ModifyDocumentoWebAgCatTesauroCommand = ObtenerComando(sqlDocumentoWebAgCatTesauroModify);
                    ////AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("O_CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("O_BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("O_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);

                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    //AgregarParametro(ModifyDocumentoWebAgCatTesauroCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoWebAgCatTesauro", InsertDocumentoWebAgCatTesauroCommand, ModifyDocumentoWebAgCatTesauroCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoWebAgCatTesauro actualizada. Siguiente: BaseRecursosOrganizacion");
                    timer.Restart();

                    #region Actualizar tabla BaseRecursosOrganizacion
                    DbCommand InsertBaseRecursosOrganizacionCommand = ObtenerComando(sqlBaseRecursosOrganizacionInsert);
                    AgregarParametro(InsertBaseRecursosOrganizacionCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    AgregarParametro(InsertBaseRecursosOrganizacionCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertBaseRecursosOrganizacionCommand, IBD.ToParam("EspacioMaxMyGnossMB"), DbType.Double, "EspacioMaxMyGnossMB", DataRowVersion.Current);
                    AgregarParametro(InsertBaseRecursosOrganizacionCommand, IBD.ToParam("EspacioActualMyGnossMB"), DbType.Double, "EspacioActualMyGnossMB", DataRowVersion.Current);

                    DbCommand ModifyBaseRecursosOrganizacionCommand = ObtenerComando(sqlBaseRecursosOrganizacionModify);
                    AgregarParametro(ModifyBaseRecursosOrganizacionCommand, IBD.ToParam("O_BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Original);
                    AgregarParametro(ModifyBaseRecursosOrganizacionCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    //AgregarParametro(ModifyBaseRecursosOrganizacionCommand, IBD.ToParam("O_EspacioMaxMyGnossMB"), DbType.Double, "EspacioMaxMyGnossMB", DataRowVersion.Original);
                    //AgregarParametro(ModifyBaseRecursosOrganizacionCommand, IBD.ToParam("O_EspacioActualMyGnossMB"), DbType.Double, "EspacioActualMyGnossMB", DataRowVersion.Original);

                    AgregarParametro(ModifyBaseRecursosOrganizacionCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    AgregarParametro(ModifyBaseRecursosOrganizacionCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyBaseRecursosOrganizacionCommand, IBD.ToParam("EspacioMaxMyGnossMB"), DbType.Double, "EspacioMaxMyGnossMB", DataRowVersion.Current);
                    AgregarParametro(ModifyBaseRecursosOrganizacionCommand, IBD.ToParam("EspacioActualMyGnossMB"), DbType.Double, "EspacioActualMyGnossMB", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "BaseRecursosOrganizacion", InsertBaseRecursosOrganizacionCommand, ModifyBaseRecursosOrganizacionCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla BaseRecursosOrganizacion actualizada. Siguiente: BaseRecursosProyecto");
                    timer.Restart();

                    #region Actualizar tabla BaseRecursosProyecto
                    //DbCommand InsertBaseRecursosProyectoCommand = ObtenerComando(sqlBaseRecursosProyectoInsert);
                    //AgregarParametro(InsertBaseRecursosProyectoCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    //AgregarParametro(InsertBaseRecursosProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    //AgregarParametro(InsertBaseRecursosProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);

                    //DbCommand ModifyBaseRecursosProyectoCommand = ObtenerComando(sqlBaseRecursosProyectoModify);
                    //AgregarParametro(ModifyBaseRecursosProyectoCommand, IBD.ToParam("O_BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Original);
                    //AgregarParametro(ModifyBaseRecursosProyectoCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    //AgregarParametro(ModifyBaseRecursosProyectoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);

                    //AgregarParametro(ModifyBaseRecursosProyectoCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    //AgregarParametro(ModifyBaseRecursosProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    //AgregarParametro(ModifyBaseRecursosProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "BaseRecursosProyecto", InsertBaseRecursosProyectoCommand, ModifyBaseRecursosProyectoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla BaseRecursosProyecto actualizada. Siguiente: BaseRecursosUsuario");
                    timer.Restart();

                    #region Actualizar tabla BaseRecursosUsuario
                    DbCommand InsertBaseRecursosUsuarioCommand = ObtenerComando(sqlBaseRecursosUsuarioInsert);
                    AgregarParametro(InsertBaseRecursosUsuarioCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    AgregarParametro(InsertBaseRecursosUsuarioCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(InsertBaseRecursosUsuarioCommand, IBD.ToParam("EspacioMaxMyGnossMB"), DbType.Double, "EspacioMaxMyGnossMB", DataRowVersion.Current);
                    AgregarParametro(InsertBaseRecursosUsuarioCommand, IBD.ToParam("EspacioActualMyGnossMB"), DbType.Double, "EspacioActualMyGnossMB", DataRowVersion.Current);

                    DbCommand ModifyBaseRecursosUsuarioCommand = ObtenerComando(sqlBaseRecursosUsuarioModify);
                    AgregarParametro(ModifyBaseRecursosUsuarioCommand, IBD.ToParam("O_BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Original);
                    AgregarParametro(ModifyBaseRecursosUsuarioCommand, IBD.ToParam("O_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
                    //AgregarParametro(ModifyBaseRecursosUsuarioCommand, IBD.ToParam("O_EspacioMaxMyGnossMB"), DbType.Double, "EspacioMaxMyGnossMB", DataRowVersion.Original);
                    //AgregarParametro(ModifyBaseRecursosUsuarioCommand, IBD.ToParam("O_EspacioActualMyGnossMB"), DbType.Double, "EspacioActualMyGnossMB", DataRowVersion.Original);

                    AgregarParametro(ModifyBaseRecursosUsuarioCommand, IBD.ToParam("BaseRecursosID"), IBD.TipoGuidToObject(DbType.Guid), "BaseRecursosID", DataRowVersion.Current);
                    AgregarParametro(ModifyBaseRecursosUsuarioCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyBaseRecursosUsuarioCommand, IBD.ToParam("EspacioMaxMyGnossMB"), DbType.Double, "EspacioMaxMyGnossMB", DataRowVersion.Current);
                    AgregarParametro(ModifyBaseRecursosUsuarioCommand, IBD.ToParam("EspacioActualMyGnossMB"), DbType.Double, "EspacioActualMyGnossMB", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "BaseRecursosUsuario", InsertBaseRecursosUsuarioCommand, ModifyBaseRecursosUsuarioCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla BaseRecursosUsuario actualizada. Siguiente: DocumentoEnvioNewsLetter");
                    timer.Restart();

                    #region Actualizar tabla DocumentoEnvioNewsLetter
                    DbCommand InsertDocumentoEnvioNewsLetterCommand = ObtenerComando(sqlDocumentoEnvioNewsLetterInsert);
                    AgregarParametro(InsertDocumentoEnvioNewsLetterCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoEnvioNewsLetterCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoEnvioNewsLetterCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoEnvioNewsLetterCommand, IBD.ToParam("Idioma"), DbType.String, "Idioma", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoEnvioNewsLetterCommand, IBD.ToParam("EnvioSolicitado"), DbType.Boolean, "EnvioSolicitado", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoEnvioNewsLetterCommand, IBD.ToParam("EnvioRealizado"), DbType.Boolean, "EnvioRealizado", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoEnvioNewsLetterCommand, IBD.ToParam("Grupos"), DbType.String, "Grupos", DataRowVersion.Current);

                    DbCommand ModifyDocumentoEnvioNewsLetterCommand = ObtenerComando(sqlDocumentoEnvioNewsLetterModify);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Original_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Original_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Original_Idioma"), DbType.String, "Idioma", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Original_EnvioSolicitado"), DbType.Boolean, "EnvioSolicitado", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Original_EnvioRealizado"), DbType.Boolean, "EnvioRealizado", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Idioma"), DbType.String, "Idioma", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("EnvioSolicitado"), DbType.Boolean, "EnvioSolicitado", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("EnvioRealizado"), DbType.Boolean, "EnvioRealizado", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoEnvioNewsLetterCommand, IBD.ToParam("Grupos"), DbType.String, "Grupos", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoEnvioNewsLetter", InsertDocumentoEnvioNewsLetterCommand, ModifyDocumentoEnvioNewsLetterCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoEnvioNewsLetter actualizada. Siguiente: ColaDocumento");
                    timer.Restart();

                    #region Actualizar tabla ColaDocumento
                    //DbCommand InsertColaDocumentoCommand = ObtenerComando(sqlColaDocumentoInsert);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("ID"), DbType.Int32, "ID", DataRowVersion.Current);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("AccionRealizada"), DbType.Int16, "AccionRealizada", DataRowVersion.Current);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("FechaEncolado"), DbType.DateTime, "FechaEncolado", DataRowVersion.Current);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("InfoExtra"), DbType.String, "InfoExtra", DataRowVersion.Current);
                    //AgregarParametro(InsertColaDocumentoCommand, IBD.ToParam("EstadoCargaID"), DbType.Int64, "EstadoCargaID", DataRowVersion.Current);

                    //DbCommand ModifyColaDocumentoCommand = ObtenerComando(sqlColaDocumentoModify);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("Original_ID"), DbType.Int32, "ID", DataRowVersion.Original);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("Original_AccionRealizada"), DbType.Int16, "AccionRealizada", DataRowVersion.Original);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("Original_FechaEncolado"), DbType.DateTime, "FechaEncolado", DataRowVersion.Original);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("Original_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);

                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("ID"), DbType.Int32, "ID", DataRowVersion.Current);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("AccionRealizada"), DbType.Int16, "AccionRealizada", DataRowVersion.Current);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("FechaEncolado"), DbType.DateTime, "FechaEncolado", DataRowVersion.Current);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("InfoExtra"), DbType.String, "InfoExtra", DataRowVersion.Current);
                    //AgregarParametro(ModifyColaDocumentoCommand, IBD.ToParam("EstadoCargaID"), DbType.Int64, "EstadoCargaID", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaDocumento", InsertColaDocumentoCommand, ModifyColaDocumentoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla ColaDocumento actualizada. Siguiente: DocumentoVincDoc");
                    timer.Restart();

                    #region Actualizar tabla DocumentoVincDoc
                    DbCommand InsertDocumentoVincDocCommand = ObtenerComando(sqlDocumentoVincDocInsert);
                    AgregarParametro(InsertDocumentoVincDocCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoVincDocCommand, IBD.ToParam("DocumentoVincID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoVincID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoVincDocCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoVincDocCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);

                    DbCommand ModifyDocumentoVincDocCommand = ObtenerComando(sqlDocumentoVincDocModify);
                    AgregarParametro(ModifyDocumentoVincDocCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoVincDocCommand, IBD.ToParam("Original_DocumentoVincID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoVincID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoVincDocCommand, IBD.ToParam("Original_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    //AgregarParametro(ModifyDocumentoVincDocCommand, IBD.ToParam("Original_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoVincDocCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoVincDocCommand, IBD.ToParam("DocumentoVincID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoVincID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoVincDocCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoVincDocCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoVincDoc", InsertDocumentoVincDocCommand, ModifyDocumentoVincDocCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoVincDoc actualizada. Siguiente: DocumentoRespuesta");
                    timer.Restart();

                    #region Actualizar tabla DocumentoRespuesta
                    DbCommand InsertDocumentoRespuestaCommand = ObtenerComando(sqlDocumentoRespuestaInsert);
                    AgregarParametro(InsertDocumentoRespuestaCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRespuestaCommand, IBD.ToParam("RespuestaID"), IBD.TipoGuidToObject(DbType.Guid), "RespuestaID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRespuestaCommand, IBD.ToParam("Descripcion"), IBD.TipoGuidToObject(DbType.String), "Descripcion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRespuestaCommand, IBD.ToParam("NumVotos"), DbType.Int32, "NumVotos", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRespuestaCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);

                    DbCommand ModifyDocumentoRespuestaCommand = ObtenerComando(sqlDocumentoRespuestaModify);
                    AgregarParametro(ModifyDocumentoRespuestaCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoRespuestaCommand, IBD.ToParam("Original_RespuestaID"), IBD.TipoGuidToObject(DbType.Guid), "RespuestaID", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoRespuestaCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoRespuestaCommand, IBD.ToParam("RespuestaID"), IBD.TipoGuidToObject(DbType.Guid), "RespuestaID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoRespuestaCommand, IBD.ToParam("Descripcion"), IBD.TipoGuidToObject(DbType.String), "Descripcion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoRespuestaCommand, IBD.ToParam("NumVotos"), IBD.TipoGuidToObject(DbType.Int32), "NumVotos", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoRespuestaCommand, IBD.ToParam("Orden"), IBD.TipoGuidToObject(DbType.Int16), "Orden", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoRespuesta", InsertDocumentoRespuestaCommand, ModifyDocumentoRespuestaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoRespuesta actualizada. Siguiente: DocumentoRespuestaVoto");
                    timer.Restart();

                    #region Actualizar tabla DocumentoRespuestaVoto
                    DbCommand InsertDocumentoRespuestaVotoCommand = ObtenerComando(sqlDocumentoRespuestaVotoInsert);
                    AgregarParametro(InsertDocumentoRespuestaVotoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRespuestaVotoCommand, IBD.ToParam("RespuestaID"), IBD.TipoGuidToObject(DbType.Guid), "RespuestaID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoRespuestaVotoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);

                    DbCommand ModifyDocumentoRespuestaVotoCommand = ObtenerComando(sqlDocumentoRespuestaVotoModify);
                    AgregarParametro(ModifyDocumentoRespuestaVotoCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoRespuestaVotoCommand, IBD.ToParam("Original_RespuestaID"), IBD.TipoGuidToObject(DbType.Guid), "RespuestaID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoRespuestaVotoCommand, IBD.ToParam("Original_IdentidadID"), IBD.TipoGuidToObject(DbType.String), "IdentidadID", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoRespuestaVoto", InsertDocumentoRespuestaVotoCommand, ModifyDocumentoRespuestaVotoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoRespuestaVoto actualizada. Siguiente: DocumentoTokenBrightcove");
                    timer.Restart();

                    #region Actualizar tabla DocumentoTokenBrightcove
                    DbCommand InsertDocumentoTokenBrightcoveCommand = ObtenerComando(sqlDocumentoTokenBrightcoveInsert);
                    AgregarParametro(InsertDocumentoTokenBrightcoveCommand, IBD.ToParam("TokenID"), IBD.TipoGuidToObject(DbType.Guid), "TokenID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenBrightcoveCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenBrightcoveCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenBrightcoveCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenBrightcoveCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenBrightcoveCommand, IBD.ToParam("FechaCreacion"), DbType.DateTime, "FechaCreacion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenBrightcoveCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenBrightcoveCommand, IBD.ToParam("NombreArchivo"), DbType.String, "NombreArchivo", DataRowVersion.Current);

                    DbCommand ModifyDocumentoTokenBrightcoveCommand = ObtenerComando(sqlDocumentoTokenBrightcoveModify);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Original_TokenID"), IBD.TipoGuidToObject(DbType.Guid), "TokenID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Original_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Original_FechaCreacion"), DbType.DateTime, "FechaCreacion", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Original_NombreArchivo"), DbType.String, "NombreArchivo", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("TokenID"), IBD.TipoGuidToObject(DbType.Guid), "TokenID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("FechaCreacion"), DbType.DateTime, "FechaCreacion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenBrightcoveCommand, IBD.ToParam("NombreArchivo"), DbType.String, "NombreArchivo", DataRowVersion.Current);


                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoTokenBrightcove", InsertDocumentoTokenBrightcoveCommand, ModifyDocumentoTokenBrightcoveCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoTokenBrightcove actualizada. Siguiente: DocumentoTokenTOP");
                    timer.Restart();

                    #region Actualizar tabla DocumentoTokenTOP
                    DbCommand InsertDocumentoTokenTOPCommand = ObtenerComando(sqlDocumentoTokenTOPInsert);
                    AgregarParametro(InsertDocumentoTokenTOPCommand, IBD.ToParam("TokenID"), IBD.TipoGuidToObject(DbType.Guid), "TokenID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenTOPCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenTOPCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenTOPCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenTOPCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenTOPCommand, IBD.ToParam("FechaCreacion"), DbType.DateTime, "FechaCreacion", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenTOPCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoTokenTOPCommand, IBD.ToParam("NombreArchivo"), DbType.String, "NombreArchivo", DataRowVersion.Current);

                    DbCommand ModifyDocumentoTokenTOPCommand = ObtenerComando(sqlDocumentoTokenTOPModify);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Original_TokenID"), IBD.TipoGuidToObject(DbType.Guid), "TokenID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Original_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Original_FechaCreacion"), DbType.DateTime, "FechaCreacion", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Original_NombreArchivo"), DbType.String, "NombreArchivo", DataRowVersion.Original);

                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("TokenID"), IBD.TipoGuidToObject(DbType.Guid), "TokenID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("FechaCreacion"), DbType.DateTime, "FechaCreacion", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoTokenTOPCommand, IBD.ToParam("NombreArchivo"), DbType.String, "NombreArchivo", DataRowVersion.Current);


                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoTokenTOP", InsertDocumentoTokenTOPCommand, ModifyDocumentoTokenTOPCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoTokenTOP actualizada. Siguiente: DocumentoNewsletter");
                    timer.Restart();

                    #region Actualizar tabla DocumentoNewsletter
                    DbCommand InsertDocumentoNewsletterCommand = ObtenerComando(sqlDocumentoNewsletterInsert);
                    AgregarParametro(InsertDocumentoNewsletterCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoNewsletterCommand, IBD.ToParam("Newsletter"), DbType.String, "Newsletter", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoNewsletterCommand, IBD.ToParam("NewsletterTemporal"), DbType.String, "NewsletterTemporal", DataRowVersion.Current);

                    DbCommand ModifyDocumentoNewsletterCommand = ObtenerComando(sqlDocumentoNewsletterModify);
                    AgregarParametro(ModifyDocumentoNewsletterCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoNewsletterCommand, IBD.ToParam("Original_Newsletter"), DbType.String, "Newsletter", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoNewsletterCommand, IBD.ToParam("Original_NewsletterTemporal"), DbType.String, "NewsletterTemporal", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoNewsletterCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoNewsletterCommand, IBD.ToParam("Newsletter"), DbType.String, "Newsletter", DataRowVersion.Current);
                    AgregarParametro(ModifyDocumentoNewsletterCommand, IBD.ToParam("NewsletterTemporal"), DbType.String, "NewsletterTemporal", DataRowVersion.Current);


                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoNewsletter", InsertDocumentoNewsletterCommand, ModifyDocumentoNewsletterCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoNewsletter actualizada. Siguiente: ColaCargaRecursos");
                    timer.Restart();

                    #region Actualizar tabla ColaCargaRecursos
                    DbCommand InsertColaCargaRecursosCommand = ObtenerComando(sqlColaCargaRecursosInsert);
                    AgregarParametro(InsertColaCargaRecursosCommand, IBD.ToParam("ID"), IBD.TipoGuidToObject(DbType.Guid), "ID", DataRowVersion.Current);
                    AgregarParametro(InsertColaCargaRecursosCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaCargaRecursosCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(InsertColaCargaRecursosCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertColaCargaRecursosCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaCargaRecursosCommand, IBD.ToParam("NombreFichImport"), DbType.String, "NombreFichImport", DataRowVersion.Current);

                    DbCommand ModifyColaCargaRecursosCommand = ObtenerComando(sqlColaCargaRecursosModify);
                    AgregarParametro(ModifyColaCargaRecursosCommand, IBD.ToParam("Original_ColaID"), DbType.Int32, "ColaID", DataRowVersion.Original);


                    AgregarParametro(ModifyColaCargaRecursosCommand, IBD.ToParam("ID"), IBD.TipoGuidToObject(DbType.Guid), "ID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCargaRecursosCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCargaRecursosCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCargaRecursosCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCargaRecursosCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCargaRecursosCommand, IBD.ToParam("NombreFichImport"), DbType.String, "NombreFichImport", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaCargaRecursos", InsertColaCargaRecursosCommand, ModifyColaCargaRecursosCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla ColaCargaRecursos actualizada. Siguiente: ColaCrawler");
                    timer.Restart();

                    #region Actualizar tabla ColaCrawler
                    DbCommand InsertColaCrawlerCommand = ObtenerComando(sqlColaCrawlerInsert);
                    AgregarParametro(InsertColaCrawlerCommand, IBD.ToParam("DBLP"), DbType.String, "DBLP", DataRowVersion.Current);
                    AgregarParametro(InsertColaCrawlerCommand, IBD.ToParam("DxDoi"), DbType.String, "DxDoi", DataRowVersion.Current);
                    AgregarParametro(InsertColaCrawlerCommand, IBD.ToParam("RealURL"), DbType.String, "RealURL", DataRowVersion.Current);
                    AgregarParametro(InsertColaCrawlerCommand, IBD.ToParam("html"), DbType.String, "html", DataRowVersion.Current);
                    AgregarParametro(InsertColaCrawlerCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertColaCrawlerCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);

                    DbCommand ModifyColaCrawlerCommand = ObtenerComando(sqlColaCrawlerModify);
                    AgregarParametro(ModifyColaCrawlerCommand, IBD.ToParam("Original_ColaID"), DbType.Int32, "ColaID", DataRowVersion.Original);

                    AgregarParametro(ModifyColaCrawlerCommand, IBD.ToParam("DBLP"), DbType.String, "DBLP", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCrawlerCommand, IBD.ToParam("DxDoi"), DbType.String, "DxDoi", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCrawlerCommand, IBD.ToParam("RealURL"), DbType.String, "RealURL", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCrawlerCommand, IBD.ToParam("html"), DbType.String, "html", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCrawlerCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(ModifyColaCrawlerCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaCrawler", InsertColaCrawlerCommand, ModifyColaCrawlerCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla ColaCrawler actualizada. Siguiente: DocumentoUrlCanonica");
                    timer.Restart();

                    #region Actualizar tabla DocumentoUrlCanonica
                    DbCommand InsertDocumentoUrlCanonicaCommand = ObtenerComando(sqlDocumentoUrlCanonicaInsert);
                    AgregarParametro(InsertDocumentoUrlCanonicaCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertDocumentoUrlCanonicaCommand, IBD.ToParam("UrlCanonica"), DbType.String, "UrlCanonica", DataRowVersion.Current);

                    DbCommand ModifyDocumentoUrlCanonicaCommand = ObtenerComando(sqlDocumentoUrlCanonicaModify);
                    AgregarParametro(ModifyDocumentoUrlCanonicaCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyDocumentoUrlCanonicaCommand, IBD.ToParam("UrlCanonica"), DbType.String, "UrlCanonica", DataRowVersion.Current);


                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "DocumentoUrlCanonica", InsertDocumentoUrlCanonicaCommand, ModifyDocumentoUrlCanonicaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Tabla DocumentoUrlCanonica actualizada. ");
                    timer.Stop();

                    #endregion


                    addedAndModifiedDataSet.Dispose();
                }
            }
            catch (Exception ex)
            {
                timer.Stop();
                errorMessage.AppendLine($"{timer.ElapsedMilliseconds} -> Error al actualizar la tabla. ");
                mLoggingService.GuardarLogError(ex, errorMessage.ToString(), mlogger);

                throw;
            }
        }

        /// <summary>
        /// Borra del ácido los documentos
        /// </summary>
        /// <param name="pDocumentosID">Lista de identificadores de los documentos a borrar</param>
        public void EliminarDocumentos(List<Guid> pDocumentosID)
        {
            List<Documento> listaDocumentosEliminar = mEntityContext.Documento.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<VotoDocumento> listaVotoDocumentoEliminar = mEntityContext.VotoDocumento.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<DocumentoWebVinBaseRecursosExtra> listaDocumentoWebVinBaseRecursosExtraEliminar = mEntityContext.DocumentoWebVinBaseRecursosExtra.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<DocumentoWebVinBaseRecursos> listaDocumentoWebVinBaseRecursosEliminar = mEntityContext.DocumentoWebVinBaseRecursos.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauroEliminar = mEntityContext.DocumentoWebAgCatTesauro.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<DocumentoRolIdentidad> listaDocumentoRolIdentidadEliminar = mEntityContext.DocumentoRolIdentidad.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<HistorialDocumento> listaHistorialDocumentoEliminar = mEntityContext.HistorialDocumento.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<DocumentoComentario> listaDocumentoComentarioEliminar = mEntityContext.DocumentoComentario.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<DocumentoRolGrupoIdentidades> listaDocumentoRolGrupoIdentidadesEliminar = mEntityContext.DocumentoRolGrupoIdentidades.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            List<DocumentoEnEdicion> listaDocumentoEnEdicionEliminar = mEntityContext.DocumentoEnEdicion.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();


            foreach (DocumentoEnEdicion item in listaDocumentoEnEdicionEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (DocumentoRolIdentidad item in listaDocumentoRolIdentidadEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (DocumentoComentario item in listaDocumentoComentarioEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (HistorialDocumento item in listaHistorialDocumentoEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (DocumentoWebVinBaseRecursosExtra item in listaDocumentoWebVinBaseRecursosExtraEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (DocumentoWebVinBaseRecursosExtra item in listaDocumentoWebVinBaseRecursosExtraEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (DocumentoWebVinBaseRecursos item in listaDocumentoWebVinBaseRecursosEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (DocumentoWebVinBaseRecursosExtra item in listaDocumentoWebVinBaseRecursosExtraEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (DocumentoWebAgCatTesauro item in listaDocumentoWebAgCatTesauroEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (DocumentoRolGrupoIdentidades item in listaDocumentoRolGrupoIdentidadesEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (VotoDocumento item in listaVotoDocumentoEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }
            foreach (Documento item in listaDocumentosEliminar)
            {
                mEntityContext.Entry(item).State = EntityState.Deleted;
            }

            ActualizarBaseDeDatosEntityContext();
        }


        /// <summary>
        /// Obtiene el valor de los votos que un usuario ha realizado a los documentos
        /// </summary>
        /// <param name="pListaDocumentosID">Lista de documentos</param>
        /// <param name="pIdentidadID">Identidad del votante</param>
        /// <returns>Diccionario con el DocumentoID y el valor del voto realizado</return s>
        public Dictionary<Guid, double> ObtenerVotoRecurso(List<Guid> pListaDocumentosID, Guid pIdentidadID)
        {
            Dictionary<Guid, double> diccionario = null;

            if (pListaDocumentosID.Count > 0)
            {
                diccionario = new Dictionary<Guid, double>();

                List<RegistroVotoDocumento> consulta = mEntityContext.Voto.JoinVotoDocumento().JoinDocumento().Where(item => item.Voto.IdentidadID.Equals(pIdentidadID) && !item.Documento.CreadorID.Equals(pIdentidadID) && pListaDocumentosID.Contains(item.Documento.DocumentoID)).Select(item => new RegistroVotoDocumento
                {
                    DocumentoID = item.VotoDocumento.DocumentoID,
                    Voto = item.Voto.Voto1
                }).ToList();

                Dictionary<Guid, double> idsVotados = new Dictionary<Guid, double>();

                foreach (RegistroVotoDocumento votoDocumento in consulta)
                {
                    idsVotados.Add(votoDocumento.DocumentoID, votoDocumento.Voto);
                }

                foreach (Guid DocID in pListaDocumentosID)
                {
                    double voto = 0;

                    if (idsVotados.ContainsKey(DocID))
                    {
                        voto = idsVotados[DocID];
                    }

                    if (!diccionario.ContainsKey(DocID))
                    {
                        diccionario.Add(DocID, voto);
                    }
                    else
                    {
                        diccionario[DocID] = voto;
                    }
                }
            }
            return diccionario;
        }

        /// <summary>
        /// Obtiene el valor de los votos que un usuario ha realizado a los documentos
        /// </summary>
        /// <param name="pListaDocumentosID">Lista de documentos</param>
        /// <param name="pIdentidadID">Identidad del votante</param>
        /// <returns>Diccionario con el DocumentoID y el valor del voto realizado</return s>
        public List<Guid> ObtenerRecursosCompartidosEnBRUsuario(List<Guid> pListaDocumentosID, Guid pUsuarioID)
        {
            List<Guid> listaCompartidos = new List<Guid>();
            if (pListaDocumentosID.Count > 0)
            {
                listaCompartidos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario().Where(objeto => !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.BaseRecursosUsuario.UsuarioID.Equals(pUsuarioID) && pListaDocumentosID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID).ToList();
            }

            return listaCompartidos;
        }

        /// <summary>
        /// Obtiene el valor de los votos que un usuario ha realizado a los documentos
        /// </summary>
        /// <param name="pListaDocumentosID">Lista de documentos</param>
        /// <param name="pIdentidadID">Identidad del votante</param>
        /// <returns>Diccionario con el DocumentoID y el valor del voto realizado</return s>
        public List<Guid> ObtenerRecursosCompartidosEnBROrganizacion(List<Guid> pListaDocumentosID, Guid pOrganizacionID)
        {
            List<Guid> listaCompartidos = new List<Guid>();
            if (pListaDocumentosID.Count > 0)
            {

                listaCompartidos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario().JoinPersona().JoinPerfil().Where(objeto => !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Perfil.OrganizacionID.HasValue && objeto.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && pListaDocumentosID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID).ToList();

            }

            return listaCompartidos;
        }

        public List<Documento> ObtenerListaRecursosUsuarioActualizarPorComunidad(Guid pPerfilID)
        {
            return mEntityContext.Identidad.JoinDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.UltimaVersion).OrderByDescending(objeto => objeto.Documento.FechaCreacion).Select(objeto => objeto.Documento).Take(100).ToList();
        }

        /// <summary>
        /// Obtiene el documento que se encuentra vinculado a un elemento y no esté marcado como eliminado
        /// </summary>
        /// <param name="pElementoVinculadoID">Identificador del elemento vinculado</param>
        /// <returns>Dataset de documentación</return s>
        public DataWrapperDocumentacion ObtenerDocumentoDeElementoVinculado(Guid pElementoVinculadoID)
        {
            return ObtenerDocumentoDeElementoVinculado(pElementoVinculadoID, true);
        }

        /// <summary>
        /// Obtiene el enlace del documento vinculado al documento dado
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <returns></return s>
        public string ObtenerEnlaceDocumentoVinculadoADocumento(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Join(mEntityContext.Documento, documento => new { DocumentoID = documento.DocumentoID }, documentoOnto => new { DocumentoID = documentoOnto.ElementoVinculadoID.Value }, (documento, documentoOnto) => new
            {
                Documento = documento,
                DocumentoOnto = documentoOnto
            }).Where(objeto => objeto.DocumentoOnto.ElementoVinculadoID.HasValue && objeto.DocumentoOnto.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.Documento.Enlace).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el documento que se encuentra vinculado a un elemento y no esté marcado como eliminado
        /// </summary>
        /// <param name="pClaveElementoVinculadoID">Identificador del elemento al que están vinculados los documentos</param>
        /// <param name="pHacerCargaTotalDocumentos">TRUE si debe hacerse una carga completa de los documentos resultado, 
        /// FALSE si sólo debe traerse sus datos de la tabla documento</param>
        /// <returns>Dataset de documentación</return s>
        public DataWrapperDocumentacion ObtenerDocumentoDeElementoVinculado(Guid pClaveElementoVinculadoID, bool pHacerCargaTotalDocumentos)
        {
            DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();

            docDW.ListaDocumento = mEntityContext.Documento.Where(doc => doc.ElementoVinculadoID.Value.Equals(pClaveElementoVinculadoID) && !doc.Eliminado).ToList();
            if (docDW.ListaDocumento.Count == 1 && pHacerCargaTotalDocumentos)
            {
                DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();
                ObtenerDocumentoPorIDCargarTotal(docDW.ListaDocumento.First().DocumentoID, documentacionDW, true, true, Guid.Empty);
                docDW = null;

                return documentacionDW;
            }
            else
            {
                return docDW;
            }
        }

        /// <summary>
        /// Obtiene los IDs de las ontologías de un proyecto a partir del enlace de las ontologías encontradas en otro proyecto
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de las ontologías encontradas en un proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se quieren buscar ontologías con el mismo enlace</param>
        /// <returns>Diccionario del tipo IDOntologiaOriginal -> IDOntologíaEnProyecto</return s>
        public Dictionary<Guid, Guid> ObtenerElementoVinculadoIDDeOtroProyectoConMismoEnlace(List<Guid> pListaDocumentosID, Guid pProyectoID)
        {
            Dictionary<Guid, Guid> dicElementosVinculadosProyecto = new Dictionary<Guid, Guid>();

            if (pListaDocumentosID.Count > 0)
            {

                var resultado = mEntityContext.Documento.Join(mEntityContext.Documento, documento => documento.Enlace, docOntoOriginal => docOntoOriginal.Enlace, (documento, docOntoOriginal) => new
                {
                    Documento = documento,
                    DocOntoOriginal = docOntoOriginal
                }).Where(objeto => pListaDocumentosID.Contains(objeto.DocOntoOriginal.DocumentoID) && (objeto.Documento.Tipo.Equals(7) || objeto.Documento.Tipo.Equals(23)) && objeto.Documento.ProyectoID.HasValue && objeto.Documento.ProyectoID.Value.Equals(pProyectoID) && !objeto.Documento.Eliminado).Select(objeto => new { DocOntoOriginalID = objeto.DocOntoOriginal.DocumentoID, objeto.Documento.DocumentoID }).ToList();

                //SELECT docOntoOriginal.DocumentoID, Documento.DocumentoID  FROM Documento INNER JOIN Documento docOntoOriginal on docOntoOriginal.Enlace = Documento.Enlace where  docOntoOriginal.DocumentoID = '26b9ecbf-5979-4738-b6fc-3e05211726d1' and Documento.Tipo in (7, 23) and Documento.ProyectoID = 'd4ca22ca-4c3c-460f-8d31-8f3e951ed0b0' and Documento.Eliminado = 0

                foreach (var fila in resultado)
                {
                    if (!dicElementosVinculadosProyecto.ContainsKey(fila.DocOntoOriginalID))
                    {
                        dicElementosVinculadosProyecto.Add(fila.DocOntoOriginalID, fila.DocumentoID);
                    }
                }

            }

            return dicElementosVinculadosProyecto;
        }

        /// <summary>
        /// Obtiene los elementos vinculados asociados a los documentos
        /// </summary>
        /// <param name="pClaveElementoVinculadoID"></param>
        /// <returns>DocumentacionDS</return s>
        public Dictionary<Guid, Guid> ObtenerElementoVinculadoIDPorDocumentoID(List<Guid> pListaDocumentosID)
        {
            Dictionary<Guid, Guid> dicElementosVinculadosDocumentos = new Dictionary<Guid, Guid>();

            if (pListaDocumentosID.Count > 0)
            {
                var resultado = mEntityContext.Documento.Where(doc => pListaDocumentosID.Contains(doc.DocumentoID)).Select(doc => new { doc.DocumentoID, doc.ElementoVinculadoID }).ToList();

                foreach (var fila in resultado)
                {
                    if (fila.ElementoVinculadoID.HasValue)
                    {
                        dicElementosVinculadosDocumentos.Add(fila.DocumentoID, fila.ElementoVinculadoID.Value);
                    }
                }
            }
            return dicElementosVinculadosDocumentos;
        }

        /// <summary>
        /// Obtiene si un usuario tiene acceso a un recurso concreto para leerlo o editarlo
        /// </summary>
        /// <param name="pProyectosID">Identificador de los proyecto en el que se desea comprobar el permiso</param>
        /// <param name="pDocumentoID">Identificador del recurso que se está viendo</param>
        /// <param name="pPerfilID">Identificador del perfil con el que está conectado el usuario (NULL si el usuario no está conectado)</param>
        /// <param name="pIdentidadComunidadID">Identificador de la identidad del usuario en la comunidad (NULL si el usuario no participa en la comunidad)</param>
        /// <param name="pIdentidadMyGnossID">Identificador de la identidad del usuario en MyGnoss (NULL si el usuario no está conectado)</param>
        /// <param name="pEditando">Verdad si el usuario quiere editar el recurso</param>
        /// <param name="pUsuarioPerteneceACom">Verdad si el usuario pertenece a la comunidad</param>
        /// <returns>True si el usuario tiene acceso al recurso, False en caso contrario</return s>
        public bool TieneUsuarioAccesoADocumentoEnProyecto(List<Guid> pProyectosID, Guid pDocumentoID, Guid? pPerfilID, Guid? pIdentidadComunidadID, Guid? pIdentidadMyGnossID, bool pEditando, bool pUsuarioPerteneceACom)
        {
            if (!pEditando)
            {
                //Si no está editando, el usuario puede ver los recursos públicos
                var resNoEditando = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => objeto.Documento.DocumentoID.Equals(pDocumentoID) && objeto.Documento.Eliminado == false && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores == false && pProyectosID.Contains(objeto.BaseRecursosProyecto.ProyectoID));

                if (!pUsuarioPerteneceACom)
                {
                    //Quitamos los recursos visibles sólo para miembros
                    resNoEditando = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => objeto.Documento.DocumentoID.Equals(pDocumentoID) && objeto.Documento.Eliminado == false && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores == false && pProyectosID.Contains(objeto.BaseRecursosProyecto.ProyectoID) && objeto.Documento.Visibilidad < (short)VisibilidadDocumento.PrivadoMiembrosComunidad);
                }

                if (pUsuarioPerteneceACom)
                {
                    var usPerteneceACom = mEntityContext.Documento.JoinDocumentoDocumentoRolGrupoIdentidades().JoinGrupoIdentidadesParticipacion().Where(objeto => objeto.Documento.DocumentoID.Equals(pDocumentoID) && objeto.Documento.Eliminado == false && ((objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadComunidadID.Value) && pIdentidadComunidadID.HasValue) || (objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadMyGnossID.Value) && pIdentidadMyGnossID.HasValue))).Select(objeto => objeto.Documento)
                        .Concat(
                        mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().Where(objeto => objeto.Documento.DocumentoID.Equals(pDocumentoID) && objeto.Documento.Eliminado == false && (pPerfilID.HasValue && objeto.DocumentoRolIdentidad.PerfilID.Equals(pPerfilID.Value))).Select(objeto => objeto.Documento)
                        );

                    return resNoEditando.Select(objeto => objeto.Documento).Concat(usPerteneceACom).Any();
                }
                else
                {
                    return resNoEditando.Any();
                }
            }

            if (pUsuarioPerteneceACom && pEditando)
            {
                var usPerteneceACom = mEntityContext.Documento.JoinDocumentoDocumentoRolGrupoIdentidades().JoinGrupoIdentidadesParticipacion().Where(objeto => objeto.Documento.DocumentoID.Equals(pDocumentoID) && objeto.Documento.Eliminado == false && ((objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadComunidadID.Value) && pIdentidadComunidadID.HasValue) || (objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadMyGnossID.Value) && pIdentidadMyGnossID.HasValue)) && objeto.DocumentoRolGrupoIdentidades.Editor && objeto.Documento.UltimaVersion).Select(objeto => objeto.Documento)
                        .Concat(
                        mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().Where(objeto => objeto.Documento.DocumentoID.Equals(pDocumentoID) && objeto.Documento.Eliminado == false && (pPerfilID.HasValue && objeto.DocumentoRolIdentidad.PerfilID.Equals(pPerfilID.Value)) && objeto.DocumentoRolIdentidad.Editor && objeto.Documento.UltimaVersion).Select(objeto => objeto.Documento)
                        );
                return usPerteneceACom.Any();

            }
            return false;
        }

        /// <summary>
        /// Carga la tabla DocumentoWebVinBaseRecursos con la lista de los documentos modificados
        /// </summary>
        /// <param name="pDocumentosID">Lista de ids de los documentos modificados</param>
        /// <returns>Devuelve el dataset con la tabla cargada</return s>
        public List<DocumentoWebVinBaseRecursos> ObtenerWebVinBaseRecursosDocumentosModificados(List<Guid> pDocumentosID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Where(docWebVin => pDocumentosID.Contains(docWebVin.DocumentoID)).ToList();
        }

        /// <summary>
        /// Obtiene una lista de identificadores de los documentos que han sido modificados, comentados o votados a partir de una fecha en una comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        /// <param name="pFechaBusqueda">Fecha a partir de la cual buscar recursos con actividad</param>
        /// <returns>Lista de identificadores de documento</return s>
        public List<Guid> ObtenerDocumentosActivosEnFecha(Guid pProyectoID, DateTime pFechaBusqueda)
        {
            var primeraParteConsulta = mEntityContext.VotoDocumento.JoinVoto().Where(item => item.Voto.FechaVotacion.Value >= pFechaBusqueda && item.VotoDocumento.ProyectoID.Value.Equals(pProyectoID)).Select(item => item.VotoDocumento.DocumentoID);
            var segundaParteConsulta = mEntityContext.DocumentoComentario.JoinComentario().Where(item => item.Comentario.Fecha >= pFechaBusqueda && item.DocumentoComentario.ProyectoID.Value.Equals(pProyectoID)).Select(item => item.DocumentoComentario.DocumentoID);
            var terceraParteCosulta = mEntityContext.HistorialDocumento.Where(item => item.Fecha >= pFechaBusqueda && item.ProyectoID.Equals(pProyectoID)).Select(item => item.DocumentoID);

            return primeraParteConsulta.Union(segundaParteConsulta).Union(terceraParteCosulta).ToList();
        }

        /// <summary>
        /// Obtiene la documentación de una lista de entidades
        /// </summary>
        /// <param name="pListaEntidades">Lista de claves</param>
        /// <param name="pSoloUltimaVersionNoEliminados">TRUE si solo se deben traer los que sean última version y NO eliminados, FALSE para traer todos</param>
        /// <returns>DocumentacionDS</return s>
        public DataWrapperDocumentacion ObtenerDocumentosDeEntidades(List<Guid> pListaEntidades, bool pSoloUltimaVersionNoEliminados)
        {
            //Todo => Campo DocumentoEntidadGnoss.EntidadGnossID vacío

            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            if (pListaEntidades.Count > 0)
            {
                var queryDocumento = mEntityContext.Documento.JoinDocumentoEntidadGnoss().Where(item => pListaEntidades.Contains(item.DocumentoEntidadGnoss.EntidadGnossID));
                var queryDocumentoAtributoBiblio = mEntityContext.DocumentoAtributoBiblio.JoinDocumento().JoinDocumentoEntidadGnoss().Where(item => pListaEntidades.Contains(item.DocumentoEntidadGnoss.EntidadGnossID));
                var queryVersionDocumento = mEntityContext.VersionDocumento.JoinDocumento().JoinDocumentoEntidadGnoss().Where(item => pListaEntidades.Contains(item.DocumentoEntidadGnoss.EntidadGnossID));
                var queryDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumento().JoinDocumentoEntidadGnoss().Where(item => pListaEntidades.Contains(item.DocumentoEntidadGnoss.EntidadGnossID));
                var queryDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumento().JoinDocumentoEntidadGnoss().Where(item => pListaEntidades.Contains(item.DocumentoEntidadGnoss.EntidadGnossID));

                if (pSoloUltimaVersionNoEliminados)
                {
                    //Documento
                    documentacionDW.ListaDocumento = queryDocumento.Where(item => item.Documento.Eliminado.Equals(0) && item.Documento.UltimaVersion.Equals(1)).Select(item => item.Documento).ToList();

                    //DocumentoAtributoBiblio
                    documentacionDW.ListaDocumentoAtributoBiblio = queryDocumentoAtributoBiblio.Where(item => item.Documento.Eliminado.Equals(0) && item.Documento.UltimaVersion.Equals(1)).Select(item => item.DocumentoAtributoBiblio).ToList();

                    //VersionDocumento
                    documentacionDW.ListaVersionDocumento = queryVersionDocumento.Where(item => item.Documento.Eliminado.Equals(0) && item.Documento.UltimaVersion.Equals(1)).Select(item => item.VersionDocumento).ToList();

                    //DocumentoWebAgCatTesauro
                    documentacionDW.ListaDocumentoWebAgCatTesauro = queryDocumentoWebAgCatTesauro.Where(item => item.Documento.Eliminado.Equals(0) && item.Documento.UltimaVersion.Equals(1)).Select(item => item.DocumentoWebAgCatTesauro).ToList();

                    //DocumentoWebVinBaseRecursos
                    documentacionDW.ListaDocumentoWebVinBaseRecursos = queryDocumentoWebVinBaseRecursos.Where(item => pListaEntidades.Contains(item.DocumentoEntidadGnoss.EntidadGnossID)).Select(item => item.DocumentoWebVinBaseRecursos).ToList();
                }
                else
                {
                    //Documento
                    documentacionDW.ListaDocumento = queryDocumento.Select(item => item.Documento).ToList();

                    //DocumentoAtributoBiblio
                    documentacionDW.ListaDocumentoAtributoBiblio = queryDocumentoAtributoBiblio.Select(item => item.DocumentoAtributoBiblio).ToList();

                    //VersionDocumento
                    documentacionDW.ListaVersionDocumento = queryVersionDocumento.Select(item => item.VersionDocumento).ToList();

                    //DocumentoWebAgCatTesauro
                    documentacionDW.ListaDocumentoWebAgCatTesauro = queryDocumentoWebAgCatTesauro.Select(item => item.DocumentoWebAgCatTesauro).ToList();

                    //DocumentoWebVinBaseRecursos
                    documentacionDW.ListaDocumentoWebVinBaseRecursos = queryDocumentoWebVinBaseRecursos.Select(item => item.DocumentoWebVinBaseRecursos).ToList();
                }
            }

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene la documentación de una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de documentación</return s>
        public List<Documento> ObtenerDocumentacion(Guid pOrganizacionID)
        {
            return mEntityContext.Documento.Where(doc => doc.OrganizacionID.Equals(pOrganizacionID)).ToList();
        }

        public void ObtenerDocumentosDeIdentidadEnProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pIdentidadID, Guid pProyectoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWebVin => docWebVin.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (docWebVin, baseRecursoProy) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                BaseRecursosProyecto = baseRecursoProy
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value.Equals(pIdentidadID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).ToList().Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (objeto, baseRecProy) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value.Equals(pIdentidadID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).ToList().Select(objeto => objeto.Documento).ToList();
            pDataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
        }

        /// <summary>
        /// Obtiene unos documentos a partir de sus identificadores.
        /// </summary>
        /// <param name="pListaDocumentoID">Lista de identificadores de documento</param>
        /// <param name="pTraerBasesRecurso">Indica si se debe traer las bases de recursos o no</param>
        /// <param name="pObtenerUsuariosExternos">Indica si se debe obtener los usuarios externos o no</param>
        /// <returns>Dataset de documentación con los documentos cargados</return s>
        public DataWrapperDocumentacion ObtenerDocumentosPorID(List<Guid> pListaDocumentoID, bool pTraerBasesRecurso)
        {
            return ObtenerDocumentosPorID(pListaDocumentoID, pTraerBasesRecurso, Guid.Empty);
        }

        public List<Guid> ObtenerRecursosSemanticosPublicadosDesdeFecha(DateTime fecha, Guid? pProyectoID = null)
        {
            List<Guid> listaRecursos = new List<Guid>();

            if (pProyectoID.HasValue && !pProyectoID.Value.Equals(Guid.Empty))
            {
                listaRecursos = mEntityContext.Documento.Where(documento => documento.Tipo.Equals((short)TiposDocumentacion.Semantico) && !documento.Eliminado && documento.FechaModificacion > fecha && documento.ProyectoID.Equals(pProyectoID)).Select(doc => doc.DocumentoID).ToList();
            }
            else
            {
                listaRecursos = mEntityContext.Documento.Where(documento => documento.Tipo.Equals((short)TiposDocumentacion.Semantico) && !documento.Eliminado && documento.FechaModificacion > fecha).Select(doc => doc.DocumentoID).ToList();
            }

            return listaRecursos;
        }

        /// <summary>
        /// Obtiene unos documentos a partir de sus identificadores.
        /// </summary>
        /// <param name="pListaDocumentoID">Identificadores de documento</param>
        /// <param name="pTraerBasesRecurso">Indica si se debe traer las tablas baseRecursos o no</param>
        /// <param name="pProyectoID">Proyecto en el que se realiza la búsqueda</param>
        /// <param name="pObtenerUsuariosExternos">Indica si se debe obtener los usuarios externos o no</param>
        /// <returns>Dataset de documentación con los documentos cargados</return s>
        public DataWrapperDocumentacion ObtenerDocumentosPorID(List<Guid> pListaDocumentoID, bool pTraerBasesRecurso, Guid pProyectoID)
        {
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            if (pListaDocumentoID.Count > 0)
            {
                //Documento
                documentacionDW.ListaDocumento = mEntityContext.Documento.Where(doc => pListaDocumentoID.Contains(doc.DocumentoID)).ToList();

                //DocumentoWebVinBaseRecursos
                documentacionDW.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(docWebVin => pListaDocumentoID.Contains(docWebVin.DocumentoID)).ToList();

                //DocumentoWebAgCatTesauro
                documentacionDW.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(docWebAg => pListaDocumentoID.Contains(docWebAg.DocumentoID)).ToList();

                //DocumentoRolIdentidad
                documentacionDW.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(docRolIden => pListaDocumentoID.Contains(docRolIden.DocumentoID)).ToList();

                //DocumentoRolGrupoIdentidades
                documentacionDW.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(docRolGrId => pListaDocumentoID.Contains(docRolGrId.DocumentoID)).ToList();

                if (pTraerBasesRecurso)
                {
                    //BaseRecursos
                    //documentacionDW.ListaDocumentoWebVinBaseRecursos
                    List<Guid> baseRecursoIDs = documentacionDW.ListaDocumentoWebVinBaseRecursos.Select(item => item.BaseRecursosID).ToList();
                    documentacionDW.ListaBaseRecursos = mEntityContext.BaseRecursos.Where(baseRec => baseRecursoIDs.Contains(baseRec.BaseRecursosID)).ToList();

                    if (pProyectoID.Equals(Guid.Empty) || !pProyectoID.Equals(ProyectoAD.MetaProyecto))
                    {
                        //BaseRecursosProyecto
                        documentacionDW.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Where(objeto => baseRecursoIDs.Contains(objeto.BaseRecursosID)).ToList();
                    }
                    if (pProyectoID.Equals(Guid.Empty) || pProyectoID.Equals(ProyectoAD.MetaProyecto))
                    {
                        //BaseRecursosOrganizacion
                        documentacionDW.ListaBaseRecursosOrganizacion = mEntityContext.BaseRecursosOrganizacion.Where(objeto => baseRecursoIDs.Contains(objeto.BaseRecursosID)).ToList();

                        //BaseRecursosUsuario
                        documentacionDW.ListaBaseRecursosUsuario = mEntityContext.BaseRecursosUsuario.Where(objeto => baseRecursoIDs.Contains(objeto.BaseRecursosID)).ToList();

                    }
                }
            }

            return documentacionDW;
        }

        /// <summary>
        /// Carga las tablas: Documento, DocumentoWebVinBaseRecursos, VotoDocumento, BaseRecursos, BaseRecursosProyecto y BaseRecursosUsuario necesarias para pintar el listado de acciones
        /// </summary>
        /// <param name="pListaDocumentos">Lista de documentos para traer</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pUsuarioID">ID del usuario</param>
        public DataWrapperDocumentacion ObtenerDocumentosPorIDParaListadoDeAcciones(List<Guid> pListaDocumentoID, Guid pProyectoID, Guid pUsuarioID)
        {
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            if (pListaDocumentoID.Count > 0)
            {
                documentacionDW.ListaDocumento = mEntityContext.Documento.Where(doc => pListaDocumentoID.Contains(doc.DocumentoID)).ToList();
                documentacionDW.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => pListaDocumentoID.Contains(doc.DocumentoID)).ToList();
                documentacionDW.ListaVotoDocumento = mEntityContext.VotoDocumento.Where(item => pListaDocumentoID.Contains(item.DocumentoID)).ToList();
                documentacionDW.ListaBaseRecursos = mEntityContext.BaseRecursos.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRec => baseRec.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRec, docWebVin) => new
                {
                    BaseRecursos = baseRec,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => pListaDocumentoID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID)).Select(objeto => objeto.BaseRecursos).ToList();

                documentacionDW.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRec => baseRec.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRec, docWebVin) => new
                {
                    BaseRecursosProyecto = baseRec,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => pListaDocumentoID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.BaseRecursosProyecto).ToList();

                documentacionDW.ListaBaseRecursosUsuario = mEntityContext.BaseRecursosUsuario.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRec => baseRec.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRec, docWebVin) => new
                {
                    BaseRecursosUsuario = baseRec,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => pListaDocumentoID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID)).Select(objeto => objeto.BaseRecursosUsuario).ToList();
                documentacionDW.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(docRolGrupoIden => pListaDocumentoID.Contains(docRolGrupoIden.DocumentoID)).ToList();

                documentacionDW.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(docRolIden => pListaDocumentoID.Contains(docRolIden.DocumentoID)).ToList();
            }
            return documentacionDW;
        }

        /// <summary>
        /// Obtiene unos documentos a partir de sus identificadores, solo tabla Documento.
        /// </summary>
        /// <param name="pListaDocumentoID">Identificadores de documento</param>
        /// <returns>Dataset de documentación con los documentos cargados, solo tabla Documento</return s>
        public List<Documento> ObtenerDocumentosPorIDSoloDocumento(List<Guid> pListaDocumentoID)
        {
            List<Documento> listaDocumentos = new List<Documento>();

            if (pListaDocumentoID.Count > 0)
            {
                listaDocumentos = mEntityContext.Documento.Where(documento => pListaDocumentoID.Contains(documento.DocumentoID)).ToList();
            }

            return listaDocumentos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaDocumentoID"></param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerDocumentosPorIDYComunidadesComparticion(List<Guid> pListaDocumentoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            // Documento
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(doc => pListaDocumentoID.Contains(doc.DocumentoID)).ToList();
            // DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => pListaDocumentoID.Contains(doc.DocumentoID)).ToList();
            // BaseRecursosProyecto
            dataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWebVin => docWebVin.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (docWebVin, baseRecProy) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => pListaDocumentoID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID)).Select(objeto => objeto.BaseRecursosProyecto).ToList();
            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Actualiza el ranking del documento web vin base recursos pasado como parámetro
        /// </summary>
        /// <param name="pDocID">Documento que se va a actualizar</param>
        /// <param name="pBaseRecursos">Base de recursos a la que pertenece el recurso</param>
        /// <param name="pRank">Ranking a actualizar</param>
        public void ActualizarRankingDocumentoWebVinBaseRecurso(Guid pDocID, Guid pBaseRecursos, int pRank)
        {
            #region Actualiza Ranking en DocumentoWebVinBaseRecursos
            DocumentoWebVinBaseRecursos docWebVinBaseRec = mEntityContext.DocumentoWebVinBaseRecursos.FirstOrDefault(doc => doc.DocumentoID.Equals(pDocID) && doc.BaseRecursosID.Equals(pBaseRecursos));
            if (docWebVinBaseRec != null)
            {
                docWebVinBaseRec.Rank = pRank;
                mEntityContext.SaveChanges();
            }


            #endregion
        }

        /// <summary>
        /// Actualiza el ranking del Documento.
        /// </summary>
        /// <param name="pDocID">Documento que se va a actualizar</param>
        /// <param name="pRank">Ranking del documento que se va a actualizar.</param>
        public void ActualizarRankingDocumento(Guid pDocID, int pRank)
        {
            #region Actualizar Ranking en Documento
            Documento documento = mEntityContext.Documento.FirstOrDefault(doc => doc.DocumentoID.Equals(pDocID));
            if (documento != null)
            {
                documento.Rank = pRank;
                mEntityContext.SaveChanges();
            }

            #endregion
        }

        /// <summary>
        /// Obtiene los documentos más vistos de las comunidades públicas de GNOSS sin tener en cuenta ayuda y FAQS y Noticias
        /// </summary>
        /// <param name="numDocumentos">Numero de documentos para traer</param>
        public List<Documento> ObtenerDocumentosMasVistos(int numDocumentos)
        {
            var query = mEntityContext.Documento.Join(mEntityContext.Proyecto, doc => doc.ProyectoID, proy => proy.ProyectoID, (doc, proy) => new
            {
                Documento = doc,
                Proyecto = proy
            }).Where(objeto => objeto.Documento.Publico && !objeto.Documento.Borrador && !objeto.Documento.Eliminado && objeto.Documento.Rank_Tiempo.HasValue && objeto.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Publico) && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.ProyectoFAQ) && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.ProyectoNoticias)).Select(objeto => objeto.Documento).OrderByDescending(objeto => objeto.Rank_Tiempo.Value).Take(numDocumentos);

            return query.ToList();
        }

        public List<Guid> ObtenerDocumentosMasVistosProyecto(Guid pProyectoID, int numDocumentos)
        {//Javi Ruiz:  No quitar el orderByDescending sirve para el metodo que llama este metodo (DocumentacionCN.ObtenerDocumentosMasVistosProyecto)

            //Documento
            //string consulta = "SELECT TOP " + numDocumentos + " Documento.DocumentoID, DocumentoWebVinBaseRecursosExtra.NumeroConsultas FROM Documento  INNER JOIN DocumentoWebVinBaseRecursos ON DocumentoWebVinBaseRecursos.DocumentoID = Documento.DocumentoID INNER JOIN BaseRecursosProyecto ON BaseRecursosProyecto.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID INNER JOIN DocumentoWebVinBaseRecursosExtra on DocumentoWebVinBaseRecursos.DocumentoID = DocumentoWebVinBaseRecursosExtra.DocumentoID AND DocumentoWebVinBaseRecursosExtra.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID WHERE BaseRecursosProyecto.ProyectoID = " + IBD.GuidValor(pProyectoID) + " AND (DocumentoWebVinBaseRecursos.PrivadoEditores = 0) AND (DocumentoWebVinBaseRecursos.Eliminado = 0) AND (Documento.Borrador = 0) AND (Documento.Eliminado = 0) ORDER BY DocumentoWebVinBaseRecursosExtra.NumeroConsultas";

            //DbCommand commandsqlSelectDocumento = ObtenerComando(consulta);
            //CargarDataSet(commandsqlSelectDocumento, dataSet, "MasVisitados");

            return mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().JoinDocumentoWebVinBaseRecursosExtra().Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.Documento.Borrador && !objeto.Documento.Eliminado).OrderBy(objeto => objeto.DocumentoWebVinBaseRecursosExtra.NumeroConsultas).Select(objeto => objeto.Documento.DocumentoID).Take(numDocumentos).ToList();
        }

        /// <summary>
        /// Obtiene los documentos publicos más populares de la comunidad indicada
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNumDocumentos">Numero de documentos que queremos traer</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerRecursosPopularesProyecto(Guid pProyectoID, int pNumDocumentos)
        {
            List<Guid> listaDocs = new List<Guid>();
            listaDocs = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (objeto, baseRecProy) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && !objeto.Documento.Borrador && !objeto.Documento.Eliminado && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Encuesta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).OrderByDescending(objeto => objeto.DocumentoWebVinBaseRecursos.Rank_Tiempo).Select(objeto => objeto.Documento.DocumentoID).Take(pNumDocumentos).ToList();

            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();
            documentacionDW.Merge(ObtenerDocumentosPorID(listaDocs, false, pProyectoID));

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene los IDs de los documentos publicos más populares de la comunidad indicada
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNumDocumentos">Numero de documentos que queremos traer</param>
        /// <returns>Lista de DocumentoID</return s>
        public List<Guid> ObtenerListaRecursosPopularesProyecto(Guid pProyectoID, int pNumDocumentos)
        {
            List<Guid> listaDocs = new List<Guid>();
            listaDocs = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (objeto, baseRecProy) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && !objeto.Documento.Borrador && !objeto.Documento.Eliminado && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Encuesta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria) && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).OrderByDescending(objeto => objeto.DocumentoWebVinBaseRecursos.Rank_Tiempo).Select(objeto => objeto.Documento.DocumentoID).Take(pNumDocumentos).ToList();

            return listaDocs;
        }

        /// <summary>
        /// Obtiene un datawrapper con el documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset de documentación con el documento cargado</return s>
        public DataWrapperDocumentacion ObtenerDocumentoPorID(Guid pDocumentoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();
            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns></returns>
        public Documento ObtenerDocumentoPorIdentificador(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset de documentación con el documento cargado</return s>
        public DataWrapperDocumentacion ObtenerDocumentoPorIDConSubEventos(Guid pDocumentoID, Guid pProyectoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, pProyectoID);

            //Documento
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWebVin => docWebVin.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (docWebVin, baseRecProy) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            //VotoDocumento
            dataWrapperDocumentacion.ListaVotoDocumento = mEntityContext.VotoDocumento.JoinVoto().Where(item => item.VotoDocumento.DocumentoID.Equals(pDocumentoID) && item.VotoDocumento.ProyectoID.Value.Equals(pProyectoID)).Select(item => item.VotoDocumento).Take(1).ToList();

            //DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().JoinNivelCertificacion().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.DocumentoWebVinBaseRecursos.NivelCertificacionID.HasValue && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.DocumentoWebVinBaseRecursos).ToList();

            //DocumentoComentario
            dataWrapperDocumentacion.ListaDocumentoComentario = mEntityContext.DocumentoComentario.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene las filas de GrupoEditorRecurso 
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns>DataSet con las filas de GrupoEditorRecurso </return s>
        public List<DocumentoRolGrupoIdentidades> ObtenerFilasGrupoEditorRecurso(Guid pGrupoID)
        {
            return mEntityContext.DocumentoRolGrupoIdentidades.Where(doc => doc.GrupoID.Equals(pGrupoID)).ToList();
        }

        /// <summary>
        /// Obtiene el Dataset del token de brightcove de un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset del token de brightcove del documento</return s>
        public DataWrapperDocumentacion ObtenerDocumentoTokenBrightcovePorID(Guid pDocumentoID)
        {
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoTokenBirghtcove
            documentacionDW.ListaDocumentoTokenBrightcove = mEntityContext.DocumentoTokenBrightcove.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene el Dataset de tokens de brightcove pendientes
        /// </summary>
        /// <returns>Dataset de tokens de brightcove pendientes</return s>
        public DataWrapperDocumentacion ObtenerDocumentoTokenBrightcovePendientes()
        {
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoTokenBirghtcove
            documentacionDW.ListaDocumentoTokenBrightcove = mEntityContext.DocumentoTokenBrightcove.Where(item => !item.Estado.Equals((short)EstadoSubidaVideo.Finalizado) && !item.Estado.Equals((short)EstadoSubidaVideo.Fallido) && !item.Estado.Equals((short)EstadoSubidaVideo.FinalizadoYCorrecto)).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene el Dataset de tokens de brightcove finalizadas
        /// </summary>
        /// <returns>Dataset de tokens de brightcove finalizadas</return s>
        public DataWrapperDocumentacion ObtenerDocumentoTokenBrightcoveFinalizadas()
        {
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoTokenBirghtcove
            documentacionDW.ListaDocumentoTokenBrightcove = mEntityContext.DocumentoTokenBrightcove.Where(item => item.Estado.Equals((short)EstadoSubidaVideo.Finalizado)).ToList();

            return documentacionDW;
        }


        /// <summary>
        /// Obtiene el Dataset del token de brightcove de un documento a partir de su token.
        /// </summary>
        /// <param name="pTokenID">Identificador del token</param>
        /// <returns>Dataset del token de brightcove del documento</return s>
        public DataWrapperDocumentacion ObtenerDocumentoTokenBrightcovePorTokenID(Guid pTokenID)
        {
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoTokenBirghtcove
            documentacionDW.ListaDocumentoTokenBrightcove = mEntityContext.DocumentoTokenBrightcove.Where(item => item.TokenID.Equals(pTokenID)).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene el Dataset del token de TOP de un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset del token de TOP del documento</return s>
        public DataWrapperDocumentacion ObtenerDocumentoTokenTOPPorID(Guid pDocumentoID)
        {
            // TODO Probar -> Tabla DocumentoTokenTop vacía
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoTokenTOP
            documentacionDW.ListaDocumentoTokenTOP = mEntityContext.DocumentoTokenTOP.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene el Dataset de tokens de TOP pendientes
        /// </summary>
        /// <returns>Dataset de tokens de brightcove pendientes</return s>
        public DataWrapperDocumentacion ObtenerDocumentoTokenTOPPendientes()
        {
            // TODO Probar -> Tabla DocumentoTokenTop vacía
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoTokenTOP
            documentacionDW.ListaDocumentoTokenTOP = mEntityContext.DocumentoTokenTOP.Where(item => item.Estado.Equals((short)EstadoSubidaVideo.SubidoaGNOSSEditado) || item.Estado.Equals((short)EstadoSubidaVideo.SubidoaGNOSS)).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene el Dataset de tokens de TOP finalizadas
        /// </summary>
        /// <returns>Dataset de tokens de TOP finalizadas</return s>
        public DataWrapperDocumentacion ObtenerDocumentoTokenTOPFinalizadas()
        {
            // TODO Probar -> Tabla DocumentoTokenTop vacía
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoTokenTOP
            documentacionDW.ListaDocumentoTokenTOP = mEntityContext.DocumentoTokenTOP.Where(item => item.Estado.Equals((short)EstadoSubidaVideo.Finalizado)).ToList();

            return documentacionDW;
        }


        /// <summary>
        /// Obtiene el Dataset del token de TOP de un documento a partir de su token.
        /// </summary>
        /// <param name="pTokenID">Identificador del token</param>
        /// <returns>Dataset del token de TOP del documento</return s>
        public DataWrapperDocumentacion ObtenerDocumentoTokenTOPPorTokenID(Guid pTokenID)
        {
            // TODO Probar -> Tabla DocumentoTokenTop vacía
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoTokenTOP
            documentacionDW.ListaDocumentoTokenTOP = mEntityContext.DocumentoTokenTOP.Where(item => item.TokenID.Equals(pTokenID)).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene el Dataset de DocumentoEnvioNewsLetter pendientes de enviar.
        /// </summary>
        /// <returns>DocumentoEnvioNewsLetter</return s>
        public List<NewsletterPendientes> ObtenerDocumentoEnvioNewsletterPendienteEnvio(Guid? pDocumentoID = null)
        {
            // TODO Probar -> Utilizar BD Didactalia

            var queryPrimeraParte = mEntityContext.DocumentoEnvioNewsLetter.JoinDocumento().Where(item => item.DocumentoEnvioNewsLetter.EnvioSolicitado.Equals(false) && item.DocumentoEnvioNewsLetter.EnvioRealizado.Equals(false) && !item.Documento.Descripcion.Equals("")).Select(item => new NewsletterPendientes
            {
                DocumentoID = item.DocumentoEnvioNewsLetter.DocumentoID,
                IdentidadID = item.DocumentoEnvioNewsLetter.IdentidadID,
                Fecha = item.DocumentoEnvioNewsLetter.Fecha,
                ProyectoID = item.Documento.ProyectoID,
                Titulo = item.Documento.Titulo,
                Descripcion = item.Documento.Descripcion,
                Idioma = item.DocumentoEnvioNewsLetter.Idioma,
                EnvioSolicitado = item.DocumentoEnvioNewsLetter.EnvioSolicitado,
                EnvioRealizado = item.DocumentoEnvioNewsLetter.EnvioRealizado,
                Grupos = item.DocumentoEnvioNewsLetter.Grupos,
                Newsletter = null
            });

            var querySegundaParte = mEntityContext.DocumentoEnvioNewsLetter.JoinDocumento().JoinDocumentoNewsLetter().Where(item => item.DocumentoEnvioNewsLetter.EnvioSolicitado.Equals(false) && item.DocumentoEnvioNewsLetter.EnvioRealizado.Equals(false)).Select(item => new NewsletterPendientes
            {
                DocumentoID = item.DocumentoEnvioNewsLetter.DocumentoID,
                IdentidadID = item.DocumentoEnvioNewsLetter.IdentidadID,
                Fecha = item.DocumentoEnvioNewsLetter.Fecha,
                ProyectoID = item.Documento.ProyectoID,
                Titulo = item.Documento.Titulo,
                Descripcion = item.Documento.Descripcion,
                Idioma = item.DocumentoEnvioNewsLetter.Idioma,
                EnvioSolicitado = item.DocumentoEnvioNewsLetter.EnvioSolicitado,
                EnvioRealizado = item.DocumentoEnvioNewsLetter.EnvioRealizado,
                Grupos = item.DocumentoEnvioNewsLetter.Grupos,
                Newsletter = item.DocumentoNewsLetter.Newsletter
            });

            if (pDocumentoID.HasValue)
            {
                queryPrimeraParte = queryPrimeraParte.Where(item => item.DocumentoID.Equals(pDocumentoID.Value));

                querySegundaParte = querySegundaParte.Where(item => item.DocumentoID.Equals(pDocumentoID.Value));
            }

            return queryPrimeraParte.Concat(querySegundaParte).OrderByDescending(item => item.Fecha).ToList().Distinct().ToList();
        }

        /// <summary>
        /// Obtiene el Dataset de DocumentoEnvioNewsLetter pendientes de enviar.
        /// </summary>
        /// <returns>DocumentoEnvioNewsLetter</return s>
        public List<NewsletterPendientes> ObtenerDocumentoEnvioNewsletterPendienteEnvioRabbit(DocumentoEnvioNewsLetter pDocumentoEnvioNewsletter)
        {
            // TODO Probar -> Utilizar BD Didactalia

            var queryPrimeraParte = mEntityContext.Documento.Where(item => pDocumentoEnvioNewsletter.EnvioSolicitado.Equals(false) && pDocumentoEnvioNewsletter.EnvioRealizado.Equals(false) && !item.Descripcion.Equals("") && item.DocumentoID.Equals(pDocumentoEnvioNewsletter.DocumentoID)).Select(item => new NewsletterPendientes
            {
                DocumentoID = pDocumentoEnvioNewsletter.DocumentoID,
                IdentidadID = pDocumentoEnvioNewsletter.IdentidadID,
                Fecha = pDocumentoEnvioNewsletter.Fecha,
                ProyectoID = item.ProyectoID,
                Titulo = item.Titulo,
                Descripcion = item.Descripcion,
                Idioma = pDocumentoEnvioNewsletter.Idioma,
                EnvioSolicitado = pDocumentoEnvioNewsletter.EnvioSolicitado,
                EnvioRealizado = pDocumentoEnvioNewsletter.EnvioRealizado,
                Grupos = pDocumentoEnvioNewsletter.Grupos,
                Newsletter = null
            });

            var querySegundaParte = mEntityContext.Documento.Join(mEntityContext.DocumentoNewsletter, documento => documento.DocumentoID, documentoNewsletter => documentoNewsletter.DocumentoID, (documento, documentoNewsletter) => new
            {
                Documento = documento,
                DocumentoNewsLetter = documentoNewsletter
            }).Where(item => pDocumentoEnvioNewsletter.EnvioSolicitado.Equals(false) && pDocumentoEnvioNewsletter.EnvioRealizado.Equals(false) && item.Documento.DocumentoID.Equals(pDocumentoEnvioNewsletter.DocumentoID)).Select(item => new NewsletterPendientes
            {
                DocumentoID = pDocumentoEnvioNewsletter.DocumentoID,
                IdentidadID = pDocumentoEnvioNewsletter.IdentidadID,
                Fecha = pDocumentoEnvioNewsletter.Fecha,
                ProyectoID = item.Documento.ProyectoID,
                Titulo = item.Documento.Titulo,
                Descripcion = item.Documento.Descripcion,
                Idioma = pDocumentoEnvioNewsletter.Idioma,
                EnvioSolicitado = pDocumentoEnvioNewsletter.EnvioSolicitado,
                EnvioRealizado = pDocumentoEnvioNewsletter.EnvioRealizado,
                Grupos = pDocumentoEnvioNewsletter.Grupos,
                Newsletter = item.DocumentoNewsLetter.Newsletter
            });

            return queryPrimeraParte.Concat(querySegundaParte).OrderByDescending(item => item.Fecha).ToList().Distinct().ToList();
        }

        /// <summary>
        /// Actualiza los campos EnvioSolicitado y EnvioRealizado de la tabla DocumentoEnvioNewsletter
        /// </summary>
        /// <param name="pEnvioSolicitado">True/False que indica si se ha procesado el envío de la newsletter</param>
        /// <param name="pEnvioRealizado">True/False que indica si se ha encolado la newsletter en 
        /// satisfactoriamente</param>
        /// <param name="pDocumentoID">Identificador de la newsletter</param>
        /// <param name="pIdentidadID">Identidad publicadora de la newsletter</param>
        /// <param name="pFecha">Fecha de publicación de la newsletter</param>
        public void ActuarlizarEnvioRealizadoDocumentoEnvioNewsletter(bool pEnvioSolicitado, bool pEnvioRealizado, Guid pDocumentoID, Guid pIdentidadID, DateTime pFecha)
        {
            DocumentoEnvioNewsLetter documento = mEntityContext.DocumentoEnvioNewsLetter.Where(item => item.DocumentoID.Equals(pDocumentoID) && item.IdentidadID.Equals(pIdentidadID) && item.Fecha.Equals(pFecha)).FirstOrDefault();

            documento.EnvioRealizado = pEnvioRealizado;
            documento.EnvioSolicitado = pEnvioSolicitado;

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene el Dataset de la newsletter a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset</return s>
        public DataWrapperDocumentacion ObtenerDocumentoNewsletterPorDocumentoID(Guid pDocumentoID)
        {
            // TODO Probar -> Utilizar BD Didactalia

            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            //DocumentoNewsletter
            documentacionDW.ListaDocumentoNewsLetter = mEntityContext.DocumentoNewsletter.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene la descripcion de un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>DataSet de documentación con el documento cargado.</return s>
        public string ObtenerDescripcionDocumentoPorID(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).ToList().FirstOrDefault().Descripcion;
        }

        /// <summary>
        /// Obtiene la descripcion de una suscripcion de un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>DataSet de documentación con el documento cargado.</return s>
        public string ObtenerDescripcionSuscripcionDocumentoPorID(Guid pDocumentoID, Guid pSuscripcionID)
        {
            return mEntityContext.ResultadoSuscripcion.Where(item => item.RecursoID.Equals(pDocumentoID) && item.SuscripcionID.Equals(pSuscripcionID)).Select(item => item.Descripcion).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las tablas "Documento", "DocumentoWebVinBaseRecursos" a partir del identificador de un documento en un determinado proyecto.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DocumentacionDS de documentación con el documento cargado</return s>
        public DataWrapperDocumentacion ObtenerDocumentoDocumentoWebVinBRPorID(Guid pDocumentoID, Guid pProyectoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //Documento
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).ToList();
            //DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWebVin => docWebVin.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (docWebVin, baseRecProy) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            //DocumentoRolIdentidad
            dataWrapperDocumentacion.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoRolGrupoIdentidades 
            dataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            if (dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Count > 0)
            {
                //DocumentoWebAgCatTesauro:
                Guid documentoWebVinBaseRecursosGuid = dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.First().BaseRecursosID;
                dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(doc => doc.BaseRecursosID.Equals(documentoWebVinBaseRecursosGuid) && doc.DocumentoID.Equals(pDocumentoID)).ToList();
            }

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene todas las bases de recursos del documento id pasado por parámetro
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Lista de DocumentoWebVinBaseRecursos del documento pasado por parámetro</returns>
        public List<DocumentoWebVinBaseRecursos> ObtenerListaDocumentoWebVinBaseRecursoPorDocumentoID(Guid pDocumentoID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();
        }

        public List<DocumentoWebVinBaseRecursos> ObtenerDocumentoWebVinBRPorDocIDYProyID(Guid pDocumentoID, Guid pProyectoID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWebVin => docWebVin.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (docWebVin, baseRecProy) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();
        }

        /// <summary>
        /// Obtiene las tablas "Documento", "DocumentoWebVinBaseRecursos" a partir del identificador de un documento en un determinado proyecto.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>DocumentacionDS de documentación con el documento cargado</return s>
        public DataWrapperDocumentacion ObtenerDocumentoDocumentoWebVinBRPorIDDeUsuario(Guid pDocumentoID, Guid pUsuarioID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //Documento
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).ToList();
            //DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosUsuario, docWebVin => docWebVin.BaseRecursosID, baseRecUs => baseRecUs.BaseRecursosID, (docWebVin, baseRecUs) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                BaseRecursosUsuario = baseRecUs
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosUsuario.UsuarioID.Equals(pUsuarioID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            //DocumentoRolIdentidad
            dataWrapperDocumentacion.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoRolGrupoIdentidades
            dataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            if (dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Count > 0)
            {
                Guid documentoWebVinBaseRecursosBaseRecursosID = dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.First().BaseRecursosID;

                //DocumentoWebAgCatTesauro:
                dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(doc => doc.BaseRecursosID.Equals(documentoWebVinBaseRecursosBaseRecursosID) && doc.DocumentoID.Equals(pDocumentoID)).ToList();
            }

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene las tablas "Documento", "DocumentoWebVinBaseRecursos" a partir del identificador de un documento en un determinado proyecto.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>DocumentacionDS de documentación con el documento cargado</return s>
        public DataWrapperDocumentacion ObtenerDocumentoDocumentoWebVinBRPorIDDeOrganizacion(Guid pDocumentoID, Guid pOrganizacionID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //Documento
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosOrganizacion, docWebVin => docWebVin.BaseRecursosID, baseRecOrg => baseRecOrg.BaseRecursosID, (docWebVin, baseRecOrg) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                BaseRecursosOrganizacion = baseRecOrg
            }
            ).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            //DocumentoRolIdentidad
            dataWrapperDocumentacion.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoRolGrupoIdentidades
            dataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            if (dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Count > 0)
            {
                Guid baseRecursosID = dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.First().BaseRecursosID;
                //DocumentoWebAgCatTesauro:
                dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.BaseRecursosID.Equals(baseRecursosID)).ToList();
            }

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Carga toda la información de la ultima encuesta de una comunidad
        /// </summary>       
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public DataWrapperDocumentacion ObtenerEncuestaParaHome(Guid pProyectoID, Guid pBaseRecursosID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            Documento documento = mEntityContext.Documento.Where(doc => doc.ProyectoID.Value.Equals(pProyectoID) && !doc.Eliminado && doc.FechaCreacion.HasValue && !doc.Borrador && doc.Tipo.Equals((short)TiposDocumentacion.Encuesta)).OrderByDescending(doc => doc.FechaCreacion.Value).FirstOrDefault();


            if (documento != null)
            {
                //TODO : Hacer una carga mas ligera
                ObtenerDocumentoPorIDCargarTotal(documento.DocumentoID, dataWrapperDocumentacion, true, true, pBaseRecursosID);
            }

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Carga el ID de la ultima encuesta de una comunidad
        /// </summary>       
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public Guid? ObtenerIDEncuestaParaHome(Guid pProyectoID)
        {
            Documento documento = mEntityContext.Documento.Where(doc => doc.ProyectoID.Value.Equals(pProyectoID) && !doc.Eliminado && doc.FechaCreacion.HasValue && !doc.Borrador && doc.Tipo.Equals((short)TiposDocumentacion.Encuesta)).OrderByDescending(doc => doc.FechaCreacion.Value).FirstOrDefault();
            if (documento != null)
            {
                return documento.DocumentoID;
            }
            return null;
        }

        /// <summary>
        /// Carga toda la información de un documento menos las versiones del mismo.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="dataWrapperDocumentacion">Dataset de documentación</param>
        /// <param name="pDatosCompletos">TRUE si debe cargarse todo del documento, FALSE si sólo los complementos(historial, versión, ect.)</param>
        /// <param name="pTraerBaseRecursos">Indica si se traen las BR o no</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos a la que pertenece el documento o Guid.Empty si se desea obtener de la que se publico o NULL si se desea traer todas</param>
        public void ObtenerDocumentoPorIDCargarTotal(Guid pDocumentoID, DataWrapperDocumentacion pDataWrapperDocumentacion, bool pDatosCompletos, bool pTraerBaseRecursos, Guid? pBaseRecursosID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            if (pDatosCompletos)
            {
                //DbCommand commandsqlSelectDocumentoPorID = ObtenerComando(selectDocumento + " FROM Documento WHERE Documento.DocumentoID =" + IBD.GuidValor(pDocumentoID));
                var consulta = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID));
                dataWrapperDocumentacion.ListaDocumento = consulta.ToList();

                //DocumentoWebVinBR
                dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

                //DocumentoWebVinBRExtra
                dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursosExtra = mEntityContext.DocumentoWebVinBaseRecursosExtra.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

                //DocumentoWebAgCatTesauro
                dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

                //VersionDocumento
                dataWrapperDocumentacion.ListaVersionDocumento = mEntityContext.VersionDocumento.Where(version => version.DocumentoID.Equals(pDocumentoID) || version.DocumentoOriginalID.Equals(pDocumentoID)).ToList();
            }
            //DocumentoAtributoBiblio
            dataWrapperDocumentacion.ListaDocumentoAtributoBiblio = mEntityContext.DocumentoAtributoBiblio.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            //HistorialDocumento
            dataWrapperDocumentacion.ListaHistorialDocumento = mEntityContext.HistorialDocumento.Where(historial => historial.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoComentario
            dataWrapperDocumentacion.ListaDocumentoComentario = mEntityContext.DocumentoComentario.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            ////DocumentoGrupoUsuario
            dataWrapperDocumentacion.ListaDocumentoGrupoUsuario = mEntityContext.DocumentoGrupoUsuario.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoRolIdentidad
            dataWrapperDocumentacion.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(docRolIden => docRolIden.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoRolGrupoIdentidades
            dataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            //VotoDocumento
            dataWrapperDocumentacion.ListaVotoDocumento = mEntityContext.VotoDocumento.Where(voto => voto.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoTipologia
            dataWrapperDocumentacion.ListaDocumentoTipologia = mEntityContext.DocumentoTipologia.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            if (dataWrapperDocumentacion.ListaDocumento.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.Tipo.Equals((short)TiposDocumentacion.Encuesta)).ToList().Count > 0)
            {
                //DocumentoRespuesta
                dataWrapperDocumentacion.ListaDocumentoRespuesta = mEntityContext.DocumentoRespuesta.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

                //DocumentoRespuestaVoto
                dataWrapperDocumentacion.ListaDocumentoRespuestaVoto = mEntityContext.DocumentoRespuestaVoto.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();
            }

            if (pTraerBaseRecursos)
            {

                //BaseRecursos
                dataWrapperDocumentacion.ListaBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList().Select(doc => new BaseRecursos { BaseRecursosID = doc.BaseRecursosID }).Distinct().ToList();

                //BaseRecursosProyecto
                dataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRecProy => baseRecProy.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRecProy, docWebVin) => new
                {
                    BaseRecursosProyecto = baseRecProy,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList().Select(objeto => objeto.BaseRecursosProyecto).ToList();

                //BaseRecursosOrganizacion
                dataWrapperDocumentacion.ListaBaseRecursosOrganizacion = mEntityContext.BaseRecursosOrganizacion.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRecOrg => baseRecOrg.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRecOrg, docWebVin) => new
                {
                    BaseRecursosOrganizacion = baseRecOrg,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.BaseRecursosOrganizacion).ToList();

                //BaseRecursosUsuario
                dataWrapperDocumentacion.ListaBaseRecursosUsuario = mEntityContext.BaseRecursosUsuario.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRecUs => baseRecUs.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRecUs, docWebVin) => new
                {
                    BaseRecursosUsuario = baseRecUs,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.BaseRecursosUsuario).ToList();
            }
            pDataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
        }

        /// <summary>
        /// Carga toda la información de unos documentos menos las versiones del mismo.
        /// </summary>
        /// <param name="pDocumentosID">Identificadores de los documentos</param>
        /// <param name="dataWrapperDocumentacion">Dataset de documentación</param>
        /// <param name="pDatosCompletos">TRUE si debe cargarse todo del documento, FALSE si sólo los complementos(historial, versión, ect.)</param>
        /// <param name="pTraerBaseRecursos">Indica si se traen las BR o no</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos a la que pertenece el documento o Guid.Empty si se desea obtener de la que se publico o NULL si se desea traer todas</param>
        public void ObtenerDocumentosPorIDCargarTotal(List<Guid> pDocumentosID, DataWrapperDocumentacion pDataWrapperDocumentacion, bool pDatosCompletos, bool pTraerBaseRecursos, Guid? pBaseRecursosID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            if (pDatosCompletos)
            {
                //DbCommand commandsqlSelectDocumentoPorID = ObtenerComando(selectDocumento + " FROM Documento WHERE Documento.DocumentoID =" + IBD.GuidValor(pDocumentoID));
                var consulta = mEntityContext.Documento.Where(doc => pDocumentosID.Contains(doc.DocumentoID));
                dataWrapperDocumentacion.ListaDocumento = consulta.ToList();

                //DocumentoWebVinBR
                dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => pDocumentosID.Contains(doc.DocumentoID)).ToList();

                //DocumentoWebVinBRExtra
                dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursosExtra = mEntityContext.DocumentoWebVinBaseRecursosExtra.Where(doc => pDocumentosID.Contains(doc.DocumentoID)).ToList();

                //DocumentoWebAgCatTesauro
                dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(doc => pDocumentosID.Contains(doc.DocumentoID)).ToList();

                //VersionDocumento
                dataWrapperDocumentacion.ListaVersionDocumento = mEntityContext.VersionDocumento.Where(version => pDocumentosID.Contains(version.DocumentoID) || pDocumentosID.Contains(version.DocumentoOriginalID)).ToList();
            }
            //DocumentoAtributoBiblio
            dataWrapperDocumentacion.ListaDocumentoAtributoBiblio = mEntityContext.DocumentoAtributoBiblio.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();

            //HistorialDocumento
            dataWrapperDocumentacion.ListaHistorialDocumento = mEntityContext.HistorialDocumento.Where(historial => pDocumentosID.Contains(historial.DocumentoID)).ToList();

            //DocumentoComentario
            dataWrapperDocumentacion.ListaDocumentoComentario = mEntityContext.DocumentoComentario.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();

            //DocumentoGrupoUsuario
            dataWrapperDocumentacion.ListaDocumentoGrupoUsuario = mEntityContext.DocumentoGrupoUsuario.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();

            //DocumentoRolIdentidad
            dataWrapperDocumentacion.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(docRolIden => pDocumentosID.Contains(docRolIden.DocumentoID)).ToList();

            //DocumentoRolGrupoIdentidades
            dataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(doc => pDocumentosID.Contains(doc.DocumentoID)).ToList();

            //VotoDocumento
            dataWrapperDocumentacion.ListaVotoDocumento = mEntityContext.VotoDocumento.Where(voto => pDocumentosID.Contains(voto.DocumentoID)).ToList();

            //DocumentoTipologia
            dataWrapperDocumentacion.ListaDocumentoTipologia = mEntityContext.DocumentoTipologia.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();

            if (dataWrapperDocumentacion.ListaDocumento.Where(doc => pDocumentosID.Contains(doc.DocumentoID) && doc.Tipo.Equals((short)TiposDocumentacion.Encuesta)).ToList().Count > 0)
            {
                //DocumentoRespuesta
                dataWrapperDocumentacion.ListaDocumentoRespuesta = mEntityContext.DocumentoRespuesta.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();

                //DocumentoRespuestaVoto
                dataWrapperDocumentacion.ListaDocumentoRespuestaVoto = mEntityContext.DocumentoRespuestaVoto.Where(item => pDocumentosID.Contains(item.DocumentoID)).ToList();
            }

            if (pTraerBaseRecursos)
            {

                //BaseRecursos
                dataWrapperDocumentacion.ListaBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => pDocumentosID.Contains(doc.DocumentoID)).ToList().Select(doc => new BaseRecursos { BaseRecursosID = doc.BaseRecursosID }).Distinct().ToList();

                //BaseRecursosProyecto
                dataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRecProy => baseRecProy.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRecProy, docWebVin) => new
                {
                    BaseRecursosProyecto = baseRecProy,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => pDocumentosID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID)).ToList().Select(objeto => objeto.BaseRecursosProyecto).ToList();

                //BaseRecursosOrganizacion
                dataWrapperDocumentacion.ListaBaseRecursosOrganizacion = mEntityContext.BaseRecursosOrganizacion.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRecOrg => baseRecOrg.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRecOrg, docWebVin) => new
                {
                    BaseRecursosOrganizacion = baseRecOrg,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => pDocumentosID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID)).Select(objeto => objeto.BaseRecursosOrganizacion).ToList();

                //BaseRecursosUsuario
                dataWrapperDocumentacion.ListaBaseRecursosUsuario = mEntityContext.BaseRecursosUsuario.Join(mEntityContext.DocumentoWebVinBaseRecursos, baseRecUs => baseRecUs.BaseRecursosID, docWebVin => docWebVin.BaseRecursosID, (baseRecUs, docWebVin) => new
                {
                    BaseRecursosUsuario = baseRecUs,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Where(objeto => pDocumentosID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID)).Select(objeto => objeto.BaseRecursosUsuario).ToList();
            }
            pDataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
        }

        public DataWrapperDocumentacion ObtenerOpcionesEncuesta(Guid pDocumentoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //DocumentoRespuesta
            dataWrapperDocumentacion.ListaDocumentoRespuesta = mEntityContext.DocumentoRespuesta.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoRespuestaVoto
            dataWrapperDocumentacion.ListaDocumentoRespuestaVoto = mEntityContext.DocumentoRespuestaVoto.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene las vinculaciones de un recurso (DocumentoVin..).
        /// </summary>
        /// <param name="pDocumentosID">Lista de identificadores de documento</param>
        /// <param name="pTraerRelacionesInversas">Verdad si se deben traer los documentos que tienen una vinculación a los documentos de pDocumentosID</param>
        public DataWrapperDocumentacion ObtenerVinculacionesRecursos(List<Guid> pDocumentosID, bool pTraerRelacionesInversas = false)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            if (pDocumentosID != null && pDocumentosID.Count > 0)
            {
                if (pTraerRelacionesInversas)
                {
                    dataWrapperDocumentacion.ListaDocumentoVincDoc = mEntityContext.DocumentoVincDoc.Where(docVinDoc => pDocumentosID.Contains(docVinDoc.DocumentoVincID)).ToList();
                }
                else
                {
                    dataWrapperDocumentacion.ListaDocumentoVincDoc = mEntityContext.DocumentoVincDoc.Where(docVinDoc => pDocumentosID.Contains(docVinDoc.DocumentoID)).ToList();
                }
            }
            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Comprueba si el documento ya esta vincualdo o viceversa
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <param name="pDocumentoVincID">ID del documento a vincular</param>
        public bool EstaVinculadoDocumento(Guid pDocumentoID, Guid pDocumentoVincID)
        {
            return mEntityContext.DocumentoVincDoc.Any(doc => (doc.DocumentoID.Equals(pDocumentoID) && doc.DocumentoVincID.Equals(pDocumentoVincID)) || (doc.DocumentoID.Equals(pDocumentoVincID) && doc.DocumentoVincID.Equals(pDocumentoID)));
        }

        public string obtenerIDDesdeURI(string URIID) { return URIID.Substring(URIID.IndexOf("gnoss") + 6); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataWrapperDocumentacion"></param>
        /// <param name="pDocumentoID"></param>
        /// <param name="pInicio"></param>
        /// <param name="pFinal"></param>
        /// <param name="pNombreTablaCOMUNIDADES"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></return s>
        public void ObtenerListaRecursosRelacionados(DataWrapperDocumentacion pDataWrapperDocumentacion, FacetadoDS pFacetadoDS, Guid pDocumentoID, int pInicio, int pFinal, string pNombreTablaCOMUNIDADES, Guid pProyectoID)
        {
            // NO SE HACE
            if (pFacetadoDS.Tables["RecurosRelacionados"].Rows.Count > 0)
            {
                DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();

                List<Guid> listaDocumentosID = new List<Guid>();
                foreach (DataRow myrow in pFacetadoDS.Tables["RecurosRelacionados"].Rows)
                {
                    listaDocumentosID.Add(new Guid(obtenerIDDesdeURI((string)myrow[0])));
                }

                List<DocumentoConProyNombreCortoProyRelacionadoID> listaDocumentoConProyNombreCorto = mEntityContext.Documento.JoinDocumentoProyecto().Where(objeto => listaDocumentosID.Contains(objeto.Documento.DocumentoID) && !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion).Select(objeto => new DocumentoConProyNombreCortoProyRelacionadoID
                {
                    Documento = objeto.Documento,
                    ProyectoNombreCorto = objeto.Proyecto.NombreCorto,
                    ProyectoRelacionadoID = pProyectoID,
                    ProyectoRelacionadoNombreCorto = mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.NombreCorto).FirstOrDefault()
                }).ToList();

                docDW.ListaDocumentoConProyNombreCorto = listaDocumentoConProyNombreCorto;

                docDW.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().Where(objeto => listaDocumentosID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

                foreach (DataRow myrow in pFacetadoDS.Tables["RecurosRelacionados"].Rows)
                {
                    List<DocumentoConProyNombreCortoProyRelacionadoID> filasDoc = docDW.ListaDocumentoConProyNombreCorto.Where(docConNombreCorto => docConNombreCorto.Documento.DocumentoID.Equals(new Guid(obtenerIDDesdeURI((string)myrow[0])))).ToList();
                    if (filasDoc.Count > 0)
                    {
                        pDataWrapperDocumentacion.ListaDocumentoConProyNombreCorto.Add(filasDoc[0]);
                        // pDocumetacionDS.Documento.ImportRow(filasDoc[0]);

                        List<DocumentoWebVinBaseRecursos> filasDocWebVin = docDW.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(new Guid(obtenerIDDesdeURI((string)myrow[0])))).ToList();
                        if (filasDocWebVin.Count > 0)
                        {
                            pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Add(filasDocWebVin[0]);
                            //pDocumetacionDS.DocumentoWebVinBaseRecursos.ImportRow(filasDocWebVin[0]);
                        }
                    }
                }
                //pDataWrapperDocumentacion.AcceptChanges();
            }
        }

        /// <summary>
        /// Obtiene una lista de recursos para la vista prensa.
        /// </summary>
        /// <param name="pDocumentos">IDs de documento</param>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <returns>DataSet de documento con una lista de recursos para la vista prensa</return s>
        public List<DocumentoTieneImagenConNombresCortoProy> ObtenerListaRecursosPrensa(List<Guid> pDocumentos, Guid pProyectoID)
        {
            List<DocumentoTieneImagenConNombresCortoProy> listaDocumentoTieneImagenConNombresCortoProy = new List<DocumentoTieneImagenConNombresCortoProy>();
            if (pDocumentos.Count > 0)
            {
                listaDocumentoTieneImagenConNombresCortoProy = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => pDocumentos.Contains(objeto.Documento.DocumentoID) && !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.Documento.Borrador && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => new DocumentoTieneImagenConNombresCortoProy
                {
                    DocumentoID = objeto.Documento.DocumentoID,
                    OrganizacionID = objeto.Documento.OrganizacionID,
                    CompartirPermitido = objeto.Documento.CompartirPermitido,
                    Titulo = objeto.Documento.Titulo,
                    Tipo = objeto.Documento.Tipo,
                    Enlace = objeto.Documento.Enlace,
                    TipoEntidad = objeto.Documento.TipoEntidad,
                    NombreCategoriaDoc = objeto.Documento.NombreCategoriaDoc,
                    ProyectoID = objeto.Documento.ProyectoID,
                    Publico = objeto.Documento.Publico,
                    Borrador = objeto.Documento.Borrador,
                    CreadorEsAutor = objeto.Documento.CreadorEsAutor,
                    Protegido = objeto.Documento.Protegido,
                    UltimaVersion = objeto.Documento.UltimaVersion,
                    Eliminado = objeto.Documento.Eliminado,
                    NumeroComentariosPublicos = objeto.Documento.NumeroComentariosPublicos,
                    ProyectoRelacionadoID = pProyectoID,
                    TieneImagen = mEntityContext.ColaDocumento.Any(cola => objeto.Documento.DocumentoID.Equals(cola.DocumentoID)),
                    ProyectoRelacionadoNombreCorto = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proy => proy.NombreCorto).FirstOrDefault()
                }).ToList();
            }

            return listaDocumentoTieneImagenConNombresCortoProy;
        }



        #region Recursos Vinculados

        /// <summary>
        /// Obtiene el numero de documentos vinculados a un documento por ID.
        /// </summary>

        /// <param name="pDocumentoID">DocumentoID del que se traerán los vinculos</param>
        /// <param name="pPerfilActualID">PerfilID actual</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Fin paginación</param>
        /// <returns>Documento vinculados que puede ver el perfil actual</return s>
        public int ObtenerNumeroDocumentosVinculadosDocuemntoPorID(Guid pDocumentoID)
        {
            return mEntityContext.DocumentoVincDoc.Count(vinDoc => vinDoc.DocumentoID.Equals(pDocumentoID));
        }

        /// <param name="pIdentidadID">IdentidadID del que se traerán los vinculos</param>
        /// <returns>Obtener los recursos publicados</return s>
        public int ObtenerNumeroRecursosPublicados(Guid pIdentidadID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Count(vinDoc => vinDoc.IdentidadPublicacionID.HasValue && vinDoc.IdentidadPublicacionID.Value.Equals(pIdentidadID));
        }

        /// <summary>
        /// Comprueba se un recurso tiene algún recurso vinculado
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a comprobar</param>
        /// <returns>True si el documento tiene algún recurso vinculado, false en caso contrario</return s>
        public bool TieneDocumentoDocumentosVinculados(Guid pDocumentoID)
        {
            return mEntityContext.DocumentoVincDoc.Join(mEntityContext.Documento, vinDoc => vinDoc.DocumentoID, doc => doc.DocumentoID, (vinDoc, doc) => new
            {
                DocumentoVinDoc = vinDoc,
                Documento = doc
            }).Where(objeto => objeto.DocumentoVinDoc.DocumentoID.Equals(pDocumentoID) && !objeto.Documento.Eliminado).Any();
        }

        /// <summary>
        /// Obtiene los documento vinculados que puede ver el perfil actual.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">proyecto id del documento</param>
        /// <param name="pDocumentoID">DocumentoID del que se traerán los vinculos</param>
        /// <param name="pPerfilActualID">PerfilID actual</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Fin paginación</param>
        /// <returns>Documento vinculados que puede ver el perfil actual</return s>
        public int ObtenerDocumentosVinculadosDocumento(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, Guid pDocumentoID, Guid pPerfilActualID, int pInicio, int pLimite)
        {

            int cuantos = 0;

            if (pPerfilActualID != UsuarioAD.Invitado)
            {
                cuantos = mEntityContext.DocumentoVincDoc.JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().GroupJoin(mEntityContext.DocumentoRolIdentidad, item => new { DocumentoID = item.Documento.DocumentoID, PerfilID = pPerfilActualID }, documentoRolIdentidad => new { DocumentoID = documentoRolIdentidad.DocumentoID, PerfilID = documentoRolIdentidad.PerfilID }, (item, documentoRolIdentidad) => new
                {
                    BaseRecursosProyecto = item.BaseRecursosProyecto,
                    Documento = item.Documento,
                    DocumentoVinDoc = item.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = documentoRolIdentidad
                }).SelectMany(item => item.DocumentoRolIdentidad.DefaultIfEmpty(), (x, y) => new
                {
                    Item = x,
                    DocumentoRolIdentidad = y
                }).GroupJoin(mEntityContext.DocumentoRolGrupoIdentidades, item => item.Item.Documento.DocumentoID, documentoRolGrupoIdentidades => documentoRolGrupoIdentidades.DocumentoID, (item, documentoRolGrupoIdentidades) => new
                {
                    BaseRecursosProyecto = item.Item.BaseRecursosProyecto,
                    Documento = item.Item.Documento,
                    DocumentoVinDoc = item.Item.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = item.Item.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = documentoRolGrupoIdentidades
                }).SelectMany(item => item.DocumentoRolGrupoIdentidades.DefaultIfEmpty(), (x, y) => new
                {
                    BaseRecursosProyecto = x.BaseRecursosProyecto,
                    Documento = x.Documento,
                    DocumentoVinDoc = x.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = x.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = x.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = y
                }).GroupJoin(mEntityContext.GrupoIdentidadesParticipacion, item => item.DocumentoRolGrupoIdentidades.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (item, grupoIdentidadesParticipacion) => new
                {
                    BaseRecursosProyecto = item.BaseRecursosProyecto,
                    Documento = item.Documento,
                    DocumentoVinDoc = item.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                }).SelectMany(item => item.GrupoIdentidadesParticipacion.DefaultIfEmpty(), (x, y) => new
                {
                    BaseRecursosProyecto = x.BaseRecursosProyecto,
                    Documento = x.Documento,
                    DocumentoVinDoc = x.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = x.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = x.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = x.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = y
                }).GroupJoin(mEntityContext.Identidad, item => new { item.GrupoIdentidadesParticipacion.IdentidadID, PerfilID = pPerfilActualID }, identidad => new { identidad.IdentidadID, identidad.PerfilID }, (item, identidad) => new
                {
                    BaseRecursosProyecto = item.BaseRecursosProyecto,
                    Documento = item.Documento,
                    DocumentoVinDoc = item.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = item.GrupoIdentidadesParticipacion,
                    Identidad = identidad
                }).SelectMany(item => item.Identidad.DefaultIfEmpty(), (x, y) => new
                {
                    BaseRecursosProyecto = x.BaseRecursosProyecto,
                    Documento = x.Documento,
                    DocumentoVinDoc = x.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = x.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = x.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = x.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = x.GrupoIdentidadesParticipacion,
                    Identidad = y
                }).Where(item => item.DocumentoVinDoc.DocumentoID.Equals(pDocumentoID) && item.Documento.Eliminado.Equals(false) && item.DocumentoWebVinBaseRecursos.Eliminado.Equals(false) && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && (item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(false) || item.Identidad.PerfilID.Equals(pPerfilActualID))).Select(item => item.DocumentoVinDoc.DocumentoVincID).Distinct().Count();
            }
            else
            {
                cuantos = mEntityContext.DocumentoVincDoc.JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.DocumentoVinDoc.DocumentoID.Equals(pDocumentoID) && item.Documento.Eliminado.Equals(false) && item.DocumentoWebVinBaseRecursos.Eliminado.Equals(false) && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(false) && item.Documento.Visibilidad.Equals((short)VisibilidadDocumento.Todos)).Select(item => item.DocumentoVinDoc.DocumentoVincID).Distinct().Count();
            }

            if (cuantos > 0 && pDataWrapperDocumentacion != null)
            {
                DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

                if (pPerfilActualID != UsuarioAD.Invitado)
                {
                    dataWrapperDocumentacion.ListaDocumentoVincDoc = mEntityContext.DocumentoVincDoc.JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().GroupJoin(mEntityContext.DocumentoRolIdentidad, item => new { DocumentoID = item.Documento.DocumentoID, PerfilID = pPerfilActualID }, documentoRolIdentidad => new { DocumentoID = documentoRolIdentidad.DocumentoID, PerfilID = documentoRolIdentidad.PerfilID }, (item, documentoRolIdentidad) => new
                    {
                        BaseRecursosProyecto = item.BaseRecursosProyecto,
                        Documento = item.Documento,
                        DocumentoVinDoc = item.DocumentoVinDoc,
                        DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                        DocumentoRolIdentidad = documentoRolIdentidad
                    }).SelectMany(item => item.DocumentoRolIdentidad.DefaultIfEmpty(), (x, y) => new
                    {
                        Item = x,
                        DocumentoRolIdentidad = y
                    }).GroupJoin(mEntityContext.DocumentoRolGrupoIdentidades, item => item.Item.Documento.DocumentoID, documentoRolGrupoIdentidades => documentoRolGrupoIdentidades.DocumentoID, (item, documentoRolGrupoIdentidades) => new
                    {
                        BaseRecursosProyecto = item.Item.BaseRecursosProyecto,
                        Documento = item.Item.Documento,
                        DocumentoVinDoc = item.Item.DocumentoVinDoc,
                        DocumentoWebVinBaseRecursos = item.Item.DocumentoWebVinBaseRecursos,
                        DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                        DocumentoRolGrupoIdentidades = documentoRolGrupoIdentidades
                    }).SelectMany(item => item.DocumentoRolGrupoIdentidades.DefaultIfEmpty(), (x, y) => new
                    {
                        BaseRecursosProyecto = x.BaseRecursosProyecto,
                        Documento = x.Documento,
                        DocumentoVinDoc = x.DocumentoVinDoc,
                        DocumentoWebVinBaseRecursos = x.DocumentoWebVinBaseRecursos,
                        DocumentoRolIdentidad = x.DocumentoRolIdentidad,
                        DocumentoRolGrupoIdentidades = y
                    }).GroupJoin(mEntityContext.GrupoIdentidadesParticipacion, item => item.DocumentoRolGrupoIdentidades.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (item, grupoIdentidadesParticipacion) => new
                    {
                        BaseRecursosProyecto = item.BaseRecursosProyecto,
                        Documento = item.Documento,
                        DocumentoVinDoc = item.DocumentoVinDoc,
                        DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                        DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                    }).SelectMany(item => item.GrupoIdentidadesParticipacion.DefaultIfEmpty(), (x, y) => new
                    {
                        BaseRecursosProyecto = x.BaseRecursosProyecto,
                        Documento = x.Documento,
                        DocumentoVinDoc = x.DocumentoVinDoc,
                        DocumentoWebVinBaseRecursos = x.DocumentoWebVinBaseRecursos,
                        DocumentoRolIdentidad = x.DocumentoRolIdentidad,
                        DocumentoRolGrupoIdentidades = x.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesParticipacion = y
                    }).GroupJoin(mEntityContext.Identidad, item => new { item.GrupoIdentidadesParticipacion.IdentidadID, PerfilID = pPerfilActualID }, identidad => new { identidad.IdentidadID, identidad.PerfilID }, (item, identidad) => new
                    {
                        BaseRecursosProyecto = item.BaseRecursosProyecto,
                        Documento = item.Documento,
                        DocumentoVinDoc = item.DocumentoVinDoc,
                        DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                        DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesParticipacion = item.GrupoIdentidadesParticipacion,
                        Identidad = identidad
                    }).SelectMany(item => item.Identidad.DefaultIfEmpty(), (x, y) => new
                    {
                        BaseRecursosProyecto = x.BaseRecursosProyecto,
                        Documento = x.Documento,
                        DocumentoVinDoc = x.DocumentoVinDoc,
                        DocumentoWebVinBaseRecursos = x.DocumentoWebVinBaseRecursos,
                        DocumentoRolIdentidad = x.DocumentoRolIdentidad,
                        DocumentoRolGrupoIdentidades = x.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesParticipacion = x.GrupoIdentidadesParticipacion,
                        Identidad = y
                    }).Where(item => item.DocumentoVinDoc.DocumentoID.Equals(pDocumentoID) && item.Documento.Eliminado.Equals(false) && item.DocumentoWebVinBaseRecursos.Eliminado.Equals(false) && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && (item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(false) || item.Identidad.PerfilID.Equals(pPerfilActualID))).Select(item => item.DocumentoVinDoc).OrderBy(item => item.Fecha).Skip(pInicio).Take(pLimite - pInicio).ToList();
                }
                else
                {
                    dataWrapperDocumentacion.ListaDocumentoVincDoc = mEntityContext.DocumentoVincDoc.JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.DocumentoVinDoc.DocumentoID.Equals(pDocumentoID) && item.Documento.Eliminado.Equals(false) && item.DocumentoWebVinBaseRecursos.Eliminado.Equals(false) && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(false) && item.Documento.Visibilidad.Equals((short)VisibilidadDocumento.Todos)).Select(item => item.DocumentoVinDoc).OrderBy(item => item.Fecha).Skip(pInicio).Take(pLimite - pInicio).ToList();
                }

                List<Guid> listaDocumentosVinculados = new List<Guid>();
                foreach (DocumentoVincDoc docVin in dataWrapperDocumentacion.ListaDocumentoVincDoc)
                {
                    listaDocumentosVinculados.Add(docVin.DocumentoVincID);
                }

                dataWrapperDocumentacion.Merge(ObtenerDocumentosPorID(listaDocumentosVinculados, true));

                //Traigo los complementarios vinculados:
                dataWrapperDocumentacion.ListaDocumentoVincDoc = mEntityContext.DocumentoVincDoc.Where(item => item.DocumentoVincID.Equals(pDocumentoID)).ToList();

                pDataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
            }

            return cuantos;
        }

        /// <summary>
        /// Obtiene los documento vinculados que puede ver el perfil actual.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">proyecto id del documento</param>
        /// <param name="pDocumentoID">DocumentoID del que se traerán los vinculos</param>
        /// <param name="pPerfilActualID">PerfilID actual</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Fin paginación</param>
        /// <returns>Documento vinculados que puede ver el perfil actual</return s>
        public Dictionary<Guid, List<Guid>> ObtenerListaDocumentosVinculadosDocumento(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, Guid pDocumentoID, Guid pPerfilActualID, int pInicio, int pLimite, out int pNumVinculados)
        {
            Dictionary<Guid, List<Guid>> listaClavesRecursosProyecto = new Dictionary<Guid, List<Guid>>();
            List<DocumentoVinculadoDocumento> listaDocumentoVinculadoDocumento = new List<DocumentoVinculadoDocumento>();
            pInicio--;
            if (pPerfilActualID != UsuarioAD.Invitado)
            {
                var query = mEntityContext.DocumentoVincDoc.JoinDocumento().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().GroupJoin(mEntityContext.DocumentoRolIdentidad, item => new { DocumentoID = item.Documento2.DocumentoID, PerfilID = pPerfilActualID }, documentoRolIdentidad => new { DocumentoID = documentoRolIdentidad.DocumentoID, PerfilID = documentoRolIdentidad.PerfilID }, (item, documentoRolIdentidad) => new
                {
                    Item = item,
                    DocumentoRolIdentidad = documentoRolIdentidad
                }).SelectMany(item => item.DocumentoRolIdentidad.DefaultIfEmpty(), (x, y) => new
                {
                    BaseRecursosProyecto = x.Item.BaseRecursosProyecto,
                    Documento2 = x.Item.Documento2,
                    Documento = x.Item.Documento,
                    DocumentoVinDoc = x.Item.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = x.Item.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = y
                }).GroupJoin(mEntityContext.DocumentoRolGrupoIdentidades, item => item.Documento2.DocumentoID, documentoRolGrupoIdentidades => documentoRolGrupoIdentidades.DocumentoID, (item, documentoRolGrupoIdentidades) => new
                {
                    Item = item,
                    DocumentoRolGrupoIdentidades = documentoRolGrupoIdentidades
                }).SelectMany(item => item.DocumentoRolGrupoIdentidades.DefaultIfEmpty(), (x, y) => new
                {
                    BaseRecursosProyecto = x.Item.BaseRecursosProyecto,
                    Documento = x.Item.Documento,
                    Documento2 = x.Item.Documento2,
                    DocumentoVinDoc = x.Item.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = x.Item.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = x.Item.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = y
                }).GroupJoin(mEntityContext.GrupoIdentidadesParticipacion, item => item.DocumentoRolGrupoIdentidades.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (item, grupoIdentidadesParticipacion) => new
                {
                    Item = item,
                    GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                }).SelectMany(item => item.GrupoIdentidadesParticipacion.DefaultIfEmpty(), (x, y) => new
                {
                    BaseRecursosProyecto = x.Item.BaseRecursosProyecto,
                    Documento = x.Item.Documento,
                    Documento2 = x.Item.Documento2,
                    DocumentoVinDoc = x.Item.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = x.Item.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = x.Item.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = x.Item.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = y
                }).GroupJoin(mEntityContext.Identidad, item => new { item.GrupoIdentidadesParticipacion.IdentidadID, PerfilID = pPerfilActualID }, identidad => new { identidad.IdentidadID, identidad.PerfilID }, (item, identidad) => new
                {
                    Item = item,
                    Identidad = identidad
                }).SelectMany(item => item.Identidad.DefaultIfEmpty(), (x, y) => new
                {
                    BaseRecursosProyecto = x.Item.BaseRecursosProyecto,
                    Documento = x.Item.Documento,
                    Documento2 = x.Item.Documento2,
                    DocumentoVinDoc = x.Item.DocumentoVinDoc,
                    DocumentoWebVinBaseRecursos = x.Item.DocumentoWebVinBaseRecursos,
                    DocumentoRolIdentidad = x.Item.DocumentoRolIdentidad,
                    DocumentoRolGrupoIdentidades = x.Item.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = x.Item.GrupoIdentidadesParticipacion,
                    Identidad = y
                }).Where(item => item.DocumentoVinDoc.DocumentoID.Equals(pDocumentoID) && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !item.Documento2.Eliminado && (!item.DocumentoWebVinBaseRecursos.PrivadoEditores || item.DocumentoRolIdentidad.PerfilID.Equals(pPerfilActualID) || item.Identidad.PerfilID.Equals(pPerfilActualID))).Select(item => new DocumentoVinculadoDocumento
                {
                    DocumentoID = item.Documento2.DocumentoID,
                    ProyectoID = item.BaseRecursosProyecto.ProyectoID,
                    Fecha = item.DocumentoVinDoc.Fecha
                }).OrderByDescending(item => item.Fecha).Skip(pInicio).Take(pLimite - pInicio);

                listaDocumentoVinculadoDocumento = query.ToList();

            }
            else
            {
                listaDocumentoVinculadoDocumento = mEntityContext.DocumentoVincDoc.JoinDocumento().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.DocumentoVinDoc.DocumentoID.Equals(pDocumentoID) && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !item.Documento2.Eliminado && !item.DocumentoWebVinBaseRecursos.PrivadoEditores && item.Documento.Visibilidad.Equals((short)VisibilidadDocumento.Todos)).Select(item => new DocumentoVinculadoDocumento
                {
                    DocumentoID = item.Documento2.DocumentoID,
                    ProyectoID = item.BaseRecursosProyecto.ProyectoID,
                    Fecha = item.DocumentoVinDoc.Fecha
                }).OrderByDescending(item => item.Fecha).Skip(pInicio).Take(pLimite - pInicio).ToList();
            }

            List<Guid> listaDocumentos = new List<Guid>();

            foreach (DocumentoVinculadoDocumento documento in listaDocumentoVinculadoDocumento)
            {
                Guid documentoID = documento.DocumentoID;
                Guid? proyectoID = documento.ProyectoID;

                if (!listaClavesRecursosProyecto.ContainsKey(proyectoID.Value))
                {
                    listaClavesRecursosProyecto.Add(proyectoID.Value, new List<Guid>());
                }

                if (!listaClavesRecursosProyecto[proyectoID.Value].Contains(documentoID))
                {
                    listaClavesRecursosProyecto[proyectoID.Value].Add(documentoID);
                }

                if (!listaDocumentos.Contains(documentoID))
                {
                    listaDocumentos.Add(documentoID);
                }
            }

            pNumVinculados = pLimite - pInicio;

            if (listaDocumentos.Any())
            {
                DataWrapperDocumentacion docDW = ObtenerDocumentosPorID(listaDocumentos, true);

                docDW.ListaDocumentoVincDoc = mEntityContext.DocumentoVincDoc.Where(item => listaDocumentos.Contains(item.DocumentoID)).Distinct().ToList();

                pDataWrapperDocumentacion.Merge(docDW);
            }
            return listaClavesRecursosProyecto;
        }


        /// <summary>
        /// Desvincula directamente 2 recursos de la BD.
        /// </summary>
        /// <param name="pDocumento1ID">ID del documento 1</param>
        /// <param name="pDocumento2ID">ID del documento 2</param>
        /// <returns>TRUE si se han desvinculado correctamente, FALSE en caso contrario</return s>
        public bool DesvincularRecursos(Guid pDocumento1ID, Guid pDocumento2ID)
        {
            try
            {
                int contador = 0;
                List<DocumentoVincDoc> ListaDocumentosVincDoc = mEntityContext.DocumentoVincDoc.Where(item => (item.DocumentoID.Equals(pDocumento1ID) && item.DocumentoVincID.Equals(pDocumento2ID)) || (item.DocumentoID.Equals(pDocumento2ID) && item.DocumentoVincID.Equals(pDocumento1ID))).ToList();

                foreach (DocumentoVincDoc documentoVincDoc in ListaDocumentosVincDoc)
                {
                    mEntityContext.Entry(documentoVincDoc).State = EntityState.Deleted;
                    contador++;
                }

                ActualizarBaseDeDatosEntityContext();
                return contador == 2;
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Obtiene los atributos de las ficha bibliograficas de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentoID">Lista de documentos</param>
        /// <param name="pDocumentacionDW">DataSet de documentación</param>
        public void ObtenerFichaBibliograficaDocumentos(List<Guid> pListaDocumentoID, DataWrapperDocumentacion pDocumentacionDW)
        {
            //TODO probar --> Tabla DocumentoAtributoBiblio vacía
            pDocumentacionDW.ListaDocumentoAtributoBiblio = mEntityContext.DocumentoAtributoBiblio.Where(item => pListaDocumentoID.Contains(item.DocumentoID)).ToList();
        }

        /// <summary>
        /// Obtiene todos los documentos que están vinculados a un serie de categorias (y las relaciones con otras categorías).
        /// </summary>
        /// <param name="pListaCategorias">Lista con las categorias a las que están agregados los documentos</param>
        /// <param name="pTesauroID">Identificador del tesauro al que pertenecen las categorías</param>
        /// <returns>DataSet de documentación con los documentos</return s>
        public DataWrapperDocumentacion ObtenerVinculacionDocumentosDeCategoriasTesauro(List<Guid> pListaCategorias, Guid pTesauroID)
        {//Probar con mas de 3000 recursos
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            if (pListaCategorias.Count > 0)
            {
                dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(agCatTes => pListaCategorias.Contains(agCatTes.CategoriaTesauroID) && agCatTes.TesauroID.Equals(pTesauroID)).ToList();

                if (dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Count > 0)
                {
                    List<Guid> listaDocumentos = dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Select(ag => ag.DocumentoID).Distinct().ToList();
                    dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Union(mEntityContext.DocumentoWebAgCatTesauro.Where(doc => listaDocumentos.Contains(doc.DocumentoID) && doc.TesauroID.Equals(pTesauroID))).ToList();
                }
            }
            return dataWrapperDocumentacion;
        }


        /// <summary>
        /// Obtiene un array cuyo primer elemento es la identidad del creador del recurso  la segunda componente es el usuario
        /// </summary>
        /// <param name="pRecursoID">Recurso que busca</param>        
        /// <returns>Array cuyo primer elemento es el perfil de la identidad del creador del recurso  la segunda componente es el usuario</return s>
        public List<Guid> ObtenerPerfilesIDEstaCompartidoYEliminadoRecurso(Guid pRecursoID)
        {
            List<Guid> listaPerfiles = new List<Guid>();

            listaPerfiles = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.Identidad, objeto => new { IdentidadID = objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value }, identidad => new
            { IdentidadID = identidad.IdentidadID }, (objeto, identidad) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Identidad = identidad
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.HasValue && (objeto.DocumentoWebVinBaseRecursos.Eliminado || objeto.Documento.Eliminado) && objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pRecursoID)).Select(objeto => objeto.Identidad.PerfilID).ToList();

            return listaPerfiles;
        }

        /// <summary>
        /// Obtiene un Guid que es la organizacion del creador del recurso(Guid.Empty si no es de Organizacion) 
        /// </summary>
        /// <param name="pRecursoID">Recurso que busca</param>        
        /// <returns>Guid que es la organizacion del creador del recurso(Guid.Empty si no es de Organizacion) </return s>
        public Guid ObtenerOrganizacionPublicadorIDdeRecurso(Guid pRecursoID)
        {
            Guid orgID = Guid.Empty;

            EntityModel.Models.IdentidadDS.Perfil perfil = mEntityContext.Perfil.JoinPerfilIdentidad().JoinDocumento().Where(objeto => objeto.Documento.DocumentoID.Equals(pRecursoID)).Select(objeto => objeto.Perfil).FirstOrDefault();
            if (perfil != null && perfil.OrganizacionID.HasValue)
            {
                orgID = perfil.OrganizacionID.Value;
            }


            return orgID;
        }

        /// <summary>
        /// Obtiene un array cuyo primer elemento es la identidad del creador del recurso  la segunda componente es la organizacion que lo tiene en su base de recursos de organizacion
        /// </summary>
        /// <param name="pRecursoID">Recurso que busca</param>        
        /// <returns>Array cuyo primer elemento es la identidad del creador del recurso  la segunda componente es la organizacion que lo tiene en su base de recursos de organizacion</return s>
        public Dictionary<Guid, Guid> ObtenerIdentidadyOrganizacionIDdeRecurso(Guid pRecursoID)
        {

            Dictionary<Guid, Guid> DicIdOrg = new Dictionary<Guid, Guid>();

            var consulta = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosOrganizacion().Where(objeto => objeto.Documento.DocumentoID.Equals(pRecursoID)).Select(objeto => new
            {
                IdentidadPublicacionID = objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID,
                OrganizacionID = objeto.BaseRecursoOrganizacion.OrganizacionID
            });
            foreach (var fila in consulta)
            {
                if (fila.IdentidadPublicacionID.HasValue)
                {
                    DicIdOrg.Add(fila.IdentidadPublicacionID.Value, fila.OrganizacionID);
                }

            }


            return DicIdOrg;
        }

        /// <summary>
        /// Devuelve una lista con los perfiles que tienen acceso a algun recurso privado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista con los perfiles</return s>
        public List<Guid> ObtenerPerfilesConRecursosPrivados(Guid pProyectoID)
        {
            List<Guid> listaPerfiles = mEntityContext.DocumentoRolIdentidad.JoinDocumentoRolIdentidadDocumentoWebVinBaseRecursos().JoinDocumentoRolIdentidadDocumentoWebVinBaseRecursos().Where(objeto => objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoRolIdentidad.PerfilID).Distinct().ToList();

            return listaPerfiles;
        }



        /// <summary>
        /// Devuelve una lista con los perfiles que tienen acceso a algun debate privado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista con los perfiles</return s>
        public List<Guid> ObtenerPerfilesConDebatesPrivados(Guid pProyectoID)
        {
            List<Guid> listaPerfiles = mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(objeto => objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Eliminado).OrderBy(objeto => objeto.DocumentoRolIdentidad.PerfilID).Select(objeto => objeto.DocumentoRolIdentidad.PerfilID).ToList();

            return listaPerfiles;
        }

        public bool TienePerfilRecursosPrivadosEnComunConElPerfilPagina(Guid? pProyectoID, Guid pPerfilActualID, Guid pPerfilPaginaID)
        {
            // Documentos del proyecto de los que el perfil actual es editor
            var consulta1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinDocumentoRolIdentidad().JoinIdentidad().Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilActualID) && (objeto.DocumentoWebVinBaseRecursos.PrivadoEditores || objeto.Documento.PrivadoEditores) && !objeto.Documento.Eliminado && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Documento.UltimaVersion);

            if (pProyectoID.HasValue)
            {
                consulta1 = consulta1.Where(objeto => objeto.Identidad.ProyectoID.Equals(pProyectoID.Value));
            }

            // Documentos publicados/compartidos por el perfil de la página en el proyecto que son privados
            var consulta2 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilPaginaID) && (objeto.DocumentoWebVinBaseRecursos.PrivadoEditores || objeto.Documento.PrivadoEditores) && !objeto.Documento.Eliminado && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Documento.UltimaVersion);

            if (pProyectoID.HasValue)
            {
                consulta2 = consulta2.Where(objeto => objeto.Identidad.ProyectoID.Equals(pProyectoID.Value));
            }

            List<Guid> listaDocumentoID = consulta1.Select(objeto => objeto.Documento.DocumentoID).Intersect(consulta2.Select(objeto => objeto.Documento.DocumentoID)).ToList();

            return (listaDocumentoID.Count > 0);
        }


        /// <summary>
        /// Carga los tipos de fichas bibliograficas.
        /// </summary>
        /// <param name="pDocumentacionDS">DataSet de documentacion</param>
        public void ObtenerTiposFichaBibliografica(DataWrapperDocumentacion pDocumentacionDW)
        {
            pDocumentacionDW.ListaFichaBibliografica = mEntityContext.FichaBibliografica.ToList();

            pDocumentacionDW.ListaAtributoFichaBibliografica = mEntityContext.AtributoFichaBibliografica.ToList();
        }

        /// <summary>
        /// Obtiene el tipo de un documento
        /// </summary>
        /// <param name="pDocumentoID">DocumentoID</param>
        public TiposDocumentacion ObtenerTipoDocumentoPorDocumentoID(Guid pDocumentoID)
        {
            short tipo = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).Select(documento => documento.Tipo).ToList().FirstOrDefault();
            return (TiposDocumentacion)tipo;
        }

        /// <summary>
        /// Obtiene los tuipos de varios documentos
        /// </summary>
        /// <param name="pListaDocumentoID">Lista de IDs de documentos</param>
        public Dictionary<Guid, TiposDocumentacion> ObtenerTiposDocumentosPorDocumentosID(List<Guid> pListaDocumentoID)
        {
            Dictionary<Guid, TiposDocumentacion> listaDocTipo = new Dictionary<Guid, TiposDocumentacion>();
            if (pListaDocumentoID.Count > 0)
            {
                var consulta = mEntityContext.Documento.Where(documento => pListaDocumentoID.Contains(documento.DocumentoID)).Select(documento => new { documento.DocumentoID, documento.Tipo }).ToList();
                foreach (var fila in consulta)
                {
                    Guid documentoID = fila.DocumentoID;
                    short tipo = fila.Tipo;
                    listaDocTipo.Add(documentoID, (TiposDocumentacion)tipo);
                }
            }

            return listaDocTipo;
        }
        /// <summary>
        /// Devuelve un dictionary con los enlaces y un booleano que determina si existe o no
        /// </summary>
        /// <param name="pListaEnlaces">Lista de enlaces para comprobar</param>
        /// <param name="pBaseRecursosID">Base de recursos en la que buscar</param>
        /// <returns>dictionary con los enlaces y un booleano que determina si existe o no</return s>
        public Dictionary<string, bool> DocumentosRepetidosEnlaces(List<string> pListaEnlaces, Guid pBaseRecursosID)
        {
            Dictionary<string, bool> listaEnlaces = new Dictionary<string, bool>();

            if (pListaEnlaces.Count > 0)
            {
                var consulta = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && objeto.DocumentoWebVinBaseRecursos.BaseRecursosID.Equals(pBaseRecursosID) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && pListaEnlaces.Contains(objeto.Documento.Enlace)).Select(doc => doc.Documento.Enlace).ToList();

                foreach (var fila in consulta)
                {
                    if (!listaEnlaces.ContainsKey(fila))
                    {
                        listaEnlaces.Add(fila, true);
                    }
                }

                foreach (string enlace in pListaEnlaces)
                {
                    if (!listaEnlaces.ContainsKey(enlace))
                    {
                        listaEnlaces.Add(enlace, false);
                    }
                }
            }

            return listaEnlaces;
        }

        /// <summary>
        /// Comprueba si el título o el enlace de un doc están repetidos.
        /// </summary>
        /// <param name="pEnlace">Enlace del doc</param>
        /// <param name="pTitulo">Titulo del doc</param>
        /// <param name="pDocumentoID">Identificador del documento que se está revisando</param>
        /// <param name="pBaseRecursosID">Identificador de base de recursos</param>
        /// <param name="pDocumentoRepetidoID">Identificador del documento repetido</param>
        /// <returns>0 si no hay repetidos, 1 si se repite el título, 2 si se repite el enlace, 3 si se repiten ambos</return s>
        public int DocumentoRepetidoTituloEnlace(string pTitulo, string pEnlace, Guid pDocumentoID, Guid pBaseRecursosID, out Guid pDocumentoRepetidoID)
        {
            int codigo = 0;
            string sqlConsulta = sqlRepeticionTituloDocumento;
            pDocumentoRepetidoID = Guid.Empty;

            if (pTitulo != null && pTitulo != "")
            {
                List<Guid> documentoTit = null;

                if (pBaseRecursosID != Guid.Empty)
                {
                    sqlConsulta = sqlRepeticionTituloDocumentoDeBR;
                    documentoTit = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(objeto => objeto.Documento.Titulo.Equals(pTitulo) && !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && objeto.DocumentoWebVinBaseRecursos.BaseRecursosID.Equals(pBaseRecursosID) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores).Select(objeto => objeto.Documento.DocumentoID).ToList();
                }
                else
                {
                    documentoTit = mEntityContext.Documento.Where(doc => doc.Titulo.Equals(pTitulo) && !doc.Eliminado && doc.UltimaVersion).Select(doc => doc.DocumentoID).ToList();
                }

                foreach (Guid fila in documentoTit)
                {
                    if (pDocumentoID == Guid.Empty || pDocumentoID != fila)
                    {
                        pDocumentoRepetidoID = fila;
                        codigo = 1;//Título repetido
                        break;
                    }
                }
            }

            if (pEnlace != null && pEnlace != "")
            {
                List<Guid> documentoEn1 = null;
                sqlConsulta = sqlRepeticionEnlaceDocumento;
                if (pEnlace.IndexOf("http://") == 0)
                {
                    if (pBaseRecursosID != Guid.Empty)
                    {
                        documentoEn1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(objeto => (objeto.Documento.Enlace.Equals(pEnlace) || objeto.Documento.Enlace.Equals(pEnlace.Substring(7))) && !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && objeto.DocumentoWebVinBaseRecursos.BaseRecursosID.Equals(pBaseRecursosID) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores).Select(objeto => objeto.Documento.DocumentoID).ToList();
                    }
                    else
                    {
                        documentoEn1 = mEntityContext.Documento.Where(doc => (doc.Enlace.Equals(pEnlace) || doc.Enlace.Equals(pEnlace.Substring(7))) && !doc.Eliminado && doc.UltimaVersion).Select(doc => doc.DocumentoID).ToList();
                    }

                }
                else if (pBaseRecursosID != Guid.Empty)
                {
                    documentoEn1 = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(objeto => objeto.Documento.Enlace.Equals(pEnlace) && !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && objeto.DocumentoWebVinBaseRecursos.BaseRecursosID.Equals(pBaseRecursosID) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores).Select(objeto => objeto.Documento.DocumentoID).ToList();
                }
                else
                {
                    documentoEn1 = mEntityContext.Documento.Where(doc => doc.Enlace.Equals(pEnlace) && !doc.Eliminado && doc.UltimaVersion).Select(doc => doc.DocumentoID).ToList();
                }

                foreach (Guid fila in documentoEn1)
                {
                    if (pDocumentoID == Guid.Empty || pDocumentoID != fila)
                    {
                        pDocumentoRepetidoID = fila;
                        if (codigo == 1)
                        {
                            codigo = 3;//Ambos repetidos
                        }
                        else
                        {
                            codigo = 2;//Enlace repetido
                        }

                        break;
                    }
                }
            }

            return codigo;
        }

        /// <summary>
        /// Comprueba si el título o el enlace de un doc están repetidos en las BRs especificadas.
        /// </summary>
        /// <param name="pEnlace">Enlace del doc</param>
        /// <param name="pTitulo">Titulo del doc</param>
        /// <param name="pDocumentoID">Identificador del documento que se está revisando o Guid.Empty si no se desea omitir ninguno</param>
        /// <param name="pBasesRecursosID">Lista con los identificadores de las bases de recursos</param>
        /// <param name="pDocumentoRepetidoID">Identificador del documento repetido</param>
        /// <returns>0 si no hay repetidos, 1 si se repite el título, 2 si se repite el enlace, 3 si se repiten ambos</return s>
        public int DocumentoRepetidoTituloEnlaceEnVariasBRs(string pTitulo, string pEnlace, Guid pDocumentoID, List<Guid> pBasesRecursosID, out Guid pDocumentoRepetidoID)
        {
            int codigo = 0;
            pDocumentoRepetidoID = Guid.Empty;

            if (pBasesRecursosID.Count > 0)
            {
                if (pTitulo != null && pTitulo != "")
                {
                    var comandRepetidoTitulo = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(objeto => objeto.Documento.Titulo.Equals(pTitulo) && objeto.Documento.UltimaVersion && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && pBasesRecursosID.Contains(objeto.DocumentoWebVinBaseRecursos.BaseRecursosID)).Select(objeto => objeto.Documento.DocumentoID).ToList();

                    for (int i = 0; i < comandRepetidoTitulo.Count && codigo == 0; i++)
                    {
                        if (pDocumentoID == Guid.Empty || pDocumentoID != comandRepetidoTitulo[i])
                        {
                            pDocumentoRepetidoID = comandRepetidoTitulo[i];
                            codigo = 1;//Título repetido
                            break;
                        }
                    }
                }


                if (pEnlace != null && pEnlace != "")
                {
                    var comandRepetidoEnlace = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(objeto => objeto.Documento.Enlace.Equals(pEnlace) && objeto.Documento.UltimaVersion && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && pBasesRecursosID.Contains(objeto.DocumentoWebVinBaseRecursos.BaseRecursosID)).Select(objeto => objeto.Documento.DocumentoID).ToList();

                    for (int i = 0; i < comandRepetidoEnlace.Count && (codigo == 0 || codigo == 1); i++)
                    {
                        if (pDocumentoID == Guid.Empty || pDocumentoID != comandRepetidoEnlace[i])
                        {
                            pDocumentoRepetidoID = comandRepetidoEnlace[i];

                            if (codigo == 1)
                            {
                                codigo = 3;//Ambos repetidos
                            }
                            else
                            {
                                codigo = 2;//Enlace repetido
                            }

                            break;
                        }
                    }
                }
            }

            return codigo;
        }

        /// <summary>
        /// Obtiene el historial de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        public void ObtenerHistorialDocumentoPorID(Guid pDocumentoID, DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            pDataWrapperDocumentacion.ListaHistorialDocumento = mEntityContext.HistorialDocumento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();
        }

        /// <summary>
        /// Obtienen si un proyecto tiene o no articulos wiki (no eliminados)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>True si tiene</return s>
        public bool TieneArticulosWikiProyecto(Guid pProyectoID)
        {
            return mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Wiki) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Any();
        }

        /// <summary>
        /// Comprueba si un documento tiene algún comentario
        /// </summary>
        /// <param name="pDocumentoID">Clave del documento</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se buscan comentarios</param>
        /// <returns>True si tiene</return s>
        public bool TieneComentariosDocumento(Guid pDocumentoID, Guid pProyectoID)
        {
            return mEntityContext.DocumentoComentario.JoinComentario().Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocumentoID) && !item.Comentario.Eliminado && mEntityContext.Proyecto.Where(item2 => item2.ProyectoID.Equals(item.DocumentoComentario.ProyectoID.Value) && (item2.ProyectoID.Equals(pProyectoID) || item2.TipoAcceso.Equals((short)TipoAcceso.Publico) || item2.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Any()).Any();
        }

        /// <summary>
        /// Comprueba si una lista de documentos son borrador
        /// </summary>
        /// <param name="pListaDocumentoID">Claves de documentos</param>
        /// <returns>True si el documento es borrador</return s>
        public Dictionary<Guid, bool> EsDocumentoBorradorLista(List<Guid> pListaDocumentoID)
        {
            Dictionary<Guid, bool> listaDocs = new Dictionary<Guid, bool>();

            if (pListaDocumentoID.Count > 0)
            {
                foreach (Guid documentoID in pListaDocumentoID)
                {
                    listaDocs.Add(documentoID, false);
                }

                var resultado = mEntityContext.Documento.Where(documento => pListaDocumentoID.Contains(documento.DocumentoID)).Select(doc => new { doc.DocumentoID, doc.Borrador }).ToList();

                foreach (var filaDoc in resultado)
                {
                    Guid docID = filaDoc.DocumentoID;
                    bool borrador = filaDoc.Borrador;
                    listaDocs[docID] = borrador;
                }

            }
            return listaDocs;
        }


        /// <summary>
        /// Comprueba si un documento está en una categoría
        /// </summary>
        /// <param name="pDocumentoID">Clave del documento</param>
        /// <param name="pCategoriaTesauroID">Clave de la categoría</param>
        /// <returns>True si el recurso está en una categoría</return s>
        public bool EstaDocumentoEnCategoria(Guid pDocumentoID, Guid pCategoriaTesauroID)
        {
            return mEntityContext.DocumentoWebAgCatTesauro.Any(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.CategoriaTesauroID.Equals(pCategoriaTesauroID));
        }

        /// <summary>
        /// Obtiene el estado de una pregunta en un proyecto concreto
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documeto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></return s>
        public EstadoPregunta ComprobarEstadoPregunta(Guid pDocumentoID, Guid pProyectoID)
        {
            bool tieneComentarios = this.TieneComentariosDocumento(pDocumentoID, pProyectoID);

            bool permiteComentarios = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, doc => doc.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (doc, baseRecProy) => new
            {
                DocumentoWebVinBaseRecursos = doc,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos.PermiteComentarios).FirstOrDefault();

            EstadoPregunta estado = EstadoPregunta.SinRespuesta;

            if (tieneComentarios)
            {
                estado = EstadoPregunta.Abierta;
            }

            if (!permiteComentarios)
            {
                estado = EstadoPregunta.Contestada;
            }

            return estado;
        }

        /// <summary>
        /// Comprueba si un documento es pregunta o debate
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></return s>
        public bool EsRecursoPreguntaODebate(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Any(documento => documento.DocumentoID.Equals(pDocumentoID) && (documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) || documento.Tipo.Equals((short)TiposDocumentacion.Debate)));
        }


        /// <summary>
        /// Obtiene la imagen principal de los documentos. Lo devuelve en formato "DocumentoID|tamaño,rutaImagenPrincipal"
        /// </summary>
        /// <param name="pListaIDsDocumentos">Lista de identificadores de los documentos</param>
        /// <returns>Array de cadenas con el documentoID y la ruta de la imagen principal</return s>
        public string[] ObtenerImagenesPrincipalesDocumentos(List<Guid> pListaIDsDocumentos)
        {
            List<string> listaImagenes = new List<string>();

            if (pListaIDsDocumentos.Count > 0)
            {
                var resultado = mEntityContext.Documento.Where(documento => pListaIDsDocumentos.Contains(documento.DocumentoID)).Select(documento => new { DocumentoID = documento.DocumentoID, NombreCategoriasDoc = documento.NombreCategoriaDoc }).ToList();
                foreach (var fila in resultado)
                {
                    string documentoID = fila.DocumentoID.ToString();
                    string imagenPincipal = fila.NombreCategoriasDoc;
                    listaImagenes.Add(documentoID + "|" + imagenPincipal);
                }
            }

            return listaImagenes.ToArray();
        }

        /// <summary>
        /// Devuelve los documentos de un proyecto con más de un editor siendo uno de ellos la identidad pasada como parámetro.
        /// </summary>
        /// <param name="pProyID">ProyectoID</param>
        /// <param name="pPerfilID">Perfil que se va a revisar</param>
        /// <param name="pUnicoEditor">pUnicoEditor=true Documentos de los que el perfil es el único editor, pUnicoEditor=false Documentos en los que el perfil comparte edicion con más gente</param>
        /// <returns>Documentación DS con los documentos con más de un editor</return s>
        public List<Documento> ObtenerRecursosIdentidadProyectoEditor(Guid pProyID, Guid pPerfilID, bool pUnicoEditor)
        {
            var queryAntesIntersect = mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().Where(objeto => objeto.Documento.ProyectoID.Value.Equals(pProyID) && objeto.Documento.ProyectoID.HasValue && objeto.DocumentoRolIdentidad.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.DocumentoRolIdentidad.DocumentoID);

            var queryDespuesIntersect = mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().Where(objeto => objeto.Documento.ProyectoID.HasValue && objeto.Documento.ProyectoID.Value.Equals(pProyID)).GroupBy(objeto => objeto.DocumentoRolIdentidad.DocumentoID).Where(objeto => objeto.Count() > 1).Select(objeto => objeto.Key);
            //Si no es único editor, sacamos todos los recursos con más de un editor
            if (pUnicoEditor)
            {
                //Si es único editor, solo los recursos de los que el perfil sea el editor
                queryDespuesIntersect = mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().Where(objeto => objeto.Documento.ProyectoID.HasValue && objeto.Documento.ProyectoID.Value.Equals(pProyID)).GroupBy(objeto => objeto.DocumentoRolIdentidad.DocumentoID).Where(objeto => objeto.Count() == 1).Select(objeto => objeto.Key);
            }

            List<Guid> resultadoFinalSubQuery = queryAntesIntersect.Intersect(queryDespuesIntersect).ToList();
            List<Documento> resultado = mEntityContext.Documento.Where(documento => resultadoFinalSubQuery.Contains(documento.DocumentoID)).ToList();

            return resultado;
        }

        /// <summary>
        /// Obtiene un diccionario de ciertos documentos los nombres cortos de los proyectos donde están subidos y compartidos.
        /// </summary>
        /// <param name="pDocumentoIDs">IDs de documentos</param>
        /// <returns>Diccionario de ciertos documentos con los nombres cortos de los proyectos donde están subidos y compartidos</return s>
        public Dictionary<Guid, List<string>> ObtenerProyectosDocumentos(List<Guid> pDocumentoIDs)
        {
            Dictionary<Guid, List<string>> dicDocs = new Dictionary<Guid, List<string>>();

            if (pDocumentoIDs.Count > 0)
            {
                var resultado = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().JoinProyecto().Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.Documento.Borrador && !objeto.DocumentoWebVinBaseRecursos.Eliminado && pDocumentoIDs.Contains(objeto.Documento.DocumentoID)).Select(objeto => new { objeto.Documento.DocumentoID, objeto.Proyecto.NombreCorto }).ToList();

                foreach (var fila in resultado)
                {
                    Guid docID = fila.DocumentoID;
                    if (!dicDocs.ContainsKey(docID))
                    {
                        dicDocs.Add(docID, new List<string>());
                    }

                    string nombreCorto = fila.NombreCorto;

                    if (!dicDocs[docID].Contains(nombreCorto))
                    {
                        dicDocs[docID].Add(nombreCorto);
                    }
                }
            }

            return dicDocs;
        }

        /// <summary>
        /// Obtiene un diccionario de ciertos documentos con sus tipos y los IDs de los proyectos donde están subidos y compartidos.
        /// </summary>
        /// <param name="pDocumentoIDs">IDs de documentos</param>
        /// <returns>Diccionario de ciertos documentos con sus tipos y los IDs de los proyectos donde están subidos y compartidos</return s>
        public Dictionary<Guid, KeyValuePair<short, List<Guid>>> ObtenerTipoYProyectosDocumentos(List<Guid> pDocumentoIDs)
        {
            Dictionary<Guid, KeyValuePair<short, List<Guid>>> listaDocs = new Dictionary<Guid, KeyValuePair<short, List<Guid>>>();

            if (pDocumentoIDs.Count > 0)
            {
                var resultaoSelTipoProy = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.Documento.Borrador && !objeto.DocumentoWebVinBaseRecursos.Eliminado && pDocumentoIDs.Contains(objeto.Documento.DocumentoID)).Select(objeto => new
                {
                    objeto.Documento.DocumentoID,
                    objeto.Documento.Tipo,
                    objeto.BaseRecursosProyecto.ProyectoID
                }).ToList();

                foreach (var fila in resultaoSelTipoProy)
                {
                    Guid docID = fila.DocumentoID;
                    if (!listaDocs.ContainsKey(docID))
                    {
                        listaDocs.Add(docID, new KeyValuePair<short, List<Guid>>(fila.Tipo, new List<Guid>()));
                    }

                    Guid proyID = fila.ProyectoID;

                    if (!listaDocs[docID].Value.Contains(proyID))
                    {
                        listaDocs[docID].Value.Add(proyID);
                    }
                }
            }

            return listaDocs;
        }

        public Dictionary<string, List<Guid>> ObtenerRecursosSubidosPorUsuario(Guid user_id)
        {
            Dictionary<string, List<Guid>> listaRecursos = new Dictionary<string, List<Guid>>();

            var resultadoRecSubPorUsu = mEntityContext.Documento.JoinIdentidad().JoinPerfil().JoinPersona().JoinProyecto().Where(objeto => objeto.Persona.UsuarioID.Value.Equals(user_id) && !objeto.Documento.Eliminado && !objeto.Documento.UltimaVersion && !objeto.Documento.Borrador).Select(objeto => new { objeto.Proyecto.NombreCorto, objeto.Documento.DocumentoID }).OrderBy(objeto => objeto.NombreCorto).ToList();

            List<Guid> listaRecursoPorNombreCorto;
            foreach (var fila in resultadoRecSubPorUsu)
            {
                string NombreCorto = fila.NombreCorto;
                Guid DocumentoID = fila.DocumentoID;
                if (!listaRecursos.ContainsKey(NombreCorto))
                {
                    listaRecursoPorNombreCorto = new List<Guid>();
                    listaRecursos.Add(NombreCorto, listaRecursoPorNombreCorto);
                }
                listaRecursos[NombreCorto].Add(DocumentoID);
            }
            return listaRecursos;
        }

        public string ObtenerPathEstilos(Guid pProyectoID)
        {
            string parametro = mEntityContext.ParametroProyecto.Where(parametroProy => parametroProy.Parametro.Equals("RutaEstilos") && parametroProy.ProyectoID.Equals(pProyectoID)).Select(parametroProy => parametroProy.Valor).ToList().FirstOrDefault();
            if (string.IsNullOrEmpty(parametro))
            {
                parametro = pProyectoID.ToString();
            }

            return parametro;
        }

        #endregion

        #region Ontologías

        public class DocumentoConsulta
        {
            public Documento Documento { get; set; }
            public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
            public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        }

        /// <summary>
        /// Carga el dataSet con las ontologías del proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pTraerProtegidos">Indica si se deben obtener las ontologías protegidas o no</param>
        /// <param name="pTraerSecundarias">Indica si hay que cargar ontologías secundarias</param>
        /// <param name="pTraerOntosEntorno">Indica si deben traerse las ontologías del entorno</param>
        public void ObtenerOntologiasProyecto(Guid pProyectoID, DataWrapperDocumentacion pDataWrapperDocumentacion, bool pTraerProtegidos, bool pTraerSecundarias, bool pTraerOntosEntorno, bool pTraerDocWebVinBaseRecursos)
        {
            List<Documento> listaDocs = new List<Documento>();
            if (pTraerSecundarias)
            {
                var listaAux = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) || objeto.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)).ToList();

                listaDocs = listaAux.Where(x => x.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !x.Documento.Eliminado && !x.DocumentoWebVinBaseRecursos.Eliminado).Select(x => x.Documento).ToList();


                //listaDocs = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => (objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) || objeto.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !objeto.Documento.Eliminado && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.Documento).ToList();
            }
            else
            {
                Guid baseRecursoProyecto = mEntityContext.BaseRecursosProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).Select(item => item.BaseRecursosID).FirstOrDefault();

                listaDocs = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && objeto.DocumentoWebVinBaseRecursos.BaseRecursosID.Equals(baseRecursoProyecto) && !objeto.Documento.Eliminado && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.Documento).ToList();
            }

            if (!pTraerProtegidos)
            {
                listaDocs = listaDocs.Where(doc => !doc.Protegido).ToList();
            }

            pDataWrapperDocumentacion.ListaDocumento = pDataWrapperDocumentacion.ListaDocumento.Concat(listaDocs).Distinct().ToList();

            if (pTraerOntosEntorno)
            {
                DataWrapperDocumentacion docEntornoDW = ObtenerOntologiasEntorno();
                pDataWrapperDocumentacion.Merge(docEntornoDW);
            }

            if (pTraerDocWebVinBaseRecursos && pDataWrapperDocumentacion.ListaDocumento.Count > 0)
            {
                List<Guid> listaDocumentosID = listaDocs.Select(docum => docum.DocumentoID).ToList();
                pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Union(mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => listaDocumentosID.Contains(doc.DocumentoID))).ToList();
            }
        }

        public List<Documento> ObtenerOntologiasSecundarias(Guid pProyectoID)
        {
            return mEntityContext.Documento.Where(doc => doc.ProyectoID.Equals(pProyectoID) && !doc.Borrador && !doc.Eliminado && doc.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)).ToList();
        }

        /// <summary>
        /// Obtiene la ontología proyecto a partir de la ontología y el proyecto id
        /// </summary>
        /// <param name="pOntologia">Nombre de la ontología</param>
        /// <param name="pProyectoID">Identificador del proyecto al que pertenece la ontología</param>
        /// <returns></returns>
        public OntologiaProyecto ObtenerOntologiaProyectoPorOntologia(string pOntologia, Guid pProyectoID)
        {
            return mEntityContext.OntologiaProyecto.Where(item => item.OntologiaProyecto1.Equals(pOntologia) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el ID de un recurso a traves de su nombre y su proyecto
        /// </summary>
        /// <param name="pNombre">Nombre del documento</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns></return s>
        public Guid ObtenerDocumentoWikiPorNombreyProyecto(string pNombre, Guid pProyectoID)
        {
            Documento documento = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().JoinDocumento().Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && objeto.Documento.Titulo.ToLower().Equals(pNombre.ToLower()) && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Wiki) && objeto.Documento.UltimaVersion && !objeto.Documento.Eliminado).Select(objeto => objeto.Documento).FirstOrDefault();
            if (documento != null)
            {
                return documento.DocumentoID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public void ObtenerOntologiasPlataforma(DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            pDataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(documento => (documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) || documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)) && !documento.Eliminado && !documento.Protegido).ToList();

        }

        /// <summary>
        /// Obtiene las ontologías de un entorno
        /// </summary>
        /// <param name="pDocumentacionDS">Dataset de documentación ya inicializado</param>
        public DataWrapperDocumentacion ObtenerOntologiasEntorno()
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(documento => documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && !documento.Eliminado && documento.Visibilidad == 1 && !documento.Protegido).ToList();

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene id del publicador a partir del id del comentario 
        /// </summary>
        /// <param name="pComentarioID">ID del comentario</param>
        /// <returns>ID de la identidad del publicador</return s>
        public Guid ObtenerPublicadorAPartirIDsComentario(Guid pComentarioID)
        {
            return mEntityContext.Comentario.Where(item => item.ComentarioID.Equals(pComentarioID)).Select(item => item.IdentidadID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene id del publicador a partir del id del documento y del proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <returns>ID de la identidad del publicador</return s>
        public Guid ObtenerPublicadorAPartirIDsRecursoYProyecto(Guid pProyectoID, Guid pDocumentoID)
        {
            Guid? resultado = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID).FirstOrDefault();
            Guid documentoID = Guid.Empty;

            if (resultado.HasValue)
            {
                documentoID = resultado.Value;
            }

            return documentoID;
        }

        public void ObtenerDatasetConOntologiaAPartirNombre(Guid pProyectoID, string pNombre, DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            pDataWrapperDocumentacion.ListaDocumento = pDataWrapperDocumentacion.ListaDocumento.Concat(mEntityContext.Documento.Where(documento => documento.Tipo == 7 && documento.ProyectoID.Value.Equals(pProyectoID) && documento.Enlace.Equals(pNombre) && !documento.Eliminado)).ToList().Distinct().ToList();
        }
        /// <summary>
        /// Obtiene una ontología a partir de su nombre en una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNombre">Nombre de la ontología</param>
        /// <returns>ID de la ontología</return s>
        public Guid ObtenerOntologiaAPartirNombre(Guid pProyectoID, string pNombre, bool ptraerSecundarias = true)
        {
            if (pNombre.Contains("/"))
            {
                pNombre = pNombre.Substring(pNombre.LastIndexOf("/") + 1);
            }

            if (pNombre.LastIndexOf("#") == (pNombre.Length - 1))
            {
                pNombre = pNombre.Substring(0, pNombre.Length - 1);
            }

            var resOnto = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => (objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) || objeto.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)) && objeto.Documento.Enlace.ToLower().Equals(pNombre.ToLower()));
            if (!ptraerSecundarias)
            {
                resOnto = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && objeto.Documento.Enlace.ToLower().Equals(pNombre.ToLower()));
            }

            string patronIDOntologias = mEntityContext.ParametroProyecto.Where(param => param.Parametro.Equals(ParametroAD.ProyectoIDPatronOntologias) && param.ProyectoID.Equals(pProyectoID)).Select(param => param.Valor).FirstOrDefault();
            Guid patronOntologia = new Guid();
            try
            {
                if (!string.IsNullOrEmpty(patronIDOntologias))
                {
                    patronOntologia = Guid.Parse(patronIDOntologias);
                }
                else
                {
                    patronOntologia = Guid.Empty;
                }
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, "Error al procesar el patronIDOntologia", mlogger);
            }
            List<Guid> listaIDs = new List<Guid>();
            if (pProyectoID != Guid.Empty)
            {
                IEnumerable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyecto> resOntoMemory;
                if (patronOntologia != Guid.Empty)
                {
                    resOntoMemory = resOnto.ToList().Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) || objeto.BaseRecursosProyecto.ProyectoID.Equals(patronOntologia));
                }
                else
                {
                    resOntoMemory = resOnto.ToList().Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID));
                }
                listaIDs = resOntoMemory.ToList().Where(objeto => !objeto.Documento.Eliminado && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.Documento.DocumentoID).ToList().Union(mEntityContext.Documento.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList().Where(doc => !doc.Eliminado && doc.Visibilidad == 1 && doc.Enlace.ToLower().Equals(pNombre.ToLower())).Select(doc => doc.DocumentoID)).ToList();
            }
            else
            {
                listaIDs = resOnto.ToList().Where(objeto => !objeto.Documento.Eliminado && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.Documento.DocumentoID).ToList().Union(mEntityContext.Documento.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList().Where(doc => !doc.Eliminado && doc.Visibilidad == 1 && doc.Enlace.ToLower().Equals(pNombre.ToLower())).Select(doc => doc.DocumentoID)).ToList();

            }
            Guid documentoID = Guid.Empty;

            if (listaIDs.Count > 0)
            {
                documentoID = listaIDs.First();
            }

            return documentoID;
        }

        /// <summary>
        /// Actualiza la foto de los recursos de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontologia</param>
        /// <param name="pRutaFoto">Ruta de la foto</param>
        /// <param name="pBorrar">Indica si se ha borrado la foto</param>
        public void ActualizarFotoDocumentosDeOntologia(Guid pOntologiaID, string pRutaFoto, bool pBorrar)
        {
            List<Documento> consultaDocumento;
            if (pBorrar)
            {
                consultaDocumento = mEntityContext.Documento.Where(doc => doc.NombreCategoriaDoc.Equals(pRutaFoto)).ToList();
            }
            else
            {
                consultaDocumento = mEntityContext.Documento.Where(doc => string.IsNullOrEmpty(doc.NombreCategoriaDoc)).ToList();
            }
            consultaDocumento = consultaDocumento.Where(doc => doc.ElementoVinculadoID.Equals(pOntologiaID) && !doc.Eliminado).ToList();

            if (pBorrar)
            {
                foreach (Documento docu in consultaDocumento)
                {
                    docu.NombreCategoriaDoc = null;
                }
            }
            else
            {
                foreach (Documento docu in consultaDocumento)
                {
                    docu.NombreCategoriaDoc = pRutaFoto;
                }
            }

            if (!pBorrar) //Aumentamos la versión de las fotos
            {
                List<Documento> listaDocs = mEntityContext.Documento.Where(doc => doc.NombreCategoriaDoc.Equals(pRutaFoto) && doc.ElementoVinculadoID.Value.Equals(pOntologiaID) && !doc.Eliminado).ToList();
                foreach (Documento doc in listaDocs)
                {
                    if (doc.VersionFotoDocumento.HasValue)
                    {
                        doc.VersionFotoDocumento = Math.Abs(doc.VersionFotoDocumento.Value) + 1;
                    }
                    else
                    {
                        doc.VersionFotoDocumento = 1;
                    }
                }
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Obtiene los últimos recursos de una determinada ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de ontología</param>
        /// <param name="pNumRec">Número de recursos</param>
        /// <returns>Lista con los IDs de los últimos recursos de la ontología</return s>
        public List<Guid> ObtenerUltimosRecursosDeOnto(Guid pOntologiaID, int pNumRec)
        {
            return mEntityContext.Documento.Where(documento => documento.ElementoVinculadoID.Value.Equals(pOntologiaID) && !documento.Eliminado && !documento.Borrador && documento.UltimaVersion && documento.Tipo == 5).OrderByDescending(doc => doc.FechaCreacion.Value).Take(pNumRec).Select(doc => doc.DocumentoID).ToList();
        }

        /// <summary>
        /// Obtiene los ID de todos los recursos de una determinada ontología.
        /// </summary>
        /// <param name="pOntologiaID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerRecursosDeOntologia(Guid pOntologiaID)
        {
            return mEntityContext.Documento.Where(documento => documento.ElementoVinculadoID.Value.Equals(pOntologiaID) && !documento.Eliminado && !documento.Borrador && documento.UltimaVersion && documento.Tipo == 5).OrderByDescending(doc => doc.FechaCreacion.Value).Select(doc => doc.DocumentoID).ToList();
        }

        /// <summary>
        /// Obtiene los identificadores de los recursos que pertenezcan al proyecto indicado y sean de los tipos que se pasan por parametro
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pTipos"></param>
        /// <param name="pOntologias"></param>
        /// <returns></returns>
        public List<Guid> ObtenerRecursosConMejorasActivas(Guid pProyectoID, List<int> pTipos, List<Guid?> pOntologias)
        {
            var ultimasPendientes = mEntityContext.VersionDocumento.JoinDocumento().Where(
                versionDoc => pTipos.Contains(versionDoc.Documento.Tipo) && versionDoc.Documento.ProyectoID.Equals(pProyectoID) &&
                 !versionDoc.Documento.Eliminado && !versionDoc.Documento.Borrador && (pOntologias.Contains(versionDoc.Documento.ElementoVinculadoID)) && versionDoc.VersionDocumento.EsMejora && versionDoc.VersionDocumento.EstadoVersion == (short)EstadoVersion.Pendiente);

            var buscarMaximaVersionPorMejora = ultimasPendientes.GroupBy(x => x.VersionDocumento.MejoraID).Select(g => new
            {
                MejoraID = g.Key,
                MaxVersion = g.Max(doc => doc.VersionDocumento.Version)
            });

            var resultado = ultimasPendientes.Join(
                buscarMaximaVersionPorMejora,
                ultimasPendientes => new { ultimasPendientes.VersionDocumento.MejoraID, ultimasPendientes.VersionDocumento.Version },
                maxVersionPorMejora => new { maxVersionPorMejora.MejoraID, Version = maxVersionPorMejora.MaxVersion },
                (ultimasPendientes, maxVersionPorMejora) => ultimasPendientes.VersionDocumento.DocumentoID);

            return resultado.ToList();
        }

        /// <summary>
        /// Obtiene la cantidad recursos de una determinada ontología.
        /// </summary>
        /// <param name="pOntologiaID"></param>
        /// <returns></returns>
        public int ObtenerCantidadRecursosDeOntologia(Guid pOntologiaID)
        {
            return mEntityContext.Documento.Count(documento => documento.ElementoVinculadoID.Value.Equals(pOntologiaID) && !documento.Eliminado && documento.Tipo == 5);
        }


        /// <summary>
        /// Indica si existe una ontología con determinado nombre en un proyecto, o si 'pDocumentoID' no es nulo comprueba si no existe ninguna además de la pasada como parámetro.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pEnlaceOntologia">Enlace de la ontología</param>
        /// <param name="pDocumentoID">ID del documento con dicho enlace o NULL si se quiere comprobar en general</param>
        /// <returns>TRUE si existe una ontología con determinado nombre en un proyecto, FALSE si no</return s>
        public bool ExisteOntologiaEnProyecto(Guid pProyectoID, string pEnlaceOntologia, Guid? pDocumentoID)
        {
            if (pDocumentoID.HasValue)
            {
                return mEntityContext.Documento.Any(documento => documento.ProyectoID.HasValue && documento.ProyectoID.Value.Equals(pProyectoID) && (documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) || documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)) && documento.Enlace.Equals(pEnlaceOntologia) && !documento.Eliminado && !documento.DocumentoID.Equals(pDocumentoID.Value));
            }
            else
            {
                return mEntityContext.Documento.Any(documento => documento.ProyectoID.HasValue && documento.ProyectoID.Value.Equals(pProyectoID) && (documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) || documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria)) && documento.Enlace.Equals(pEnlaceOntologia) && !documento.Eliminado);
            }

        }


        /// <summary>
        /// Existe documento en proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>TRUE si existe un documento con identificador en el proyecto indicado</returns>
        public bool ExisteDocumentoEnProyecto(Guid pProyectoID, Guid pDocumentoID)
        {
            return mEntityContext.Documento.Any(documento => documento.DocumentoID.Equals(pDocumentoID) && documento.ProyectoID.HasValue && documento.ProyectoID.Equals(pProyectoID) && !documento.Eliminado);
        }

        /// <summary>
        /// Obtiene una lista con IDs de documentos y sus elementosvinculadosID.
        /// </summary>
        /// <param name="pDocIDs">IDs de documentos</param>
        /// <returns>Lista con IDs de documentos y sus elementosvinculadosID</return s>
        public Dictionary<Guid, Guid> ObtenerListaRecursosConElementoVinculadoID(List<Guid> pDocIDs)
        {
            Dictionary<Guid, Guid> docsElemV = new Dictionary<Guid, Guid>();

            if (pDocIDs.Count > 0)
            {
                var resultado = mEntityContext.Documento.Where(doc => pDocIDs.Contains(doc.DocumentoID) && doc.ElementoVinculadoID.HasValue).Select(doc => new { doc.DocumentoID, doc.ElementoVinculadoID }).Distinct().ToList();

                foreach (var fila in resultado)
                {
                    docsElemV.Add(fila.DocumentoID, fila.ElementoVinculadoID.Value);
                }

            }

            return docsElemV;
        }

        /// <summary>
        /// Comprueba si un usuario es administrador de alguna comunidad que tenga una ontologia concreta
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOntologia">Ontología de la que el usuario debe ser administrador</param>
        /// <returns>Verdad si el usuario administra alguna comunidad que contenga esta ontología</return s>
        public bool ComprobarUsuarioAdministraOntologia(Guid pUsuarioID, string pOntologia)
        {
            return mEntityContext.AdministradorProyecto.Join(mEntityContext.Documento, adminProy => adminProy.ProyectoID, doc => doc.ProyectoID.Value, (adminProy, doc) => new
            {
                AdministradorProyecto = adminProy,
                Documento = doc
            }).Where(objeto => (objeto.Documento.Tipo == 7 || objeto.Documento.Tipo == 23) && objeto.AdministradorProyecto.Tipo == 0 && objeto.Documento.Enlace.ToLower().Equals(pOntologia.ToLower()) && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID) && !objeto.Documento.Eliminado).Any();
        }

        #endregion

        #region Documentos Web

        /// <summary>
        /// Comprueba si un documento ha sido editado por otra persona al mismo tiempo
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pFechaModificacion">Última fecha de modificación del documento</param>
        /// <param name="pNuevaVersionDocumentoID">Si esta versión del documento está obsoleta, esta variable devolverá el ID de la versión actual del documento</param>
        /// <returns>Verdad si hay error de concurrencia</return s>
        public ErroresConcurrencia ComprobarConcurrenciaDocumento(Guid pDocumentoID, DateTime pFechaModificacion, out Guid? pNuevaVersionDocumentoID)
        {
            //int version = 0;
            pNuevaVersionDocumentoID = null;
            int version = mEntityContext.VersionDocumento.Join(mEntityContext.VersionDocumento, v1 => v1.DocumentoOriginalID, v2 => v2.DocumentoOriginalID, (v1, v2) => new
            {
                VersionDocumentoV1 = v1,
                VersionDocumentoV2 = v2
            })
            .Where(objeto => objeto.VersionDocumentoV1.DocumentoID.Equals(pDocumentoID))
            .OrderByDescending(item => item.VersionDocumentoV1.Version)
            .Select(objeto => objeto.VersionDocumentoV1.Version).FirstOrDefault();


            var consulta1 = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID) && documento.FechaModificacion.HasValue && !documento.FechaModificacion.Equals(pFechaModificacion) && documento.UltimaVersion).Select(doc => doc.DocumentoID).ToList();
            var consulta2 = mEntityContext.VersionDocumento.Join(mEntityContext.VersionDocumento, v1 => v1.DocumentoOriginalID, v2 => v2.DocumentoOriginalID, (v1, v2) => new
            {
                VersionDocumentoV1 = v1,
                VersionDocumentoV2 = v2
            }).Where(objeto => objeto.VersionDocumentoV1.DocumentoID.Equals(pDocumentoID) && objeto.VersionDocumentoV2.Version == version && !objeto.VersionDocumentoV2.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.VersionDocumentoV1.DocumentoID).ToList();
            var consulta3 = mEntityContext.VersionDocumento.Where(vDoc => vDoc.DocumentoOriginalID.Equals(pDocumentoID)).ToList().Where(x => !x.Documento.FechaModificacion.Equals(pFechaModificacion)).Select(vDoc => vDoc.DocumentoID);

            Guid? resConsulta = consulta1.Union(consulta2).Union(consulta3).FirstOrDefault();

            if (resConsulta.HasValue && !resConsulta.Value.Equals(Guid.Empty))
            {
                Guid id = resConsulta.Value;
                if (id.Equals(pDocumentoID))
                {
                    return ErroresConcurrencia.ConcurrenciaMismaVersion;
                }
                else
                {
                    pNuevaVersionDocumentoID = id;
                    return ErroresConcurrencia.VersionObsoleta;
                }
            }

            return ErroresConcurrencia.NoConcurrencia;
        }

        /// <summary>
        /// Obtiene un array cuyo primer elemento es la identidad del creador del recurso  la segunda componente es el usuario
        /// </summary>
        /// <param name="pRecursoID">Recurso que busca</param>        
        /// <returns>Array cuyo primer elemento es el perfil de la identidad del creador del recurso  la segunda componente es el usuario</return s>
        public Dictionary<Guid, Guid> ObtenerIdentidadyUsuarioIDdeRecurso(Guid pRecursoID)
        {
            Dictionary<Guid, Guid> DicIdUs = new Dictionary<Guid, Guid>();

            var consulta = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => item.Documento.DocumentoID.Equals(pRecursoID)).Select(item => new { item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID, item.BaseRecursosUsuario.UsuarioID }).ToList();
            foreach (var fila in consulta)
            {
                if (fila.IdentidadPublicacionID.HasValue)
                {
                    DicIdUs.Add(fila.IdentidadPublicacionID.Value, fila.UsuarioID);
                }

            }
            return DicIdUs;
        }

        /// <summary>
        /// Obtiene identificadores de los documentos de la tabla Documento vinculados a un proyecto
        /// </summary>
        /// <param name="pTop">Tope de documentos recuperados</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        public List<Guid> ObtenerDocumentosIDVinculadosAProyecto(Guid pProyectoID)
        {

            List<Guid> listaDocumentosID = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                DocumentoID = doc.DocumentoID,
                BaseRecursosID = docWebVin.BaseRecursosID
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (objeto, baseRecursoProy) => new
            {
                DocumentoID = objeto.DocumentoID,
                ProyectoID = baseRecursoProy.ProyectoID
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoID).Distinct().ToList();

            return listaDocumentosID;
        }

        public List<Guid> ObtenerDocumentosSemIDVinculadosAProyecto(Guid pProyectoID)
        {

            List<Guid> listaDocumentosID = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                DocumentoID = doc.DocumentoID,
                BaseRecursosID = docWebVin.BaseRecursosID,
                Tipo = doc.Tipo
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (objeto, baseRecursoProy) => new
            {
                DocumentoID = objeto.DocumentoID,
                ProyectoID = baseRecursoProy.ProyectoID,
                Tipo = objeto.Tipo
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID) && objeto.Tipo.Equals(TiposDocumentacion.Semantico)).Select(objeto => objeto.DocumentoID).Distinct().ToList();

            return listaDocumentosID;
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento vinculados a un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public DataWrapperDocumentacion ObtenerDocumentosVinculadosAProyecto(Guid pProyectoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                BaseRecursosID = docWebVin.BaseRecursosID
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (objeto, baseRecursoProy) => new
            {
                Documento = objeto.Documento,
                ProyectoID = baseRecursoProy.ProyectoID
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.Documento).ToList();
            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene una lista de identificadores cuyo elemento vinculado es el ID de la ontología
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista de identificadores de los documentos cuyo elemento vinculado es la ontología</return s>
        public List<Guid> ObtenerDocumentosIDVinculadosAOntologiaProyecto(Guid pOntologiaID, Guid pProyectoID)
        {
            return mEntityContext.Documento.Where(doc => doc.ProyectoID.Value.Equals(pProyectoID) && doc.ElementoVinculadoID.Value.Equals(pOntologiaID)).Select(doc => doc.DocumentoID).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene si existen documentos cuyo elemento vinculado es el ID de la ontología
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>True si existe</returns>
        public bool ExistenVinculadosAOntologiaProyecto(Guid pOntologiaID, Guid pProyectoID)
        {
            return mEntityContext.Documento.Any(doc => doc.ProyectoID.Value.Equals(pProyectoID) && doc.ElementoVinculadoID.Value.Equals(pOntologiaID));
        }

        /// <summary>
        /// Obtiene una diccionario cuya clave es el DocumentoId y el valor el nombre de la ontología
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Diccionario de las ontologías para su borrado masivo</return s>
        public Dictionary<Guid, string> ObtenerOntologiasParaBorrado(Guid pProyectoID)
        {
            List<Documento> ontologias = new List<Documento>();
            var varOntologiasBorradoMasivo = mEntityContext.Documento.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Ontologia) && doc.Eliminado.Equals(false) && doc.ProyectoID.HasValue && doc.ProyectoID.Value.Equals(pProyectoID));
            ontologias = varOntologiasBorradoMasivo.ToList();
            Dictionary<Guid, string> ontologiasBorrado = new Dictionary<Guid, string>();
            foreach (Documento doc in ontologias)
            {
                ontologiasBorrado.Add(doc.DocumentoID, doc.Titulo);
            }
            return ontologiasBorrado;
        }

        /// <summary>
        /// Borra los documentos de forma FISICA, NO LÓGICA
        /// </summary>
        /// <param name="pDocumentoIN"></param>
        public void BorrarDocumentosScript(List<Guid> pDocumentosIN, Guid pProyectoID)
        {
            string deleteDocumentoQuery;
            string selectDocumentoQuery;

            bool isSQLServerConnection = !(ConexionMaster is NpgsqlConnection) && !(ConexionMaster is OracleConnection);

            string tablock = string.Empty;

            if (isSQLServerConnection)
            {
                tablock = "WITH(TABLOCK)";
            }

            if (pDocumentosIN.Count == 1)
            {
                deleteDocumentoQuery = $"delete from \"Documento\" {tablock} where \"ElementoVinculadoID\" = {IBD.FormatearGuid(pDocumentosIN.FirstOrDefault())} and \"ProyectoID\" = {IBD.FormatearGuid(pProyectoID)}";
                selectDocumentoQuery = $"select \"DocumentoID\" from \"Documento\" where \"ElementoVinculadoID\" = {IBD.FormatearGuid(pDocumentosIN.FirstOrDefault())} and \"ProyectoID\" = {IBD.FormatearGuid(pProyectoID)}";
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                string INconsulta = IBD.FormatearGuid(pDocumentosIN[0]);
                for (int i = 1; i < pDocumentosIN.Count; i++)
                {
                    stringBuilder.Append($", {IBD.FormatearGuid(pDocumentosIN[i])}");
                }
                INconsulta += stringBuilder.ToString();
                deleteDocumentoQuery = $"delete from \"Documento\" {tablock} where \"ElementoVinculadoID\" IN ({INconsulta}) and \"ProyectoID\" = {IBD.FormatearGuid(pProyectoID)}";
                selectDocumentoQuery = $"select \"DocumentoID\" from \"Documento\" where \"ElementoVinculadoID\" IN ({INconsulta}) and \"ProyectoID\" = {IBD.FormatearGuid(pProyectoID)}";
            }
            string sqlDocumentoEnEdicion = $"delete from \"DocumentoEnEdicion\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";
            string sqlDeleteVotoDocumento = $"delete from \"VotoDocumento\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";
            string sqlDelteDocumentoWebVinBaseRecursosExtra = $"delete from \"DocumentoWebVinBaseRecursosExtra\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";
            string sqlDelteDocumentoWebVinBaseRecursos = $"delete from \"DocumentoWebVinBaseRecursos\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";
            string sqlDelteDocumentoWebAgCatTesauro = $"delete from \"DocumentoWebAgCatTesauro\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";
            string sqlDeleteDocumentoRolIdentidad = $"delete from \"DocumentoRolIdentidad\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";
            string sqlDeleteHistorialDocumento = $"delete from \"HistorialDocumento\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";
            string sqlDeleteDocumentoComentario = $"delete from \"DocumentoComentario\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";
            string sqlDelteDocumentoRolGrupoIdentidades = $"delete from \"DocumentoRolGrupoIdentidades\" {tablock} where \"DocumentoID\" IN ({selectDocumentoQuery})";

            try
            {
                bool transaccionIniciada = IniciarTransaccion();
                DbCommand comsqlDeleteDocumentoRolIdentidad = ObtenerComando(sqlDeleteDocumentoRolIdentidad);
                comsqlDeleteDocumentoRolIdentidad.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDeleteDocumentoRolIdentidad, false, true, true);

                DbCommand comsqlDeleteDocumentoComentario = ObtenerComando(sqlDeleteDocumentoComentario);
                comsqlDeleteDocumentoComentario.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDeleteDocumentoComentario, false, true, true);

                DbCommand comsqlDeleteHistorialDocumento = ObtenerComando(sqlDeleteHistorialDocumento);
                comsqlDeleteHistorialDocumento.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDeleteHistorialDocumento, false, true, true);

                DbCommand comsqlDelteDocumentoWebVinBaseRecursosExtra = ObtenerComando(sqlDelteDocumentoWebVinBaseRecursosExtra);
                comsqlDelteDocumentoWebVinBaseRecursosExtra.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDelteDocumentoWebVinBaseRecursosExtra, false, true, true);

                DbCommand comsqlDelteDocumentoWebVinBaseRecursos = ObtenerComando(sqlDelteDocumentoWebVinBaseRecursos);
                comsqlDelteDocumentoWebVinBaseRecursos.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDelteDocumentoWebVinBaseRecursos, false, true, true);

                DbCommand comsqlDelteDocumentoWebAgCatTesauro = ObtenerComando(sqlDelteDocumentoWebAgCatTesauro);
                comsqlDelteDocumentoWebAgCatTesauro.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDelteDocumentoWebAgCatTesauro, false, true, true);

                DbCommand comsqlDelteDocumentoRolGrupoIdentidades = ObtenerComando(sqlDelteDocumentoRolGrupoIdentidades);
                comsqlDelteDocumentoRolGrupoIdentidades.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDelteDocumentoRolGrupoIdentidades, false, true, true);

                DbCommand comsqlDeleteVotoDocumento = ObtenerComando(sqlDeleteVotoDocumento);
                comsqlDeleteVotoDocumento.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDeleteVotoDocumento, false, true, true);

                DbCommand comsqlDeleteDocumentoEnEdicion = ObtenerComando(sqlDocumentoEnEdicion);
                comsqlDeleteDocumentoEnEdicion.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDeleteDocumentoEnEdicion, false, true, true);

                DbCommand comsqlDeleteDocumento = ObtenerComando(deleteDocumentoQuery);
                comsqlDeleteDocumento.CommandTimeout = 600;
                ActualizarBaseDeDatos(comsqlDeleteDocumento, false, true, true);

                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
                }
            }
            catch (Exception)
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Obtiene la ontología seleccionada
        /// </summary>
        /// <param name="pOntologiaID">Identificador del documento de la ontologia</param>
        /// <returns>Obtiene la ontología seleccionada</return s>
        public Documento OntologiaSeleccionada(Guid pOntologiaID)
        {
            return mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pOntologiaID)).FirstOrDefault();
        }
        /// <summary>
        /// Obtiene la ontología seleccionada
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Obtiene la ontología seleccionada</return s>
        public Documento HayOntologiaSeleccionada(Guid pProyectoID)
        {
            return mEntityContext.Documento.Where(doc => doc.ProyectoID.HasValue && doc.ProyectoID.Value.Equals(pProyectoID)).FirstOrDefault();
        }
        /// <summary>
        /// Obtiene registros de la tabla Documento y DocumentoWevBin vinculados a un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public DataWrapperDocumentacion ObtenerDocumentosVinculadosAProyectoYVinculaciones(Guid pProyectoID)
        {
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            documentacionDW.ListaDocumento = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                BaseRecursosID = docWebVin.BaseRecursosID
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (objeto, baseRecursoProy) => new
            {
                Documento = objeto.Documento,
                ProyectoID = baseRecursoProy.ProyectoID
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.Documento).ToList();

            documentacionDW.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWebVin => docWebVin.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (docWebVin, baseRecursoProy) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                ProyectoID = baseRecursoProy.ProyectoID
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Devuelve true si el proyecto que se le pasa en el parametro contiene algún documento de tipo plantilla, false en caso contrario
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto que queremos comprobar</param>
        /// <returns></return s>
        public bool ProyectoTieneDocsPlantillas(Guid pProyectoID)
        {
            return mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docWebVin => docWebVin.DocumentoID, (documento, docWebVin) => new
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecurso => baseRecurso.BaseRecursosID, (objeto, baseRecurso) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecurso
            }).Where(objeto => objeto.Documento.Tipo.Equals((int)TiposDocumentacion.Ontologia) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Any();
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento de la base de recursos personal de un usuario
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public DataWrapperDocumentacion ObtenerDocumentosDeBaseRecursosUsuario(Guid pUsuarioID)
        {
            DataWrapperDocumentacion documentacionDW = new DataWrapperDocumentacion();

            documentacionDW.ListaDocumento = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosUsuario().Where(item => item.BaseRecursosUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.Documento).ToList();

            return documentacionDW;
        }

        /// <summary>
        /// Obtiene los ids y su tipos de los documentos de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de comunidad</param>
        /// <returns>ids y su tipos de los documentos de una comunidad</return s>
        public Dictionary<Guid, short> ObtenerDocumentosYTipodeProyecto(Guid pProyectoID)
        {
            Dictionary<Guid, short> docs = new Dictionary<Guid, short>();
            mEntityContext.Database.SetCommandTimeout(3600);
            var resultado = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecurso => baseRecurso.BaseRecursosID, (objeto, baseRecurso) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecurso
            }).Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => new { objeto.Documento.DocumentoID, objeto.Documento.Tipo }).ToList();

            foreach (var fila in resultado)
            {
                docs.Add(fila.DocumentoID, fila.Tipo);
            }
            return docs;
        }


        /// <summary>
        /// Obtiene nivel certificacion
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public short ObtenerNivelCertificacion(Guid pDocumentoID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.JoinNivelCertificacion().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(item => item.NivelCertificacion.Orden).FirstOrDefault();
        }

        /// <summary>
        /// Devuelve los recursos vinculados con el nivel de certificacion indicado
        /// </summary>
        /// <param name="pNivelCertifiacionID"></param>
        /// <returns></returns>
        public List<DocumentoWebVinBaseRecursos> ObtenerBaseRecursosPorNivelCertifiacion(Guid pNivelCertifiacionID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Where(item => item.NivelCertificacionID.Equals(pNivelCertifiacionID)).ToList();
        }

        /// <summary>
        /// Obtiene nivel certificacion
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public Dictionary<Guid, Dictionary<Guid, string>> ObtenerNivelCertificacionDeDocumentos(List<Guid> pDocumentosID, Guid pProyectoID)
        {
            var consulta = mEntityContext.DocumentoWebVinBaseRecursos.JoinNivelCertificacion().Where(doc => pDocumentosID.Contains(doc.DocumentoWebVinBaseRecursos.DocumentoID) && doc.NivelCertificacion.ProyectoID.Equals(pProyectoID)).Select(item => new
            {
                DocumentoID = item.DocumentoWebVinBaseRecursos.DocumentoID,
                NivelCertificacionID = item.NivelCertificacion.NivelCertificacionID,
                Descripcion = item.NivelCertificacion.Descripcion
            });

            Dictionary<Guid, Dictionary<Guid, string>> docs = new Dictionary<Guid, Dictionary<Guid, string>>();

            foreach (var objeto in consulta)
            {
                Dictionary<Guid, string> dic = new Dictionary<Guid, string>();
                dic.Add(objeto.NivelCertificacionID, objeto.Descripcion);
                docs.Add(objeto.DocumentoID, dic);
            }

            return docs;
        }

        /// <summary>
        /// Obtiene niveles de certificacion de recursos de un proyecto
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public Dictionary<int, List<Guid>> ObtenerNivelesCertificacionDeDocsEnProyecto(Guid pProyectoID)
        {
            Dictionary<int, List<Guid>> listaNievelesDocs = new Dictionary<int, List<Guid>>();

            DataWrapperDocumentacion dataSetAux = new DataWrapperDocumentacion();

            var query = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosProyecto().GroupJoin(mEntityContext.NivelCertificacion, item => item.DocumentoWebVinBaseRecursos.NivelCertificacionID, nivelCertificacion => nivelCertificacion.NivelCertificacionID, (item, nivelCertificacion) => new
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                NivelCertificacion = nivelCertificacion
            }).SelectMany(item => item.NivelCertificacion.DefaultIfEmpty(), (x, y) => new
            {
                NivelCertificacion = y,
                DocumentoWebVinBaseRecursos = x.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = x.BaseRecursosProyecto
            }).Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(item => new
            {
                Orden = (item.NivelCertificacion == null) ? 100 : item.NivelCertificacion.Orden,
                DocumentoID = item.DocumentoWebVinBaseRecursos.DocumentoID
            });
            var lista = query.ToList();
            foreach (var objeto in query)
            {
                int nivel = objeto.Orden;
                if (listaNievelesDocs.ContainsKey(nivel))
                {
                    listaNievelesDocs[nivel].Add(objeto.DocumentoID);
                }
                else
                {
                    List<Guid> nuevaLista = new List<Guid>();
                    nuevaLista.Add(objeto.DocumentoID);
                    listaNievelesDocs.Add(nivel, nuevaLista);
                }
            }

            return listaNievelesDocs;
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento de la base de recursos de una organización
        /// </summary>
        /// <param name="pOrganizacionID">ID de la organización</param>
        public DataWrapperDocumentacion ObtenerDocumentosDeBaseRecursosOrganizacion(Guid pOrganizacionID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosOrganizacion().Where(objeto => objeto.BaseRecursoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(objeto => objeto.Documento).ToList();

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene la fecha de creación del documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Devuelve DateTime con la fecha de creación del documento</return s>
        public long ObtenerFechaCreacionDocumento(Guid pDocumentoID)
        {
            DateTime? fechaCreacion = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).Select(doc => doc.FechaCreacion).ToList().FirstOrDefault();
            if (fechaCreacion.HasValue)
            {
                return fechaCreacion.Value.Ticks;
            }
            return 0;
        }

        public long ObtenerFechaModificacionDocumento(Guid pDocumentoID)
        {
            DateTime? fechaModificacion = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).Select(doc => doc.FechaModificacion).ToList().FirstOrDefault();
            if (fechaModificacion.HasValue)
            {
                return fechaModificacion.Value.Ticks;
            }
            return 0;
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento de la base de recursos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public List<Documento> ObtenerDocumentosDeBaseRecursosProyecto(Guid pProyectoID)
        {
            var variable = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (documento, documentoWebVinBaseRecursos) => new
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            })
            .Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.Documento.Borrador && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.ProyectoID.HasValue && objeto.Documento.ProyectoID.Value.Equals(pProyectoID))
            .OrderBy(objeto => objeto.Documento.FechaCreacion)
            .Select(objeto => objeto.Documento);

            if (variable != null)
                return variable.ToList();

            return null;
        }

        /// <summary>
        /// Obtiene los documentos web de una base de recursos pasada como parámetro
        /// </summary>
        public DataWrapperDocumentacion ObtenerDocumentosWebDeBaseRecursos(Guid pBaseRecursosID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(docWebVin => docWebVin.BaseRecursosID.Equals(pBaseRecursosID)).ToList();
            dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(docWebAg => docWebAg.BaseRecursosID.Equals(pBaseRecursosID)).ToList();

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene las vinculaciones de los documentos obsoletos de una base de recursos
        /// </summary>
        /// <param name="pBaseRecursosID">Base de recursos</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerDocumentosObsoletosDeBaseRecursos(Guid pBaseRecursosID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            // DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.Documento, docWebVin => docWebVin.DocumentoID, documento => documento.DocumentoID, (docWebVin, documento) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                Documento = documento
            }).Where(objeto => !objeto.Documento.UltimaVersion && objeto.DocumentoWebVinBaseRecursos.BaseRecursosID.Equals(pBaseRecursosID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            // DocumentoWebAgCatTesauro
            dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Join(mEntityContext.Documento, docWebAg => docWebAg.DocumentoID, documento => documento.DocumentoID, (docWebAg, documento) => new
            {
                DocumentoWebAgCatTesauro = docWebAg,
                Documento = documento
            }).Where(objeto => !objeto.Documento.UltimaVersion && objeto.DocumentoWebAgCatTesauro.BaseRecursosID.Equals(pBaseRecursosID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            return dataWrapperDocumentacion;
        }


        /// <summary>
        /// Obtiene los documentos web según la lista de IDs pasada como parámetro.
        /// </summary>
        /// <param name="pDocumentos">Lista con los id de documentos</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentacion</param>
        public void ObtenerDocumentosWebPorIDWEB(List<Guid> pDocumentos, DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Union(mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => pDocumentos.Contains(doc.DocumentoID)).ToList()).ToList();
            pDataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = pDataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Union(mEntityContext.DocumentoWebAgCatTesauro.Where(doc => pDocumentos.Contains(doc.DocumentoID)).ToList()).ToList();
        }

        /// <summary>
        /// Obtiene las comunidades web de la persona según un documento que tenga en estas, el cual es pasado como parametro.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pIdentidadID">Identificador de persona</param>
        /// <param name="pDocumentoID">Identificador de documento</param>
        public void ObtenerComunidadesDePersonaPorDocumentoWEB(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pIdentidadID, Guid pDocumentoID)
        {
            pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursos().Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).Distinct().ToList();

            pDataWrapperDocumentacion.ListaBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursos().Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.BaseRecursos).Distinct().ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.BaseRecursosProyecto).Distinct().ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosUsuario = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosUsuario().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(item => item.BaseRecursosUsuario).ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosOrganizacion = mEntityContext.DocumentoWebVinBaseRecursos.JoinBaseRecursosOrganizacion().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(item => item.BaseRecursosOrganizacion).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene una lista con las bases de recursos en las que esta compartido un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Lista con las bases de recursos en las que esta compartido el documento</return s>
        public List<Guid> ObtenerBREstaCompartidoDocPorID(Guid pDocumentoID)
        {
            List<Guid> listaBR = new List<Guid>();
            listaBR = mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && !doc.Eliminado).Select(doc => doc.BaseRecursosID).Distinct().ToList();

            return listaBR;
        }

        /// <summary>
        /// Comprueba si un recurso está compartido en una comunidad y no está eliminado
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a comprobar</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se quiere comprobar que esté compartido el recurso</param>
        /// <returns>Verdad si el recurso está compartido o publicado en esa comunidad</return s>
        public bool EstaDocumentoCompartidoEnProyecto(Guid pDocumentoID, Guid pProyectoID)
        {
            return mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVinBaseRec => docWebVinBaseRec.DocumentoID, (doc, docWebVinBaseRec) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVinBaseRec
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (objeto, baseRecProy) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecProy
            }).Where(objeto => objeto.Documento.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !objeto.Documento.Eliminado && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Any();
        }

        /// <summary>
        /// Obtiene un diccionario con clave documento y valor laa lista con las bases de recursos en las que esta compartido un documento.
        /// </summary>
        ///<param name="pListaDocumentoID">Lista de documentos</param>
        /// <returns>Lista con las bases de recursos en las que estan compartidos los documentos</return s>
        public Dictionary<Guid, List<Guid>> ObtenerProyectosEstanCompartidosDocsPorID(List<Guid> pListaDocumentoID)
        {
            Dictionary<Guid, List<Guid>> listaDocProy = new Dictionary<Guid, List<Guid>>();
            if (pListaDocumentoID.Count > 0)
            {
                var resConsulta = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWeb => docWeb.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (docWeb, baseRecProy) => new
                {
                    DocumentoWebVinBaseRecursos = docWeb,
                    BaseRecursosProyecto = baseRecProy
                }).Where(objeto => pListaDocumentoID.Contains(objeto.DocumentoWebVinBaseRecursos.DocumentoID) && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => new { objeto.DocumentoWebVinBaseRecursos.DocumentoID, objeto.BaseRecursosProyecto.ProyectoID }).Distinct().ToList();
                foreach (var fila in resConsulta)
                {
                    Guid documentoID = fila.DocumentoID;
                    Guid baseRecursosID = fila.ProyectoID;

                    if (!listaDocProy.ContainsKey(documentoID))
                    {
                        List<Guid> listaProy = new List<Guid>();
                        listaProy.Add(baseRecursosID);
                        listaDocProy.Add(documentoID, listaProy);
                    }
                    else
                    {
                        listaDocProy[documentoID].Add(baseRecursosID);
                    }
                }
            }

            return listaDocProy;
        }

        /// <summary>
        /// Obtiene una lista con las bases de recursos en las que esta compartida una wiki.
        /// </summary>
        /// <param name="pNombreDoc">Identificador de documento</param>
        /// <returns>Lista con las bases de recursos en las que esta compartida la wiki</return s>
        public List<Guid> ObtenerBREstaCompartidoWikiPorNombre(string pNombreDoc)
        {
            List<Guid> listaBR = new List<Guid>();

            listaBR = mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.Documento, docWebVin => docWebVin.DocumentoID, doc => doc.DocumentoID, (docWebVin, doc) => new
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                Documento = doc
            }).Where(objeto => objeto.Documento.Titulo.Equals(pNombreDoc)).Select(objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID).Distinct().ToList();

            return listaBR;
        }

        /// <summary>
        /// Obtiene las comunidades web de la persona según un documento wiki que tenga en estas, el cual es pasado como parametro.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pIdentidadID">Identificador de persona</param>
        /// <param name="pNombreDoc">Nombre del documento Wiki</param>
        public void ObtenerComunidadesDePersonaPorNombreDocumentoWiki(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pIdentidadID, string pNombreDoc)
        {
            pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursos().JoinDocumento().Where(objeto => objeto.Documento.Titulo.Equals(pNombreDoc)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).Distinct().ToList();

            pDataWrapperDocumentacion.ListaBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursos().JoinDocumento().Where(objeto => objeto.Documento.Titulo.Equals(pNombreDoc)).Select(objeto => objeto.BaseRecursos).Distinct().ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().JoinDocumento().Where(objeto => objeto.Documento.Titulo.Equals(pNombreDoc)).Select(objeto => objeto.BaseRecursosProyecto).Distinct().ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosUsuario = mEntityContext.BaseRecursosUsuario.JoinDocumentoWebVinBaseRecursos().JoinDocumento().Where(item => item.Documento.Titulo.Equals(pNombreDoc)).Select(item => item.BaseRecursosUsuario).Distinct().ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosOrganizacion = mEntityContext.BaseRecursosOrganizacion.JoinDocumentoWebVinBaseRecursos().JoinDocumento().Where(item => item.Documento.Titulo.Equals(pNombreDoc)).Select(item => item.BaseRecursosOrganizacion).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene todas las categorías en la que está compartido un documento.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pListaDocumentos">Identificadores de documentos</param>
        public void ObtenerTodasCategoriasTesauroPorDocumentoID(DataWrapperDocumentacion pDataWrapperDocumentacion, List<Guid> pListaDocumentos)
        {
            if (pListaDocumentos.Count > 0)
            {
                pDataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = pDataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Union(mEntityContext.DocumentoWebAgCatTesauro.Where(docWebAg => pListaDocumentos.Contains(docWebAg.DocumentoID)).Distinct().ToList()).ToList();
            }
        }

        /// <summary>
        /// Actualiza el número de descargas y consultas de un documento.
        /// </summary>
        /// <param name="pDocID">Identificador del documento.</param>
        /// <param name="pNumVisitas">Número de visitas realizadas al documento</param>
        /// <param name="baseRecursosID">Identificados de BR de proyecto</param>
        public void ActualizarNumeroDescargasDocumento(Guid pDocumentoID, Guid pBaseRcursosID)
        {
            //Actualizar número de descargas en la tabla "Documento"
            Documento documento = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList().FirstOrDefault();
            documento.NumeroTotalDescargas += 1;
            if (pBaseRcursosID != Guid.Empty)
            {
                //Actualizar número de descargas en la tabla "DocumentoWebVinBR"
                DocumentoWebVinBaseRecursosExtra documentoWebVinBaseRecursosExtra = mEntityContext.DocumentoWebVinBaseRecursosExtra.Where(docWebVin => docWebVin.DocumentoID.Equals(pDocumentoID) && docWebVin.BaseRecursosID.Equals(pBaseRcursosID)).ToList().FirstOrDefault();
                documentoWebVinBaseRecursosExtra.NumeroDescargas += 1;
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Actualiza el número de descargas y consultas de un documento.
        /// </summary>
        /// <param name="pDocID">Identificador del documento.</param>
        /// <param name="pNumVisitas">Número de visitas realizadas al documento</param>
        /// <param name="baseRecursosID">Identificados de BR de proyecto</param>
        public int ActualizarNumeroConsultasDocumento(Guid pDocumentoID, int pNumVisitas, Guid pBaseRcursosID)
        {
            //Actualizar número de visitas y descargas en la tabla "Documento"

            Documento documento = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList().FirstOrDefault();
            documento.NumeroTotalConsultas = documento.NumeroTotalConsultas + pNumVisitas;
            // El número de visitas que nos interesa para actualizar el modelo es el numero de visitas del recurso en el proyecto.
            // La tabla que tiene las visitas por proyecto es DocumentoWebVinBaseRecursos
            int numeroTotalConsultasDocumento = -1;
            if (pBaseRcursosID != Guid.Empty)
            {
                //Actualizar número de visitas y descargas en la tabla "DocumentoWebVinBR"
                DocumentoWebVinBaseRecursosExtra documentoWebVinBaseRecursosExtra = mEntityContext.DocumentoWebVinBaseRecursosExtra.Where(docWebVin => docWebVin.DocumentoID.Equals(pDocumentoID) && docWebVin.BaseRecursosID.Equals(pBaseRcursosID)).ToList().FirstOrDefault();
                if (documentoWebVinBaseRecursosExtra != null)
                {
                    documentoWebVinBaseRecursosExtra.NumeroConsultas = documentoWebVinBaseRecursosExtra.NumeroConsultas + pNumVisitas;
                    numeroTotalConsultasDocumento = documentoWebVinBaseRecursosExtra.NumeroConsultas;
                }
            }
            mEntityContext.SaveChanges();
            return numeroTotalConsultasDocumento;
        }

        public void ActualizarNumeroComentariosDocumento(Guid pDocuementoID)
        {
            Documento documento = mEntityContext.Documento.Where(item => item.DocumentoID.Equals(pDocuementoID)).FirstOrDefault();

            if (documento != null)
            {
                int numeroComentarios = mEntityContext.DocumentoComentario.JoinComentario().Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocuementoID) && !item.Comentario.Eliminado).Count();

                documento.NumeroComentariosPublicos = numeroComentarios;

                List<DocumentoWebVinBaseRecursos> listaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(item => item.DocumentoID.Equals(pDocuementoID)).ToList();

                foreach (DocumentoWebVinBaseRecursos documentoWebVinBaseRecursos in listaDocumentoWebVinBaseRecursos)
                {
                    numeroComentarios = mEntityContext.DocumentoComentario.JoinBaseRecursosProyecto().JoinDocumentoWebVinBaseRecursos().JoinComentario().Where(item => !item.Comentario.Eliminado && item.DocumentoComentario.DocumentoID.Equals(pDocuementoID) && item.DocumentoWebVinBaseRecursos.BaseRecursosID.Equals(documentoWebVinBaseRecursos.BaseRecursosID)).Count();

                    documentoWebVinBaseRecursos.NumeroComentarios = numeroComentarios;
                }

                ActualizarBaseDeDatosEntityContext();
            }
            else
            {
                throw new Exception("No existe ningún documento con ese ID");
            }

        }

        /// <summary>
        /// Actualiza el número de comentarios de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos en la que se ha añadido o eliminado el comentario</param>
        /// <param name="pEliminado">Verdad si se ha eliminado un comentario, falso en caso de que sea agregado</param>
        /// <param name="pEnPrivado">Verdad si el comentario se ha hecho en una comunidad privada</param>
        public void ActualizarNumeroComentariosDocumento(Guid pDocumentoID, Guid pBaseRecursosID, bool pEliminado, bool pEnPrivado)
        {
            int suma = 1;

            if (pEliminado)
            {
                suma = -1;
            }

            if (!pEnPrivado)
            {
                Documento doc = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();
                if (doc != null)
                {
                    doc.NumeroComentariosPublicos = doc.NumeroComentariosPublicos + suma;
                }
                ActualizarBaseDeDatosEntityContext();
            }
            List<DocumentoWebVinBaseRecursos> listaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(docWebVin => docWebVin.DocumentoID.Equals(pDocumentoID) && docWebVin.BaseRecursosID.Equals(pBaseRecursosID)).ToList();
            foreach (DocumentoWebVinBaseRecursos docWebVin in listaDocumentoWebVinBaseRecursos)
            {
                docWebVin.NumeroComentarios = docWebVin.NumeroComentarios + suma;
            }

            ActualizarBaseDeDatosEntityContext();
        }
        public void EjecutarScriptCargaMasiva(string pScript)
        {
            var comando = ObtenerComando(pScript);
            ActualizarBaseDeDatos(comando, pEjecutarSiEsOracle: true, pEjecutarSiEsPostgres: true);
        }

        /// <summary>
        /// Actualiza el número de comentarios para un documento en una BR específica
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <param name="pBaseRecursosID"></param>
        /// <param name="pNumComentarios"></param>
        /// <param name="pEnPrivado"></param>
        public void ActualizarNumeroComentariosDocumento(Guid pDocumentoID, Guid pBaseRecursosID, int pNumComentarios, bool pEnPrivado)
        {
            if (!pEnPrivado)
            {
                Documento documento = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();
                if (documento != null)
                {
                    documento.NumeroComentariosPublicos = documento.NumeroComentariosPublicos + pNumComentarios;
                }
                ActualizarBaseDeDatosEntityContext();
            }
            List<DocumentoWebVinBaseRecursos> listaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(docWeb => docWeb.DocumentoID.Equals(pDocumentoID) && docWeb.BaseRecursosID.Equals(pBaseRecursosID)).ToList();
            foreach (DocumentoWebVinBaseRecursos docWebVin in listaDocumentoWebVinBaseRecursos)
            {
                docWebVin.NumeroComentarios = docWebVin.NumeroComentarios + pNumComentarios;
            }
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene el DocumentoWebVinBaseRecusros por el DocumentoID
        /// </summary>
        /// <param name="pDocumentoID">Identificador del Documento a obtener de la base de recursos</param>
        /// <returns>Devuelve el DocumentoWebVinBaseRecursos</returns>
        public DocumentoWebVinBaseRecursos ObtenerDocumentoWebVinBaseRecursoPorDocumentoID(Guid pDocumentoID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Where(item => item.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();
        }

        /// <summary>
        /// Devuelve las filas de los documentos compartio
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public List<DocumentoWebVinBaseRecursosConProyectoID> ObtenerFilasDocumentoWebVinBaseRecDeDocumento(Guid pDocumentoID)
        {
            List<DocumentoWebVinBaseRecursosConProyectoID> listaDocumentoWebVinBaseRecursosConProyectoID = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).ToList().Select(objeto => new DocumentoWebVinBaseRecursosConProyectoID
            {
                DocumentoID = objeto.DocumentoWebVinBaseRecursos.DocumentoID,
                BaseRecursosID = objeto.DocumentoWebVinBaseRecursos.BaseRecursosID,
                IdentidadPublicacionID = objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID,
                FechaPublicacion = objeto.DocumentoWebVinBaseRecursos.FechaPublicacion,
                TipoPublicacion = objeto.DocumentoWebVinBaseRecursos.TipoPublicacion,
                LinkAComunidadOrigen = objeto.DocumentoWebVinBaseRecursos.LinkAComunidadOrigen,
                Eliminado = objeto.DocumentoWebVinBaseRecursos.Eliminado,
                NumeroComentarios = objeto.DocumentoWebVinBaseRecursos.NumeroComentarios,
                NumeroVotos = objeto.DocumentoWebVinBaseRecursos.NumeroVotos,
                PermiteComentarios = objeto.DocumentoWebVinBaseRecursos.PermiteComentarios,
                Rank = objeto.DocumentoWebVinBaseRecursos.Rank,
                Rank_Tiempo = objeto.DocumentoWebVinBaseRecursos.Rank_Tiempo,
                IndexarRecurso = objeto.DocumentoWebVinBaseRecursos.IndexarRecurso,
                PrivadoEditores = objeto.DocumentoWebVinBaseRecursos.PrivadoEditores,
                ProyectoID = objeto.BaseRecursosProyecto.ProyectoID
            }).Distinct().ToList();

            return listaDocumentoWebVinBaseRecursosConProyectoID;
        }

        /// <summary>
        /// Devuelve las filas de los documentos compartio
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos</param>
        /// <param name="pListaDocumentosID">Lista con los documentos</param>
        public void ObtenerFilasDocumentoWebDeBRDeListaDocumentos(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pBaseRecursosID, List<Guid> pListaDocumentosID)
        {
            if (pListaDocumentosID.Count > 0)
            {
                pDataWrapperDocumentacion.ListaDocumento = pDataWrapperDocumentacion.ListaDocumento.Concat(mEntityContext.Documento.Where(doc => pListaDocumentosID.Contains(doc.DocumentoID)).Distinct()).ToList().Distinct().ToList();

                pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Union(mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(pBaseRecursosID) && pListaDocumentosID.Contains(doc.DocumentoID)).Distinct()).ToList();

                pDataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = pDataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Union(mEntityContext.DocumentoWebAgCatTesauro.Where(doc => doc.BaseRecursosID.Equals(pBaseRecursosID) && pListaDocumentosID.Contains(doc.DocumentoID)).Distinct()).ToList();
            }
        }

        /// <summary>
        /// Obtiene los tags de varios documentos
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></return s>
        public Dictionary<Guid, string> ObtenerTagsDeDocumentos(List<Guid> pListaDocumentoID)
        {
            Dictionary<Guid, string> tags = new Dictionary<Guid, string>();

            foreach (Guid id in pListaDocumentoID)
            {
                tags.Add(id, "");
            }

            if (pListaDocumentoID.Count > 0)
            {
                var resultado = mEntityContext.Documento.Where(doc => pListaDocumentoID.Contains(doc.DocumentoID)).Select(doc => new { doc.DocumentoID, doc.Tags }).ToList().Distinct().ToList();

                foreach (var fila in resultado)
                {
                    if (!string.IsNullOrEmpty(fila.Tags))
                    {
                        Guid idDoc = fila.DocumentoID;
                        string tagss = fila.Tags;

                        tags[idDoc] = tagss;
                    }
                }

            }
            return tags;
        }


        /// <summary>
        /// Obtiene los tags de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></return s>
        public string ObtenerTagsDeDocumento(Guid pDocumentoID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pDocumentoID);
            return ObtenerTagsDeDocumentos(lista)[pDocumentoID];
        }

        /// <summary>
        /// Obtiene el título, la descripción y los tags de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Fila del documento con el título la descripción y los tags cargados</return s>
        public Documento ObtenerTituloDescripcionTagsDeDocumento(Guid pDocumentoID)
        {
            //string select = "SELECT Documento.DocumentoID, Documento.Titulo, Documento.Descripcion, Documento.Tags, Documento.ElementoVinculadoID FROM Documento WHERE Documento.DocumentoID = " + IBD.ToParam("DocumentoID"); 
            return mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).ToList().FirstOrDefault();

        }

        /// <summary>
        /// Obtiene la lista de categorías de tesauro vinculadas con un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador de un documento</param>
        /// <returns></return s>
        public List<string> ObtenerNombresCategoriasVinculadoDocumento(Guid pDocumentoID)
        {
            return mEntityContext.DocumentoWebAgCatTesauro.JoinCategoriaTesauro().Where(item => item.DocumentoWebAgCatTesaruo.DocumentoID.Equals(pDocumentoID)).Select(item => item.CategoriaTesauro.Nombre).ToList();
        }

        /// <summary>
        /// Obtiene el nombre de la entidad vinculada de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de un documento</param>
        /// <returns>Nombre de la entidad vinculada de un documento</return s>
        public string ObtenerNombreElementoVinculadoDocumento(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).Select(documento => documento.NombreElementoVinculado).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el campo 'NombreCategoriaDoc' de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de un documento</param>
        /// <returns>Campo 'NombreCategoriaDoc' de un documento</return s>
        public string ObtenerNombreCategoriaDocDocumento(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).Select(documento => documento.NombreCategoriaDoc).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el creador del documentoID pasado como parámetro.
        /// </summary>
        /// <param name="pDocID">DocumentoID del que queremos obtener el creador.</param>
        /// <returns>CreadorID del documento.</return s>
        public Guid ObtenerCreadorDocumentoID(Guid pDocID)
        {
            Guid? creadorID = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocID)).Select(doc => doc.CreadorID).FirstOrDefault();
            if (creadorID.HasValue)
            {
                return creadorID.Value;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Dictionary<Guid, string> ObtenerEmailCreadoresDocumentosID(List<Guid> pDocsID)
        {
            Dictionary<Guid, string> listaCreadores = new Dictionary<Guid, string>();

            if (pDocsID.Count > 0)
            {
                var resultado = mEntityContext.Documento.JoinDocumentoIdentidad().JoinPerfil().JoinPersona().Where(objeto => pDocsID.Contains(objeto.Documento.DocumentoID)).Select(objeto => new { DocumentoID = objeto.Documento.DocumentoID, Email = objeto.Persona.Email }).ToList();

                foreach (var fila in resultado)
                {
                    listaCreadores.Add(fila.DocumentoID, fila.Email);
                }
            }

            return listaCreadores;
        }

        /// <summary>
        /// Devuelve una lista con los últimos ids de los recursos publicados en la comunidad pProyID
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que se quieren ver los últimos recursos publicados.</param>
        /// <param name="pNumElementos">Número de elementos máximo a traer</param>
        /// <param name="pOntologia">(Opcional) Ontología por la que se quiere filtrar</param>
        /// <returns>Lista con los últimos ids de los recursos publicados.</return s>
        public List<Guid> ObtenerUltimosRecursosIDPublicados(Guid pProyectoID, int pNumElementos, string pOntologia = null)
        {
            List<Guid> listaDocumentos = new List<Guid>();

            var listaIDsObjeto = mEntityContext.Documento.Join(mEntityContext.OntologiaProyecto, doc => new { Enlace = doc.Enlace }, onto => new { Enlace = onto.OntologiaProyecto1 + ".owl" }, (doc, onto) => new
            {
                Documento = doc,
                OntologiaProyecto = onto
            }).Where(objeto => objeto.OntologiaProyecto.ProyectoID.Equals(pProyectoID) && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && objeto.OntologiaProyecto.EsBuscable);

            if (!string.IsNullOrEmpty(pOntologia))
            {
                List<string> ontologias = pOntologia.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                listaIDsObjeto = listaIDsObjeto.Where(objeto => ontologias.Contains(objeto.OntologiaProyecto.OntologiaProyecto1));
            }

            var resultado = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Where(objeto => !objeto.Documento.Borrador && !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.ProyectoID.HasValue && objeto.Documento.ProyectoID.Value.Equals(pProyectoID) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria) && (!objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Semantico) || (objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Semantico) && objeto.Documento.ElementoVinculadoID.HasValue && listaIDsObjeto.Select(item => item.Documento.DocumentoID).Contains(objeto.Documento.ElementoVinculadoID.Value))));

            listaDocumentos = resultado.Where(objeto => objeto.Documento.FechaCreacion.HasValue).OrderByDescending(objeto => objeto.Documento.FechaCreacion.Value).Select(objeto => objeto.Documento.DocumentoID).Take(pNumElementos).ToList();

            return listaDocumentos;
        }

        /// <summary>
        /// Devuelve un DS con los últimos recursos publicados por un perfil
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que se quieren ver los últimos recursos publicados.</param>
        /// <returns>DS Con los últimos recursos publicados.</return s>
        [Obsolete]
        public DataWrapperDocumentacion ObtenerUltimosRecursosPublicadosPorPerfil(Guid pPerfilID, Guid pUsuarioID, int pNumElementos)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            ObtenerBaseRecursosUsuario(dataWrapperDocumentacion, pUsuarioID);

            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.Identidad, objeto => new { IdentidadID = objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value }, identidad => new
            {
                IdentidadID = identidad.IdentidadID
            }, (objeto, identidad) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Identidad = identidad
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.HasValue && !objeto.Documento.Borrador && !objeto.Documento.Eliminado && !objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Identidad.PerfilID.Equals(pPerfilID)).OrderByDescending(objeto => objeto.Documento.FechaModificacion).Select(objeto => objeto.Documento).OrderByDescending(item => item.FechaModificacion).Take(pNumElementos).ToList().Distinct().ToList();

            if (dataWrapperDocumentacion.ListaDocumento.Count > 0)
            {
                List<Guid> listaIDS = dataWrapperDocumentacion.ListaDocumento.Select(doc => doc.DocumentoID).ToList();
                dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(docWebVin => listaIDS.Contains(docWebVin.DocumentoID)).ToList();
            }
            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Devuelve un DS con los últimos recursos publicados por un perfil
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que se quieren ver los últimos recursos publicados.</param>
        /// <returns>DS Con los últimos recursos publicados.</return s>
        public DataWrapperDocumentacion ObtenerUltimosRecursosPublicados(Guid pProyectoID, int pNumElementos)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            ObtenerBaseRecursosUsuario(dataWrapperDocumentacion, pProyectoID);

            //Documento
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(item => item.Documento.Borrador == false && item.Documento.Eliminado == false && item.Documento.UltimaVersion == true && item.DocumentoWebVinBaseRecursos.Eliminado == false && item.DocumentoWebVinBaseRecursos.PrivadoEditores == false && item.Documento.ProyectoID.Equals(pProyectoID)).Select(item => item.Documento).OrderBy(item => item.FechaModificacion).ToList().Distinct().ToList();

            if (dataWrapperDocumentacion.ListaDocumento.Count > 0)
            {
                List<Guid> listaDocumentos = dataWrapperDocumentacion.ListaDocumento.Select(doc => doc.DocumentoID).ToList();

                //DocumentoWebVinBaseRecursos
                dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(doc => listaDocumentos.Contains(doc.DocumentoID)).ToList();
            }
            return dataWrapperDocumentacion;
        }


        /// <summary>
        /// Devuelve un DS con los últimos debates y preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerUltimosDebatesPublicados(Guid pProyectoID, int pNumElementos)
        {
            return ObtenerUltimosDebatesPreguntasPublicados(pProyectoID, pNumElementos, true, false);
        }

        /// <summary>
        /// Devuelve un DS con los últimos debates y preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerUltimasPreguntasPublicados(Guid pProyectoID, int pNumElementos)
        {
            return ObtenerUltimosDebatesPreguntasPublicados(pProyectoID, pNumElementos, false, true);
        }

        /// <summary>
        /// Devuelve un DS con los últimos debates y preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerUltimosDebatesPreguntasPublicados(Guid pProyectoID, int pNumElementos)
        {
            return ObtenerUltimosDebatesPreguntasPublicados(pProyectoID, pNumElementos, true, true);
        }

        /// <summary>
        /// Devuelve un DS con los últimos debates y preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns></return s>
        private DataWrapperDocumentacion ObtenerUltimosDebatesPreguntasPublicados(Guid pProyectoID, int pNumElementos, bool pTraerDebates, bool pTraerPreguntas)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, pProyectoID);

            var listaDocumentos = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Where(objeto => !objeto.Documento.Borrador && !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.ProyectoID.Value.Equals(pProyectoID) && objeto.Documento.ProyectoID.HasValue).Select(objeto => objeto.Documento);
            if (pTraerDebates && pTraerPreguntas)
            {
                listaDocumentos = listaDocumentos.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Debate) || doc.Tipo.Equals((short)TiposDocumentacion.Pregunta));
            }
            else if (pTraerDebates)
            {
                listaDocumentos = listaDocumentos.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Debate));
            }
            else if (pTraerPreguntas)
            {
                listaDocumentos = listaDocumentos.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Pregunta));
            }
            dataWrapperDocumentacion.ListaDocumento = listaDocumentos.OrderByDescending(doc => doc.FechaModificacion).Take(pNumElementos).ToList();
            if (dataWrapperDocumentacion.ListaDocumento.Count > 0)
            {
                List<Guid> listaDocumentosID = dataWrapperDocumentacion.ListaDocumento.Select(doc => doc.DocumentoID).ToList();
                dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(docWebVin => listaDocumentosID.Contains(docWebVin.DocumentoID)).ToList();
            }
            return dataWrapperDocumentacion;
        }


        /// <summary>
        /// Devuelve una lista con los IDs de los últimos debates y preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns>Lista de documentoID</return s>
        public List<Guid> ObtenerListaUltimosDebatesPreguntasPublicados(Guid pProyectoID, int pNumElementos, bool pTraerDebates, bool pTraerPreguntas)
        {
            var consultaSinTipo = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().Where(objeto => !objeto.Documento.Borrador && !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.ProyectoID.Value.Equals(pProyectoID) && objeto.Documento.FechaModificacion.HasValue).OrderByDescending(objeto => objeto.Documento.FechaModificacion.Value);

            List<Guid> listaDocs = new List<Guid>();
            if (pTraerDebates && pTraerPreguntas)
            {
                listaDocs = consultaSinTipo.Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) || objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta)).Select(objeto => objeto.Documento.DocumentoID).Distinct().Take(pNumElementos).ToList();
            }
            else if (pTraerDebates)
            {
                listaDocs = consultaSinTipo.Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate)).Select(objeto => objeto.Documento.DocumentoID).Distinct().Take(pNumElementos).ToList();
            }
            else if (pTraerPreguntas)
            {
                listaDocs = consultaSinTipo.Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta)).Select(objeto => objeto.Documento.DocumentoID).Distinct().Take(pNumElementos).ToList();
            }

            return listaDocs;
        }

        /// <summary>
        /// Actualiza la fecha del documento de la base de recursos pasada
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <param name="pBaseRecursosID"></param>
        /// <param name="pFechaUltimaVisita"></param>
        public void ActualizarFechaUltimaVisitaDocumento(Guid pDocumentoID, Guid pBaseRecursosID, DateTime pFechaUltimaVisita)
        {
            DocumentoWebVinBaseRecursosExtra documentoWebVinBaseRecursosExtra = mEntityContext.DocumentoWebVinBaseRecursosExtra.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.BaseRecursosID.Equals(pBaseRecursosID)).FirstOrDefault();
            documentoWebVinBaseRecursosExtra.FechaUltimaVisita = pFechaUltimaVisita;
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene los campos Titulo, ElementoVinculadoID y Tipo de un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a obtener sus datos</param>
        /// <param name="pTitulo">Variable en la que se devovlerá el título del recurso</param>
        /// <param name="pElementoVinculadoID">Variable en la que se devolverá el campo ElementoVinculadoID (NULL si no tiene valor)</param>
        /// <param name="pTipo">Variable en la que se devolverá el campo Tipo del recurso</param>
        public void ObtenerTituloElementoVinculadoIDTipoDeRecurso(Guid pDocumentoID, out string pTitulo, out Guid? pElementoVinculadoID, out short pTipo)
        {
            pTitulo = "";
            pElementoVinculadoID = null;
            pTipo = 0;

            Documento documento = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();
            if (documento != null)
            {
                pTitulo = documento.Titulo;
                pElementoVinculadoID = documento.ElementoVinculadoID;
                pTipo = documento.Tipo;
            }

        }

        #endregion

        #region Base de Recursos

        /// <summary>
        /// Obtiene la base de recursos de un determinado proyecto.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacionID">Identificador de organziación</param>
        public void ObtenerBaseRecursosProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, Guid pOrganizacionID)
        {
            pDataWrapperDocumentacion.ListaBaseRecursos = mEntityContext.BaseRecursosProyecto.Where(baseRecProy => baseRecProy.ProyectoID.Equals(pProyectoID) && baseRecProy.OrganizacionID.Equals(pOrganizacionID)).ToList().Select(baseRecProy => new BaseRecursos { BaseRecursosID = baseRecProy.BaseRecursosID }).ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Where(baseRecProy => baseRecProy.ProyectoID.Equals(pProyectoID) && baseRecProy.OrganizacionID.Equals(pOrganizacionID)).ToList();
        }

        /// <summary>
        /// Obtiene la base de recursos de un determinado proyecto.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public void ObtenerBaseRecursosProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID)
        {
            pDataWrapperDocumentacion.ListaBaseRecursos = mEntityContext.BaseRecursosProyecto.Where(baseRecProy => baseRecProy.ProyectoID.Equals(pProyectoID)).ToList().Select(baseRecProy => new BaseRecursos { BaseRecursosID = baseRecProy.BaseRecursosID }).ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Where(baseRecProy => baseRecProy.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Comprueba si una comunidad contiene el recurso o no (un recurso sin eliminado lógico)
        /// </summary>
        /// <param name="id">Identificador del recurso</param>
        /// <param name="pOrganizacionDestinoID">Identificador de la organizacion de búsqueda</param>
        /// <param name="pProyectoDestinoID">Identificador de la comunidad de búsqueda</param>
        /// <returns>Booleano true si esa comunidad contiene ese recurso, false en caso contrario</return s>
        public bool ContieneRecurso(Guid id, Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().Where(objeto => !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(id) && objeto.BaseRecursosProyecto.OrganizacionID.Equals(pOrganizacionID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Any();
        }

        /// <summary>
        /// Obtiene los ID de las bases de recursos de proyectos.(Proy,BR)
        /// </summary>
        /// <param name="pListaProyectosID">Identificadores de proyecto</param>
        public Dictionary<Guid, Guid> ObtenerBasesRecursosIDporProyectosID(List<Guid> pListaProyectosID)
        {
            Dictionary<Guid, Guid> listaProyBR = new Dictionary<Guid, Guid>();
            if (pListaProyectosID.Count > 0)
            {
                var resultado = mEntityContext.BaseRecursosProyecto.Where(baseRecProy => pListaProyectosID.Contains(baseRecProy.ProyectoID)).Select(baseRecProy => new { baseRecProy.BaseRecursosID, baseRecProy.OrganizacionID, baseRecProy.ProyectoID }).ToList();

                foreach (var fila in resultado)
                {
                    if (!listaProyBR.ContainsKey(fila.ProyectoID))
                    {
                        listaProyBR.Add(fila.ProyectoID, fila.BaseRecursosID);
                    }
                }
            }
            return listaProyBR;
        }

        /// <summary>
        /// Obtiene los ID de las bases de recursos de organizaciones (Org,BR).
        /// </summary>
        /// <param name="pListaOrgsID">Identificadores de organizaciones</param>
        public Dictionary<Guid, Guid> ObtenerBasesRecursosIDporOrganizacionesID(List<Guid> pListaOrgsID)
        {
            Dictionary<Guid, Guid> listaOrgBR = new Dictionary<Guid, Guid>();
            if (pListaOrgsID.Count > 0)
            {
                List<BaseRecursosOrganizacion> listaBaseRecursosOrganizacion = mEntityContext.BaseRecursosOrganizacion.Where(item => pListaOrgsID.Contains(item.OrganizacionID)).ToList();

                foreach (BaseRecursosOrganizacion fila in listaBaseRecursosOrganizacion)
                {
                    if (!listaOrgBR.ContainsKey(fila.OrganizacionID))
                    {
                        listaOrgBR.Add(fila.OrganizacionID, fila.BaseRecursosID);
                    }
                }
            }
            return listaOrgBR;
        }

        /// <summary>
        /// Obtiene la base de recursos de un usuario.
        /// </summary>
        /// <param name="pDocumentacionDS">DataSet de documentación</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void ObtenerBaseRecursosUsuario(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pUsuarioID)
        {
            pDataWrapperDocumentacion.ListaBaseRecursos = mEntityContext.BaseRecursos.Join(mEntityContext.BaseRecursosUsuario, baseRec => baseRec.BaseRecursosID, baseRecUs => baseRecUs.BaseRecursosID, (baseRec, baseRecUs) => new
            {
                BaseRecursos = baseRec,
                UsuarioID = baseRecUs.UsuarioID
            }).Where(objeto => objeto.UsuarioID.Equals(pUsuarioID)).Select(objeto => objeto.BaseRecursos).ToList();

            pDataWrapperDocumentacion.ListaBaseRecursosUsuario = mEntityContext.BaseRecursosUsuario.Where(baseUs => baseUs.UsuarioID.Equals(pUsuarioID)).ToList();
        }

        /// <summary>
        /// Obtiene la clave de la base de recursos de un usuario.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Clave de la base de recursos del usuario</return s>
        public Guid ObtenerBaseRecursosIDUsuario(Guid pUsuarioID)
        {
            return mEntityContext.BaseRecursos.JoinBaseRecursosUsuario().Where(item => item.BaseRecursosUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.BaseRecursos.BaseRecursosID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la clave de la base de recursos de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Clave de la base de recursos del proyecto</return s>
        public Guid ObtenerBaseRecursosIDProyecto(Guid pProyectoID)
        {
            Guid baseRecursosID = Guid.Empty;
            BaseRecursosProyecto baseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Where(baseRecProy => baseRecProy.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (baseRecursosProyecto != null)
            {
                baseRecursosID = baseRecursosProyecto.BaseRecursosID;
            }

            return baseRecursosID;
        }

        /// <summary>
        /// Obtiene la clave de la base de recursos de una Organizacion.
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la Organizacion</param>
        /// <returns>Clave de la base de recursos de la Organizacion</return s>
        public Guid ObtenerBaseRecursosIDOrganizacion(Guid pOrganizacionID)
        {
            return mEntityContext.BaseRecursos.JoinBaseRecursosOrganizacion().Where(item => item.BaseRecursosOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.BaseRecursos.BaseRecursosID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las meta etiquetas del documento indicado
        /// </summary>
        /// <param name="pDocumentoClave">Identificador del documento a obtener las meta etiquetas</param>
        /// <returns></returns>
        public DocumentoMetaDatos ObtenerEtiquetasMeta(Guid pDocumentoClave)
        {
            DocumentoMetaDatos documentoMetaDatos = mEntityContext.DocumentoMetaDatos.FirstOrDefault(x => x.DocumentoID == pDocumentoClave);
            return documentoMetaDatos;
        }

        public List<Guid> ObtenerDocumentosIDSuscripcionPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID, int pNumElementos)
        {
            List<Guid> listaDocumentos = new List<Guid>();

            var primeraParteQuery = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebAgCatTesauro().JoinCategoriaTesVinSuscrip().JoinSuscripcion().JoinIdentidad().JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Documento.Borrador.Equals(false) && item.Documento.Eliminado.Equals(false) && item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(false) && item.DocumentoWebVinBaseRecursos.Eliminado.Equals(false)).Select(item => new
            {
                DocumentoID = item.DocumentoWebVinBaseRecursos.DocumentoID,
                FechaPublicacion = item.DocumentoWebVinBaseRecursos.FechaPublicacion
            });

            var segundaParteQuery = mEntityContext.DocumentoWebVinBaseRecursos.JoinSuscripcionIdentidadProyecto().JoinSuscripcion().JoinIdentidad().JoinDocumento().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Documento.Borrador.Equals(false) && item.Documento.Eliminado.Equals(false) && item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(false) && item.DocumentoWebVinBaseRecursos.Eliminado.Equals(false)).Select(item => new
            {
                DocumentoID = item.DocumentoWebVinBaseRecursos.DocumentoID,
                FechaPublicacion = item.DocumentoWebVinBaseRecursos.FechaPublicacion
            });

            var query = primeraParteQuery.Take(pNumElementos).OrderByDescending(item => item.FechaPublicacion).Concat(segundaParteQuery.Take(pNumElementos).OrderByDescending(item => item.FechaPublicacion)).Take(pNumElementos).OrderByDescending(item => item.FechaPublicacion);

            foreach (var objeto in query.Distinct().ToList())
            {
                Guid documentoID = objeto.DocumentoID;
                if (!listaDocumentos.Contains(documentoID))
                {
                    listaDocumentos.Add(documentoID);
                }
            }

            return listaDocumentos;
        }

        public List<Guid> ObtenerDocumentosIDActividadRecienteEnProyecto(Guid pProyectoID)
        {
            //TODO: Obtener los documentos que no sean compartidos
            List<Guid> listaDocumentos = new List<Guid>();

            var primeraParteConsulta = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumento().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !item.Documento.Borrador && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.PrivadoEditores && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.TipoPublicacion == 0).Select(item => new
            {
                DocumentoID = item.Documento.DocumentoID,
                ElementoVinculadoID = item.Documento.ElementoVinculadoID,
                Fecha = !item.DocumentoWebVinBaseRecursos.FechaCertificacion.HasValue ? item.DocumentoWebVinBaseRecursos.FechaPublicacion.Value : item.DocumentoWebVinBaseRecursos.FechaCertificacion.Value
            });

            var segundaParteConsulta = mEntityContext.Comentario.JoinDocumentoComentario().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !item.Documento.Borrador && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.PrivadoEditores && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.TipoPublicacion == 0 && !item.Comentario.Eliminado).Select(item => new
            {
                DocumentoID = item.Documento.DocumentoID,
                ElementoVinculadoID = item.Documento.ElementoVinculadoID,
                Fecha = item.Comentario.Fecha
            });

            var terceraParteConsulta = mEntityContext.VotoDocumento.JoinVoto().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.DocumentoWebVinBaseRecursos.FechaCertificacion.HasValue && !item.Documento.Borrador && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.PrivadoEditores && !item.DocumentoWebVinBaseRecursos.Eliminado && item.DocumentoWebVinBaseRecursos.TipoPublicacion == 0).GroupBy(item => new { DocumentoID = item.Documento.DocumentoID, ElementoVinculadoID = item.Documento.ElementoVinculadoID.Value }).Select(item => new
            {
                DocumentoID = item.Key.DocumentoID,
                ElementoVinculadoID = (Guid?)item.Key.ElementoVinculadoID,
                Fecha = item.Max(item2 => item2.Voto.FechaVotacion.Value)
            });

            var temp = primeraParteConsulta.Concat(segundaParteConsulta).Concat(terceraParteConsulta);

            var subconsulta = mEntityContext.Documento.Join(mEntityContext.OntologiaProyecto, documento => new { ProyectoID = documento.ProyectoID.Value, Enlace = documento.Enlace }, ontologiaProyecto => new { ProyectoID = ontologiaProyecto.ProyectoID, Enlace = ontologiaProyecto.OntologiaProyecto1 + ".owl" }, (documento, ontologiaProyecto) => new
            {
                Documento = documento,
                OntologiaProyecto = ontologiaProyecto
            }).Where(item => item.Documento.Tipo.Equals(7) && item.OntologiaProyecto.EsBuscable && item.Documento.ProyectoID.Value.Equals(pProyectoID)).Select(item => item.Documento.DocumentoID);

            var query = temp.Where(item => (!item.ElementoVinculadoID.HasValue || item.ElementoVinculadoID.Equals(Guid.Empty) || subconsulta.Contains(item.ElementoVinculadoID.Value)) && !mEntityContext.VersionDocumento.Select(item => item.DocumentoID).Contains(item.DocumentoID)).OrderByDescending(item => item.Fecha).GroupBy(item => item.DocumentoID).Select(item => item.Key);

            listaDocumentos = query.Distinct().ToList();

            return listaDocumentos;
        }

        /// <summary>
        /// Obtiene la base de recursos de una organización.
        /// </summary>
        /// <param name="pDocumentacionDS">DataSet de documentación</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        public void ObtenerBaseRecursosOrganizacion(DataWrapperDocumentacion pDocumentacionDS, Guid pOrganizacionID)
        {
            pDocumentacionDS.ListaBaseRecursos = mEntityContext.BaseRecursos.Join(mEntityContext.BaseRecursosOrganizacion, baseRec => baseRec.BaseRecursosID, baseRecOrg => baseRecOrg.BaseRecursosID, (baseRec, baseRecOrg) => new
            {
                BaseRecursos = baseRec,
                BaseRecursosOrganizacion = baseRecOrg
            }).Where(objeto => objeto.BaseRecursosOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(objeto => objeto.BaseRecursos).ToList();

            pDocumentacionDS.ListaBaseRecursosOrganizacion = mEntityContext.BaseRecursosOrganizacion.Where(baseRecOrg => baseRecOrg.OrganizacionID.Equals(pOrganizacionID)).ToList();
        }

        /// <summary>
        /// Obtiene el ID de un recurso a traves de su nombre y del proyecto al que pertenece
        /// </summary>
        /// <param name="pNombre">Nombre del documento</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Identificador de documento</return s>
        public Guid ObtenerDocumentoIDPorNombreYProyecto(string pNombre, Guid pProyectoID)
        {
            Documento documento = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().JoinDocumento().Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && objeto.Documento.Titulo.ToLower().Equals(pNombre.ToLower())).Select(objeto => objeto.Documento).FirstOrDefault();
            if (documento != null)
            {
                return documento.DocumentoID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Obtiene el Identificador del proyecto en el que se ha creado un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Identificador de proyecto</return s>
        public Guid ObtenerProyectoIDPorDocumentoID(Guid pDocumentoID)
        {
            Guid? resultado = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).Select(doc => doc.ProyectoID).ToList().FirstOrDefault();

            if (!resultado.HasValue)
            {
                return Guid.Empty;
            }
            else
            {
                return resultado.Value;
            }
        }

        /// <summary>
        /// Obtiene el Enlace de un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></return s>
        public string ObtenerEnlaceDocumentoPorDocumentoID(Guid pDocumentoID)
        {
            string resultado = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID) && documento.Enlace != null).Select(documento => documento.Enlace).FirstOrDefault();
            if (!string.IsNullOrEmpty(resultado))
            {
                return resultado;
            }

            return "";
        }

        /// <summary>
        /// Obtiene los Enlaces de varios recursos
        /// </summary>
        /// <param name="pListaDocumentoID">Identificadores de los documentos</param>
        /// <returns></return s>
        public Dictionary<Guid, string> ObtenerEnlacesDocumentosPorDocumentoID(List<Guid> pListaDocumentoID)
        {
            Dictionary<Guid, string> lista = new Dictionary<Guid, string>();

            if (pListaDocumentoID.Count > 0)
            {
                var resultado = mEntityContext.Documento.Where(documento => pListaDocumentoID.Contains(documento.DocumentoID) && documento.Enlace != null).Select(documento => new { documento.DocumentoID, documento.Enlace }).ToList();

                foreach (var fila in resultado)
                {
                    lista.Add(fila.DocumentoID, fila.Enlace);
                }

            }

            return lista;
        }

        /// <summary>
        /// Obtiene el ID de un recurso a traves de su nombre y de la persona a la que pertenece
        /// </summary>
        /// <param name="pNombre">Nombre del documento</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns></return s>
        public Guid ObtenerDocumentoIDPorNombreYPersona(string pNombre, Guid pPersonaID)
        {
            Documento documento = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario().JoinDocumento().JoinPersona().Where(objeto => objeto.Persona.PersonaID.Equals(pPersonaID) && objeto.Documento.Titulo.ToLower().Equals(pNombre.ToLower()) && objeto.Documento.UltimaVersion && !objeto.Documento.Eliminado).Select(objeto => objeto.Documento).FirstOrDefault();
            if (documento != null)
            {
                return documento.DocumentoID;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Devuelve las vinculaciones de los documentos de una organziación. Carga "DocumentoWebVinBaseRecursos" , "DocumentoWebAgCatTesauro"
        /// </summary>
        /// <param name="pOrganizacionID">Clave de la organizacion</param>
        /// <returns>DocumentacionDS</return s>
        public DataWrapperDocumentacion ObtenerVinculacionesDocumentosDeOrganizacion(Guid pOrganizacionID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosOrganizacion().Where(objeto => objeto.BaseRecursosOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            //DocumentoWebAgCatTesauro
            dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinBaseRecursosOrganizacion().Where(objeto => objeto.BaseRecursosOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Verdad si el documento está marcado como privado para editores
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Verdad si el documento está marcado como privado para editores</return s>
        public bool EsDocumentoEnProyectoPrivadoEditores(Guid pDocumentoID, Guid pProyectoID)
        {
            if (!pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                return mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto().Where(objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.DocumentoWebVinBaseRecursos.PrivadoEditores).FirstOrDefault();
            }
            else
            {
                return mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).Select(doc => doc.Publico).FirstOrDefault();
            }
        }

        /// <summary>
        /// Verdad si el documento está marcado como publico sólo para los miembros de la comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Verdad si el documento está marcado como publico sólo para los miembros de la comunidad</return s>
        public bool EsDocumentoEnProyectoPublicoSoloParaMiembros(Guid pDocumentoID)
        {
            short visibilidad = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).Select(doc => doc.Visibilidad).FirstOrDefault();
            return (visibilidad.Equals((short)VisibilidadDocumento.PrivadoMiembrosComunidad));
        }

        /// <summary>
        /// Obtiene la lista de categorias de un recurso en una BR de una persona.
        /// </summary>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <returns>Lista de categorias de un recurso en una BR de una persona</return s>
        public List<Guid> ObtenerListaIDsCategoriasBRPersonalPersonsaDeDocumento(Guid pPersonaID, Guid pDocumentoID)
        {
            return mEntityContext.DocumentoWebAgCatTesauro.Join(mEntityContext.BaseRecursosUsuario, agCat => agCat.BaseRecursosID, baseRecUs => baseRecUs.BaseRecursosID, (agCat, baseRecUs) => new
            {
                DocumentoWebAgCatTesauro = agCat,
                BaseRecursosUsuario = baseRecUs
            }).Join(mEntityContext.Persona, objeto => objeto.BaseRecursosUsuario.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new
            {
                DocumentoWebAgCatTesauro = objeto.DocumentoWebAgCatTesauro,
                BaseRecursosUsuario = objeto.BaseRecursosUsuario,
                Persona = persona
            }).Where(objeto => objeto.Persona.PersonaID.Equals(pPersonaID) && objeto.DocumentoWebAgCatTesauro.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.DocumentoWebAgCatTesauro.CategoriaTesauroID).ToList();

        }

        /// <summary>
        /// Devuelve true si el enlace de un recurso esta en la comunidad dada, false si no contiene ese enlace (recurso)
        /// </summary>
        /// <param name="pEnlace">Url del enlace del recurso</param>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        /// <returns>Devuelve si está o no el documento con "pEnlace" en la comunidad "pProyectoID"</return s>
        public bool EstaEnlaceEnComunidad(string pEnlace, Guid pProyectoID)
        {
            return mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && objeto.Documento.Descripcion.Contains(pEnlace)).Any();
        }

        /// <summary>
        /// Devuleve el estado del documento 0 ->No eliminado, 1 -> Eliminado, 2 -> No existe
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Devuleve el estado del documento</return s>
        public int EstaDocumentoEliminado(Guid pDocumentoID)
        {
            Documento doc = mEntityContext.Documento.Where(documento => documento.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();
            int resultado;
            if (doc == null)
            {
                resultado = 2;
            }
            else if (doc.Eliminado)
            {
                resultado = 1;
            }
            else
            {
                resultado = 0;
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene los editores de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerEditoresDocumento(Guid pDocumentoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //DocumentoRolIdentidad
            dataWrapperDocumentacion.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(docRolIden => docRolIden.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoRolGrupoIdentidades
            dataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();

            return dataWrapperDocumentacion;
        }

        public Guid ObtenerPerfilPublicadorDocumento(Guid pDocumentoID, Guid pProyectoID)
        {
            Guid perfilPublicadorID = Guid.Empty;

            EntityModel.Models.IdentidadDS.Identidad identidad = mEntityContext.Identidad.JoinIdentidadDocumentoWebVinBaseRecursos().Where(objeto => objeto.Identidad.ProyectoID.Equals(pProyectoID) && objeto.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID)).Select(objeto => objeto.Identidad).FirstOrDefault();
            if (identidad != null)
            {
                perfilPublicadorID = identidad.PerfilID;
            }

            return perfilPublicadorID;
        }

        /// <summary>
        /// Obtiene los votos de una encuesta
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerVotosEncuestaDocumento(Guid pDocumentoID)
        {
            DataWrapperDocumentacion documentacionDS = new DataWrapperDocumentacion();

            documentacionDS.ListaDocumentoRespuestaVoto = mEntityContext.DocumentoRespuestaVoto.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            return documentacionDS;
        }

        /// <summary>
        /// Obtiene los editores de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documentos</param>
        /// <returns>Tabla DocumentoRolIdentidad cargada con los editores de los documentos</return s>
        public DataWrapperDocumentacion ObtenerEditoresDocumentos(List<Guid> pListaDocumentosID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            if (pListaDocumentosID.Count > 0)
            {
                dataWrapperDocumentacion.ListaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(doc => pListaDocumentosID.Contains(doc.DocumentoID)).ToList();

                dataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(doc => pListaDocumentosID.Contains(doc.DocumentoID)).ToList();
            }

            return dataWrapperDocumentacion;
        }

        #endregion

        #region VersionDocumento

        /// <summary>
        /// Obtiene la última versión de un documento en base de datos
        /// </summary>
        /// <param name="pDocumentoID">Documento original</param>
        /// <returns></returns>
        public Documento ObtenerUltimaVersionDocumento(Guid pDocumentoID)
        {
           return mEntityContext.Documento.Join(mEntityContext.VersionDocumento, doc => doc.DocumentoID, version => version.DocumentoID, (doc, version) => new
            {
                Documento = doc,
                VersionDocumento = version
            }).Where(item => item.Documento.UltimaVersion && item.VersionDocumento.DocumentoOriginalID.Equals(pDocumentoID)).Select(item => item.Documento).FirstOrDefault();      
        }
        /// <summary>
        /// Devuelve la version mas alta de una mejora activa dado un documento id original
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <param name="pMejoraID"></param>
        /// <returns></returns>
        public Documento ObtenerUltimaVersionDocumentoMejora(Guid pDocumentoID, Guid pMejoraID)
        {
            return mEntityContext.VersionDocumento.JoinDocumento().Where(item => item.VersionDocumento.DocumentoOriginalID.Equals(pDocumentoID) && item.VersionDocumento.EsMejora && item.VersionDocumento.MejoraID.Equals(pMejoraID) && item.VersionDocumento.EstadoVersion == (short)EstadoVersion.Pendiente).Select(item => item.Documento).FirstOrDefault();
        }

        public bool ComprobarDocumentoTieneMejoraPendiente(Guid pDocumentoID, Guid pMejoraID)
        {
            return mEntityContext.VersionDocumento.JoinDocumento().Any(item => item.VersionDocumento.DocumentoOriginalID.Equals(pDocumentoID) && item.VersionDocumento.EsMejora && item.VersionDocumento.MejoraID.Equals(pMejoraID) && item.VersionDocumento.EstadoVersion == (short)EstadoVersion.Pendiente);
        }

        public bool ComprobarDocumentoTieneMejoraActiva(Guid pDocumentoID)
        {
            return mEntityContext.VersionDocumento.JoinDocumento().Any(item => item.VersionDocumento.DocumentoOriginalID.Equals(pDocumentoID) && item.VersionDocumento.EsMejora &&  item.VersionDocumento.EstadoVersion == (short)EstadoVersion.Pendiente);
        }

        public bool ComprobarDocumentoTieneVersiones(Guid pDocumentoID)
        {
            return mEntityContext.VersionDocumento.Any(item => item.DocumentoOriginalID.Equals(pDocumentoID)) || mEntityContext.VersionDocumento.Any(item => item.DocumentoID.Equals(pDocumentoID));
        }

        /// <summary>
        /// Obtiene la fecha de edición de un recurso, si está bloqueado
        /// </summary>
        /// <param name="pDocumentoID">Identificador del recurso</param>
        /// <returns></return s>
        public DateTime? ObtenerFechaRecursoEnEdicion(Guid pDocumentoID)
        {
            //Compruebo si alguien ya esta editando el recurso

            var resultado = mEntityContext.DocumentoEnEdicion.Where(item => item.DocumentoID.Equals(pDocumentoID)).Select(item => item.FechaEdicion);

            DateTime? fecha = null;
            if (resultado.Any())
            {
                fecha = resultado.First();
            }

            return fecha;
        }

        /// <summary>
        /// Le añade al tiempo de bloqueo de un recurso otros 60 segundos
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a bloquear durante otros 60 segundos</param>
        public DateTime? ActualizarFechaRecursoEnEdicion(Guid pDocumentoID, DateTime pFechaEdicion)
        {
            // Sólo una petición podrá bloquear el recurso con cada token

            DocumentoEnEdicion documentoEnEdicion = mEntityContext.DocumentoEnEdicion.Where(item => item.DocumentoID.Equals(pDocumentoID) && item.FechaEdicion.Equals(pFechaEdicion)).FirstOrDefault();
            if (documentoEnEdicion != null)
            {
                documentoEnEdicion.FechaEdicion = DateTime.UtcNow.AddSeconds(60);
                int numFilasEditadas = mEntityContext.SaveChanges();

                if (numFilasEditadas > 0)
                {
                    return ObtenerFechaRecursoEnEdicion(pDocumentoID);
                }
            }
            return null;
        }

        /// <summary>
        /// Comprueba si un documento está siendo actualizado por algún usuario en este instante. Si no es así, lo marca en edición
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a comprobar</param>
        /// <param name="pIdentidadID">Identificador de la identidad del editor</param>
        /// <returns>La fila del documento en edición, null si nadie lo está editando</return s>
        public DocumentoEnEdicion ComprobarDocumentoEnEdicion(Guid pDocumentoID, Guid pIdentidadID, int pSegundosDuracionBloqueo = 60)
        {
            return ComprobarDocumentoEnEdicion(pDocumentoID, pIdentidadID, 3, pSegundosDuracionBloqueo);
        }

        /// <summary>
        /// Finaliza la edición de un recurso, eliminando la fila de DocumentoEnEdicion
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void FinalizarEdicionRecurso(Guid pDocumentoID)
        {
            DocumentoEnEdicion documentoEnEdicion = mEntityContext.DocumentoEnEdicion.Where(item => item.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();

            try
            {
                mEntityContext.EliminarElemento(documentoEnEdicion);

                ActualizarBaseDeDatosEntityContext();
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
            }
        }

        /// <summary>
        /// Comprueba si un documento está siendo actualizado por algún usuario en este instante. Si no es así, lo marca en edición
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a comprobar</param>
        /// <param name="pIdentidadID">Identificador de la identidad del editor</param>
        /// <returns>La fila del documento en edición, null si nadie lo está editando</return s>
        public DocumentoEnEdicion ComprobarDocumentoEnEdicion(Guid pDocumentoID, Guid pIdentidadID, int pNumeroIntentos, int pSegundosDuracionBloqueo)
        {
            //Compruebo si alguien ya esta editando el recurso
            DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();

            docDW.ListaDocumentoEnEdicion = mEntityContext.DocumentoEnEdicion.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            if (docDW.ListaDocumentoEnEdicion.Count > 0)
            {
                DocumentoEnEdicion filaEnEdicion = docDW.ListaDocumentoEnEdicion.FirstOrDefault();

                if (DateTime.UtcNow > filaEnEdicion.FechaEdicion)
                {
                    //Si hace más de 1 minuto que el recurso estaba en edición y aún sigue marcado como en edición, quiere decir que algo fué mal, elimino la fila de edición. (FechaEdicion se crea con 60 segundos añadidos)
                    docDW.ListaDocumentoEnEdicion.Remove(filaEnEdicion);
                    mEntityContext.EliminarElemento(filaEnEdicion);

                    try
                    {
                        ActualizarBaseDeDatosEntityContext();
                    }
                    catch (Exception e)
                    {
                        mLoggingService.GuardarLogError(e, mlogger);
                    }

                    if (pNumeroIntentos > 3)
                    {
                        // Sólo intento eliminar la fila 3 veces, si tras tres intentos no se ha podido, es que hay algún problema en la base de datos
                        pNumeroIntentos = 3;
                    }
                    if (pNumeroIntentos > 0)
                    {
                        return ComprobarDocumentoEnEdicion(pDocumentoID, pIdentidadID, pNumeroIntentos - 1, pSegundosDuracionBloqueo);
                    }
                    else
                    {
                        throw new Exception($"Imposible eliminar la fila de DocumentoEnEdicion para el recurso {pDocumentoID} para la identidad {pIdentidadID} tras 3 intentos");
                    }
                }
                else
                {
                    if (pNumeroIntentos > 0)
                    {
                        // El recurso está bloqueado por otro usuario
                        // duermo un segundo e intento bloquearlo de nuevo
                        Thread.Sleep(1000);
                        return ComprobarDocumentoEnEdicion(pDocumentoID, pIdentidadID, pNumeroIntentos - 1, pSegundosDuracionBloqueo);
                    }
                    else
                    {
                        return filaEnEdicion;
                    }
                }
            }
            else
            {
                //No hay nadie editándolo, lo marco en edición
                DocumentoEnEdicion documentoEnEdicion = new DocumentoEnEdicion();

                documentoEnEdicion.DocumentoID = pDocumentoID;
                documentoEnEdicion.IdentidadID = pIdentidadID;
                documentoEnEdicion.FechaEdicion = DateTime.UtcNow.AddSeconds(pSegundosDuracionBloqueo);

                docDW.ListaDocumentoEnEdicion.Add(documentoEnEdicion);
                mEntityContext.DocumentoEnEdicion.Add(documentoEnEdicion);
                try
                {
                    ActualizarBaseDeDatosEntityContext();
                    return null;
                }
                catch (Exception ex)
                {
                    mEntityContext.EliminarElemento(documentoEnEdicion);
                    mLoggingService.GuardarLogError(ex, $"Error intentando bloquear el recurso {pDocumentoID} para la identidad {pIdentidadID} quedan {pNumeroIntentos} intentos", mlogger);
                    if (mEntityContext.Documento.Any(item => item.DocumentoID.Equals(pDocumentoID)))
                    {
                        if (pNumeroIntentos > 0)
                        {
                            // Si ha habido una excepción, seguramente es porque alguien ha guardado el recurso a la vez que yo
                            // duermo un segundo antes de volver a intentarlo
                            Thread.Sleep(1000);
                            return ComprobarDocumentoEnEdicion(pDocumentoID, pIdentidadID, pNumeroIntentos - 1, pSegundosDuracionBloqueo);
                        }
                        else
                        {
                            throw new Exception($"No ha sido posible bloquear la edición del recurso {pDocumentoID} para la identidad {pIdentidadID} tras 3 intentos");
                        }
                    }
                    else
                    {
                        throw new Exception($"El documentoID {pDocumentoID} no existe en la base de datos");
                    }
                }
            }
        }

		public bool ComprobarSiDocumentoEsUnaMejora(Guid pDocumentoID)
        {
            VersionDocumento versionMejora = mEntityContext.VersionDocumento.Where(x => x.DocumentoID.Equals(pDocumentoID) && x.EsMejora && x.EstadoVersion == (short)EstadoVersion.Pendiente).FirstOrDefault();

            if (versionMejora == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

		/// <summary>
		/// Carga la tabla VersionDocumento para los documentos pasados en la lista.
		/// </summary>
		/// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
		/// <param name="pListaDocumentosID">Lista de identificadores de los documentos para traer la información</param>
		/// <param name="pRelaciones">Especifica si deben traerse todas la versiones de los documentos originales o no</param>
		public void ObtenerVersionDocumentosPorIDs(DataWrapperDocumentacion pDataWrapperDocumentacion, List<Guid> pListaDocumentosID, bool pRelaciones)
        {
            if (pListaDocumentosID.Count > 0)
            {
                pDataWrapperDocumentacion.ListaVersionDocumento = pDataWrapperDocumentacion.ListaVersionDocumento.Union(mEntityContext.VersionDocumento.Where(version => pListaDocumentosID.Contains(version.DocumentoID) || pListaDocumentosID.Contains(version.DocumentoOriginalID))).ToList();

            }

            if (pRelaciones)
            {
                List<Guid> listaDocumentosOriginalesID = new List<Guid>();

                foreach (VersionDocumento filaVersionDoc in pDataWrapperDocumentacion.ListaVersionDocumento)
                {
                    if (!listaDocumentosOriginalesID.Contains(filaVersionDoc.DocumentoOriginalID))
                    {
                        listaDocumentosOriginalesID.Add(filaVersionDoc.DocumentoOriginalID);
                    }
                }

                if (listaDocumentosOriginalesID.Count > 0)
                {
                    pDataWrapperDocumentacion.ListaVersionDocumento = pDataWrapperDocumentacion.ListaVersionDocumento.Union(mEntityContext.VersionDocumento.Where(version => listaDocumentosOriginalesID.Contains(version.DocumentoOriginalID))).ToList();
                }
            }
        }

        /// <summary>
        /// Carga todas las versiones de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento del que se quieren obtener las versiones</param>
        /// <returns>Identificadores de los documentos</return s>
        public Dictionary<Guid, int> ObtenerVersionesDocumentoIDPorID(Guid pDocumentoID)
        {
            Dictionary<Guid, int> listaDocumentosID = new Dictionary<Guid, int>();
            bool versionOriginalEliminada = mEntityContext.Documento.FirstOrDefault(doc => doc.DocumentoID.Equals(pDocumentoID)).Eliminado;
            var resultado = mEntityContext.VersionDocumento.Where(doc => doc.DocumentoOriginalID.Equals(pDocumentoID)).OrderBy(doc => doc.Version).ToList();
            VersionDocumento primeraVersion = resultado.FirstOrDefault(verDoc => verDoc.DocumentoID == verDoc.DocumentoOriginalID);
            if (!versionOriginalEliminada && primeraVersion == null)
            {
                listaDocumentosID.Add(pDocumentoID, 0);
            }

            foreach (var fila in resultado)
            {
                listaDocumentosID.Add(fila.DocumentoID, fila.Version);
            }
            return listaDocumentosID;
        }

        public Guid ObtenerUltimaVersionDeDocumento(Guid pDocumentoID)
        {
            return mEntityContext.VersionDocumento.Join(mEntityContext.Documento, vd => vd.DocumentoID, d => d.DocumentoID, (vd, d) => new { vd, d }).Where(x => x.vd.DocumentoOriginalID == pDocumentoID && x.d.UltimaVersion).Select(x => x.d.DocumentoID).FirstOrDefault();
		}

        /// <summary>
        /// Dado un documentoID que tiene que ser una version vigente o pendiente
        /// devuelve los ids de las versiones anteriores.
        /// Si se usa un documentoID de una version historica no funcionara.
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerIdsVersionesAnteriores(Guid pDocumentoID)
        {
            List<Guid> listaVersionesID;
            VersionDocumento versionActual = mEntityContext.VersionDocumento.FirstOrDefault(x => x.DocumentoID.Equals(pDocumentoID));
            bool esVigente = versionActual.EstadoVersion == (short)EstadoVersion.Vigente;
            if (esVigente)
            {
                listaVersionesID = mEntityContext.VersionDocumento.Where(x => x.DocumentoOriginalID.Equals(versionActual.DocumentoOriginalID) && x.Version <= versionActual.Version && x.MejoraID == null).Select(x => x.DocumentoID).ToList();
                if (!listaVersionesID.Contains(versionActual.DocumentoOriginalID)) listaVersionesID.Add(versionActual.DocumentoOriginalID);
            }
            else
            {
                int versionVigente = mEntityContext.VersionDocumento.FirstOrDefault(x => x.DocumentoOriginalID.Equals(versionActual.DocumentoOriginalID) && x.EstadoVersion == (short)EstadoVersion.Vigente).Version;
                listaVersionesID = mEntityContext.VersionDocumento.Where(x => x.DocumentoOriginalID.Equals(versionActual.DocumentoOriginalID) && x.Version > versionVigente && x.Version <= versionActual.Version).Select(x => x.DocumentoID).ToList();
            }

            return listaVersionesID;
        }

        public Guid ObtenerDocumentoOriginalIDPorID(Guid pDocumentoID)
        {
            // Comprobar si el documento pasado es una version de otro sino es un documento original
            var version = mEntityContext.VersionDocumento.FirstOrDefault(doc => doc.DocumentoID.Equals(pDocumentoID));
            if(version != null)
            {
                return version.DocumentoOriginalID;
            }

            return pDocumentoID;
        }

        /// <summary>
        /// Carga todas las versiones de una página de forma ordenada.
        /// </summary>
        /// <param name="pPestanyaID">Identificador de la página de la que se quieren obtener las versiones</param>
        /// <returns>Listado ordenano por versiones de las páginas </returns>
        public List<ProyectoPestanyaMenuVersionPagina> ObtenerListaVersionesPaginaPorID(Guid pPestanyaID)
        {
            List<ProyectoPestanyaMenuVersionPagina> proyectoPestanyaMenuVersionPaginas = mEntityContext.ProyectoPestanyaMenuVersionPaginas
                .Where(pagina => pagina.PestanyaID.Equals(pPestanyaID))
                .OrderBy(pagina => pagina.Fecha)
                .ToList();

            List<ProyectoPestanyaMenuVersionPagina> proyectoPestanyaMenuVersionPaginasCorrectas = new List<ProyectoPestanyaMenuVersionPagina>();

            int index = 0;
            foreach (ProyectoPestanyaMenuVersionPagina p in proyectoPestanyaMenuVersionPaginas)
            {
                if (p.VersionAnterior == null)
                {
                    proyectoPestanyaMenuVersionPaginasCorrectas.Insert(0, p);
                }
                else
                {
                    proyectoPestanyaMenuVersionPaginasCorrectas.Insert(index, p);
                }
                index++;
            }

            return proyectoPestanyaMenuVersionPaginasCorrectas;
        }


        /// <summary>
        /// Carga todas las versiones de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento del que se quieren obtener las versiones</param>
        /// <returns>DataSet de documentación</return s>
        public DataWrapperDocumentacion ObtenerVersionesDocumentoPorID(Guid pDocumentoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            Guid documentoOriginal = mEntityContext.VersionDocumento.Where(version => version.DocumentoID.Equals(pDocumentoID)).Select(version => version.DocumentoOriginalID).FirstOrDefault();

            if (documentoOriginal.Equals(Guid.Empty))
            {
                documentoOriginal = pDocumentoID;
            }

            var consulta = mEntityContext.Documento.Join(mEntityContext.VersionDocumento, doc => doc.DocumentoID, version => version.DocumentoID, (doc, version) => new
            {
                Documento = doc,
                VersionDocumento = version
            }).Where(objeto => objeto.VersionDocumento.DocumentoOriginalID.Equals(documentoOriginal)).Select(objeto => objeto.Documento).Concat(mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(documentoOriginal))).ToList().Distinct().ToList();

            dataWrapperDocumentacion.ListaDocumento = consulta.ToList();

            dataWrapperDocumentacion.ListaVersionDocumento = mEntityContext.VersionDocumento.Where(version => version.DocumentoOriginalID.Equals(documentoOriginal)).ToList();

            dataWrapperDocumentacion.ListaDocumentoComentario = mEntityContext.DocumentoComentario.JoinVersionDocumento().Where(item => item.VersionDocumento.DocumentoOriginalID.Equals(documentoOriginal)).Select(item => item.DocumentoComentario).Union(mEntityContext.DocumentoComentario.Where(item => item.DocumentoID.Equals(documentoOriginal))).ToList();

            return dataWrapperDocumentacion;
        }

        #endregion

        #region Votos y Comentarios

        /// <summary>
        /// Obtiene los comentarios y votos de los documentos.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDW">DataSet de documentación</param>
        public void ObtenerComenariosYVotosDocumento(Guid pDocumentoID, DataWrapperDocumentacion pDocumentacionDW)
        {
            pDocumentacionDW.ListaDocumentoComentario = mEntityContext.DocumentoComentario.JoinComentario().Where(item => item.Comentario.Eliminado.Equals(false) && item.DocumentoComentario.DocumentoID.Equals(pDocumentoID)).Select(item => item.DocumentoComentario).ToList();

            pDocumentacionDW.ListaVotoDocumento = mEntityContext.VotoDocumento.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDW">DataWrapper de documentación</param>
        public void ObtenerComenariosDocumento(Guid pDocumentoID, DataWrapperDocumentacion pDocumentacionDW)
        {
            pDocumentacionDW.ListaDocumentoComentario = mEntityContext.DocumentoComentario.JoinComentario().Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocumentoID)).Select(item => item.DocumentoComentario).ToList();
        }

        /// <summary>
        /// Obtiene los votos de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDW">DataSet de documentación</param>
        public void ObtenerVotosDocumento(Guid pDocumentoID, DataWrapperDocumentacion pDocumentacionDW)
        {
            pDocumentacionDW.ListaVotoDocumento = mEntityContext.VotoDocumento.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();
        }

        /// <summary>
        /// Obtiene los votos de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public List<string> ObtenerComentadores(Guid pDocumentoID, Guid pProyectoID)
        {
            List<string> listaComentadores = new List<string>();


            listaComentadores = mEntityContext.DocumentoComentario.JoinComentario().JoinIdentidad().JoinPerfil().Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocumentoID) && item.DocumentoComentario.ProyectoID.HasValue && item.DocumentoComentario.ProyectoID.Value.Equals(pProyectoID)).Select(item => item.Perfil.NombrePerfil).ToList();

            return listaComentadores;
        }

        /// <summary>
        /// Obtiene los votos de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDS">DataSet de documentación</param>
        public string ObtenerNumeroVisitas(Guid pDocumentoID, Guid pBaseRecursoID)
        {
            int numeroVisitas = 0;

            numeroVisitas = mEntityContext.DocumentoWebVinBaseRecursosExtra.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).Select(doc => doc.NumeroConsultas).FirstOrDefault();
            return numeroVisitas.ToString();
        }

        /// <summary>
        /// Obtiene los perfiles de los autores de los comentarios de un documento en un proyecto.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pProyectoID">Identificador del proyecto donde se han hecho los comentarios</param>
        /// <param name="pTraerEliminados">Indica si se deben traer los perfiles autores de los comentarios eliminados de documentos</param>
        /// <returns>Lista de PerfilesID de los autores de los comentarios de un documento en un proyecto</return s>
        public List<Guid> ObtenerPerfilesAutoresComenariosDocumento(Guid pDocumentoID, Guid pProyectoID, bool pTraerEliminados)
        {
            var query = mEntityContext.Comentario.JoinIdentidad().Join(mEntityContext.DocumentoComentario, item => new { ComentarioID = item.Comentario.ComentarioID, ProyectoID = pProyectoID }, documentoComentario => new { ComentarioID = documentoComentario.ComentarioID, ProyectoID = documentoComentario.ProyectoID.Value }, (item, documentoComentario) => new
            {
                DocumentoComentario = documentoComentario,
                Comentario = item.Comentario,
                Identidad = item.Identidad
            }).Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocumentoID));

            if (pTraerEliminados)
            {
                return query.Select(item => item.Identidad.PerfilID).Distinct().ToList();
            }
            else
            {
                return query.Where(item => !item.Comentario.Eliminado).Select(item => item.Identidad.PerfilID).Distinct().ToList();
            }
        }

        /// <summary>
        /// Actualiza la valoración del documento según su número de votos.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>DataSet de documentación con la información actulizada</return s>
        public DataWrapperDocumentacion ActualizarValoracionDocumento(Guid pDocumentoID)
        {
            DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();

            Documento documento = mEntityContext.Documento.Where(item => item.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();
            documento.Valoracion = mEntityContext.Voto.JoinVotoDocumento().Where(item => item.VotoDocumento.DocumentoID.Equals(pDocumentoID)).Union(mEntityContext.Voto.JoinVotoDocumento().Where(item => item.VotoDocumento.DocumentoID.Equals(pDocumentoID))).Sum(item => item.Voto.Voto1);

            ActualizarBaseDeDatosEntityContext();

            docDW.ListaDocumento = mEntityContext.Documento.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();
            return docDW;
        }

        /// <summary>
        /// Actualiza la fila de la cola de cargas de un CSV a un estado.
        /// </summary>
        /// <param name="pClaveCSV">Clave del archivo CSV</param>
        /// <param name="pEstado">Estado</param>
        /// <returns>TRUE si todo ha ido bien</return s>
        public void ActualizarRecursoColaCargaRecursosAEstado(Guid pClaveCSV, short pEstado)
        {
            ColaCargaRecursos colaCargarRecursos = mEntityContext.ColaCargaRecursos.Where(item => item.ID.Equals(pClaveCSV)).FirstOrDefault();
            colaCargarRecursos.Estado = pEstado;

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene los comentarios de comunidades públicas y la comunidad actual de los documentos cargados en la tabla Documento.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoActual">Proyecto actual en el que está el usuario</param>
        public void ObtenerComentariosPublicosMasProyectoActualDeDocumentos(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoActual)
        {
            pDataWrapperDocumentacion.ListaDocumentoComentario.Clear();

            if (pDataWrapperDocumentacion.ListaDocumento.Count > 0)
            {
                List<Guid> listaID = pDataWrapperDocumentacion.ListaDocumento.Select(item2 => item2.DocumentoID).ToList();

                pDataWrapperDocumentacion.ListaDocumentoComentario = mEntityContext.DocumentoComentario.JoinComentario().JoinProyecto().Where(item => !item.Comentario.Eliminado && item.Proyecto.ProyectoID.Equals(pProyectoActual) || item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Publico) || item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Restringido) && (listaID.Contains(item.DocumentoComentario.DocumentoID))).Select(item => item.DocumentoComentario).ToList();
            }
        }

        /// <summary>
        /// Obtiene el identificador del documento al que pertenece un voto.
        /// </summary>
        /// <param name="pVotoID">Identificador del voto</param>
        /// <returns>Identificador del documento al que pertenece el voto</return s>
        public Guid ObtenerIDDocumentoDeVotoPorID(Guid pVotoID)
        {
            return mEntityContext.VotoDocumento.Where(item => item.VotoID.Equals(pVotoID)).Select(item => item.DocumentoID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el título del documento de un documento.
        /// </summary>
        /// <param name="pVotoID">Identificador del documento</param>
        /// <returns>Título del documento</return s>
        public string ObtenerTituloDocumentoPorID(Guid pDocumentoID)
        {
            string nombre = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).Select(doc => doc.Titulo).FirstOrDefault();

            if (nombre == null)
            {
                nombre = "";
            }
            return nombre;
        }

        /// <summary>
        /// Obtiene el identificador del documento al que pertenece el comentario.
        /// </summary>
        /// <param name="pComentarioID">Identificador del comentario</param>
        /// <returns>Identificador del documento al que pertenece el comentario</return s>
        public Guid ObtenerIDDocumentoDeComentarioPorID(Guid pComentarioID)
        {
            return mEntityContext.DocumentoComentario.Where(item => item.ComentarioID.Equals(pComentarioID)).Select(item => item.DocumentoID).FirstOrDefault();
        }

        #endregion

        #region Categorías del tesauro

        /// <summary>
        /// Devuelve el dataSet de documentación con las categorías del tesauro del los documentos requeridas.
        /// </summary>
        /// <param name="pListaDocumentosID">Lista con los identificadores de los documentos</param>
        /// <returns>DataSet de documentación con la tabla DocumentoWebAgCatTesauro</return s>
        public List<DocumentoWebAgCatTesauro> ObtenerCategoriasTesauroListaDocumentosProyectoID(List<Guid> pListaDocumentosID, Guid pProyectoID)
        {
            List<DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = new List<DocumentoWebAgCatTesauro>();
            if (pListaDocumentosID.Count > 0)
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroBaseRecursosProyecto().Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && pListaDocumentosID.Contains(objeto.DocumentoWebAgCatTesauro.DocumentoID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }

            return listaDocumentoWebAgCatTesauro;
        }

        /// <summary>
        /// Devuelve el dataSet de documentación con las categorías del tesauro del los documentos requeridas.
        /// </summary>
        /// <param name="pListaDocumentosID">Lista con los identificadores de los documentos</param>
        /// <returns>DataSet de documentación con la tabla DocumentoWebAgCatTesauro</return s>
        public List<DocumentoWebAgCatTesauroConVinculoTesauroID> ObtenerCategoriasTesauroYTesauroDeDocumentos(List<Guid> pListaDocumentosID)
        {
            List<DocumentoWebAgCatTesauroConVinculoTesauroID> listaDocumentoWebAgCatTesauroConVinculoTesauroID = new List<DocumentoWebAgCatTesauroConVinculoTesauroID>();

            if (pListaDocumentosID.Count > 0)
            {
                var consulta1 = mEntityContext.DocumentoWebAgCatTesauro.Join(mEntityContext.TesauroProyecto, docWebAg => docWebAg.TesauroID, tesProy => tesProy.TesauroID, (docWebAg, tesProy) => new
                {
                    DocumentoWebAgCatTesauro = docWebAg,
                    TesauroProyecto = tesProy
                }).Where(objeto => pListaDocumentosID.Contains(objeto.DocumentoWebAgCatTesauro.DocumentoID)).ToList().Select(objeto => new DocumentoWebAgCatTesauroConVinculoTesauroID
                {
                    BaseRecursosID = objeto.DocumentoWebAgCatTesauro.BaseRecursosID,
                    CategoriaTesauroID = objeto.DocumentoWebAgCatTesauro.CategoriaTesauroID,
                    DocumentoID = objeto.DocumentoWebAgCatTesauro.DocumentoID,
                    Fecha = objeto.DocumentoWebAgCatTesauro.Fecha,
                    TesauroID = objeto.DocumentoWebAgCatTesauro.TesauroID,
                    VinculoTesauroID = objeto.TesauroProyecto.ProyectoID,
                    TipoTesauro = 0
                });

                var consulta2 = mEntityContext.DocumentoWebAgCatTesauro.Join(mEntityContext.TesauroUsuario, docWebAg => docWebAg.TesauroID, tesUs => tesUs.TesauroID, (docWebAg, tesUs) => new
                {
                    DocumentoWebAgCatTesauro = docWebAg,
                    TesauroUsuario = tesUs
                }).Join(mEntityContext.DocumentoWebVinBaseRecursos, objeto => new { objeto.DocumentoWebAgCatTesauro.DocumentoID, objeto.DocumentoWebAgCatTesauro.BaseRecursosID }, doc => new { doc.DocumentoID, doc.BaseRecursosID }, (objeto, doc) => new
                {
                    DocumentoWebAgCatTesauro = objeto.DocumentoWebAgCatTesauro,
                    TesauroUsuario = objeto.TesauroUsuario,
                    DocumentoWebVinBaseRecursos = doc
                }).Join(mEntityContext.Identidad, objeto => new { IdentidadID = objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value }, ident1 => new
                { IdentidadID = ident1.IdentidadID }, (objeto, ident1) => new
                {
                    DocumentoWebAgCatTesauro = objeto.DocumentoWebAgCatTesauro,
                    TesauroUsuario = objeto.TesauroUsuario,
                    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                    Ident1 = ident1
                }).Join(mEntityContext.Perfil, objeto => objeto.Ident1.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
                {
                    DocumentoWebAgCatTesauro = objeto.DocumentoWebAgCatTesauro,
                    TesauroUsuario = objeto.TesauroUsuario,
                    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                    Ident1 = objeto.Ident1,
                    Perfil = perfil
                }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.HasValue && pListaDocumentosID.Contains(objeto.DocumentoWebAgCatTesauro.DocumentoID)).ToList().Select(objeto => new DocumentoWebAgCatTesauroConVinculoTesauroID
                {
                    BaseRecursosID = objeto.DocumentoWebAgCatTesauro.BaseRecursosID,
                    CategoriaTesauroID = objeto.DocumentoWebAgCatTesauro.CategoriaTesauroID,
                    DocumentoID = objeto.DocumentoWebAgCatTesauro.DocumentoID,
                    Fecha = objeto.DocumentoWebAgCatTesauro.Fecha,
                    TesauroID = objeto.DocumentoWebAgCatTesauro.TesauroID,
                    VinculoTesauroID = objeto.Perfil.PerfilID,
                    TipoTesauro = 1
                });

                listaDocumentoWebAgCatTesauroConVinculoTesauroID = consulta1.Union(consulta2).ToList();

            }

            return listaDocumentoWebAgCatTesauroConVinculoTesauroID;
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro.
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</return s>
        public DataWrapperDocumentacion ObtenerDocAgCatTesauroDeTesauroID(Guid pTesauroID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            if (pTesauroID != Guid.Empty)
            {
                dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinDocumentoWebAgCatTesauro().Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.TesauroID.Equals(pTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }
            else
            {
                dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinDocumentoWebAgCatTesauro().Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre recursos publicos y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</return s>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatRecursosPublicosTesauroDeTesauroID(Guid pTesauroID)
        {
            List<DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = new List<DocumentoWebAgCatTesauro>();

            if (pTesauroID != Guid.Empty)
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.DafoProyecto) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.TesauroID.Equals(pTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }
            else
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.DafoProyecto) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }

            return listaDocumentoWebAgCatTesauro;
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre preguntas publicos y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</return s>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatPreguntasPublicasTesauroDeTesauroID(Guid pTesauroID)
        {
            List<DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = new List<DocumentoWebAgCatTesauro>();

            if (pTesauroID != Guid.Empty)
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.TesauroID.Equals(pTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }
            else
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }

            return listaDocumentoWebAgCatTesauro;
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre debates publicos y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</return s>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatDebatesPublicosTesauroDeTesauroID(Guid pTesauroID)
        {
            List<DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = new List<DocumentoWebAgCatTesauro>();

            if (pTesauroID != Guid.Empty)
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.TesauroID.Equals(pTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }
            else
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }

            return listaDocumentoWebAgCatTesauro;
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre recursos privados y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</return s>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatRecursosPrivadosTesauroDeTesauroID(Guid pTesauroID)
        {
            List<DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = new List<DocumentoWebAgCatTesauro>();

            if (pTesauroID != Guid.Empty)
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.DafoProyecto) && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.TesauroID.Equals(pTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }
            else
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.DafoProyecto) && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }

            return listaDocumentoWebAgCatTesauro;
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre preguntas privados y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</return s>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatPreguntasPrivadasTesauroDeTesauroID(Guid pTesauroID)
        {
            List<DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = new List<DocumentoWebAgCatTesauro>();

            if (pTesauroID != Guid.Empty)
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.TesauroID.Equals(pTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }
            else
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }

            return listaDocumentoWebAgCatTesauro;
        }

        /// <summary>
        /// Devuelve una lista de GUID con las vinculaciones entre documentos y una categoría concreta
        /// </summary>
        /// <param name="pCategoriaTesauroID">Identificador de la categoría</param>
        /// <returns>Lista con los documentos
        public List<Guid> ObtenerListaDocsAgCatDeCategoriaTesauroID(Guid pCategoriaTesauroID)
        {
            List<Guid> listaDocs = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.CategoriaTesauroID.Equals(pCategoriaTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro.DocumentoID).ToList();

            return listaDocs;
        }

        /// <summary>
        /// Devuelve una lista de GUID con las vinculaciones entre documentos y las categorías de un tesauro
        /// </summary>
        /// <param name="pCategoriaTesauroID">Identificador de la categoría</param>
        /// <returns>Lista con los documentos
        public List<Guid> ObtenerListaDocsAgCatDeTesauroID(Guid pTesauroID)
        {
            List<Guid> listaDocs = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && !objeto.Documento.Borrador && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.TesauroID.Equals(pTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro.DocumentoID).ToList();

            return listaDocs;
        }

        /// <summary>
        /// Descomparte de una comunidad todos los recursos compartidos con la comunidad de destino salvo los excluidos
        /// </summary>
        /// <param name="pListaDocumentosExcluidos">Documentos exluidos de la comprobación</param>
        /// <param name="pProyectoDestinoID">Identificador del proyecto de compartición de destino</param>
        /// <param name="pProyectoOrigenID">Identificador del proyecto de origen de los documentos</param>
        public void DescompartirDocumentosCompartidos(List<Guid> pListaDocumentosExcluidos, Guid pProyectoOrigenID, Guid pProyectoDestinoID)
        {
            var consulta = mEntityContext.DocumentoWebVinBaseRecursos.Where(documentoWebVinBaseRecursos => (mEntityContext.Documento.Join(
              mEntityContext.BaseRecursosProyecto, doc => documentoWebVinBaseRecursos.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (item, baseRecursoProy) => new
              {
                  Documento = item,
                  BaseRecursosProyecto = baseRecursoProy
              }
                 ).Where(item => item.Documento.DocumentoID.Equals(documentoWebVinBaseRecursos.DocumentoID) && item.Documento.Eliminado == false && item.Documento.Borrador == false && item.Documento.ProyectoID.Equals(pProyectoOrigenID) && !pListaDocumentosExcluidos.Contains(documentoWebVinBaseRecursos.DocumentoID) && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoDestinoID))).Any() && documentoWebVinBaseRecursos.Eliminado == false);

            List<DocumentoWebVinBaseRecursos> listaActualizar = consulta.ToList();

            foreach (DocumentoWebVinBaseRecursos fila in listaActualizar)
            {
                fila.Eliminado = true;
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre debates privados y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</return s>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatDebatesPrivadosTesauroDeTesauroID(Guid pTesauroID)
        {
            List<DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = new List<DocumentoWebAgCatTesauro>();
            if (pTesauroID != Guid.Empty)
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Borrador && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.DocumentoWebAgCatTesauro.TesauroID.Equals(pTesauroID)).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }
            else
            {
                listaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos().JoinDocumento().Where(objeto => !objeto.Documento.Eliminado && objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Borrador && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado).Select(objeto => objeto.DocumentoWebAgCatTesauro).ToList();
            }

            return listaDocumentoWebAgCatTesauro;
        }

        /// <summary>
        /// Obtiene un dictionary cuya clave es un documento y el valor es una lista de Guid de las identidades de los editores
        /// </summary>
        /// <param name="pDocsID">Lista de identificadores de los documentos</param>
        /// <returns>dictionary cuya clave es un documento y el valor es una lista de Guid de los editores</return s>
        public Dictionary<Guid, List<Guid>> ObtenerListaEditoresDeDocumentosPrivadosEnProyecto(List<Guid> pDocsID, Guid pProyectoID)
        {
            Dictionary<Guid, List<Guid>> listaEditoresRecursos = new Dictionary<Guid, List<Guid>>();

            if (pDocsID.Count > 0)
            {
                var listaDocumentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.Where(docRolIden => pDocsID.Contains(docRolIden.DocumentoID)).Select(docRolIden => new { docRolIden.DocumentoID, docRolIden.PerfilID }).ToList();

                var listaDocumentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.Where(docRolGrupIden => pDocsID.Contains(docRolGrupIden.DocumentoID)).Select(docRolGrupIden => new { docRolGrupIden.DocumentoID, docRolGrupIden.GrupoID, docRolGrupIden.Editor }).ToList();

                //Creamos una lista de perfiles de los que obtendremos las identidades
                List<Guid> listaPerfiles = new List<Guid>();
                foreach (var filaDocRol in listaDocumentoRolIdentidad)
                {
                    if (!listaPerfiles.Contains(filaDocRol.PerfilID))
                    {
                        listaPerfiles.Add(filaDocRol.PerfilID);
                    }
                }

                IdentidadAD identidadAd = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadAD>(), mLoggerFactory);
                DataWrapperIdentidad dataWrapperIdentidad = identidadAd.ObtenerIdentidadesDePerfilesEnProyecto(listaPerfiles, pProyectoID);

                Dictionary<Guid, Guid> listaPerfilIdentidad = new Dictionary<Guid, Guid>();
                foreach (EntityModel.Models.IdentidadDS.Identidad filaIdentidad in dataWrapperIdentidad.ListaIdentidad)
                {
                    if (!listaPerfilIdentidad.ContainsKey(filaIdentidad.PerfilID))
                    {
                        listaPerfilIdentidad.Add(filaIdentidad.PerfilID, filaIdentidad.IdentidadID);
                    }
                }

                foreach (var filaDocRol in listaDocumentoRolIdentidad)
                {
                    if (listaEditoresRecursos.ContainsKey(filaDocRol.DocumentoID))
                    {
                        //Da fallo en administrar categorias comunidad al guardar.
                        if (listaPerfilIdentidad.ContainsKey(filaDocRol.PerfilID))
                        {
                            listaEditoresRecursos[filaDocRol.DocumentoID].Add(listaPerfilIdentidad[filaDocRol.PerfilID]);
                        }
                    }
                    else
                    {
                        if (listaPerfilIdentidad.ContainsKey(filaDocRol.PerfilID))
                        {
                            List<Guid> listaEditores = new List<Guid>();
                            listaEditores.Add(listaPerfilIdentidad[filaDocRol.PerfilID]);
                            listaEditoresRecursos.Add(filaDocRol.DocumentoID, listaEditores);
                        }
                    }
                }
            }

            return listaEditoresRecursos;
        }

        public DataWrapperDocumentacion ObtenerDocWebAgCatTesauroPorCategoriasId(List<Guid> pListaCategorias)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            dataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro = mEntityContext.DocumentoWebAgCatTesauro.Where(item => pListaCategorias.Contains(item.CategoriaTesauroID)).ToList();

            return dataWrapperDocumentacion;
        }

        #endregion

        #region Documentos Temporales

        /// <summary>
        /// Comprueba si el enlace del recurso ya existe y devuelve la fila del documento
        /// </summary>
        /// <param name="pNombreEnlaceTemporal">Nombre del enlace del documento temporal</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pObtenerSimilares">Obtner los documentos que se llaman parecidos</param>
        /// <param name="pBuscarEnTitulo">Verdad si se debe buscar también en el titulo del documento</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerDocumentoDeEnlace(string pNombreEnlaceTemporal, Guid pUsuarioID, bool pObtenerSimilares, bool pBuscarEnTitulo)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            var resultadoDocumentoDeEnlace = mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(objeto => objeto.DocumentoRolIdentidad.Editor && objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Documento.Enlace.Contains(pNombreEnlaceTemporal));
            if (pBuscarEnTitulo)
            {
                resultadoDocumentoDeEnlace = mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(objeto => objeto.DocumentoRolIdentidad.Editor && objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && (objeto.Documento.Enlace.Contains(pNombreEnlaceTemporal) || objeto.Documento.Titulo.Contains(pNombreEnlaceTemporal)));
            }

            var resultadoDocumentoDeEnlaceParte2 = mEntityContext.Documento.JoinDocumentoIdentidad().JoinPerfil().JoinAdministradorOrganizacion().Where(objeto => objeto.Documento.Enlace.Contains(pNombreEnlaceTemporal) && objeto.Identidad.Tipo == 3 && objeto.AdministradorOrganizacion.UsuarioID.Equals(pUsuarioID));
            if (pBuscarEnTitulo)
            {
                resultadoDocumentoDeEnlaceParte2 = mEntityContext.Documento.JoinDocumentoIdentidad().JoinPerfil().JoinAdministradorOrganizacion().Where(objeto =>
                (objeto.Documento.Enlace.Contains(pNombreEnlaceTemporal) || objeto.Documento.Titulo.Contains(pNombreEnlaceTemporal)) && objeto.Identidad.Tipo == 3 && objeto.AdministradorOrganizacion.UsuarioID.Equals(pUsuarioID));
            }
            dataWrapperDocumentacion.ListaDocumento = resultadoDocumentoDeEnlace.ToList().Select(objeto => objeto.Documento).Concat(resultadoDocumentoDeEnlaceParte2.ToList().Select(objeto => objeto.Documento)).OrderBy(doc => doc.UltimaVersion).ToList().Distinct().ToList();
            if (pObtenerSimilares)
            {
                dataWrapperDocumentacion.ListaDocumento = resultadoDocumentoDeEnlace.ToList().Select(objeto => new Documento
                {
                    DocumentoID = objeto.Documento.DocumentoID,
                    OrganizacionID = objeto.Documento.OrganizacionID,
                    CompartirPermitido = objeto.Documento.CompartirPermitido,
                    ElementoVinculadoID = objeto.Documento.ElementoVinculadoID,
                    Titulo = objeto.Documento.Titulo,
                    Descripcion = objeto.Documento.Descripcion,
                    Tipo = objeto.Documento.Tipo,
                    Enlace = objeto.Documento.Enlace,
                    FechaCreacion = objeto.Documento.FechaCreacion,
                    TipoEntidad = objeto.Documento.TipoEntidad,
                    NombreCategoriaDoc = objeto.Documento.NombreCategoriaDoc,
                    NombreElementoVinculado = objeto.Documento.NombreElementoVinculado,
                    Publico = objeto.Documento.Publico,
                    Borrador = objeto.Documento.Borrador,
                    FichaBibliograficaID = objeto.Documento.FichaBibliograficaID,
                    CreadorEsAutor = objeto.Documento.CreadorEsAutor,
                    Valoracion = objeto.Documento.Valoracion,
                    Autor = objeto.Documento.Autor,
                    FechaModificacion = objeto.Documento.FechaModificacion,
                    IdentidadProteccionID = objeto.Documento.IdentidadProteccionID,
                    FechaProteccion = objeto.Documento.FechaProteccion,
                    Protegido = objeto.Documento.Protegido,
                    UltimaVersion = objeto.Documento.UltimaVersion,
                    Eliminado = objeto.Documento.Eliminado,
                    NumeroComentariosPublicos = objeto.Documento.NumeroComentariosPublicos,
                    NumeroTotalVotos = objeto.Documento.NumeroTotalVotos,
                    NumeroTotalConsultas = objeto.Documento.NumeroTotalConsultas,
                    NumeroTotalDescargas = objeto.Documento.NumeroTotalDescargas,
                    VersionFotoDocumento = objeto.Documento.VersionFotoDocumento,
                    Rank = objeto.Documento.Rank,
                    Rank_Tiempo = objeto.Documento.Rank_Tiempo,
                    Licencia = objeto.Documento.Licencia,
                    Tags = objeto.Documento.Tags,
                    Visibilidad = objeto.Documento.Visibilidad,
                    CreadorID = objeto.Documento.CreadorID,
                    ProyectoID = objeto.Documento.ProyectoID
                }).Concat(resultadoDocumentoDeEnlaceParte2.ToList().Select(objeto => new Documento
                {
                    DocumentoID = objeto.Documento.DocumentoID,
                    OrganizacionID = objeto.Documento.OrganizacionID,
                    CompartirPermitido = objeto.Documento.CompartirPermitido,
                    ElementoVinculadoID = objeto.Documento.ElementoVinculadoID,
                    Titulo = objeto.Documento.Titulo,
                    Descripcion = objeto.Documento.Descripcion,
                    Tipo = objeto.Documento.Tipo,
                    Enlace = objeto.Documento.Enlace,
                    FechaCreacion = objeto.Documento.FechaCreacion,
                    TipoEntidad = objeto.Documento.TipoEntidad,
                    NombreCategoriaDoc = objeto.Documento.NombreCategoriaDoc,
                    NombreElementoVinculado = objeto.Documento.NombreElementoVinculado,
                    Publico = objeto.Documento.Publico,
                    Borrador = objeto.Documento.Borrador,
                    FichaBibliograficaID = objeto.Documento.FichaBibliograficaID,
                    CreadorEsAutor = objeto.Documento.CreadorEsAutor,
                    Valoracion = objeto.Documento.Valoracion,
                    Autor = objeto.Documento.Autor,
                    FechaModificacion = objeto.Documento.FechaModificacion,
                    IdentidadProteccionID = objeto.Documento.IdentidadProteccionID,
                    FechaProteccion = objeto.Documento.FechaProteccion,
                    Protegido = objeto.Documento.Protegido,
                    UltimaVersion = objeto.Documento.UltimaVersion,
                    Eliminado = objeto.Documento.Eliminado,
                    NumeroComentariosPublicos = objeto.Documento.NumeroComentariosPublicos,
                    NumeroTotalVotos = objeto.Documento.NumeroTotalVotos,
                    NumeroTotalConsultas = objeto.Documento.NumeroTotalConsultas,
                    NumeroTotalDescargas = objeto.Documento.NumeroTotalDescargas,
                    VersionFotoDocumento = objeto.Documento.VersionFotoDocumento,
                    Rank = objeto.Documento.Rank,
                    Rank_Tiempo = objeto.Documento.Rank_Tiempo,
                    Licencia = objeto.Documento.Licencia,
                    Tags = objeto.Documento.Tags,
                    Visibilidad = objeto.Documento.Visibilidad,
                    CreadorID = objeto.Identidad.IdentidadID,
                    ProyectoID = objeto.Documento.ProyectoID
                })).OrderBy(doc => doc.UltimaVersion).Distinct(new ComparadorDocumento()).ToList();
            }

            if (dataWrapperDocumentacion.ListaDocumento.Count > 0)
            {
                var resultadoDocumentoRolIdentidadDeEnlace = mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(objeto => objeto.DocumentoRolIdentidad.Editor && objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Documento.Enlace.Contains(pNombreEnlaceTemporal));
                if (pBuscarEnTitulo)
                {
                    resultadoDocumentoDeEnlace = mEntityContext.Documento.JoinDocumentoDocumentoRolIdentidad().JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(objeto => objeto.DocumentoRolIdentidad.Editor && objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && (objeto.Documento.Enlace.Contains(pNombreEnlaceTemporal) || objeto.Documento.Titulo.Contains(pNombreEnlaceTemporal)));
                }
                dataWrapperDocumentacion.ListaDocumentoRolIdentidad = resultadoDocumentoRolIdentidadDeEnlace.Select(objeto => objeto.DocumentoRolIdentidad).Distinct().ToList();
            }

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene un documento por el enlace del mismo
        /// </summary>        
        /// <param name="pEnlace"></param>
        /// <returns></returns>
        public Documento ObtenerDocumentoPorEnlace(string pEnlace)
        {
            return mEntityContext.Documento.Where(item => item.Enlace.ToLower().Equals(pEnlace.ToLower())).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los documentos temporales creados a partir de otro no temporal.
        /// </summary>
        /// <param name="pDocumentoOriginalID">Documento no temporal del que se ha realizado las versiones temporales</param>
        /// <param name="pIdentidadID">Identidad que ha creado los documentos temporales</param>
        /// <param name="pTipoDocumento">Indica el tipo de documento temporal o null si es indiferente</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerDocumentosTemporalesDeDocumento(Guid pDocumentoOriginalID, Guid pIdentidadID, TiposDocumentacion? pTipoDocumento)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            if (pTipoDocumento.HasValue)
            {
                dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(doc => !doc.Eliminado && doc.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Temporal) && doc.ElementoVinculadoID.Value.Equals(pDocumentoOriginalID) && doc.CreadorID.Equals(pIdentidadID) && doc.Tipo.Equals((short)pTipoDocumento.Value)).ToList();
            }
            else
            {
                dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(doc => !doc.Eliminado && doc.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Temporal) && doc.ElementoVinculadoID.Value.Equals(pDocumentoOriginalID) && doc.CreadorID.Equals(pIdentidadID)).ToList();
            }

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene los documentos temporales creados a partir de otro no temporal.
        /// </summary>
        /// <param name="pNombreDocumentoOriginal">Nombre del documento temporal</param>
        /// <param name="pIdentidadID">Identidad que ha creado los documentos temporales</param>
        /// <param name="pTipoDocumento">Indica el tipo de documento temporal o null si es indiferente</param>
        /// <returns></return s>
        public DataWrapperDocumentacion ObtenerDocumentosTemporalesDeDocumentoPorNombre(string pNombreDocumentoOriginal, Guid pIdentidadID, TiposDocumentacion? pTipoDocumento)
        {
            DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();
            List<Documento> listaDocumento = new List<Documento>();
            if (!string.IsNullOrEmpty(pNombreDocumentoOriginal))
            {
                listaDocumento = mEntityContext.Documento.Where(doc => !doc.Eliminado && doc.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Temporal) && doc.Titulo.Equals(pNombreDocumentoOriginal) && doc.CreadorID.Equals(pIdentidadID)).ToList();
                if (pTipoDocumento.HasValue)
                {
                    listaDocumento = listaDocumento.Where(doc => doc.Tipo.Equals((short)pTipoDocumento.Value)).ToList();
                }
            }
            else
            {
                listaDocumento = mEntityContext.Documento.Where(doc => !doc.Eliminado && doc.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Temporal) && doc.CreadorID.Equals(pIdentidadID)).ToList();
                if (pTipoDocumento.HasValue)
                {
                    listaDocumento = listaDocumento.Where(doc => doc.Tipo.Equals((short)pTipoDocumento.Value)).ToList();
                }
            }

            docDW.ListaDocumento = listaDocumento;

            CargarDocumentoWebVinBRYDocumentoWebAgCatTesDeDocumentosCargados(docDW);

            return docDW;
        }

        #endregion

        #region Documento NewsLetter

        /// <summary>
        /// Obtiene los datos de los envios realizados de un documento de tipo newsletter.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento newletter</param>
        /// <returns>DataSet con los datos cargados de envio de documentos</return s>
        public DataWrapperDocumentacion ObtenerEnviosNewsLetterPorDocumentoID(Guid pDocumentoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            dataWrapperDocumentacion.ListaDocumentoEnvioNewsLetter = mEntityContext.DocumentoEnvioNewsLetter.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();

            return dataWrapperDocumentacion;
        }

        #endregion

        #region Cola Documento

        /// <summary>
        /// Agrega el documento a la cola para que sea procesado por un servicio.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pDocAgregado">Indica si el documento a sido agregado (TRUE), o es modificado (FALSE)</param>
        public void AgregarDocumentoAColaTareas(Guid pDocumentoID, bool pDocAgregado, PrioridadColaDocumento pPrioridadColaDocumento, long pEstadoCargaID)
        {
            ColaDocumento colaDocumento = new ColaDocumento();

            colaDocumento.DocumentoID = pDocumentoID;
            if (pDocAgregado)
            {
                colaDocumento.AccionRealizada = (short)AccionHistorialDocumento.Agregar;
            }
            else
            {
                colaDocumento.AccionRealizada = (short)AccionHistorialDocumento.GuardarDocumento;
            }
            colaDocumento.Estado = (short)EstadoElementoCola.Espera;
            colaDocumento.FechaEncolado = DateTime.Now;
            colaDocumento.Prioridad = (short)pPrioridadColaDocumento;

            //Se agega aunque sea -1
            colaDocumento.EstadoCargaID = pEstadoCargaID;
            try
            {
                InsertarFilaEnColaMiniatura(colaDocumento);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla ColaDocumento", mlogger);
                mEntityContext.ColaDocumento.Add(colaDocumento);
                ActualizarBaseDeDatosEntityContext();
            }
        }

        public void AgregarDocumentoAColaTareas(List<Guid> pDocumentosID, bool pDocAgregado, PrioridadColaDocumento pPrioridadColaDocumento, long pEstadoCargaID)
        {
            List<string> elementosAInsertar = new List<string>();

            foreach (Guid documentoID in pDocumentosID)
            {
                ColaDocumento colaDocumento = new ColaDocumento();

                colaDocumento.DocumentoID = documentoID;
                if (pDocAgregado)
                {
                    colaDocumento.AccionRealizada = (short)AccionHistorialDocumento.Agregar;
                }
                else
                {
                    colaDocumento.AccionRealizada = (short)AccionHistorialDocumento.GuardarDocumento;
                }
                colaDocumento.Estado = (short)EstadoElementoCola.Espera;
                colaDocumento.FechaEncolado = DateTime.Now;
                colaDocumento.Prioridad = (short)pPrioridadColaDocumento;

                //Se agega aunque sea -1
                colaDocumento.EstadoCargaID = pEstadoCargaID;
                elementosAInsertar.Add(JsonConvert.SerializeObject(colaDocumento));
            }

            InsertarFilasEnColaMiniatura(elementosAInsertar);

        }

        public void InsertarFilaEnColaMiniatura(ColaDocumento pColaDocumento)
        {

            if (!string.IsNullOrEmpty(mConfigService.ObtenerRabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN)))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_MINIATURA, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, EXCHANGE, COLA_MINIATURA))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pColaDocumento));
                }
            }
            else
            {
                mEntityContext.ColaDocumento.Add(pColaDocumento);
                ActualizarBaseDeDatosEntityContext();
            }
        }

        public void InsertarFilasEnColaMiniatura(List<string> pFilasDocumento)
        {
            if (!string.IsNullOrEmpty(mConfigService.ObtenerRabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN)))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_MINIATURA, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, EXCHANGE, COLA_MINIATURA))
                {
                    List<string> mensajesFallidos = (List<string>)rabbitMQ.AgregarElementosACola(pFilasDocumento);
                    if (mensajesFallidos.Count > 0)
                    {
                        foreach (string mensajeFallido in mensajesFallidos)
                        {
                            ColaDocumento colaDocumento = JsonConvert.DeserializeObject<ColaDocumento>(mensajeFallido);
                            mLoggingService.GuardarLogError($"Fallo al insertar en Rabbit:\n Cola Documento ID {colaDocumento.ID}, Documento ID {colaDocumento.DocumentoID}\n insertamos en la base de datos, tabla ColaDocumento", mlogger);
                            mEntityContext.ColaDocumento.Add(colaDocumento);
                            ActualizarBaseDeDatosEntityContext();
                        }
                    }
                }
            }
            else
            {
                foreach (string filaDocumento in pFilasDocumento)
                {
                    mEntityContext.ColaDocumento.Add(JsonConvert.DeserializeObject<ColaDocumento>(filaDocumento));
                }
                ActualizarBaseDeDatosEntityContext();
            }
        }

        /// <summary>
        /// Obtiene los documentos que están en la ColaDocumento sin procesar y sin desechar.
        /// </summary>
        /// <param name="pNumDoc">Número de documentos para procesar</param>
        /// <returns>DataSet de documentación con los datos cargados</return s>
        public DataWrapperDocumentacion ObtenerDocumentosEnColaParaProcesar(int pNumDoc)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            dataWrapperDocumentacion.ListaColaDocumento = mEntityContext.ColaDocumento.Where(item => item.Estado < ((short)EstadoElementoCola.Fallido)).OrderBy(item => item.Prioridad).Take(pNumDoc).ToList();

            //Traigo infomación de los documentos (tabla Documento y DocumentoWebVinBaseRecursos):
            if (dataWrapperDocumentacion.ListaColaDocumento.Count > 0)
            {
                List<Guid> listaDocumentoDeCola = dataWrapperDocumentacion.ListaColaDocumento.Select(item => item.DocumentoID).ToList();
                dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(item => !item.Eliminado && listaDocumentoDeCola.Contains(item.DocumentoID)).ToList();
                dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(item => item.TipoPublicacion.Equals(0) && listaDocumentoDeCola.Contains(item.DocumentoID)).ToList();

                if (dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Count > 0)
                {
                    List<Guid> listaBaseRecursosID = dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Select(item => item.BaseRecursosID).ToList();
                    dataWrapperDocumentacion.ListaBaseRecursosUsuario = mEntityContext.BaseRecursosUsuario.Where(item => listaBaseRecursosID.Contains(item.BaseRecursosID)).ToList();
                    dataWrapperDocumentacion.ListaBaseRecursosOrganizacion = mEntityContext.BaseRecursosOrganizacion.Where(item => listaBaseRecursosID.Contains(item.BaseRecursosID)).ToList();
                    dataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Where(item => listaBaseRecursosID.Contains(item.BaseRecursosID)).ToList();
                }
            }

            return dataWrapperDocumentacion;
        }

        public DataWrapperDocumentacion ObtenerDocumentosColaDocumentoRabbitMQ(Guid pDocumentoID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //Documento
            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Where(item => !item.Eliminado && item.DocumentoID.Equals(pDocumentoID)).ToList();

            //DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.Where(item => item.TipoPublicacion.Equals(0) && item.DocumentoID.Equals(pDocumentoID)).ToList();

            if (dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Count > 0)
            {
                List<Guid> listaBaseRecursosID = dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Select(item => item.BaseRecursosID).ToList();
                //BaseRecursosUsuario
                dataWrapperDocumentacion.ListaBaseRecursosUsuario = mEntityContext.BaseRecursosUsuario.Where(item => listaBaseRecursosID.Contains(item.BaseRecursosID)).ToList();

                //BaseRecursosOrganizacion
                dataWrapperDocumentacion.ListaBaseRecursosOrganizacion = mEntityContext.BaseRecursosOrganizacion.Where(item => listaBaseRecursosID.Contains(item.BaseRecursosID)).ToList();

                //BaseRecursosProyecto
                dataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.Where(item => listaBaseRecursosID.Contains(item.BaseRecursosID)).ToList();
            }

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Obtiene las imagenes de los documentos
        /// </summary>
        /// <param name="pDocumentosID">Lista de identificadores de documento</param>
        /// <returns>Lista con el campo NombreCategoriaDoc </return s>
        public List<string> ObtenerImagenDocumentos(List<Guid> pDocumentosID)
        {
            List<string> imagenes = new List<string>();

            imagenes = mEntityContext.Documento.Where(documento => pDocumentosID.Contains(documento.DocumentoID) && !string.IsNullOrEmpty(documento.NombreCategoriaDoc)).Select(documento => documento.NombreCategoriaDoc).ToList();

            return imagenes;
        }

        /// <summary>
        /// Comprueba si el documento actual es la última versión
        /// </summary>
        /// <param name="pDocumentoID">Documentoid</param>
        /// <returns></return s>
        public bool ComprobarSiEsUltimaVersionDocumento(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Any(documento => documento.DocumentoID.Equals(pDocumentoID) && documento.UltimaVersion);
        }

        /// <summary>
        /// Devuelve las versiónes de la imagenes o Nulo si no tienen imagen
        /// </summary>
        /// <param name="pListaDocumentos">Lista de documentos</param>
        /// <returns></return s>
        public Dictionary<Guid, int?> ComprobarSiListaDocumentosTienenImagen(List<Guid> pListaDocumentos)
        {
            Dictionary<Guid, int?> listaDocs = new Dictionary<Guid, int?>();

            if (pListaDocumentos.Count > 0)
            {
                var resultado = mEntityContext.Documento.Where(documento => pListaDocumentos.Contains(documento.DocumentoID)).Select(documento => new { documento.DocumentoID, documento.VersionFotoDocumento }).ToList();

                foreach (var fila in resultado)
                {
                    listaDocs.Add(fila.DocumentoID, fila.VersionFotoDocumento);
                }
            }
            return listaDocs;
        }

        /// <summary>
        /// Devuelve las versiónes de la imagenes o Nulo si no tienen imagen
        /// </summary>
        /// <param name="pListaDocumentos">Lista de documentos</param>
        /// <returns></return s>
        public Dictionary<Guid, bool> ComprobarSiOntologiaTieneRecursos(List<Guid> pListaDocumentos)
        {
            Dictionary<Guid, bool> listaDocs = new Dictionary<Guid, bool>();

            if (pListaDocumentos.Count > 0)
            {
                listaDocs = mEntityContext.Documento.Where(documento => !documento.Eliminado && documento.ElementoVinculadoID.HasValue && pListaDocumentos.Contains(documento.ElementoVinculadoID.Value)).Select(item => item.ElementoVinculadoID.Value).Distinct().ToDictionary(item => item, item => true);
            }
            return listaDocs;
        }

        /// Indica si la ontologia pasada por parámetro tiene algun recurso vinculado
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <returns>True si tiene elementos vinculados, False si no</returns>
        public bool OntologiaTieneRecursos(Guid pOntologiaID)
        {
            return mEntityContext.Documento.Any(documento => !documento.Eliminado && documento.ElementoVinculadoID.HasValue && pOntologiaID.Equals(documento.ElementoVinculadoID.Value));
        }

        #endregion
        #region mejoras en flujos
        public VersionDocumento ObtenerVersionPorVersionID(Guid pDocumentoID)
        {
            return mEntityContext.VersionDocumento.FirstOrDefault(item => item.DocumentoID.Equals(pDocumentoID));
        }

        public List<VersionDocumento> ObtenerVersionesPorDocumentoOriginalID(Guid pDocumentoID)
        {
            return mEntityContext.VersionDocumento.Where(item => item.DocumentoOriginalID.Equals(pDocumentoID)).ToList();
        }
        /// <summary>
        /// Cambia la mejora a Activa y el resto de versiones asociadas a la versión pero que no es la aceptada a historico
        /// </summary>
        /// <param name="pVersionDocumento"></param>
        public void CambiarEstadoVersionesDeMejoraAprobada(VersionDocumento pVersionDocumento)
        {
            if (pVersionDocumento.MejoraID.HasValue)
            {
                List<VersionDocumento> listaVersiones = mEntityContext.VersionDocumento.Where(item => (item.MejoraID.HasValue && item.MejoraID.Value.Equals(pVersionDocumento.MejoraID.Value)) && !item.DocumentoID.Equals(pVersionDocumento.DocumentoID)).ToList();
                foreach (VersionDocumento versionDocumento in listaVersiones)
                {
                    versionDocumento.EstadoVersion = (short)EstadoVersion.Historico;
                }
            }
            pVersionDocumento.EstadoVersion = (short)EstadoVersion.Vigente;
            pVersionDocumento.EsMejora = false;
        }
        public void EliminarVersionesDeMejora(Guid pMejoraID)
        {
            List<VersionDocumento> listaVersionesDocumento = mEntityContext.VersionDocumento.Where(item => item.MejoraID.HasValue && item.MejoraID.Value.Equals(pMejoraID)).ToList();
            foreach (VersionDocumento versionDocumento in listaVersionesDocumento)
            {
                mEntityContext.EliminarElemento(versionDocumento);
            }
            List<Guid> listaVersiones = listaVersionesDocumento.Select(item => item.DocumentoID).ToList();
            EliminarDocumentos(listaVersiones);
        }
        #endregion
        /// <summary>
        /// Obtiene los editores de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documentos</param>
        /// <returns>Tabla NombrePerfil, NombreGrupo y NombreGrupoOrg cargadas con los editores de los documentos</return s>
        public DataWrapperDocumentacion ObtenerEditoresYGruposEditoresDocumentos(List<Guid> pListaDocumentosID)
        {
            return ObtenerEditoresLectoresDocumentos(pListaDocumentosID, true);
        }

        /// <summary>
        /// Obtiene los lectores de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documentos</param>
        /// <returns>Tabla NombrePerfil, NombreGrupo y NombreGrupoOrg cargadas con los lectores de los documentos</return s>
        public DataWrapperDocumentacion ObtenerLectoresYGruposLectoresDocumentos(List<Guid> pListaDocumentosID)
        {
            return ObtenerEditoresLectoresDocumentos(pListaDocumentosID, false);
        }
        /// <summary>
        /// Obtiene los lectores o editores de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documentos</param>
        /// <param name="pEditores">True para obtener solo editores, False para obtener solo lectores</param>
        /// <returns>Tabla NombrePerfil, NombreGrupo y NombreGrupoOrg cargadas con los lectores o editores de los documentos</return s>
        private DataWrapperDocumentacion ObtenerEditoresLectoresDocumentos(List<Guid> pListaDocumentosID, bool pEditores)
        {

            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            if (pListaDocumentosID.Count > 0)
            {
                dataWrapperDocumentacion.ListaNombrePerfil = mEntityContext.DocumentoRolIdentidad.Join(mEntityContext.Identidad, docRol => docRol.PerfilID, identidad => identidad.PerfilID, (docRol, identidad) => new
                {
                    DocumentoRolIdentidad = docRol,
                    Identidad = identidad
                }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
                {
                    DocumentoRolIdentidad = objeto.DocumentoRolIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = perfil
                }).Where(objeto => objeto.DocumentoRolIdentidad.Editor.Equals(pEditores) && pListaDocumentosID.Contains(objeto.DocumentoRolIdentidad.DocumentoID)).Distinct().Select(objeto => new NombrePerfil
                {
                    NombrePerfilAtributo = objeto.Perfil.NombreCortoUsu,
                    DocumentoID = objeto.DocumentoRolIdentidad.DocumentoID
                }).Distinct().ToList();

                dataWrapperDocumentacion.ListaNombreGrupo = mEntityContext.DocumentoRolGrupoIdentidades.Join(mEntityContext.GrupoIdentidadesProyecto, docRol => docRol.GrupoID, grupoIden => grupoIden.GrupoID, (docRol, grupoIDen) => new
                {
                    DocumentoRolGrupoIdentidades = docRol,
                    GrupoIdentidadesProyecto = grupoIDen
                }).Join(mEntityContext.GrupoIdentidades, objeto => objeto.DocumentoRolGrupoIdentidades.GrupoID, grupoIden => grupoIden.GrupoID, (objeto, grupoIden) => new
                {
                    DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesProyecto = objeto.GrupoIdentidadesProyecto,
                    GrupoIdentidades = grupoIden
                }).Where(objeto => objeto.DocumentoRolGrupoIdentidades.Editor.Equals(pEditores) && pListaDocumentosID.Contains(objeto.DocumentoRolGrupoIdentidades.DocumentoID)).ToList().Select(objeto => new NombreGrupo
                {
                    NombreGrupoAtributo = objeto.GrupoIdentidades.NombreCorto,
                    DocumentoID = objeto.DocumentoRolGrupoIdentidades.DocumentoID
                }).Distinct().ToList();


                dataWrapperDocumentacion.ListaNombreGrupoOrg = mEntityContext.DocumentoRolGrupoIdentidades.Join(mEntityContext.GrupoIdentidadesOrganizacion, docuRol => docuRol.GrupoID, grupoIden => grupoIden.GrupoID, (docuRol, grupoIden) => new
                {
                    DocumentoRolGrupoIdentidades = docuRol,
                    GrupoIdentidadesOrganizacion = grupoIden
                }).Join(mEntityContext.GrupoIdentidades, objeto => objeto.DocumentoRolGrupoIdentidades.GrupoID, grupoIen => grupoIen.GrupoID, (objeto, grupoIden) => new
                {
                    DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesOrganizacion = objeto.GrupoIdentidadesOrganizacion,
                    GrupoIdentidades = grupoIden
                }).Join(mEntityContext.Organizacion, objeto => objeto.GrupoIdentidadesOrganizacion.OrganizacionID, org => org.OrganizacionID, (objeto, org) => new
                {
                    DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesOrganizacion = objeto.GrupoIdentidadesOrganizacion,
                    GrupoIdentidades = objeto.GrupoIdentidades,
                    Organizacion = org
                }).Where(objeto => objeto.DocumentoRolGrupoIdentidades.Editor.Equals(pEditores) && pListaDocumentosID.Contains(objeto.DocumentoRolGrupoIdentidades.DocumentoID)).ToList().Select(objeto => new NombreGrupoOrg
                {
                    NombreGrupo = objeto.GrupoIdentidades.NombreCorto,
                    NombreOrganizacion = objeto.Organizacion.NombreCorto,
                    DocumentoID = objeto.DocumentoRolGrupoIdentidades.DocumentoID
                }).Distinct().ToList();
            }

            return dataWrapperDocumentacion;
        }

        public DataWrapperDocumentacion ObtenerUltimosRecursosVisitados(int pNumHorasIntervalo)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            //DocumentoWebVinBaseRecursos
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumentoWebVinBaseRecursosDocumentoWebVinBaseRecursosExtra().Where(objeto => objeto.DocumentoWebVinBaseRecursosExtra.FechaUltimaVisita.HasValue && EF.Functions.DateDiffHour(objeto.DocumentoWebVinBaseRecursosExtra.FechaUltimaVisita.Value, DateTime.Now) <= pNumHorasIntervalo).Select(objeto => objeto.DocumentoWebVinBaseRecursos).ToList();

            //DocumentoWebVinBaseRecursosExtra
            dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursosExtra = mEntityContext.DocumentoWebVinBaseRecursosExtra.Where(documentoWebVinBaseRecursosExtra => documentoWebVinBaseRecursosExtra.FechaUltimaVisita.HasValue && EF.Functions.DateDiffHour(documentoWebVinBaseRecursosExtra.FechaUltimaVisita.Value, DateTime.Now) <= pNumHorasIntervalo).ToList();

            //BaseRecursosProyecto
            dataWrapperDocumentacion.ListaBaseRecursosProyecto = mEntityContext.BaseRecursosProyecto.JoinBaseRecursosProyectoDocumentoWebVinBaseRecursosExtra().Where(objeto => objeto.DocumentoWebVinBaseRecursosExtra.FechaUltimaVisita.HasValue && EF.Functions.DateDiffHour(objeto.DocumentoWebVinBaseRecursosExtra.FechaUltimaVisita.Value, DateTime.Now) <= pNumHorasIntervalo).Select(objeto => objeto.BaseRecursosProyecto).Distinct().ToList();

            return dataWrapperDocumentacion;
        }

        public DataWrapperDocumentacion ObtenerOntologiasDeDocumentos(List<Guid> pListaDocumentosID)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            dataWrapperDocumentacion.ListaDocumento = mEntityContext.Documento.Join(mEntityContext.Documento, docOriginal => new { DocID = docOriginal.ElementoVinculadoID.Value }, docVinculado => new { DocID = docVinculado.DocumentoID }, (docOriginal, docVinculado) => new
            {
                DocOriginal = docOriginal,
                DocVinculado = docVinculado
            }).Where(objeto => objeto.DocOriginal.ElementoVinculadoID.HasValue && objeto.DocOriginal.Tipo != 7 && objeto.DocVinculado.Tipo == 7 && pListaDocumentosID.Contains(objeto.DocOriginal.DocumentoID)).Select(objeto => objeto.DocVinculado).Distinct().ToList();

            return dataWrapperDocumentacion;
        }

        /// <summary>
        /// Se obtiene el documento vinculado al pasado por parámetro
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento del cual queremos su documento vinculado</param>
        /// <returns></returns>
        public Documento ObtenerElementoVinculadoDeDocumento(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Join(mEntityContext.Documento, documentoOriginal => documentoOriginal.ElementoVinculadoID, documentoVinculado => documentoVinculado.DocumentoID, (documentoOriginal, documentoVinculado) => new
            {
                DocumentoOriginal = documentoOriginal,
                DocumentoVinculado = documentoVinculado
            }).Where(item => item.DocumentoOriginal.ElementoVinculadoID.HasValue && item.DocumentoOriginal.DocumentoID.Equals(pDocumentoID)).Select(item => item.DocumentoVinculado).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la URL canonica de un documento almacenada en la tabla DocumentoUrlCanonica
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></return s>
        public string ObtenerDocumentoUrlCanonica(Guid pDocumentoID)
        {
            return mEntityContext.DocumentoUrlCanonica.Where(item => item.DocumentoID.Equals(pDocumentoID)).Select(item => item.UrlCanonica).FirstOrDefault();
        }

        /// <summary>
        /// Guarda un recurso en la base de datos marcado com traducido por IA
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="targetLanguages"></param>
        public void GuardarTraduccionAutomatica(Guid pDocumentoID, List<string> pTargetLanguages)
        {
            EliminarTraduccionesAutomaticasDocumento(pDocumentoID);

            foreach (string idioma in pTargetLanguages)
            {
                IdiomaTraduccionAutomaticaDocumento idiomaTraduccionAutomaticaDocumento = new IdiomaTraduccionAutomaticaDocumento()
                {
                    DocumentoID = pDocumentoID,
                    Idioma = idioma
                };
                mEntityContext.IdiomaTraduccionAutomaticaDocumento.Add(idiomaTraduccionAutomaticaDocumento);
            }

            mEntityContext.SaveChanges();
        }
        public Guid? ObtenerEstadoIDDeDocumento(Guid pDocumentoID)
        {
            return mEntityContext.Documento.Where(x => x.DocumentoID.Equals(pDocumentoID)).Select(x => x.EstadoID).FirstOrDefault();
        }        

        public void EliminarTraduccionesAutomaticasDocumento(Guid pDocumentoID)
        {
			List<IdiomaTraduccionAutomaticaDocumento> listaTraduccionesAutomaticas = mEntityContext.IdiomaTraduccionAutomaticaDocumento.Where(item => item.DocumentoID.Equals(pDocumentoID)).ToList();
			foreach (IdiomaTraduccionAutomaticaDocumento idiomaTraduccionAutomaticaDocumento in listaTraduccionesAutomaticas)
			{
				mEntityContext.EliminarElemento(idiomaTraduccionAutomaticaDocumento);
			}

			mEntityContext.SaveChanges();
		}

		public bool ComprobarSiDocumentoEstaTraducidoConIAEnIdioma(Guid pDocumentoID, string pLanguageCode)
		{
            IdiomaTraduccionAutomaticaDocumento idiomaTraduccionAutomaticaDocumento = mEntityContext.IdiomaTraduccionAutomaticaDocumento.FirstOrDefault(x => x.DocumentoID.Equals(pDocumentoID) && x.Idioma.Equals(pLanguageCode));
            return idiomaTraduccionAutomaticaDocumento != null;
		}
        #region Estados        
        public void CambiarEstadoDocumento(Guid pDocumentoID, Guid pEstadoID)
        {
            Documento filaDoc = ObtenerDocumentoPorIdentificador(pDocumentoID);
            filaDoc.EstadoID = pEstadoID;
            VersionDocumento versionDocumento = ObtenerVersionPorVersionID(pDocumentoID);
            if (versionDocumento != null)
            {
                versionDocumento.EstadoID = pEstadoID;
            }

            mEntityContext.SaveChanges();
        }

		#endregion

		#endregion

		#region Privados


		/// <summary>
		/// Obtiene los datos de las tablas DocumentoWebVinBaseRecursos y DocumentoWebAgCatTesauro de los documentos ya cargados en la
		/// tabla Documento.
		/// </summary>
		/// <param name="pDocumentacionDS">DataSet con los documentos cargados y donde hay que cargar las tablas</param>
		private void CargarDocumentoWebVinBRYDocumentoWebAgCatTesDeDocumentosCargados(DataWrapperDocumentacion pDocumentacionDS)
        {
            //Cargo los Tag de los documentos antes cargados
            if (pDocumentacionDS.ListaDocumento.Count > 0)
            {
                List<Guid> listaDocumentosID = new List<Guid>();

                foreach (Documento fila in pDocumentacionDS.ListaDocumento)
                {
                    if (!listaDocumentosID.Contains(fila.DocumentoID))
                    {
                        listaDocumentosID.Add(fila.DocumentoID);
                    }
                }

                ObtenerDocumentosWebPorIDWEB(listaDocumentosID, pDocumentacionDS);
            }
        }
        #endregion

        #region Cargar Consultas y DataAdapters

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="pIBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos pIBD)
        {
            #region Consultas

            #region Select tablas

            this.selectDocumento = "SELECT " + IBD.CargarGuid("Documento.DocumentoID") + ", " + IBD.CargarGuid("Documento.OrganizacionID") + ", Documento.CompartirPermitido, " + IBD.CargarGuid("Documento.ElementoVinculadoID") + ", Documento.Titulo, Documento.Descripcion, Documento.Tipo, Documento.Enlace, Documento.FechaCreacion, " + IBD.CargarGuid("Documento.CreadorID") + ", Documento.TipoEntidad, Documento.NombreCategoriaDoc, Documento.NombreElementoVinculado, " + IBD.CargarGuid("Documento.ProyectoID") + ", Documento.Publico, Documento.Borrador, " + IBD.CargarGuid("Documento.FichaBibliograficaID") + ", Documento.CreadorEsAutor, Documento.Valoracion, Documento.Autor, Documento.FechaModificacion, " + IBD.CargarGuid("Documento.IdentidadProteccionID") + ", Documento.FechaProteccion, Documento.Protegido, Documento.UltimaVersion, Documento.Eliminado, Documento.NumeroComentariosPublicos, Documento.NumeroTotalVotos, Documento.NumeroTotalConsultas, Documento.NumeroTotalDescargas, Documento.VersionFotoDocumento, Documento.Rank, Documento.Rank_Tiempo, Documento.Licencia, Documento.Tags, Documento.Visibilidad ";
            this.selectBaseRecursos = "SELECT " + pIBD.CargarGuid("BaseRecursos.BaseRecursosID") + " ";
            this.selectBaseRecursosIDProyecto = "SELECT " + pIBD.CargarGuid("BaseRecursosProyecto.BaseRecursosID") + " ";
            this.selectTipologia = "SELECT " + pIBD.CargarGuid("Tipologia.TipologiaID") + ", " + pIBD.CargarGuid("Tipologia.AtributoID") + ", " + pIBD.CargarGuid("Tipologia.OrganizacionID") + ", " + pIBD.CargarGuid("Tipologia.ProyectoID") + ", Tipologia.Nombre, Tipologia.Descripcion, Tipologia.Tipo, Tipologia.Orden ";
            this.selectDocumentoTipologia = "SELECT " + pIBD.CargarGuid("DocumentoTipologia.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoTipologia.TipologiaID") + ", " + pIBD.CargarGuid("DocumentoTipologia.AtributoID") + ", DocumentoTipologia.Valor ";
            this.selectDocumentoRolIdentidad = "SELECT " + pIBD.CargarGuid("DocumentoRolIdentidad.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoRolIdentidad.PerfilID") + ", DocumentoRolIdentidad.Editor ";
            this.selectDocumentoRolGrupoIdentidades = "SELECT " + pIBD.CargarGuid("DocumentoRolGrupoIdentidades.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoRolGrupoIdentidades.GrupoID") + ", DocumentoRolGrupoIdentidades.Editor ";
            this.selectDocumentoGrupoUsuario = "SELECT " + pIBD.CargarGuid("DocumentoGrupoUsuario.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoGrupoUsuario.GrupoUsuarioID") + ", DocumentoGrupoUsuario.Editor ";
            this.selectHistorialDocumento = "SELECT " + pIBD.CargarGuid("HistorialDocumento.HistorialDocumentoID") + ", " + pIBD.CargarGuid("HistorialDocumento.DocumentoID") + ", " + pIBD.CargarGuid("HistorialDocumento.IdentidadID") + ", HistorialDocumento.Fecha, HistorialDocumento.TagNombre, " + pIBD.CargarGuid("HistorialDocumento.CategoriaTesauroID") + ", HistorialDocumento.Accion, " + pIBD.CargarGuid("HistorialDocumento.ProyectoID") + " ";
            this.selectVotoDocumento = "SELECT " + pIBD.CargarGuid("VotoDocumento.DocumentoID") + ", " + pIBD.CargarGuid("VotoDocumento.VotoID") + ", " + pIBD.CargarGuid("VotoDocumento.ProyectoID") + " ";
            this.selectVersionDocumento = "SELECT " + pIBD.CargarGuid("VersionDocumento.DocumentoID") + ", VersionDocumento.Version, " + pIBD.CargarGuid("VersionDocumento.DocumentoOriginalID") + ", " + pIBD.CargarGuid("VersionDocumento.IdentidadID") + " ";
            this.selectTagDocumento = "SELECT TagDocumento.TagID, TagDocumento.Tipo, " + pIBD.CargarGuid("TagDocumento.DocumentoID") + " ";
            this.selectFichaBibliografica = "SELECT " + pIBD.CargarGuid("FichaBibliografica.FichaBibliograficaID") + ", FichaBibliografica.Nombre, FichaBibliografica.Descripcion ";
            this.selectAtributoFichaBibliografica = "SELECT " + pIBD.CargarGuid("AtributoFichaBibliografica.AtributoID") + ", " + pIBD.CargarGuid("AtributoFichaBibliografica.FichaBibliograficaID") + ", AtributoFichaBibliografica.Nombre, AtributoFichaBibliografica.Descripcion, AtributoFichaBibliografica.Tipo, AtributoFichaBibliografica.Orden, AtributoFichaBibliografica.Longitud ";

            this.selectDocumentoAtributoBiblio = "SELECT " + pIBD.CargarGuid("DocumentoAtributoBiblio.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoAtributoBiblio.AtributoID") + ", DocumentoAtributoBiblio.Valor, " + pIBD.CargarGuid("DocumentoAtributoBiblio.FichaBibliograficaID");
            this.selectDocumentoComentario = "SELECT " + pIBD.CargarGuid("DocumentoComentario.ComentarioID") + ", " + pIBD.CargarGuid("DocumentoComentario.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoComentario.ProyectoID") + " ";

            this.selectDocumentoWebVinBaseRecursos = "SELECT " + pIBD.CargarGuid("DocumentoWebVinBaseRecursos.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoWebVinBaseRecursos.BaseRecursosID") + ", " + pIBD.CargarGuid("DocumentoWebVinBaseRecursos.IdentidadPublicacionID") + ", DocumentoWebVinBaseRecursos.FechaPublicacion, DocumentoWebVinBaseRecursos.Eliminado, " + pIBD.CargarGuid("DocumentoWebVinBaseRecursos.PublicadorOrgID") + ", DocumentoWebVinBaseRecursos.PermiteComentarios, DocumentoWebVinBaseRecursos.NumeroComentarios, DocumentoWebVinBaseRecursos.NumeroVotos, " + pIBD.CargarGuid("DocumentoWebVinBaseRecursos.NivelCertificacionID") + ", DocumentoWebVinBaseRecursos.Rank, DocumentoWebVinBaseRecursos.Rank_Tiempo, DocumentoWebVinBaseRecursos.IndexarRecurso, DocumentoWebVinBaseRecursos.PrivadoEditores, DocumentoWebVinBaseRecursos.TipoPublicacion, DocumentoWebVinBaseRecursos.LinkAComunidadOrigen, DocumentoWebVinBaseRecursos.FechaCertificacion ";

            this.selectDocumentoWebVinBaseRecursosExtra = "SELECT " + pIBD.CargarGuid("DocumentoWebVinBaseRecursosExtra.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoWebVinBaseRecursosExtra.BaseRecursosID") + ", DocumentoWebVinBaseRecursosExtra.NumeroDescargas, DocumentoWebVinBaseRecursosExtra.NumeroConsultas,  DocumentoWebVinBaseRecursosExtra.FechaUltimaVisita ";

            this.selectDocumentoWebVinBaseRecursosDistinct = selectDocumentoWebVinBaseRecursos.Replace("SELECT ", "SELECT Distinct ");

            this.selectDocumentoWebAgCatTesauro = "SELECT DocumentoWebAgCatTesauro.Fecha, " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.TesauroID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.CategoriaTesauroID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.BaseRecursosID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.DocumentoID") + " ";

            this.selectBaseRecursosOrganizacion = "SELECT " + pIBD.CargarGuid("BaseRecursosOrganizacion.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosOrganizacion.OrganizacionID") + ", BaseRecursosOrganizacion.EspacioMaxMyGnossMB, BaseRecursosOrganizacion.EspacioActualMyGnossMB ";

            this.selectBaseRecursosProyecto = "SELECT " + pIBD.CargarGuid("BaseRecursosProyecto.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosProyecto.OrganizacionID") + ", " + pIBD.CargarGuid("BaseRecursosProyecto.ProyectoID") + " ";
            this.selectBaseRecursosUsuario = "SELECT " + pIBD.CargarGuid("BaseRecursosUsuario.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosUsuario.UsuarioID") + ", BaseRecursosUsuario.EspacioMaxMyGnossMB, BaseRecursosUsuario.EspacioActualMyGnossMB ";
            this.selectDocumentoEnvioNewsLetter = "SELECT " + pIBD.CargarGuid("DocumentoEnvioNewsLetter.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoEnvioNewsLetter.IdentidadID") + ", DocumentoEnvioNewsLetter.Fecha, DocumentoEnvioNewsLetter.Idioma, DocumentoEnvioNewsLetter.EnvioSolicitado, DocumentoEnvioNewsLetter.EnvioRealizado, DocumentoEnvioNewsLetter.Grupos ";
            this.selectDocumentoRespuesta = "SELECT " + pIBD.CargarGuid("DocumentoRespuesta.RespuestaID") + ", " + pIBD.CargarGuid("DocumentoRespuesta.DocumentoID") + ", DocumentoRespuesta.Descripcion, DocumentoRespuesta.NumVotos, DocumentoRespuesta.Orden ";
            this.selectDocumentoRespuestaVoto = "SELECT " + pIBD.CargarGuid("DocumentoRespuestaVoto.RespuestaID") + ", " + pIBD.CargarGuid("DocumentoRespuestaVoto.DocumentoID") + pIBD.CargarGuid("DocumentoRespuestaVoto.IdentidadID") + " ";
            this.selectDocumentoTokenBrightcove = "SELECT " + pIBD.CargarGuid("DocumentoTokenBrightcove.TokenID") + ", " + pIBD.CargarGuid("DocumentoTokenBrightcove.ProyectoID") + ", " + pIBD.CargarGuid("DocumentoTokenBrightcove.OrganizacionID") + ", " + pIBD.CargarGuid("DocumentoTokenBrightcove.UsuarioID") + ", " + pIBD.CargarGuid("DocumentoTokenBrightcove.DocumentoID") + ", DocumentoTokenBrightcove.FechaCreacion, DocumentoTokenBrightcove.Estado, DocumentoTokenBrightcove.NombreArchivo ";
            this.selectDocumentoTokenTOP = "SELECT " + pIBD.CargarGuid("DocumentoTokenTOP.TokenID") + ", " + pIBD.CargarGuid("DocumentoTokenTOP.ProyectoID") + ", " + pIBD.CargarGuid("DocumentoTokenTOP.OrganizacionID") + ", " + pIBD.CargarGuid("DocumentoTokenTOP.UsuarioID") + ", " + pIBD.CargarGuid("DocumentoTokenTOP.DocumentoID") + ", DocumentoTokenTOP.FechaCreacion, DocumentoTokenTOP.Estado, DocumentoTokenTOP.NombreArchivo ";
            this.selectDocumentoNewsletter = "SELECT " + pIBD.CargarGuid("DocumentoNewsletter.DocumentoID") + ", DocumentoNewsletter.Newsletter, DocumentoNewsletter.NewsletterTemporal ";
            this.selectColaCargaRecursos = "SELECT ColaCargaRecursos.ColaID, " + IBD.CargarGuid("ColaCargaRecursos.ID") + ", " + IBD.CargarGuid("ColaCargaRecursos.ProyectoID") + ", " + IBD.CargarGuid("ColaCargaRecursos.UsuarioID") + ", ColaCargaRecursos.Fecha, ColaCargaRecursos.Estado, ColaCargaRecursos.NombreFichImport ";

            #endregion

            #region Consultas sencillas

            this.sqlSelectDocumentoSimple = selectDocumento;

            this.sqlSelectDocumento = selectDocumento + " FROM Documento";



            this.sqlSelectBaseRecursos = "SELECT DISTINCT " + pIBD.CargarGuid("BaseRecursos.BaseRecursosID") + " FROM BaseRecursos";

            this.sqlSelectBaseRecursosDocumentoWebVinBaseRecursos = "SELECT DISTINCT " + pIBD.CargarGuid("DocumentoWebVinBaseRecursos.BaseRecursosID") + " FROM DocumentoWebVinBaseRecursos";

            this.sqlSelectTipologia = "SELECT " + pIBD.CargarGuid("TipologiaID") + ", " + pIBD.CargarGuid("AtributoID") + ", " + pIBD.CargarGuid("OrganizacionID") + ", " + pIBD.CargarGuid("ProyectoID") + ", Nombre, Descripcion, Tipo, Orden FROM Tipologia";

            this.sqlSelectDocumentoTipologia = "SELECT " + pIBD.CargarGuid("DocumentoID") + ", " + pIBD.CargarGuid("TipologiaID") + ", " + pIBD.CargarGuid("AtributoID") + ", Valor FROM DocumentoTipologia";

            this.sqlSelectDocumentoRolIdentidad = "SELECT " + pIBD.CargarGuid("DocumentoRolIdentidad.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoRolIdentidad.PerfilID") + ", DocumentoRolIdentidad.Editor FROM DocumentoRolIdentidad";

            this.sqlSelectDocumentoRolGrupoIdentidades = "SELECT " + pIBD.CargarGuid("DocumentoRolGrupoIdentidades.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoRolGrupoIdentidades.GrupoID") + ", DocumentoRolGrupoIdentidades.Editor FROM DocumentoRolGrupoIdentidades";

            this.sqlSelectDocumentoGrupoUsuario = "SELECT " + pIBD.CargarGuid("DocumentoID") + ", " + pIBD.CargarGuid("GrupoUsuarioID") + ", Editor FROM DocumentoGrupoUsuario";

            this.sqlSelectHistorialDocumento = "SELECT " + pIBD.CargarGuid("HistorialDocumentoID") + ", " + pIBD.CargarGuid("DocumentoID") + ", " + pIBD.CargarGuid("IdentidadID") + ", Fecha, TagNombre, " + pIBD.CargarGuid("CategoriaTesauroID") + ", Accion, " + pIBD.CargarGuid("ProyectoID") + " FROM HistorialDocumento";

            this.sqlSelectVotoDocumento = "SELECT " + pIBD.CargarGuid("DocumentoID") + ", " + pIBD.CargarGuid("VotoID") + ", " + pIBD.CargarGuid("ProyectoID") + " FROM VotoDocumento";

            this.sqlSelectVersionDocumento = "SELECT " + pIBD.CargarGuid("DocumentoID") + ", Version, " + pIBD.CargarGuid("DocumentoOriginalID") + ", " + pIBD.CargarGuid("IdentidadID") + " FROM VersionDocumento";

            this.sqlSelectTagDocumento = "SELECT TagID, Tipo, " + IBD.CargarGuid("DocumentoID") + " FROM TagDocumento";

            this.sqlSelectFichaBibliografica = "SELECT " + pIBD.CargarGuid("FichaBibliograficaID") + ", Nombre, Descripcion FROM FichaBibliografica";

            this.sqlSelectAtributoFichaBibliografica = "SELECT " + pIBD.CargarGuid("AtributoID") + ", " + pIBD.CargarGuid("FichaBibliograficaID") + ", Nombre, Descripcion, Tipo, Orden, Longitud FROM AtributoFichaBibliografica";

            this.sqlSelectDocumentoAtributoBiblio = "SELECT " + pIBD.CargarGuid("DocumentoID") + ", " + pIBD.CargarGuid("AtributoID") + ", Valor, " + pIBD.CargarGuid("FichaBibliograficaID") + " FROM DocumentoAtributoBiblio";

            this.sqlSelectDocumentoComentario = "SELECT " + pIBD.CargarGuid("DocumentoComentario.ComentarioID") + ", " + pIBD.CargarGuid("DocumentoComentario.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoComentario.ProyectoID") + " FROM DocumentoComentario";

            this.sqlSelectDocumentoWebVinBaseRecursos = selectDocumentoWebVinBaseRecursos + " FROM DocumentoWebVinBaseRecursos";

            this.sqlSelectDocumentoWebVinBaseRecursosExtra = selectDocumentoWebVinBaseRecursosExtra + " FROM DocumentoWebVinBaseRecursosExtra";

            this.sqlSelectDocumentoWebAgCatTesauro = selectDocumentoWebAgCatTesauro + " FROM DocumentoWebAgCatTesauro";

            this.sqlSelectDocumentoWebAgCatTesauroSimple = "SELECT DocumentoWebAgCatTesauro.Fecha, " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.TesauroID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.CategoriaTesauroID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.BaseRecursosID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.DocumentoID");

            this.sqlSelectBaseRecursosOrganizacion = "SELECT DISTINCT " + pIBD.CargarGuid("BaseRecursosOrganizacion.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosOrganizacion.OrganizacionID") + ", BaseRecursosOrganizacion.EspacioMaxMyGnossMB, BaseRecursosOrganizacion.EspacioActualMyGnossMB FROM BaseRecursosOrganizacion";

            this.sqlSelectBaseRecursosProyecto = "SELECT DISTINCT " + pIBD.CargarGuid("BaseRecursosProyecto.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosProyecto.OrganizacionID") + ", " + pIBD.CargarGuid("BaseRecursosProyecto.ProyectoID") + " FROM BaseRecursosProyecto";

            this.sqlSelectBaseRecursosUsuario = "SELECT " + pIBD.CargarGuid("BaseRecursosUsuario.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosUsuario.UsuarioID") + ", BaseRecursosUsuario.EspacioMaxMyGnossMB, BaseRecursosUsuario.EspacioActualMyGnossMB FROM BaseRecursosUsuario";

            this.sqlSelectComprobarConcurrenciaDocumento = "SELECT DocumentoID FROM Documento WHERE DocumentoID = " + pIBD.ToParam("DocumentoID") + " AND datediff(minute, FechaModificacion, " + pIBD.ToParam("FechaModificacion") + ") != 0 AND UltimaVersion = 1 UNION ALL SELECT v2.DocumentoID FROM VersionDocumento v1 INNER JOIN VersionDocumento v2 ON v1.DocumentoOriginalID = v2.DocumentoOriginalID WHERE v1.DocumentoID = " + pIBD.ToParam("DocumentoID") + " AND v2.Version IN (SELECT max(v2.Version) FROM VersionDocumento v1 INNER JOIN VersionDocumento v2 ON v1.DocumentoOriginalID = v2.DocumentoOriginalID WHERE v1.DocumentoID = " + pIBD.ToParam("DocumentoID") + " GROUP BY v2.DocumentoOriginalID) AND v2.DocumentoID != " + pIBD.ToParam("DocumentoID") + " UNION ALL SELECT VersionDocumento.DocumentoID FROM VersionDocumento WHERE VersionDocumento.DocumentoOriginalID = " + pIBD.ToParam("DocumentoID");

            this.sqlSelectDistinctBaseRecursosProyecto = "SELECT DISTINCT " + pIBD.CargarGuid("BaseRecursosProyecto.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosProyecto.OrganizacionID") + ", " + pIBD.CargarGuid("BaseRecursosProyecto.ProyectoID") + " FROM BaseRecursosProyecto ";

            this.sqlSelectDistinctDocumentoWebAgCatTesauro = "SELECT DISTINCT DocumentoWebAgCatTesauro.Fecha, " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.TesauroID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.CategoriaTesauroID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.BaseRecursosID") + ", " + pIBD.CargarGuid("DocumentoWebAgCatTesauro.DocumentoID") + " FROM DocumentoWebAgCatTesauro";

            this.sqlSelectDistinctBaseRecursosUsuario = "SELECT DISTINCT " + pIBD.CargarGuid("BaseRecursosUsuario.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosUsuario.UsuarioID") + ", BaseRecursosUsuario.EspacioMaxMyGnossMB, BaseRecursosUsuario.EspacioActualMyGnossMB FROM BaseRecursosUsuario";

            this.selectDistinctDocumento = selectDocumento.Replace("SELECT", "SELECT DISTINCT");

            this.selectDistinctTagDocumento = "SELECT DISTINCT TagDocumento.TagID, TagDocumento.Tipo, " + IBD.CargarGuid("TagDocumento.DocumentoID") + " ";

            this.sqlSelectDocumentoEnvioNewsLetter = "SELECT " + pIBD.CargarGuid("DocumentoEnvioNewsLetter.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoEnvioNewsLetter.IdentidadID") + ", DocumentoEnvioNewsLetter.Fecha, DocumentoEnvioNewsLetter.Idioma, DocumentoEnvioNewsLetter.EnvioSolicitado, DocumentoEnvioNewsLetter.EnvioRealizado, DocumentoEnvioNewsLetter.Grupos FROM DocumentoEnvioNewsLetter";

            this.sqlSelectColaDocumento = "SELECT ColaDocumento.ID, " + pIBD.CargarGuid("ColaDocumento.DocumentoID") + ", ColaDocumento.AccionRealizada, ColaDocumento.Estado, ColaDocumento.FechaEncolado, ColaDocumento.FechaProcesado, ColaDocumento.Prioridad, ColaDocumento.InfoExtra, ColaDocumento.EstadoCargaID FROM ColaDocumento";

            this.sqlSelectDocumentoVincDoc = "SELECT " + pIBD.CargarGuid("DocumentoVincDoc.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoVincDoc.DocumentoVincID") + ", " + pIBD.CargarGuid("DocumentoVincDoc.IdentidadID") + ", DocumentoVincDoc.Fecha FROM DocumentoVincDoc";

            this.sqlSelectDocumentoRespuesta = "SELECT " + pIBD.CargarGuid("DocumentoRespuesta.RespuestaID") + ", " + pIBD.CargarGuid("DocumentoRespuesta.DocumentoID") + ", DocumentoRespuesta.Descripcion, DocumentoRespuesta.NumVotos, DocumentoRespuesta.Orden FROM DocumentoRespuesta";
            this.sqlSelectDocumentoRespuestaVoto = "SELECT " + pIBD.CargarGuid("DocumentoRespuestaVoto.RespuestaID") + ", " + pIBD.CargarGuid("DocumentoRespuestaVoto.DocumentoID") + ", " + pIBD.CargarGuid("DocumentoRespuestaVoto.IdentidadID") + " FROM DocumentoRespuestaVoto";

            this.sqlSelectDocumentoTokenBrightcove = "SELECT " + pIBD.CargarGuid("DocumentoTokenBrightcove.TokenID") + ", " + pIBD.CargarGuid("DocumentoTokenBrightcove.ProyectoID") + ", " + pIBD.CargarGuid("DocumentoTokenBrightcove.OrganizacionID") + ", " + pIBD.CargarGuid("DocumentoTokenBrightcove.UsuarioID") + ", " + pIBD.CargarGuid("DocumentoTokenBrightcove.DocumentoID") + ", DocumentoTokenBrightcove.FechaCreacion, DocumentoTokenBrightcove.Estado, DocumentoTokenBrightcove.NombreArchivo FROM DocumentoTokenBrightcove ";

            this.sqlSelectDocumentoTokenTOP = "SELECT " + pIBD.CargarGuid("DocumentoTokenTOP.TokenID") + ", " + pIBD.CargarGuid("DocumentoTokenTOP.ProyectoID") + ", " + pIBD.CargarGuid("DocumentoTokenTOP.OrganizacionID") + ", " + pIBD.CargarGuid("DocumentoTokenTOP.UsuarioID") + ", " + pIBD.CargarGuid("DocumentoTokenTOP.DocumentoID") + ", DocumentoTokenTOP.FechaCreacion, DocumentoTokenTOP.Estado, DocumentoTokenTOP.NombreArchivo FROM DocumentoTokenTOP ";

            this.sqlSelectDocumentoNewsletter = "SELECT " + pIBD.CargarGuid("DocumentoNewsletter.DocumentoID") + ", DocumentoNewsletter.Newsletter, DocumentoNewsletter.NewsletterTemporal FROM DocumentoNewsletter";

            this.sqlSelectColaCargaRecursos = "SELECT ColaCargaRecursos.ColaID, " + IBD.CargarGuid("ColaCargaRecursos.ID") + ", " + IBD.CargarGuid("ColaCargaRecursos.ProyectoID") + ", " + IBD.CargarGuid("ColaCargaRecursos.UsuarioID") + ", ColaCargaRecursos.Fecha, ColaCargaRecursos.Estado, ColaCargaRecursos.NombreFichImport FROM ColaCargaRecursos";

            #endregion

            #region Distinct

            this.sqlSelectDistinctDocumentoComentario = sqlSelectDocumentoComentario.Replace("SELECT", "SELECT DISTINCT");

            #endregion

            #region Consultas Generales

            #region Muy Generales

            this.sqlSelectTagsDocumentosDeOrganizacion = selectTagDocumento + " FROM Documento INNER JOIN TagDocumento ON Documento.DocumentoID = TagDocumento.DocumentoID WHERE (Documento.OrganizacionID  = " + pIBD.GuidParamValor("OrganizacionID") + ")";

            this.sqlSelectDocumentoRolIdentidadPorDocumentoID = sqlSelectDocumentoRolIdentidad + " WHERE DocumentoID = " + pIBD.GuidParamValor("documentoID");

            this.sqlSelectDocumentoRolGrupoIdentidadesPorDocumentoID = sqlSelectDocumentoRolGrupoIdentidades + " WHERE DocumentoID = " + pIBD.GuidParamValor("documentoID");

            this.sqlSelectTagsDocumentosDePersona = selectTagDocumento + " FROM Documento INNER JOIN TagDocumento ON Documento.DocumentoID = TagDocumento.DocumentoID WHERE  (Documento.creadorID = " + pIBD.GuidParamValor("creadorID") + ")";

            this.sqlSelectDocumentosDeOrganizacion = selectDocumento + " FROM Documento WHERE OrganizacionID = " + pIBD.GuidParamValor("OrganizacionID") + "";

            this.sqlSelectDocumentosDeEntidades = selectDocumento + " FROM Documento INNER JOIN DocumentoEntidadGnoss ON Documento.DocumentoID = DocumentoEntidadGnoss.DocumentoID "; // David: La otra parte del WHERE se concatena dentro de la funcion donde se usa

            this.sqlSelectTagDocumentosDeEntidades = selectTagDocumento + " FROM TagDocumento INNER JOIN Documento ON TagDocumento.DocumentoID=Documento.DocumentoID INNER JOIN DocumentoEntidadGnoss ON Documento.DocumentoID = DocumentoEntidadGnoss.DocumentoID"; // David: La otra parte del WHERE se concatena dentro de la funcion donde se usa

            this.sqlSelectDocumentoAtributoBiblioDeEntidades = selectDocumentoAtributoBiblio + " FROM DocumentoAtributoBiblio INNER JOIN Documento ON DocumentoAtributoBiblio.DocumentoID=Documento.DocumentoID INNER JOIN DocumentoEntidadGnoss ON DocumentoEntidadGnoss.DocumentoID = Documento.DocumentoID"; // David: La otra parte del WHERE se concatena dentro de la funcion donde se usa

            this.sqlSelectVersionDocumentoDeEntidades = selectVersionDocumento + " FROM VersionDocumento INNER JOIN Documento ON VersionDocumento.DocumentoID=Documento.DocumentoID INNER JOIN DocumentoEntidadGnoss ON DocumentoEntidadGnoss.DocumentoID = Documento.DocumentoID";

            this.sqlSelectDocumentoWebAgCatTesauroDeEntidades = selectDocumentoWebAgCatTesauro + " FROM DocumentoWebAgCatTesauro INNER JOIN Documento ON DocumentoWebAgCatTesauro.DocumentoID=Documento.DocumentoID INNER JOIN DocumentoEntidadGnoss ON DocumentoEntidadGnoss.DocumentoID = Documento.DocumentoID";

            this.sqlSelectDocumentoWebVinBaseRecursosDeEntidades = selectDocumentoWebVinBaseRecursos + " FROM DocumentoWebVinBaseRecursos INNER JOIN Documento ON DocumentoWebVinBaseRecursos.DocumentoID=Documento.DocumentoID INNER JOIN DocumentoEntidadGnoss ON DocumentoEntidadGnoss.DocumentoID = Documento.DocumentoID";


            this.sqlSelectDocumentoPorID = selectDocumento + " FROM Documento WHERE Documento.DocumentoID = " + pIBD.GuidParamValor("documentoID");


            this.sqlSelectDocumentoTokenBrightcovePorID = selectDocumentoTokenBrightcove + " FROM DocumentoTokenBrightcove WHERE DocumentoTokenBrightcove.DocumentoID = " + pIBD.GuidParamValor("DocumentoID");

            this.sqlSelectDocumentoTokenBrightcovePorTokenID = selectDocumentoTokenBrightcove + " FROM DocumentoTokenBrightcove WHERE DocumentoTokenBrightcove.TokenID = " + pIBD.GuidParamValor("TokenID");

            this.sqlSelectDocumentoTokenTOPPorID = selectDocumentoTokenTOP + " FROM DocumentoTokenTOP WHERE DocumentoTokenTOP.DocumentoID = " + pIBD.GuidParamValor("DocumentoID");

            this.sqlSelectDocumentoTokenTOPPorTokenID = selectDocumentoTokenTOP + " FROM DocumentoTokenTOP WHERE DocumentoTokenTOP.TokenID = " + pIBD.GuidParamValor("TokenID");

            this.sqlSelectDocumentoNewsletterPorDocumentoID = selectDocumentoNewsletter + " FROM DocumentoNewsletter WHERE DocumentoNewsletter.DocumentoID = " + pIBD.GuidParamValor("DocumentoID");

            this.sqlSelectDocumentoEnvioNewsletterPendienteEnvio = "SELECT DocumentoEnvioNewsLetter.DocumentoID, DocumentoEnvioNewsLetter.IdentidadID, DocumentoEnvioNewsLetter.Fecha, Documento.ProyectoID, Documento.Titulo, Documento.Descripcion, DocumentoEnvioNewsLetter.Idioma, DocumentoEnvioNewsLetter.EnvioSolicitado, DocumentoEnvioNewsLetter.EnvioRealizado, DocumentoEnvioNewsLetter.Grupos, null as Newsletter FROM DocumentoEnvioNewsLetter INNER JOIN Documento ON DocumentoEnvioNewsLetter.DocumentoID = Documento.DocumentoID WHERE DocumentoEnvioNewsLetter.EnvioSolicitado = 0 and DocumentoEnvioNewsLetter.EnvioRealizado = 0 and Documento.Descripcion != '' UNION SELECT DocumentoEnvioNewsLetter.DocumentoID, DocumentoEnvioNewsLetter.IdentidadID, DocumentoEnvioNewsLetter.Fecha, Documento.ProyectoID, Documento.Titulo, Documento.Descripcion, DocumentoEnvioNewsLetter.Idioma, DocumentoEnvioNewsLetter.EnvioSolicitado, DocumentoEnvioNewsLetter.EnvioRealizado, DocumentoEnvioNewsLetter.Grupos, DocumentoNewsLetter.Newsletter as Newsletter  FROM DocumentoEnvioNewsLetter INNER JOIN DocumentoNewsLetter ON DocumentoEnvioNewsLetter.DocumentoID = DocumentoNewsLetter.DocumentoID INNER JOIN Documento ON DocumentoEnvioNewsLetter.DocumentoID = Documento.DocumentoID where DocumentoEnvioNewsLetter.EnvioSolicitado = 0 and DocumentoEnvioNewsLetter.EnvioRealizado = 0 order by DocumentoEnvioNewsLetter.Fecha desc";

            /* David: Dejo la consulta original de super Altu por si acaso la mia peta
            this.sqlSelectDocumentoPorIDConNombrePerfil = selectDocumento + ", Perfil.NombrePerfil NombreCreador, Perfil.NombreOrganizacion NombreOrganizacion, Perfil.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil  FROM Documento, Identidad, Perfil WHERE Documento.DocumentoID = " + IBD.GuidParamValor("documentoID") + " AND Documento.CreadorID = Identidad.IdentidadID AND Identidad.PerfilID = Perfil.PerfilID";
             */
            this.sqlSelectDocumentoPorIDConNombrePerfil = selectDocumento + ", Perfil.NombrePerfil NombreCreador, Perfil.NombreOrganizacion NombreOrganizacion, Identidad.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN Identidad ON DocumentoWebVinBaseRecursos.IdentidadPublicacionID = Identidad.IdentidadID INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID WHERE Documento.DocumentoID = " + pIBD.GuidParamValor("documentoID") + " AND DocumentoWebVinBaseRecursos.TipoPublicacion = 0 ";

            this.sqlSelectTagsDocumentoPorID = selectTagDocumento + " FROM TagDocumento WHERE TagDocumento.DocumentoID = " + pIBD.GuidParamValor("documentoID");

            // Actualiza los nombres de las categorías de los documentos de entidades cuando este se cambia
            this.sqlActualizarNombreCategoriaDocumentalEntidadGnoss = pIBD.ReplaceParam("UPDATE Documento SET NombreCategoriaDoc = @NombreCategoria FROM Documento INNER JOIN DocumentoEntidadGnoss ON Documento.DocumentoID = DocumentoEntidadGnoss.DocumentoID AND Documento.OrganizacionID = DocumentoEntidadGnoss.OrganizacionID INNER JOIN CategoriaDocumentacion ON DocumentoEntidadGnoss.OrganizacionID = CategoriaDocumentacion.OrganizacionID AND DocumentoEntidadGnoss.ProyectoID = CategoriaDocumentacion.ProyectoID AND DocumentoEntidadGnoss.CategoriaDocumentacionID = CategoriaDocumentacion.CategoriaDocumentacionID WHERE (CategoriaDocumentacion.OrganizacionID = " + pIBD.GuidParamValor("OrganizacionID") + " AND CategoriaDocumentacion.CategoriaDocumentacionID = " + pIBD.GuidParamValor("CategoriaDocumentacionID") + " AND CategoriaDocumentacion.ProyectoID = " + pIBD.GuidParamValor("ProyectoID") + ")");

            // Actualiza los tags de documentos con el nombre de categoría de una entidad cuando este se cambia desde la configuración
            this.sqlActualizarTagNombreCategoriaDocumentacionEntidadGnoss = IBD.ReplaceParam("UPDATE Tag SET Nombre = @NombreTag FROM CategoriaDocumentacion INNER JOIN DocumentoEntidadGnoss ON CategoriaDocumentacion.OrganizacionID = DocumentoEntidadGnoss.OrganizacionID AND CategoriaDocumentacion.ProyectoID = DocumentoEntidadGnoss.ProyectoID AND CategoriaDocumentacion.CategoriaDocumentacionID = DocumentoEntidadGnoss.CategoriaDocumentacionID INNER JOIN Documento ON DocumentoEntidadGnoss.DocumentoID = Documento.DocumentoID AND DocumentoEntidadGnoss.OrganizacionID = Documento.OrganizacionID INNER JOIN TagDocumento ON Documento.DocumentoID = TagDocumento.DocumentoID INNER JOIN Tag ON TagDocumento.TagID = Tag.TagID WHERE (CategoriaDocumentacion.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND CategoriaDocumentacion.CategoriaDocumentacionID = " + IBD.GuidParamValor("CategoriaDocumentacionID") + " AND CategoriaDocumentacion.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND TagDocumento.Tipo=@TipoTag)");

            this.sqlSelectTagDocumentoCurriculum = IBD.ReplaceParam("SELECT TagDocumento.TagID,TagDocumento.Tipo, " + IBD.CargarGuid("TagDocumento.DocumentoID") + " FROM DocumentoCurriculum INNER JOIN Documento ON DocumentoCurriculum.DocumentoID = Documento.DocumentoID INNER JOIN TagDocumento ON Documento.DocumentoID = TagDocumento.DocumentoID WHERE (TagDocumento.Tipo= @TipoTag AND DocumentoCurriculum.PersonaID= " + IBD.GuidParamValor("PersonaID") + ")");//TODO: JAVIER -> Cambiar por acreditación

            this.sqlSelectTagDocumentoCurriculum = pIBD.ReplaceParam("SELECT TagDocumento.TagID,TagDocumento.Tipo, " + pIBD.CargarGuid("TagDocumento.DocumentoID") + " FROM DocumentoCurriculum INNER JOIN Documento ON DocumentoCurriculum.DocumentoID = Documento.DocumentoID INNER JOIN TagDocumento ON Documento.DocumentoID = TagDocumento.DocumentoID WHERE (TagDocumento.Tipo= @TipoTag AND DocumentoCurriculum.PersonaID= " + pIBD.GuidParamValor("PersonaID") + ")");//TODO: JAVIER -> Cambiar por acreditación

            this.sqlDeleteTagDocumentoCurriculum = pIBD.ReplaceParam("DELETE FROM TagDocumento FROM DocumentoCurriculum INNER JOIN Documento ON DocumentoCurriculum.DocumentoID = Documento.DocumentoID INNER JOIN TagDocumento ON Documento.DocumentoID = TagDocumento.DocumentoID WHERE (TagDocumento.Tipo= @TipoTag AND DocumentoCurriculum.PersonaID= " + pIBD.GuidParamValor("PersonaID") + ")");//TODO: JAVIER -> Cambiar por acreditación

            this.sqlInsertTagDocumentoCurriculum = IBD.ReplaceParam("INSERT INTO TagDocumento(TagID ,Tipo,DocumentoID) VALUES (@TagID,@TipoTag," + IBD.GuidParamValor("DocumentoID") + ")");//TODO: JAVIER -> Cambiar por acreditación

            this.sqlUpdateDocumentoNombreEntidadCurriculum = pIBD.ReplaceParam("UPDATE Documento SET NombreElementoVinculado=@NombreElementoVinculado WHERE (DocumentoID= " + pIBD.GuidParamValor("DocumentoID") + " AND OrganizacionID =" + pIBD.GuidParamValor("OrganizacionID") + ")");//TODO: JAVIER -> Cambiar por acreditación

            this.sqlSelectDocumentosSuscripcion = selectDocumento + " FROM Documento";

            this.sqlSelectTagsDocumentosSuscripcion = selectTagDocumento + " FROM Documento INNER JOIN TagDocumento ON Documento.DocumentoID = TagDocumento.DocumentoID";

            this.sqlSelectDocumentosWebDePersonaCompartidos = selectDocumento + " FROM (SELECT distinct Documento.DocumentoID FROM Documento, DocumentoWebVinBaseRecursos, (SELECT distinct Documento.DocumentoID FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID  INNER JOIN DocumentoWebVinBaseRecursos ON DocumentoWebVinBaseRecursos.DocumentoID = Documento.DocumentoID AND DocumentoWebVinBaseRecursos.BaseRecursosID = DocumentoWebAgCatTesauro.BaseRecursosID WHERE DocumentoWebVinBaseRecursos.IdentidadPublicacionID = " + pIBD.GuidParamValor("identidadPublicacionID") + " AND DocumentoWebVinBaseRecursos.BaseRecursosID = " + pIBD.GuidParamValor("baseRecursosID") + " AND DocumentoWebVinBaseRecursos.TipoPublicacion > 0 AND Documento.Tipo != " + ((short)TiposDocumentacion.ImagenWiki).ToString() + ") DocTemporal WHERE Documento.DocumentoID = DocTemporal.DocumentoID AND Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID) DocTemp INNER JOIN Documento ON (DocTemp.DocumentoID = Documento.DocumentoID) ";//, DocumentoWebVinBaseRecursos.FechaPublicacion;INNER JOIN DocumentoWebVinBaseRecursos ON (DocumentoWebVinBaseRecursos.DocumentoID = Documento.DocumentoID)


            //NUEVAS:

            this.sqlRepeticionTituloDocumento = pIBD.ReplaceParam("SELECT " + pIBD.CargarGuid("DocumentoID") + " FROM Documento WHERE Titulo=@Titulo AND Documento.Eliminado = 0 AND Documento.UltimaVersion = 1");

            this.sqlRepeticionEnlaceDocumento = pIBD.ReplaceParam("SELECT " + pIBD.CargarGuid("DocumentoID") + " FROM Documento WHERE Enlace=@Enlace AND Documento.Eliminado = 0 AND Documento.UltimaVersion = 1");

            this.sqlRepeticionTituloDocumentoDeBR = pIBD.ReplaceParam("SELECT " + pIBD.CargarGuid("Documento.DocumentoID") + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON (Documento.DocumentoID=DocumentoWebVinBaseRecursos.DocumentoID) WHERE Titulo=@Titulo AND Documento.Eliminado = 0 AND Documento.UltimaVersion = 1 AND Documento.TipoEntidad=" + (short)TipoEntidadVinculadaDocumento.Web + " AND DocumentoWebVinBaseRecursos.BaseRecursosID=" + pIBD.GuidParamValor("BaseRecursosID") + " AND DocumentoWebVinBaseRecursos.Eliminado=0 AND DocumentoWebVinBaseRecursos.PrivadoEditores = 0 ");
            this.sqlRepeticionEnlaceDocumentoDeBR = pIBD.ReplaceParam("SELECT " + pIBD.CargarGuid("Documento.DocumentoID") + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON (Documento.DocumentoID=DocumentoWebVinBaseRecursos.DocumentoID) WHERE Enlace=@Enlace AND Documento.Eliminado = 0 AND Documento.UltimaVersion = 1 AND Documento.TipoEntidad=" + (short)TipoEntidadVinculadaDocumento.Web + " AND DocumentoWebVinBaseRecursos.BaseRecursosID=" + pIBD.GuidParamValor("BaseRecursosID") + " AND DocumentoWebVinBaseRecursos.Eliminado=0 AND DocumentoWebVinBaseRecursos.PrivadoEditores = 0 ");

            this.sqlRepeticionTituloDocumentoDeVariasBRs = "SELECT " + pIBD.CargarGuid("Documento.DocumentoID") + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON (Documento.DocumentoID=DocumentoWebVinBaseRecursos.DocumentoID) WHERE Titulo=@Titulo AND Documento.Eliminado = 0 AND Documento.UltimaVersion = 1 AND Documento.TipoEntidad=" + (short)TipoEntidadVinculadaDocumento.Web + " AND DocumentoWebVinBaseRecursos.Eliminado=0 ";
            this.sqlRepeticionEnlaceDocumentoDeVariasBRs = "SELECT " + pIBD.CargarGuid("Documento.DocumentoID") + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON (Documento.DocumentoID=DocumentoWebVinBaseRecursos.DocumentoID) WHERE Enlace=@Enlace AND Documento.Eliminado = 0 AND Documento.UltimaVersion = 1 AND Documento.TipoEntidad=" + (short)TipoEntidadVinculadaDocumento.Web + " AND DocumentoWebVinBaseRecursos.Eliminado=0 ";

            this.sqlSelectHistorialDocumentoPorDocumentoID = selectHistorialDocumento + " FROM HistorialDocumento Where DocumentoID=" + pIBD.GuidParamValor("DocumentoID") + "";

            this.sqlSelectOntologiasProyecto = selectDocumento + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON (DocumentoWebVinBaseRecursos.DocumentoID=Documento.DocumentoID) INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID=BaseRecursosProyecto.BaseRecursosID) WHERE Tipo = " + ((short)TiposDocumentacion.Ontologia).ToString() + " AND BaseRecursosProyecto.ProyectoID =" + pIBD.GuidParamValor("ProyectoID") + " AND Documento.Eliminado=0 AND DocumentoWebVinBaseRecursos.Eliminado=0 ";

            this.sqlSelectOntologiasPrincYSecundariasProyecto = selectDocumento + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON DocumentoWebVinBaseRecursos.DocumentoID = Documento.DocumentoID INNER JOIN BaseRecursosProyecto ON DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID WHERE (Tipo = " + (short)TiposDocumentacion.Ontologia + " OR Tipo = " + (short)TiposDocumentacion.OntologiaSecundaria + ") AND BaseRecursosProyecto.ProyectoID =" + pIBD.GuidParamValor("ProyectoID") + " AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 ";

            this.sqlSelectTieneArticulosWikiProyecto = "SELECT " + pIBD.CargarGuid("Documento.DocumentoID") + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON (Documento.DocumentoID=DocumentoWebVinBaseRecursos.DocumentoID) INNER JOIN BaseRecursosProyecto ON BaseRecursosProyecto.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID WHERE Documento.Eliminado = 0 AND Documento.UltimaVersion = 1 AND Documento.TipoEntidad=" + (short)TipoEntidadVinculadaDocumento.Web + " AND DocumentoWebVinBaseRecursos.Eliminado=0 AND Documento.Tipo = " + (short)TiposDocumentacion.Wiki + "AND BaseRecursosProyecto.ProyectoID =" + pIBD.GuidParamValor("proyectoID") + "";

            #endregion

            #region Documentos Web

            this.sqlSelectDocumentoWebAgCatTesauroDeBR = selectDocumentoWebAgCatTesauro + " FROM DocumentoWebAgCatTesauro WHERE BaseRecursosID = " + pIBD.GuidParamValor("baseRecursosID") + "";

            this.sqlSelectDocumentoWebAgCatTesauroDeBRDeDoc = selectDocumentoWebAgCatTesauro + " FROM DocumentoWebAgCatTesauro WHERE BaseRecursosID = " + pIBD.GuidParamValor("baseRecursosID") + " AND DocumentoID=" + pIBD.GuidParamValor("documentoID");

            this.sqlComunidadesDePersonaPorDocumentoWEB = " from DocumentoWebVinBaseRecursos INNER JOIN BaseRecursos ON BaseRecursos.BaseRecursosID=DocumentoWebVinBaseRecursos.BaseRecursosID INNER JOIN BaseRecursosproyecto on BaseRecursosproyecto.BaseRecursosid = DocumentoWebVinBaseRecursos.BaseRecursosid where DocumentoWebVinBaseRecursos.documentoid = " + pIBD.GuidParamValor("documentoID");

            this.sqlComunidadesDePersonaPorDocumentoBRWEB = " from DocumentoWebVinBaseRecursos INNER JOIN BaseRecursos ON BaseRecursos.BaseRecursosID=DocumentoWebVinBaseRecursos.BaseRecursosID where DocumentoWebVinBaseRecursos.documentoid = " + pIBD.GuidParamValor("documentoID");

            this.sqlComunidadesDePersonaPorDocumentoBRUsuarioWEB = " from DocumentoWebVinBaseRecursos INNER JOIN BaseRecursosUsuario on BaseRecursosUsuario.BaseRecursosid = DocumentoWebVinBaseRecursos.BaseRecursosid where DocumentoWebVinBaseRecursos.documentoid = " + pIBD.GuidParamValor("documentoID");

            this.sqlComunidadesDePersonaPorDocumentoBROrganizacionWEB = " from DocumentoWebVinBaseRecursos INNER JOIN BaseRecursosOrganizacion on BaseRecursosOrganizacion.BaseRecursosid = DocumentoWebVinBaseRecursos.BaseRecursosid where DocumentoWebVinBaseRecursos.documentoid = " + pIBD.GuidParamValor("documentoID");

            this.sqlSelectDocumentosVinculadosAProyecto = selectDocumento + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON BaseRecursosProyecto.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID WHERE BaseRecursosProyecto.ProyectoID = " + pIBD.GuidParamValor("proyectoID");

            this.sqlSelectDocumentosWebVinDeProyecto = selectDocumentoWebVinBaseRecursos + " FROM DocumentoWebVinBaseRecursos INNER JOIN BaseRecursosProyecto ON BaseRecursosProyecto.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID WHERE BaseRecursosProyecto.ProyectoID = " + pIBD.GuidParamValor("proyectoID");

            this.sqlSelectNivelCertificacion = " select NivelCertificacion.Orden FROM       DocumentoWebVinBaseRecursos inner join NivelCertificacion on NivelCertificacion.NivelCertificacionID= DocumentoWebVinBaseRecursos.NivelCertificacionID where DocumentoWebVinBaseRecursos.DocumentoID=" + pIBD.GuidParamValor("documentoID");

            this.sqlSelectNivelesCertificacionProyecto = " select case when NivelCertificacion.Orden is null then 100 else NivelCertificacion.Orden end As Orden,DocumentoWebVinBaseRecursos.DocumentoID  FROM DocumentoWebVinBaseRecursos INNER JOIN BaseRecursosProyecto ON BaseRecursosProyecto.BaseRecursosID=DocumentoWebVinBaseRecursos.BaseRecursosID left join NivelCertificacion on NivelCertificacion.NivelCertificacionID= DocumentoWebVinBaseRecursos.NivelCertificacionID where BaseRecursosProyecto.ProyectoID=" + pIBD.GuidParamValor("proyectoID");

            this.sqlSelectDocumentosBaseRecursosUsuario = selectDocumento + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON BaseRecursosUsuario.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID WHERE BaseRecursosUsuario.UsuarioID = " + pIBD.GuidParamValor("usuarioID");

            this.sqlSelectDocumentosBaseRecursosOrganizacion = selectDocumento + " FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosOrganizacion ON BaseRecursosOrganizacion.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID WHERE BaseRecursosOrganizacion.OrganizacionID = " + pIBD.GuidParamValor("organizacionID");

            this.sqlActualizarNumerosDocumentacion = pIBD.ReplaceParam("UPDATE DocumentoWebVinBaseRecursosExtra SET NumeroConsultas = NumeroConsultas + 1, NumeroDescargas = NumeroDescargas + 1  WHERE DocumentoWebVinBaseRecursosExtra.DocumentoID = " + pIBD.GuidParamValor("DocumentoID") + " AND DocumentoWebVinBaseRecursosExtra.BaseRecursosID = " + pIBD.GuidParamValor("BaseRecursosID"));

            this.sqlActualizarContadoresTotalesDocumento = pIBD.ReplaceParam("UPDATE Documento SET NumeroTotalConsultas = NumeroTotalConsultas + 1, NumeroTotalDescargas = NumeroTotalDescargas + 1  WHERE Documento.DocumentoID = " + pIBD.GuidParamValor("DocumentoID"));

            this.sqlActualizarUltimaVersionDocumento = pIBD.ReplaceParam("UPDATE Documento SET UltimaVersion = 0, FechaModificacion = " + pIBD.CapturarFecha() + " WHERE Documento.DocumentoID = " + pIBD.GuidParamValor("DocumentoID") + " ");

            #endregion

            #region Base de Recursos

            this.sqlSelectBaseRecursosDEProyecto = selectBaseRecursosIDProyecto + " FROM BaseRecursosProyecto WHERE BaseRecursosProyecto.ProyectoID=" + pIBD.GuidParamValor("ProyectoID") + " AND BaseRecursosProyecto.OrganizacionID=" + pIBD.GuidParamValor("OrganizacionID") + " ";
            this.sqlSelectBaseRecursosProyectoDEProyecto = selectBaseRecursosProyecto + " FROM BaseRecursosProyecto WHERE BaseRecursosProyecto.ProyectoID=" + pIBD.GuidParamValor("ProyectoID") + " AND BaseRecursosProyecto.OrganizacionID=" + pIBD.GuidParamValor("OrganizacionID") + " ";

            this.sqlSelectBaseRecursosDEUsuario = selectBaseRecursos + " FROM BaseRecursos INNER JOIN BaseRecursosUsuario ON (BaseRecursos.BaseRecursosID=BaseRecursosUsuario.BaseRecursosID) WHERE BaseRecursosUsuario.UsuarioID=" + pIBD.GuidParamValor("UsuarioID") + " ";
            this.sqlSelectBaseRecursosUsuarioDEUsuario = selectBaseRecursosUsuario + " FROM BaseRecursosUsuario WHERE BaseRecursosUsuario.UsuarioID=" + pIBD.GuidParamValor("UsuarioID") + " ";

            this.sqlSelectBaseRecursosDEOrganizacion = selectBaseRecursos + " FROM BaseRecursos INNER JOIN BaseRecursosOrganizacion ON (BaseRecursos.BaseRecursosID=BaseRecursosOrganizacion.BaseRecursosID) WHERE BaseRecursosOrganizacion.OrganizacionID=" + pIBD.GuidParamValor("OrganizacionID") + " ";
            this.sqlSelectBaseRecursosOrganizacionDEOrganizacion = selectBaseRecursosOrganizacion + " FROM BaseRecursosOrganizacion WHERE BaseRecursosOrganizacion.OrganizacionID=" + pIBD.GuidParamValor("OrganizacionID") + " ";

            this.selectDistinctBaseRecursosProyecto = "SELECT Distinct " + pIBD.CargarGuid("BaseRecursosProyecto.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosProyecto.OrganizacionID") + ", " + pIBD.CargarGuid("BaseRecursosProyecto.ProyectoID") + " ";

            this.selectDistinctBaseRecursosUsuario = "SELECT Distinct " + pIBD.CargarGuid("BaseRecursosUsuario.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosUsuario.UsuarioID") + ", BaseRecursosUsuario.EspacioMaxMyGnossMB, BaseRecursosUsuario.EspacioActualMyGnossMB ";

            this.selectDistinctBaseRecursosOrganizacion = "SELECT Distinct " + pIBD.CargarGuid("BaseRecursosOrganizacion.BaseRecursosID") + ", " + pIBD.CargarGuid("BaseRecursosOrganizacion.OrganizacionID") + ", BaseRecursosOrganizacion.EspacioMaxMyGnossMB, BaseRecursosOrganizacion.EspacioActualMyGnossMB ";

            this.sqlSelectDocumentoWebVinBaseRecursosDEOrganizacion = selectDocumentoWebVinBaseRecursos + " FROM DocumentoWebVinBaseRecursos INNER JOIN BaseRecursosOrganizacion ON BaseRecursosOrganizacion.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID WHERE BaseRecursosOrganizacion.OrganizacionID=" + pIBD.GuidParamValor("OrganizacionID");

            this.sqlSelectDocumentoWebAgCatTesauroDEOrganizacion = selectDocumentoWebAgCatTesauro + " FROM DocumentoWebAgCatTesauro INNER JOIN DocumentoWebVinBaseRecursos ON DocumentoWebVinBaseRecursos.BaseRecursosID = DocumentoWebAgCatTesauro.BaseRecursosID AND DocumentoWebVinBaseRecursos.DocumentoID = DocumentoWebAgCatTesauro.DocumentoID INNER JOIN BaseRecursosOrganizacion ON BaseRecursosOrganizacion.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID WHERE BaseRecursosOrganizacion.OrganizacionID=" + pIBD.GuidParamValor("OrganizacionID");

            #endregion

            #region VersionDocumento

            this.sqlSelectVersionDocPorOriginalDoc = selectVersionDocumento + " FROM VersionDocumento WHERE DocumentoOriginalID=" + pIBD.GuidParamValor("DocumentoOriginalID");

            #endregion

            #region Comentarios y Votos

            sqlSelectTodosComentariosDocumento = sqlSelectDocumentoComentario + " INNER JOIN Comentario ON (Comentario.ComentarioID = DocumentoComentario.ComentarioID) WHERE Comentario.Eliminado = 0 AND DocumentoID = " + pIBD.GuidParamValor("DocumentoID");

            sqlSelectTodosVotosDocumento = sqlSelectVotoDocumento + " WHERE DocumentoID = " + pIBD.GuidParamValor("DocumentoID");

            #endregion

            #region Documentos Temporales

            sqlSelectDocumentosTemporales = selectDistinctDocumento + " FROM Documento WHERE Documento.Eliminado=0 AND Documento.TipoEntidad=" + (short)TipoEntidadVinculadaDocumento.Temporal + " AND Documento.ElementoVinculadoID=" + pIBD.GuidParamValor("DocumentoOriginalID") + " AND Documento.CreadorID=" + pIBD.GuidParamValor("IdentidadID") + " ";

            sqlSelectDocumentosTemporalesEspecificandoTipo = sqlSelectDocumentosTemporales + " AND Documento.Tipo=@Tipo ";

            sqlSelectDocumentosTemporalesPorSoloCreador = selectDistinctDocumento + " FROM Documento WHERE Documento.Eliminado=0 AND Documento.TipoEntidad=" + (short)TipoEntidadVinculadaDocumento.Temporal + " AND Documento.CreadorID=" + pIBD.GuidParamValor("IdentidadID") + " ";

            sqlSelectDocumentosTemporalesEspecificandoTipoPorSoloCreador = sqlSelectDocumentosTemporalesPorSoloCreador + " AND Documento.Tipo=@Tipo ";

            sqlSelectDocumentosTemporalesPorNombre = selectDistinctDocumento + " FROM Documento WHERE Documento.Eliminado=0 AND Documento.TipoEntidad=" + (short)TipoEntidadVinculadaDocumento.Temporal + " AND Documento.Titulo=@NombreDocOriginal AND Documento.CreadorID=" + pIBD.GuidParamValor("IdentidadID") + " ";

            sqlSelectDocumentosTemporalesEspecificandoTipoPorNombre = sqlSelectDocumentosTemporalesPorNombre + " AND Documento.Tipo=@Tipo ";

            #endregion

            #region Documento Newsletter

            sqlSelectDocumentoEnvioNewsLetterPorDocID = sqlSelectDocumentoEnvioNewsLetter + " WHERE DocumentoID=" + pIBD.ToParam("DocumentoID");

            #endregion

            #region Cola de documentos


            this.sqlInsertEnColaDocumento = pIBD.ReplaceParam("INSERT INTO ColaDocumento (DocumentoID, AccionRealizada, Estado, FechaEncolado,Prioridad,EstadoCargaID) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", @AccionRealizada, @Estado, @FechaEncolado,@Prioridad,@EstadoCargaID)");

            #endregion

            #endregion

            #endregion

            #region DataAdapter

            //CUIDADO con los campos 'NumeroComentariosPublicos', 'NumeroTotalDescargas', 'NumeroTotalConsultas', 'NumeroTotalVotos' y 'FechaModificacion' que no estén en el original (concurrencia)

            #region Documento (TOCAR CON CUIDADO)

            this.sqlDocumentoInsert = pIBD.ReplaceParam("INSERT INTO Documento (DocumentoID, OrganizacionID, CompartirPermitido, ElementoVinculadoID, Titulo, Descripcion, Tipo, Enlace, FechaCreacion, CreadorID, TipoEntidad, NombreCategoriaDoc, NombreElementoVinculado, ProyectoID, Publico, Borrador, FichaBibliograficaID, CreadorEsAutor, Valoracion, Autor, FechaModificacion, IdentidadProteccionID, FechaProteccion, UltimaVersion, Eliminado, Protegido, NumeroComentariosPublicos, NumeroTotalVotos, NumeroTotalConsultas, NumeroTotalDescargas, VersionFotoDocumento, Rank, Rank_Tiempo, Licencia, Tags, Visibilidad) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", @CompartirPermitido, " + pIBD.GuidParamColumnaTabla("ElementoVinculadoID") + ", @Titulo, @Descripcion, @Tipo, @Enlace, @FechaCreacion, " + pIBD.GuidParamColumnaTabla("CreadorID") + ", @TipoEntidad, @NombreCategoriaDoc, @NombreElementoVinculado, " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", @Publico, @Borrador, " + pIBD.GuidParamColumnaTabla("FichaBibliograficaID") + ", @CreadorEsAutor, @Valoracion, @Autor, @FechaModificacion, " + pIBD.GuidParamColumnaTabla("IdentidadProteccionID") + ", @FechaProteccion, @UltimaVersion, @Eliminado, @Protegido, @NumeroComentariosPublicos, @NumeroTotalVotos, @NumeroTotalConsultas, @NumeroTotalDescargas, @VersionFotoDocumento, @Rank, @Rank_Tiempo, @Licencia, @Tags, @Visibilidad)");

            this.sqlDocumentoDelete = pIBD.ReplaceParam("DELETE FROM Documento WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ")");

            this.sqlDocumentoModify = pIBD.ReplaceParam("UPDATE Documento SET OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", CompartirPermitido = @CompartirPermitido, ElementoVinculadoID = " + pIBD.GuidParamColumnaTabla("ElementoVinculadoID") + ", Titulo = @Titulo, Descripcion = @Descripcion, Tipo = @Tipo, Enlace = @Enlace, FechaCreacion = @FechaCreacion, CreadorID = " + pIBD.GuidParamColumnaTabla("CreadorID") + ", TipoEntidad = @TipoEntidad, NombreCategoriaDoc = @NombreCategoriaDoc, NombreElementoVinculado = @NombreElementoVinculado, ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", Publico = @Publico, Borrador = @Borrador, FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("FichaBibliograficaID") + ", CreadorEsAutor = @CreadorEsAutor, Valoracion = @Valoracion, Autor = @Autor, FechaModificacion = @FechaModificacion, IdentidadProteccionID = " + pIBD.GuidParamColumnaTabla("IdentidadProteccionID") + ", FechaProteccion = @FechaProteccion, UltimaVersion = @UltimaVersion, Eliminado = @Eliminado, Protegido = @Protegido, VersionFotoDocumento = @VersionFotoDocumento,NumeroTotalVotos=@NumeroTotalVotos, Rank = @Rank, Rank_Tiempo = @Rank_Tiempo, Licencia = @Licencia, Tags = @Tags, Visibilidad = @Visibilidad WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ")");

            #endregion

            #region BaseRecursos
            this.sqlBaseRecursosInsert = pIBD.ReplaceParam("INSERT INTO BaseRecursos (BaseRecursosID) VALUES (" + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ")");
            this.sqlBaseRecursosDelete = pIBD.ReplaceParam("DELETE FROM BaseRecursos WHERE (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ")");
            this.sqlBaseRecursosModify = pIBD.ReplaceParam("UPDATE BaseRecursos SET BaseRecursosID = " + pIBD.GuidParamColumnaTabla("BaseRecursosID") + " WHERE (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ")");
            #endregion

            #region Tipologia
            this.sqlTipologiaInsert = pIBD.ReplaceParam("INSERT INTO Tipologia (TipologiaID, AtributoID, OrganizacionID, ProyectoID, Nombre, Descripcion, Tipo, Orden) VALUES (" + pIBD.GuidParamColumnaTabla("TipologiaID") + ", " + pIBD.GuidParamColumnaTabla("AtributoID") + ", " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", @Nombre, @Descripcion, @Tipo, @Orden)");
            this.sqlTipologiaDelete = pIBD.ReplaceParam("DELETE FROM Tipologia WHERE (TipologiaID = " + pIBD.GuidParamColumnaTabla("O_TipologiaID") + ") AND (AtributoID = " + pIBD.GuidParamColumnaTabla("O_AtributoID") + ")");
            this.sqlTipologiaModify = pIBD.ReplaceParam("UPDATE Tipologia SET TipologiaID = " + pIBD.GuidParamColumnaTabla("TipologiaID") + ", AtributoID = " + pIBD.GuidParamColumnaTabla("AtributoID") + ", OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", Nombre = @Nombre, Descripcion = @Descripcion, Tipo = @Tipo, Orden = @Orden WHERE (TipologiaID = " + pIBD.GuidParamColumnaTabla("O_TipologiaID") + ") AND (AtributoID = " + pIBD.GuidParamColumnaTabla("O_AtributoID") + ")");
            #endregion

            #region DocumentoTipologia
            this.sqlDocumentoTipologiaInsert = pIBD.ReplaceParam("INSERT INTO DocumentoTipologia (DocumentoID, TipologiaID, AtributoID, Valor) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("TipologiaID") + ", " + pIBD.GuidParamColumnaTabla("AtributoID") + ", @Valor)");
            this.sqlDocumentoTipologiaDelete = pIBD.ReplaceParam("DELETE FROM DocumentoTipologia WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND (TipologiaID = " + pIBD.GuidParamColumnaTabla("O_TipologiaID") + ") AND (AtributoID = " + pIBD.GuidParamColumnaTabla("O_AtributoID") + ")");
            this.sqlDocumentoTipologiaModify = pIBD.ReplaceParam("UPDATE DocumentoTipologia SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", TipologiaID = " + pIBD.GuidParamColumnaTabla("TipologiaID") + ", AtributoID = " + pIBD.GuidParamColumnaTabla("AtributoID") + ", Valor = @Valor WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND (TipologiaID = " + pIBD.GuidParamColumnaTabla("O_TipologiaID") + ") AND (AtributoID = " + pIBD.GuidParamColumnaTabla("O_AtributoID") + ")");
            #endregion

            #region DocumentoRolIdentidad
            this.sqlDocumentoRolIdentidadInsert = pIBD.ReplaceParam("INSERT INTO DocumentoRolIdentidad (DocumentoID, PerfilID, Editor) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("PerfilID") + ", @Editor)");
            this.sqlDocumentoRolIdentidadDelete = pIBD.ReplaceParam("DELETE FROM DocumentoRolIdentidad WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (PerfilID = " + pIBD.GuidParamColumnaTabla("Original_PerfilID") + ") AND (Editor = @Original_Editor)");
            this.sqlDocumentoRolIdentidadModify = pIBD.ReplaceParam("UPDATE DocumentoRolIdentidad SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", PerfilID = " + pIBD.GuidParamColumnaTabla("PerfilID") + ", Editor = @Editor WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (PerfilID = " + pIBD.GuidParamColumnaTabla("Original_PerfilID") + ") AND (Editor = @Original_Editor)");
            #endregion

            #region DocumentoRolGrupoIdentidades
            this.sqlDocumentoRolGrupoIdentidadesInsert = pIBD.ReplaceParam("INSERT INTO DocumentoRolGrupoIdentidades (DocumentoID, GrupoID, Editor) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("GrupoID") + ", @Editor)");
            this.sqlDocumentoRolGrupoIdentidadesDelete = pIBD.ReplaceParam("DELETE FROM DocumentoRolGrupoIdentidades WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (GrupoID = " + pIBD.GuidParamColumnaTabla("Original_GrupoID") + ") AND (Editor = @Original_Editor)");
            this.sqlDocumentoRolGrupoIdentidadesModify = pIBD.ReplaceParam("UPDATE DocumentoRolGrupoIdentidades SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", GrupoID = " + pIBD.GuidParamColumnaTabla("GrupoID") + ", Editor = @Editor WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (GrupoID = " + pIBD.GuidParamColumnaTabla("Original_GrupoID") + ") AND (Editor = @Original_Editor)");
            #endregion

            #region DocumentoGrupoUsuario
            this.sqlDocumentoGrupoUsuarioInsert = pIBD.ReplaceParam("INSERT INTO DocumentoGrupoUsuario (DocumentoID, GrupoUsuarioID, Editor) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("GrupoUsuarioID") + ", @Editor)");
            this.sqlDocumentoGrupoUsuarioDelete = pIBD.ReplaceParam("DELETE FROM DocumentoGrupoUsuario WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND (GrupoUsuarioID = " + pIBD.GuidParamColumnaTabla("O_GrupoUsuarioID") + ")");
            this.sqlDocumentoGrupoUsuarioModify = pIBD.ReplaceParam("UPDATE DocumentoGrupoUsuario SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", GrupoUsuarioID = " + pIBD.GuidParamColumnaTabla("GrupoUsuarioID") + ", Editor = @Editor WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND (GrupoUsuarioID = " + pIBD.GuidParamColumnaTabla("O_GrupoUsuarioID") + ")");
            #endregion

            #region HistorialDocumento
            this.sqlHistorialDocumentoInsert = pIBD.ReplaceParam("INSERT INTO HistorialDocumento (HistorialDocumentoID, DocumentoID, IdentidadID, Fecha, TagNombre, CategoriaTesauroID, Accion, ProyectoID) VALUES (" + pIBD.GuidParamColumnaTabla("HistorialDocumentoID") + ", " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("IdentidadID") + ", @Fecha, @TagNombre, " + pIBD.GuidParamColumnaTabla("CategoriaTesauroID") + ", @Accion, " + pIBD.GuidParamColumnaTabla("ProyectoID") + ")");
            this.sqlHistorialDocumentoDelete = pIBD.ReplaceParam("DELETE FROM HistorialDocumento WHERE (HistorialDocumentoID = " + pIBD.GuidParamColumnaTabla("O_HistorialDocumentoID") + ")");
            this.sqlHistorialDocumentoModify = pIBD.ReplaceParam("UPDATE HistorialDocumento SET HistorialDocumentoID = " + pIBD.GuidParamColumnaTabla("HistorialDocumentoID") + ", DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", IdentidadID = " + pIBD.GuidParamColumnaTabla("IdentidadID") + ", Fecha = @Fecha, TagNombre = @TagNombre, CategoriaTesauroID = " + pIBD.GuidParamColumnaTabla("CategoriaTesauroID") + ", Accion = @Accion, ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (HistorialDocumentoID = " + pIBD.GuidParamColumnaTabla("O_HistorialDocumentoID") + ")");
            #endregion

            #region VotoDocumento

            this.sqlVotoDocumentoInsert = pIBD.ReplaceParam("INSERT INTO VotoDocumento (DocumentoID, VotoID, ProyectoID) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("VotoID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ")");
            this.sqlVotoDocumentoDelete = pIBD.ReplaceParam("DELETE FROM VotoDocumento WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND (VotoID = " + pIBD.GuidParamColumnaTabla("O_VotoID") + ")");
            this.sqlVotoDocumentoModify = pIBD.ReplaceParam("UPDATE VotoDocumento SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", VotoID = " + pIBD.GuidParamColumnaTabla("VotoID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND (VotoID = " + pIBD.GuidParamColumnaTabla("O_VotoID") + ")");

            #endregion

            #region VersionDocumento
            this.sqlVersionDocumentoInsert = pIBD.ReplaceParam("INSERT INTO VersionDocumento (DocumentoID, Version, DocumentoOriginalID, IdentidadID) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", @Version, " + pIBD.GuidParamColumnaTabla("DocumentoOriginalID") + ", " + pIBD.GuidParamColumnaTabla("IdentidadID") + ")");
            this.sqlVersionDocumentoDelete = pIBD.ReplaceParam("DELETE FROM VersionDocumento WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ")");
            this.sqlVersionDocumentoModify = pIBD.ReplaceParam("UPDATE VersionDocumento SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", Version = @Version, DocumentoOriginalID = " + pIBD.GuidParamColumnaTabla("DocumentoOriginalID") + ", IdentidadID = " + pIBD.GuidParamColumnaTabla("IdentidadID") + " WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ")");
            #endregion

            #region TagDocumento
            this.sqlTagDocumentoInsert = IBD.ReplaceParam("INSERT INTO TagDocumento (TagID, Tipo, DocumentoID) VALUES (@TagID, @Tipo, " + IBD.GuidParamColumnaTabla("DocumentoID") + ")");
            this.sqlTagDocumentoDelete = IBD.ReplaceParam("DELETE FROM TagDocumento WHERE (TagID = @O_TagID) AND (Tipo = @O_Tipo) AND (DocumentoID = " + IBD.GuidParamColumnaTabla("O_DocumentoID") + ")");
            this.sqlTagDocumentoModify = IBD.ReplaceParam("UPDATE TagDocumento SET TagID = @TagID, Tipo = @Tipo, DocumentoID = " + IBD.GuidParamColumnaTabla("DocumentoID") + " WHERE (TagID = @O_TagID) AND (Tipo = @O_Tipo) AND (DocumentoID = " + IBD.GuidParamColumnaTabla("O_DocumentoID") + ")");
            #endregion

            #region FichaBibliografica
            this.sqlFichaBibliograficaInsert = pIBD.ReplaceParam("INSERT INTO FichaBibliografica (FichaBibliograficaID, Nombre, Descripcion) VALUES (" + pIBD.GuidParamColumnaTabla("FichaBibliograficaID") + ", @Nombre, @Descripcion)");
            this.sqlFichaBibliograficaDelete = pIBD.ReplaceParam("DELETE FROM FichaBibliografica WHERE (FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("O_FichaBibliograficaID") + ")");
            this.sqlFichaBibliograficaModify = pIBD.ReplaceParam("UPDATE FichaBibliografica SET FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("FichaBibliograficaID") + ", Nombre = @Nombre, Descripcion = @Descripcion WHERE (FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("O_FichaBibliograficaID") + ")");
            #endregion

            #region AtributoFichaBibliografica
            this.sqlAtributoFichaBibliograficaInsert = pIBD.ReplaceParam("INSERT INTO AtributoFichaBibliografica (AtributoID, FichaBibliograficaID, Nombre, Descripcion, Tipo, Orden, Longitud) VALUES (" + pIBD.GuidParamColumnaTabla("AtributoID") + ", " + pIBD.GuidParamColumnaTabla("FichaBibliograficaID") + ", @Nombre, @Descripcion, @Tipo, @Orden, @Longitud)");
            this.sqlAtributoFichaBibliograficaDelete = pIBD.ReplaceParam("DELETE FROM AtributoFichaBibliografica WHERE (AtributoID = " + pIBD.GuidParamColumnaTabla("O_AtributoID") + ") AND (FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("O_FichaBibliograficaID") + ")");
            this.sqlAtributoFichaBibliograficaModify = pIBD.ReplaceParam("UPDATE AtributoFichaBibliografica SET AtributoID = " + pIBD.GuidParamColumnaTabla("AtributoID") + ", FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("FichaBibliograficaID") + ", Nombre = @Nombre, Descripcion = @Descripcion, Tipo = @Tipo, Orden = @Orden, Longitud = @Longitud WHERE (AtributoID = " + pIBD.GuidParamColumnaTabla("O_AtributoID") + ") AND (FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("O_FichaBibliograficaID") + ")");
            #endregion

            #region DocumentoAtributoBiblio
            this.sqlDocumentoAtributoBiblioInsert = pIBD.ReplaceParam("INSERT INTO DocumentoAtributoBiblio (DocumentoID, AtributoID, Valor, FichaBibliograficaID) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("AtributoID") + ", @Valor, " + pIBD.GuidParamColumnaTabla("FichaBibliograficaID") + ")");
            this.sqlDocumentoAtributoBiblioDelete = pIBD.ReplaceParam("DELETE FROM DocumentoAtributoBiblio WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND (AtributoID = " + pIBD.GuidParamColumnaTabla("O_AtributoID") + ") AND (FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("O_FichaBibliograficaID") + ")");
            this.sqlDocumentoAtributoBiblioModify = pIBD.ReplaceParam("UPDATE DocumentoAtributoBiblio SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", AtributoID = " + pIBD.GuidParamColumnaTabla("AtributoID") + ", Valor = @Valor, FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("FichaBibliograficaID") + " WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND (AtributoID = " + pIBD.GuidParamColumnaTabla("O_AtributoID") + ") AND (FichaBibliograficaID = " + pIBD.GuidParamColumnaTabla("O_FichaBibliograficaID") + ")");
            #endregion

            #region DocumentoComentario
            this.sqlDocumentoComentarioInsert = pIBD.ReplaceParam("INSERT INTO DocumentoComentario (ComentarioID, DocumentoID, ProyectoID) VALUES (" + pIBD.GuidParamColumnaTabla("ComentarioID") + ", " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ")");
            this.sqlDocumentoComentarioDelete = pIBD.ReplaceParam("DELETE FROM DocumentoComentario WHERE (ComentarioID = " + pIBD.GuidParamColumnaTabla("O_ComentarioID") + ") AND (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ") AND ProyectoID IS NULL)");
            this.sqlDocumentoComentarioModify = pIBD.ReplaceParam("UPDATE DocumentoComentario SET ComentarioID = " + pIBD.GuidParamColumnaTabla("ComentarioID") + ", DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (ComentarioID = " + pIBD.GuidParamColumnaTabla("O_ComentarioID") + ") AND (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ")");
            #endregion

            #region DocumentoWebVinBaseRecursos

            this.sqlDocumentoWebVinBaseRecursosInsert = pIBD.ReplaceParam("INSERT INTO DocumentoWebVinBaseRecursos (DocumentoID, BaseRecursosID, IdentidadPublicacionID, FechaPublicacion, TipoPublicacion, Compartido, LinkAComunidadOrigen, Eliminado, NumeroComentarios, NumeroVotos, PublicadorOrgID, PermiteComentarios, NivelCertificacionID, Rank,Rank_Tiempo, IndexarRecurso, PrivadoEditores, FechaCertificacion) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", " + pIBD.GuidParamColumnaTabla("IdentidadPublicacionID") + ", @FechaPublicacion, @TipoPublicacion, CAST(CASE WHEN @TipoPublicacion = 0 THEN 0 ELSE 1 END AS bit), @LinkAComunidadOrigen, @Eliminado, @NumeroComentarios, @NumeroVotos, " + pIBD.GuidParamColumnaTabla("PublicadorOrgID") + ", @PermiteComentarios, " + pIBD.GuidParamColumnaTabla("NivelCertificacionID") + ", @Rank, @Rank_Tiempo, @IndexarRecurso, @PrivadoEditores, @FechaCertificacion)");
            this.sqlDocumentoWebVinBaseRecursosDelete = pIBD.ReplaceParam("DELETE FROM DocumentoWebVinBaseRecursos WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("Original_BaseRecursosID") + ")");

            this.sqlDocumentoWebVinBaseRecursosModify = pIBD.ReplaceParam("UPDATE DocumentoWebVinBaseRecursos SET FechaPublicacion = @FechaPublicacion, TipoPublicacion = @TipoPublicacion, Compartido = CAST(CASE WHEN @TipoPublicacion = 0 THEN 0 ELSE 1 END AS bit), LinkAComunidadOrigen = @LinkAComunidadOrigen, Eliminado = @Eliminado,NumeroComentarios = @NumeroComentarios, NumeroVotos = @NumeroVotos, PublicadorOrgID = " + pIBD.GuidParamColumnaTabla("PublicadorOrgID") + ", PermiteComentarios = @PermiteComentarios, NivelCertificacionID = " + pIBD.GuidParamColumnaTabla("NivelCertificacionID") + " , Rank = @Rank, Rank_Tiempo = @Rank_Tiempo, IndexarRecurso = @IndexarRecurso, PrivadoEditores = @PrivadoEditores, FechaCertificacion = @FechaCertificacion WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("Original_BaseRecursosID") + ") ");

            #endregion


            #region DocumentoWebVinBaseRecursosExtra

            this.sqlDocumentoWebVinBaseRecursosExtraInsert = pIBD.ReplaceParam("INSERT INTO DocumentoWebVinBaseRecursosExtra (DocumentoID, BaseRecursosID, NumeroDescargas, NumeroConsultas, FechaUltimaVisita) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", @NumeroDescargas, @NumeroConsultas, @FechaUltimaVisita)");
            this.sqlDocumentoWebVinBaseRecursosExtraDelete = pIBD.ReplaceParam("DELETE FROM DocumentoWebVinBaseRecursosExtra WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("Original_BaseRecursosID") + ")");

            this.sqlDocumentoWebVinBaseRecursosExtraModify = pIBD.ReplaceParam("UPDATE DocumentoWebVinBaseRecursosExtra SET NumeroDescargas = @NumeroDescargas, NumeroConsultas = @NumeroConsultas, FechaUltimaVisita = @FechaUltimaVisita WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("Original_BaseRecursosID") + ") ");

            #endregion

            #region DocumentoWebAgCatTesauro
            this.sqlDocumentoWebAgCatTesauroInsert = pIBD.ReplaceParam("INSERT INTO DocumentoWebAgCatTesauro (Fecha, TesauroID, CategoriaTesauroID, BaseRecursosID, DocumentoID) VALUES (@Fecha, " + pIBD.GuidParamColumnaTabla("TesauroID") + ", " + pIBD.GuidParamColumnaTabla("CategoriaTesauroID") + ", " + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", " + pIBD.GuidParamColumnaTabla("DocumentoID") + ")");

            this.sqlDocumentoWebAgCatTesauroDelete = pIBD.ReplaceParam("DELETE FROM DocumentoWebAgCatTesauro WHERE (TesauroID = " + pIBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + pIBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ") AND (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ") AND (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ")");

            this.sqlDocumentoWebAgCatTesauroModify = pIBD.ReplaceParam("UPDATE DocumentoWebAgCatTesauro SET Fecha = @Fecha, TesauroID = " + pIBD.GuidParamColumnaTabla("TesauroID") + ", CategoriaTesauroID = " + pIBD.GuidParamColumnaTabla("CategoriaTesauroID") + ", BaseRecursosID = " + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + " WHERE (TesauroID = " + pIBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + pIBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ") AND (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ") AND (DocumentoID = " + pIBD.GuidParamColumnaTabla("O_DocumentoID") + ")");
            #endregion

            #region BaseRecursosOrganizacion
            this.sqlBaseRecursosOrganizacionInsert = pIBD.ReplaceParam("INSERT INTO BaseRecursosOrganizacion (BaseRecursosID, OrganizacionID, EspacioMaxMyGnossMB, EspacioActualMyGnossMB) VALUES (" + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", @EspacioMaxMyGnossMB, @EspacioActualMyGnossMB)");
            this.sqlBaseRecursosOrganizacionDelete = pIBD.ReplaceParam("DELETE FROM BaseRecursosOrganizacion WHERE (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ") AND (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");
            this.sqlBaseRecursosOrganizacionModify = pIBD.ReplaceParam("UPDATE BaseRecursosOrganizacion SET BaseRecursosID = " + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", EspacioMaxMyGnossMB = @EspacioMaxMyGnossMB, EspacioActualMyGnossMB = @EspacioActualMyGnossMB WHERE (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ") AND (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");
            #endregion

            #region BaseRecursosProyecto
            this.sqlBaseRecursosProyectoInsert = pIBD.ReplaceParam("INSERT INTO BaseRecursosProyecto (BaseRecursosID, OrganizacionID, ProyectoID) VALUES (" + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ")");
            this.sqlBaseRecursosProyectoDelete = pIBD.ReplaceParam("DELETE FROM BaseRecursosProyecto WHERE (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ") AND (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ")");
            this.sqlBaseRecursosProyectoModify = pIBD.ReplaceParam("UPDATE BaseRecursosProyecto SET BaseRecursosID = " + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ") AND (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ")");
            #endregion

            #region BaseRecursosUsuario
            this.sqlBaseRecursosUsuarioInsert = pIBD.ReplaceParam("INSERT INTO BaseRecursosUsuario (BaseRecursosID, UsuarioID, EspacioMaxMyGnossMB, EspacioActualMyGnossMB) VALUES (" + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", " + pIBD.GuidParamColumnaTabla("UsuarioID") + ", @EspacioMaxMyGnossMB, @EspacioActualMyGnossMB)");
            this.sqlBaseRecursosUsuarioDelete = pIBD.ReplaceParam("DELETE FROM BaseRecursosUsuario WHERE (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ") AND (UsuarioID = " + pIBD.GuidParamColumnaTabla("O_UsuarioID") + ")");
            this.sqlBaseRecursosUsuarioModify = pIBD.ReplaceParam("UPDATE BaseRecursosUsuario SET BaseRecursosID = " + pIBD.GuidParamColumnaTabla("BaseRecursosID") + ", UsuarioID = " + pIBD.GuidParamColumnaTabla("UsuarioID") + ", EspacioMaxMyGnossMB = @EspacioMaxMyGnossMB, EspacioActualMyGnossMB = @EspacioActualMyGnossMB WHERE (BaseRecursosID = " + pIBD.GuidParamColumnaTabla("O_BaseRecursosID") + ") AND (UsuarioID = " + pIBD.GuidParamColumnaTabla("O_UsuarioID") + ")");
            #endregion

            #region DocumentoEnvioNewsLetter
            this.sqlDocumentoEnvioNewsLetterInsert = pIBD.ReplaceParam("INSERT INTO DocumentoEnvioNewsLetter (DocumentoID, IdentidadID, Fecha, Idioma, EnvioSolicitado, EnvioRealizado, Grupos) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("IdentidadID") + ", @Fecha, @Idioma, @EnvioSolicitado, @EnvioRealizado, @Grupos)");
            this.sqlDocumentoEnvioNewsLetterDelete = pIBD.ReplaceParam("DELETE FROM DocumentoEnvioNewsLetter WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (IdentidadID = " + pIBD.GuidParamColumnaTabla("Original_IdentidadID") + ") AND (Fecha = @Original_Fecha)");
            this.sqlDocumentoEnvioNewsLetterModify = pIBD.ReplaceParam("UPDATE DocumentoEnvioNewsLetter SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", IdentidadID = " + pIBD.GuidParamColumnaTabla("IdentidadID") + ", Fecha = @Fecha, Idioma = @Idioma, EnvioSolicitado = @EnvioSolicitado, EnvioRealizado = @EnvioRealizado, Grupos = @Grupos WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (IdentidadID = " + pIBD.GuidParamColumnaTabla("Original_IdentidadID") + ") AND (Fecha = @Original_Fecha)");
            #endregion

            #region ColaDocumento, Cuidado con la columna de identiad
            this.sqlColaDocumentoInsert = pIBD.ReplaceParam("INSERT INTO ColaDocumento (DocumentoID, AccionRealizada, Estado, FechaEncolado, FechaProcesado,Prioridad,InfoExtra,EstadoCargaID) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", @AccionRealizada, @Estado, @FechaEncolado, @FechaProcesado,@Prioridad,@InfoExtra,@EstadoCargaID)");
            // Elimindo ID, @ID, Ya que es una columna de identidad.
            this.sqlColaDocumentoDelete = pIBD.ReplaceParam("DELETE FROM ColaDocumento WHERE (ID = @Original_ID)");
            this.sqlColaDocumentoModify = pIBD.ReplaceParam("UPDATE ColaDocumento SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", AccionRealizada = @AccionRealizada, Estado = @Estado, FechaEncolado = @FechaEncolado, FechaProcesado = @FechaProcesado,Prioridad=@Prioridad,InfoExtra=@InfoExtra,EstadoCargaID=@EstadoCargaID WHERE (ID = @Original_ID)");// Elimindo ID = @ID, Es una columna de identidad.
            #endregion

            #region DocumentoVincDoc
            this.sqlDocumentoVincDocInsert = pIBD.ReplaceParam("INSERT INTO DocumentoVincDoc (DocumentoID, DocumentoVincID, IdentidadID, Fecha) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("DocumentoVincID") + ", " + pIBD.GuidParamColumnaTabla("IdentidadID") + ", @Fecha)");
            this.sqlDocumentoVincDocDelete = pIBD.ReplaceParam("DELETE FROM DocumentoVincDoc WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (DocumentoVincID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoVincID") + ")");
            this.sqlDocumentoVincDocModify = pIBD.ReplaceParam("UPDATE DocumentoVincDoc SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", DocumentoVincID = " + pIBD.GuidParamColumnaTabla("DocumentoVincID") + ", IdentidadID = " + pIBD.GuidParamColumnaTabla("IdentidadID") + ", Fecha = @Fecha WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (DocumentoVincID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoVincID") + ")");
            #endregion

            #region DocumentoRespuesta
            this.sqlDocumentoRespuestaInsert = pIBD.ReplaceParam("INSERT INTO DocumentoRespuesta (DocumentoID,RespuestaID,Descripcion,NumVotos,Orden) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("RespuestaID") + ", @Descripcion, @NumVotos,@Orden)");
            this.sqlDocumentoRespuestaDelete = pIBD.ReplaceParam("DELETE FROM DocumentoRespuesta WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (RespuestaID = " + pIBD.GuidParamColumnaTabla("Original_RespuestaID") + ")");
            this.sqlDocumentoRespuestaModify = pIBD.ReplaceParam("UPDATE DocumentoRespuesta SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", RespuestaID = " + pIBD.GuidParamColumnaTabla("RespuestaID") + ", Descripcion = @Descripcion, NumVotos = @NumVotos, Orden=@Orden WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (RespuestaID = " + pIBD.GuidParamColumnaTabla("Original_RespuestaID") + ")");
            #endregion

            #region DocumentoRespuestaVoto
            this.sqlDocumentoRespuestaVotoInsert = pIBD.ReplaceParam("INSERT INTO DocumentoRespuestaVoto (DocumentoID, RespuestaID, IdentidadID) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", " + pIBD.GuidParamColumnaTabla("RespuestaID") + ", " + pIBD.GuidParamColumnaTabla("IdentidadID") + ")");
            this.sqlDocumentoRespuestaVotoDelete = pIBD.ReplaceParam("DELETE FROM DocumentoRespuestaVoto WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (RespuestaID = " + pIBD.GuidParamColumnaTabla("Original_RespuestaID") + ")  AND (IdentidadID = " + pIBD.GuidParamColumnaTabla("Original_IdentidadID") + ")");
            this.sqlDocumentoRespuestaVotoModify = pIBD.ReplaceParam("UPDATE DocumentoRespuestaVoto SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", RespuestaID = " + pIBD.GuidParamColumnaTabla("RespuestaID") + ", IdentidadID = " + pIBD.GuidParamColumnaTabla("IdentidadID") + " WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (RespuestaID = " + pIBD.GuidParamColumnaTabla("Original_RespuestaID") + ") AND (IdentidadID = " + pIBD.GuidParamColumnaTabla("Original_IdentidadID") + ")");
            #endregion

            #region DocumentoTokenBrightcove
            this.sqlDocumentoTokenBrightcoveInsert = pIBD.ReplaceParam("INSERT INTO DocumentoTokenBrightcove (TokenID, ProyectoID, OrganizacionID,UsuarioID,DocumentoID,FechaCreacion, Estado,NombreArchivo) VALUES (" + pIBD.GuidParamColumnaTabla("TokenID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("UsuarioID") + ", " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", @FechaCreacion, @Estado, @NombreArchivo)");
            this.sqlDocumentoTokenBrightcoveDelete = pIBD.ReplaceParam("DELETE FROM DocumentoTokenBrightcove WHERE (TokenID = " + pIBD.GuidParamColumnaTabla("Original_TokenID") + ")");
            this.sqlDocumentoTokenBrightcoveModify = pIBD.ReplaceParam("UPDATE DocumentoTokenBrightcove SET TokenID = " + pIBD.GuidParamColumnaTabla("TokenID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", UsuarioID = " + pIBD.GuidParamColumnaTabla("UsuarioID") + ", DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + " , FechaCreacion=@FechaCreacion , Estado=@Estado,  NombreArchivo=@NombreArchivo WHERE (TokenID = " + pIBD.GuidParamColumnaTabla("Original_TokenID") + ")");
            #endregion

            #region DocumentoTokenTOP
            this.sqlDocumentoTokenTOPInsert = pIBD.ReplaceParam("INSERT INTO DocumentoTokenTOP (TokenID, ProyectoID, OrganizacionID,UsuarioID,DocumentoID,FechaCreacion, Estado,NombreArchivo) VALUES (" + pIBD.GuidParamColumnaTabla("TokenID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("UsuarioID") + ", " + pIBD.GuidParamColumnaTabla("DocumentoID") + ", @FechaCreacion, @Estado, @NombreArchivo)");
            this.sqlDocumentoTokenTOPDelete = pIBD.ReplaceParam("DELETE FROM DocumentoTokenTOP WHERE (TokenID = " + pIBD.GuidParamColumnaTabla("Original_TokenID") + ")");
            this.sqlDocumentoTokenTOPModify = pIBD.ReplaceParam("UPDATE DocumentoTokenTOP SET TokenID = " + pIBD.GuidParamColumnaTabla("TokenID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", UsuarioID = " + pIBD.GuidParamColumnaTabla("UsuarioID") + ", DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + " , FechaCreacion=@FechaCreacion , Estado=@Estado,  NombreArchivo=@NombreArchivo WHERE (TokenID = " + pIBD.GuidParamColumnaTabla("Original_TokenID") + ")");
            #endregion

            #region DocumentoNewsletter
            this.sqlDocumentoNewsletterInsert = pIBD.ReplaceParam("INSERT INTO DocumentoNewsletter (DocumentoID,Newsletter,NewsletterTemporal) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", @Newsletter,@NewsletterTemporal)");
            this.sqlDocumentoNewsletterDelete = pIBD.ReplaceParam("DELETE FROM DocumentoNewsletter WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ")");
            this.sqlDocumentoNewsletterModify = pIBD.ReplaceParam("UPDATE DocumentoNewsletter SET DocumentoID = " + pIBD.GuidParamColumnaTabla("DocumentoID") + " , Newsletter=@Newsletter, NewsletterTemporal=@NewsletterTemporal WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ")");
            #endregion

            #region ColaCargaRecursos CUIDADO, quitar COLAID
            this.sqlColaCargaRecursosInsert = IBD.ReplaceParam("INSERT INTO ColaCargaRecursos (ID, ProyectoID, UsuarioID, Fecha, Estado, NombreFichImport) VALUES (" + IBD.GuidParamColumnaTabla("ID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("UsuarioID") + ", @Fecha, @Estado, @NombreFichImport)");
            this.sqlColaCargaRecursosDelete = IBD.ReplaceParam("DELETE FROM ColaCargaRecursos WHERE (ColaID = @Original_ColaID)");
            this.sqlColaCargaRecursosModify = IBD.ReplaceParam("UPDATE ColaCargaRecursos SET ID = " + IBD.GuidParamColumnaTabla("ID") + ",ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", Fecha = @Fecha, Estado = @Estado, NombreFichImport = @NombreFichImport WHERE (ColaID = @Original_ColaID)");
            #endregion

            #region ColaCrawler
            this.sqlColaCrawlerInsert = IBD.ReplaceParam("INSERT INTO ColaCrawler (DBLP, DxDoi, RealURL, html, Fecha, Estado) VALUES (@DBLP, @DxDoi, @RealURL, @html, @Fecha, @Estado)");
            this.sqlColaCrawlerDelete = IBD.ReplaceParam("DELETE FROM ColaCrawler WHERE (ColaID = @Original_ColaID)");
            this.sqlColaCrawlerModify = IBD.ReplaceParam("UPDATE ColaCrawler SET DBLP = @DBLP, DxDoi = @DxDoi, RealURL = @RealURL, html = @html, Fecha = @Fecha, Estado = @Estado WHERE (ColaID = @Original_ColaID)");
            #endregion

            #region DocumentoUrlCanonica
            this.sqlDocumentoUrlCanonicaInsert = pIBD.ReplaceParam("INSERT INTO DocumentoUrlCanonica (DocumentoID,UrlCanonica) VALUES (" + pIBD.GuidParamColumnaTabla("DocumentoID") + ", @UrlCanonica)");
            this.sqlDocumentoUrlCanonicaDelete = pIBD.ReplaceParam("DELETE FROM DocumentoUrlCanonica WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ")");
            this.sqlDocumentoUrlCanonicaModify = pIBD.ReplaceParam("UPDATE DocumentoUrlCanonica SET  UrlCanonica=@UrlCanonica WHERE (DocumentoID = " + pIBD.GuidParamColumnaTabla("Original_DocumentoID") + ")");
            #endregion

            #endregion
        }

        



        #endregion
    }

    /// <summary>
    /// Clase que contiene constantes con los texto de las entidades vinculadas.
    /// </summary>
    public class TipoEntidadVinculadaDocumentoTexto
    {
        /// <summary>
        /// Contien la cadena de texto para RecursoWeb.
        /// </summary>
        public const string RecursoWeb = "Base de recursos";
        /// <summary>
        /// Contien la cadena de texto para RecursoWeb.
        /// </summary>
        public const string RecursoObjetoGnoss = "Recursos objeto gnoss";
        /// <summary>
        /// Contien la cadena de texto para Competencia.
        /// </summary>
        public const string Competencia = "Competencia";
        /// <summary>
        /// Contien la cadena de texto para Competencia.
        /// </summary>
        public const string Dimension = "Dimensión";
        /// <summary>
        /// Contien la cadena de texto para Proceso.
        /// </summary>
        public const string Proceso = "Proceso";
        /// <summary>
        /// Contien la cadena de texto para Objetivo.
        /// </summary>
        public const string Objetivo = "Objetivo";
        /// <summary>
        /// Contien la cadena de texto para CurrículumVitae.
        /// </summary>
        public const string CurriculumVitae = "Currículum Vitae";
        /// <summary>
        /// Contien la cadena de texto para Cualquiera.
        /// </summary>
        public const string Cualquiera = "Cualquiera";
        /// <summary>
        /// Contien la cadena de texto para ImagenWiki.
        /// </summary>
        public const string ImagenWiki = "Imagen Wiki";
        /// <summary>
        /// Contien la cadena de texto para grupo funcional.
        /// </summary>
        public const string GrupoFuncional = "Grupo funcional";
        /// <summary>
        /// Nombre de la carpeta donde se almacenan los recursos web.
        /// </summary>
        public const string BASE_RECURSOS = "BaseRecursos";

        /// <summary>
        /// Nombre de la carpeta donde se almacenan los recursos web.
        /// </summary>
        public const string Ontologia = "Ontologia";

        /// <summary>
        /// Nombre para la carpeta de acreditaciones al CV.
        /// </summary>
        public const string CV_Acreditacion = "CVAcreditacion";
    }

    /// <summary>
    /// Clase que contiene constantes con los texto de las entidades vinculadas.
    /// </summary>
    public static class TipoDocumentoTexto
    {
        /// <summary>
        /// Contien la cadena de texto para FicheroServidor.
        /// </summary>
        public const string FicheroServidor = "Fichero digital";
        public const string FicheroServidorEN = "Digital file";

        /// <summary>
        /// Contien la cadena de texto para Audio.
        /// </summary>
        public const string Audio = "Audio";

        /// <summary>
        /// Contien la cadena de texto para Debate.
        /// </summary>
        public const string Debate = "Debate";

        /// <summary>
        /// Contien la cadena de texto para Pregunta.
        /// </summary>
        public const string Pregunta = "Pregunta";
        public const string PreguntaEN = "Question";

        /// <summary>
        /// Contien la cadena de texto para Encuesta.
        /// </summary>
        public const string Encuesta = "Encuesta";
        public const string EncuestaEN = "Poll";

        /// <summary>
        /// Contien la cadena de texto para Hipervinculo.
        /// </summary>
        public const string Hipervinculo = "Enlace Web";
        public const string HipervinculoEN = "Hiperlink";
        /// <summary>
        /// Contien la cadena de texto para Imagen.
        /// </summary>
        public const string Imagen = "Imagen";
        public const string ImagenEN = "Image";
        /// <summary>
        /// Contien la cadena de texto para ReferenciaADoc.
        /// </summary>
        public const string ReferenciaADoc = "Documento físico";
        public const string ReferenciaADocEN = "Document reference";
        /// <summary>
        /// Contien la cadena de texto para Semantico.
        /// </summary>
        public const string Semantico = "Ontología específica";
        public const string SemanticoEN = "Specific ontology";
        /// <summary>
        /// Contien la cadena de texto para Video.
        /// </summary>
        public const string Video = "Video";

        /// <summary>
        /// Contien la cadena de texto para nota.
        /// </summary>
        public const string Nota = "Nota";
        public const string NotaEN = "Note";

        /// <summary>
        /// Contien la cadena de texto para Newsletter.
        /// </summary>
        public const string Newsletter = "Newsletter";

        /// <summary>
        /// Contien la cadena de texto para nota.
        /// </summary>
        public const string ArticuloWiki = "Artículo Wiki";
        public const string ArticuloWikiEN = "Wiki article";

        /// <summary>
        /// Contien la cadena de texto para nota.
        /// </summary>
        public const string ArticuloBlog = "Artículo de Blog";
        public const string ArticuloBlogEN = "Blog article";

        public static string ToFrindlyString(this TiposDocumentacion pTipo, string pLanguage = "es")
        {
            switch (pLanguage)
            {
                case "es":
                default:
                    switch (pTipo)
                    {
                        case TiposDocumentacion.Audio:
                            return Audio;
                        case TiposDocumentacion.Debate:
                            return Debate;
                        case TiposDocumentacion.Encuesta:
                            return Encuesta;
                        case TiposDocumentacion.FicheroServidor:
                            return FicheroServidor;
                        case TiposDocumentacion.Hipervinculo:
                            return Hipervinculo;
                        case TiposDocumentacion.Imagen:
                            return Imagen;
                        case TiposDocumentacion.Newsletter:
                            return Newsletter;
                        case TiposDocumentacion.Nota:
                            return Nota;
                        case TiposDocumentacion.Pregunta:
                            return Pregunta;
                        case TiposDocumentacion.Semantico:
                            return Semantico;
                        case TiposDocumentacion.Video:
                            return Video;
                        case TiposDocumentacion.Wiki:
                            return ArticuloWiki;
                        default:
                            return Nota;
                    }
                case "en":
                    switch (pTipo)
                    {
                        case TiposDocumentacion.Audio:
                            return Audio;
                        case TiposDocumentacion.Debate:
                            return Debate;
                        case TiposDocumentacion.Encuesta:
                            return EncuestaEN;
                        case TiposDocumentacion.FicheroServidor:
                            return FicheroServidorEN;
                        case TiposDocumentacion.Hipervinculo:
                            return HipervinculoEN;
                        case TiposDocumentacion.Imagen:
                            return ImagenEN;
                        case TiposDocumentacion.Newsletter:
                            return Newsletter;
                        case TiposDocumentacion.Nota:
                            return NotaEN;
                        case TiposDocumentacion.Pregunta:
                            return PreguntaEN;
                        case TiposDocumentacion.Semantico:
                            return SemanticoEN;
                        case TiposDocumentacion.Video:
                            return Video;
                        case TiposDocumentacion.Wiki:
                            return ArticuloWikiEN;
                        default:
                            return NotaEN;
                    }
            }
        }

        /// <summary>
        /// Obtiene el tipo de documento a partir de su texto.
        /// </summary>
        /// <param name="pTipoTexto">Tipo de documento en modo texto</param>
        /// <returns>TiposDocumentacion que corresponde al tipo texto</return s>
        public static TiposDocumentacion ObtenerTipoDocumento(string pTipoTexto)
        {
            switch (pTipoTexto)
            {
                case FicheroServidor:
                    return TiposDocumentacion.FicheroServidor;
                case Hipervinculo:
                    return TiposDocumentacion.Hipervinculo;
                case Imagen:
                    return TiposDocumentacion.Imagen;
                case ReferenciaADoc:
                    return TiposDocumentacion.ReferenciaADoc;
                case Semantico:
                    return TiposDocumentacion.Semantico;
                case Video:
                    return TiposDocumentacion.Video;
                case Nota:
                    return TiposDocumentacion.Nota;
                case Newsletter:
                    return TiposDocumentacion.Newsletter;
                case ArticuloWiki:
                    return TiposDocumentacion.Wiki;
                case ArticuloBlog:
                    return TiposDocumentacion.EntradaBlog;
            }
            return TiposDocumentacion.FicheroServidor;
        }
    }
}
