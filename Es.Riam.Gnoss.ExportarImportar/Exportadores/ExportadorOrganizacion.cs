using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

namespace Es.Riam.Gnoss.ExportarImportar.Exportadores
{
    /// <summary>
    /// Exportador de organización
    /// </summary>
    public class ExportadorOrganizacion : ExportadorElementoGnoss, IDisposable
    {

        #region Miembros 

        /// <summary>
        /// Verdad si se va a exportar el personal de las organizaciones
        /// </summary>
        private static bool mExportarVinculosConPersonas = false;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private UtilSemCms mUtilSemCms;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructor

        /// <summary>
        /// Crea un nuevo exportador de organización a partir de la ontología pasada por parámetro
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        public ExportadorOrganizacion(Ontologia pOntologia, string pIdiomaUsuario, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, UtilSemCms utilSemCms, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, VirtuosoAD virtuosoAd, ILogger<ExportadorOrganizacion> logger, ILoggerFactory loggerFactory)
            : base(pOntologia, pIdiomaUsuario, loggingService, entityContext, configService, redisCacheWrapper, utilSemCms, servicesUtilVirtuosoAndReplication, virtuosoAd, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mRedisCacheWrapper = redisCacheWrapper;
            mUtilSemCms = utilSemCms;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene la entidad y todas sus relaciones
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener</param>
        /// <param name="pElementoGnoss">Elemento gnoss que representa la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public override void ObtenerEntidad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, bool pEspecializacion, GestionGnoss pGestor)
        {
            if (pElementoGnoss is Organizacion)
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((Organizacion)pElementoGnoss).FilaOrganizacion, pEspecializacion, pGestor);
            else if (pElementoGnoss is DatosTrabajoPersonaOrganizacion)
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((DatosTrabajoPersonaOrganizacion)pElementoGnoss).FilaVinculo, pEspecializacion, pGestor);
            else
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((ElementoGnoss)pElementoGnoss).FilaElemento, pEspecializacion, pGestor);
        }

        /// <summary>
        /// Obtiene los atributos de un elemento de estructura
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la que se buscan sus atributos</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        public override void ObtenerAtributosEntidad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidadBuscada, IElementoGnoss pElementoGnoss)
        {
            switch (pEntidadBuscada.TipoEntidad)
            {
                case TipoElementoGnoss.Organizacion:
                    UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Organizacion)pElementoGnoss).FilaOrganizacion);
                    break;
                case TipoElementoGnoss.PersonaVinculoOrganizacion:
                    UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((DatosTrabajoPersonaOrganizacion)pElementoGnoss).FilaVinculo);
                    break;
                default:
                    break;
            }
            base.ObtenerAtributosEntidad(pEntidadBuscada, pElementoGnoss);

            if (pElementoGnoss is GestionOrganizaciones)
            {
                pEntidadBuscada.Descripcion = "Todas las organizaciones";
            }
        }

        /// <summary>
        /// Generaliza un elemento de estructura
        /// </summary>
        /// <param name="pEntidad">Entidad a generalizar</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pFilaElemento">Fila del elemento que representa la entidad</param>
        /// <param name="pGestor">Gestor de estructura</param>
        protected override void GeneralizarEntidad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, object pFilaElemento, GestionGnoss pGestor)
        {
        }

        /// <summary>
        /// Trata los casos especiales para estructura.
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la propiedad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pPropiedad">Propiedad que relaciona la entidad con otra entidad.</param>
        /// <param name="pGestor">Gestor de estructura.</param>
        /// <returns></returns>
        protected override bool TratarCasoEspecial(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            bool resultado = false;

            switch (pPropiedad.Nombre)
            {
                case UtilImportarExportar.PROPIEDAD_PERSONA_VINCULO_ORGANIZACION:
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_ORGANIZACION_DE_PERSONA:
                    if (ExportadorPersona.ExportarVinculosConOrganizaciones)
                    {
                        ObtenerOrganizacionDePersona(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    }
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_PERSONAL_ORGANIZACION:
                    if (ExportarVinculosConPersonas)
                    {
                        ObtenerPersonalDeOrganizacion(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    }
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_RESULTADOS:
                    pPropiedad.NombreReal = "Organizaciones";
                    foreach (Organizacion org in pGestor.Hijos)
                    {
                        ElementoOntologia organizacion = new ElementoOntologia(Ontologia.GetEntidadTipo(TipoElementoGnoss.Organizacion));
                        UtilImportarExportar.ObtenerID(organizacion, org.FilaOrganizacion, org);

                        AgregarYObtenerEntidadRelacionada(pEntidad, pPropiedad, organizacion, org, org.FilaOrganizacion, false, pGestor);
                    }
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_FILTROS:
                    resultado = true;
                    break;
                default:
                    resultado = false;
                    break;
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene los datos de trabajo de las vinculaciones de persona con organización de la persona pasada por parámetro
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la propiedad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad que relaciona la entidad con controles</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <param name="pOrganizacionID">Identificador de la organización vinculada</param>
        public void ObtenerVinculoPersonaOrganizacion(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor, Guid pOrganizacionID)
        {
            if (((Persona)pElementoGnoss).ListaDatosTrabajoPersonaOrganizacion.ContainsKey(pOrganizacionID))
            {
                DatosTrabajoPersonaOrganizacion elementoDatosTrabajoPersonaOrg = ((Persona)pElementoGnoss).ListaDatosTrabajoPersonaOrganizacion[pOrganizacionID];

                Es.Riam.Semantica.OWL.ElementoOntologia datosTrabajoPersonaOrganizacion = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.PersonaVinculoOrganizacion));
                datosTrabajoPersonaOrganizacion.ID = Guid.NewGuid().ToString();
                Es.Riam.Semantica.OWL.ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(datosTrabajoPersonaOrganizacion.ID);

                if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
                {
                    if (entidadAuxiliar != null)
                        datosTrabajoPersonaOrganizacion = entidadAuxiliar;

                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, datosTrabajoPersonaOrganizacion);
                    ObtenerEntidad(datosTrabajoPersonaOrganizacion, elementoDatosTrabajoPersonaOrg, false, pGestor);
                }
                else
                {
                    //Asigno la entidad ya creada
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadAuxiliar);
                }
            }
        }

        /// <summary>
        /// Obtiene los datos de la organización vinculada a la persona
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la propiedad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad que relaciona la entidad con controles</param>
        /// <param name="pGestor">Gestor de entidades</param>
        private void ObtenerPersonalDeOrganizacion(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            Organizacion org = (Organizacion)pElementoGnoss;
            ExportadorPersona exportEstr = new ExportadorPersona(Ontologia, IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd, mloggerFactory.CreateLogger<ExportadorPersona>(), mloggerFactory);

            foreach (Persona elemPersona in org.ListaPersonasVinculadasConLaOrganizacion.Values)
            {
                Es.Riam.Semantica.OWL.ElementoOntologia persona = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.Persona));
                persona.ID = Guid.NewGuid().ToString();
                Es.Riam.Semantica.OWL.ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(persona.ID);

                if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
                {
                    if (entidadAuxiliar != null)
                        persona = entidadAuxiliar;

                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, persona);
                    exportEstr.ObtenerEntidad(persona, elemPersona, false, pGestor);
                    persona.Descripcion = elemPersona.NombreConApellidos;
                    //exportEstr.ObtenerDatosTrabajoPersona(persona, elemPersona, org.Clave, org.GestorOrganizaciones.GestorPersonas);
                }
                else
                {
                    //Asigno la entidad ya creada
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadAuxiliar);
                }
            }
        }

        /// <summary>
        /// Obtiene los datos de la organización vinculada a la persona
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la propiedad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad que relaciona la entidad con controles</param>
        /// <param name="pGestor">Gestor de entidades</param>
        private void ObtenerOrganizacionDePersona(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            Es.Riam.Semantica.OWL.ElementoOntologia organizacion = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.Organizacion));
            organizacion.ID = Guid.NewGuid().ToString();
            Es.Riam.Semantica.OWL.ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(organizacion.ID);

            if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
            {
                if (entidadAuxiliar != null)
                    organizacion = entidadAuxiliar;

                //Obtengo la entidad
                AgregarEntidadRelacionada(pEntidad, pPropiedad, organizacion);
                ObtenerEntidad(organizacion, ((DatosTrabajoPersonaOrganizacion)pElementoGnoss).Organizacion, false, pGestor);
            }
            else
            {
                //Asigno la entidad ya creada
                AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadAuxiliar);
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece si se va a exportar el personal de las organizaciones
        /// </summary>
        public static bool ExportarVinculosConPersonas
        {
            get
            {
                return mExportarVinculosConPersonas;
            }
            set
            {
                mExportarVinculosConPersonas = value;

                if (value && ExportadorPersona.ExportarVinculosConOrganizaciones)
                {
                    ExportadorPersona.ExportarVinculosConOrganizaciones = false;
                }
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~ExportadorOrganizacion()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion
    }
}
