using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Gestor de usuarios
    /// </summary>
    [Serializable]
    public class GestionUsuarios : GestionGnoss , ISerializable
    {
        #region Miembros

        /// <summary>
        /// Lista de Usuarios gnoss
        /// </summary>
        private SortedList<Guid, UsuarioGnoss> mListaUsuariosGnoss = null;

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestorIdentidades;

        /// <summary>
        /// Gestor de tesauro
        /// </summary>
        private GestionTesauro mGestorTesauro;

        /// <summary>
        /// Gestor documental
        /// </summary>
        private GestorDocumental mGestorDocumental;

        /// <summary>
        /// Gestor de suscripciones
        /// </summary>
        private GestionSuscripcion mGestorSuscripcion;

        /// <summary>
        /// Gestor de peticiones
        /// </summary>
        private GestionPeticiones mGestorPeticiones;

        /// <summary>
        /// Dataset de solicitudes del usuario
        /// </summary>
        private DataWrapperSolicitud mSolicitudDW;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        public GestionUsuarios() { }

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public GestionUsuarios( LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base()
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        protected GestionUsuarios(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;
            //mConfigService = configService;     

            mGestorDocumental = (GestorDocumental)pInfo.GetValue("GestorDocumental", typeof(GestorDocumental));
            mGestorIdentidades = (GestionIdentidades)pInfo.GetValue("GestorIdentidades", typeof(GestionIdentidades));
            mGestorTesauro = (GestionTesauro)pInfo.GetValue("GestorTesauro", typeof(GestionTesauro));
            mSolicitudDW = (DataWrapperSolicitud)pInfo.GetValue("SolicitudDW", typeof(DataWrapperSolicitud));
            mGestorSuscripcion = (GestionSuscripcion)pInfo.GetValue("GestionSuscripcion", typeof(GestionSuscripcion));
            mGestorPeticiones = (GestionPeticiones)pInfo.GetValue("GestorPeticiones", typeof(GestionPeticiones));
        }

        /// <summary>
        /// Constructor a partir del DataWrapperUsuario pasado por parámetro
        /// </summary>
        /// <param name="pDataWrapperUsuario">Dataset de usuarios</param>
        public GestionUsuarios(DataWrapperUsuario pDataWrapperUsuario,  LoggingService loggingService, EntityContext entityContext, ConfigService configService, ILogger<GestionUsuarios> logger, ILoggerFactory loggerFactory)
            : base(pDataWrapperUsuario)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;

            CargarUsuariosGnoss();
        }
        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el gestor de identidades
        /// </summary>
        public GestionIdentidades GestorIdentidades
        {
            get
            {
                return mGestorIdentidades;
            }
            set
            {
                mGestorIdentidades = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el DataWrapper de usuarios
        /// </summary>
        public DataWrapperUsuario DataWrapperUsuario
        {
            get
            {
                return (DataWrapperUsuario)this.DataWrapper;
            }
            set
            {
                DataWrapper = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de usuarios
        /// </summary>
        public SortedList<Guid, UsuarioGnoss> ListaUsuarios
        {
            get
            {
                if (mListaUsuariosGnoss == null)
                {
                    CargarUsuariosGnoss();
                }
                return mListaUsuariosGnoss;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de tesauro
        /// </summary>
        public GestionTesauro GestorTesauro
        {
            get
            {
                return this.mGestorTesauro;
            }
            set
            {
                this.mGestorTesauro = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de documentacion
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
            set
            {
                mGestorDocumental = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de suscripciones del usuario
        /// </summary>
        public GestionSuscripcion GestorSuscripciones
        {
            get
            {
                return mGestorSuscripcion;
            }
            set
            {
                mGestorSuscripcion = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de peticiones del usuario
        /// </summary>
        public GestionPeticiones GestorPeticiones
        {
            get
            {
                return mGestorPeticiones;
            }
            set
            {
                mGestorPeticiones = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el dataset de solicitudes del usuario
        /// </summary>
        public DataWrapperSolicitud SolicitudDW
        {
            get
            {
                return mSolicitudDW;
            }
            set
            {
                mSolicitudDW = value;
            }
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Agrega un usuario a un proyecto
        /// </summary>
        /// <param name="pFilaUsuario">Fila de usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organziación</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadID">Identificador de la identidad con la que va a participar el usuario en este proyecto</param>
        public void AgregarUsuarioAProyecto(AD.EntityModel.Models.UsuarioDS.Usuario pFilaUsuario, Guid pOrganizacionID, Guid pProyectoID, Guid pIdentidadID)
        {
            AgregarUsuarioAProyecto(pFilaUsuario, pOrganizacionID, pProyectoID, pIdentidadID, true);
        }

        /// <summary>
        /// Agrega un usuario a un proyecto
        /// </summary>
        /// <param name="pFilaUsuario">Fila de usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organziación</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadID">Identificador de la identidad con la que va a participar el usuario en este proyecto</param>
        /// <param name="pAgregarPermisos">Verdad si se deben de agrega el rol de permisos al proyecto</param>LoggingService.
        public void AgregarUsuarioAProyecto(AD.EntityModel.Models.UsuarioDS.Usuario pFilaUsuario, Guid pOrganizacionID, Guid pProyectoID, Guid pIdentidadID, bool pAgregarPermisos = true)
        {
            //ProyectoUsuarioIdentidad

            DateTime fechaEntrada = DateTime.Now;
            //.FindByIdentidadIDUsuarioIDOrganizacionGnossIDProyectoID(pIdentidadID, pFilaUsuario.UsuarioID, pOrganizacionID, pProyectoID)
            if (this.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.FirstOrDefault(proyUsIden=>proyUsIden.IdentidadID.Equals(pIdentidadID) && proyUsIden.UsuarioID.Equals(pFilaUsuario.UsuarioID) && proyUsIden.OrganizacionGnossID.Equals(pOrganizacionID) && proyUsIden.ProyectoID.Equals(pProyectoID)) == null)
            {
                AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad filaIdentidadUsuario = new AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad();
                filaIdentidadUsuario.UsuarioID = pFilaUsuario.UsuarioID;
                filaIdentidadUsuario.FechaEntrada = fechaEntrada;
                filaIdentidadUsuario.IdentidadID = pIdentidadID;
                filaIdentidadUsuario.OrganizacionGnossID = pOrganizacionID;
                filaIdentidadUsuario.ProyectoID = pProyectoID;
                filaIdentidadUsuario.Reputacion = 0;
                mEntityContext.ProyectoUsuarioIdentidad.Add(filaIdentidadUsuario);
                DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Add(filaIdentidadUsuario);
            }
            //FindByOrganizacionGnossIDProyectoIDUsuarioID(pOrganizacionID, pProyectoID, pFilaUsuario.UsuarioID);
            AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyectoRolUsuario = this.DataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs=>proyRolUs.OrganizacionGnossID.Equals(pOrganizacionID) && proyRolUs.ProyectoID.Equals(pProyectoID) && proyRolUs.UsuarioID.Equals(pFilaUsuario.UsuarioID));
            if (filaProyectoRolUsuario == null)
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                filaProyectoRolUsuario = usuarioCN.ObtenerRolUsuarioEnProyecto(pProyectoID, pFilaUsuario.UsuarioID);
            }
            if (pAgregarPermisos && filaProyectoRolUsuario == null)
            {
                AgregarProyectoRolUsuario(pFilaUsuario.UsuarioID, pOrganizacionID, pProyectoID);
            }
            else if(filaProyectoRolUsuario != null && filaProyectoRolUsuario.EstaBloqueado)
            {
                //Se le había bloqueado en el proyecto, le volvemos a aceptar
                filaProyectoRolUsuario.EstaBloqueado = false;
            }
            //Agregar el HistoricoProyectousuario
            AgregarHistoricoProyectoUsuario(pIdentidadID, pFilaUsuario.UsuarioID, pProyectoID, pOrganizacionID, fechaEntrada);
        }

        public void AgregarProyectoRolUsuario(Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID)
        {
            //ProyectoRolUsuario
            AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyectoRol = new AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario();

            filaProyectoRol.UsuarioID = pUsuarioID;
            filaProyectoRol.OrganizacionGnossID = pOrganizacionID;
            filaProyectoRol.ProyectoID = pProyectoID;
            filaProyectoRol.RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
            filaProyectoRol.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
            filaProyectoRol.EstaBloqueado = false;

            DataWrapperUsuario.ListaProyectoRolUsuario.Add(filaProyectoRol);
            mEntityContext.ProyectoRolUsuario.Add(filaProyectoRol);
        }

        /// <summary>
        /// Retoma la participación de un usuario en un proyecto retomando su identidad vieja
        /// </summary>
        /// <param name="pFilaUsuario">Fila de usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organziación</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadID">Identificador de la identidad que RETOMA el usuario en este proyecto</param>
        public void RetomarUsuarioEnProyecto(AD.EntityModel.Models.UsuarioDS.Usuario pFilaUsuario, Guid pOrganizacionID, Guid pProyectoID, Guid pIdentidadID)
        {
            if (DataWrapperUsuario.ListaProyectoRolUsuario.Count(proy=>proy.ProyectoID.Equals(pProyectoID)) == 0)
            {
                //Añado la fila de ProyectoRolusuario para ese usuario
                AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario proyectoRolUsuario = new AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario();
                proyectoRolUsuario.OrganizacionGnossID = pOrganizacionID;
                proyectoRolUsuario.ProyectoID = pProyectoID;
                proyectoRolUsuario.UsuarioID = pFilaUsuario.UsuarioID;
                proyectoRolUsuario.RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                proyectoRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                proyectoRolUsuario.EstaBloqueado = false;
                DataWrapperUsuario.ListaProyectoRolUsuario.Add(proyectoRolUsuario);
                mEntityContext.ProyectoRolUsuario.Add(proyectoRolUsuario);
            }

            //Actualizo la fecha de "Identidad"
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad;
            filaIdentidad = GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.IdentidadID.Equals(pIdentidadID)).FirstOrDefault();
            DateTime fechaActual = System.DateTime.Now;
            filaIdentidad.FechaAlta = fechaActual;
            filaIdentidad.FechaBaja = null;

            if (filaIdentidad.FechaExpulsion.HasValue)
            {
                filaIdentidad.FechaExpulsion = null;
            }
            //Creo la fila "ProyectoUsuarioIdentidad"
            AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad filaProyectoUsuarioIdentidad = new AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad();
            filaProyectoUsuarioIdentidad.IdentidadID = filaIdentidad.IdentidadID;
            filaProyectoUsuarioIdentidad.UsuarioID = pFilaUsuario.UsuarioID;
            filaProyectoUsuarioIdentidad.OrganizacionGnossID = pOrganizacionID;
            filaProyectoUsuarioIdentidad.ProyectoID = pProyectoID;
            filaProyectoUsuarioIdentidad.FechaEntrada = fechaActual;
            filaProyectoUsuarioIdentidad.Reputacion = 0;
            DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Add(filaProyectoUsuarioIdentidad);
            mEntityContext.ProyectoUsuarioIdentidad.Add(filaProyectoUsuarioIdentidad);

            //Creo fila en "HistoricoProyectoUsuario"
            AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario filaHistoricoProyectoUsuario = new AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario();
            filaHistoricoProyectoUsuario.IdentidadID = filaIdentidad.IdentidadID;
            filaHistoricoProyectoUsuario.UsuarioID = pFilaUsuario.UsuarioID;
            filaHistoricoProyectoUsuario.OrganizacionGnossID = pOrganizacionID;
            filaHistoricoProyectoUsuario.ProyectoID = pProyectoID;
            filaHistoricoProyectoUsuario.FechaEntrada = fechaActual;
            filaHistoricoProyectoUsuario.FechaSalida = null;

            DataWrapperUsuario.ListaHistoricoProyectoUsuario.Add(filaHistoricoProyectoUsuario);
        }

        /// <summary>
        /// Agrega el valor de las cláusulas adicionales del registro de un usuario en un proyecto.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organziación</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pClausulasAceptadas">Lista con el ID de las cláusulas aceptadas</param>
        public void AgregarClausulasAdicionalesRegistroProy(Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID, List<Guid> pClausulasAceptadas)
        {
            List<AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg> listaAuxiliarProyRolUsuClausulaReg = DataWrapperUsuario.ListaProyRolUsuClausulaReg;
            foreach (AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg filaClausulaRegUsu in listaAuxiliarProyRolUsuClausulaReg)
            {
                if (filaClausulaRegUsu.UsuarioID == pUsuarioID && filaClausulaRegUsu.OrganizacionID == pOrganizacionID && filaClausulaRegUsu.ProyectoID == pProyectoID)
                {
                    mEntityContext.ProyRolUsuClausulaReg.Remove(filaClausulaRegUsu);
                    DataWrapperUsuario.ListaProyRolUsuClausulaReg.Remove(filaClausulaRegUsu);
                }
            }

            List<AD.EntityModel.Models.UsuarioDS.ClausulaRegistro> listaAuxiliarClausulaRegistro = mEntityContext.ClausulaRegistro.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            foreach (AD.EntityModel.Models.UsuarioDS.ClausulaRegistro filaClausula in listaAuxiliarClausulaRegistro)
            {
                if (filaClausula.Tipo == (short)TipoClausulaAdicional.Opcional)
                {
                    AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario proyectoRolUsuario = new AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario();
                    proyectoRolUsuario = DataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(x => x.OrganizacionGnossID.Equals(pOrganizacionID) && x.ProyectoID.Equals(pProyectoID) && x.UsuarioID.Equals(pUsuarioID));

                    AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg proyRolUsuClausulaReg = new AD.EntityModel.Models.UsuarioDS.ProyRolUsuClausulaReg();
                    proyectoRolUsuario.ProyRolUsuClausulaReg.Add(proyRolUsuClausulaReg);
                    proyRolUsuClausulaReg.ClausulaRegistro = filaClausula;
                    proyRolUsuClausulaReg.ProyectoRolUsuario = proyectoRolUsuario;

                    filaClausula.ProyRolUsuClausulaReg.Add(proyRolUsuClausulaReg);
                    DataWrapperUsuario.ListaProyRolUsuClausulaReg.Add(proyRolUsuClausulaReg);
                    mEntityContext.ProyRolUsuClausulaReg.Add(proyRolUsuClausulaReg);

                    proyRolUsuClausulaReg.ClausulaID = filaClausula.ClausulaID;
                    proyRolUsuClausulaReg.OrganizacionID = pOrganizacionID;
                    proyRolUsuClausulaReg.ProyectoID = pProyectoID;
                    proyRolUsuClausulaReg.OrganizacionGnossID = pOrganizacionID;   
                    proyRolUsuClausulaReg.UsuarioID = pUsuarioID;
                    proyRolUsuClausulaReg.Valor = pClausulasAceptadas.Contains(filaClausula.ClausulaID);
                    

                }
            }
        }

        /// <summary>
        /// Agrega un usuario al DataSet
        /// </summary>
        /// <param name="pNombre">Login del usuario</param>
        /// <param name="pNombreCorto">Nombre corto del usuario</param>
        /// <param name="pContrasenia">Contraseña (Codificada)</param>
        public UsuarioGnoss AgregarUsuario(string pNombre, string pNombreCorto, string pContrasenia, bool RegistroAutomaticoEcosistema = true)
        {
            return AgregarUsuario(pNombre, pNombreCorto,pContrasenia, false, RegistroAutomaticoEcosistema);
        }

        /// <summary>
        /// Agrega un usuario al DataSet
        /// </summary>
        /// <param name="pNombre">Login del usuario</param>
        /// <param name="pNombreCorto">Nombre corto del usuario</param>
        /// <param name="pContrasenia">Contraseña (Codificada)</param>
        /// <param name="pCrearSoloFilaUsuario">Verdad si solo se desea crear la fila del usuario 
        /// (para un registro en el que el usuario debe de ser validado previamente para que pueda acceder a GNOSS. 
        /// El usuario estará bloqueado inicialmente.)</param>
        public UsuarioGnoss AgregarUsuario(string pNombre, string pNombreCorto, string pContrasenia, bool pCrearSoloFilaUsuario, bool RegistroAutomaticoEcosistema = true)
        {
            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = new AD.EntityModel.Models.UsuarioDS.Usuario();

            filaUsuario.EstaBloqueado = pCrearSoloFilaUsuario;
            filaUsuario.Login = pNombre;
            filaUsuario.NombreCorto = pNombreCorto;
            filaUsuario.Password = pContrasenia;
            filaUsuario.UsuarioID = Guid.NewGuid();
            filaUsuario.Version = 1;
            if (RegistroAutomaticoEcosistema)
            {
                filaUsuario.Validado = (short)ValidacionUsuario.Verificado;
            }
            else
            {
                filaUsuario.Validado = (short)ValidacionUsuario.SinVerificar;
            }

            this.DataWrapperUsuario.ListaUsuario.Add(filaUsuario);
            mEntityContext.Usuario.Add(filaUsuario);
            UsuarioGnoss usuario = new UsuarioGnoss(filaUsuario, this);
            mListaUsuariosGnoss.Add(usuario.Clave, usuario);

            if (!pCrearSoloFilaUsuario)
            {
                CompletarUsuarioNuevo(filaUsuario);
            }
            return usuario;
        }

        /// <summary>
        /// Termina de crear el resto de filas necesarias para un usuario de GNOSS (BR, Tesauro, GeneralRolUsuario)
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario nuevo</param>
        public void CompletarUsuarioNuevo(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            CompletarUsuarioNuevo(pUsuario, GestionTesauro.NOMBRE_CATEGORIA_PUBLICA, GestionTesauro.NOMBRE_CATEGORIA_PRIVADA);
        }

        /// <summary>
        /// Termina de crear el resto de filas necesarias para un usuario de GNOSS (BR, Tesauro, GeneralRolUsuario)
        /// </summary>
        /// <param name="pUsuario">Identificador del usuario nuevo</param>
        /// <param name="pNombreCategoriaPrivada">Nombre de la categoría del tesauro privada (En el idioma correspondiente)</param>
        /// <param name="pNombreCategoriaPublica">Nombre de la categoría del tesauro pública (En el idioma correspondiente)</param>
        public void CompletarUsuarioNuevo(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, string pNombreCategoriaPublica, string pNombreCategoriaPrivada)
        {
            // Si ya existe la fila General Rol Usuario, significa que ya ha entrado por aquí y ha creado estas filas
            if (!DataWrapperUsuario.ListaGeneralRolUsuario.Any(item => item.UsuarioID.Equals(pUsuario.UsuarioID)))
            {
                //GeneralRolUsuario
                AD.EntityModel.Models.UsuarioDS.GeneralRolUsuario filaGeneralRol = new AD.EntityModel.Models.UsuarioDS.GeneralRolUsuario();
                filaGeneralRol.UsuarioID = pUsuario.UsuarioID;
                filaGeneralRol.RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                filaGeneralRol.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;

                DataWrapperUsuario.ListaGeneralRolUsuario.Add(filaGeneralRol);
                mEntityContext.GeneralRolUsuario.Add(filaGeneralRol);

                AD.EntityModel.Models.UsuarioDS.UsuarioContadores filaContadores = new AD.EntityModel.Models.UsuarioDS.UsuarioContadores();
                filaContadores.UsuarioID = pUsuario.UsuarioID;
                filaContadores.NumeroAccesos = 0;
                filaContadores.FechaUltimaVisita = DateTime.Now;
                DataWrapperUsuario.ListaUsuarioContadores.Add(filaContadores);
                mEntityContext.UsuarioContadores.Add(filaContadores);

                if (GestorTesauro == null)
                {
                    GestorTesauro = new GestionTesauro(new DataWrapperTesauro(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                }
                GestorTesauro.AgregarTesauroUsuario(pUsuario.UsuarioID, pNombreCategoriaPublica, pNombreCategoriaPrivada);

                if (GestorDocumental == null)
                {
                    GestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
                }
                GestorDocumental.AgregarBRDeUsuario(pUsuario);
            }
        }

        /// <summary>
        /// Cambia el inicio de sesión de un usuario
        /// </summary>
        /// <param name="pFilaInicioSesion">Fila de inicio de sesión</param>
        /// <param name="pOrganizacionGnossID">Identificador de organización Gnoss</param>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Fila de inicio de sesión modificada</returns>
        public AD.EntityModel.Models.UsuarioDS.InicioSesion CambiarInicioSesionUsuario(AD.EntityModel.Models.UsuarioDS.InicioSesion pFilaInicioSesion, Guid pOrganizacionGnossID, Guid pPersonaID, Guid pProyectoID, Guid pUsuarioID)
        {
            if (pFilaInicioSesion != null)
            {
                pFilaInicioSesion.OrganizacionGnossID = null;
                pFilaInicioSesion.PersonaID = null;
                pFilaInicioSesion.ProyectoID = null;
            }

            //Si no hay inicio de sesión lo creo
            else if(DataWrapperUsuario != null)
            {
                pFilaInicioSesion = new AD.EntityModel.Models.UsuarioDS.InicioSesion();
                pFilaInicioSesion.UsuarioID = pUsuarioID;
                pFilaInicioSesion.OrganizacionGnossID = null;
                pFilaInicioSesion.PersonaID = null;
                pFilaInicioSesion.ProyectoID = null;
                DataWrapperUsuario.ListaInicioSesion.Add(pFilaInicioSesion);
            }

            if (!pOrganizacionGnossID.Equals(Guid.Empty))
            {
                pFilaInicioSesion.OrganizacionGnossID = pOrganizacionGnossID;
            }

            if (!pPersonaID.Equals(Guid.Empty))
            {
                pFilaInicioSesion.PersonaID = pPersonaID;
            }

            if (!pProyectoID.Equals(Guid.Empty))
            {
                pFilaInicioSesion.ProyectoID = pProyectoID;
            }
            return pFilaInicioSesion;
        }

        /// <summary>
        /// Carga los usuarios gnoss en la lista
        /// </summary>
        private void CargarUsuariosGnoss()
        {
            mListaUsuariosGnoss = new SortedList<Guid, UsuarioGnoss>();
            if(DataWrapperUsuario != null)//Migrar EF
            {
                foreach (AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario in DataWrapperUsuario.ListaUsuario)
                {
                    UsuarioGnoss usuario = new UsuarioGnoss(filaUsuario, this);

                    if (!mListaUsuariosGnoss.ContainsKey(filaUsuario.UsuarioID))
                    {
                        mListaUsuariosGnoss.Add(usuario.Clave, usuario);
                    }
                }
            }
            
        }

        /// <summary>
        /// Agrega al usuario pasado por parámetro como administrador general
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void AgregarAdministradorGeneral(Guid pUsuarioID)
        {
            AD.EntityModel.Models.UsuarioDS.AdministradorGeneral filaAdminGen = new AD.EntityModel.Models.UsuarioDS.AdministradorGeneral();
            filaAdminGen.UsuarioID = pUsuarioID;
            DataWrapperUsuario.ListaAdministradorGeneral.Add(filaAdminGen);
            mEntityContext.AdministradorGeneral.Add(filaAdminGen);

            //Actualizo los permisos generales del usuario
            AD.EntityModel.Models.UsuarioDS.GeneralRolUsuario filaGeneralRolUsuario = this.DataWrapperUsuario.ListaGeneralRolUsuario.FirstOrDefault(generalRolUs=>generalRolUs.UsuarioID.Equals(pUsuarioID));
            string RolPermitido = filaGeneralRolUsuario.RolPermitido;
            string RolDenegado;

            if (filaGeneralRolUsuario.RolDenegado == null)
            {
                RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
            }
            else
            {
                RolDenegado = filaGeneralRolUsuario.RolDenegado;
            }
            //Le doy todos los permisos
            RolPermitido = UsuarioAD.FilaPermisosAdministrador;

            //No le deniego ninguno
            RolDenegado = UsuarioAD.FilaPermisosSinDefinir;

            if (RolPermitido != filaGeneralRolUsuario.RolPermitido)
            {
                filaGeneralRolUsuario.RolPermitido = RolPermitido;
            }

            if (filaGeneralRolUsuario.RolDenegado == null)
            {
                filaGeneralRolUsuario.RolDenegado = RolDenegado;
            }
            else
            {
                filaGeneralRolUsuario.RolDenegado = RolDenegado;
            }
        }

        /// <summary>
        /// Elimina al usuario pasado por parámetro como administrador general
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void EliminarAdministradorGeneral(Guid pUsuarioID)
        {
            AD.EntityModel.Models.UsuarioDS.AdministradorGeneral filaAdminGen = DataWrapperUsuario.ListaAdministradorGeneral.FirstOrDefault(adminGen => adminGen.UsuarioID.Equals(pUsuarioID));
            
            if (filaAdminGen != null)
            {
                DataWrapperUsuario.ListaAdministradorGeneral.Remove(filaAdminGen);
                mEntityContext.EliminarElemento(filaAdminGen);

                //Le actualizo los permisos generales
                AD.EntityModel.Models.UsuarioDS.GeneralRolUsuario filaGeneralRolUsuario = this.DataWrapperUsuario.ListaGeneralRolUsuario.FirstOrDefault(genRolUs => genRolUs.UsuarioID.Equals(pUsuarioID));
                string RolPermitido = filaGeneralRolUsuario.RolPermitido;
                string RolDenegado;

                if (filaGeneralRolUsuario.RolDenegado == null)
                {
                    RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                }
                else
                {
                    RolDenegado = filaGeneralRolUsuario.RolDenegado;
                }
                //Le quito todos los permisos
                RolPermitido = UsuarioAD.FilaPermisosSinDefinir;

                //No le deniego ninguno
                RolDenegado = UsuarioAD.FilaPermisosSinDefinir;

                if (RolPermitido != filaGeneralRolUsuario.RolPermitido)
                {
                    filaGeneralRolUsuario.RolPermitido = RolPermitido;
                }

                if (filaGeneralRolUsuario.RolDenegado == null)
                {
                    filaGeneralRolUsuario.RolDenegado = RolDenegado;
                }
                else
                {
                    filaGeneralRolUsuario.RolDenegado = RolDenegado;
                }
            }
        }

        /// <summary>
        /// Crea el rol de un usuario en una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        public void AgregarOrganizacionRolUsuario(Guid pUsuarioID, Guid pOrganizacionID)
        {
            if (!mEntityContext.OrganizacionRolUsuario.Any(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)))
            {
                AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario filaOrganizacionRolUsuario = new AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario();
                filaOrganizacionRolUsuario.UsuarioID = pUsuarioID;
                filaOrganizacionRolUsuario.OrganizacionID = pOrganizacionID;
                filaOrganizacionRolUsuario.RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                filaOrganizacionRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;
                DataWrapperUsuario.ListaOrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
                mEntityContext.OrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
            }
            else if (!DataWrapperUsuario.ListaOrganizacionRolUsuario.Any(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)))
            {
                AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario filaOrganizacionRolUsuario = new AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario();
                filaOrganizacionRolUsuario.UsuarioID = pUsuarioID;
                filaOrganizacionRolUsuario.OrganizacionID = pOrganizacionID;
                filaOrganizacionRolUsuario.RolPermitido = UsuarioAD.FilaPermisosSinDefinir;
                filaOrganizacionRolUsuario.RolDenegado = UsuarioAD.FilaPermisosSinDefinir;

                DataWrapperUsuario.ListaOrganizacionRolUsuario.Add(filaOrganizacionRolUsuario);
            }
        }

        /// <summary>
        /// Recarga los usuarios
        /// </summary>
        public void RecargarUsuarios()
        {
            mListaUsuariosGnoss.Clear();
            CargarUsuariosGnoss();
        }

        /// <summary>
        /// Crea la identidad de un usuario en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacionGnossID">Identificador de organización Gnoss</param>
        /// <param name="pFechaActual">Fecha actual</param>
        public void AgregarProyectoUsuarioIdentidad(Guid pIdentidadID, Guid pUsuarioID, Guid pProyectoID, Guid pOrganizacionGnossID, DateTime pFechaActual)
        {
            AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad filaProyectoUsuarioIdentidad = new AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad();
            filaProyectoUsuarioIdentidad.IdentidadID = pIdentidadID;
            filaProyectoUsuarioIdentidad.UsuarioID = pUsuarioID;
            filaProyectoUsuarioIdentidad.OrganizacionGnossID = pOrganizacionGnossID;
            filaProyectoUsuarioIdentidad.ProyectoID = pProyectoID;
            filaProyectoUsuarioIdentidad.FechaEntrada = pFechaActual;
            filaProyectoUsuarioIdentidad.Reputacion = 0;

            this.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Add(filaProyectoUsuarioIdentidad);
            mEntityContext.ProyectoUsuarioIdentidad.Add(filaProyectoUsuarioIdentidad);
        }

        /// <summary>
        /// Agrega una fila de histórico del usuario en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacionGnossID">Identificador de organización Gnoss</param>
        /// <param name="pFechaActual">Fecha actual</param>
        public void AgregarHistoricoProyectoUsuario(Guid pIdentidadID, Guid pUsuarioID, Guid pProyectoID, Guid pOrganizacionGnossID, DateTime pFechaActual)
        {
            AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario filaHistoricoProyectoUsuario = new AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario();
            filaHistoricoProyectoUsuario.IdentidadID = pIdentidadID;
            filaHistoricoProyectoUsuario.UsuarioID = pUsuarioID;
            filaHistoricoProyectoUsuario.OrganizacionGnossID = pOrganizacionGnossID;
            filaHistoricoProyectoUsuario.ProyectoID = pProyectoID;
            filaHistoricoProyectoUsuario.FechaEntrada = pFechaActual;
            filaHistoricoProyectoUsuario.FechaSalida = null;

            this.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Add(filaHistoricoProyectoUsuario);
            mEntityContext.HistoricoProyectoUsuario.Add(filaHistoricoProyectoUsuario);
        }

        /// <summary>
        /// Elimina de forma física un usuario del sistema
        /// </summary>
        /// <param name="pElemento">Elemento</param>
        public override void EliminarElemento(ElementoGnoss pElemento)
        {
            UsuarioGnoss usuarioBorrar = (UsuarioGnoss)pElemento;

            // Borrar GeneralRolUsuario
            //if (usuarioBorrar.FilaUsuario.GeneralRolUsuario != null)
            //{
            //    mEntityContext.Entry(usuarioBorrar.FilaUsuario.GeneralRolUsuario).State = EntityState.Deleted;
            //    usuarioBorrar.FilaUsuario.GeneralRolUsuario = null;
            //}

            // OrganizacionRolUsuario
            List<AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario> filasRolBorrar = usuarioBorrar.FilaUsuario.OrganizacionRolUsuario.ToList();
            foreach (AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario fila in filasRolBorrar)
            {
                mEntityContext.EliminarElemento(fila);
            }
            usuarioBorrar.FilaUsuario.OrganizacionRolUsuario = new HashSet<AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario>();

            // GrupoUsuarioUsuario
            foreach (AD.EntityModel.Models.UsuarioDS.GrupoUsuarioUsuario fila in usuarioBorrar.FilaUsuario.GrupoUsuarioUsuario)
            {
                mEntityContext.EliminarElemento(fila);
            }
            usuarioBorrar.FilaUsuario.GrupoUsuarioUsuario = new HashSet<AD.EntityModel.Models.UsuarioDS.GrupoUsuarioUsuario>();

            // AdministradorGeneral
            //if (usuarioBorrar.FilaUsuario.AdministradorGeneral != null)
            //{
            //    mEntityContext.Entry(usuarioBorrar.FilaUsuario.AdministradorGeneral).State = EntityState.Deleted;
            //}
            //usuarioBorrar.FilaUsuario.AdministradorGeneral = null;

            // ProyectoRolUsuario
            foreach (AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario fila in usuarioBorrar.FilaUsuario.ProyectoRolUsuario)
            {
                mEntityContext.EliminarElemento(fila);
            }
            usuarioBorrar.FilaUsuario.ProyectoRolUsuario = new HashSet<AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario>();

            // ProyectoUsuarioIdentidad
            //Guardo las identidades del usuario en una lista para mas adelante eliminar todas las suscripciones que tenga
            List<Guid> listaDeIdentidadesDelUsuario = new List<Guid>();
            
            foreach (AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad fila in usuarioBorrar.FilaUsuario.ProyectoUsuarioIdentidad)
            {
                // Por cada identidad en proyecto eliminada, se guarda la fecha de baja en la fila de historico
                // en caso de no existir esta, se crea una nueva
                AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario filaHistorico = DataWrapperUsuario.ListaHistoricoProyectoUsuario.Where(item => item.UsuarioID.Equals(fila.UsuarioID) && item.OrganizacionGnossID.Equals(fila.OrganizacionGnossID) && item.ProyectoID.Equals(fila.ProyectoID) && item.IdentidadID.Equals(fila.IdentidadID) && item.FechaEntrada.Equals(fila.FechaEntrada.Value)).FirstOrDefault();
                
                if (filaHistorico == null)
                {
                    filaHistorico = new AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario();
                    filaHistorico.IdentidadID = fila.IdentidadID;
                    filaHistorico.UsuarioID = fila.UsuarioID;
                    filaHistorico.OrganizacionGnossID = fila.OrganizacionGnossID;
                    filaHistorico.ProyectoID = fila.ProyectoID;
                    filaHistorico.FechaEntrada = fila.FechaEntrada.Value;

                    DataWrapperUsuario.ListaHistoricoProyectoUsuario.Add(filaHistorico);
                    mEntityContext.HistoricoProyectoUsuario.Add(filaHistorico);
                }
                filaHistorico.FechaSalida = DateTime.Now;

                if (!listaDeIdentidadesDelUsuario.Contains(fila.IdentidadID))
                {
                    listaDeIdentidadesDelUsuario.Add(fila.IdentidadID);
                }
                mEntityContext.EliminarElemento(fila);
            }
            usuarioBorrar.FilaUsuario.ProyectoUsuarioIdentidad = new HashSet<AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad>();
            // InicioSesion
            //if (usuarioBorrar.FilaUsuario.InicioSesion != null)
            //{
            //    mEntityContext.Entry(usuarioBorrar.FilaUsuario.InicioSesion).State = EntityState.Deleted;
            //}
            //usuarioBorrar.FilaUsuario.InicioSesion = null;

            // Eliminar las peticiones
            if (GestorPeticiones != null && GestorPeticiones.PeticionDW != null)
            {
                foreach (AD.EntityModel.Models.Peticion.Peticion filaPeticion in GestorPeticiones.PeticionDW.ListaPeticion.Where(item => item.UsuarioID.Equals(usuarioBorrar.Clave)).ToList())
                {
                    //Eliminamos las filas de petición de invitación a comunidad relacionadas
                    foreach(AD.EntityModel.Models.Peticion.PeticionInvitacionComunidad filaPeticionInvitaComunidad in GestorPeticiones.PeticionDW.ListaPeticionInvitacionComunidad.Where(item => item.PeticionID.Equals(filaPeticion.PeticionID)).ToList())
                    {
                        mEntityContext.EliminarElemento(filaPeticionInvitaComunidad);
                        GestorPeticiones.PeticionDW.ListaPeticionInvitacionComunidad.Remove(filaPeticionInvitaComunidad);
                    }

                    //Eliminamos las filas de petición de invitación a organización relacionadas
                    foreach (AD.EntityModel.Models.Peticion.PeticionOrgInvitaPers filaPeticionInvitaOrg in GestorPeticiones.PeticionDW.ListaPeticionOrgInvitaPers.Where(item => item.PeticionID.Equals(filaPeticion.PeticionID)).ToList())
                    {
                        mEntityContext.EliminarElemento(filaPeticionInvitaOrg);
                        GestorPeticiones.PeticionDW.ListaPeticionOrgInvitaPers.Remove(filaPeticionInvitaOrg);
                    }

                    //Eliminamos la fila de petición
                    mEntityContext.EliminarElemento(filaPeticion);
                    GestorPeticiones.PeticionDW.ListaPeticion.Remove(filaPeticion);
                }
            }

            // Eliminar la base de recursos del usuario y los documentos web vinculados a la misma
            if (GestorDocumental != null)
            {
                GestorDocumental.EliminarBaseRecursos(usuarioBorrar.BaseRecursosID);
            }

            // Eliminar el tesauro del usuario
            if (GestorTesauro != null)
            {
                GestorTesauro.EliminarTesauro(usuarioBorrar.TesauroID);
            }

            //Eliminar las solicitudes
            if (mSolicitudDW != null)
            {
                // Eliminar SolicitudUsuario
                foreach(SolicitudUsuario fila in mSolicitudDW.ListaSolicitudUsuario.Where(item => item.UsuarioID.Equals(usuarioBorrar.Clave)).ToList())
                {
                    //Eliminamos la fila de solicitud relacionada
                    Solicitud solicitudEliminar = mSolicitudDW.ListaSolicitud.Where(item => item.SolicitudID.Equals(fila.SolicitudID)).FirstOrDefault();

                    mSolicitudDW.ListaSolicitud.Remove(solicitudEliminar);
                    mEntityContext.EliminarElemento(solicitudEliminar);

                    mSolicitudDW.ListaSolicitudUsuario.Remove(fila);
                    mEntityContext.EliminarElemento(fila);
                }

                // Eliminar SolicitudNuevoUsuario
                foreach (SolicitudNuevoUsuario fila in mSolicitudDW.ListaSolicitudNuevoUsuario.Where(item => item.UsuarioID.Equals(usuarioBorrar.Clave)).ToList())
                {
                    //Eliminamos la fila de solicitud relacionada
                    Solicitud solicitudEliminar = mSolicitudDW.ListaSolicitud.Where(item => item.SolicitudID.Equals(fila.SolicitudID)).FirstOrDefault();

                    mSolicitudDW.ListaSolicitud.Remove(solicitudEliminar);
                    mEntityContext.EliminarElemento(solicitudEliminar);
                    
                    mSolicitudDW.ListaSolicitudNuevoUsuario.Remove(fila);
                    mEntityContext.EliminarElemento(fila);
                }

                // Eliminar SolicitudNuevaOrganizacion
                foreach (SolicitudNuevaOrganizacion fila in mSolicitudDW.ListaSolicitudNuevaOrganizacion.Where(item => item.UsuarioAdminID.Equals(usuarioBorrar.Clave)).ToList())
                {
                    //Eliminamos la fila de solicitud relacionada
                    Solicitud solicitudEliminar = mSolicitudDW.ListaSolicitud.Where(item => item.SolicitudID.Equals(fila.SolicitudID)).FirstOrDefault();

                    mSolicitudDW.ListaSolicitud.Remove(solicitudEliminar);
                    mEntityContext.EliminarElemento(solicitudEliminar);

                    mSolicitudDW.ListaSolicitudNuevaOrganizacion.Remove(fila);
                    mEntityContext.EliminarElemento(fila);
                }
            }

            // Eliminar las suscripciones
            if (mGestorSuscripcion != null && mGestorSuscripcion.SuscripcionDW != null && mGestorSuscripcion.GestorNotificaciones != null && mGestorSuscripcion.GestorNotificaciones.NotificacionDW != null)
            {
                foreach (Guid identidadID in listaDeIdentidadesDelUsuario)
                {
                    mGestorSuscripcion.EliminarSuscripciones(identidadID);
                }
            }
            listaDeIdentidadesDelUsuario.Clear();
            ListaUsuarios.Remove(usuarioBorrar.Clave);
            mEntityContext.EliminarElemento(usuarioBorrar.FilaUsuario);

            base.EliminarElemento(pElemento);
        }

        /// <summary>
        /// Elimina/actualiza de UsuarioDS, de las tablas "ProyectoRolUsuario", "ProyectoUsuarioIdentidad", "HistoricoProyectoUsuario",
        /// "GrupoUsuarioUsuario", "InicioSesion" para cada uno de los proyectos donde el usuario participe con pPerfilID. 
        /// NOTA: Para el proyecto MYGNOSS no se borra "ProyectoRolUsuario"
        /// </summary>
        /// <param name="pPerfilID">Identificador de uno de los perfiles de usuario</param>
        /// <param name="pPersona">Objeto persona vinculado al usuario</param>
        public void EliminarAccesoTodosProyectosDePerfil(Guid pPerfilID, Persona pPersona)
        {
            Guid usuarioID = pPersona.UsuarioID;
            Guid identidadID;
            Guid proyectoID;
            Guid organizacionID;

            //Obtenemos el metaproyecto
            ProyectoCN proyCN = new ProyectoCN( mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Guid? metaProyectoID = proyCN.ObtenerMetaProyectoID();
            proyCN.Dispose();
            
            foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in this.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad=>identidad.PerfilID.Equals(pPerfilID)).ToList())
            {
                identidadID = filaIdentidad.IdentidadID;
                proyectoID = filaIdentidad.ProyectoID;
                organizacionID = filaIdentidad.OrganizacionID;

                //Elimino ProyectorRolUsuario
                //NOTA: No se borran los permisos en el metaproyecto ya que puede estar con otra identidad, otro perfil
                if (proyectoID != metaProyectoID)
                {
                    AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario proyectoRolUsuario = this.DataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(proyRolUsuario => proyRolUsuario.OrganizacionGnossID.Equals(organizacionID) && proyRolUsuario.ProyectoID.Equals(proyectoID) && proyRolUsuario.UsuarioID.Equals(usuarioID));
                    if (proyectoRolUsuario != null)
                    {
                        this.DataWrapperUsuario.ListaProyectoRolUsuario.Remove(proyectoRolUsuario);
                        mEntityContext.EliminarElemento(proyectoRolUsuario);
                    }
                }

                //Elimino de ProyectoUsuarioIdentidad
                AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad proyectoUsuadioIdentidad = this.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.FirstOrDefault(proyUsIden => proyUsIden.IdentidadID.Equals(identidadID) && proyUsIden.UsuarioID.Equals(usuarioID) && proyUsIden.OrganizacionGnossID.Equals(organizacionID) && proyUsIden.ProyectoID.Equals(proyectoID));
                if (proyectoUsuadioIdentidad != null)
                {
                    this.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Remove(proyectoUsuadioIdentidad);
                    mEntityContext.EliminarElemento(proyectoUsuadioIdentidad);
                }

                //Si su InicioSesion erá a este proyecto lo borro
                if (this.DataWrapperUsuario.ListaInicioSesion.Where(item => item.UsuarioID.Equals(usuarioID) && item.ProyectoID.Equals(proyectoID)).Count() > 0)
                {
                    AD.EntityModel.Models.UsuarioDS.InicioSesion inicioSesionEliminar = this.DataWrapperUsuario.ListaInicioSesion.Where(item => item.UsuarioID.Equals(usuarioID) && item.ProyectoID.Equals(proyectoID)).FirstOrDefault();

                    mEntityContext.EliminarElemento(inicioSesionEliminar);
                    DataWrapperUsuario.ListaInicioSesion.Remove(inicioSesionEliminar);
                }

                //Elimino la relación con los grupos de permisos del proyecto que pueda tener
                foreach (AD.EntityModel.Models.UsuarioDS.ProyectoRolGrupoUsuario filaProyectoRolGrupoUsuario in this.DataWrapperUsuario.ListaProyectoRolGrupoUsuario.Where(item => item.ProyectoID.Equals(proyectoID)))
                {
                    Guid GrupoUsuarioID = filaProyectoRolGrupoUsuario.GrupoUsuarioID;

                    AD.EntityModel.Models.UsuarioDS.GrupoUsuarioUsuario grupoUsuarioUsuarioEliminar = this.DataWrapperUsuario.ListaGrupoUsuarioUsuario.Where(item => item.UsuarioID.Equals(usuarioID) && item.GrupoUsuarioID.Equals(GrupoUsuarioID)).FirstOrDefault();
                    if (grupoUsuarioUsuarioEliminar != null)
                    {
                        mEntityContext.EliminarElemento(grupoUsuarioUsuarioEliminar);
                        this.DataWrapperUsuario.ListaGrupoUsuarioUsuario.Remove(grupoUsuarioUsuarioEliminar);
                    }
                }
                //Actualizo el HistoricoProyectoUsuario
                DateTime fechaSalida = DateTime.Now;

                AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario historicoProyectoUsuario = this.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Where(item => item.UsuarioID.Equals(usuarioID) && item.OrganizacionGnossID.Equals(organizacionID) && item.ProyectoID.Equals(proyectoID) && item.IdentidadID.Equals(identidadID) && !item.FechaSalida.HasValue).FirstOrDefault();

                if (historicoProyectoUsuario != null)
                {
                    AD.EntityModel.Models.UsuarioDS.HistoricoProyectoUsuario filaHistoricoProyectoUsuario = historicoProyectoUsuario;
                    filaHistoricoProyectoUsuario.FechaSalida = fechaSalida;
                }
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("GestorDocumental", GestorDocumental);
            info.AddValue("GestorIdentidades", GestorIdentidades);
            info.AddValue("GestorTesauro", GestorTesauro);
            info.AddValue("SolicitudDW", mSolicitudDW);
            info.AddValue("GestionSuscripcion", GestorSuscripciones);
            info.AddValue("GestorPeticiones", GestorPeticiones);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~GestionUsuarios()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool pDisposing)
        {
            if (!this.mDisposed)
            {
                mDisposed = true;

                try
                {
                    if (pDisposing)
                    {
                        //Liberamos todos los recursos administrados que hemos añadido a esta clase
                        foreach (UsuarioGnoss usuario in ListaUsuarios.Values)
                        {
                            usuario.Dispose();
                        }
                    }
                }
                finally
                {
                    mGestorIdentidades = null;
                    mGestorTesauro = null;
                    mGestorDocumental = null;
                    mGestorSuscripcion = null;
                    mGestorPeticiones = null;
                    mSolicitudDW = null;

                    // Llamamos al dispose de la clase base
                    base.Dispose(pDisposing);
                }
            }
        }

        #endregion
    }
}
