using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Es.Riam.Gnoss.Logica.ServiciosGenerales
{
    /// <summary>
    /// L�gica referente a Organizaci�n
    /// </summary>
    public class OrganizacionCN : BaseCN, IDisposable
    {

        #region Miembros

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;

        #endregion

        #region Constructores
        /// <summary>
        /// Constructor para OrganizacionCN
        /// </summary>
        public OrganizacionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            this.OrganizacionAD = new OrganizacionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuraci�n de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public OrganizacionCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            OrganizacionAD = new OrganizacionAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region M�todos generales

        #region P�blicos

        public bool GuardarCambiosPrivacidadOrganizacion(Organizacion pFilaOrganizacion, ConfiguracionGnossOrg pFilaConfigOrg)
        {
            return OrganizacionAD.GuardarCambiosPrivacidadOrganizacion(pFilaOrganizacion, pFilaConfigOrg);
        }

        /// <summary>
        /// Carga las tablas "Organizacion" y "AdministradorOrganizacion" para la organizaci�n pasada por par�metro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarAdministradoresdeOrganizacion(Guid pOrganizacionID)
        {
            return OrganizacionAD.CargarAdministradoresdeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Carga SOLO la tabla "PersonasVisiblesDeOrg" para las organizaciones en las que una persona es visible
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarOrganizacionesDePersonaVisible(Guid pPersonaID)
        {
            return OrganizacionAD.CargarOrganizacionesDePersonaVisible(pPersonaID);
        }
        /// <summary>
        /// Lista con las personas que pertenecen a la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Id de la organizaci�n</param>
        /// <returns>Lista con los usuarios que pertenecen a la organizacion</returns>
        public List<Guid> ObetenerPersonasDeLaOrganizacion(Guid pOrganizacionID)
        {
            return OrganizacionAD.ObetenerPersonasDeLaOrganizacion(pOrganizacionID);
        }
        /// <summary>
        /// Actualiza el valor del registro autom�tico en una comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Id de la organizacion</param>
        /// <param name="pProyectoID">Id de la comunidad</param>
        public void ActualizarRegAuto(Guid pOrganizacionID, Guid pProyectoID)
        {
            OrganizacionAD.ActualizarRegAuto(pOrganizacionID, pProyectoID);
        }
        /// <summary>
        /// Carga SOLO la tabla  "PersonaVinculoOrganizacion" carga ligera  para la organizaci�n pasada por par�metro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarPersonasVinculoOrgDeOrganizacion(Guid pOrganizacionID)
        {
            return OrganizacionAD.CargarPersonasVinculoOrgDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Carga SOLO la tabla "PersonasVisiblesDeOrg" para la organizaci�n pasada por par�metro (ordenada por "Orden")
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarPersonasVisiblesDeOrganizacion(Guid pOrganizacionID)
        {
            return OrganizacionAD.CargarPersonasVisiblesDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Indica si la persona es visible en la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Booleano que indica si la persona es visible en la organizacion</returns>
        public bool EsPersonaVisibleEnOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            return OrganizacionAD.EsPersonaVisibleEnOrganizacion(pOrganizacionID, pPersonaID);
        }

        /// <summary>
        /// Carga las organizaciones que administra el usuario cuyo identificador se pasa por par�metro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion CargarOrganizacionesAdministraUsuario(Guid pUsuarioID)
        {
            return OrganizacionAD.CargarOrganizacionesAdministraUsuario(pUsuarioID);
        }

        /// <summary>
        /// Indica si el usuario ers el �nico administrador de una organizaci�n
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>True si es el �nico administrador de alguna organizaci�n</returns>
        public bool EsUsuarioAdministradorUnicoDeOrganizacion(Guid pUsuarioID)
        {
            return OrganizacionAD.EsUsuarioAdministradorUnicoDeOrganizacion(pUsuarioID);
        }
        /// <summary>
        /// Obtiene si esta activada el registro autom�tico en la comunidad
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pOrganizacionID">Id de la organizaci�n</param>
        /// <returns>Devuelve cierto en caso de que este activado el registro autom�tico y falso en otro caso</returns>
        public Dictionary<Guid, bool> ObtenerParametroRegistroautomatico(List<Guid> listaComunidades, Guid pOrganizacionID)
        {
            return OrganizacionAD.ObtenerParametroRegistroautomatico(listaComunidades, pOrganizacionID);
        }
        /// <summary>
        /// Comprueba si existe la organizaci�n cuyo nombre se pasa por par�metro
        /// </summary>
        /// <param name="pNombreOrganizacion">Nombre de organizaci�n</param>
        /// <returns>TRUE si la organizaci�n ya existe, FALSE en caso contrario</returns>
        public bool ExisteOrganizacionPorOrganizacionID(string pOrganizacionID)
        {
            return OrganizacionAD.ExisteOrganizacionPorOrganizacionID(pOrganizacionID);
        }


        /// <summary>
        /// Comprueba si existe la organizaci�n cuyo nombre se pasa por par�metro
        /// </summary>
        /// <param name="pNombreOrganizacion">Nombre de organizaci�n</param>
        /// <returns>TRUE si la organizaci�n ya existe, FALSE en caso contrario</returns>
        public bool ExisteOrganizacion(string pNombreOrganizacion)
        {
            return OrganizacionAD.ExisteOrganizacion(pNombreOrganizacion);
        }

        /// <summary>
        /// Obtiene Organizacion y PersonaVinculoOrganizacion de las organizaciones en las que participa una persona
        /// </summary>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDePersona(Guid pPersonaID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesDePersona(pPersonaID);
        }

        /// <summary>
        /// Obtiene Organizacion y PersonaVinculoOrganizacion de las organizaciones en las que participan las personas pasadas por par�metro
        /// </summary>
        /// <param name="pListaPersonaID">Lista de identificadores de personas</param>
        /// <returns>Dataset de organizaciones con las organizaciones y sus v�nculos con personas</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDeListaPersona(List<Guid> pListaPersonaID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesDeListaPersona(pListaPersonaID);
        }

        /// <summary>
        /// Obtiene las organizaciones a las que pertenecen las personas de la estructura del proyecto pasado por par�metro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDePersonasDeEstructuraDeProyectoCargaLigera(Guid pProyectoID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesDePersonasDeEstructuraDeProyectoCargaLigera(pProyectoID);
        }

        /// <summary>
        /// Obtiene el id autonum�rico que se le asigna a cada organizaci�n para crear la tabla BASE
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns></returns>
        public int ObtenerTablaBaseOrganizacionIDOrganizacionPorID(Guid pOrganizacionID)
        {
            return OrganizacionAD.ObtenerTablaBaseOrganizacionIDOrganizacionPorID(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene una Organizaci�n a partir de su Identificador
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorID(Guid pOrganizacionID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionPorID(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene una Organizaci�n a partir de su Identificador CARGA LIGERA. "Organizacion"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorIDCargaLigera(Guid pOrganizacionID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionPorIDCargaLigera(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene una Organizaci�n a partir de su Identificador
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorID(List<Guid> pListaOrganizacionID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesPorID(pListaOrganizacionID);
        }

        /// <summary>
        /// Obtiene una Organizaci�n a partir de su Identificador CARGA LIGERA. "Organizacion"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIDCargaLigera(List<Guid> pListaOrganizacionID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesPorIDCargaLigera(pListaOrganizacionID);
        }

        /// <summary>
        /// Obtiene el ID de una organizacion a partir de su id de tesauro (null si el usuario no existe)
        /// </summary>
        /// <param name="pPerfilID">Id del perfil</param>
        /// <returns></returns>
        public Guid? ObtenerOrganizacionIDPorIDTesauro(Guid pTesauroID)
        {
            return OrganizacionAD.ObtenerOrganizacionIDPorIDTesauro(pTesauroID);
        }
        /// <summary>
        /// CARGA LIGERA. Obtiene las tablas "Organizacion" y "PersonaVinculoOrganizacion" de las organizaciones de una lista de identidades (se entiende que para identidades de tipo 1,2 0 3)  NO SE OBTIENEN LOS CAMPOS DE ORGANIZACION.LOGOTIPO NI DE PERSONAVINCULOORGANIZACION.FOTO
        /// </summary>
        /// <param name="pIdentidades">Lista de identificadores de las identidades</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDeIdentidades(List<Guid> pIdentidades)
        {
            return OrganizacionAD.ObtenerOrganizacionesDeIdentidades(pIdentidades);
        }

        /// <summary>
        /// Obtiene el identificador de una organizaci�n a partir de su nombre corto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto de la organizaci�n</param>
        /// <returns>Identificador de organizaci�n</returns>
        public Guid ObtenerOrganizacionesIDPorNombre(string pNombreCorto)
        {
            return OrganizacionAD.ObtenerOrganizacionesIDPorNombre(pNombreCorto);
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de identidades cargardas en un dataSet.
        /// </summary>
        /// <param name="pDataWrapperIdentidad">DataSet con las identidades de organizaciones</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesDeIdentidadesCargadas(DataWrapperIdentidad pDataWrapperIdentidad)
        {
            return OrganizacionAD.ObtenerOrganizacionesDeIdentidadesCargadas(pDataWrapperIdentidad);
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de una lista de identidades "Organizacion", "OrganizacionParticipaProy", "ConfiguracionGnossOrg"
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades de organizaciones</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIdentidad(List<Guid> pListaIdentidades)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesPorIdentidad(pListaIdentidades);
        }

        /// <summary>
        /// Obtiene las organizaciones a partir de una identidad
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIdentidad(Guid pIdentidad)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesPorIdentidad(pIdentidad);
        }

        /// <summary>
        /// Obtiene el nombre de una Organizaci�n a partir de su Identificador
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de organizaci�n</returns>
        public Organizacion ObtenerNombreOrganizacionPorID(Guid pOrganizacionID)
        {
            return this.OrganizacionAD.ObtenerNombreOrganizacionPorID(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene el nombre de una Organizaci�n a partir del identificador de su identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad de la organizacion</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerNombreOrganizacionPorIdentidad(Guid pIdentidadID)
        {
            return this.OrganizacionAD.ObtenerNombreOrganizacionPorIdentidad(pIdentidadID);
        }

        /// <summary>
        /// Obtiene los nombres de las organizaciones a partir de sus Identificadores.
        /// </summary>
        /// <param name="pOrganizacionesIDs">Identificadores de las organizaci�nes</param>
        /// <returns>Nombres de las organizaciones</returns>
        public Dictionary<Guid, KeyValuePair<string, string>> ObtenerNombreOrganizacionesPorIDs(List<Guid> pOrganizacionesIDs)
        {
            return this.OrganizacionAD.ObtenerNombreOrganizacionesPorIDs(pOrganizacionesIDs);
        }

        /// <summary>
        /// Obtiene las Organizaciones en las que participa un Usuario (Organizaciones Gnoss, Empresas Proveedoras, Empresas Colaboradoras)
        /// </summary>
        /// <param name="pUsuarioID">Identificador del Usuario</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesParticipaUsuario(Guid pUsuarioID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesParticipaUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las organizaciones que solicitan acceso a un proyecto concreto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesSolicitanAccesoProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return this.OrganizacionAD.ObtenerOrganizacionesSolicitanAccesoProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las organizaciones en las que participa el usuario actual (Organizaciones Gnoss, Empresas Proveedoras, Empresas Colaboradoras)
        /// </summary>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesParticipaPropioUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            return OrganizacionAD.ObtenerOrganizacionesParticipaUsuario(pUsuario.UsuarioID);
        }

        /// <summary>
        /// Elimina Organizaciones
        /// </summary>
        /// <param name="pOrganizacionDW">Organizaciones a eliminar</param>
        public void EliminarOrganizaciones(DataWrapperOrganizacion pOrganizacionDW)
        {
            try
            {
                if (Transaccion != null)
                {
                    mEntityContext.SaveChanges();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        mEntityContext.SaveChanges();
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
                //Error interno de la aplicaci�n	
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
        /// Guarda Organizaciones
        /// </summary>
        /// <param name="pOrganizacionDW">Organizaciones a guardar</param>
        public void GuardarOrganizaciones()
        {
            //TODO: Castillo revisar permiso
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesOrganizacion.EditarOrganizaciones);

            try
            {
                if (Transaccion != null)
                {
                    mEntityContext.SaveChanges();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        mEntityContext.SaveChanges();

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
                //Error interno de la aplicaci�n
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
        /// Actualiza Organizaciones 
        /// </summary>
        /// <param name="pOrganizacionDW">Lista de Organizaciones para actualizar</param>
        public void ActualizarOrganizaciones(DataWrapperOrganizacion pOrganizacionDW)
        {
            ActualizarOrganizaciones(pOrganizacionDW, true);
        }

        /// <summary>
        /// Actualiza Organizaciones 
        /// </summary>
        /// <param name="pOrganizacionDW">Lista de Organizaciones para actualizar</param>
        /// <param name="pComprobarAutorizacion">Verdad si se debe de comprobar la autorizaci�n del usuario</param>
        public void ActualizarOrganizaciones(DataWrapperOrganizacion pOrganizacionDW, bool pComprobarAutorizacion)
        {

            if (pComprobarAutorizacion)
            {
                //TODO: Castillo revisar permiso
                //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesOrganizacion.EditarOrganizaciones);
            }

            try
            {
                if (Transaccion != null)
                {
                    this.OrganizacionAD.ActualizarOrganizaciones();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        this.OrganizacionAD.ActualizarOrganizaciones();

                        if (pOrganizacionDW != null)
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
                //Error interno de la aplicaci�n	
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
        /// Comprueba si se puede eliminar la organizaci�n
        /// </summary>
        /// <param name="pFilaOrganizacion">Fila de la organizaci�n</param>
        /// <returns>TRUE si se puede eliminar, FALSE en caso contrario</returns>
        public bool SePuedeEliminarOrganizacion(DataRow pFilaOrganizacion)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool sePuedeEliminar = !personaCN.TienePersonasOrganizacion((Guid)pFilaOrganizacion["OrganizacionID"]);
            personaCN.Dispose();

            return sePuedeEliminar;
        }

        /// <summary>
        /// Ontiene las organizaciones vinculadas a la persona pasada por par�metro "Organizacion","PersonaVinculoOrganizacion","TagOrganizacion"
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesVinculadasAPersona(Guid pPersonaID)
        {
            return OrganizacionAD.ObtenerOrganizacionesVinculadasAPersona(pPersonaID, false);
        }

        /// <summary>
        /// Obtiene la fila de Configuraci�nGnossOrg de una organizaci�n
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerConfiguracionGnossOrg(Guid pOrganizacionID)
        {
            return OrganizacionAD.ObtenerConfiguracionGnossOrg(pOrganizacionID);
        }


        /// <summary>
        /// Obtiene las organizaciones vinculadas a la persona pasada por par�metro 
        /// Carga las tablas Organizacion, OrganizacionEmpresa, OrganizacionClase, PersonaVinculoOrganizacion
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pOrganizacionID">TRUE si debe hacerse una carga pesada, FALSE si debe ser ligera</param>
        /// <returns>Dataset de organizaciones</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionVinculadaAPersona(Guid pPersonaID, Guid pOrganizacionID)
        {
            return OrganizacionAD.ObtenerOrganizacionVinculadaAPersona(pPersonaID, pOrganizacionID);
        }

        /// <summary>
        /// Ontiene las organizaciones vinculadas a la persona pasada por par�metro "Organizacion","PersonaVinculoOrganizacion","TagOrganizacion"
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pCargaPesada">TRUE si debe hacerse una carga pesada, FALSE si la carga debe ser ligera</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesVinculadasAPersona(Guid pPersonaID, bool pCargaPesada)
        {
            return OrganizacionAD.ObtenerOrganizacionesVinculadasAPersona(pPersonaID, pCargaPesada);
        }

        /// <summary>
        /// Obtiene la organizaci�n a partir de su identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionDeIdentidad(Guid pIdentidadID)
        {
            return OrganizacionAD.ObtenerOrganizacionDeIdentidad(pIdentidadID);
        }

        /// <summary>
        /// Obtiene la organizaci�n a partir de su perfil
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionPorPerfil(Guid pPerfil)
        {
            return OrganizacionAD.ObtenerOrganizacionPorPerfil(pPerfil);
        }

        /// <summary>
        /// Obtiene la OrganizacionDS.OrganizacionRow de una organizacion a partir de su identidad
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad</param>
        /// <returns>Fila Organizaci�n</returns>
        public List<Organizacion> ObtenerOrganizacionPorIdentidadCargaLigera(Guid pIdentidad)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pIdentidad);

            DataWrapperOrganizacion orgDW = ObtenerOrganizacionesPorIdentidadesCargaLigera(lista);

            if (orgDW.ListaOrganizacion.Count > 0)
            {
                return orgDW.ListaOrganizacion;
            }
            return null;
        }

        /// <summary>
        /// Obtiene el n�mero de alumnos de una clase
        /// </summary>
        /// <param name="pOrganizacionID">GUID de la clase</param>
        /// <returns>N�mero de alumnos de la clase</returns>
        public int ObtenerNumeroAlumnosDeClase(Guid pOrganizacionID)
        {
            return OrganizacionAD.ObtenerNumeroAlumnosDeClase(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene el n�mero de miembros de una organizaci�n en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">GUID de la organizaci�n</param>
        /// <param name="pProyectoID">GUID del proyecto</param>
        /// <param name="pVisibles">True para obtener solo los visibles, False para obtener todos</param>
        /// <returns>N�mero de miembros</returns>
        public int ObtenerNumeroMiembrosDeOrganizacionEnProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pVisibles)
        {
            return OrganizacionAD.ObtenerNumeroMiembrosDeOrganizacionEnProyecto(pOrganizacionID, pProyectoID, pVisibles);
        }

        /// <summary>
        /// Comprueba si el usuario administra alguna clase
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>TRUE si administra alguna clase, FALSE en caso contrario</returns>
        public bool ComprobarUsuarioAdministraAlgunaClase(Guid pUsuarioID)
        {
            return OrganizacionAD.ComprobarUsuarioAdministraAlgunaClase(pUsuarioID);
        }

        /// <summary>
        /// Verdad si una organizaci�n es una clase
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns></returns>
        public bool ComprobarOrganizacionEsClase(Guid pOrganizacionID)
        {
            return OrganizacionAD.ComprobarOrganizacionEsClase(pOrganizacionID);
        }

        /// <summary>
        /// Verdad si una organizaci�n es una clase de primaria
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns></returns>
        public bool ComprobarOrganizacionEsClasePrimaria(Guid pOrganizacionID)
        {
            return OrganizacionAD.ComprobarOrganizacionEsClasePrimaria(pOrganizacionID);
        }
        /// <summary>
        /// Comprueba si una persona es visible en una organizaci�n
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>True si es visible, false si no lo es</returns>
        public bool ComprobarPersonaEsVisibleEnOrg(Guid pPersonaID, Guid pOrganizacionID)
        {
            return OrganizacionAD.ComprobarPersonaEsVisibleEnOrg(pPersonaID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene una carga ligera de las organizaciones a partir de una lista de identidades pasada por par�metro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorIdentidadesCargaLigera(List<Guid> pListaIdentidades)
        {
            return OrganizacionAD.ObtenerOrganizacionesPorIdentidadesCargaLigera(pListaIdentidades);
        }

        /// <summary>
        /// Obtiene las tablas "Organizacion" y "OrganizacionParticipaProy" del proyecto pasado por p�rametro
        /// </summary>
        /// <param name="pProyecto">Identificador del proyecto</param>
        /// <returns>Dataset de organizaci�n</returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesPorSusIdentidadesDeProyecto(Guid pProyecto)
        {
            return OrganizacionAD.ObtenerOrganizacionesPorSusIdentidadesDeProyecto(pProyecto);
        }



        /// <summary>
        /// Comprueba si ya existe el email para el v�nculo de la persona pasada por par�metro con la organizaci�n
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pPersonaID">Identificador de la persona a comprobar</param>
        /// <returns>TRUE si el email ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmailPersonaVinculoOrganizacion(string pEmail, Guid pPersonaID)
        {
            return OrganizacionAD.ExisteEmailPersonaVinculoOrganizacion(pEmail, pPersonaID);
        }

        /// <summary>
        /// Comprueba si una organizaci�n tiene tesauro propio
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n</param>
        /// <returns>TRUE si tiene tesauro, FALSE en caso contrario</returns>
        public bool TieneTesauroPropio(Guid pOrganizacionID)
        {
            return OrganizacionAD.TieneTesauroPropio(pOrganizacionID);
        }

        /// <summary>
        /// Comprueba si una organizaci�n tiene  base de recursos propia
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n</param>
        /// <returns>TRUE si tiene base de recursos, FALSE en caso contrario</returns>
        public bool TieneBaseDeRecursos(Guid pOrganizacionID)
        {
            return OrganizacionAD.TieneBaseDeRecursos(pOrganizacionID);
        }



        /// <summary>
        /// Obtiene la fila de la vinculaci�n de una persona a una organizaci�n.
        /// </summary>
        /// <param name="pOrganizacionID">ID de la org</param>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <returns>Fila de la vinculaci�n de una persona a una organizaci�n</returns>
        public string ObtenerCargoPersonaVinculoOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            return OrganizacionAD.ObtenerCargoPersonaVinculoOrganizacion(pOrganizacionID, pPersonaID);
        }

        /// <summary>
        /// Comprueba si existe una organizaci�n con el nombre corto pasado por par�metro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>TRUE si lo encuentra, FALSE en caso contrario</returns>
        public bool ExisteNombreCortoEnBD(string pNombreCorto)
        {
            return OrganizacionAD.ExisteNombreCortoEnBD(pNombreCorto);
        }

        /// <summary>
        /// Obtiene los nombres cortos que empizan con los caracteres introducidos.
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>nombres cortos que empizan con los caracteres introducidos</returns>
        public List<string> ObtenerNombresCortosEmpiezanPor(string pNombreCorto)
        {
            return OrganizacionAD.ObtenerNombresCortosEmpiezanPor(pNombreCorto);
        }

        /// <summary>
        /// Comprueba si una organizaci�n tiene en base de datos creado un perfil de organizaci�n
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n</param>
        /// <returns>TRUE si tiene perfil de organizaci�n, FALSE en caso contrario</returns>
        public bool TienePerfil(Guid pOrganizacionID)
        {
            return OrganizacionAD.TienePerfil(pOrganizacionID);
        }

        /// <summary>
        /// Valida el formato del nombre corto de la organizaci�n
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>TRUE si el nombre corto de la organizaci�n es correcto, FALSE en caso contrario</returns>
        public static bool ValidarFormatoNombreCortoOrganizacion(string pNombreCorto)
        {
            if (pNombreCorto.Contains(" "))
            {
                return false;
            }
            Regex expresionRegular = new Regex(@"(^([a-zA-Z0-9-��]{4,30})$)");

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
        /// Comprueba si el usuario pasado como par�metro es administrador de la clase indicada
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n de tipo clase</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>TRUE si el usuario administra la clase, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorClase(Guid pOrganizacionID, Guid pUsuarioID)
        {
            return OrganizacionAD.EsUsuarioAdministradorClase(pOrganizacionID, pUsuarioID);
        }

        /// <summary>
        /// Comprueba si hay alguna persona visible en la organizaci�n
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>TRUE si hay alguna persona visible, FALSE en caso contrario</returns>
        public bool HayPersonasVisibles(Guid pOrganizacionID)
        {
            return OrganizacionAD.HayPersonasVisibles(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene los tags de una organizaci�n en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public string ObtenerTagsDeOrganizacionEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return OrganizacionAD.ObtenerTagsDeOrganizacionEnProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las organizaciones que participan en un proyecto (OrganizacionID, Nombre, NombreCorto y Alias)
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperOrganizacion ObtenerOrganizacionesParticipanEnProyecto(Guid pProyectoID)
        {
            return OrganizacionAD.ObtenerOrganizacionesParticipanEnProyecto(pProyectoID);
        }

        /// <summary>
        /// Comprueba si la organizaci�n participa en el proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>true si la organizaci�n participa en el proyecto</returns>
        public bool ParticipaOrganizacionEnProyecto(Guid pProyectoID, Guid pOrganizacionID)
        {
            return OrganizacionAD.ParticipaOrganizacionEnProyecto(pProyectoID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene las filas de la tabla PersonaVinculoOrganizacion cuya organizacion comienzapor pInicio
        /// </summary>
        /// <param name="pInicio">caracteres por los que comienza la organizacion</param>
        /// <returns></returns>
        public List<PersonaVinculoOrganizacion> ObtenerFilasPersonaVincOrganizacion(string pInicio)
        {
            return OrganizacionAD.ObtenerFilasPersonaVincOrganizacion(pInicio);
        }

        /// <summary>
        /// Obtiene las filas de la tabla Organizacion cuya organizacion comienzapor pInicio
        /// </summary>
        /// <param name="pInicio">caracteres por los que comienza la organizacion</param>
        /// <returns></returns>
        public List<Organizacion> ObtenerFilasOrganizaciones(string pInicio)
        {
            return OrganizacionAD.ObtenerFilasOrganizaciones(pInicio);
        }

        /// <summary>
        /// Actualiza las coordenadas de la organizacion indicada
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Organizacion</param>
        /// <param name="pCoordenadas">Coordenadas del Logo</param>
        public void ActualizarCoordenadasOrganizacion(Guid pOrganizacionID, string pCoordenadas)
        {
            OrganizacionAD.ActualizarCoordenadasOrganizacion(pOrganizacionID, pCoordenadas);
        }

        /// <summary>
        /// Actualiza las coordenadas de la persona en la organizacion indicada
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Organizacion</param>
        /// <param name="pPersonaID">ID de la Persona</param>
        /// <param name="pCoordenadas">Coordenadas de la foto</param>
        public void ActualizarCoordenadasPersonaVincOrganizacion(Guid pOrganizacionID, Guid pPersonaID, string pCoordenadas)
        {
            OrganizacionAD.ActualizarCoordenadasPersonaVincOrganizacion(pOrganizacionID, pPersonaID, pCoordenadas);
        }

        /// <summary>
        /// Actualiza el n�mero de la versi�n de la foto de la organizaci�n
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Orgnizacion</param>
        public void ActualizarVersionFotoOrganizacion(Guid pOrganizacionID)
        {
            OrganizacionAD.ActualizarVersionFotoOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Actualiza el n�mero de la versi�n de la foto de la persona en la organizaci�n
        /// </summary>
        /// <param name="pOrganizacionID">ID de la Orgnizacion</param>
        /// <param name="pPersonaID">ID de la Persona</param>
        public void ActualizarVersionFotoPersonaVincOrganizacion(Guid pOrganizacionID, Guid pPersonaID)
        {
            OrganizacionAD.ActualizarVersionFotoPersonaVincOrganizacion(pOrganizacionID, pPersonaID);
        }

        #endregion

        #region Protegidos

        /// <summary>
        /// Valida la lista de organizaciones pasada como par�metro
        /// </summary>
        /// <param name="pOrganizaciones">lista de Organizaciones para validar</param>
        protected void ValidarOrganizaciones(List<Organizacion> pOrganizaciones)
        {
            //if (pOrganizaciones != null)
            //{
            //    for (int i = 0; i < pOrganizaciones.Length; i++)
            //    {
            //        //Nombre vac�o
            //        if (pOrganizaciones[i]["Nombre"].ToString().Trim().Length == 0)
            //        {
            //            throw new ErrorDatoNoValido("El nombre de la organizaci�n no puede estar vacio");
            //        }

            //        //Nombre superior a 255 caracteres
            //        if (pOrganizaciones[i]["Nombre"].ToString().Length > 255)
            //        {
            //            throw new ErrorDatoNoValido("El nombre de la organizaci�n '" + pOrganizaciones[i]["Nombre"] + "' no puede contener m�s de 255 caracteres");
            //        }

            //        //CIF de formato incorrecto

            //        if (pOrganizaciones[i]["CIF"].ToString().Trim().Length == 0)
            //        {
            //            //throw new ErrorDatoNoValido("El CIF de la organizaci�n '" + pOrganizaciones[i]["Nombre"] + "' no puede ser una cadena vac�a");
            //        }
            //        else if(!ValidarNIFyCIF(pOrganizaciones[i]["CIF"].ToString()))
            //        {
            //            //throw new ErrorDatoNoValido("El CIF de la organizaci�n '" + pOrganizaciones[i]["Nombre"] + "' no tiene un formato correcto");
            //        }

            //        //Tel�fono no v�lido
            //        if (!pOrganizaciones[i].IsNull("Telefono"))
            //        {
            //            //Si es vac�o lo ponemos a Null
            //            if (pOrganizaciones[i]["Telefono"].ToString().Trim().Length == 0)
            //            {
            //                pOrganizaciones[i]["Telefono"] = null;
            //            }

            //                //Formato incorrecto
            //            else if (!ValidarTelefono(pOrganizaciones[i]["Telefono"].ToString()))
            //            {
            //                throw new ErrorDatoNoValido("El tel�fono de la organizaci�n '" + pOrganizaciones[i]["Nombre"] + "' tiene un formato incorrecto. El tel�fono tiene que ser un dato num�rico");
            //            }
            //        }

            //        //Fax no v�lido
            //        if (!pOrganizaciones[i].IsNull("Fax"))
            //        {
            //            //Si es vac�o lo ponemos a Null
            //            if (pOrganizaciones[i]["Fax"].ToString().Trim().Length == 0)
            //            {
            //                pOrganizaciones[i]["Fax"] = null;
            //            }

            //                //Formato incorrecto
            //            else if (!ValidarTelefono(pOrganizaciones[i]["Fax"].ToString()))
            //            {
            //                throw new ErrorDatoNoValido("El fax de la organizaci�n '" + pOrganizaciones[i]["Nombre"] + "' tiene un formato incorrecto. El fax tiene que ser un dato num�rico");
            //            }
            //        }

            //        //Email no v�lido
            //        if (!pOrganizaciones[i].IsNull("Email"))
            //        {
            //            //Si es vac�o lo ponemos a Null
            //            if (pOrganizaciones[i]["Email"].ToString().Trim().Length == 0)
            //            {
            //                pOrganizaciones[i]["Email"] = null;
            //            }
            //            if (!ValidarEmail(pOrganizaciones[i]["Email"].ToString()))
            //            {
            //                if (pOrganizaciones[i]["Email"].ToString().Trim().Length != 0)
            //                {
            //                    throw new ErrorDatoNoValido("El correo electr�nico de la organizaci�n '" + pOrganizaciones[i]["Nombre"] + "' no es v�lido");
            //                }
            //            }
            //        }

            //        //Web no v�lida
            //        if (!pOrganizaciones[i].IsNull("Web"))
            //        {
            //            //Si es vac�o lo ponemos a Null
            //            if (pOrganizaciones[i]["Web"].ToString().Trim().Length == 0)
            //            {
            //                pOrganizaciones[i]["Web"] = null;
            //            }
            //            if (!ValidarWeb(pOrganizaciones[i]["Web"].ToString()))
            //            {
            //                throw new ErrorDatoNoValido("La web de la organizaci�n '" + pOrganizaciones[i]["Nombre"] + "' no es v�lido");
            //            }
            //        }

            //        //Direcci�n no v�lida
            //        if (!pOrganizaciones[i].IsNull("Direccion"))
            //        {
            //            //Si es vac�o lo ponemos a Null
            //            if (pOrganizaciones[i]["Direccion"].ToString().Trim().Length == 0)
            //            {
            //                pOrganizaciones[i]["Direccion"] = null;
            //            }

            //            //Formato incorrecto
            //            else if (pOrganizaciones[i]["Fax"].ToString().Length > 255)
            //            {
            //                throw new ErrorDatoNoValido("La direcci�n de la organizaci�n '" + pOrganizaciones[i]["Nombre"] + "' tiene un formato incorrecto.");
            //            }
            //        }

            //        //C�digo postal no v�lido
            //        if (!pOrganizaciones[i].IsNull("CP"))
            //        {
            //            //C�digo Postal vac�o lo ponemos a Null
            //            if (pOrganizaciones[i]["CP"].ToString().Trim().Length == 0)
            //            {
            //                pOrganizaciones[i]["CP"] = null;
            //            }
            //            //Formato no v�lido
            //            else if (!ValidarCP(pOrganizaciones[i]["CP"].ToString()))
            //            {
            //                throw new ErrorDatoNoValido("El C�digo Postal '" + pOrganizaciones[i]["CP"] + "' no es v�lido");
            //            }
            //        }

            //        //TODO: Comprobar provincia, pais, localidad
            //    }
            //}
        }

        #endregion

        #region Propios de OrganizacionGnoss

        #endregion

        #region Tipos de organizaci�n

        /// <summary>
        /// Obtiene el listado de tipos de organizaci�n y sus correspondientes literales
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TiposOrganizacion, string> ObtenerListadoTiposOrganizacion()
        {
            Dictionary<TiposOrganizacion, string> listado = new Dictionary<TiposOrganizacion, string>();

            listado.Add(TiposOrganizacion.AdministracionPublica, TipoOrganizacionToString(TiposOrganizacion.AdministracionPublica));
            listado.Add(TiposOrganizacion.AgrupacionInteresEconomico, TipoOrganizacionToString(TiposOrganizacion.AgrupacionInteresEconomico));
            listado.Add(TiposOrganizacion.Asociaciones, TipoOrganizacionToString(TiposOrganizacion.Asociaciones));
            listado.Add(TiposOrganizacion.ComunidadBienes, TipoOrganizacionToString(TiposOrganizacion.ComunidadBienes));
            listado.Add(TiposOrganizacion.ComunidadPropietarios, TipoOrganizacionToString(TiposOrganizacion.ComunidadPropietarios));
            listado.Add(TiposOrganizacion.Corporaciones, TipoOrganizacionToString(TiposOrganizacion.Corporaciones));
            listado.Add(TiposOrganizacion.EmpresarioIndividual, TipoOrganizacionToString(TiposOrganizacion.EmpresarioIndividual));
            listado.Add(TiposOrganizacion.Fundaciones, TipoOrganizacionToString(TiposOrganizacion.Fundaciones));
            listado.Add(TiposOrganizacion.GrupoInvestigacion, TipoOrganizacionToString(TiposOrganizacion.GrupoInvestigacion));
            listado.Add(TiposOrganizacion.Otros, TipoOrganizacionToString(TiposOrganizacion.Otros));
            listado.Add(TiposOrganizacion.SociedadAnonima, TipoOrganizacionToString(TiposOrganizacion.SociedadAnonima));
            listado.Add(TiposOrganizacion.SociedadCapitalRiesgo, TipoOrganizacionToString(TiposOrganizacion.SociedadCapitalRiesgo));
            listado.Add(TiposOrganizacion.SociedadColectiva, TipoOrganizacionToString(TiposOrganizacion.SociedadColectiva));
            listado.Add(TiposOrganizacion.SociedadComanditaria, TipoOrganizacionToString(TiposOrganizacion.SociedadComanditaria));
            listado.Add(TiposOrganizacion.SociedadesCiviles, TipoOrganizacionToString(TiposOrganizacion.SociedadesCiviles));
            listado.Add(TiposOrganizacion.SociedadGarantiaReciproca, TipoOrganizacionToString(TiposOrganizacion.SociedadGarantiaReciproca));
            listado.Add(TiposOrganizacion.SociedadInversionMobiliaria, TipoOrganizacionToString(TiposOrganizacion.SociedadInversionMobiliaria));
            listado.Add(TiposOrganizacion.SociedadLaboral, TipoOrganizacionToString(TiposOrganizacion.SociedadLaboral));
            listado.Add(TiposOrganizacion.SociedadLimitada, TipoOrganizacionToString(TiposOrganizacion.SociedadLimitada));
            listado.Add(TiposOrganizacion.SociedadLimitadaNuevaEmpresa, TipoOrganizacionToString(TiposOrganizacion.SociedadLimitadaNuevaEmpresa));
            listado.Add(TiposOrganizacion.SociedadLimitadaUnipersonal, TipoOrganizacionToString(TiposOrganizacion.SociedadLimitadaUnipersonal));
            listado.Add(TiposOrganizacion.UnionTemporalEmpresas, TipoOrganizacionToString(TiposOrganizacion.UnionTemporalEmpresas));

            return listado;
        }

        /// <summary>
        /// Obtiene en formato String del tipo de una organizacion
        /// </summary>
        /// <param name="pTipo">Tipo de organizacion</param>
        public static string TipoOrganizacionToString(TiposOrganizacion pTipo)
        {
            return TipoOrganizacionToString((short)pTipo);
        }

        /// <summary>
        /// Obtiene en formato String del tipo de una organizacion
        /// </summary>
        /// <param name="pTipoShort">Short que representa el tipo de organizacion</param>
        public static string TipoOrganizacionToString(short pTipoShort)
        {
            switch (pTipoShort)
            {
                case (short)TiposOrganizacion.Otros:
                    {
                        return "Otros";
                    }
                case (short)TiposOrganizacion.AdministracionPublica:
                    {
                        return "Administraci�n publica";
                    }
                case (short)TiposOrganizacion.AgrupacionInteresEconomico:
                    {
                        return "Agrupaci�n de inter�s econ�mico";
                    }
                case (short)TiposOrganizacion.Asociaciones:
                    {
                        return "Asociaciones ";
                    }
                case (short)TiposOrganizacion.ComunidadBienes:
                    {
                        return "Comunidad de bienes";
                    }
                case (short)TiposOrganizacion.ComunidadPropietarios:
                    {
                        return "Comunidad de propietarios";
                    }
                case (short)TiposOrganizacion.Corporaciones:
                    {
                        return "Corporaciones";
                    }
                case (short)TiposOrganizacion.EmpresarioIndividual:
                    {
                        return "Empresario individual (aut�nomo)";
                    }
                case (short)TiposOrganizacion.Fundaciones:
                    {
                        return "Fundaciones";
                    }
                case (short)TiposOrganizacion.SociedadAnonima:
                    {
                        return "Sociedad an�nima";
                    }
                case (short)TiposOrganizacion.SociedadCapitalRiesgo:
                    {
                        return "Sociedad de capital riesgo";
                    }
                case (short)TiposOrganizacion.SociedadColectiva:
                    {
                        return "Sociedad colectiva";
                    }
                case (short)TiposOrganizacion.SociedadComanditaria:
                    {
                        return "Sociedad comanditaria (simple y por acciones)";
                    }
                case (short)TiposOrganizacion.SociedadesCiviles:
                    {
                        return "Sociedades civiles";
                    }
                case (short)TiposOrganizacion.SociedadGarantiaReciproca:
                    {
                        return "Sociedad de garant�a reciproca";
                    }
                case (short)TiposOrganizacion.SociedadInversionMobiliaria:
                    {
                        return "Sociedad de inversi�n mobiliaria";
                    }
                case (short)TiposOrganizacion.SociedadLaboral:
                    {
                        return "Sociedad laboral (limitada y an�nima)";
                    }
                case (short)TiposOrganizacion.SociedadLimitada:
                    {
                        return "Sociedad limitada";
                    }
                case (short)TiposOrganizacion.SociedadLimitadaNuevaEmpresa:
                    {
                        return "Sociedad limitada nueva empresa";
                    }
                case (short)TiposOrganizacion.SociedadLimitadaUnipersonal:
                    {
                        return "Sociedad limitada unipersonal";
                    }
                case (short)TiposOrganizacion.UnionTemporalEmpresas:
                    {
                        return "Uni�n temporal de empresas";
                    }
                case (short)TiposOrganizacion.GrupoInvestigacion:
                    {
                        return "Grupo de investigaci�n";
                    }
                default:
                    {
                        return "(Sin definir)"; // David: Este caso nunca deber�a darse
                    }
            }
        }

        #endregion

        #region Sectores de organizaci�n

        /// <summary>
        /// Obtiene el listado de sectores de  organizaci�n y sus correspondientes literales
        /// </summary>
        /// <returns></returns>
        public static Dictionary<SectoresOrganizacion, string> ObtenerListadoSectoresOrganizacion()
        {
            Dictionary<SectoresOrganizacion, string> listado = new Dictionary<SectoresOrganizacion, string>();

            listado.Add(SectoresOrganizacion.Administracion_de_empresas, SectorOrganizacionToString(SectoresOrganizacion.Administracion_de_empresas));
            listado.Add(SectoresOrganizacion.Administracion_publica, SectorOrganizacionToString(SectoresOrganizacion.Administracion_publica));
            listado.Add(SectoresOrganizacion.Agricultura_Pesca_Mineria, SectorOrganizacionToString(SectoresOrganizacion.Agricultura_Pesca_Mineria));
            listado.Add(SectoresOrganizacion.Arquitectura, SectorOrganizacionToString(SectoresOrganizacion.Arquitectura));
            listado.Add(SectoresOrganizacion.Arte_Cultura_Sociedad, SectorOrganizacionToString(SectoresOrganizacion.Arte_Cultura_Sociedad));
            listado.Add(SectoresOrganizacion.Artes_Oficios, SectorOrganizacionToString(SectoresOrganizacion.Artes_Oficios));
            listado.Add(SectoresOrganizacion.Atencion_al_cliente, SectorOrganizacionToString(SectoresOrganizacion.Atencion_al_cliente));
            listado.Add(SectoresOrganizacion.Bioquimica_Farmacia, SectorOrganizacionToString(SectoresOrganizacion.Bioquimica_Farmacia));
            listado.Add(SectoresOrganizacion.Calidad, SectorOrganizacionToString(SectoresOrganizacion.Calidad));
            listado.Add(SectoresOrganizacion.Comercial_Ventas, SectorOrganizacionToString(SectoresOrganizacion.Comercial_Ventas));
            listado.Add(SectoresOrganizacion.Compras, SectorOrganizacionToString(SectoresOrganizacion.Compras));
            listado.Add(SectoresOrganizacion.Construccion, SectorOrganizacionToString(SectoresOrganizacion.Construccion));
            listado.Add(SectoresOrganizacion.Consultoria, SectorOrganizacionToString(SectoresOrganizacion.Consultoria));
            listado.Add(SectoresOrganizacion.Direccion_Gerencia, SectorOrganizacionToString(SectoresOrganizacion.Direccion_Gerencia));
            listado.Add(SectoresOrganizacion.Disenio_ArtesGraficas, SectorOrganizacionToString(SectoresOrganizacion.Disenio_ArtesGraficas));
            listado.Add(SectoresOrganizacion.Educacion_Formacion, SectorOrganizacionToString(SectoresOrganizacion.Educacion_Formacion));
            listado.Add(SectoresOrganizacion.Finanzas_Banca, SectorOrganizacionToString(SectoresOrganizacion.Finanzas_Banca));
            listado.Add(SectoresOrganizacion.Hosteleria_Turismo, SectorOrganizacionToString(SectoresOrganizacion.Hosteleria_Turismo));
            listado.Add(SectoresOrganizacion.Informatica_Telecomunicaciones, SectorOrganizacionToString(SectoresOrganizacion.Informatica_Telecomunicaciones));
            listado.Add(SectoresOrganizacion.Ingenieros_Tecnicos, SectorOrganizacionToString(SectoresOrganizacion.Ingenieros_Tecnicos));
            listado.Add(SectoresOrganizacion.Inmobiliario, SectorOrganizacionToString(SectoresOrganizacion.Inmobiliario));
            listado.Add(SectoresOrganizacion.Investigacion_ID, SectorOrganizacionToString(SectoresOrganizacion.Investigacion_ID));
            listado.Add(SectoresOrganizacion.Legal, SectorOrganizacionToString(SectoresOrganizacion.Legal));
            listado.Add(SectoresOrganizacion.Limpieza_ServiciosUrbanos, SectorOrganizacionToString(SectoresOrganizacion.Limpieza_ServiciosUrbanos));
            listado.Add(SectoresOrganizacion.Logistica_Almacenaje, SectorOrganizacionToString(SectoresOrganizacion.Logistica_Almacenaje));
            listado.Add(SectoresOrganizacion.Mantenimiento_de_Instalaciones, SectorOrganizacionToString(SectoresOrganizacion.Mantenimiento_de_Instalaciones));
            listado.Add(SectoresOrganizacion.Marketing_Comunicacion, SectorOrganizacionToString(SectoresOrganizacion.Marketing_Comunicacion));
            listado.Add(SectoresOrganizacion.MedioAmbiente, SectorOrganizacionToString(SectoresOrganizacion.MedioAmbiente));
            listado.Add(SectoresOrganizacion.Otros, SectorOrganizacionToString(SectoresOrganizacion.Otros));
            listado.Add(SectoresOrganizacion.Periodismo, SectorOrganizacionToString(SectoresOrganizacion.Periodismo));
            listado.Add(SectoresOrganizacion.Recursos_Humanos, SectorOrganizacionToString(SectoresOrganizacion.Recursos_Humanos));
            listado.Add(SectoresOrganizacion.Sanidad_Salud, SectorOrganizacionToString(SectoresOrganizacion.Sanidad_Salud));
            listado.Add(SectoresOrganizacion.Secretariado_Administrativo, SectorOrganizacionToString(SectoresOrganizacion.Secretariado_Administrativo));
            listado.Add(SectoresOrganizacion.Seguridad_Defensa, SectorOrganizacionToString(SectoresOrganizacion.Seguridad_Defensa));
            listado.Add(SectoresOrganizacion.Seguros, SectorOrganizacionToString(SectoresOrganizacion.Seguros));
            listado.Add(SectoresOrganizacion.Turismo_Restauracion, SectorOrganizacionToString(SectoresOrganizacion.Turismo_Restauracion));

            return listado;
        }

        /// <summary>
        /// Obtiene en formato String del sector de una organizacion
        /// </summary>
        /// <param name="pSector">Sector de organizacion</param>
        public static string SectorOrganizacionToString(SectoresOrganizacion pSector)
        {
            return SectorOrganizacionToString((short)pSector);
        }

        /// <summary>
        /// Obtiene en formato String del sector de una organizacion
        /// </summary>
        /// <param name="pTipoShort">Short que representa el sector de organizacion</param>
        public static string SectorOrganizacionToString(short pTipoShort)
        {
            switch (pTipoShort)
            {
                case (short)SectoresOrganizacion.Otros:
                    {
                        return "Otros";
                    }
                case (short)SectoresOrganizacion.Administracion_de_empresas:
                    {
                        return "Administraci�n de empresas";
                    }
                case (short)SectoresOrganizacion.Administracion_publica:
                    {
                        return "Administraci�n p�blica";
                    }
                case (short)SectoresOrganizacion.Agricultura_Pesca_Mineria:
                    {
                        return "Agricultura, pesca y miner�a";
                    }
                case (short)SectoresOrganizacion.Arquitectura:
                    {
                        return "Arquitectura";
                    }
                case (short)SectoresOrganizacion.Arte_Cultura_Sociedad:
                    {
                        return "Arte, cultura y sociedad";
                    }
                case (short)SectoresOrganizacion.Artes_Oficios:
                    {
                        return "Artes y oficios";
                    }
                case (short)SectoresOrganizacion.Atencion_al_cliente:
                    {
                        return "Atenci�n al cliente";
                    }
                case (short)SectoresOrganizacion.Bioquimica_Farmacia:
                    {
                        return "Bioqu�mica, farmacia";
                    }
                case (short)SectoresOrganizacion.Calidad:
                    {
                        return "Calidad";
                    }
                case (short)SectoresOrganizacion.Comercial_Ventas:
                    {
                        return "Comercial, ventas";
                    }
                case (short)SectoresOrganizacion.Compras:
                    {
                        return "Compras";
                    }
                case (short)SectoresOrganizacion.Construccion:
                    {
                        return "Construcci�n";
                    }
                case (short)SectoresOrganizacion.Consultoria:
                    {
                        return "Consultor�a";
                    }
                case (short)SectoresOrganizacion.Direccion_Gerencia:
                    {
                        return "Direcci�n, gerencia";
                    }
                case (short)SectoresOrganizacion.Disenio_ArtesGraficas:
                    {
                        return "Dise�o, artes graficas";
                    }
                case (short)SectoresOrganizacion.Educacion_Formacion:
                    {
                        return "Educaci�n, formaci�n";
                    }
                case (short)SectoresOrganizacion.Finanzas_Banca:
                    {
                        return "Finanzas, banca";
                    }
                case (short)SectoresOrganizacion.Hosteleria_Turismo:
                    {
                        return "Hosteler�a, turismo";
                    }
                case (short)SectoresOrganizacion.Informatica_Telecomunicaciones:
                    {
                        return "Inform�tica, telecomunicaciones";
                    }
                case (short)SectoresOrganizacion.Ingenieros_Tecnicos:
                    {
                        return "Ingenieros, t�cnicos";
                    }
                case (short)SectoresOrganizacion.Inmobiliario:
                    {
                        return "Inmobiliario";
                    }
                case (short)SectoresOrganizacion.Investigacion_ID:
                    {
                        return "Investigaci�n e i+D";
                    }
                case (short)SectoresOrganizacion.Legal:
                    {
                        return "Legal";
                    }
                case (short)SectoresOrganizacion.Limpieza_ServiciosUrbanos:
                    {
                        return "Limpieza, servicios urbanos";
                    }
                case (short)SectoresOrganizacion.Logistica_Almacenaje:
                    {
                        return "Log�stica, almacenaje";
                    }
                case (short)SectoresOrganizacion.Mantenimiento_de_Instalaciones:
                    {
                        return "Mantenimiento de instalaciones";
                    }
                case (short)SectoresOrganizacion.Marketing_Comunicacion:
                    {
                        return "Marketing, comunicaci�n";
                    }
                case (short)SectoresOrganizacion.MedioAmbiente:
                    {
                        return "Medio ambiente";
                    }
                case (short)SectoresOrganizacion.Periodismo:
                    {
                        return "Periodismo";
                    }
                case (short)SectoresOrganizacion.Recursos_Humanos:
                    {
                        return "Recursos humanos";
                    }
                case (short)SectoresOrganizacion.Sanidad_Salud:
                    {
                        return "Sanidad, salud";
                    }
                case (short)SectoresOrganizacion.Secretariado_Administrativo:
                    {
                        return "Secretariado, administrativo";
                    }
                case (short)SectoresOrganizacion.Seguridad_Defensa:
                    {
                        return "Seguridad, defensa";
                    }
                case (short)SectoresOrganizacion.Seguros:
                    {
                        return "Seguros";
                    }
                case (short)SectoresOrganizacion.Turismo_Restauracion:
                    {
                        return "Turismo, restauraci�n";
                    }
                default:
                    {
                        return "(Sin definir)"; // Castillo: Este caso nunca deber�a darse
                    }
            }
        }

        #endregion



        #endregion

        #region Dispose

        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~OrganizacionCN()
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
        /// <param name="disposing">Determina si se est� llamando desde el Dispose()</param>
        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    //Libero todos los recursos administrados que he a�adido a esta clase
                    if (this.OrganizacionAD != null)
                    {
                        OrganizacionAD.Dispose();
                    }
                }
                OrganizacionAD = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Dataadapter de organizaci�n
        /// </summary>
        protected OrganizacionAD OrganizacionAD
        {
            get
            {
                return (OrganizacionAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}
