using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Principal;

namespace Es.Riam.Gnoss.Util.Seguridad
{
    /// <summary>
	/// Representa la identidad del usuario que ha iniciado sesión y se ha autenticado
	/// </summary>
    [Serializable]
    public class GnossIdentity : IIdentity, ISerializable
    {
        #region Miembros

        private Guid usuarioID;
        private Guid identidadID;
        private Guid organizacionID;
        private Guid personaID;
        private Guid proyectoID;
        private string login;
        private ulong rolPermitidoGeneral;
        private ulong rolPermitidoProyecto;
        private bool mEstaBloqueadoEnProyecto;
        private bool estaPasswordAutenticada;
        private bool mEsUsuarioInvitado = false;
        private bool mEsIdentidadInvitada = false;
        private Guid? mPerfilProfesorID = null;
        private string mNombrePersona = String.Empty;
        private string mApellidosPersona = String.Empty;
        private SortedList<Guid, ulong> mListaRolesOrganizacion = new SortedList<Guid, ulong>();
        private short tipoAcceso;
        private short tipoProyecto;

        /// <summary>
        /// Identificador del perfil actual.
        /// </summary>
        private Guid perfilID;

        /// <summary>
        /// Fecha en la que el usuario hizo el último guardado (por defecto null)
        /// </summary>
        private DateTime? mFechaUltimoGuardado = null;

        /// <summary>
        /// Tiempo de consulta a la base de datos maestra después de un guardado (por defecto 60 segundos)
        /// </summary>
        private static int? mTiempoConsultaMaster = null;

        /// <summary>
        /// Idioma del usuario.
        /// </summary>
        private string mIdioma;

        #endregion
        
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public GnossIdentity()
        {

        }

        /// <summary>
        /// Constructor completo para GnossIdentity
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pLogin">Login del usuario</param>
        /// <param name="pRolPermitido">Rol permitido</param>
        public GnossIdentity(System.Guid pUsuarioID, string pLogin, ulong pRolPermitido, ConfigService configService)
        {
            this.usuarioID = pUsuarioID;
            this.login = pLogin;
            this.rolPermitidoGeneral = pRolPermitido;

        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GnossIdentity(SerializationInfo info, StreamingContext context, Conexion conexion)
        {
            estaPasswordAutenticada = (bool)info.GetValue("estaPasswordAutenticada", typeof(bool));
            identidadID = (Guid)info.GetValue("identidadID", typeof(Guid));
            login = (string)info.GetValue("login", typeof(string));
            mApellidosPersona = (string)info.GetValue("mApellidosPersona", typeof(string));
            mEsIdentidadInvitada = (bool)info.GetValue("mEsIdentidadInvitada", typeof(bool));
            mEsUsuarioInvitado = (bool)info.GetValue("mEsUsuarioInvitado", typeof(bool));
            mPerfilProfesorID = (Guid?)info.GetValue("mPerfilProfesorID", typeof(Guid?));
            mListaRolesOrganizacion = (SortedList<Guid, ulong>)info.GetValue("mListaRolesOrganizacion", typeof(SortedList<Guid, ulong>));
            mNombrePersona = (string)info.GetValue("mNombrePersona", typeof(string));
            organizacionID = (Guid)info.GetValue("organizacionID", typeof(Guid));
            personaID = (Guid)info.GetValue("personaID", typeof(Guid));
            proyectoID = (Guid)info.GetValue("proyectoID", typeof(Guid));
            rolPermitidoGeneral = (ulong)info.GetValue("rolPermitidoGeneral", typeof(ulong));
            rolPermitidoProyecto = (ulong)info.GetValue("rolPermitidoProyecto", typeof(ulong));
            tipoAcceso = (short)info.GetValue("estaPasswordAutenticada", typeof(short));
            tipoProyecto = (short)info.GetValue("tipoProyecto", typeof(short));
            usuarioID = (Guid)info.GetValue("usuarioID", typeof(Guid));
            perfilID = (Guid)info.GetValue("perfilID", typeof(Guid));
            mFechaUltimoGuardado = (DateTime)info.GetValue("FechaUltimoGuardado", typeof(DateTime));
            FechaCambioPassword = (DateTime?)info.GetValue("FechaCambioPassword", typeof(DateTime?));

            //TODO JUAN: Quitar el try catch en la siguiente versión (hecho en la versión 2.3.1000)
            try
            {
                UltimaUrlVisitada = (string)info.GetValue("UltimaUrlVisitada", typeof(string));
            }
            catch { }

            if (mFechaUltimoGuardado.Value.Equals(DateTime.MinValue))
            {
                mFechaUltimoGuardado = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece la lista de roles en la organización
        /// </summary>
        public SortedList<Guid, ulong> ListaRolesOrganizacion
        {
            get
            {
                return mListaRolesOrganizacion;
            }
            set
            {
                mListaRolesOrganizacion = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de acceso del proyecto al que se conecta el usuario (público,privado,etc...)
        /// </summary>
        public short TipoAcceso
        {
            get
            {
                return tipoAcceso;
            }
            set
            {
                tipoAcceso = value;
            }
        }

        /// <summary>
        ///  Obtiene o establece el tipo de proyecto al que se conecta el usuario (Comunidad, Proyecto de organización, etc...)
        /// </summary>
        public short TipoProyecto
        {
            get
            {
                return tipoProyecto;
            }
            set
            {
                tipoProyecto = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el identificador del usuario
        /// </summary>
        public System.Guid UsuarioID
        {
            get
            {
                return this.usuarioID;
            }
            set
            {
                this.usuarioID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la identidad del usuario
        /// </summary>
        public Guid IdentidadID
        {
            get
            {
                return identidadID;
            }
            set
            {
                this.identidadID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el identificador de la organización gnoss propietaria del proyecto con la que inicia sesión el usuario
        /// </summary>
        public System.Guid OrganizacionID
        {
            get
            {
                return this.organizacionID;
            }
            set
            {
                this.organizacionID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el identificador de la persona de inicio de sesión
        /// </summary>
        public System.Guid PersonaID
        {
            get
            {
                return this.personaID;
            }
            set
            {
                this.personaID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el identificador del proyecto de inicio de sesión
        /// </summary>
        public System.Guid ProyectoID
        {
            get
            {
                return this.proyectoID;
            }
            set
            {
                this.proyectoID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el identificador del perfil actual.
        /// </summary>
        public System.Guid PerfilID
        {
            get
            {
                return this.perfilID;
            }
            set
            {
                this.perfilID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre de la persona vinculada con el usuario
        /// </summary>
        public String NombrePersona
        {
            get
            {
                return this.mNombrePersona;
            }
            set
            {
                this.mNombrePersona = value;
            }
        }

        /// <summary>
        /// Obtiene o establece los apellidos de la persona vinculada con el usuario
        /// </summary>
        public String ApellidosPersona
        {
            get
            {
                return this.mApellidosPersona;
            }
            set
            {
                this.mApellidosPersona = value;
            }

        }

        /// <summary>
        /// Obtiene o establece el login de acceso del usuario
        /// </summary>
        public string Login
        {
            get
            {
                return this.login;
            }
            set
            {
                this.login = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si la contraseña está autenticada
        /// </summary>
        public bool EstaPasswordAutenticada
        {
            get
            {
                return this.estaPasswordAutenticada;
            }
            set
            {
                this.estaPasswordAutenticada = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el usuario es el invitado
        /// </summary>
        public bool EsUsuarioInvitado
        {
            get
            {
                return this.mEsUsuarioInvitado;
            }
            set
            {
                this.mEsUsuarioInvitado = value;
            }
        }

        /// <summary>
        /// Verdad si la identidad actual del usuario es de invitado (no tiene acceso a una comunidad pero se le permite visitarla en calidad de invitado)
        /// </summary>
        public bool EsIdentidadInvitada
        {
            get
            {
                return this.EsUsuarioInvitado || this.mEsIdentidadInvitada;
            }
            set
            {
                this.mEsIdentidadInvitada = value;
            }
        }

        /// <summary>
        /// Obtiene si el usuario es un profesor
        /// </summary>
        public bool EsProfesor
        {
            get
            {
                return this.PerfilProfesorID.HasValue;
            }
        }

        /// <summary>
        /// Obtiene o establece el GUID de un profesor
        /// </summary>
        public Guid? PerfilProfesorID
        {
            get
            {
                return this.mPerfilProfesorID;
            }
            set
            {
                this.mPerfilProfesorID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el rol permitido para el usuario en el sistema (General)
        /// </summary>
        public ulong RolPermitidoGeneral
        {
            get
            {
                return this.rolPermitidoGeneral;
            }
            set
            {
                this.rolPermitidoGeneral = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el rol permitido para el usuario en el proyecto actual
        /// </summary>
        public ulong RolPermitidoProyecto
        {
            get
            {
                return this.rolPermitidoProyecto;
            }
            set
            {
                this.rolPermitidoProyecto = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el usuario está bloqueado en el proyecto actual
        /// </summary>
        public bool EstaBloqueadoEnProyecto
        {
            get
            {
                return this.mEstaBloqueadoEnProyecto;
            }
            set
            {
                this.mEstaBloqueadoEnProyecto = value;
            }
        }

        /// <summary>
        /// Obtiene si el login de un usuario está identificado
        /// </summary>
        public bool EstaLoginIdentificado
        {
            get
            {
                return ((this.usuarioID != System.Guid.Empty) && (this.login != null));

            }
        }

        /// <summary>
        /// Obtiene o establece si el usuario actual debe usar la base de datos maestra para consultar
        /// </summary>
        public bool UsarMasterParaLectura
        {
            get
            {
                if (mFechaUltimoGuardado.HasValue)
                {
                    if (DateTime.Now.Subtract(mFechaUltimoGuardado.Value).TotalSeconds < TiempoConsultaMaster)
                    {
                        return true;
                    }
                    else
                    {
                        mFechaUltimoGuardado = null;
                    }
                }
                return false;
            }
            set
            {
                if (value)
                {
                    mFechaUltimoGuardado = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Obtiene el tiempo de consulta a la base de datos maestra después de un guardado (por defecto 60 segundos)
        /// </summary>
        private int TiempoConsultaMaster
        {
            get
            {
                if (!mTiempoConsultaMaster.HasValue)
                {
                    int t = 60;
                    //Cambiar por configService
                    string tiempo = "";
                    if (!string.IsNullOrEmpty(tiempo))
                    {
                        if (!int.TryParse(tiempo, out t))
                        {
                            t = 60;
                        }
                    }
                    mTiempoConsultaMaster = t;
                }
                return mTiempoConsultaMaster.Value;
            }
        }

        /// <summary>
        /// Idioma del usuario.
        /// </summary>
        public string Idioma
        {
            get
            {
                return mIdioma;
            }
            set
            {
                mIdioma = value;
            }
        }

        public DateTime? FechaCambioPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Obtiene la última URL visitada
        /// </summary>
        public string UltimaUrlVisitada
        {
            get;
            set;
        }

        #endregion

        #region Miembros de IIdentity

        /// <summary>
        /// Indica si el Usuario está completamente autenticado
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return ((this.EstaLoginIdentificado) && (this.estaPasswordAutenticada));

            }
        }

        /// <summary>
        /// Obtiene el nombre
        /// </summary>
		public string Name
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene el tipo de autenticación
        /// </summary>
		public string AuthenticationType
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Destruye el usuario actual
        /// </summary>
        public void DestruirUsuario()
        {
            //Destruyo el hilo del usuario
            System.Threading.Thread.CurrentPrincipal = null;
        }

        /// <summary>
        /// Comprueba si el usuario actual puede ver la pantalla de proyectos
        /// </summary>
        /// <returns>TRUE si puede entrar a la pantalla de proyectos</returns>
        public bool PuedeEntrarAPantallaProyectos()
        {
            if (EstaAutorizado((ulong)Capacidad.General.CapacidadesProyectos.VerTODOSproyectos))
            {
                return true;
            }

            foreach (KeyValuePair<Guid, ulong> parValores in mListaRolesOrganizacion)
            {
                if (mListaRolesOrganizacion.ContainsKey(parValores.Key))
                {
                    if ((mListaRolesOrganizacion[parValores.Key] & (ulong)Capacidad.Organizacion.CapacidadesProyectos.VerProyectos) != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region General

        /// <summary>
        /// Comprueba si la Identidad (usuario) está autorizado para una cierta capacidad general
        /// </summary>
        /// <param name="pCapacidad">Capacidad para comprobar</param>
        /// <returns>TRUE en caso de que la identidad esté autorizada, FALSE en caso contrario</returns>
        public bool EstaAutorizado(ulong pCapacidad)
        {
            return ((this.rolPermitidoGeneral & pCapacidad) != 0);
        }

        #endregion

        #region Proyecto

        /// <summary>
        /// Comprueba si la Identidad (usuario) está autorizado para una cierta capacidad en el proyecto actual
        /// </summary>
        /// <param name="pCapacidad">Capacidad para comprobar</param>
        /// <returns>TRUE en caso de que la identidad esté autorizada, FALSE en caso contrario</returns>
        public bool EstaAutorizadoEnProyecto(ulong pCapacidad)
        {
            return ((this.rolPermitidoProyecto & pCapacidad) != 0);
        }

        #endregion

        #region Organización

        /// <summary>
        /// Comprueba si la Identidad (usuario) está autorizado para una cierta capacidad en la organización actual
        /// </summary>
        /// <param name="pCapacidad">Capacidad para comprobar</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>TRUE en caso de que la identidad esté autorizada, FALSE en caso contrario</returns>
        public bool EstaAutorizadoEnOrganizacion(ulong pCapacidad, Guid pOrganizacionID)
        {
            //El usuario no tiene fila de permisos para esa organizacion
            if (!mListaRolesOrganizacion.ContainsKey(pOrganizacionID))
            {
                return false;
            }
            //El usuario TIENE fila de permisos, tendre que comprobar
            else
            {
                return ((mListaRolesOrganizacion[pOrganizacionID] & pCapacidad) != 0);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Obtiene los datos del objeto
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        public void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            pInfo.AddValue("estaPasswordAutenticada", estaPasswordAutenticada);
            pInfo.AddValue("identidadID", identidadID);
            pInfo.AddValue("login", login);
            pInfo.AddValue("mApellidosPersona", mApellidosPersona);
            pInfo.AddValue("mEsIdentidadInvitada", mEsIdentidadInvitada);
            pInfo.AddValue("mPerfilProfesorID", mPerfilProfesorID);
            pInfo.AddValue("mEsUsuarioInvitado", mEsUsuarioInvitado);
            pInfo.AddValue("mListaRolesOrganizacion", mListaRolesOrganizacion);
            pInfo.AddValue("mNombrePersona", mNombrePersona);
            pInfo.AddValue("organizacionID", organizacionID);
            pInfo.AddValue("personaID", personaID);
            pInfo.AddValue("proyectoID", proyectoID);
            pInfo.AddValue("rolPermitidoGeneral", rolPermitidoGeneral);
            pInfo.AddValue("rolPermitidoProyecto", rolPermitidoProyecto);
            pInfo.AddValue("tipoAcceso", tipoAcceso);
            pInfo.AddValue("tipoProyecto", tipoProyecto);
            pInfo.AddValue("usuarioID", usuarioID);
            pInfo.AddValue("perfilID", perfilID);
            pInfo.AddValue("FechaCambioPassword", FechaCambioPassword);
            pInfo.AddValue("UltimaUrlVisitada", UltimaUrlVisitada);
            DateTime fecha = DateTime.MinValue;
            if (mFechaUltimoGuardado.HasValue)
            {
                fecha = mFechaUltimoGuardado.Value;
            }
            pInfo.AddValue("FechaUltimoGuardado", fecha);
        }

        #endregion
    }
}
