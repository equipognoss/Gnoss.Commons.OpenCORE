using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.ServiciosGenerales
{
    public class ControladorOrganizaciones
    {

        #region Miembros

        private static Dictionary<Guid, bool> mListaOrganizacionesSonClases = new Dictionary<Guid, bool>();

        private Identidad mIdentidad;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private EntityContextBASE mEntityContextBASE;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        public ControladorOrganizaciones(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorOrganizaciones> logger, ILoggerFactory loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Metodos generales

        /// <summary>
        /// Obtiene el nombre del país de una organizacion
        /// </summary>
        /// <param name="pPersona">Organizacion de la que se quiere obtener el país</param>
        /// <returns></returns>
        public string ObtenerNombrePaisOrganizacion(Organizacion pOrganizacion)
        {
            string nombrePais = "";

            if (pOrganizacion.FilaOrganizacion.PaisID.HasValue)
            {
                DataWrapperPais paisDW = pOrganizacion.GestorOrganizaciones.PaisDW;

                if (paisDW == null)
                {
                    PaisCN paisCN = new PaisCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PaisCN>(), mLoggerFactory);
                    paisDW = paisCN.ObtenerPaisesProvincias();
                    paisCN.Dispose();

                    pOrganizacion.GestorOrganizaciones.PaisDW = paisDW;
                }

                if (paisDW != null)
                {
                    AD.EntityModel.Models.Pais.Pais filasPais = paisDW.ListaPais.Where(item => item.PaisID.Equals(pOrganizacion.FilaOrganizacion.PaisID)).FirstOrDefault();

                    if ((filasPais != null))
                    {
                        nombrePais = filasPais.Nombre;
                    }
                }
            }

            return nombrePais;
        }

        /// <summary>
        /// Obtiene el nombre de la provincia de una organizacion
        /// </summary>
        /// <param name="pOrganizacion">Organizacion de la que se quiere obtener la provincia</param>
        /// <returns></returns>
        public string ObtenerNombreProvinciaOrganizacion(Organizacion pOrganizacion)
        {
            string nombreProvincia = "";

            if ((!pOrganizacion.FilaOrganizacion.ProvinciaID.HasValue) && (pOrganizacion.FilaOrganizacion.Provincia != null))
            {
                nombreProvincia = pOrganizacion.FilaOrganizacion.Provincia;
            }
            else if ((pOrganizacion.FilaOrganizacion.ProvinciaID.HasValue) && (pOrganizacion.FilaOrganizacion.PaisID.HasValue))
            {
                DataWrapperPais paisDW = pOrganizacion.GestorOrganizaciones.PaisDW;

                if (paisDW == null)
                {
                    PaisCN paisCN = new PaisCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PaisCN>(), mLoggerFactory);
                    paisDW = paisCN.ObtenerProvinciasDePais(pOrganizacion.FilaOrganizacion.PaisID.Value);
                    paisCN.Dispose();

                    pOrganizacion.GestorOrganizaciones.PaisDW = paisDW;
                }

                if (paisDW != null)
                {
                    AD.EntityModel.Models.Pais.Provincia filasProvincia = paisDW.ListaProvincia.Where(item => item.ProvinciaID.Equals(pOrganizacion.FilaOrganizacion.ProvinciaID)).FirstOrDefault();

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
        /// <param name="pActualizarTagsOrganizacionSincronamente">Verdad si se deben actualizar los tags de las personas</param>
        /// <param name="pTagsViejos">Tags viejos antes del cambio</param>
        public void ActualizarModeloBASE(Identidad pIdentidad, Guid pProyectoID, bool pEntraEnCom, bool pActualizarTagsOrganizacionSincronamente,PrioridadBase pPrioridadBase, IAvailableServices pAvailableServices)
        {
            mIdentidad = pIdentidad;

            ActualizarModeloBaseSimple(pIdentidad.PerfilUsuario.FilaPerfil.OrganizacionID.Value, pProyectoID, pPrioridadBase, pAvailableServices);
        }

        private void ActualizarTagsOrganizacion()
        {
            Identidad identidad = mIdentidad;

            GestionIdentidades gestorIdentidades = identidad.GestorIdentidades;

            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);

            bool estaOrganizacionCargada = (gestorIdentidades.GestorOrganizaciones != null) && (gestorIdentidades.GestorOrganizaciones.ListaOrganizaciones.ContainsKey(identidad.OrganizacionID.Value));

            if ((!estaOrganizacionCargada) || !(identidad.OrganizacionPerfil.FilaOrganizacion.PaisID.HasValue))
            {
                if (!estaOrganizacionCargada)
                {
                    gestorIdentidades.GestorOrganizaciones = new GestionOrganizaciones(orgCN.ObtenerOrganizacionPorID(identidad.OrganizacionID.Value), mLoggingService, mEntityContext);
                }
                else
                {
                    DataWrapperOrganizacion orgDW = orgCN.ObtenerOrganizacionPorID(identidad.OrganizacionID.Value);
                    AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrgCargada = orgDW.ListaOrganizacion.FirstOrDefault(item => item.OrganizacionID.Equals(identidad.OrganizacionID.Value));
                    gestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaOrganizacion.Remove(identidad.OrganizacionPerfil.FilaOrganizacion);
                    gestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaOrganizacion.Add(filaOrgCargada);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void ActualizarModeloBaseSimple(Guid pOrganizacionID, Guid pProyectoID,PrioridadBase pPrioridadBase, IAvailableServices pAvailableServices)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);

            BaseComunidadCN basePerOrgComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<BaseComunidadCN>(), mLoggerFactory);
            BasePerOrgComunidadDS basePerOrgComDS = new BasePerOrgComunidadDS();

            string todosTags = Constantes.PERS_U_ORG + "o" + Constantes.PERS_U_ORG;
            todosTags += ", " + Constantes.ID_TAG_PER + pOrganizacionID + Constantes.ID_TAG_PER;

            BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org_Add = basePerOrgComDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
            MeterValoresEnFilaCola(filaColaTagsCom_Per_Org_Add, todosTags, id, TiposElementosEnCola.Agregado,pPrioridadBase);
            basePerOrgComDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org_Add);

            basePerOrgComCN.InsertarFilasEnColaTagsCom_Per_Org(basePerOrgComDS, pAvailableServices);
        }

        public void ActualizarModeloBaseSimpleMultiple(Guid pOrganizacionID, List<Guid> pListaProyectosID, IAvailableServices pAvailableServices)
        {
            ActualizarModeloBaseSimpleMultiple(pOrganizacionID, pListaProyectosID, true, pAvailableServices);
        }

        public void ActualizarModeloBaseSimpleMultiple(Guid pOrganizacionID, List<Guid> pListaProyectosID, bool pAgregado, IAvailableServices pAvailableServices)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Dictionary<Guid, int> diccionarioProyectoBaseProyectoID = proyCN.ObtenerTablasBaseProyectoIDProyectoPorID(pListaProyectosID);
            proyCN.Dispose();

            BaseComunidadCN basePerOrgComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<BaseComunidadCN>(), mLoggerFactory);
            BasePerOrgComunidadDS basePerOrgComDS = new BasePerOrgComunidadDS();

            foreach (Guid proyectoID in diccionarioProyectoBaseProyectoID.Keys)
            {
                string todosTags = Constantes.PERS_U_ORG + "o" + Constantes.PERS_U_ORG;
                todosTags += ", " + Constantes.ID_TAG_PER + pOrganizacionID + Constantes.ID_TAG_PER;

                BasePerOrgComunidadDS.ColaTagsCom_Per_OrgRow filaColaTagsCom_Per_Org_Add = basePerOrgComDS.ColaTagsCom_Per_Org.NewColaTagsCom_Per_OrgRow();
                TiposElementosEnCola tipoElemento = TiposElementosEnCola.Agregado;
                if (!pAgregado)
                {
                    tipoElemento = TiposElementosEnCola.Eliminado;
                }
                MeterValoresEnFilaCola(filaColaTagsCom_Per_Org_Add, todosTags, diccionarioProyectoBaseProyectoID[proyectoID], tipoElemento, PrioridadBase.Alta);
                basePerOrgComDS.ColaTagsCom_Per_Org.AddColaTagsCom_Per_OrgRow(filaColaTagsCom_Per_Org_Add);
            }

            basePerOrgComCN.InsertarFilasEnColaTagsCom_Per_Org(basePerOrgComDS, pAvailableServices);
        }

        /// <summary>
        /// Obtiene los tags para el modelo base de una organización.
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns>Cadena con los tags para el modelo base de una organización</returns>
        public string ObtenerTagsOrganizacionModeloBase(Identidad pIdentidad)
        {
            mIdentidad = pIdentidad;
            ActualizarTagsOrganizacion();

            Guid? curriculumID = null;

            if (pIdentidad.FilaIdentidad.CurriculumID.HasValue)
            {
                curriculumID = pIdentidad.FilaIdentidad.CurriculumID;
            }

            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = pIdentidad.FilaIdentidad;


            //"ColaTagsCom_Per_Org"
            AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = pIdentidad.PerfilUsuario.FilaPerfil;
            List<string> listaTags = new List<string>();

            Guid organizacionID = filaPerfil.OrganizacionID.Value;

            listaTags.Add(Constantes.PERS_U_ORG + "o" + Constantes.PERS_U_ORG);

            listaTags.Add(Constantes.ID_TAG_PER + organizacionID + Constantes.ID_TAG_PER);

            return UtilCadenas.ComponerTextoSepComasDeLista(listaTags);
        }

        private void MeterValoresEnFilaCola(DataRow pFilaCola, string pTags, int pIdProyecto, TiposElementosEnCola pTipoRelacionTags,PrioridadBase pPrioridadBase)
        {
            pFilaCola["TablaBaseProyectoID"] = pIdProyecto;
            pFilaCola["Tags"] = pTags;
            pFilaCola["Tipo"] = (short)pTipoRelacionTags;
            pFilaCola["Estado"] = 0;
            pFilaCola["FechaPuestaEnCola"] = DateTime.Now;
            pFilaCola["Prioridad"] = (short)pPrioridadBase;
        }

        /// <summary>
        /// Comprueba si una organización es clase
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns></returns>
        public bool ComprobarOrganizacionEsClase(Guid pOrganizacionID)
        {
            if (mListaOrganizacionesSonClases.ContainsKey(pOrganizacionID))
            {
                return mListaOrganizacionesSonClases[pOrganizacionID];
            }
            else
            {
                bool esClase = new OrganizacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCL>(), mLoggerFactory).ComprobarOrganizacionEsClase(pOrganizacionID);
                
                //Agregamos al diccionario la organización y si es clase o no.
                mListaOrganizacionesSonClases.Add(pOrganizacionID, esClase);

                return esClase;
            }
        }

        /// <summary>
        /// Devuelve el HTML del inicio del menú pulgarcito.
        /// </summary>
        /// <param name="pBaseUrlIdioma">Url base</param>
        /// <param name="pUrlPerfil">Url del perfil conectado</param>
        /// <param name="pUtilIdiomas">UtilIdiomas</param>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <returns>HTML del inicio del menú pulgarcito</returns>
        public static string ObtenerHtmlInicioMenuPulgarcitoMisClases(string pBaseUrlIdioma, string pUrlPerfil, UtilIdiomas pUtilIdiomas, Identidad pIdentidadActual)
        {
            return "<a href=\"" + pBaseUrlIdioma + pUrlPerfil + "home" + "\">" + pUtilIdiomas.GetText("COMMON", "INICIO") + "</a> &gt; <a href=\"" + pBaseUrlIdioma + "/" + pUtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + pIdentidadActual.IdentidadProfesorMyGnoss.PerfilUsuario.NombreCortoUsu + "/" + pUtilIdiomas.GetText("URLSEM", "VERCLASESADMINISTRO") + "\">" + pUtilIdiomas.GetText("ADMINISTRARCLASES", "MISCLASES") + "</a> &gt; ";
        }

        #endregion

    }
}
