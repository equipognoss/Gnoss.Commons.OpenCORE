using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Live
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración que contiene los tipos de elementos de la comunidad que pueden haber sido modificados
    /// </summary>
    public enum TipoLive
    {
        /// <summary>
        /// Recurso genérico
        /// </summary>
        Recurso = 0,
        /// <summary>
        /// Recurso de tipo pregunta
        /// </summary>
        Pregunta = 1,
        /// <summary>
        /// Recurso de tipo debate
        /// </summary>
        Debate = 2,
        /// <summary>
        /// Dafo
        /// </summary>
        Dafo = 3,
        /// <summary>
        /// Miembro
        /// </summary>
        Miembro = 4,
        /// <summary>
        /// EntradaBlog
        /// </summary>
        EntradaBlog = 5,
        /// <summary>
        /// Blog
        /// </summary>
        Blog = 6,
        /// <summary>
        /// Suscripcion a un usuario
        /// </summary>
        SuscripcionUsuario = 7,
        /// <summary>
        /// Suscripción a un blog
        /// </summary>
        SuscripcionBlog = 8,

        /// <summary>
        /// Suscripción a un blog
        /// </summary>
        SuscripcionUsuarioComunidad = 9,

        ///// <summary>
        // /// Suscripción a un blog
        // /// </summary>
        // CV = 10,


        // /// <summary>
        // /// Comunidad
        // /// </summary>
        Comunidad = 11,

        /// <summary>
        /// Cuando se agrupan los nuevos miembros de una comunidad
        /// </summary>
        AgrupacionNuevosMiembros = 12
    }

    /// <summary>
    /// Enumeración que contiene las diferentes acciones que pueden haber sucedido
    /// </summary>
    public enum AccionLive
    {
        /// <summary>
        /// Agregado
        /// </summary>
        Agregado = 0,
        /// <summary>
        /// Eliminado
        /// </summary>
        Eliminado = 1,
        /// <summary>
        /// Editado
        /// </summary>
        Editado = 2,
        /// <summary>
        /// Se ha agregado un comentado
        /// </summary>
        ComentarioAgregado = 3,
        /// <summary>
        /// Se ha eliminado un comentario
        /// </summary>
        ComentarioEliminado = 4,
        /// <summary>
        /// Se ha editado un comentario
        /// </summary>
        ComentarioEditado = 5,
        /// <summary>
        /// Cambia el estado de la pregunta
        /// </summary>
        EstadoCambiado = 6,
        /// <summary>
        /// Votado
        /// </summary>
        Votado = 7,
        /// <summary>
        /// Ha editado el perfil
        /// </summary>
        PerfilEditado = 8,
        /// <summary>
        /// Ha creado un factor de dafo
        /// </summary>
        FactorNuevo = 9,
        /// <summary>
        /// Ha votado un factor de dafo
        /// </summary>
        FactorVotado = 10,
        /// <summary>
        /// Ha eliminado un factor de dafo
        /// </summary>
        FactorEliminado = 11,
        /// <summary>
        /// Se ha certificado un recurso.
        /// </summary>
        RecursoCertificado = 12,
        /// <summary>
        /// Se ha certificado un recurso.
        /// </summary>
        VersionCreada = 13,
        /// <summary>
        /// Ha editado un factor.
        /// </summary>
        FactorEditado = 14,
        /// <summary>
        /// Ha agregado un recurso.
        /// </summary>
        RecursoAgregado = 15,
        /// <summary>
        /// Acción generada por el live para propagar una votación.
        /// </summary>
        VotadoPropagacion = 16,
        /// <summary>
        /// Acción para comprobar la caducidad de un elemento.
        /// </summary>
        ComprobacionCaducidad = 17,

        /// <summary>
        /// Ha agregado un CV.
        /// </summary>
        CVAgregado = 18,

        /// <summary>
        /// ComunidadAbierta.
        /// </summary>
        ComunidadAbierta = 19,

        /// <summary>
        /// Visitar Recurso.
        /// </summary>
        VisitaRecurso = 20,


        /// <summary>
        /// Vincular Recurso a Recurso.
        /// </summary>
        VincularRecursoaRecurso = 21,


        /// <summary>
        /// Suscribirse a usuario.
        /// </summary>
        SuscribirseUsuario = 22,

        /// <summary>
        /// Suscribirse a usuario.
        /// </summary>
        SuscribirseUsuarioComunidad = 23,


        /// <summary>
        /// Suscribirse a usuario.
        /// </summary>
        SuscribirseUsuarioBlog = 24,



        /// <summary>
        /// Suscribirse a usuario.
        /// </summary>
        AgregarArticuloBlog = 25,


        /// <summary>
        /// Ha elinado o descompartido de una comunidad un CV.
        /// </summary>
        CVEliminado = 26,


        /// <summary>
        /// Dessuscribirse a usuario.
        /// </summary>
        DessuscribirseUsuario = 27,

        /// <summary>
        /// Suscribirse a usuario.
        /// </summary>
        DessuscribirseUsuarioComunidad = 28,


        /// <summary>
        /// Dessuscribirse a usuario.
        /// </summary>
        DessuscribirseUsuarioBlog = 29,

        /// <summary>
        /// Desincular Recurso a Recurso.
        /// </summary>
        DesincularRecursoaRecurso = 30,


        /// <summary>
        /// Se ha descertificado un recurso.
        /// </summary>
        RecursoDesCertificado = 31,


        /// <summary>
        /// ComunidadCerrada.
        /// </summary>
        ComunidadCerrada = 32,


        /// <summary>
        /// Suscribirse a usuario.
        /// </summary>
        EliminarArticuloBlog = 33,

        /// <summary>
        /// Un usuario se ha hecho contacto de otro
        /// </summary>
        ContactoAgregado = 34,

        /// <summary>
        /// Se reprocesa el evento para regenerarlo en la home del proyecto.
        /// </summary>
        ReprocesarEventoHomeProyecto = 35
    }

    /// <summary>
    /// Enumeración que indica la visibilidad de los eventos de perfil
    /// </summary>
    public enum VisibilidadEventoPerfil
    {
        /// <summary>
        /// Público en internet.
        /// </summary>
        Publico = 0,
        /// <summary>
        /// Visible para los usuario de Gnosss.
        /// </summary>
        UsuariosGnoss = 1,
        /// <summary>
        /// Visible para los miembros de una comunidad.
        /// </summary>
        PrivadoParaComunidad = 2,
        /// <summary>
        /// Visible para ciertos perfiles.
        /// </summary>
        PrivadoParaPerfiles = 3
    }

    /// <summary>
    /// Tipo de filtro para las pestañas de comentario.
    /// </summary>
    public enum TipoLiveComentario
    {
        /// <summary>
        /// Pestaña de contribuciones.
        /// </summary>
        Contribuciones = 0,
        /// <summary>
        /// Pestaña de mis recursos.
        /// </summary>
        MisRecursos = 1,
        /// <summary>
        /// Pestaña de blogs.
        /// </summary>
        Blogs = 2
    }

    /// <summary>
    /// Caducidad para los comentario del perfil.
    /// </summary>
    public enum CaducidadComentariosPerfil
    {
        /// <summary>
        /// Semanal.
        /// </summary>
        Semanal = 0,
        /// <summary>
        /// Mensual.
        /// </summary>
        Mensual = 1
    }

    /// <summary>
    /// Prioridad para las colas del Live
    /// </summary>
    public enum PrioridadLive
    {
        /// <summary>
        /// Prioridad Alta.
        /// </summary>
        Alta = 0,
        /// <summary>
        /// Prioridad Media.
        /// </summary>
        Media = 1,
        /// <summary>
        /// Prioridad Baja.
        /// </summary>
        Baja = 2
    }

    #endregion

    /// <summary>
    /// DataAdapter de Live
    /// </summary>
    public class LiveAD : BaseAD
    {
        private bool mUsariRabbitSiEstaConfigurado;

        private EntityContext mEntityContext;

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public LiveAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
           
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public LiveAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
           
        }

        #endregion

        #region Consultas

        private string sqlSelectCola;
        private string sqlSelectContadorPerfil;
        private string sqlSelectConfiguracionContPerfil;
        private string sqlSelectColaPopularidad;

        #endregion

        #region DataAdapter

        #region Cola

        private string sqlColaInsert;
        private string sqlColaDelete;
        private string sqlColaModify;

        #endregion

        #region ContadorPerfil
        private string sqlContadorPerfilInsert;
        private string sqlContadorPerfilDelete;
        private string sqlContadorPerfilModify;
        #endregion

        #region ConfiguracionContPerfil
        private string sqlConfiguracionContPerfilInsert;
        private string sqlConfiguracionContPerfilDelete;
        private string sqlConfiguracionContPerfilModify;
        #endregion
        #region ColaPopularidad
        private string sqlColaPopularidadInsert;
        private string sqlColaPopularidadDelete;
        private string sqlColaPopularidadModify;
        #endregion

        #endregion


        #region Métodos AD

        /// <summary>
        /// Actualiza la base de datos
        /// </summary>
        /// <param name="pDataSet">Data set con las modificaciones</param>
        public void ActualizarBD(DataSet pDataSet, bool pUsariRabbitSiEstaConfigurado = true)
        {
            mUsariRabbitSiEstaConfigurado = pUsariRabbitSiEstaConfigurado;

            //EliminarBorrados(pDataSet);
            //GuardarActualizaciones(pDataSet);
        }

        /// Actualiza BD y el disminuye el numero de comentarios del contador de perfil.
        /// </summary>
        /// <param name="pLiveDS">Dataset Live</param>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pTiposComentarioActualizar">Tipos de comentarios de los contadores que hay que disminuir</param>
        /// <param name="pDisminuir">Indica si se debe disminuir o aumentar los contadores</param>
        public void ActualizarComentariosBD(LiveDS pLiveDS, Guid pPerfilID, List<TipoLiveComentario> pTiposComentarioActualizar, bool pDisminuir)
        {
            //EliminarBorrados(pLiveDS);
            //GuardarActualizaciones(pLiveDS);

            ActualizarContadoresPerfil(pPerfilID, pTiposComentarioActualizar, pDisminuir);
        }

        #region Nuevos elementos bandeja
        //No se usa en la Web
        /// <summary>
        /// Aumenta o disminuye en 1 el número de nuevos comentarios de un perfill.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pDisminuir">Indica si se debe disminuir o aumentar los contadores</param>
        /// <param name="pFechaProcesado">Fecha en la que se realizo la 1º acción o NULL si no hay tenerla en cuenta</param>
        public void ActualizarNuevosComentarios(Guid pPerfilID, bool pDisminuir, DateTime? pFechaProcesado)
        {
            if (pDisminuir)
            {
                ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID) && item.NuevosComentarios > 0);
                if (pFechaProcesado.HasValue)
                {
                    contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID) && item.NuevosComentarios > 0 && (!item.FechaVisitaComentarios.HasValue || item.FechaVisitaComentarios < pFechaProcesado.Value));
                }
                contadorPerfil.NuevosComentarios = contadorPerfil.NuevosComentarios - 1;
            }
            else
            {//sumar
                ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
                if (pFechaProcesado.HasValue)
                {
                    contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID) && (!item.FechaVisitaComentarios.HasValue || item.FechaVisitaComentarios < pFechaProcesado.Value));
                }
                contadorPerfil.NuevosComentarios = contadorPerfil.NuevosComentarios + 1;
            }

            mEntityContext.SaveChanges();
        }
        //No se usa en la Web
        /// <summary>
        /// Aumenta o disminuye en 1 el número de nuevas suscripciones de un perfill.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pDisminuir">Indica si se debe disminuir o aumentar los contadores</param>
        /// <param name="pFechaProcesado">Fecha en la que se realizo la 1º acción o NULL si no hay tenerla en cuenta</param>
        public void ActualizarNuevasSuscripciones(Guid pPerfilID, bool pDisminuir, DateTime? pFechaProcesado)
        {
            if (pDisminuir)
            {
                ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID) && item.NuevosComentarios > 0);
                if (pFechaProcesado.HasValue)
                {
                    contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID) && item.NuevasSuscripciones > 0 && (!item.FechaVisitaSuscripciones.HasValue || item.FechaVisitaSuscripciones < pFechaProcesado.Value));
                }
                contadorPerfil.NuevasSuscripciones = contadorPerfil.NuevasSuscripciones - 1;
            }
            else
            {//sumar
                ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
                if (pFechaProcesado.HasValue)
                {
                    contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID) && (!item.FechaVisitaSuscripciones.HasValue || item.FechaVisitaSuscripciones < pFechaProcesado.Value));
                }
                contadorPerfil.NuevasSuscripciones = contadorPerfil.NuevasSuscripciones + 1;
            }

            mEntityContext.SaveChanges();
        }
        //No se usa en la Web
        /// <summary>
        /// Aumenta en 1 el contador de nuevos mensajes.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void AumentarContadorNuevosMensajes(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            if (contadorPerfil == null) //Fila de contador no creada, hay que crearla.
            {
                contadorPerfil = CrearFilaContadorPerfil(pPerfilID);
                contadorPerfil.NumNuevosMensajes = contadorPerfil.NumNuevosMensajes + 1;
                contadorPerfil.NumMensajesSinLeer = contadorPerfil.NumMensajesSinLeer + 1;
                mEntityContext.ContadorPerfil.Add(contadorPerfil);
            }
            else
            {
                contadorPerfil.NumNuevosMensajes = contadorPerfil.NumNuevosMensajes + 1;
                contadorPerfil.NumMensajesSinLeer = contadorPerfil.NumMensajesSinLeer + 1;
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Aumenta en 1 el contador de nuevas invitaciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void AumentarContadorNuevasInvitaciones(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            if (contadorPerfil == null) //Fila de contador no creada, hay que crearla.
            {
                contadorPerfil = CrearFilaContadorPerfil(pPerfilID);
                contadorPerfil.NuevasInvitaciones = contadorPerfil.NuevasInvitaciones + 1;
                contadorPerfil.NumInvitacionesSinLeer = contadorPerfil.NumInvitacionesSinLeer + 1;
                mEntityContext.ContadorPerfil.Add(contadorPerfil);
            }
            else
            {
                contadorPerfil.NuevasInvitaciones = contadorPerfil.NuevasInvitaciones + 1;
                contadorPerfil.NumInvitacionesSinLeer = contadorPerfil.NumInvitacionesSinLeer + 1;
            }
            mEntityContext.SaveChanges();
        }
        //No se usa en la Web
        /// <summary>
        /// Aumenta en 1 el contador de nuevas Suscripciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void AumentarContadorNuevasSuscripciones(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));

            if (contadorPerfil == null) //Fila de contador no creada, hay que crearla.
            {
                contadorPerfil = CrearFilaContadorPerfil(pPerfilID);
                mEntityContext.ContadorPerfil.Add(contadorPerfil);
            }
            if(contadorPerfil.NuevasSuscripciones < 100)
            {
                contadorPerfil.NuevasSuscripciones = contadorPerfil.NuevasSuscripciones + 1;
            }
            mEntityContext.SaveChanges();
        }
        //No se usa en la Web
        /// <summary>
        /// Aumenta en 1 el contador de nuevos comentarios.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void AumentarContadorNuevosComentarios(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));

            if (contadorPerfil == null) //Fila de contador no creada, hay que crearla.
            {
                contadorPerfil = CrearFilaContadorPerfil(pPerfilID);
                contadorPerfil.NuevosComentarios = contadorPerfil.NuevosComentarios + 1;
                contadorPerfil.NumComentariosSinLeer = contadorPerfil.NumComentariosSinLeer + 1;
                mEntityContext.ContadorPerfil.Add(contadorPerfil);
            }
            else
            {
                contadorPerfil.NuevosComentarios = contadorPerfil.NuevosComentarios + 1;
                contadorPerfil.NumComentariosSinLeer = contadorPerfil.NumComentariosSinLeer + 1;
            }

            mEntityContext.SaveChanges();
        }
        /// <summary>
        /// Disminuye en 1 el contador de mensajes sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void DisminuirContadorMensajesLeidos(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.Where(x => x.PerfilID.Equals(pPerfilID) && x.NumMensajesSinLeer > 0).FirstOrDefault();
            if(contadorPerfil != null)
            {
                contadorPerfil.NumMensajesSinLeer = contadorPerfil.NumMensajesSinLeer - 1;
            }

            mEntityContext.SaveChanges();
        }
        /// <summary>
        /// Disminuye en 1 el contador de invitaciones sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void DisminuirContadoInvitacionesLeidas(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.Where(item => item.PerfilID.Equals(pPerfilID) && item.NumInvitacionesSinLeer > 0).FirstOrDefault();
            if (contadorPerfil != null)
            {
                contadorPerfil.NumInvitacionesSinLeer = contadorPerfil.NumInvitacionesSinLeer - 1;
            }

            mEntityContext.SaveChanges();
        }
        /// <summary>
        /// Disminuye en 1 el contador de invitaciones sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void DisminuirContadorComentariosLeidos(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.Where(item => item.PerfilID.Equals(pPerfilID) && item.NumComentariosSinLeer > 0).FirstOrDefault();
            if (contadorPerfil != null)
            {
                contadorPerfil.NumComentariosSinLeer = contadorPerfil.NumComentariosSinLeer - 1;
            }

            mEntityContext.SaveChanges();
        }
        //No se usa en la Web
        /// <summary>
        /// Disminuye en 1 el contador de suscripciones sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pDisminuir">Indica si se disminuye o no el contador</param>
        public void AumentarDisminuirContadoSuscripcionesLeidas(Guid pPerfilID, bool pDisminuir)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));

            if (!pDisminuir) //Fila de contador no creada, hay que crearla.
            {
                contadorPerfil.NumSuscripcionesSinLeer = contadorPerfil.NumSuscripcionesSinLeer + 1;
            }
            else
            {
                if (contadorPerfil.NumSuscripcionesSinLeer > 0)
                {
                    contadorPerfil.NumSuscripcionesSinLeer = contadorPerfil.NumSuscripcionesSinLeer - 1;
                }
            }

            mEntityContext.SaveChanges();
        }
        //No se usa en la Web
        /// <summary>
        /// Establece el contador de suscripciones sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pNumero">Número de suscripciones sin leer</param>
        public void EstablecerContadorSuscripcionesSinLeer(Guid pPerfilID, int pNumero)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            contadorPerfil.NumSuscripcionesSinLeer = pNumero;
            mEntityContext.SaveChanges();
        }
        //No se usa en la Web
        /// <summary>
        /// Comprueba si un perfil tiene fila de contadorPerfil, si no es así la crea.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ComprobarPerfilTieneFilaContadorPerfil(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            if (contadorPerfil == null)
            {
                contadorPerfil = CrearFilaContadorPerfil(pPerfilID);
                mEntityContext.ContadorPerfil.Add(contadorPerfil);
                mEntityContext.SaveChanges();
            }
            
        }

        /// <summary>
        /// Creo la fila de contador para un perfil.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public ContadorPerfil CrearFilaContadorPerfil(Guid pPerfilID)
        {
            ContadorPerfil filaContador = new ContadorPerfil();
            filaContador.PerfilID = pPerfilID;
            filaContador.NuevasInvitaciones = 0;
            filaContador.NuevasSuscripciones = 0;
            filaContador.NuevosComentarios = 0;
            filaContador.NumComentarios = 0;
            filaContador.NumComentBlog = 0;
            filaContador.NumComentContribuciones = 0;
            filaContador.NumComentMisRec = 0;
            filaContador.NumNuevosMensajes = 0;
            filaContador.NumMensajesSinLeer = 0;
            filaContador.NumComentariosSinLeer = 0;
            filaContador.NumInvitacionesSinLeer = 0;
            filaContador.NumSuscripcionesSinLeer = 0;
            return filaContador;
        }

        /// <summary>
        /// Pone a 0 el contador de nuevos mensajes.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevosMensajes(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            if(contadorPerfil != null)
            {
                contadorPerfil.NumNuevosMensajes = 0;
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Pone a 0 el contador de nuevas invitaciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevasInvitaciones(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            if (contadorPerfil != null)
            {
                contadorPerfil.NuevasInvitaciones = 0;
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Pone a 0 el contador de nuevos comentarios.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevosComentarios(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            if (contadorPerfil != null)
            {
                contadorPerfil.NuevosComentarios = 0;
                contadorPerfil.NumComentariosSinLeer = 0;
                contadorPerfil.FechaVisitaComentarios = DateTime.Now;
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Pone a 0 el contador de nuevas sucripciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevasSuscripciones(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            if (contadorPerfil != null)
            {
                contadorPerfil.NuevasSuscripciones = 0;
                contadorPerfil.FechaVisitaSuscripciones = DateTime.Now;
            }
            mEntityContext.SaveChanges();
        }

        #endregion
        #region Métodos de consultas

        /// <summary>
        /// Obtiene los contadores de un Perfil
        /// </summary>
        /// <param name="pPerfilID">perfilID</param>
        /// <returns>Data set con los contadores recuperados</returns>
        public ContadorPerfil ObtenerContadoresPerfil(Guid pPerfilID)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            return contadorPerfil;
        }

        /// <summary>
        /// Obtiene los contadores de varios Perfiles
        /// </summary>
        /// <param name="pPerfilID">perfilID</param>
        /// <returns>Data set con los contadores recuperados</returns>
        public List<ContadorPerfil> ObtenerContadoresPerfiles(List<Guid> pPerfilesIDs)
        {
            List<ContadorPerfil> listaContadorPerfiles = new List<ContadorPerfil>();

            if (pPerfilesIDs.Count > 0)
            {
                //TODO Comentado por edma no existe la tabla en la BD
                try
                {
                    listaContadorPerfiles = mEntityContext.ContadorPerfil.Where(item => pPerfilesIDs.Contains(item.PerfilID)).ToList();
                }
                catch (Exception)
                {

                }
            }
            return listaContadorPerfiles;
        }

        /// <summary>
        /// Actualiza los contadores de un Perfil disminuyendo o aumentando en 1 los tipo solicitdos además del total.
        /// </summary>
        /// <param name="pPerfilID">perfilID</param>
        /// <param name="pTiposComentarioActualizar">Tipos de comentarios de los contadores que hay que disminuir o aumentar</param>
        /// <param name="pDisminuir">Indica si se debe disminuir o aumentar los contadores</param>
        public void ActualizarContadoresPerfil(Guid pPerfilID, List<TipoLiveComentario> pTiposComentarioActualizar, bool pDisminuir)
        {
            if (pTiposComentarioActualizar.Count > 0)
            {
                int numTotales = pTiposComentarioActualizar.Count;
                int numComentContr = 0;
                int numComentMisRec = 0;
                int numComentBlog = 0;

                foreach (TipoLiveComentario tipoComent in pTiposComentarioActualizar)
                {
                    if (tipoComent == TipoLiveComentario.Contribuciones)
                    {
                        numComentContr++;
                    }
                    else if (tipoComent == TipoLiveComentario.MisRecursos)
                    {
                        numComentMisRec++;
                    }
                    else if (tipoComent == TipoLiveComentario.Blogs)
                    {
                        numComentBlog++;
                    }
                }
                ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
                if (contadorPerfil != null) 
                {
                    if (pDisminuir)
                    {//Restar
                        contadorPerfil.NumComentarios = contadorPerfil.NumComentarios - numTotales;
                        contadorPerfil.NumComentContribuciones = contadorPerfil.NumComentContribuciones - numComentContr;
                        contadorPerfil.NumComentMisRec = contadorPerfil.NumComentMisRec - numComentMisRec;
                        contadorPerfil.NumComentBlog = contadorPerfil.NumComentBlog - numComentBlog;
                    }
                    else
                    {//Sumar
                        contadorPerfil.NumComentarios = contadorPerfil.NumComentarios + numTotales;
                        contadorPerfil.NumComentContribuciones = contadorPerfil.NumComentContribuciones + numComentContr;
                        contadorPerfil.NumComentMisRec = contadorPerfil.NumComentMisRec + numComentMisRec;
                        contadorPerfil.NumComentBlog = contadorPerfil.NumComentBlog + numComentBlog;
                    }
                    mEntityContext.SaveChanges();
                }
            }
        }
        //No se usa en la Web
        public void ActualizarContadorPerfilMensajesSinLeer(Guid pPerfilID, int pMensajesSinLeer)
        {
            ContadorPerfil contadorPerfil = mEntityContext.ContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(pPerfilID));
            contadorPerfil.NumMensajesSinLeer = pMensajesSinLeer;
            mEntityContext.SaveChanges();
        }

        #endregion

        #endregion
    }
}
