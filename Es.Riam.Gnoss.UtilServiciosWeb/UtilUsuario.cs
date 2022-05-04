using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using System;
using System.Linq;
using System.Security.Principal;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class UtilUsuario
    {
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;

        public UtilUsuario(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
        }
        /// <summary>
        /// Valida nombre y contraseña del usuario
        /// </summary>
        /// <param name="pNombre">Nombre del usuario</param>
        /// <param name="pContraseña">Contraseña de la cuenta</param>
        /// <param name="pLanzarExcepciones">TRUE si se deben lanzar las excepciones producidas, FALSE en caso contrario</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto al que se va a conectar el usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto al que se conecta el usuario</param>
        /// <param name="pValidarPassword">Verdad si se debe validar el password</param>
        /// <returns>TRUE si usuario y contraseña son correctos, FALSE en caso contrario</returns>
        public bool ComprobarUsuario(string pNombre, string pContraseña, bool pLanzarExcepciones, Guid pOrganizacionID, Guid pProyectoID, bool pValidarPassword)
        {
            try
            {
                // Autenticamos el login para la organización (autenticación parcial)
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, null);
                DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
                Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = null;

                if (!string.IsNullOrEmpty(pNombre))
                {
                    dataWrapperUsuario = usuarioCN.AutenticarLogin(pNombre, false);
                    if (dataWrapperUsuario.ListaUsuario.Count > 0)
                    {
                        filaUsuario = dataWrapperUsuario.ListaUsuario.First();

                        //Autenticamos la password (autenticación completa)
                        if (pValidarPassword)
                        {
                            if (!usuarioCN.ValidarPasswordUsuario(filaUsuario, pContraseña))
                            {
                                throw new ErrorPassword();
                            }
                        }
                    }
                }

                usuarioCN.Dispose();
                return true;
            }
            catch (Exception)
            {
                if (pLanzarExcepciones)
                {
                    throw;
                }
                return false;
            }
        }
        /// <summary>
        /// Valida nombre y contraseña del usuario
        /// </summary>
        /// <param name="pNombre">Nombre del usuario</param>
        /// <param name="pContraseña">Contraseña de la cuenta</param>
        /// <param name="pLanzarExcepciones">TRUE si se deben lanzar las excepciones producidas, FALSE en caso contrario</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto al que se va a conectar el usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto al que se conecta el usuario</param>
        /// <param name="pValidarPassword">Verdad si se debe validar el password</param>
        /// <returns>TRUE si usuario y contraseña son correctos, FALSE en caso contrario</returns>
        public GnossIdentity ValidarUsuario(string pLogin, Guid pOrganizacionID, Guid pProyectoID)
        {
            // Autenticamos el login para la organización (autenticación parcial)
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, null);
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();

            if (!string.IsNullOrEmpty(pLogin))
            {
                dataWrapperUsuario = usuarioCN.AutenticarLogin(pLogin, false);
            }

            AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = null;
            AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad filaIdentidadProy = null;
            AD.EntityModel.Models.PersonaDS.Persona filaPersona = null;

            Guid perfilID = Guid.Empty;
            Guid identidadID = Guid.Empty;
            Guid? profesorID = null;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);

            if (dataWrapperUsuario.ListaUsuario.Count > 0)
            {
                filaUsuario = dataWrapperUsuario.ListaUsuario.First();

                DataWrapperPersona dataWrapperPersona = new PersonaCN(mEntityContext, mLoggingService, mConfigService, null).ObtenerPersonaPorUsuario(filaUsuario.UsuarioID);

                if (dataWrapperPersona.ListaPersona.Count > 0)
                {
                    filaPersona = dataWrapperPersona.ListaPersona.First();
                }
            }

            if (pProyectoID.Equals(ProyectoAD.MetaProyecto) && (dataWrapperUsuario.ListaProyectoUsuarioIdentidad.Count > 0) && filaUsuario != null)
            {
                filaIdentidadProy = dataWrapperUsuario.ListaProyectoUsuarioIdentidad.First();

                identidadID = filaIdentidadProy.IdentidadID;
                perfilID = identidadCN.ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(filaUsuario.NombreCorto, pProyectoID, "", false)[1];
            }
            else
            {
                if (filaPersona != null) 
                {
                    try
                    {
                        object array = identidadCN.ObtenerIdentidadIDDePersonaEnProyecto(pProyectoID, filaPersona.PersonaID);

                        if (array != null)
                        {
                            Guid[] ids = (Guid[])array;
                            identidadID = ids[0];
                            perfilID = ids[1];
                        }
                        else
                        {
                            identidadID = UsuarioAD.Invitado;
                            perfilID = UsuarioAD.Invitado;
                        }
                    }
                    catch
                    {
                        throw new ErrorAccesoProyecto(pProyectoID.ToString());
                    }
                }
            }

            if (filaPersona != null)
            {
                profesorID = identidadCN.ObtenerProfesorID(filaPersona.PersonaID);
            }

            identidadCN.Dispose();

            //Crear la identidad
            GnossIdentity identity = new GnossIdentity();
            if (filaUsuario != null)
            {
                identity.UsuarioID = filaUsuario.UsuarioID;
                identity.IdentidadID = identidadID;
                identity.Login = filaUsuario.Login;
                identity.OrganizacionID = pOrganizacionID;
                identity.ProyectoID = pProyectoID;
                identity.PerfilProfesorID = profesorID;

                if (filaPersona == null)
                {
                    throw new ErrorUsuarioSinPersona();
                }
                identity.PersonaID = filaPersona.PersonaID;

                if (filaUsuario.FechaCambioPassword.HasValue)
                {
                    identity.FechaCambioPassword = filaUsuario.FechaCambioPassword;
                }

                //Obtengo la identidad y el proyecto al que se conecta el usuario
                identity.EstaPasswordAutenticada = true;

                //Calculo los permisos del usuario
                //Calculamos el rol permitido final de usuario
                identity.RolPermitidoGeneral = usuarioCN.CalcularRolFinalUsuario(filaUsuario);

                //Calculo el rol en las organizaciones de las que es miembro
                DataWrapperUsuario usuDW = usuarioCN.ObtenerRolesOrganizaciones(filaUsuario.UsuarioID);

                foreach (AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario filaOrganizacionRolUsuario in usuDW.ListaOrganizacionRolUsuario)
                {
                    if (!identity.ListaRolesOrganizacion.ContainsKey(filaOrganizacionRolUsuario.OrganizacionID))
                    {
                        identity.ListaRolesOrganizacion.Add(filaOrganizacionRolUsuario.OrganizacionID, usuarioCN.CalcularRolFinalOrganizacion(filaOrganizacionRolUsuario.UsuarioID, filaOrganizacionRolUsuario.OrganizacionID));
                    }
                }
                identity.RolPermitidoProyecto = (ulong)(0ul);

                if (!identidadID.Equals(UsuarioAD.Invitado))
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null);
                    identity.RolPermitidoProyecto = proyCN.CalcularRolFinalUsuarioEnProyecto(filaUsuario.UsuarioID, pLogin, pOrganizacionID, pProyectoID);
                    proyCN.Dispose();
                    usuarioCN.Dispose();
                }
                else
                {
                    identity.RolPermitidoProyecto = 0000000000000000;
                    identity.EsIdentidadInvitada = true;
                }
            }
            return identity;
        }

        /// <summary>
        /// Conecta a un usuario invitado que no existe en el sistema
        /// </summary>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        public void CrearUsuarioInvitado(Guid pOrganizacionID, Guid pProyectoID)
        {
            //Crear la identidad
            GnossIdentity identity = new GnossIdentity();

            //Establezco los valores del invitado
            identity.UsuarioID = UsuarioAD.Invitado;
            identity.PersonaID = UsuarioAD.Invitado;
            identity.IdentidadID = UsuarioAD.Invitado;

            identity.EstaPasswordAutenticada = true;
            identity.EsUsuarioInvitado = true;
            identity.EsIdentidadInvitada = true;
            identity.Login = "Invitado";

            //Le conecto al metaproyecto
            identity.OrganizacionID = pOrganizacionID;
            identity.ProyectoID = pProyectoID;

            //GnossWebPrincipal principal = new GnossWebPrincipal((WindowsIdentity)Thread.CurrentPrincipal.Identity);
            //principal.GnossIdentity = identity;
            //Thread.CurrentPrincipal = principal;

            //TODO Javier Web a Middleware para recuperar usuario desde el this.User de la web
            //UtilPeticion.AgregarObjetoAPeticionActual("GnossIdentity", identity);

            //Establecemos los roles permitidos del usuario
            identity.RolPermitidoGeneral = (ulong)(0ul);
            identity.RolPermitidoProyecto = (ulong)(0ul);
        }


        /// <summary>
        /// Obtiene atraves de un guid y el ParametroGeneralDS la fila de parametros generales del proyecto correspondiente
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar</param>
        /// <param name="pPage">Página</param>
        /// <returns>Fila del parametro general</returns>
        public ParametroGeneral ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        {
            ParametroGeneral filaParametroGeneral = null;

            ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            GestorParametroGeneral gestorParametroGeneral;
            gestorParametroGeneral = paramCL.ObtenerParametrosGeneralesDeProyecto(pProyectoID);
            paramCL.Dispose();
            filaParametroGeneral = gestorParametroGeneral.ListaParametroGeneral.FirstOrDefault(parametroGeneral => parametroGeneral.ProyectoID.Equals(pProyectoID));

            return filaParametroGeneral;
        }
    }

    public class GnossWebPrincipal : WindowsPrincipal
    {
        #region Miembros

        private GnossIdentity mGnossIdentity;

        #endregion

        public GnossWebPrincipal(WindowsIdentity ntIdentity)
            : base(ntIdentity)
        {
        }

        #region Propiedades

        public override IIdentity Identity
        {
            get
            {
                return mGnossIdentity;
            }
        }

        public GnossIdentity GnossIdentity
        {
            get
            {
                return mGnossIdentity;
            }
            set
            {
                mGnossIdentity = value;
            }
        }

        #endregion

    }
}
