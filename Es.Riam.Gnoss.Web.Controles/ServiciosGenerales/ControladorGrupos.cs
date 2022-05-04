using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.ServiciosGenerales
{
    public class ControladorGrupos : ControladorBase
    {

        private VirtuosoAD mVirtuosoAD;
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private EntityContextBASE mEntityContextBASE;

        public ControladorGrupos(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            :  base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;
        }

        /// <summary>
        /// Elimina un usuario de todos los grupos de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de la comunidad</param>
        /// <param name="pIdentidadID">ID de Identidad</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public void EliminarAUsuarioDeGruposDeComunidad(Guid pProyectoID, Guid pIdentidadID, string pUrlIntragnoss)
        {
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperIdentidad identDW = identCN.ObtenerGruposDeProyecto(pProyectoID, true);
            identCN.Dispose();

            List<object[]> datosEliminar = new List<object[]>();
            foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupo in identDW.ListaGrupoIdentidades)
            {//"GrupoID='" + filaGrupo.GrupoID + "' AND IdentidadID='" + pIdentidadID + "'"
                if (identDW.ListaGrupoIdentidadesParticipacion.Count(grupoIdenPart => grupoIdenPart.GrupoID.Equals(filaGrupo.GrupoID) && grupoIdenPart.IdentidadID.Equals(pIdentidadID)) > 0)
                {
                    datosEliminar.Add(new object[] { filaGrupo.GrupoID, filaGrupo.Nombre, filaGrupo.NombreCorto });
                }
            }

            foreach (object[] datoEliminacion in datosEliminar)
            {
                EliminarIdentidadDeGruposDeComunidad(pProyectoID, pIdentidadID, pUrlIntragnoss, identDW, (Guid)datoEliminacion[0], (string)datoEliminacion[1], (string)datoEliminacion[2], false);
            }
        }

        /// <summary>
        /// Elimina un usuario de todos los grupos de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de la comunidad</param>
        /// <param name="pIdentidadID">ID de Identidad</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <returns>Cadena con el mensaje de error ó vacío</returns>
        public string EliminarUsuariosDeGrupoDeComunidad(Guid pProyectoID, string pUrlIntragnoss, Guid pGrupoID, List<Guid> pListaIdentidadesID)
        {
            string error = "";
            try
            {
                if (!pGrupoID.Equals(Guid.Empty))
                {
                    List<Guid> listaGrupos = new List<Guid>();
                    listaGrupos.Add(pGrupoID);
                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperIdentidad identDW = identCN.ObtenerGruposPorIDGrupo(listaGrupos);
                    identCN.Dispose();

                    AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupo = identDW.ListaGrupoIdentidades.FirstOrDefault(grupoIden => grupoIden.GrupoID.Equals(pGrupoID));
                    if (filaGrupo != null && pListaIdentidadesID.Count > 0)
                    {
                        foreach (Guid identidadID in pListaIdentidadesID)
                        {
                            try
                            {
                                EliminarIdentidadDeGruposDeComunidad(pProyectoID, identidadID, pUrlIntragnoss, identDW, pGrupoID, filaGrupo.Nombre, filaGrupo.NombreCorto, true);
                            }
                            catch (Exception)
                            {
                                error += "\r\n No se ha podido eliminar la identidad: " + identidadID + " del grupo: " + pGrupoID.ToString();
                            }
                        }

                        ActualizarBase(pProyectoID, pGrupoID);
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, error);
                error = "\r\n ERROR ControladorIdentidades.EliminarUsuariosDeGrupoDeComunidad." + error;
            }

            return error;
        }

        /// <summary>
        /// Elimina una identidad de un grupo de comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de la comunidad</param>
        /// <param name="pIdentidadID">ID de Identidad</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pDataWrapperIdentidad">DataSet con los datos de los grupos cargados</param>
        /// <param name="pGrupoID">ID del grupo</param>
        /// <param name="pNombre">Nombre del grupo</param>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        public void EliminarIdentidadDeGruposDeComunidad(Guid pProyectoID, Guid pIdentidadID, string pUrlIntragnoss, DataWrapperIdentidad pDataWrapperIdentidad, Guid pGrupoID, string pNombre, string pNombreCorto, bool pBorradoMultiple)
        {
            EliminarIdentidadDeGrupoDeComunidad(pProyectoID, pIdentidadID, pDataWrapperIdentidad, pGrupoID, pNombre, pNombreCorto, pBorradoMultiple);

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionIdentidades gestIdenti = new GestionIdentidades(identidadCN.ObtenerIdentidadPorIDCargaLigeraTablas(pIdentidadID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            Identidad identidad = gestIdenti.ListaIdentidades[pIdentidadID];

            FacetadoCN facetadoCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCN.BorrarParticipanteDeGrupo(pGrupoID, identidad.Clave, identidad.NombreCompuesto(), pProyectoID);
            facetadoCN.Dispose();

            FacetadoCL facetadoCL = new FacetadoCL(pUrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheQueContengaCadena(NombresCL.PRIMEROSRECURSOS + "_" + pProyectoID.ToString() + "_" + identidad.PerfilID);
            facetadoCL.Dispose();
        }

        /// <summary>
        /// Elimina una identidad de un grupo de comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de la comunidad</param>
        /// <param name="pIdentidadID">ID de Identidad</param>
        /// <param name="pDataWrapperIdentidad">DataSet con los datos de los grupos cargados</param>
        /// <param name="pGrupoID">ID del grupo</param>
        /// <param name="pNombre">Nombre del grupo</param>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pBorradoMultiple">Si se van a borrar varios usuarios no invalida la caché del grupo. Lo tendrá que borrar el que consuma este método para evitar meter una fila al Base por cada llamada</param>
        private void EliminarIdentidadDeGrupoDeComunidad(Guid pProyectoID, Guid pIdentidadID, DataWrapperIdentidad pDataWrapperIdentidad, Guid pGrupoID, string pNombre, string pNombreCorto, bool pBorradoMultiple)
        {
            AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion grupIdentParticipacion = pDataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.FirstOrDefault(grupoIdenPart => grupoIdenPart.GrupoID.Equals(pGrupoID) && grupoIdenPart.IdentidadID.Equals(pIdentidadID));
            if (grupIdentParticipacion != null)
            {
                pDataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Remove(grupIdentParticipacion);
                mEntityContext.EliminarElemento(grupIdentParticipacion);
                List<Guid> identidadEliminada = new List<Guid>();
                identidadEliminada.Add(pIdentidadID);

                //AQUI no nos hace falta de momento
                ////Notificamos a los usuarios de que han sido eliminados del grupo.
                //EnviarMensajeMiembros(identidadEliminada, pNombre, pNombreCorto, true);

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.ActualizaIdentidades();
                identidadCN.Dispose();

                if (!pBorradoMultiple)
                {
                    InvalidarCacheGrupo(false, null, pNombreCorto, pProyectoID, null, null);
                }
            }
        }

        public void EliminarGrupoComunidad(string pNombreCortoGrupo, Guid pProyectoID, GestionIdentidades pGestorIdentidades, ParametroGeneral pParametroGeneralRow, string pUrlIntraGnoss, bool pEsGrupoOrganizacion, Dictionary<string, List<string>> pInformacionOntologias, Guid? pOrganizacionIDIdentidadActual)
        {
            Guid grupoID = pGestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades.FirstOrDefault().GrupoID;
            GrupoIdentidades grupo = pGestorIdentidades.ListaGrupos[grupoID];

            #region Virtuoso

            FacetadoCN facetadoCN = new FacetadoCN(pUrlIntraGnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCN.BorrarGrupo(grupoID, grupo.Nombre, pProyectoID);
            facetadoCN.Dispose();

            #endregion

            #region Cache

            FacetadoCL facetadoCL = new FacetadoCL(pUrlIntraGnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarCacheQueContengaCadena(NombresCL.PRIMEROSRECURSOS + "_" + pProyectoID.ToString());
            facetadoCL.Dispose();

            #endregion

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades> listaDocumentoRolGrupoIdentidades = docCN.ObtenerFilasGrupoEditorRecurso(grupoID);

            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaDocRolGrupoIdent in listaDocumentoRolGrupoIdentidades)
            {
                mEntityContext.EliminarElemento(filaDocRolGrupoIdent);
            }

            //docCN.ActualizarDocumentacion(docDS);
            //docCN.Dispose();

            foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion fila in grupo.FilasGrupoIdentidadesParticipacion)
            {
                mEntityContext.EliminarElemento(fila);
            }

            //Eliminar las solicitudes pendientes a este grupo:
            SolicitudCN solicitudCN = new SolicitudCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperSolicitud solicitudDW = solicitudCN.ObtenerSolicitudesGrupoPorProyecto(pProyectoID);
            List<SolicitudGrupo> listaSolicitudGrupoBorrar = solicitudDW.ListaSolicitudGrupo.Where(item => item.GrupoID.Equals(grupoID)).ToList();
            foreach (SolicitudGrupo sgr in listaSolicitudGrupoBorrar)
            {
                List<Solicitud> listaSolicitud = solicitudDW.ListaSolicitud.Where(item => item.SolicitudID.Equals(sgr.SolicitudID)).ToList();
                foreach (Solicitud solicitudRow in listaSolicitud)
                {
                    solicitudDW.ListaSolicitud.Remove(solicitudRow);
                    mEntityContext.EliminarElemento(solicitudRow);
                }

                solicitudDW.ListaSolicitudGrupo.Remove(sgr);
                mEntityContext.EliminarElemento(sgr);
            }

            ////Guardamos los cambios hechos en el SolicitudDS.
            //solicitudCN.ActualizarBD(solicitudDS);
            //solicitudCN.Dispose();

            //foreach (IdentidadDS.GrupoIdentidadesParticipacionRow fila in grupo.FilasGrupoIdentidadesParticipacion)
            //{
            //    fila.Delete();
            //}

            if (grupo.FilaGrupoProyecto != null)
            {
                mEntityContext.EliminarElemento(grupo.FilaGrupoProyecto);
            }
            if (grupo.FilaGrupoOrganizacion != null)
            {
                mEntityContext.EliminarElemento(grupo.FilaGrupoOrganizacion);
            }

            mEntityContext.EliminarElemento(grupo.FilaGrupoIdentidades);

            //IdentidadCN identidadCN = new IdentidadCN();
            //identidadCN.ActualizaIdentidades(grupo.GestorIdentidades.IdentidadesDS);
            //identidadCN.Dispose();
            DataWrapperDocumentacion dataWrapperDoc = new DataWrapperDocumentacion();
            dataWrapperDoc.ListaDocumentoRolGrupoIdentidades = listaDocumentoRolGrupoIdentidades;
            mEntityContext.SaveChanges();

            //string nombreCortoGrupo = Encoding.UTF8.GetString(Encoding.GetEncoding("iso8859-1").GetBytes(HttpUtility.UrlDecode(Request.Params["grupo"])));

            InvalidarCacheGrupo(pEsGrupoOrganizacion, pOrganizacionIDIdentidadActual, pNombreCortoGrupo, pProyectoID, pParametroGeneralRow, pInformacionOntologias);
        }

        public void ActualizarBase(Guid pProyectoID, Guid pGrupoID)
        {
            ActualizarBase(pProyectoID, pGrupoID, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pGrupoID"></param>
        /// <param name="pNombreGrupoViejo"></param>
        public void ActualizarBase(Guid pProyectoID, Guid pGrupoID, string pNombreGrupoViejo)
        {
            try
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                int tablaBaseProyectoID = proyCN.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
                proyCN.Dispose();

                BasePerOrgComunidadDS basePerOrgComDS = new BasePerOrgComunidadDS();

                BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org_Add = basePerOrgComDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();

                filaColaTagsCom_Per_Org_Add.TablaBaseProyectoID = tablaBaseProyectoID;
                filaColaTagsCom_Per_Org_Add.Tags = Constantes.PERS_U_ORG + "g" + Constantes.PERS_U_ORG + ", " + Constantes.ID_TAG_PER + pGrupoID + Constantes.ID_TAG_PER;

                if (!string.IsNullOrEmpty(pNombreGrupoViejo))
                {
                    filaColaTagsCom_Per_Org_Add.Tags += ", " + Constantes.NOMBRE_PER_COMPLETO + pNombreGrupoViejo + Constantes.NOMBRE_PER_COMPLETO;
                }

                filaColaTagsCom_Per_Org_Add.Tipo = (short)TiposElementosEnCola.Agregado;
                filaColaTagsCom_Per_Org_Add.Estado = 0;
                filaColaTagsCom_Per_Org_Add.FechaPuestaEnCola = DateTime.Now;
                filaColaTagsCom_Per_Org_Add.Prioridad = (short)PrioridadBase.Alta;

                basePerOrgComDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org_Add);

                BaseComunidadCN basePerOrgComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
                basePerOrgComCN.InsertarFilasEnRabbit("ColaTagsCom_Per_Org", basePerOrgComDS);
                basePerOrgComCN.Dispose();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"No se ha podido ActualizarBase para el grupo {pGrupoID}");
            }
        }

        public void InvalidarCacheGrupo(bool pEsGrupoOrganizacion, Guid? pOrganizacionIDIdentidadActual, string pNombreCortoGrupo, Guid pProyectoID, ParametroGeneral pParametroGeneralRow, Dictionary<string, List<string>> pInformacionOntologias)
        {

            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (pEsGrupoOrganizacion && pOrganizacionIDIdentidadActual.HasValue)
            {
                identidadCL.InvalidarCacheMiembrosOrganizacionParaFiltroGrupos(pOrganizacionIDIdentidadActual.Value);
                identidadCL.InvalidarCacheGrupoPorNombreCortoYOrganizacion(pNombreCortoGrupo, pOrganizacionIDIdentidadActual.Value);
            }
            else
            {
                identidadCL.InvalidarCacheMiembrosComunidad(pProyectoID);

                identidadCL.InvalidarCacheGrupoPorNombreCortoYProyecto(pNombreCortoGrupo, pProyectoID);

                BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
                try
                {
                    baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, null);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                    baseComunidadCN.InsertarFilaEnColaRefrescoCache(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos);
                }


                if (pParametroGeneralRow != null)
                {
                    if (pParametroGeneralRow.PreguntasDisponibles)
                    {
                        try
                        {
                            baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Preguntas, null);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                            baseComunidadCN.InsertarFilaEnColaRefrescoCache(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Preguntas);
                        }
                    }
                    if (pParametroGeneralRow.DebatesDisponibles)
                    {
                        try
                        {
                            baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Debates, null);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                            baseComunidadCN.InsertarFilaEnColaRefrescoCache(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Debates);
                        }
                    }
                    if (pParametroGeneralRow.EncuestasDisponibles)
                    {
                        try
                        {
                            baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Encuestas, null);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                            baseComunidadCN.InsertarFilaEnColaRefrescoCache(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Encuestas);
                        }

                    }
                }
                if (pInformacionOntologias != null)
                {
                    foreach (List<string> ontologias in pInformacionOntologias.Values)
                    {
                        foreach (string ns in ontologias)
                        {
                            try
                            {
                                baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, $"rdf:type={ns}");
                            }
                            catch (Exception ex)
                            {
                                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                                baseComunidadCN.InsertarFilaEnColaRefrescoCache(pProyectoID, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, $"rdf:type={ns}");
                            }
                        }
                    }
                }

                baseComunidadCN.Dispose();
            }
            identidadCL.Dispose();
        }
    }
}
