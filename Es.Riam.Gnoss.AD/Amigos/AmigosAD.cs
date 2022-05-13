using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Amigos
{
    #region Enumeraciones

    /// <summary>
    /// Estado de una solicitud
    /// </summary>
    public enum EstadoSolicitud
    {
        /// <summary>
        /// Pendiente
        /// </summary>
        Pendiente = 0,
        /// <summary>
        /// Aceptada
        /// </summary>
        Aceptada = 1,
        /// <summary>
        /// Rechazada
        /// </summary>
        Rechazada = 2
    }

    /// <summary>
    /// Tipo de un grupo de amigos
    /// </summary>
    public enum TipoGrupoAmigos
    {
        /// <summary>
        /// Manual
        /// </summary>
        Manual = 0,
        /// <summary>
        /// Automatico de organizacion
        /// </summary>
        AutomaticoOrganizacion = 1
    }

    #endregion

    /// <summary>
    /// DataAdapter de Amigos
    /// </summary>
    public class AmigosAD : BaseAD
    {
        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public AmigosAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public AmigosAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string selectAmigo;
        private string selectGrupoAmigos;
        private string selectAmigoAgGrupo;
        private string selectSolicitudAmigo;
        private string selectSolicitudContacto;
        private string selectPermisoAmigoOrg;
        private string selectPermisoGrupoAmigoOrg;

        private string sqlSelectAmigosDeIdentidad;
        private string sqlSelectAmigosDeIdentidadReciproco;
        private string sqlSelectGrupoAmigosDeIdentidad;
        private string sqlSelectAmigoAgGrupoDeIdentidad;

        private string sqlSelectSolicitudAmigosDeIdentidad;
        private string sqlSelectSolicitudContactosDeIdentidad;
        private string sqlSelectAmigoAgGrupoDeIdentidadReciproco;

        private string sqlSelectAmigosDePersona;
        private string sqlSelectSolicitudAmigoDePersona;
        private string sqlSelectSolicitudContactoDePersona;
        private string sqlSelectGrupoAmigosDePersona;
        private string sqlSelectAmigoAgGrupoDePersona;
        private string sqlSelectContactosIdentidadDeMyGnoss;

        private string sqlSelectAmigosOrganizacionConPermisoParaUsuario;
        private string sqlSelectGrupoAmigosOrganizacionConPermisoParaUsuario;
        private string sqlSelectAmigoAgGrupoOrganizacionConPermisoParaUsuario;
        private string sqlSelectPermisoAmigoOrgDeUsuarioYOrganizacion;
        private string sqlSelectPermisoGrupoAmigoOrgDeUsuarioYOrganizacion;
        private string sqlSelectPermisoAmigoOrgDeIdentidad;
        private string sqlSelectPermisoAmigoOrgDeIdentidadConEliminados;
        private string sqlSelectPermisoGrupoAmigoOrgDeIdentidad;

        private string sqlSelectGrupoAmigosDeGrupoID;
        private string sqlSelectAmigoAgGrupoDeGrupoID;
        private string sqlSelectPermisoGrupoAmigoOrgDeGrupoID;

        #endregion

        #region DataAdapter

        #region Amigo

        private string sqlAmigoInsert;
        private string sqlAmigoDelete;
        private string sqlAmigoModify;

        #endregion

        #region GrupoAmigos

        private string sqlGrupoAmigosInsert;
        private string sqlGrupoAmigosDelete;
        private string sqlGrupoAmigosModify;

        #endregion

        #region AmigoAgGrupo

        private string sqlAmigoAgGrupoInsert;
        private string sqlAmigoAgGrupoDelete;
        private string sqlAmigoAgGrupoModify;

        #endregion

        #region SolicitudAmigo

        private string sqlSolicitudAmigoInsert;
        private string sqlSolicitudAmigoDelete;
        private string sqlSolicitudAmigoModify;

        #endregion

        #region SolicitudContacto

        private string sqlSolicitudContactoInsert;
        private string sqlSolicitudContactoDelete;
        private string sqlSolicitudContactoModify;

        #endregion

        #region PermisoAmigoOrg

        private string sqlPermisoAmigoOrgInsert;
        private string sqlPermisoAmigoOrgDelete;
        private string sqlPermisoAmigoOrgModify;

        #endregion

        #region PermisoGrupoAmigoOrg

        private string sqlPermisoGrupoAmigoOrgInsert;
        private string sqlPermisoGrupoAmigoOrgDelete;
        private string sqlPermisoGrupoAmigoOrgModify;

        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Comprueba dado el identificador de un perfil, si la identidad en MYGNOSS asociada a dicho perfil tiene contactos 
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>TRUE si tiene contactos, FALSE en caso contrario</returns>
        public bool TieneContactosIdentidadDeMyGnoss(Guid pPerfilID)
        {
            Object Existe;
            DbCommand commandsqlSelectContactosIdentidadDeMyGnoss = ObtenerComando(sqlSelectContactosIdentidadDeMyGnoss);
            AgregarParametro(commandsqlSelectContactosIdentidadDeMyGnoss, IBD.ToParam("perfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            AgregarParametro(commandsqlSelectContactosIdentidadDeMyGnoss, IBD.ToParam("metaproyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(ProyectoAD.MetaProyecto));

            try
            {
                Existe = EjecutarEscalar(commandsqlSelectContactosIdentidadDeMyGnoss);
            }
            finally
            {
            }
            return (Existe != null);
        }

        /// <summary>
        /// Devuelve si las dos identidades son amigos
        /// </summary>
        /// <param name="pIdentidad">Identidad del usuario</param>
        /// <param name="pAmigo">Identidad del amigo</param>
        /// <returns></returns>
        public bool EsAmigoDeIdentidad(Guid pIdentidad, Guid pAmigo)
        {
            Object Existe;
            DbCommand commandSQL = ObtenerComando(selectAmigo + " WHERE IdentidadID = " + IBD.GuidParamValor("IdentidadID") + " AND IdentidadAmigoID = " + IBD.GuidParamValor("IdentidadAmigoID"));
            AgregarParametro(commandSQL, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidad));
            AgregarParametro(commandSQL, IBD.ToParam("IdentidadAmigoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pAmigo));

            try
            {
                Existe = EjecutarEscalar(commandSQL);
            }
            finally
            {
            }
            return (Existe != null);
        }

        /// <summary>
        /// Carga todas las listas del DataWrapper con los datos de tus amigos y de los que tu eres amigo
        /// </summary>
        /// <param name="pIdentidadID">IdentidadID del usuario actual</param>
        /// <returns></returns>
        public DataWrapperAmigos CargarAmigosCompleto(Guid pIdentidadID)
        {
            DataWrapperAmigos dataWrapperAmigos = new DataWrapperAmigos();

            dataWrapperAmigos.ListaAmigo = mEntityContext.Amigo.Where(item => item.IdentidadID.Equals(pIdentidadID) || item.IdentidadAmigoID.Equals(pIdentidadID)).ToList();            
            dataWrapperAmigos.ListaGrupoAmigos = mEntityContext.GrupoAmigos.Where(item => item.IdentidadID.Equals(pIdentidadID)).ToList();
            dataWrapperAmigos.ListaPermisoAmigoOrg = mEntityContext.PermisoAmigoOrg.Where(item => item.IdentidadUsuarioID.Equals(pIdentidadID) || item.IdentidadAmigoID.Equals(pIdentidadID)).ToList();
            dataWrapperAmigos.ListaPermisoGrupoAmigoOrg = mEntityContext.PermisoGrupoAmigoOrg.Where(item => item.IdentidadUsuarioID.Equals(pIdentidadID)).ToList();

            return dataWrapperAmigos;
        }

        /// <summary>
        /// Comprueba si pIdentidadAmigo es amigo de pIdentidad o de alguno de sus amigos
        /// </summary>
        /// <param name="pIdentidad">ID de la identidad</param>
        /// <param name="pIdentidadAmigo">ID del amigo a comprobar</param>
        /// <returns>True si es amigo, false en caso contrario</returns>
        public bool ComprobarIdentidadAmigosDeAmigos(Guid pIdentidad, Guid pIdentidadAmigo)
        {
            string consulta = "SELECT 1 FROM Amigo WHERE IdentidadID = " + IBD.GuidValor(pIdentidad) + " AND IdentidadAmigoID= " + IBD.GuidValor(pIdentidadAmigo) + " UNION SELECT 1 FROM Amigo WHERE IdentidadID IN ( SELECT IdentidadAmigoID FROM Amigo WHERE IdentidadID = " + IBD.GuidValor(pIdentidad) + " ) AND IdentidadAmigoID= " + IBD.GuidValor(pIdentidadAmigo);
            DbCommand commandConsulta = ObtenerComando(consulta);
            if (EjecutarEscalar(commandConsulta) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Actualiza datos de amigos
        /// </summary>
        /// <param name="pDataSet">Dataset de amigos</param>
        public void ActualizarAmigos(DataSet pDataSet)
        {
            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);
        }

        /// <summary>
        /// Elimina datos de amigos
        /// </summary>
        /// <param name="pDataSet">Dataset de amigos</param>
        public void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    #region Deleted

                    #region Eliminar tabla PermisoGrupoAmigoOrg
                    DbCommand DeletePermisoGrupoAmigoOrgCommand = ObtenerComando(sqlPermisoGrupoAmigoOrgDelete);
                    AgregarParametro(DeletePermisoGrupoAmigoOrgCommand, IBD.ToParam("O_IdentidadOrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadOrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeletePermisoGrupoAmigoOrgCommand, IBD.ToParam("O_IdentidadUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadUsuarioID", DataRowVersion.Original);
                    AgregarParametro(DeletePermisoGrupoAmigoOrgCommand, IBD.ToParam("O_PermisoEdicion"), DbType.Boolean, "PermisoEdicion", DataRowVersion.Original);
                    AgregarParametro(DeletePermisoGrupoAmigoOrgCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "PermisoGrupoAmigoOrg", null, null, DeletePermisoGrupoAmigoOrgCommand, UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla PermisoAmigoOrg
                    DbCommand DeletePermisoAmigoOrgCommand = ObtenerComando(sqlPermisoAmigoOrgDelete);
                    AgregarParametro(DeletePermisoAmigoOrgCommand, IBD.ToParam("O_IdentidadOrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadOrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeletePermisoAmigoOrgCommand, IBD.ToParam("O_IdentidadUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadUsuarioID", DataRowVersion.Original);
                    AgregarParametro(DeletePermisoAmigoOrgCommand, IBD.ToParam("O_IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "PermisoAmigoOrg", null, null, DeletePermisoAmigoOrgCommand, UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla SolicitudAmigo
                    DbCommand DeleteSolicitudAmigoCommand = ObtenerComando(sqlSolicitudAmigoDelete);
                    AgregarParametro(DeleteSolicitudAmigoCommand, IBD.ToParam("O_SolicitudID"), IBD.TipoGuidToObject(DbType.Guid), "SolicitudID", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudAmigoCommand, IBD.ToParam("O_FechaSolicitud"), DbType.DateTime, "FechaSolicitud", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudAmigoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudAmigoCommand, IBD.ToParam("O_IdentidadIDAmigo"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadIDAmigo", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudAmigoCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "SolicitudAmigo", null, null, DeleteSolicitudAmigoCommand, UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla SolicitudContacto
                    DbCommand DeleteSolicitudContactoCommand = ObtenerComando(sqlSolicitudContactoDelete);
                    AgregarParametro(DeleteSolicitudContactoCommand, IBD.ToParam("O_SolicitudID"), IBD.TipoGuidToObject(DbType.Guid), "SolicitudID", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudContactoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudContactoCommand, IBD.ToParam("O_IdentidadIDAmigo"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadIDAmigo", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudContactoCommand, IBD.ToParam("O_FechaSolicitud"), DbType.DateTime, "FechaSolicitud", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudContactoCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(DeleteSolicitudContactoCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "SolicitudContacto", null, null, DeleteSolicitudContactoCommand, UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla AmigoAgGrupo
                    DbCommand DeleteAmigoAgGrupoCommand = ObtenerComando(sqlAmigoAgGrupoDelete);
                    AgregarParametro(DeleteAmigoAgGrupoCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    AgregarParametro(DeleteAmigoAgGrupoCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
                    AgregarParametro(DeleteAmigoAgGrupoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(DeleteAmigoAgGrupoCommand, IBD.ToParam("O_IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "AmigoAgGrupo", null, null, DeleteAmigoAgGrupoCommand, UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla GrupoAmigos
                    DbCommand DeleteGrupoAmigosCommand = ObtenerComando(sqlGrupoAmigosDelete);
                    AgregarParametro(DeleteGrupoAmigosCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
                    AgregarParametro(DeleteGrupoAmigosCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    AgregarParametro(DeleteGrupoAmigosCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    AgregarParametro(DeleteGrupoAmigosCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(DeleteGrupoAmigosCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteGrupoAmigosCommand, IBD.ToParam("O_Automatico"), DbType.Boolean, "Automatico", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "GrupoAmigos", null, null, DeleteGrupoAmigosCommand, UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla Amigo
                    DbCommand DeleteAmigoCommand = ObtenerComando(sqlAmigoDelete);
                    AgregarParametro(DeleteAmigoCommand, IBD.ToParam("O_EsFanMutuo"), DbType.Boolean, "EsFanMutuo", DataRowVersion.Original);
                    AgregarParametro(DeleteAmigoCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    AgregarParametro(DeleteAmigoCommand, IBD.ToParam("O_IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Original);
                    AgregarParametro(DeleteAmigoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "Amigo", null, null, DeleteAmigoCommand, UpdateBehavior.Transactional);

                    #endregion

                    #endregion

                    deletedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Guarda los datos modificados de amigos
        /// </summary>
        /// <param name="pDataSet">Dataset de amigos</param>
        public void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);

                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    #region Actualizar tabla Amigo

                    DbCommand InsertAmigoCommand = ObtenerComando(sqlAmigoInsert);
                    AgregarParametro(InsertAmigoCommand, IBD.ToParam("EsFanMutuo"), DbType.Boolean, "EsFanMutuo", DataRowVersion.Current);
                    AgregarParametro(InsertAmigoCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertAmigoCommand, IBD.ToParam("IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Current);
                    AgregarParametro(InsertAmigoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);

                    DbCommand ModifyAmigoCommand = ObtenerComando(sqlAmigoModify);
                    AgregarParametro(ModifyAmigoCommand, IBD.ToParam("O_EsFanMutuo"), DbType.Boolean, "EsFanMutuo", DataRowVersion.Original);
                    AgregarParametro(ModifyAmigoCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    AgregarParametro(ModifyAmigoCommand, IBD.ToParam("O_IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Original);
                    AgregarParametro(ModifyAmigoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);

                    AgregarParametro(ModifyAmigoCommand, IBD.ToParam("EsFanMutuo"), DbType.Boolean, "EsFanMutuo", DataRowVersion.Current);
                    AgregarParametro(ModifyAmigoCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(ModifyAmigoCommand, IBD.ToParam("IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Current);
                    AgregarParametro(ModifyAmigoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "Amigo", InsertAmigoCommand, ModifyAmigoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla GrupoAmigos

                    DbCommand InsertGrupoAmigosCommand = ObtenerComando(sqlGrupoAmigosInsert);
                    AgregarParametro(InsertGrupoAmigosCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);
                    AgregarParametro(InsertGrupoAmigosCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(InsertGrupoAmigosCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertGrupoAmigosCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(InsertGrupoAmigosCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertGrupoAmigosCommand, IBD.ToParam("Automatico"), DbType.Boolean, "Automatico", DataRowVersion.Current);

                    DbCommand ModifyGrupoAmigosCommand = ObtenerComando(sqlGrupoAmigosModify);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("O_Automatico"), DbType.Boolean, "Automatico", DataRowVersion.Original);

                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyGrupoAmigosCommand, IBD.ToParam("Automatico"), DbType.Boolean, "Automatico", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "GrupoAmigos", InsertGrupoAmigosCommand, ModifyGrupoAmigosCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla AmigoAgGrupo

                    DbCommand InsertAmigoAgGrupoCommand = ObtenerComando(sqlAmigoAgGrupoInsert);
                    AgregarParametro(InsertAmigoAgGrupoCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertAmigoAgGrupoCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);
                    AgregarParametro(InsertAmigoAgGrupoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(InsertAmigoAgGrupoCommand, IBD.ToParam("IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Current);

                    DbCommand ModifyAmigoAgGrupoCommand = ObtenerComando(sqlAmigoAgGrupoModify);
                    AgregarParametro(ModifyAmigoAgGrupoCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    AgregarParametro(ModifyAmigoAgGrupoCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
                    AgregarParametro(ModifyAmigoAgGrupoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(ModifyAmigoAgGrupoCommand, IBD.ToParam("O_IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Original);

                    AgregarParametro(ModifyAmigoAgGrupoCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(ModifyAmigoAgGrupoCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);
                    AgregarParametro(ModifyAmigoAgGrupoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifyAmigoAgGrupoCommand, IBD.ToParam("IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "AmigoAgGrupo", InsertAmigoAgGrupoCommand, ModifyAmigoAgGrupoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla SolicitudAmigo

                    DbCommand InsertSolicitudAmigoCommand = ObtenerComando(sqlSolicitudAmigoInsert);
                    AgregarParametro(InsertSolicitudAmigoCommand, IBD.ToParam("SolicitudID"), IBD.TipoGuidToObject(DbType.Guid), "SolicitudID", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudAmigoCommand, IBD.ToParam("FechaSolicitud"), DbType.DateTime, "FechaSolicitud", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudAmigoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudAmigoCommand, IBD.ToParam("IdentidadIDAmigo"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadIDAmigo", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudAmigoCommand, IBD.ToParam("Estado"), IBD.TipoGuidToObject(DbType.Int16), "Estado", DataRowVersion.Current);

                    DbCommand ModifySolicitudAmigoCommand = ObtenerComando(sqlSolicitudAmigoModify);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("O_SolicitudID"), IBD.TipoGuidToObject(DbType.Guid), "SolicitudID", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("O_FechaSolicitud"), DbType.DateTime, "FechaSolicitud", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("O_IdentidadIDAmigo"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadIDAmigo", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("O_Estado"), IBD.TipoGuidToObject(DbType.Int16), "Estado", DataRowVersion.Original);

                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("SolicitudID"), IBD.TipoGuidToObject(DbType.Guid), "SolicitudID", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("FechaSolicitud"), DbType.DateTime, "FechaSolicitud", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("IdentidadIDAmigo"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadIDAmigo", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudAmigoCommand, IBD.ToParam("Estado"), IBD.TipoGuidToObject(DbType.Int16), "Estado", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "SolicitudAmigo", InsertSolicitudAmigoCommand, ModifySolicitudAmigoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla SolicitudContacto

                    DbCommand InsertSolicitudContactoCommand = ObtenerComando(sqlSolicitudContactoInsert);
                    AgregarParametro(InsertSolicitudContactoCommand, IBD.ToParam("SolicitudID"), IBD.TipoGuidToObject(DbType.Guid), "SolicitudID", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudContactoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudContactoCommand, IBD.ToParam("IdentidadIDAmigo"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadIDAmigo", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudContactoCommand, IBD.ToParam("FechaSolicitud"), DbType.DateTime, "FechaSolicitud", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudContactoCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertSolicitudContactoCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);

                    DbCommand ModifySolicitudContactoCommand = ObtenerComando(sqlSolicitudContactoModify);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("O_SolicitudID"), IBD.TipoGuidToObject(DbType.Guid), "SolicitudID", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("O_IdentidadIDAmigo"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadIDAmigo", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("O_FechaSolicitud"), DbType.DateTime, "FechaSolicitud", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);

                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("SolicitudID"), IBD.TipoGuidToObject(DbType.Guid), "SolicitudID", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("IdentidadIDAmigo"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadIDAmigo", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("FechaSolicitud"), DbType.DateTime, "FechaSolicitud", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifySolicitudContactoCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "SolicitudContacto", InsertSolicitudContactoCommand, ModifySolicitudContactoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla PermisoAmigoOrg

                    DbCommand InsertPermisoAmigoOrgCommand = ObtenerComando(sqlPermisoAmigoOrgInsert);
                    AgregarParametro(InsertPermisoAmigoOrgCommand, IBD.ToParam("IdentidadOrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadOrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertPermisoAmigoOrgCommand, IBD.ToParam("IdentidadUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadUsuarioID", DataRowVersion.Current);
                    AgregarParametro(InsertPermisoAmigoOrgCommand, IBD.ToParam("IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Current);

                    DbCommand ModifyPermisoAmigoOrgCommand = ObtenerComando(sqlPermisoAmigoOrgModify);
                    AgregarParametro(ModifyPermisoAmigoOrgCommand, IBD.ToParam("O_IdentidadOrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadOrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyPermisoAmigoOrgCommand, IBD.ToParam("O_IdentidadUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadUsuarioID", DataRowVersion.Original);
                    AgregarParametro(ModifyPermisoAmigoOrgCommand, IBD.ToParam("O_IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Original);

                    AgregarParametro(ModifyPermisoAmigoOrgCommand, IBD.ToParam("IdentidadOrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadOrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyPermisoAmigoOrgCommand, IBD.ToParam("IdentidadUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadUsuarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyPermisoAmigoOrgCommand, IBD.ToParam("IdentidadAmigoID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadAmigoID", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "PermisoAmigoOrg", InsertPermisoAmigoOrgCommand, ModifyPermisoAmigoOrgCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla PermisoGrupoAmigoOrg

                    DbCommand InsertPermisoGrupoAmigoOrgCommand = ObtenerComando(sqlPermisoGrupoAmigoOrgInsert);
                    AgregarParametro(InsertPermisoGrupoAmigoOrgCommand, IBD.ToParam("IdentidadOrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadOrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertPermisoGrupoAmigoOrgCommand, IBD.ToParam("IdentidadUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadUsuarioID", DataRowVersion.Current);
                    AgregarParametro(InsertPermisoGrupoAmigoOrgCommand, IBD.ToParam("PermisoEdicion"), DbType.Boolean, "PermisoEdicion", DataRowVersion.Current);
                    AgregarParametro(InsertPermisoGrupoAmigoOrgCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);

                    DbCommand ModifyPermisoGrupoAmigoOrgCommand = ObtenerComando(sqlPermisoGrupoAmigoOrgModify);
                    AgregarParametro(ModifyPermisoGrupoAmigoOrgCommand, IBD.ToParam("O_IdentidadOrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadOrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyPermisoGrupoAmigoOrgCommand, IBD.ToParam("O_IdentidadUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadUsuarioID", DataRowVersion.Original);
                    AgregarParametro(ModifyPermisoGrupoAmigoOrgCommand, IBD.ToParam("O_PermisoEdicion"), DbType.Boolean, "PermisoEdicion", DataRowVersion.Original);
                    AgregarParametro(ModifyPermisoGrupoAmigoOrgCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);

                    AgregarParametro(ModifyPermisoGrupoAmigoOrgCommand, IBD.ToParam("IdentidadOrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadOrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyPermisoGrupoAmigoOrgCommand, IBD.ToParam("IdentidadUsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadUsuarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyPermisoGrupoAmigoOrgCommand, IBD.ToParam("PermisoEdicion"), DbType.Boolean, "PermisoEdicion", DataRowVersion.Current);
                    AgregarParametro(ModifyPermisoGrupoAmigoOrgCommand, IBD.ToParam("GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "PermisoGrupoAmigoOrg", InsertPermisoGrupoAmigoOrgCommand, ModifyPermisoGrupoAmigoOrgCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #endregion

                    addedAndModifiedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene los amigos de una identidad pasada por parámetro
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de amigos</returns>
        public DataWrapperAmigos ObtenerAmigosDeIdentidad(Guid pIdentidadID)
        {
            DataWrapperAmigos dataWrapperAmigos = new DataWrapperAmigos();

            //Solicitud Amigo
            //DbCommand commandsqlSelectSolicitudAmigosDeIdentidad = ObtenerComando(sqlSelectSolicitudAmigosDeIdentidad);
            //AgregarParametro(commandsqlSelectSolicitudAmigosDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //CargarDataSet(commandsqlSelectSolicitudAmigosDeIdentidad, AmigosDS, "SolicitudAmigo");

            //Solicitud Contacto
            //DbCommand commandsqlSelectSolicitudContactosDeIdentidad = ObtenerComando(sqlSelectSolicitudContactosDeIdentidad);
            //AgregarParametro(commandsqlSelectSolicitudContactosDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //CargarDataSet(commandsqlSelectSolicitudContactosDeIdentidad, AmigosDS, "SolicitudContacto");

            //Amigos
            DbCommand commandsqlSelectAmigosDeIdentidad = ObtenerComando(sqlSelectAmigosDeIdentidad);
            dataWrapperAmigos.ListaAmigo = mEntityContext.Amigo.Where(item => item.IdentidadID.Equals(pIdentidadID)).ToList();

            //GrupoAmigos
            //DbCommand commandsqlSelectGruposAmigosDeIdentidad = ObtenerComando(sqlSelectGrupoAmigosDeIdentidad);
            //AgregarParametro(commandsqlSelectGruposAmigosDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //CargarDataSet(commandsqlSelectGruposAmigosDeIdentidad, AmigosDS, "GrupoAmigos");

            //if (AmigosDS.GrupoAmigos.Rows.Count > 0)
            //{
            //    //AmigoAgGrupo
            //    DbCommand commandsqlSelectAmigosAgGruposDeIdentidad = ObtenerComando(sqlSelectAmigoAgGrupoDeIdentidad);
            //    AgregarParametro(commandsqlSelectAmigosAgGruposDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //    CargarDataSet(commandsqlSelectAmigosAgGruposDeIdentidad, AmigosDS, "AmigoAgGrupo");
            //}

            //PermisoAmigoOrg
            //selectPermisoAmigoOrg + " INNER JOIN identidad on PermisoAmigoOrg.IdentidadAmigoID = Identidad.IdentidadID WHERE PermisoAmigoOrg.IdentidadOrganizacionID = " + IBD.GuidParamValor("identidadID") + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL" + " UNION " + selectPermisoAmigoOrg + " INNER JOIN identidad on PermisoAmigoOrg.IdentidadAmigoID = Identidad.IdentidadID WHERE PermisoAmigoOrg.IdentidadAmigoID = " + IBD.GuidParamValor("identidadID") + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL";
            dataWrapperAmigos.ListaPermisoAmigoOrg = mEntityContext.PermisoAmigoOrg.Join(mEntityContext.Identidad, item => new { IdentidadId = item.IdentidadAmigoID }, identidad => new { IdentidadId = identidad.IdentidadID }, (permisoAmigoOrg, identidad) => new
            {
                PermisoAmigoOrg = permisoAmigoOrg,
                Identidad = identidad
            }).Where(item => item.PermisoAmigoOrg.IdentidadOrganizacionID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.PermisoAmigoOrg)
            .Union(mEntityContext.PermisoAmigoOrg.Join(mEntityContext.Identidad, item => new { IdentidadId = item.IdentidadAmigoID }, identidad => new { IdentidadId = identidad.IdentidadID }, (permisoAmigoOrg, identidad) => new
            {
                PermisoAmigoOrg = permisoAmigoOrg,
                Identidad = identidad
            }).Where(item => item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.PermisoAmigoOrg)).ToList();


            //if (AmigosDS.PermisoAmigoOrg.Rows.Count > 0)
            //{
            //    //PermisoGrupoAmigoOrg
            //    DbCommand commandsqlSelectPermisoGrupoAmigoOrg = ObtenerComando(sqlSelectPermisoGrupoAmigoOrgDeIdentidad);
            //    AgregarParametro(commandsqlSelectPermisoGrupoAmigoOrg, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            //    CargarDataSet(commandsqlSelectPermisoGrupoAmigoOrg, AmigosDS, "PermisoGrupoAmigoOrg");
            //}

            return dataWrapperAmigos;
        }

        /// <summary>
        /// Obtiene los contactos de una organización y los grupos a los que un usuario tiene acceso
        /// </summary>
        /// <param name="pIdentidadOrganizacion">Identidad de la organización en MyGNOSS</param>
        /// <param name="pIdentidadUsuario">Identidad del usuario con el perfil de persona-organización en MyGNOSS</param>
        /// <returns>Dataset de amigos</returns>
        public DataWrapperAmigos ObtenerAmigosOrganizacionConAccesoParaUsuario(Guid pIdentidadOrganizacion, Guid pIdentidadUsuario)
        {
            DataWrapperAmigos dataWrapperAmigos= new DataWrapperAmigos();
            //sqlSelectAmigosOrganizacionConPermisoParaUsuario = "(" + selectAmigo + " INNER JOIN PermisoAmigoOrg ON Amigo.IdentidadID = PermisoAmigoOrg.IdentidadOrganizacionID WHERE Amigo.IdentidadID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID") + " AND PermisoAmigoOrg.IdentidadAmigoID = Amigo.IdentidadAmigoID)
            //UNION (" + selectAmigo + " INNER JOIN AmigoAgGrupo ON AmigoAgGrupo.IdentidadID = Amigo.IdentidadID AND AmigoAgGrupo.IdentidadAmigoID = Amigo.IdentidadAmigoID
            //INNER JOIN GrupoAmigos ON GrupoAmigos.GrupoID = AmigoAgGrupo.GrupoID AND GrupoAmigos.IdentidadID = AmigoAgGrupo.IdentidadID
            //INNER JOIN PermisoGrupoAmigoOrg ON PermisoGrupoAmigoOrg.IdentidadOrganizacionID = Amigo.IdentidadID AND PermisoGrupoAmigoOrg.GrupoID = GrupoAmigos.GrupoID
            //WHERE Amigo.IdentidadID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoGrupoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID") + ")";
            //Amigos
            dataWrapperAmigos.ListaAmigo = mEntityContext.Amigo.Join(mEntityContext.PermisoAmigoOrg, amigo => new { IdentidadID = amigo.IdentidadID, IdentidadAmigoID = amigo.IdentidadAmigoID }, permisoAmigoOrg => new { IdentidadID = permisoAmigoOrg.IdentidadOrganizacionID, IdentidadAmigoID = permisoAmigoOrg.IdentidadAmigoID }, (amigo, permisoAmigoOrg) => new
            {
                Amigo = amigo,
                PermisoAmigoOrg = permisoAmigoOrg
            }).Where(item => item.Amigo.IdentidadID.Equals(pIdentidadOrganizacion) && item.PermisoAmigoOrg.IdentidadUsuarioID.Equals(pIdentidadUsuario)).Select(item => item.Amigo)
            .Union(mEntityContext.Amigo.Join(mEntityContext.AmigoAgGrupo, amigo => new { IdentidadID = amigo.IdentidadID, IdentidadAmigoID = amigo.IdentidadAmigoID }, amigoGrupo => new { IdentidadID = amigoGrupo.IdentidadID, IdentidadAmigoID = amigoGrupo.IdentidadAmigoID }, (amigo, amigoGrupo) => new
            {
                Amigo = amigo,
                AmigoGrupo = amigoGrupo
            }).Join(mEntityContext.GrupoAmigos, item => new { IdentidadID = item.AmigoGrupo.IdentidadID, GrupoID = item.AmigoGrupo.GrupoID }, grupoAmigos => new { IdentidadID = grupoAmigos.IdentidadID, GrupoID = grupoAmigos.GrupoID }, (item, grupoAmigos) => new
            {
                Amigo = item.Amigo,
                AmigoGrupo = item.AmigoGrupo,
                GrupoAmigos = grupoAmigos
            }).Join(mEntityContext.PermisoGrupoAmigoOrg, item => new { IdentidadID = item.Amigo.IdentidadID, GrupoID = item.GrupoAmigos.GrupoID }, permiso => new { IdentidadID = permiso.IdentidadOrganizacionID, GrupoID = permiso.GrupoID }, (item, permiso) => new
            {
                Amigo = item.Amigo,
                AmigoGrupo = item.AmigoGrupo,
                GrupoAmigos = item.GrupoAmigos,
                PermisoGrupoAmigoOrg = permiso
            }).Where(item => item.Amigo.IdentidadID.Equals(pIdentidadOrganizacion) && item.PermisoGrupoAmigoOrg.IdentidadUsuarioID.Equals(pIdentidadUsuario)).Select(item => item.Amigo)).ToList();


            //GrupoAmigos de la organización con acceso para el usuario
            //DbCommand commandsqlSelectGruposAmigos = ObtenerComando(sqlSelectGrupoAmigosOrganizacionConPermisoParaUsuario);
            //AgregarParametro(commandsqlSelectGruposAmigos, IBD.ToParam("IdentidadOrganizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadOrganizacion));
            //AgregarParametro(commandsqlSelectGruposAmigos, IBD.ToParam("IdentidadUsuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadUsuario));
            //CargarDataSet(commandsqlSelectGruposAmigos, amigosDS, "GrupoAmigos");

            //AmigoAgGrupo de la organización con acceso para el usuario
            //DbCommand commandsqlSelectAmigosAgGrupos = ObtenerComando(sqlSelectAmigoAgGrupoOrganizacionConPermisoParaUsuario);
            //AgregarParametro(commandsqlSelectAmigosAgGrupos, IBD.ToParam("IdentidadOrganizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadOrganizacion));
            //AgregarParametro(commandsqlSelectAmigosAgGrupos, IBD.ToParam("IdentidadUsuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadUsuario));
            //CargarDataSet(commandsqlSelectAmigosAgGrupos, amigosDS, "AmigoAgGrupo");

            //GrupoAmigos del usuario
            //DbCommand commandsqlSelectGruposAmigosDeIdentidad = ObtenerComando(sqlSelectGrupoAmigosDeIdentidad);
            //AgregarParametro(commandsqlSelectGruposAmigosDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadUsuario));
            //CargarDataSet(commandsqlSelectGruposAmigosDeIdentidad, amigosDS, "GrupoAmigos");

            //AmigoAgGrupo
            //DbCommand commandsqlSelectAmigosAgGruposDeIdentidad = ObtenerComando(sqlSelectAmigoAgGrupoDeIdentidad);
            //AgregarParametro(commandsqlSelectAmigosAgGruposDeIdentidad, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadUsuario));
            //CargarDataSet(commandsqlSelectAmigosAgGruposDeIdentidad, amigosDS, "AmigoAgGrupo");

            //PermisoAmigoOrg
            //DbCommand commandsqlSelectPermisoAmigoOrg = ObtenerComando(sqlSelectPermisoAmigoOrgDeUsuarioYOrganizacion);
            //AgregarParametro(commandsqlSelectPermisoAmigoOrg, IBD.ToParam("IdentidadOrganizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadOrganizacion));
            //AgregarParametro(commandsqlSelectPermisoAmigoOrg, IBD.ToParam("IdentidadUsuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadUsuario));
            //CargarDataSet(commandsqlSelectPermisoAmigoOrg, amigosDS, "PermisoAmigoOrg");

            //PermisoGrupoAmigoOrg
            //sqlSelectPermisoGrupoAmigoOrgDeUsuarioYOrganizacion = selectPermisoGrupoAmigoOrg + " WHERE PermisoGrupoAmigoOrg.IdentidadOrganizacionID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoGrupoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID");
            dataWrapperAmigos.ListaPermisoGrupoAmigoOrg = mEntityContext.PermisoGrupoAmigoOrg.Where(permiso => permiso.IdentidadOrganizacionID.Equals(pIdentidadOrganizacion) && permiso.IdentidadUsuarioID.Equals(pIdentidadUsuario)).ToList();

            return dataWrapperAmigos;
        }

        /// <summary>
        /// Obtiene los amigos de las organizaciones que el usuario administra pero no participa en ellas
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Proyecto al que deben pertenecer los amigos (null si se quieren obtener todos)</param>
        /// <returns></returns>
        public DataWrapperAmigos ObtenerAmigosDeOrganizacionesAdministradas(Guid pUsuarioID, Guid? pProyectoID)
        {
            IdentidadAD identAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<EntityModel.Models.IdentidadDS.Identidad> listaIdentdad = identAD.ObtenerIdentidadesDeMyGnossDEOrganizacionesAdministradas(pUsuarioID, pProyectoID);

            DataWrapperAmigos amigosDW = new DataWrapperAmigos();

            if (listaIdentdad.Count > 0)
            {
                List<Guid> listaIdentidadesID = listaIdentdad.Select(item => item.IdentidadID).ToList();
                var consultaAmigosIdentidad = mEntityContext.Amigo.Where(amigo => listaIdentidadesID.Contains(amigo.IdentidadID));
                var consultaGruposAmigosIdentidad = mEntityContext.GrupoAmigos.Where(grupoAmigo => listaIdentidadesID.Contains(grupoAmigo.IdentidadID));
                var consultaAmigoAgGrupoIdentidad = mEntityContext.AmigoAgGrupo.Where(amigoAgGrupo => listaIdentidadesID.Contains(amigoAgGrupo.IdentidadID));
                if (pProyectoID.HasValue)
                {
                    List<Guid> lista = mEntityContext.Identidad.Join(mEntityContext.Identidad, identidad1=>identidad1.PerfilID, identidad2=>identidad2.PerfilID, (identidad1,identidad2)=>new
                    {
                        Identidad1 = identidad1,
                        Identidad2 = identidad2
                    }).Where(item => item.Identidad1.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad2.Equals(pProyectoID.Value)).Select(item => item.Identidad1.IdentidadID).ToList();
                    consultaAmigosIdentidad = consultaAmigosIdentidad.Where(item => lista.Contains(item.IdentidadAmigoID));
                    //commandsqlSelectAmigosDeIdentidad.CommandText += " AND IdentidadAmigoID IN (SELECT IdentidadID FROM Identidad WHERE PerfilID IN (SELECT PerfilID FROM Identidad WHERE ProyectoID = '" + IBD.ValorDeGuid(pProyectoID.Value) + "') AND ProyectoID = '" + IBD.ValorDeGuid(ProyectoAD.MetaProyecto) + "')";
                }

                //Amigos
                amigosDW.ListaAmigo = consultaAmigosIdentidad.ToList();

                //GrupoAmigos
                amigosDW.ListaGrupoAmigos = consultaGruposAmigosIdentidad.ToList();

                //AmigoAgGrupo
                amigosDW.ListaAmigoAgGrupo = consultaAmigoAgGrupoIdentidad.ToList();
            }

            return amigosDW;
        }
        #endregion

        #region Privados

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
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas

            #region Sólo parte Select

            this.selectSolicitudContacto = "SELECT " + IBD.CargarGuid("SolicitudContacto.SolicitudID") + ", " + IBD.CargarGuid("SolicitudContacto.IdentidadID") + ", " + IBD.CargarGuid("SolicitudContacto.IdentidadIDAmigo") + ", SolicitudContacto.FechaSolicitud, SolicitudContacto.Estado, " + IBD.CargarGuid("SolicitudContacto.GrupoID") + " FROM SolicitudContacto ";

            this.selectAmigo = "SELECT Amigo.EsFanMutuo, Amigo.Fecha, " + IBD.CargarGuid("Amigo.IdentidadAmigoID") + ", " + IBD.CargarGuid("Amigo.IdentidadID") + " FROM Amigo ";

            this.selectGrupoAmigos = "SELECT " + IBD.CargarGuid("GrupoAmigos.GrupoID") + ", GrupoAmigos.Nombre, GrupoAmigos.Fecha, " + IBD.CargarGuid("GrupoAmigos.IdentidadID") + ", GrupoAmigos.Tipo, GrupoAmigos.Automatico FROM GrupoAmigos";

            this.selectAmigoAgGrupo = "SELECT AmigoAgGrupo.Fecha, " + IBD.CargarGuid("AmigoAgGrupo.GrupoID") + ", " + IBD.CargarGuid("AmigoAgGrupo.IdentidadID") + ", " + IBD.CargarGuid("AmigoAgGrupo.IdentidadAmigoID") + " FROM AmigoAgGrupo";

            this.selectSolicitudAmigo = "SELECT " + IBD.CargarGuid("SolicitudAmigo.SolicitudID") + ", SolicitudAmigo.FechaSolicitud, " + IBD.CargarGuid("SolicitudAmigo.IdentidadID") + ", " + IBD.CargarGuid("SolicitudAmigo.IdentidadIDAmigo") + ", SolicitudAmigo.Estado FROM SolicitudAmigo ";

            this.selectPermisoAmigoOrg = "SELECT " + IBD.CargarGuid("PermisoAmigoOrg.IdentidadOrganizacionID") + ", " + IBD.CargarGuid("PermisoAmigoOrg.IdentidadUsuarioID") + ", " + IBD.CargarGuid("PermisoAmigoOrg.IdentidadAmigoID") + " FROM PermisoAmigoOrg";

            this.selectPermisoGrupoAmigoOrg = "SELECT " + IBD.CargarGuid("PermisoGrupoAmigoOrg.IdentidadOrganizacionID") + ", " + IBD.CargarGuid("PermisoGrupoAmigoOrg.IdentidadUsuarioID") + ", PermisoGrupoAmigoOrg.PermisoEdicion, " + IBD.CargarGuid("PermisoGrupoAmigoOrg.GrupoID") + " FROM PermisoGrupoAmigoOrg";

            string whereDeIdentidad = " WHERE IdentidadID =" + IBD.GuidParamValor("identidadID");

            #endregion

            this.sqlSelectGrupoAmigosDeGrupoID = selectGrupoAmigos + " WHERE (GrupoID =" + IBD.GuidParamValor("grupoID") + ")";

            this.sqlSelectAmigoAgGrupoDeGrupoID = selectAmigoAgGrupo + " WHERE (GrupoID =" + IBD.GuidParamValor("grupoID") + ")";

            this.sqlSelectPermisoGrupoAmigoOrgDeGrupoID = selectPermisoGrupoAmigoOrg + " WHERE (GrupoID =" + IBD.GuidParamValor("grupoID") + ")";

            this.sqlSelectAmigosDeIdentidad = selectAmigo + " WHERE (IdentidadID =" + IBD.GuidParamValor("identidadID") + ")";

            this.sqlSelectAmigosDeIdentidadReciproco = selectAmigo + " WHERE (IdentidadID =" + IBD.GuidParamValor("identidadID") + ") OR (IdentidadAmigoID =" + IBD.GuidParamValor("identidadID") + ")";

            this.sqlSelectGrupoAmigosDeIdentidad = selectGrupoAmigos + whereDeIdentidad;

            this.sqlSelectAmigoAgGrupoDeIdentidad = selectAmigoAgGrupo + whereDeIdentidad;

            this.sqlSelectAmigoAgGrupoDeIdentidadReciproco = selectAmigoAgGrupo + whereDeIdentidad + " OR (IdentidadAmigoID =" + IBD.GuidParamValor("identidadID") + ")";

            this.sqlSelectSolicitudAmigosDeIdentidad = selectSolicitudAmigo + " WHERE (IdentidadID =" + IBD.GuidParamValor("identidadID") + " OR IdentidadIDAmigo =" + IBD.GuidParamValor("identidadID") + ") AND Estado=" + (short)EstadoSolicitud.Pendiente;

            this.sqlSelectSolicitudContactosDeIdentidad = selectSolicitudContacto + " WHERE (IdentidadID =" + IBD.GuidParamValor("identidadID") + " OR IdentidadIDAmigo =" + IBD.GuidParamValor("identidadID") + ") AND Estado = " + (short)EstadoSolicitud.Pendiente;

            // Select Amigos de la persona y registros en los que la persona aparezca como amigo
            this.sqlSelectAmigosDePersona = selectAmigo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = Amigo.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectAmigo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = Amigo.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectAmigo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = Amigo.IdentidadAmigoID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectAmigo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = Amigo.IdentidadAmigoID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID");

            // Select solicitudes de amigo hechas por la persona y hacia una persona
            this.sqlSelectSolicitudAmigoDePersona = selectSolicitudAmigo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = SolicitudAmigo.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectSolicitudAmigo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = SolicitudAmigo.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectSolicitudAmigo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = SolicitudAmigo.IdentidadIDAmigo INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectSolicitudAmigo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = SolicitudAmigo.IdentidadIDAmigo INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID");

            // Solicitudes de contacto hechas por la persona y hacia la persona
            sqlSelectSolicitudContactoDePersona = selectSolicitudContacto.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = SolicitudContacto.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectSolicitudContacto.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = SolicitudContacto.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectSolicitudContacto.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = SolicitudContacto.IdentidadIDAmigo INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectSolicitudContacto.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = SolicitudContacto.IdentidadIDAmigo INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID");

            sqlSelectGrupoAmigosDePersona = selectGrupoAmigos.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = GrupoAmigos.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectGrupoAmigos.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = GrupoAmigos.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID");

            sqlSelectAmigoAgGrupoDePersona = selectAmigoAgGrupo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = AmigoAgGrupo.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectAmigoAgGrupo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = AmigoAgGrupo.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectAmigoAgGrupo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = AmigoAgGrupo.IdentidadAmigoID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + selectAmigoAgGrupo.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Identidad ON Identidad.IdentidadID = AmigoAgGrupo.IdentidadAmigoID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID");

            sqlSelectContactosIdentidadDeMyGnoss = "SELECT " + IBD.CargarGuid("Amigo.IdentidadID") + ", " + IBD.CargarGuid("Amigo.IdentidadAmigoID") + " FROM Amigo INNER JOIN Identidad ON Identidad.IdentidadID = Amigo.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID WHERE Identidad.ProyectoID = " + IBD.GuidParamValor("metaproyectoID") + " AND Perfil.PerfilID = " + IBD.GuidParamValor("perfilID");

            sqlSelectAmigosOrganizacionConPermisoParaUsuario = "(" + selectAmigo + " INNER JOIN PermisoAmigoOrg ON Amigo.IdentidadID = PermisoAmigoOrg.IdentidadOrganizacionID WHERE Amigo.IdentidadID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID") + " AND PermisoAmigoOrg.IdentidadAmigoID = Amigo.IdentidadAmigoID) UNION (" + selectAmigo + " INNER JOIN AmigoAgGrupo ON AmigoAgGrupo.IdentidadID = Amigo.IdentidadID AND AmigoAgGrupo.IdentidadAmigoID = Amigo.IdentidadAmigoID INNER JOIN GrupoAmigos ON GrupoAmigos.GrupoID = AmigoAgGrupo.GrupoID AND GrupoAmigos.IdentidadID = AmigoAgGrupo.IdentidadID INNER JOIN PermisoGrupoAmigoOrg ON PermisoGrupoAmigoOrg.IdentidadOrganizacionID = Amigo.IdentidadID AND PermisoGrupoAmigoOrg.GrupoID = GrupoAmigos.GrupoID WHERE Amigo.IdentidadID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoGrupoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID") + ")";

            sqlSelectGrupoAmigosOrganizacionConPermisoParaUsuario = selectGrupoAmigos + " INNER JOIN PermisoGrupoAmigoOrg ON PermisoGrupoAmigoOrg.IdentidadOrganizacionID = GrupoAmigos.IdentidadID AND PermisoGrupoAmigoOrg.GrupoID = GrupoAmigos.GrupoID WHERE GrupoAmigos.IdentidadID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoGrupoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID");

            sqlSelectAmigoAgGrupoOrganizacionConPermisoParaUsuario = selectAmigoAgGrupo + " INNER JOIN PermisoGrupoAmigoOrg ON PermisoGrupoAmigoOrg.IdentidadOrganizacionID = AmigoAgGrupo.IdentidadID AND PermisoGrupoAmigoOrg.GrupoID = AmigoAgGrupo.GrupoID WHERE AmigoAgGrupo.IdentidadID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoGrupoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID");

            sqlSelectPermisoAmigoOrgDeUsuarioYOrganizacion = selectPermisoAmigoOrg + " WHERE PermisoAmigoOrg.IdentidadOrganizacionID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID");

            sqlSelectPermisoGrupoAmigoOrgDeUsuarioYOrganizacion = selectPermisoGrupoAmigoOrg + " WHERE PermisoGrupoAmigoOrg.IdentidadOrganizacionID = " + IBD.GuidParamValor("IdentidadOrganizacionID") + " AND PermisoGrupoAmigoOrg.IdentidadUsuarioID = " + IBD.GuidParamValor("IdentidadUsuarioID");

            sqlSelectPermisoAmigoOrgDeIdentidad = selectPermisoAmigoOrg + " INNER JOIN identidad on PermisoAmigoOrg.IdentidadAmigoID = Identidad.IdentidadID WHERE PermisoAmigoOrg.IdentidadOrganizacionID = " + IBD.GuidParamValor("identidadID") + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL" + " UNION " + selectPermisoAmigoOrg + " INNER JOIN identidad on PermisoAmigoOrg.IdentidadAmigoID = Identidad.IdentidadID WHERE PermisoAmigoOrg.IdentidadAmigoID = " + IBD.GuidParamValor("identidadID") + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL";

            sqlSelectPermisoAmigoOrgDeIdentidadConEliminados = selectPermisoAmigoOrg + " INNER JOIN identidad on PermisoAmigoOrg.IdentidadAmigoID = Identidad.IdentidadID WHERE PermisoAmigoOrg.IdentidadOrganizacionID = " + IBD.GuidParamValor("identidadID") + " UNION " + selectPermisoAmigoOrg + " INNER JOIN identidad on PermisoAmigoOrg.IdentidadAmigoID = Identidad.IdentidadID WHERE PermisoAmigoOrg.IdentidadAmigoID = " + IBD.GuidParamValor("identidadID");

            sqlSelectPermisoGrupoAmigoOrgDeIdentidad = selectPermisoGrupoAmigoOrg + " WHERE PermisoGrupoAmigoOrg.IdentidadOrganizacionID = " + IBD.GuidParamValor("identidadID");

            #endregion

            #region DataAdapter

            #region Amigo
            this.sqlAmigoInsert = IBD.ReplaceParam("INSERT INTO Amigo (EsFanMutuo, Fecha, IdentidadAmigoID, IdentidadID) VALUES (@EsFanMutuo, @Fecha, " + IBD.GuidParamColumnaTabla("IdentidadAmigoID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ")");
            this.sqlAmigoDelete = IBD.ReplaceParam("DELETE FROM Amigo WHERE (EsFanMutuo = @O_EsFanMutuo) AND (Fecha = @O_Fecha) AND (IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("O_IdentidadAmigoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");
            this.sqlAmigoModify = IBD.ReplaceParam("UPDATE Amigo SET EsFanMutuo = @EsFanMutuo, Fecha = @Fecha, IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("IdentidadAmigoID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + " WHERE (EsFanMutuo = @O_EsFanMutuo) AND (Fecha = @O_Fecha) AND (IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("O_IdentidadAmigoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");
            #endregion

            #region GrupoAmigos
            this.sqlGrupoAmigosInsert = IBD.ReplaceParam("INSERT INTO GrupoAmigos (GrupoID, Nombre, Fecha, IdentidadID, Tipo, Automatico) VALUES (" + IBD.GuidParamColumnaTabla("GrupoID") + ", @Nombre, @Fecha, " + IBD.GuidParamColumnaTabla("IdentidadID") + ", @Tipo, @Automatico)");
            this.sqlGrupoAmigosDelete = IBD.ReplaceParam("DELETE FROM GrupoAmigos WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (Nombre = @O_Nombre) AND (Fecha = @O_Fecha) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Tipo = @O_Tipo) AND (Automatico = @O_Automatico)");
            this.sqlGrupoAmigosModify = IBD.ReplaceParam("UPDATE GrupoAmigos SET GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + ", Nombre = @Nombre, Fecha = @Fecha, IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", Tipo = @Tipo, Automatico = @Automatico WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (Nombre = @O_Nombre) AND (Fecha = @O_Fecha) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Tipo = @O_Tipo) AND (Automatico = @O_Automatico)");
            #endregion

            #region AmigoAgGrupo
            this.sqlAmigoAgGrupoInsert = IBD.ReplaceParam("INSERT INTO AmigoAgGrupo (Fecha, GrupoID, IdentidadID, IdentidadAmigoID) VALUES (@Fecha, " + IBD.GuidParamColumnaTabla("GrupoID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ", " + IBD.GuidParamColumnaTabla("IdentidadAmigoID") + ")");
            this.sqlAmigoAgGrupoDelete = IBD.ReplaceParam("DELETE FROM AmigoAgGrupo WHERE (Fecha = @O_Fecha) AND (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("O_IdentidadAmigoID") + ")");
            this.sqlAmigoAgGrupoModify = IBD.ReplaceParam("UPDATE AmigoAgGrupo SET Fecha = @Fecha, GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("IdentidadAmigoID") + " WHERE (Fecha = @O_Fecha) AND (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("O_IdentidadAmigoID") + ")");
            #endregion

            #region SolicitudAmigo
            this.sqlSolicitudAmigoInsert = IBD.ReplaceParam("INSERT INTO SolicitudAmigo (SolicitudID, FechaSolicitud, IdentidadID, IdentidadIDAmigo, Estado) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", @FechaSolicitud, " + IBD.GuidParamColumnaTabla("IdentidadID") + ", " + IBD.GuidParamColumnaTabla("IdentidadIDAmigo") + ", @Estado)");
            this.sqlSolicitudAmigoDelete = IBD.ReplaceParam("DELETE FROM SolicitudAmigo WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (FechaSolicitud = @O_FechaSolicitud) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (IdentidadIDAmigo = " + IBD.GuidParamColumnaTabla("O_IdentidadIDAmigo") + " AND (Estado = @O_Estado) )");
            this.sqlSolicitudAmigoModify = IBD.ReplaceParam("UPDATE SolicitudAmigo SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", FechaSolicitud = @FechaSolicitud, IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", IdentidadIDAmigo = " + IBD.GuidParamColumnaTabla("IdentidadIDAmigo") + ", Estado = @Estado WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (FechaSolicitud = @O_FechaSolicitud) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (IdentidadIDAmigo = " + IBD.GuidParamColumnaTabla("O_IdentidadIDAmigo") + " AND (Estado = @O_Estado))");
            #endregion

            #region SolicitudContacto
            this.sqlSolicitudContactoInsert = IBD.ReplaceParam("INSERT INTO SolicitudContacto (SolicitudID, IdentidadID, IdentidadIDAmigo, FechaSolicitud, Estado, GrupoID) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ", " + IBD.GuidParamColumnaTabla("IdentidadIDAmigo") + ", @FechaSolicitud, @Estado," + IBD.GuidParamColumnaTabla("GrupoID") + ")");
            this.sqlSolicitudContactoDelete = IBD.ReplaceParam("DELETE FROM SolicitudContacto WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (IdentidadIDAmigo = " + IBD.GuidParamColumnaTabla("O_IdentidadIDAmigo") + ") AND (FechaSolicitud = @O_FechaSolicitud) AND (Estado = @O_Estado) AND (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + " OR " + IBD.GuidParamColumnaTabla("O_GrupoID") + " IS NULL AND GrupoID IS NULL)");
            this.sqlSolicitudContactoModify = IBD.ReplaceParam("UPDATE SolicitudContacto SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", IdentidadIDAmigo = " + IBD.GuidParamColumnaTabla("IdentidadIDAmigo") + ", FechaSolicitud = @FechaSolicitud, Estado = @Estado, GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + " WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (IdentidadIDAmigo = " + IBD.GuidParamColumnaTabla("O_IdentidadIDAmigo") + ") AND (FechaSolicitud = @O_FechaSolicitud) AND (Estado = @O_Estado) AND (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + " OR " + IBD.GuidParamColumnaTabla("O_GrupoID") + " IS NULL AND GrupoID IS NULL)");
            #endregion

            #region PermisoAmigoOrg
            this.sqlPermisoAmigoOrgInsert = IBD.ReplaceParam("INSERT INTO PermisoAmigoOrg (IdentidadOrganizacionID, IdentidadUsuarioID, IdentidadAmigoID) VALUES (" + IBD.GuidParamColumnaTabla("IdentidadOrganizacionID") + ", " + IBD.GuidParamColumnaTabla("IdentidadUsuarioID") + ", " + IBD.GuidParamColumnaTabla("IdentidadAmigoID") + ")");
            this.sqlPermisoAmigoOrgDelete = IBD.ReplaceParam("DELETE FROM PermisoAmigoOrg WHERE (IdentidadOrganizacionID = " + IBD.GuidParamColumnaTabla("O_IdentidadOrganizacionID") + ") AND (IdentidadUsuarioID = " + IBD.GuidParamColumnaTabla("O_IdentidadUsuarioID") + ") AND (IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("O_IdentidadAmigoID") + ")");
            this.sqlPermisoAmigoOrgModify = IBD.ReplaceParam("UPDATE PermisoAmigoOrg SET IdentidadOrganizacionID = " + IBD.GuidParamColumnaTabla("IdentidadOrganizacionID") + ", IdentidadUsuarioID = " + IBD.GuidParamColumnaTabla("IdentidadUsuarioID") + ", IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("IdentidadAmigoID") + " WHERE (IdentidadOrganizacionID = " + IBD.GuidParamColumnaTabla("O_IdentidadOrganizacionID") + ") AND (IdentidadUsuarioID = " + IBD.GuidParamColumnaTabla("O_IdentidadUsuarioID") + ") AND (IdentidadAmigoID = " + IBD.GuidParamColumnaTabla("O_IdentidadAmigoID") + ")");
            #endregion

            #region PermisoGrupoAmigoOrg
            this.sqlPermisoGrupoAmigoOrgInsert = IBD.ReplaceParam("INSERT INTO PermisoGrupoAmigoOrg (IdentidadOrganizacionID, IdentidadUsuarioID, PermisoEdicion, GrupoID) VALUES (" + IBD.GuidParamColumnaTabla("IdentidadOrganizacionID") + ", " + IBD.GuidParamColumnaTabla("IdentidadUsuarioID") + ", @PermisoEdicion, " + IBD.GuidParamColumnaTabla("GrupoID") + ")");
            this.sqlPermisoGrupoAmigoOrgDelete = IBD.ReplaceParam("DELETE FROM PermisoGrupoAmigoOrg WHERE (IdentidadOrganizacionID = " + IBD.GuidParamColumnaTabla("O_IdentidadOrganizacionID") + ") AND (IdentidadUsuarioID = " + IBD.GuidParamColumnaTabla("O_IdentidadUsuarioID") + ") AND (PermisoEdicion = @O_PermisoEdicion) AND (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ")");
            this.sqlPermisoGrupoAmigoOrgModify = IBD.ReplaceParam("UPDATE PermisoGrupoAmigoOrg SET IdentidadOrganizacionID = " + IBD.GuidParamColumnaTabla("IdentidadOrganizacionID") + ", IdentidadUsuarioID = " + IBD.GuidParamColumnaTabla("IdentidadUsuarioID") + ", PermisoEdicion = @PermisoEdicion, GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + " WHERE (IdentidadOrganizacionID = " + IBD.GuidParamColumnaTabla("O_IdentidadOrganizacionID") + ") AND (IdentidadUsuarioID = " + IBD.GuidParamColumnaTabla("O_IdentidadUsuarioID") + ") AND (PermisoEdicion = @O_PermisoEdicion) AND (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ")");
            #endregion

            #endregion
        }

        #endregion

        #endregion
    }
}
