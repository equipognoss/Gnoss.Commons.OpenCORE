using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.ServiciosGenerales
{

    /// <summary>
    /// Controlador para Personas
    /// </summary>
    public class ControladorPersonas : ControladorBase
    {
        #region Miembros asíncronos

        private Guid mProyectoID;
        private Guid? mCvID;
        private Identidad mIdentidad;
        private bool mEntraEnCom;
        private bool mActualizarTagsPersonaSincronamente = false;
        private bool mActualizarModeloAcido = true;
        private string mTagsViejos;

        /// <summary>
        /// ID de la tabla base del proyecto que hay que actualizar.
        /// </summary>
        private int mTablaBaseProyectoID;

        private EntityContextBASE mEntityContextBASE;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pPage">Página</param>
        public ControladorPersonas(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mEntityContextBASE = entityContextBASE;
        }

        #endregion

        #region Metodos generales

        /// <summary>
        /// Obtiene el nombre del país de una persona
        /// </summary>
        /// <param name="pPersona">Persona de la que se quiere obtener el país</param>
        /// <returns></returns>
        public string ObtenerNombrePaisPersona(Persona pPersona)
        {
            string nombrePais = "";

            if (pPersona.FilaPersona.PaisPersonalID.HasValue)
            {
                DataWrapperPais paisDW = pPersona.GestorPersonas.PaisDW;

                if (paisDW == null)
                {
                    PaisCN paisCN = new PaisCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paisDW = paisCN.ObtenerPaisesProvincias();
                    paisCN.Dispose();

                    pPersona.GestorPersonas.PaisDW = paisDW;
                }

                if (paisDW != null)
                {
                    AD.EntityModel.Models.Pais.Pais filasPais = paisDW.ListaPais.Where(item => item.PaisID.Equals(pPersona.FilaPersona.PaisPersonalID.Value)).FirstOrDefault();

                    if ((filasPais != null))
                    {
                        nombrePais = filasPais.Nombre;
                    }
                }
            }

            return nombrePais;
        }

        /// <summary>
        /// Obtiene el nombre de la provincia de una persona
        /// </summary>
        /// <param name="pPersona">Persona de la que se quiere obtener la provincia</param>
        /// <returns></returns>
        public string ObtenerNombreProvinciaPersona(Persona pPersona)
        {
            string nombreProvincia = "";

            if (!pPersona.FilaPersona.ProvinciaPersonalID.HasValue && pPersona.FilaPersona.ProvinciaPersonal != null)
            {
                nombreProvincia = pPersona.FilaPersona.ProvinciaPersonal;
            }
            else if (pPersona.FilaPersona.ProvinciaPersonalID.HasValue && pPersona.FilaPersona.PaisPersonalID.HasValue)
            {
                DataWrapperPais paisDS = pPersona.GestorPersonas.PaisDW;

                if (paisDS == null)
                {
                    PaisCN paisCN = new PaisCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paisDS = paisCN.ObtenerProvinciasDePais(pPersona.FilaPersona.PaisPersonalID.Value);
                    paisCN.Dispose();

                    pPersona.GestorPersonas.PaisDW = paisDS;
                }

                if (paisDS != null)
                {
                    AD.EntityModel.Models.Pais.Provincia filasProvincia = paisDS.ListaProvincia.Where(item => item.ProvinciaID.Equals(pPersona.FilaPersona.ProvinciaPersonalID)).FirstOrDefault();

                    if ((filasProvincia != null))
                    {
                        nombreProvincia = filasProvincia.Nombre;
                    }
                }
            }

            return nombreProvincia;
        }

        /// <summary>
        /// Actualiza el modelo base
        /// </summary>
        /// <param name="pIdentidad">Identidad que se ha metido o salido de una comunidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pEntraEnCom">Verdad si ha entrado en la comunidad, falso si ha salido de ella</param>
        /// <param name="pActualizarTagsPersonas">Verdad si se deben actualizar los tags de las personas</param>
        public void ActualizarModeloBASE(Identidad pIdentidad, Guid pProyectoID, bool pEntraEnCom, bool pActualizarTagsPersonasSincronamente, PrioridadBase pPrioridadBase)
        {
            ActualizarModeloBASE(pIdentidad, pProyectoID, pEntraEnCom, pActualizarTagsPersonasSincronamente, null, pPrioridadBase);
        }

        /// <summary>
        /// Actualiza el modelo base
        /// </summary>
        /// <param name="pIdentidad">Identidad que se ha metido o salido de una comunidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pEntraEnCom">Verdad si ha entrado en la comunidad, falso si ha salido de ella</param>
        /// <param name="pActualizarTagsPersonas">Verdad si se deben actualizar los tags de las personas</param>
        /// <param name="pTablaBaseProyectoID">ID de la tabla base del proyecto que hay que actualizar</param>
        public void ActualizarModeloBASE(Identidad pIdentidad, Guid pProyectoID, bool pEntraEnCom, bool pActualizarTagsPersonasSincronamente, int pTablaBaseProyectoID, PrioridadBase pPrioridadBase)
        {
            mTablaBaseProyectoID = pTablaBaseProyectoID;
            ActualizarModeloBASE(pIdentidad, pProyectoID, pEntraEnCom, pActualizarTagsPersonasSincronamente, null, pPrioridadBase);
        }

        /// <summary>
        /// Actualiza el modelo base
        /// </summary>
        /// <param name="pIdentidad">Identidad que se ha metido o salido de una comunidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pEntraEnCom">Verdad si ha entrado en la comunidad, falso si ha salido de ella</param>
        /// <param name="pActualizarTagsPersonas">Verdad si se deben actualizar los tags de las personas</param>
        /// <param name="pTagsViejos">Tags viejos antes del cambio</param>
        public void ActualizarModeloBASE(Identidad pIdentidad, Guid pProyectoID, bool pEntraEnCom, bool pActualizarTagsPersonasSincronamente, string pTagsViejos, PrioridadBase pPrioridadBase)
        {
            ActualizarModeloBASE(pIdentidad, pProyectoID, pEntraEnCom, pActualizarTagsPersonasSincronamente, pTagsViejos, true, pPrioridadBase);
        }

        /// <summary>
        /// Actualiza el modelo base
        /// </summary>
        /// <param name="pIdentidad">Identidad que se ha metido o salido de una comunidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pEntraEnCom">Verdad si ha entrado en la comunidad, falso si ha salido de ella</param>
        /// <param name="pActualizarTagsPersonas">Verdad si se deben actualizar los tags de las personas</param>
        /// <param name="pTagsViejos">Tags viejos antes del cambio</param>
        /// <param name="pActualizarModeloAcido">Verdad si se debe actualizar también el modelo Ácido</param>
        public void ActualizarModeloBASE(Identidad pIdentidad, Guid pProyectoID, bool pEntraEnCom, bool pActualizarTagsPersonasSincronamente, string pTagsViejos, bool pActualizarModeloAcido, PrioridadBase pPrioridadBase)
        {
            mProyectoID = pProyectoID;
            mEntraEnCom = pEntraEnCom;
            mIdentidad = pIdentidad;
            mTagsViejos = pTagsViejos;
            mActualizarTagsPersonaSincronamente = pActualizarTagsPersonasSincronamente;
            mActualizarModeloAcido = pActualizarModeloAcido;

            if (pIdentidad.FilaIdentidad.CurriculumID.HasValue)
            {
                mCvID = pIdentidad.FilaIdentidad.CurriculumID.Value;
            }

            if (pActualizarTagsPersonasSincronamente)
            {
                ActualizarTagsPersona(pActualizarTagsPersonasSincronamente);
            }

            //He tenido que quitar el hilo porque desde la versión MVC no funcionan los hilos
            //if (mEntraEnCom)
            //{
            //    Thread t = new Thread(ActualizarModeloBASEAsincronamente);
            //    t.SetApartmentState(ApartmentState.STA);
            //    t.Start();
            //}
            //else
            {
                ActualizarModeloBASEAsincronamente(pPrioridadBase);
            }
        }

        /// <summary>
        /// Carga un DataSet con el modelo base de forma masiva (no guarda en BD). Pasar cargado el gestor de personas de las identidades.
        /// </summary>
        /// <param name="pListaIdentidades">Identidades que se ha metido o salido de una comunidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pEntraEnCom">Verdad si ha entrado en la comunidad, falso si ha salido de ella</param>
        /// <param name="pActualizarTagsPersonas">Verdad si se deben actualizar los tags de las personas</param>
        /// <param name="pActualizarModeloAcido">Verdad si se debe actualizar también el modelo Ácido</param>
        /// <param name="pPrioridadBase">Prioridad para el Base</param>
        public void ActualizarModeloBASEMasivo(List<Identidad> pListaIdentidades, Guid pProyectoID, int pTablaBaseProyectoID, bool pEntraEnCom, bool pActualizarTagsPersonasSincronamente, bool pActualizarModeloAcido, PrioridadBase pPrioridadBase)
        {
            ActualizarModeloBASEAsincronamenteMasivo(pListaIdentidades, pProyectoID, pTablaBaseProyectoID, pEntraEnCom, pActualizarModeloAcido, pActualizarTagsPersonasSincronamente, pPrioridadBase);
        }

        private void ActualizarTagsPersona(bool pModificarTags)
        {
            Identidad identidad = mIdentidad;

            if (pModificarTags)
            {
                GestionIdentidades gestorIdentidades = identidad.GestorIdentidades;

                PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                bool gestorContienePersona = (gestorIdentidades.GestorPersonas != null) && identidad.PerfilUsuario.FilaPerfil.PersonaID.HasValue && gestorIdentidades.GestorPersonas.ListaPersonas.ContainsKey(identidad.PerfilUsuario.FilaPerfil.PersonaID.Value);

                if (!gestorContienePersona && identidad.PerfilUsuario.FilaPerfil.PersonaID.HasValue)
                {
                    gestorIdentidades.GestorPersonas = new GestionPersonas(persCN.ObtenerPersonaPorIDCargaLigera(identidad.PerfilUsuario.FilaPerfil.PersonaID.Value), mLoggingService, mEntityContext);
                }
            }
        }

        public void ActualizarModeloBaseSimple(Identidad pIdentidad, Guid pProyectoID, string pUrlIntragnoss)
        {
            if (pUrlIntragnoss == null)
            {
                pUrlIntragnoss = UrlIntragnoss;
            }
            new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GuardarIdentidadEnGrafoBusqueda(pIdentidad, pProyectoID, pUrlIntragnoss);
        }

        public void ActualizarModeloBaseSimpleMultiple(Guid pPersonaID, List<Guid> pListaProyectosID)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, int> diccionarioProyectoBaseProyectoID = proyCN.ObtenerTablasBaseProyectoIDProyectoPorID(pListaProyectosID);
            proyCN.Dispose();

            BaseComunidadCN basePerOrgComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            BasePerOrgComunidadDS basePerOrgComDS = new BasePerOrgComunidadDS();

            foreach (Guid proyectoID in diccionarioProyectoBaseProyectoID.Keys)
            {
                string todosTags = Constantes.PERS_U_ORG + "p" + Constantes.PERS_U_ORG;
                todosTags += ", " + Constantes.ID_TAG_PER + pPersonaID + Constantes.ID_TAG_PER;

                BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org_Add = basePerOrgComDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
                MeterValoresEnFilaCola(filaColaTagsCom_Per_Org_Add, todosTags, diccionarioProyectoBaseProyectoID[proyectoID], TiposElementosEnCola.Agregado, PrioridadBase.Alta);
                basePerOrgComDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org_Add);
            }

            basePerOrgComCN.InsertarFilasEnRabbit("ColaTagsCom_Per_Org", basePerOrgComDS);
        }

        public void ActualizarEliminacionModeloBaseSimple(Guid pPersonaID, Guid pProyectoID, PrioridadBase pPrioridadBase)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);

            BaseComunidadCN basePerOrgComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            BasePerOrgComunidadDS basePerOrgComDS = new BasePerOrgComunidadDS();

            string todosTags = Constantes.PERS_U_ORG + "p" + Constantes.PERS_U_ORG;
            todosTags += ", " + Constantes.ID_TAG_PER + pPersonaID + Constantes.ID_TAG_PER;

            BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org_Del = basePerOrgComDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
            MeterValoresEnFilaCola(filaColaTagsCom_Per_Org_Del, todosTags, id, TiposElementosEnCola.Eliminado, pPrioridadBase);
            basePerOrgComDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org_Del);

            basePerOrgComCN.InsertarFilasEnRabbit("ColaTagsCom_Per_Org", basePerOrgComDS);
        }

        /// <summary>
        /// Actualiza el modelo base
        /// </summary>
        /// <param name="pIdentidad">Identidad que se ha metido o salido de una comunidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pEntraEnCom">Verdad si ha entrado en la comunidad, falso si ha salido de ella</param>
        private void ActualizarModeloBASEAsincronamente()
        {
            ActualizarModeloBASEAsincronamente(PrioridadBase.Alta);
        }

        /// <summary>
        /// Actualiza el modelo base
        /// </summary>
        /// <param name="pIdentidad">Identidad que se ha metido o salido de una comunidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pEntraEnCom">Verdad si ha entrado en la comunidad, falso si ha salido de ella</param>
        private void ActualizarModeloBASEAsincronamente(PrioridadBase pPrioridadBase)
        {
            try
            {
                Identidad identidad = mIdentidad;

                if (mEntraEnCom)
                {
                    ActualizarModeloBaseSimple(identidad, mProyectoID, UrlIntragnoss);
                }
                else
                {
                    if (mActualizarModeloAcido && (!mActualizarTagsPersonaSincronamente))
                    {
                        try
                        {
                            ActualizarTagsPersona(mActualizarTagsPersonaSincronamente);
                        }
                        catch (Exception)
                        {
                            ActualizarTagsPersona(mActualizarTagsPersonaSincronamente);
                        }
                    }
                }
                BaseComunidadCN basePerOrgComunidadCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
                BasePerOrgComunidadDS basePerOrgComunidadDS = new BasePerOrgComunidadDS();
                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = identidad.FilaIdentidad;

                #region Tags Manuales y automaticos "ColaTagsCom_Per_Org"

                AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = identidad.PerfilUsuario.FilaPerfil;
                List<string> listaTags = new List<string>();
                string todosTags = "";

                if (filaIdentidad.Tipo == (short)TiposIdentidad.Personal || filaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || filaIdentidad.Tipo == (short)TiposIdentidad.Profesor || !mEntraEnCom)
                {
                    Guid personaID = filaPerfil.PersonaID.Value;
                    listaTags.Add(Constantes.PERS_U_ORG + "p" + Constantes.PERS_U_ORG);
                    listaTags.Add(Constantes.ID_TAG_PER + personaID + Constantes.ID_TAG_PER);

                    todosTags = UtilCadenas.ComponerTextoSepComasDeLista(listaTags);

                    TiposElementosEnCola pTipo = TiposElementosEnCola.Agregado;

                    if (!mEntraEnCom)
                    {
                        pTipo = TiposElementosEnCola.Eliminado;
                    }

                    if (!string.IsNullOrEmpty(todosTags))
                    {
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(mProyectoID);

                        BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org = basePerOrgComunidadDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
                        MeterValoresEnFilaCola(filaColaTagsCom_Per_Org, todosTags, id, pTipo, pPrioridadBase);
                        basePerOrgComunidadDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org);
                    }
                }
                else if (filaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo || filaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || filaIdentidad.Tipo == (short)TiposIdentidad.Organizacion)
                {
                    TiposElementosEnCola pTipo = TiposElementosEnCola.Agregado;

                    if (!mEntraEnCom)
                    {
                        pTipo = TiposElementosEnCola.Eliminado;
                    }
                    //Es una organización que ha creado una comunidad.
                    Guid organizacionID = filaPerfil.OrganizacionID.Value;
                    listaTags.Add(Constantes.PERS_U_ORG + "o" + Constantes.PERS_U_ORG);
                    listaTags.Add(Constantes.ID_TAG_PER + organizacionID + Constantes.ID_TAG_PER);
                    if (!string.IsNullOrEmpty(todosTags))
                    {
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(mProyectoID);

                        if (id == -1) //Estamos creando un proyecto nuevo y todavía no está en BD.
                        {
                            id = mTablaBaseProyectoID;
                        }

                        int idMyGnoss = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(ProyectoAD.MetaProyecto);

                        BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org = basePerOrgComunidadDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
                        MeterValoresEnFilaCola(filaColaTagsCom_Per_Org, todosTags, id, pTipo, pPrioridadBase);
                        basePerOrgComunidadDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org);
                    }
                }
                else if (filaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo || filaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || filaIdentidad.Tipo == (short)TiposIdentidad.Organizacion)
                {
                    //Es una organización que ha creado una comunidad.
                    Guid organizacionID = filaPerfil.OrganizacionID.Value;
                    listaTags.Add(Constantes.PERS_U_ORG + "o" + Constantes.PERS_U_ORG);
                    listaTags.Add(Constantes.ID_TAG_PER + organizacionID + Constantes.ID_TAG_PER);

                    todosTags = UtilCadenas.ComponerTextoSepComasDeLista(listaTags);

                    TiposElementosEnCola pTipo = TiposElementosEnCola.Agregado;

                    if (!mEntraEnCom)
                    {
                        pTipo = TiposElementosEnCola.Eliminado;
                    }

                    if (!string.IsNullOrEmpty(todosTags))
                    {
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(mProyectoID);

                        if (id == -1) //Estamos creando un proyecto nuevo y todavía no está en BD.
                        {
                            id = mTablaBaseProyectoID;
                        }

                        int idMyGnoss = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(ProyectoAD.MetaProyecto);

                        BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org = basePerOrgComunidadDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
                        MeterValoresEnFilaCola(filaColaTagsCom_Per_Org, todosTags, id, pTipo, pPrioridadBase);
                        basePerOrgComunidadDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org);
                    }
                }

                basePerOrgComunidadCN.InsertarFilasEnRabbit("ColaTagsCom_Per_Org", basePerOrgComunidadDS);
                #endregion
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }
        }

        /// <summary>
        /// Carga un DataSet con el modelo base de forma masiva (no guarda en BD). Pasar cargado el gestor de personas de las identidades.
        /// </summary>
        /// <param name="pListaIdentidades">Identidades que se ha metido o salido de una comunidad</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="pEntraEnCom">Verdad si ha entrado en la comunidad, falso si ha salido de ella</param>
        /// <param name="pActualizarTagsPersonas">Verdad si se deben actualizar los tags de las personas</param>
        /// <param name="pActualizarModeloAcido">Verdad si se debe actualizar también el modelo Ácido</param>
        /// <param name="pPrioridadBase">Prioridad para el Base</param>
        private void ActualizarModeloBASEAsincronamenteMasivo(List<Identidad> pListaIdentidades, Guid pProyectoID, int pTablaBaseProyectoID, bool pEntraEnCom, bool pActualizarModeloAcido, bool pActualizarTagsPersonaSincronamente, PrioridadBase pPrioridadBase)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);

            BasePerOrgComunidadDS basePerOrgComunidadDS = new BasePerOrgComunidadDS();

            foreach (Identidad identidad in pListaIdentidades)
            {
                if (identidad.FilaIdentidad.CurriculumID.HasValue)
                {
                    mCvID = identidad.FilaIdentidad.CurriculumID.Value;
                }

                try
                {
                    AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = identidad.FilaIdentidad;
                    AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = identidad.PerfilUsuario.FilaPerfil;
                    List<string> listaTags = new List<string>();
                    string todosTags = "";

                    if (filaIdentidad.Tipo == (short)TiposIdentidad.Personal || filaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || filaIdentidad.Tipo == (short)TiposIdentidad.Profesor || !pEntraEnCom && filaPerfil.PersonaID.HasValue)
                    {
                        Guid personaID = filaPerfil.PersonaID.Value;
                        listaTags.Add(Constantes.PERS_U_ORG + "p" + Constantes.PERS_U_ORG);
                        listaTags.Add(Constantes.ID_TAG_PER + personaID + Constantes.ID_TAG_PER);
                    }
                    else if (filaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalCorporativo || filaIdentidad.Tipo == (short)TiposIdentidad.ProfesionalPersonal || filaIdentidad.Tipo == (short)TiposIdentidad.Organizacion && filaPerfil.OrganizacionID.HasValue)
                    {
                        //Es una organización que ha creado una comunidad.
                        Guid organizacionID = filaPerfil.OrganizacionID.Value;
                        listaTags.Add(Constantes.PERS_U_ORG + "o" + Constantes.PERS_U_ORG);
                        listaTags.Add(Constantes.ID_TAG_PER + organizacionID + Constantes.ID_TAG_PER);
                    }

                    //si no hay tags no hay añadir la fila
                    if (listaTags.Count > 0)
                    {
                        todosTags = UtilCadenas.ComponerTextoSepComasDeLista(listaTags);

                        TiposElementosEnCola pTipo = TiposElementosEnCola.Agregado;

                        if (!pEntraEnCom)
                        {
                            pTipo = TiposElementosEnCola.Eliminado;
                        }

                        if (!string.IsNullOrEmpty(todosTags))
                        {
                            if (id == -1) //Estamos creando un proyecto nuevo y todavía no está en BD.
                            {
                                id = pTablaBaseProyectoID;
                            }

                            BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org = basePerOrgComunidadDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
                            MeterValoresEnFilaCola(filaColaTagsCom_Per_Org, todosTags, id, pTipo, pPrioridadBase);
                            basePerOrgComunidadDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org);
                        }
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                }
            }

            //si funcionan las transacciones de Juan, meter esta parte dentro de ActualizarModeloBASEMasivo
            BaseComunidadCN basePerOrgComunidadCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            basePerOrgComunidadCN.InsertarFilasEnRabbit("ColaTagsCom_Per_Org", basePerOrgComunidadDS);
        }

        private void MeterValoresEnFilaCola(DataRow pFilaCola, string pTags, int pIdProyecto, TiposElementosEnCola pTipoRelacionTags, PrioridadBase pPrioridadBase)
        {
            pFilaCola["TablaBaseProyectoID"] = pIdProyecto;
            pFilaCola["Tags"] = pTags;
            pFilaCola["Tipo"] = (short)pTipoRelacionTags;
            pFilaCola["Estado"] = 0;
            pFilaCola["FechaPuestaEnCola"] = DateTime.Now;
            pFilaCola["Prioridad"] = (short)pPrioridadBase;
        }

        public void ActivoEnComunidad(Identidad pIdentidad)
        {
            bool actualizarIdentidad = false;

            if (!pIdentidad.FilaIdentidad.ActivoEnComunidad)
            {
                pIdentidad.FilaIdentidad.ActivoEnComunidad = true;
                actualizarIdentidad = true;
            }

            if (!pIdentidad.FilaIdentidad.ActualizaHome)
            {
                pIdentidad.FilaIdentidad.ActualizaHome = true;
                actualizarIdentidad = true;
            }

            if (actualizarIdentidad)
            {
                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                idenCN.ActualizaIdentidades();
                idenCN.Dispose();

                if (pIdentidad.FilaIdentidad.ProyectoID == ProyectoAD.ProyectoDidactalia && pIdentidad.Persona != null)
                {
                    //ActualizarModeloBaseSimple(pIdentidad.Persona.Clave, pIdentidad.FilaIdentidad.ProyectoID, PrioridadBase.Alta);
                    ActualizarModeloBaseSimple(pIdentidad, pIdentidad.FilaIdentidad.ProyectoID, UrlIntragnoss);

                    new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).ActualizarGnossLive(ProyectoSeleccionado.Clave, pIdentidad.FilaIdentidad.PerfilID, AccionLive.Agregado, (int)TipoLive.Miembro, false, PrioridadLive.Alta);

                    IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    identidadCL.EliminarCacheGestorIdentidad(pIdentidad.Clave, pIdentidad.PersonaID.Value);
                    identidadCL.Dispose();
                }
            }
        }

        #endregion

    }
}
