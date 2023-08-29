using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Usuarios.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Es.Riam.Gnoss.Logica.Usuarios
{
    /// <summary>
    /// Lógica referente a usuarios
    /// </summary>
    public class UsuarioCN : BaseCN, IDisposable
    {

        #region Miembros

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de UsuarioCN
        /// </summary>
        public UsuarioCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            UsuarioAD = new UsuarioAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor de UsuarioCN
        /// </summary>
        public UsuarioCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            UsuarioAD = new UsuarioAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Valida el formato del nombre corto del usuario
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>TRUE si el nombre corto del usuario es correcto, FALSE en caso contrario</returns>
        public static bool ValidarFormatoNombreCortoUsuario(string pNombreCorto)
        {
            if (pNombreCorto.Contains(" "))
            {
                return false;
            }
            Regex expresionRegular = new Regex(@"(^([a-zA-Z0-9-ñÑ]{4,30})$)");

            if (!expresionRegular.IsMatch(pNombreCorto))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Obtiene el Guid de UsuarioID a traves de la identidadID
        /// </summary>
        /// <param name="pIdentidadID"></param>
        /// <returns></returns>
        public Guid ObtenerGuidUsuarioIDporIdentidadID(Guid pIdentidadID)
        {
            using (UsuarioAD usuarioAD = new UsuarioAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                return usuarioAD.SelectGuidUsuarioIDadoIdentidadID(pIdentidadID);
            }
        }

        public void ObtenerUsuariosSonContactoOSeguidorDePerfilUsuario(Guid pPerfilID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            UsuarioAD.ObtenerUsuariosSonContactoOSeguidorDePerfilUsuario(pPerfilID, pListaUsuarios);
        }

        /// <summary>
        /// Obtiene un UsuarioDS pasando una identidad como parámetro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de ID's de las identidades</param>
        /// <returns>UsuarioDS</returns>
        public List<AD.EntityModel.Models.UsuarioDS.Usuario> ObtenerUsuariosPorIdentidadesCargaLigera(List<Guid> pListaIdentidades)
        {
            return UsuarioAD.ObtenerUsuariosPorIdentidadesCargaLigera(pListaIdentidades);
        }

        /// <summary>
        /// Obtiene los roles de todos los usuarios en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de usuarios con los roles de usuarios en un proyecto</returns>
        public List<ProyectoRolUsuario> ObtenerRolesUsuariosEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return UsuarioAD.ObtenerRolesUsuariosEnProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene un usuario por el identificador
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Nombre corto del usuario</returns>
        public string ObtenerNombreCortoUsuarioPorID(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerNombreCortoUsuarioPorID(pUsuarioID);
        }

        /// <summary>
        /// Obtiene un usuario por el identificador. Tablas: "Usuario"
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de usuario</returns>
        public AD.EntityModel.Models.UsuarioDS.Usuario ObtenerUsuarioPorID(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerUsuarioPorID(pUsuarioID);
        }

        /// <summary>
        /// Obtiene usuarios por el identificador. Tablas: "Usuario"
        /// </summary>
        /// <param name="pListaUsuario">Lista de identificadores del usuario</param>
        /// <returns>Dataset de usuario</returns>
        public List<AD.EntityModel.Models.UsuarioDS.Usuario> ObtenerUsuariosPorID(List<Guid> pListaUsuario)
        {
            return UsuarioAD.ObtenerUsuariosPorID(pListaUsuario);
        }




        /// <summary>
        /// Obtiene el ID de un usuario a partir de su id de tesauro (null si el usuario no existe)
        /// </summary>
        /// <param name="pPerfilID">Id del perfil</param>
        /// <returns></returns>
        public Guid? ObtenerUsuarioIDPorIDTesauro(Guid pTesauroID)
        {
            return UsuarioAD.ObtenerUsuarioIDPorIDTesauro(pTesauroID);
        }
        /// <summary>
        /// Obtiene el ID de un usuario a partir de su nombre corto (null si el usuario no existe)
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del usuario</param>
        /// <returns></returns>
        public Guid? ObtenerUsuarioIDPorNombreCorto(string pNombreCorto)
        {
            return UsuarioAD.ObtenerUsuarioIDPorNombreCorto(pNombreCorto);
        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su login
        /// </summary>
        /// <param name="pLogin">login del usuario</param>
        /// <returns></returns>
        public Guid ObtenerUsuarioIDPorLogin(string pLogin)
        {
            return UsuarioAD.ObtenerUsuarioIDPorLogin(pLogin);
        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su login o email
        /// </summary>
        /// <param name="pNombreCorto">Login o email del usuario</param>
        /// <returns></returns>
        public Guid? ObtenerUsuarioIDPorLoginOEmail(string pLoginOEmail)
        {
            return UsuarioAD.ObtenerUsuarioIDPorLoginOEmail(pLoginOEmail);
        }

        /// <summary>
        /// Obtiene el login de un usuario a partir de su nombre corto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del usuario</param>
        /// <returns></returns>
        public string ObtenerLoginUsuarioPorNombreCorto(string pNombreCorto)
        {
            return UsuarioAD.ObtenerLoginUsuarioPorNombreCorto(pNombreCorto);
        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su id de perfil (null si el usuario no existe)
        /// </summary>
        /// <param name="pPerfilID">Id del perfil</param>
        /// <returns></returns>
        public Guid? ObtenerUsuarioIDPorIDPerfil(Guid pPerfilID)
        {
            return UsuarioAD.ObtenerUsuarioIDPorIDPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene el ID de los usuarios a partir de su id de perfil
        /// </summary>
        /// <param name="pListaPerfilID">Lista de Identificadores del perfil</param>
        /// <returns></returns>
        public Dictionary<Guid, Guid> ObtenerUsuariosIDPorIDPerfil(List<Guid> pListaPerfilID)
        {
            return UsuarioAD.ObtenerUsuariosIDPorIDPerfil(pListaPerfilID);
        }

        /// <summary>
        /// Obtiene un usuario por el identificador
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario ObtenerUsuarioCompletoPorID(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerUsuarioCompletoPorID(pUsuarioID);
        }

        /// <summary>
        /// Obtiene los usuarios por el identificador
        /// </summary>
        /// <param name="pListaUsuarioID">Lista de identificadores de los usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerUsuariosCompletosPorID(List<Guid> pListaUsuarioID)
        {
            return UsuarioAD.ObtenerUsuariosCompletosPorID(pListaUsuarioID);
        }

        /// <summary>
        /// Obtiene un usuario por una lista de perfiles
        /// </summary>
        /// <param name="pListaPerfiles">Lista de perfiles</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerListaUsuariosPorPerfil(List<Guid> pListaPerfiles)
        {
            return UsuarioAD.ObtenerListaUsuariosPorPerfil(pListaPerfiles);
        }

        /// <summary>
        /// Obtiene una lista de usuariosID y sus perfiles para un proyecto y un documento privado
        /// </summary>
        /// <param name="pProyectoID">ProyectoID proyecto en el que se encuentra el documento</param>
        /// <param name="pDocumentoID">DocumentoID privado</param>
        /// <returns>Lista de usuarios</returns>
        public Dictionary<Guid, List<Guid>> ObtenerDiccionarioUsuariosYPerfilesPorProyectoYDocPrivado(Guid pProyectoID, Guid? pDocumentoID)
        {
            return UsuarioAD.ObtenerDiccionarioUsuariosYPerfilesPorProyectoYDocPrivado(pProyectoID, pDocumentoID);
        }

        /// <summary>
        /// Obtiene una lista de usuariosID y sus perfiles para un proyecto y un documento privado
        /// </summary>
        /// <param name="pProyectoID">ProyectoID proyecto en el que se encuentra el documento</param>
        /// <param name="pDocumentoID">DocumentoID privado</param>
        /// <returns>Lista de usuarios</returns>
        public Dictionary<Guid, List<Guid>> ObtenerDiccionarioGruposYPerfilesPorProyectoYDocPrivado(Guid pProyectoID, Guid? pDocumentoID)
        {
            return UsuarioAD.ObtenerDiccionarioGruposYPerfilesPorProyectoYDocPrivado(pProyectoID, pDocumentoID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaGruposEditoresEliminadosEdiccionRecursoPrivado"></param>
        /// <returns></returns>
        public Dictionary<Guid, List<Guid>> ObtenerDiccionarioGruposYPerfilesPorListaGruposID(List<Guid> pListaGruposEditoresEliminadosEdiccionRecursoPrivado)
        {
            return UsuarioAD.ObtenerDiccionarioGruposYPerfilesPorListaGruposID(pListaGruposEditoresEliminadosEdiccionRecursoPrivado);
        }

        /// <summary>
        /// Obtiene una lista de grupos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <returns>Lista de grupos</returns>
        public List<Guid> ObtenerListaGruposPorProyecto(Guid pProyectoID)
        {
            return UsuarioAD.ObtenerListaGruposPorProyecto(pProyectoID);
        }

        ///// <summary>
        ///// Obtiene una lista de grupos de un proyecto y un documento privado
        ///// </summary>
        ///// <param name="pProyectoID">ProyectoID</param>
        ///// <returns>Lista de grupos</returns>
        //public List<Guid> ObtenerListaGruposPorProyectoYDocPrivado(Guid pProyectoID, Guid pDocumentoID)
        //{
        //    return UsuarioAD.ObtenerListaGruposPorProyectoYDocPrivado(pProyectoID, pDocumentoID);
        //}

        ///// <summary>
        ///// Obtiene los usuarios por el identificador
        ///// </summary>
        ///// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        ///// <returns>Dataset de usuarios</returns>
        //public UsuarioDS ObtenerUsuariosPorIDs(List<Guid> pListaUsuarios)
        //{
        //    return UsuarioAD.ObtenerUsuariosPorIDs(pListaUsuarios);
        //}

        /// <summary>
        /// Obtiene un usuario por el login
        /// </summary>
        /// <param name="pLogin">Login del usuario</param>
        /// <param name="pObtenerInicioSesion">TRUE si se debe obtener el inicio de sesión</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario ObtenerUsuarioPorLoginOEmail(string pLogin, bool pObtenerInicioSesion)
        {
            bool buscarSoloPorLogin = true;
            if (pLogin.Contains("@"))
            {
                buscarSoloPorLogin = false;
            }

            return UsuarioAD.ObtenerUsuarioPorLoginOEmail(pLogin, pObtenerInicioSesion, true, buscarSoloPorLogin);
        }


        /// <summary>
        /// Obtiene la fila de un usuario por login o email
        /// </summary>
        /// <param name="pLogin"></param>
        /// <returns></returns>
        public AD.EntityModel.Models.UsuarioDS.Usuario ObtenerFilaUsuarioPorLoginOEmail(string pLogin)
        {
            return UsuarioAD.ObtenerFilaUsuarioPorLoginOEmail(pLogin);
        }

        /// <summary>
        /// Obtiene un usuario por el login
        /// </summary>
        /// <param name="pLogin">Login del usuario</param>
        /// <param name="pObtenerInicioSesion">TRUE si se debe obtener el inicio de sesión</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario ObtenerUsuarioPorLogin(string pLogin, bool pObtenerInicioSesion)
        {
            bool buscarSoloPorLogin = true;
            if (pLogin.Contains("@"))
            {
                buscarSoloPorLogin = false;
            }

            return UsuarioAD.ObtenerUsuarioPorLoginOEmail(pLogin, pObtenerInicioSesion, false, buscarSoloPorLogin);
        }

        /// <summary>
        /// Obtiene un usuario (Usuario e InicioSesion)
        /// </summary>
        /// <param name="pLogin">Login del usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario ObtenerUsuarioPorLoginEnProyecto(string pLogin, Guid pProyectoID)
        {
            return UsuarioAD.ObtenerUsuarioPorLoginEnProyecto(pLogin, pProyectoID);
        }


        /// <summary>
        /// Obtiene un dataset con "UsuarioID", "Login", "Nombre" y "Apellidos" de los administradores de la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset sin tipar</returns>
        public UsuarioPersona ObtenerAdministradoresPorOrganizacion(Guid pOrganizacionID)
        {
            return UsuarioAD.ObtenerAdministradoresPorOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene un dataset con "UsuarioID", "Login", "Nombre" y "Apellidos" de los administradores de la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset sin tipar</returns>
        public List<UsuarioIdentidadPersona> ObtenerAdministradoresNombreApellidosPorOrganizacion(Guid pOrganizacionID)
        {
            return UsuarioAD.ObtenerAdministradoresNombreApellidosPorOrganizacion(pOrganizacionID);
        }
        /// <summary>
		/// Actualiza los cambios realizados en la lista de usuarios proporcionada
		/// </summary>
		/// <param name="pDataWrapperUsuario">Dataset de usuarios</param>
        /// <param name="pComprobarAutorizacion">TRUE si se debe comprobar que el usuario que está conectado 
        /// tiene permiso para realizar la operación</param>
		public void ActualizarUsuario(bool pComprobarAutorizacion)
        {
            // TODO
            //this.ValidarUsuarios(cambiosUsuarios);

            try
            {
                bool transaccionIniciada = UsuarioAD.IniciarTransaccionEntityContext();

                UsuarioAD.ActualizarUsuarios();

                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
                }


            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación	
                mLoggingService.GuardarLogError(ex);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Autentica el login de usuario. Obtiene "Usuario", "InicioSesion" y "ProyectoUsuarioIdentidad" de ese usuario
        /// </summary>
        /// <param name="pLogin">Login de usuario para autenticar</param>
        /// <param name="pObtenerInicioSesion">Verdad si se debe de obtener el inicio de sesión, falso en caso de que no sea necesario</param>
        /// <returns>Dataset de usuario en caso de ser válida la autenticación, las correspondientes excepciones en caso contrario</returns>
        public DataWrapperUsuario AutenticarLogin(string pLogin, bool pObtenerInicioSesion)
        {
            ChequeoSeguridad.Anonimos();

            bool buscarSoloPorLogin = true;
            if (pLogin.Contains("@"))
            {
                buscarSoloPorLogin = false;
            }

            DataWrapperUsuario dataWrapperUsuario = this.UsuarioAD.ObtenerUsuarioPorLoginOEmail(pLogin, pObtenerInicioSesion, false, buscarSoloPorLogin);

            if ((dataWrapperUsuario == null) || (dataWrapperUsuario.ListaUsuario.Count == 0))
            {
                throw new ErrorLoginUsuario();
            }

            if (this.UsuarioAD.EstaBloqueadoUsuario(dataWrapperUsuario.ListaUsuario.First()))
            {
                throw new ErrorUsuarioBloqueado();
            }
            return (dataWrapperUsuario);
        }

        /// <summary>
        /// Establece la contraseña encriptada de un usuario
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <param name="pPassword">Password</param>
        public void EstablecerPasswordUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, string pPassword)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarUsuarios);

            string hashPassword;

            ValidarFormatoPassword(pPassword);

            hashPassword = HashHelper.CalcularHash(pPassword, true);
            UsuarioAD.EstablecerPasswordUsuario(pUsuario, hashPassword);
        }

        public void EstablecerPasswordUsuario(Guid pUsuarioID, string pPassword)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarUsuarios);

            string hashPassword;

            ValidarFormatoPassword(pPassword);

            hashPassword = HashHelper.CalcularHash(pPassword, true);
            UsuarioAD.EstablecerPasswordUsuario(pUsuarioID, hashPassword);
        }

        /// <summary>
        /// Establece la contraseña encriptada del usuario actual
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <param name="pPassword">Password</param>
        public void EstablecerPasswordPropioUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, string pPassword)
        {
            string hashPassword;

            ValidarFormatoPassword(pPassword);

            hashPassword = HashHelper.CalcularHash(pPassword, true);
            UsuarioAD.EstablecerPasswordUsuario(pUsuario, hashPassword);
        }

        /// <summary>
        /// Establece la caducidad de un usuario a la fecha actual
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        public void EstablecerCaducidadPasswordUsuario(Guid pUsuarioID)
        {
            UsuarioAD.EstablecerCaducidadPasswordUsuario(pUsuarioID);
        }

        public void EstablecerLoginUsuario(Guid pUsuarioID, string pLogin)
        {
            UsuarioAD.EstablecerLoginUsuario(pUsuarioID, pLogin);
        }

        /// <summary>
        /// Suma uno a uno de los contadores del usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pContador">String del contador a actualizar. Se pueden usar las constantes definidas en UsuarioAD: CONTADOR_NUMERO_ACCESOS</param>
        public void ActualizarContadorUsuarioNumAccesos(Guid pUsuarioID)
        {
            try
            {
                UsuarioAD.ActualizarContadorUsuarioNumAccesos(pUsuarioID);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, string.Format("Error al actualizar el contador NumeroDeAccesos del usuario {0}", pUsuarioID));
            }
        }

        /// <summary>
        /// Obtiene el número de accesos del usuario a la plataforma
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Número de accesos del usuario</returns>
        public int ObtenerContadorDeAccesosDeUsuario(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerContadorDeAccesosDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene el número de accesos del usuario a la plataforma
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Número de accesos del usuario</returns>
        public DateTime? ObtenerFechaUltimoAccesoDeUsuario(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerFechaUltimoAccesoDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Comprueba si la contraseña de usuario es correcta para el usuario dado
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <param name="pPassword">Password a comprobar</param>
        /// <returns>TRUE si la contraseña es correcta</returns>
        public bool ValidarPasswordUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, string pPassword)
        {
            string passwordUsuario;

            passwordUsuario = UsuarioAD.ObtenerPasswordUsuario(pUsuario);

            bool usar256 = false;

            if (pUsuario.Version.HasValue && pUsuario.Version.Value.Equals(1))
            {
                usar256 = true;
            }

            bool passwordValida = HashHelper.ValidarPassword(pPassword, passwordUsuario, usar256);

            if (passwordValida && !usar256)
            {
                // Si la contraseña es válida pero no usa encriptación de 256 bits, actualizo la clave del usuario
                UsuarioAD.EstablecerPasswordUsuario(pUsuario, HashHelper.CalcularHash(pPassword, true));
            }

            return passwordValida;
        }

        /// <summary>
        /// Nos indica en que proyectos el usuario tiene o no foto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Diccionario con proyectoID como clave y true o false en función de si tiene foto o no en ese proyecto</returns>
        public string FotoPerfilPersonalUsuario(Guid pUsuarioID)
        {
            return UsuarioAD.FotoPerfilPersonalUsuario(pUsuarioID);
        }

        /// <summary>
        /// Comprueba si la contraseña de usuario es correcta para el usuario dado
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <param name="pPassword">Password a comprobar</param>
        /// <returns>TRUE si la contraseña es correcta</returns>
        public bool ValidarPasswordUsuarioSinActivar(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, string pPassword)
        {
            string passwordUsuario;

            passwordUsuario = UsuarioAD.ObtenerPasswordUsuario(pUsuario);

            bool usar256 = false;

            if (pUsuario.Version.HasValue && pUsuario.Version.Value.Equals(1))
            {
                usar256 = true;
            }

            bool passwordValida = HashHelper.ValidarPassword(pPassword, passwordUsuario, usar256);

            if (passwordValida && !usar256)
            {
                // Si la contraseña es válida pero no usa encriptación de 256 bits, actualizo la clave del usuario
                UsuarioAD.EstablecerPasswordUsuario(pUsuario, HashHelper.CalcularHash(pPassword, true));
            }

            return passwordValida;
        }

        /// <summary>
        /// Comprueba si la contraseña de usuario es correcta para el usuario dado
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <param name="pPassword">Password a comprobar</param>
        /// <returns>TRUE si la contraseña es correcta</returns>
        public bool ValidarPasswordUsuarioParaSolicitud(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, string pPassword)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarUsuarios);

            string passwordUsuario;

            passwordUsuario = UsuarioAD.ObtenerPasswordUsuario(pUsuario);

            bool usar256 = false;

            if (pUsuario.Version.HasValue && pUsuario.Version.Value.Equals(1))
            {
                usar256 = true;
            }

            bool passwordValida = HashHelper.ValidarPassword(pPassword, passwordUsuario, usar256);

            if (passwordValida && !usar256)
            {
                // Si la contraseña es válida pero no usa encriptación de 256 bits, actualizo la clave del usuario
                UsuarioAD.EstablecerPasswordUsuario(pUsuario, HashHelper.CalcularHash(pPassword, true));
            }

            return passwordValida;
        }

        /// <summary>
        /// Comprueba si un usuario está bloqueado
        /// </summary>
        /// <param name="pUsuario">Fila de usuario para comprobar</param>
        /// <returns>TRUE en caso de estar bloqueado, FALSE en caso contrario</returns>
        public bool EstaBloqueadoUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            // ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarUsuarios | (ulong)Capacidad.General.CapacidadesAdministracion.EditarRoles);

            return UsuarioAD.EstaBloqueadoUsuario(pUsuario);
        }

        /// <summary>
        /// Comprueba si un usuario está bloqueado en un proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si está bloqueado, FALSE si tiene acceso</returns>
        public bool ValidarUsuarioProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return UsuarioAD.EstaBloqueadoUsuarioEnProyecto(pUsuarioID, pProyectoID);
        }

        /// <summary>
        /// Bloquea un usuario (No puede entrar en el sistema hasta que un administrador desbloquee su cuenta)
        /// </summary>
        /// <param name="pUsuario">Fila del usuario para bloquear</param>
        public void BloquearPropioUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            UsuarioAD.BloquearUsuario(pUsuario);
        }

        /// <summary>
        /// Bloquea un usuario (No puede entrar en el sistema hasta que un administrador desbloquee su cuenta)
        /// </summary>
        /// <param name="pUsuario">Fila del usuario para bloquear</param>
        public void BloquearUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Es.Riam.Gnoss.Util.Seguridad.Capacidad.General.CapacidadesAdministracion.EditarUsuarios);

            UsuarioAD.BloquearUsuario(pUsuario);
        }

        /// <summary>
        /// Desbloquea un usuario
        /// </summary>
        /// <param name="pUsuario">Fila del usuario para desbloquear</param>
        public void DesbloquearUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarUsuarios);

            UsuarioAD.DesbloquearUsuario(pUsuario);
        }

        /// <summary>
        /// Asigna un usuario a una persona de una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        public void AsignarUsuarioAPersona(System.Guid pUsuarioID, System.Guid pOrganizacionID, System.Guid pPersonaID)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarUsuarios | (ulong)Capacidad.General.CapacidadesAdministracion.EditarRoles);

            UsuarioAD.AsignarUsuarioAPersona(pUsuarioID, pOrganizacionID, pPersonaID);
        }

        /// <summary>
        /// Comprueba si existe un usuario en la base de datos
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        public bool ExisteUsuarioEnBD(System.Guid pUsuarioID)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarUsuarios);

            return UsuarioAD.ExisteUsuarioEnBD(pUsuarioID);
        }

        /// <summary>
        /// Comprueba si existe un usuario en la base de datos
        /// </summary>
        /// <param name="pNombreUsuario">Nombre del usuario</param>
        public bool ExisteUsuarioEnBD(string pNombreUsuario)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarUsuarios);

            return UsuarioAD.ExisteUsuarioEnBD(pNombreUsuario);
        }

        /// <summary>
        /// Comprueba si existe un usuario en la base de datos
        /// </summary>
        /// <param name="pNombreUsuario">Nombre del usuario</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public string ObtenerLoginLibre(string pNombreUsuario)
        {
            return UsuarioAD.ObtenerLoginLibre(pNombreUsuario);
        }

        /// <summary>
        /// Comprueba si existe un usuario en la base de datos
        /// </summary>
        /// <param name="pNombreUsuario">Nombre del usuario</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public string ObtenerNombreCortoLibre(string pNombreUsuario)
        {
            return UsuarioAD.ObtenerLoginLibre(pNombreUsuario);
        }

        /// <summary>
        /// Calcula el rol permitido para un usuario teniendo en cuenta los roles de los grupos a los que pertenece
        /// </summary>
        /// <param name="pUsuario">Fila de usuario para calcular su rol</param>
        /// <returns>Rol permitido final del usuario</returns>
        public ulong CalcularRolFinalUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            //Declaramos roles por defecto 
            ulong rolPermitidoUsuario = 0;
            ulong rolDenegadoUsuario = 0;

            //1º Obtenemos los roles del usuario
            UsuarioAD rolUsuarioAD = new UsuarioAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            GeneralRolUsuario generalRolUsuario = rolUsuarioAD.ObtenerGeneralRolUsuario(pUsuario);

            if (generalRolUsuario == null)
            {
                throw new ExcepcionGeneral("Se desconoce el rol de usuario");
            }

            if (generalRolUsuario.RolPermitido != null)
            {
                rolPermitidoUsuario = System.Convert.ToUInt64(generalRolUsuario.RolPermitido, 16);
            }

            if (generalRolUsuario.RolDenegado != null)
            {
                rolDenegadoUsuario = System.Convert.ToUInt64(generalRolUsuario.RolDenegado, 16);
            }


            //4º Calculamos el rol final
            ulong rolPermitidoFinal = rolPermitidoUsuario;
            ulong rolDenegadoFinal = rolDenegadoUsuario;

            return (rolPermitidoFinal & ~(rolDenegadoFinal));
        }

        /// <summary>
        /// Obtiene el rol propio del usuario en el proyecto dado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <returns>Dataset de usuario con el rol propio del usuario en el proyecto</returns>
        public ProyectoRolUsuario ObtenerRolPropioUsuarioEnProyecto(Guid pProyectoID, AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            return (this.UsuarioAD.ObtenerRolUsuarioEnProyecto(pProyectoID, pUsuario.UsuarioID));
        }

        /// <summary>
        /// Obtiene el rol de un usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Rol de usuario en un proyecto</returns>
        public ProyectoRolUsuario ObtenerRolUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarRoles);

            return (this.UsuarioAD.ObtenerRolUsuarioEnProyecto(pProyectoID, pUsuarioID));
        }

        /// <summary>
        /// Obtiene el rol de los usuarios en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Lista de identificadores de los usuarios</param>
        /// <returns>Dataset de usuarios con el rol de los usuarios en un proyecto</returns>
        public DataWrapperUsuario ObtenerRolListaUsuariosEnProyecto(Guid pProyectoID, List<Guid> pUsuariosID)
        {
            return this.UsuarioAD.ObtenerRolListaUsuariosEnProyecto(pProyectoID, pUsuariosID);
        }

        /// <summary>
        /// Obtiene el rol de un usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuario">Identificador del usuario</param>
        /// <returns>Dataset de usuario con el rol de usuario en un proyecto</returns>
        public ProyectoRolUsuario ObtenerRolUsuarioEnProyecto(Guid pProyectoID, AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarRoles);

            return (this.UsuarioAD.ObtenerRolUsuarioEnProyecto(pProyectoID, pUsuario.UsuarioID));
        }

        /// <summary>
        /// Obtiene el rol General asignado a un usuario
        /// </summary>
        /// <param name="pUsuario">Fila de usuario para obtener su rol</param>
        /// <param name="pComprobarAutorizacion">TRUE si se debe de comprobar que el usuario tiene autorización</param>
        /// <returns>Dataset de usuario con el rol asignado a dicho usuario</returns>
        public GeneralRolUsuario ObtenerRolUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, bool pComprobarAutorizacion)
        {
            if (pComprobarAutorizacion)
            {
                // ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesAdministracion.EditarRoles);
            }
            return UsuarioAD.ObtenerGeneralRolUsuario(pUsuario);
        }

        /// <summary>
        /// Verdad si el usuario está bloqueado en este proyecto, falso en caso contrario
        /// </summary>
        /// <param name="pOrganizacionID">Idetnificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns></returns>
        public bool EstaUsuarioBloqueadoEnProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID)
        {
            return UsuarioAD.EstaUsuarioBloqueadoEnProyecto(pOrganizacionID, pProyectoID, pUsuarioID);
        }

        /// <summary>
        /// Comprueba si existe un usuario con el nombre corto pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto"></param>
        /// <returns>True si lo encuentra</returns>
        public bool ExisteNombreCortoEnBD(string pNombreCorto)
        {
            return UsuarioAD.ExisteNombreCortoEnBD(pNombreCorto);
        }

        /// <summary>
        /// Obtiene los nombres cortos que empizan con los caracteres introducidos.
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>nombres cortos que empizan con los caracteres introducidos</returns>
        public List<string> ObtenerNombresCortosEmpiezanPor(string pNombreCorto)
        {
            return UsuarioAD.ObtenerNombresCortosEmpiezanPor(pNombreCorto);
        }

        /// <summary>
        /// Obtiene la UrlRedirect del usuario
        /// </summary>
        /// <param name="pUsuarioID">pUsuarioID</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public string ObtenerUrlRedirect(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerUrlRedirect(pUsuarioID);
        }

        /// <summary>
        /// Elimina la fila con el usuarioID
        /// </summary>
        /// <param name="pUsuarioID">pUsuarioID</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public void EliminarUrlRedirect(Guid pUsuarioID)
        {
            UsuarioAD.EliminarUrlRedirect(pUsuarioID);
        }

        /// <summary>
        /// Modifica la fila con ese usuarioID
        /// </summary>
        /// <param name="pUsuarioID">pUsuarioID</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public void ModificarUrlRedirect(Guid pUsuarioID, string pRedirect)
        {
            UsuarioAD.ModificarUrlRedirect(pUsuarioID, pRedirect);
        }

        /// <summary>
        /// Inserta la fila
        /// </summary>
        /// <param name="pUsuarioID">pUsuarioID</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public void InsertarUrlRedirect(Guid pUsuarioID, string pRedirect)
        {
            UsuarioAD.InsertarUrlRedirect(pUsuarioID, pRedirect);
        }

        /// <summary>
        /// Obtiene los Login que empizan con los caracteres introducidos.
        /// </summary>
        /// <param name="pLogin">Login</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public List<string> ObtenerLoginEmpiezanPor(string pLogin)
        {
            return UsuarioAD.ObtenerLoginEmpiezanPor(pLogin);
        }

        /// <summary>
        /// Obtiene el ID de un usuario en funcion de sus datos de tipo de red social y el id en esa red social
        /// </summary>
        /// <param name="pTipoRedSocial"></param>
        /// <param name="pIDenRedSocial"></param>
        /// <returns></returns>
        public Guid ObtenerUsuarioPorLoginEnRedSocial(TipoRedSocialLogin pTipoRedSocial, string pIDenRedSocial)
        {
            return UsuarioAD.ObtenerUsuarioPorLoginEnRedSocial(pTipoRedSocial, pIDenRedSocial);
        }

        /// <summary>
        /// Obtiene el Login de la red social de un usuario en funcion de sus datos de tipo de red social y el usuario id
        /// </summary>
        /// <param name="pTipoRedSocial"></param>
        /// <param name="pUsuarioID"></param>
        /// <returns></returns>
        public string ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin pTipoRedSocial, Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerLoginEnRedSocialPorUsuarioId(pTipoRedSocial, pUsuarioID);
        }

        /// <summary>
        /// Actualiza el Login de la red social de un usuario en funcion de sus datos de tipo de red social y el usuario id 
        /// </summary>
        /// <param name="pTipoRedSocial"></param>
        /// <param name="pUsuarioID"></param>
        /// <param name="pLogin"></param>
        public void ActualizarLoginEnRedSocialPorUsuario(TipoRedSocialLogin pTipoRedSocial, Guid pUsuarioID, string pLogin)
        {
            UsuarioAD.ActualizarLoginEnRedSocialPorUsuario(pTipoRedSocial, pUsuarioID, pLogin);
        }

        /// <summary>
        /// Obtiene la tabla usuariovinculadoredessociales de un usuario
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public DataWrapperUsuario ObtenerFilaUsuarioVincRedSocialPorUsuarioID(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerFilaUsuarioVincRedSocialPorUsuarioID(pUsuarioID);
        }

        /// <summary>
        /// Obtiene el rol asignado a un usuario
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <returns>Dataset de usuario con el rol asignado a dicho usuario</returns>
        public GeneralRolUsuario ObtenerRolUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            return ObtenerRolUsuario(pUsuario, true);
        }

        /// <summary>
        /// Valida el usario pasado por parametro
        /// </summary>
        /// <param name="pUsuarioID"></param>
        public void ValidarUsuario(Guid pUsuarioID)
        {
            UsuarioAD.ValidarUsuario(pUsuarioID);
        }

        /// <summary>
        /// Devuelve...
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerRolesUsuarioPorPerfilYProyecto(Guid pUsuarioID, Guid pProyectoID, Guid pPerfilID)
        {
            return UsuarioAD.ObtenerRolesUsuarioPorPerfilYProyecto(pUsuarioID, pProyectoID, pPerfilID);
        }

        /// <summary>
        /// Obtiene los usuarios de las perosnas cargada en el dataSet.
        /// </summary>
        /// <param name="pDataWrapperUsuario">DataSet co las personas cargadas</param>
        /// <returns>Dataset de usuarios</returns>
        public List<AD.EntityModel.Models.UsuarioDS.Usuario> ObtenerUsuariosDePersonasCargadas(DataWrapperPersona pDataWrapperUsuario)
        {
            return UsuarioAD.ObtenerUsuariosDePersonasCargadas(pDataWrapperUsuario.ListaPersona);
        }

        /// <summary>
        /// Comprueba si un usuario está (tiene acceso) a un proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si está en el proyecto</returns>
        public bool EstaUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return UsuarioAD.EstaUsuarioEnProyecto(pUsuarioID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los roles (Usuario y OrganizacionRolUsuario) en las organizaciones de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerRolesOrganizaciones(Guid pUsuarioID)
        {
            return ObtenerRolesOrganizaciones(pUsuarioID, false);
        }

        /// <summary>
        /// Obtiene los roles (Usuario y OrganizacionRolUsuario) en las organizaciones de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pObtenerSoloSiTieneAlgunPermiso">Verdad si se deben obtener sólo las filas en las que el usuario tiene algún permiso</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerRolesOrganizaciones(Guid pUsuarioID, bool pObtenerSoloSiTieneAlgunPermiso)
        {
            return UsuarioAD.ObtenerRolesOrganizaciones(pUsuarioID, pObtenerSoloSiTieneAlgunPermiso);
        }

        /// <summary>
        /// Calcula el rol permitido de un usuario en una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Rol permitido final del usuario en la organización</returns>
        public ulong CalcularRolFinalOrganizacion(Guid pUsuarioID, Guid pOrganizacionID)
        {
            //Declaramos roles por defecto 
            ulong rolPermitidoUsuarioOrganizacion = 0;
            ulong rolDenegadoUsuarioOrganizacion = 0;

            //1º Obtenemos los roles del usuario en pOrganizacionID
            UsuarioAD usuarioAD = new UsuarioAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperUsuario dataWrapperUsuario = usuarioAD.ObtenerOrganizacionRolUsuario(pUsuarioID, pOrganizacionID);

            if ((dataWrapperUsuario == null) || (dataWrapperUsuario.ListaOrganizacionRolUsuario.Count == 0))
            {
                throw new Exception("Se desconoce el rol del usuario en organización");
            }

            if (dataWrapperUsuario.ListaOrganizacionRolUsuario.FirstOrDefault().RolPermitido != null)
            {
                rolPermitidoUsuarioOrganizacion = System.Convert.ToUInt64(dataWrapperUsuario.ListaOrganizacionRolUsuario.First().RolPermitido, 16);
            }

            if (dataWrapperUsuario.ListaOrganizacionRolUsuario.First().RolDenegado != null)
            {
                rolDenegadoUsuarioOrganizacion = System.Convert.ToUInt64(dataWrapperUsuario.ListaOrganizacionRolUsuario.First().RolDenegado, 16);
            }
            //2º Calculamos el rol final
            usuarioAD.Dispose();

            return (rolPermitidoUsuarioOrganizacion & ~(rolDenegadoUsuarioOrganizacion));
        }

        /// <summary>
        /// Obtiene los usuarios y sus roles en la pOrganizacionID. Carga "Usuario" y "OrganizacionRolUsuario"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario CargarUsuariosDeOrganizacion(Guid pOrganizacionID)
        {
            return UsuarioAD.CargarUsuariosDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene los roles (Usuario y OrganizacionRolUsuario) en una organización de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerOrganizacionRolUsuario(Guid pUsuarioID, Guid pOrganizacionID)
        {
            return UsuarioAD.ObtenerOrganizacionRolUsuario(pUsuarioID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene los usuarios en la pOrganizacionID
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario CargarUsuariosDeOrganizacionCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            return UsuarioAD.CargarUsuariosDeOrganizacionCargaLigeraParaFiltros(pOrganizacionID);
        }

        /// <summary>
        /// Cuenta los usuarios agrupados por tipo de una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns></returns>
        public Dictionary<short, int> ContarTiposUsuariosDeOrganizacion(Guid pOrganizacionID)
        {
            return UsuarioAD.ContarTiposUsuariosDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene los usuarios y sus roles en la pOrganizacionID 
        /// Carga "Usuario", "OrganizacionRolUsuario" y "ProyectoUsuarioIdentidad" 
        /// Esta última tabla se carga con las identidades de los usuarios de la organización en proyectos 
        /// donde dichos usuarios estan con un perfil de dicha organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario CargarUsuariosDeOrganizacionYsusIdentidadesYPermisosEnProyectosConPerfilDeDichaOrg(Guid pOrganizacionID)
        {
            return UsuarioAD.CargarUsuariosDeOrganizacionYsusIdentidadesYPermisosEnProyectosConPerfilDeDichaOrg(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene el número de personas que son editores en una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">ID de la organizacion</param>
        /// <returns></returns>
        public int ObtenerNumEditoresOrganizacion(Guid pOrganizacionID)
        {
            return UsuarioAD.ObtenerNumEditoresOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene el número de personas que son usuarios en una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">ID de la organizacion</param>
        /// <returns></returns>
        public int ObtenerNumUsuariosOrganizacion(Guid pOrganizacionID)
        {
            return UsuarioAD.ObtenerNumUsuariosOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene los usuarios que son administradores de la pOrganizacionID. Carga "Usuario" y "OrganizacionRolUsuario"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario CargarAdministradoresDeOrg(Guid pOrganizacionID)
        {
            return UsuarioAD.CargarAdministradoresDeOrg(pOrganizacionID);
        }

        /// <summary>
        /// Elimina los datos de usuario marcados para borrar
        /// </summary>
        /// <param name="pUsuarioDW">Dataset de usuario</param>
        public void EliminarBorrados(DataWrapperUsuario pUsuarioDW)
        {
            try
            {
                if (Transaccion != null)
                {
                    UsuarioAD.ActualizarUsuarios();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        UsuarioAD.ActualizarUsuarios();

                        if (pUsuarioDW != null)
                        {
                            mEntityContext.SaveChanges();
                        }
                        TerminarTransaccion(true);
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación
                mLoggingService.GuardarLogError(ex);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Guarda los datos de usuario que se deben actualizar
        /// </summary>
        /// <param name="pDataWrapperUsuario">Dataset de usuario</param>
        public void GuardarActualizaciones(DataWrapperUsuario pDataWrapperUsuario)
        {
            //Comprobaciones Usuario
            List<AD.EntityModel.Models.UsuarioDS.Usuario> cambiosUsuarios = new List<AD.EntityModel.Models.UsuarioDS.Usuario>();

            // TODO
            //this.ValidarUsuarios(cambiosUsuarios);

            try
            {
                if (Transaccion != null)
                {
                    UsuarioAD.ActualizarUsuarios();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        UsuarioAD.ActualizarUsuarios();

                        TerminarTransaccion(true);
                    }
                }

            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación
                mLoggingService.GuardarLogError(ex);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }


        /// <summary>
        /// True si el usuario es administrador general 
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <returns>True si el usuario es administrador general False en caso contrario</returns>
        public bool EsUsuarioAdministradorGeneral(Guid pUsuarioID)
        {
            return UsuarioAD.EsUsuarioAdministradorGeneral(pUsuarioID);
        }

        /// <summary>
        /// Comprueba si el usuario esta solo en proyectos públicos, o posee también privados, etc.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>TRUE si el usuario solo está en proyectos públicos, FALSE en caso contrario</returns>
        public bool UsuarioSoloEstaEnProyectosPublicos(Guid pUsuarioID)
        {
            return UsuarioAD.UsuarioSoloEstaEnProyectosPublicos(pUsuarioID);
        }

        /// <summary>
        /// Obtiene el Email de un usuario a traves de su usuarioID
        /// </summary>
        /// <param name="pUsuarioID">Usuario ID</param>
        /// <returns></returns>
        public string ObtenerEmailPorUsuarioID(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerEmailPorUsuarioID(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las clausulas adicionales de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerClausulasRegitroProyecto(Guid pProyectoID)
        {
            return UsuarioAD.ObtenerClausulasRegitroProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene la política de cookies de un proyecto (del metaproyecto si no tiene el proyecto)
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerPoliticaCookiesProyecto(Guid pProyectoID)
        {
            return UsuarioAD.ObtenerPoliticaCookiesProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene las clausulas adicionales indicadas.
        /// </summary>
        /// <param name="pListaClausulasID">Lista de las clausulas</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerClausulasRegitroPorID(List<Guid> pListaClausulasID)
        {
            return UsuarioAD.ObtenerClausulasRegitroPorID(pListaClausulasID);
        }

        /// <summary>
        /// Obtiene la tabla ProyClausulasUsu de un usuario por el identificador
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerProyClausulasUsuPorUsuarioID(Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerProyClausulasUsuPorUsuarioID(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las clausulas adicionales de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerClausulasRegitroProyectoYUsuario(Guid pProyectoID, Guid pUsuarioID)
        {
            return UsuarioAD.ObtenerClausulasRegitroProyectoYUsuario(pProyectoID, pUsuarioID);
        }

        /// <summary>
        /// Obtiene los usuarios que participan en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren saber sus usuarios</param>
        /// <returns>Lista de Guid</returns>
        public void ObtenerUsuariosParticipanEnProyecto(Guid pProyectoID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            UsuarioAD.ObtenerUsuariosParticipanEnProyecto(pProyectoID, pListaUsuarios);
        }

        /// <summary>
        /// Obtiene el identificador de los perfiles afectados por un comentario en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void ObtenerUsuarioIDPublicadoresComentarioRecursoEnProyecto(Guid pDocumentoID, Guid? pProyectoID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            UsuarioAD.ObtenerUsuarioIDPublicadoresComentarioRecursoEnProyecto(pDocumentoID, pProyectoID, pListaUsuarios);
        }

        /// <summary>
        /// Obtiene el identificador de los perfiles afectados por un comentario en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pListaUsuarios">Identificador del proyecto</param>
        /// <param name="pCaracteresInicio">Caracteres de inicio</param>
        public void ObtenerUsuarioIDEditoresLectoresRecurso(Guid pDocumentoID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            UsuarioAD.ObtenerUsuarioIDEditoresLectoresRecurso(pDocumentoID, pListaUsuarios);
        }

        /// <summary>
        /// Obtiene el identificador de los usuarios que han hecho algún comentario en un documento en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pListaUsuarios">Lista de usuarios</param>
        /// <returns></returns>
        public void ObtenerUsuarioIDVotantesRecurso(Guid pDocumentoID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            UsuarioAD.ObtenerUsuarioIDVotantesRecurso(pDocumentoID, pListaUsuarios);
        }


        public List<Guid> ObtenerUsuariosPertenecenGrupo(Guid pGrupoID, List<Guid> pListaUsuariosComprobar = null)
        {
            return UsuarioAD.ObtenerUsuariosPertenecenGrupo(pGrupoID, pListaUsuariosComprobar);
        }

        /// <summary>
        /// Obtiene una lista de identificadores de los usuarios que se han suscrito a alguna categoría del tesauro o a otros usuarios o se han hecho miembros de la comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        /// <param name="pFechaBusqueda">Fecha a partir de la cual buscar recursos con actividad</param>
        /// <returns>Lista de identificadores de usuario</returns>
        public List<Guid> ObtenerUsuariosActivosEnFecha(Guid pProyectoID, DateTime pFechaBusqueda)
        {
            return UsuarioAD.ObtenerUsuariosActivosEnFecha(pProyectoID, pFechaBusqueda);
        }

        #endregion

        #region Privados

        /// <summary>
        /// Valida una lista de usuarios
        /// </summary>
        /// <param name="pUsuarios">Conjunto de filas de usuario para validar</param>
        private void ValidarUsuarios(List<AD.EntityModel.Models.UsuarioDS.Usuario> pUsuarios)
        {
            for (int i = 0; i < pUsuarios.Count; i++)
            {
                //Login nulo
                if (pUsuarios[i].Login == null)
                    throw new ErrorDatoNoValido("El nombre de usuario no puede ser nulo");

                //Login de formato no válido
                if (pUsuarios[i].Login.Trim().Length == 0)
                    throw new ErrorDatoNoValido("El nombre de usuario no puede ser una cadena vacía");
                else if (pUsuarios[i].Login.Length > 12)
                    throw new ErrorDatoNoValido("El nombre de usuario '" + pUsuarios[i].Login + "' no puede contener más de 12 caracteres");
                else if (pUsuarios[i].Login.Trim().Length < pUsuarios[i].Login.Length)
                    throw new ErrorDatoNoValido("El nombre de usuario '" + pUsuarios[i].Login + "' no puede contener espacios en blanco");

                //Login ya existente
                if (UsuarioAD.EstaLoginUsuarioAsignado(pUsuarios[i]))
                    throw new ErrorDatoNoValido("El nombre de usuario '" + pUsuarios[i].Login + "' ya existe.");
            }
        }

        /// <summary>
        /// Valida el formato de una contraseña
        /// </summary>
        /// <param name="pPassword">Password para validar</param>
        public static void ValidarFormatoPassword(string pPassword)
        {
            Regex expresionRegular = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-ZñÑüÜ]*$)^([a-zA-ZñÑüÜ0-9#_$*]{6,12})$");
            //Password nula
            if (pPassword == null)
                throw new ErrorDatoNoValido("La contraseña de usuario no puede ser nula");

            //Password con espacios
            if (pPassword.Contains(" "))
                throw new ErrorDatoNoValido("La contraseña de usuario no puede contener espacios en blanco");

            if (!expresionRegular.IsMatch(pPassword))
                throw new ErrorDatoNoValido("La contraseña de usuario debe contener entre 6 y 12 caracteres y al menos una letra y un número." + Environment.NewLine + "Se permiten los siguientes carácteres especiales: # _ $ *");
        }

        /// <summary>
        /// Valida el formato de un login de un usuario
        /// </summary>
        /// <param name="pPassword">Password para validar</param>
        public static void ValidarFormatoLogin(string pPassword)
        {
            Regex expresionRegular = new Regex(@"(^([a-zA-Z0-9-_.ñÑ]{4,12})$)");

            //Password nula
            if (pPassword == null)
                throw new ErrorDatoNoValido("El login del usuario no puede ser nula");

            //Password con espacios
            if (pPassword.Contains(" "))
                throw new ErrorDatoNoValido("El login del usuario no puede contener espacios en blanco");

            if (!expresionRegular.IsMatch(pPassword))
                throw new ErrorDatoNoValido("El login del usuario debe contener entre 4 y 12 de los siguientes caracteres (a-z,A-Z,-,_,.)");
        }

        /// <summary>
        /// Valida un conjunto de grupos de usuarios
        /// </summary>
        /// <param name="pGruposUsuarios">Conjunto de filas de grupo de usuario</param>
        private void ValidarGruposUsuarios(List<GrupoUsuario> pGruposUsuarios)
        {
            for (int i = 0; i < pGruposUsuarios.Count; i++)
            {
                //Nombre nulo
                if (pGruposUsuarios[i].Nombre == null)
                    throw new ErrorDatoNoValido("El nombre de un grupo no puede ser nulo");

                //Nombre con formato no válido (Cadena vacía)
                if (pGruposUsuarios[i].Nombre.Trim().Length == 0)
                    throw new ErrorDatoNoValido("El nombre de grupo '" + pGruposUsuarios[i].Nombre + "' no puede ser una cadena vacía");

                //Nombre superior a 150 caracteres
                if (pGruposUsuarios[i].Nombre.Length > 150)
                    throw new ErrorDatoNoValido("El nombre de grupo '" + pGruposUsuarios[i].Nombre + "' no puede contener más de 150 caracteres");

                //Descripción con formato no válido
                if (pGruposUsuarios[i].Descripcion != null)
                {
                    //Superior a 255 caracteres
                    if (pGruposUsuarios[i].Descripcion.Length > 255)
                        throw new ErrorDatoNoValido("La descripción del grupo '" + pGruposUsuarios[i].Nombre + "' no puede contener más de 255 caracteres");

                    //Si es vacía la ponemos a Null
                    if (pGruposUsuarios[i].Descripcion.Trim().Length == 0)
                        pGruposUsuarios[i] = null;
                }
            }
        }

        /// <summary>
        /// Valida un conjunto de roles de grupos usuarios en proyectos
        /// </summary>
        /// <param name="pRolesGruposUsuariosEnProyectos">Conjunto de filas de roles de grupos usuarios en proyectos</param>
        private void ValidarRolesGruposUsuariosEnProyectos(List<ProyectoRolGrupoUsuario> pRolesGruposUsuariosEnProyectos)
        {
            for (int i = 0; i < pRolesGruposUsuariosEnProyectos.Count; i++)
            {
                //Rol permitido nulo
                if (pRolesGruposUsuariosEnProyectos[i].RolPermitido == null)
                    throw new ErrorDatoNoValido("El rol permitido de un grupo de usuario en proyecto no puede ser nulo");

                //Rol permitido de formato no válido
                if ((pRolesGruposUsuariosEnProyectos[i].RolPermitido.Trim().Length == 0) || (pRolesGruposUsuariosEnProyectos[i].RolPermitido.Length > 16))
                    throw new ErrorDatoNoValido("El rol permitido de un grupo de usuario en proyecto no respeta las restricciones del sistema");

                //Rol no permitido de formato no válido
                if (pRolesGruposUsuariosEnProyectos[i].RolDenegado != null)
                {
                    if ((pRolesGruposUsuariosEnProyectos[i].RolDenegado.Trim().Length == 0) || (pRolesGruposUsuariosEnProyectos[i].RolDenegado.Length > 16))
                        throw new ErrorDatoNoValido("El rol denegado de un grupo usuario en proyecto no respeta las restricciones del sistema");
                }
            }
        }

        /// <summary>
        /// Valida un conjunto de roles de usuarios en proyectos
        /// </summary>
        /// <param name="pRolesUsuariosEnProyectos">Conjunto de filas de roles de usuarios en proyectos</param>
        private void ValidarRolesUsuariosEnProyectos(List<ProyectoRolUsuario> pRolesUsuariosEnProyectos)
        {
            for (int i = 0; i < pRolesUsuariosEnProyectos.Count; i++)
            {
                //Rol permitido nulo
                if (pRolesUsuariosEnProyectos[i].RolPermitido == null)
                    throw new ErrorDatoNoValido("El rol permitido de usuario en un proyecto no puede ser nulo");

                //Rol permitido de formato no válido
                if ((pRolesUsuariosEnProyectos[i].RolPermitido.Trim().Length == 0) || (pRolesUsuariosEnProyectos[i].RolPermitido.Length > 16))
                    throw new ErrorDatoNoValido("El rol permitido de usuario en proyecto no respeta las restricciones del sistema");

                //Rol no permitido de formato no válido
                if (pRolesUsuariosEnProyectos[i].RolDenegado != null)
                {
                    if ((pRolesUsuariosEnProyectos[i].RolDenegado.Trim().Length == 0) || (pRolesUsuariosEnProyectos[i].RolDenegado.Length > 16))
                        throw new ErrorDatoNoValido("El rol denegado de usuario en proyecto no respeta las restricciones del sistema");
                }
            }
        }

        /// <summary>
        /// Valida un conjunto de roles de grupos de usuarios
        /// </summary>
        /// <param name="pRolesGruposUsuarios">Conjunto de filas de roles de grupos de usuarios</param>
        private void ValidarRolesGruposUsuarios(List<GeneralRolGrupoUsuario> pRolesGruposUsuarios)
        {
            for (int i = 0; i < pRolesGruposUsuarios.Count; i++)
            {
                //Rol permitido nulo
                if (pRolesGruposUsuarios[i].RolPermitido == null)
                    throw new ErrorDatoNoValido("El rol permitido del grupo de usuario no puede ser nulo");

                //Rol permitido de formato no válido
                if ((pRolesGruposUsuarios[i].RolPermitido.Trim().Length == 0) || (pRolesGruposUsuarios[i].RolPermitido.Length > 16))
                    throw new ErrorDatoNoValido("El rol permitido del grupo de usuario no respeta las restricciones del sistema");

                //Rol no permitido de formato no válido
                if (pRolesGruposUsuarios[i].RolDenegado != null)
                {
                    if ((pRolesGruposUsuarios[i].RolDenegado.Trim().Length == 0) || (pRolesGruposUsuarios[i].RolDenegado.Length > 16))
                        throw new ErrorDatoNoValido("El rol denegado del grupo usuario no respeta las restricciones del sistema");
                }
            }
        }

        /// <summary>
        /// Valida un conjunto de roles de usuarios
        /// </summary>
        /// <param name="pRolesUsuarios">Conjunto de filas de roles de usuarios para validar</param>
        private void ValidarRolesUsuarios(List<GeneralRolUsuario> pRolesUsuarios)
        {
            for (int i = 0; i < pRolesUsuarios.Count; i++)
            {
                //Rol permitido nulo
                if (pRolesUsuarios[i].RolPermitido == null)
                    throw new ErrorDatoNoValido("El rol permitido de usuario no puede ser nulo");

                //Rol permitido de formato no válido
                if ((pRolesUsuarios[i].RolPermitido.Trim().Length == 0) || (pRolesUsuarios[i].RolPermitido.Length > 16))
                    throw new ErrorDatoNoValido("El rol permitido de usuario no respeta las restricciones del sistema");

                //Rol no permitido de formato no válido
                if (pRolesUsuarios[i].RolDenegado != null)
                {
                    if ((pRolesUsuarios[i].RolDenegado.Trim().Length == 0) || (pRolesUsuarios[i].RolDenegado.Length > 16))
                        throw new ErrorDatoNoValido("El rol denegado de usuario no respeta las restricciones del sistema");
                }
            }
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~UsuarioCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (UsuarioAD != null)
                        UsuarioAD.Dispose();
                }
                UsuarioAD = null;
            }
        }

        #endregion

        #region Propiedades

        public UsuarioAD UsuarioAD
        {
            get
            {
                return (UsuarioAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}
