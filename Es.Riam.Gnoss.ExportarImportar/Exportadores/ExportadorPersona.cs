using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.ExportarImportar.Exportadores
{
    /// <summary>
    /// Exportador de persona
    /// </summary>
    public class ExportadorPersona : ExportadorElementoGnoss, IDisposable
    {

        #region Miembros

        /// <summary>
        /// Verdad si se va a exportar el vínculo de las persona con las organizaciones
        /// </summary>
        private static bool mExportarVinculosConOrganizaciones = true;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;

        #endregion

        #region Constructor

        /// <summary>
        /// Crea un nuevo exportador de persona a partir de la ontología pasada por parámetro
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        public ExportadorPersona(Ontologia pOntologia, string pIdiomaUsuario, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, UtilSemCms utilSemCms, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pOntologia, pIdiomaUsuario, loggingService, entityContext, configService, redisCacheWrapper, utilSemCms, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
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
        public override void ObtenerEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, bool pEspecializacion, GestionGnoss pGestor)
        {
            if (pElementoGnoss is Elementos.ServiciosGenerales.Persona)
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((Elementos.ServiciosGenerales.Persona)pElementoGnoss).FilaPersona, pEspecializacion, pGestor);
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
                case TipoElementoGnoss.Persona:
                    UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Elementos.ServiciosGenerales.Persona)pElementoGnoss).FilaPersona);
                    break;
                default:
                    break;
            }
            base.ObtenerAtributosEntidad(pEntidadBuscada, pElementoGnoss);
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
                case UtilImportarExportar.PROPIEDAD_DATOS_TRABAJO_PERSONA_LIBRE:
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_PERSONA_VINCULO_ORGANIZACION:
                    resultado = true;
                    break;
                default:
                    resultado = false;
                    break;
            }
            return resultado;
        }

        /// <summary>
        /// Completa una lista de entidades
        /// </summary>
        /// <param name="pListaACompletar">Lista de entidades a completar</param>
        /// <param name="pListaClaves">Lista de claves de procesos</param>
        /// <param name="pGestor">Gestor de procesos</param>
        protected override void CompletarEntidades(List<IElementoGnoss> pListaACompletar, Dictionary<Guid, AD.EntityModel.Models.PersonaDS.Persona> pListaClaves, GestionGnoss pGestor)
        {
            SortedList<Guid, AD.EntityModel.Models.PersonaDS.Persona> lista = new SortedList<Guid, AD.EntityModel.Models.PersonaDS.Persona>();

            foreach (Elementos.ServiciosGenerales.Persona persona in pListaACompletar)
            {
                if (!persona.FotoCargada)
                {
                    lista.Add(persona.Clave, persona.FilaPersona);
                    persona.FotoCargada = true;
                }
            }
            if (lista.Count > 0)
            {
                //PersonaCN personaCN = new PersonaCN();
                //personaCN.CargarDatosPesados(lista, true);

                //((Persona)pListaACompletar[0]).GestorPersonas.PersonaDS.Merge(personaCN.CargarDatosPersonasLibres(lista), true);
                //personaCN.Dispose();
            }


            List<Guid> listaPersonasOrg = new List<Guid>();

            foreach (Elementos.ServiciosGenerales.Persona persona in pListaACompletar)
            {
                listaPersonasOrg.Add(persona.Clave);
            }
            if (lista.Count > 0)
            {
                OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                ((Elementos.ServiciosGenerales.Persona)pListaACompletar[0]).GestorPersonas.GestorOrganizaciones.OrganizacionDW.Merge(organizacionCN.ObtenerOrganizacionesDeListaPersona(listaPersonasOrg));
                organizacionCN.Dispose();
                ((Elementos.ServiciosGenerales.Persona)pListaACompletar[0]).GestorPersonas.GestorOrganizaciones.CargarOrganizaciones();
            }
        }

        /// <summary>
        /// Comprueba si es necesario completar una entidad
        /// </summary>
        /// <param name="pEntidad">Entidad a completar</param>
        /// <param name="pListaACompletar">Lista de entidades a completar</param>
        /// <param name="pListaElementos">Lista de elementos</param>
        protected override void ComprobarEntidadCompleta(IElementoGnoss pEntidad, Dictionary<Guid, AD.EntityModel.Models.PersonaDS.Persona> pListaACompletar, List<IElementoGnoss> pListaElementos)
        {
            if (pEntidad is Elementos.ServiciosGenerales.Persona)
            {
                Elementos.ServiciosGenerales.Persona persona = (Elementos.ServiciosGenerales.Persona)pEntidad;

                if (!pListaACompletar.ContainsKey(persona.Clave))
                {
                    pListaACompletar.Add(persona.Clave, persona.FilaPersona);
                    pListaElementos.Add(persona);
                }
            }
        }

        #endregion

        /// <summary>
        /// Obtiene o establece si se va a exportar el vínculo de las persona con las organizaciones
        /// </summary>
        public static bool ExportarVinculosConOrganizaciones
        {
            get
            {
                return mExportarVinculosConOrganizaciones;
            }
            set
            {
                mExportarVinculosConOrganizaciones = value;

                if (value && ExportadorOrganizacion.ExportarVinculosConPersonas)
                {
                    ExportadorOrganizacion.ExportarVinculosConPersonas = false;
                }
            }
        }

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~ExportadorPersona()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion
    }
}
