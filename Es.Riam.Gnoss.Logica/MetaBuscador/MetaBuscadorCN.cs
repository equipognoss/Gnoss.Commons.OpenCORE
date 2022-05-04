using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.Comentario;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.MetaBuscadorAD;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Comentario;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.MetaBuscador
{
    /// <summary>
    /// Lógica del metabuscador
    /// </summary>
    public class MetaBuscadorCN : IDisposable
    {
        #region Miembros

        private Dictionary<string, TiposResultadosMetaBuscador> mListaOrdenadaElementos;

        private Dictionary<string, int> mListaContadorTags;

        private DataWrapperDocumentacion mDataWrapperDocumentacion;

        private OrganizacionAD mOrganizacionAD;

        private ProyectoAD mProyectoAD;

        private ComentarioAD mComentarioAD;

        private DocumentacionAD mDocumentacionAD;

        private PersonaAD mPersonaAD;

        private IdentidadAD mIdentidadAD;

        /// <summary>
        /// AD de correo.
        /// </summary>
        private CorreoAD mCorreoAD;

        /// <summary>
        /// AD de Correo.
        /// </summary>
        private NotificacionAD mNotificacionAD;

        private AmigosAD mAmigosAD;

        private DataWrapperOrganizacion mOrganizacionDW = null;

        private DataWrapperProyecto mDataWrapperProyecto = null;

        private DataWrapperComentario mComentarioDW = null;

        private DataWrapperPersona mDataWrapperPersona = null;

        private DataWrapperIdentidad mDataWrapperIdentidad = null;

        /// <summary>
        /// DataSet de notifiaciones.
        /// </summary>
        private DataWrapperNotificacion mNotificacionDW;

        private int mNumResultados;

        /// <summary>
        /// Obtiene la identidad del buscador.
        /// </summary>
        private Guid mIdentidadBusquedaID;

        /// <summary>
        /// Lista de filtros del facetado.
        /// </summary>
        private Dictionary<string, List<string>> mListaFiltros;

        /// <summary>
        /// ID del proyecto de origen para la búsqueda.
        /// </summary>
        private Guid mProyectoOrigenID;

        /// <summary>
        /// Id de la organización. 
        /// </summary>
        private Guid mOrganizacionID = Guid.Empty;

        /// <summary>
        /// Id del usuario.
        /// </summary>
        private Guid mUsuarioID = Guid.Empty;

        private ConfigService mConfigService;

        private EntityContext mEntityContext;

        private LoggingService mLoggingService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public MetaBuscadorCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication
)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la lista de elementos obtenidos ordenada por orden de relevancia
        /// </summary>
        public Dictionary<string, TiposResultadosMetaBuscador> ListaOrdenadaElementos
        {
            get
            {
                return mListaOrdenadaElementos;
            }
            set
            {
                mListaOrdenadaElementos = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de elementos obtenidos ordenada por orden de relevancia
        /// </summary>
        public Dictionary<string, int> ListaContadorTags
        {
            get
            {
                return mListaContadorTags;
            }
        }

        /// <summary>
        /// Obtiene el AD de organización
        /// </summary>
        public OrganizacionAD OrganizacionAD
        {
            get
            {
                if (mOrganizacionAD == null)
                    mOrganizacionAD = new OrganizacionAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                return mOrganizacionAD;
            }
        }

        /// <summary>
        /// Obtiene el AD de comentarios
        /// </summary>
        public ComentarioAD ComentarioAD
        {
            get
            {
                if (mComentarioAD == null)
                    mComentarioAD = new ComentarioAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                return mComentarioAD;
            }
        }

        /// <summary>
        /// Obtiene el AD de proyectos
        /// </summary>
        public ProyectoAD ProyectoAD
        {
            get
            {
                if (mProyectoAD == null)
                    mProyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                return mProyectoAD;
            }
        }

        /// <summary>
        /// Obtiene el AD de documentación
        /// </summary>
        public DocumentacionAD DocumentacionAD
        {
            get
            {
                if (mDocumentacionAD == null)
                    mDocumentacionAD = new DocumentacionAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                if (mDataWrapperDocumentacion == null)
                {
                    mDataWrapperDocumentacion = new DataWrapperDocumentacion();
                }
                return mDocumentacionAD;
            }
        }

        /// <summary>
        /// Obtiene el AD de personas
        /// </summary>
        public PersonaAD PersonaAD
        {
            get
            {
                if (mPersonaAD == null)
                    mPersonaAD = new PersonaAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                return mPersonaAD;
            }
        }

        /// <summary>
        /// Obtiene el AD de identidad
        /// </summary>
        public IdentidadAD IdentidadAD
        {
            get
            {
                if (mIdentidadAD == null)
                    mIdentidadAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                return mIdentidadAD;
            }
        }

        /// <summary>
        /// Obtiene el AD de Correo.
        /// </summary>
        public CorreoAD CorreoAD
        {
            get
            {
                if (mCorreoAD == null)
                    mCorreoAD = new CorreoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                return mCorreoAD;
            }
        }

        /// <summary>
        /// Obtiene el AD de Correo.
        /// </summary>
        public NotificacionAD NotificacionAD
        {
            get
            {
                if (mNotificacionAD == null)
                    mNotificacionAD = new NotificacionAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                return mNotificacionAD;
            }
        }

        /// <summary>
        /// Obtiene el AD de amigos
        /// </summary>
        public AmigosAD AmigosAD
        {
            get
            {
                if (mAmigosAD == null)
                    mAmigosAD = new AmigosAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                return mAmigosAD;
            }
        }

        /// <summary>
        /// Obtiene el DS de organización
        /// </summary>
        public DataWrapperOrganizacion OrganizacionDW
        {
            get
            {
                return mOrganizacionDW;
            }
        }

        /// <summary>
        /// Obtiene el DS de proyectos
        /// </summary>
        public DataWrapperProyecto DataWrapperProyecto
        {
            get
            {
                return mDataWrapperProyecto;
            }
        }


        /// <summary>
        /// Obtiene el DW de comentarios
        /// </summary>
        public DataWrapperComentario ComentarioDW
        {
            get
            {
                return mComentarioDW;
            }
        }

        /// <summary>
        /// Obtiene el DS de documentación
        /// </summary>
        public DataWrapperDocumentacion DocumentacionDW
        {
            get
            {
                return mDataWrapperDocumentacion;
            }
        }

        /// <summary>
        /// Obtiene el DS de personas
        /// </summary>
        public DataWrapperPersona DataWrapperPersona
        {
            get
            {
                return mDataWrapperPersona;
            }
        }

        /// <summary>
        /// Obtiene el DS de Identidad
        /// </summary>
        public DataWrapperIdentidad DataWrapperIdentidad
        {
            get
            {
                return mDataWrapperIdentidad;
            }
        }

        /// <summary>
        /// DataSet de notifiaciones.
        /// </summary>
        public DataWrapperNotificacion NotificacionDW
        {
            get
            {
                return mNotificacionDW;
            }
        }

        /// <summary>
        /// Obtiene el núumero de resultados
        /// </summary>
        public int NumResultados
        {
            get
            {
                return mNumResultados;
            }
            set
            {
                mNumResultados = value;
            }
        }

        /// <summary>
        /// Obtiene la identidad del buscador.
        /// </summary>
        public Guid IdentidadBusquedaID
        {
            get
            {
                return mIdentidadBusquedaID;
            }
            set
            {
                mIdentidadBusquedaID = value;
            }
        }

        /// <summary>
        /// Lista de filtros del facetado.
        /// </summary>
        public Dictionary<string, List<string>> ListaFiltros
        {
            get
            {
                return mListaFiltros;
            }
            set
            {
                mListaFiltros = value;
            }
        }

        /// <summary>
        /// ID del proyecto de origen para la búsqueda.
        /// </summary>
        public Guid ProyectoOrigenID
        {
            get
            {
                return mProyectoOrigenID;
            }
            set
            {
                mProyectoOrigenID = value;
            }
        }

        /// <summary>
        /// ID de la organización para la búsqueda.
        /// </summary>
        public Guid OrganizacionID
        {
            get
            {
                return mOrganizacionID;
            }
            set
            {
                this.mOrganizacionID = value;
            }
        }

        /// <summary>
        /// ID del usuario para la búsqueda.
        /// </summary>
        public Guid UsuarioID
        {
            get
            {
                return mUsuarioID;
            }
            set
            {
                this.mUsuarioID = value;
            }
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Busca contenidos en todas las comunidades
        /// </summary>
        /// <param name="pEsIdentidadInvitada">Verdad si es la identidad invitada</param>
        /// <param name="pPerfilID">Identificador del perifl</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void BuscarContenidos(Guid pProyectoID, Guid pPerfilID, bool pEsIdentidadInvitada)
        {
            DataSet resultadosDS = new DataSet();

            if (mListaOrdenadaElementos == null)
            {
                mListaOrdenadaElementos = new Dictionary<string, TiposResultadosMetaBuscador>();
            }
            mListaContadorTags = new Dictionary<string, int>();

            Dictionary<TiposResultadosMetaBuscador, List<string>> listaIdsPorTipoResultado = new Dictionary<TiposResultadosMetaBuscador, List<string>>();

            //Ordeno los resultado obtenidos
            foreach (string id in mListaOrdenadaElementos.Keys)
            {
                if (!mListaContadorTags.ContainsKey(id))
                {
                    mListaContadorTags.Add(id, 0);

                    if (!listaIdsPorTipoResultado.ContainsKey(mListaOrdenadaElementos[id]))
                    {
                        listaIdsPorTipoResultado.Add(mListaOrdenadaElementos[id], new List<string>());
                    }
                    listaIdsPorTipoResultado[mListaOrdenadaElementos[id]].Add(id);
                }
            }
            //Obtengo los elementos resultado
            List<Guid> listaGuids = new List<Guid>();
            //Comentarios
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.Comentario))
            {
                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Comentario]);

                ComentarioCN comentarioCN = new ComentarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                mComentarioDW = comentarioCN.ObtenerComentariosPorID(listaGuids);
            }

            //Comunidades
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.Comunidad))
            {
                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Comunidad]);
                mDataWrapperProyecto = ProyectoAD.ObtenerProyectosPorID(listaGuids);

                if (!pEsIdentidadInvitada)
                {
                    mDataWrapperProyecto.Merge(ProyectoAD.ObtenerNumeroElementosPerfilEnProyectos(listaGuids, pPerfilID));
                }
            }
            //Identidades de Personas
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.IdentidadPersona))
            {
                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.IdentidadPersona]);

                mDataWrapperIdentidad = IdentidadAD.ObtenerIdentidadesPorID(listaGuids, true);
                mDataWrapperPersona = PersonaAD.ObtenerPersonasPorIdentidad(listaGuids);
                mOrganizacionDW = OrganizacionAD.ObtenerOrganizacionesDeIdentidades(listaGuids);
                List<Guid> listaOrganizaciones = new List<Guid>();
                foreach (Organizacion filaOrg in mOrganizacionDW.ListaOrganizacion)
                {
                    if (!listaOrganizaciones.Contains(filaOrg.OrganizacionID))
                    {
                        listaOrganizaciones.Add(filaOrg.OrganizacionID);
                    }
                }
                mDataWrapperIdentidad.Merge(IdentidadAD.ObtenerIdentidadesDeOrganizaciones(listaOrganizaciones, pProyectoID, TiposIdentidad.Organizacion));
            }
            //Organizacion
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.IdentidadOrganizacion))
            {
                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.IdentidadOrganizacion]);

                //mCurriculumOrganizacionDS = CurriculumOrganizacionAD.ObtenerCurriculumReducido(listaGuids);

                if (mOrganizacionDW != null)
                    mOrganizacionDW.Merge(OrganizacionAD.ObtenerOrganizacionesPorIdentidad(listaGuids));
                else
                    mOrganizacionDW = OrganizacionAD.ObtenerOrganizacionesPorIdentidad(listaGuids);

                if (mDataWrapperIdentidad != null)
                    mDataWrapperIdentidad.Merge(IdentidadAD.ObtenerIdentidadesPorID(listaGuids, true));
                else
                    mDataWrapperIdentidad = IdentidadAD.ObtenerIdentidadesPorID(listaGuids, true);
            }
            //Personas
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.Persona))
            {
                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Persona]);

                if (mDataWrapperPersona != null)
                    //mDataWrapperPersona.Merge(PersonaAD.ObtenerPersonasPorID(listaGuids), true);
                    mDataWrapperPersona.Merge(PersonaAD.ObtenerPersonasPorID(listaGuids));
                else
                    mDataWrapperPersona = PersonaAD.ObtenerPersonasPorID(listaGuids);

                if (mOrganizacionDW != null)
                    mOrganizacionDW.Merge(OrganizacionAD.ObtenerOrganizacionesDeListaPersona(listaGuids));
                else
                    mOrganizacionDW = OrganizacionAD.ObtenerOrganizacionesDeListaPersona(listaGuids);
            }
            //Grupo
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.Grupo))
            {
                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Grupo]);

                if (mDataWrapperIdentidad == null)
                {
                    mDataWrapperIdentidad = new DataWrapperIdentidad();
                }

                mDataWrapperIdentidad.Merge(IdentidadAD.ObtenerGruposPorIDGrupo(listaGuids, false));
            }
            //Documentos
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.Documento))
            {
                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Documento]);
                mDataWrapperDocumentacion = new DataWrapperDocumentacion();
                if (ProyectoOrigenID != Guid.Empty)
                {
                    DocumentacionAD.ObtenerBaseRecursosProyecto(mDataWrapperDocumentacion, ProyectoOrigenID);
                }
                else if (pProyectoID != ProyectoAD.MetaProyecto)
                {
                    DocumentacionAD.ObtenerBaseRecursosProyecto(mDataWrapperDocumentacion, pProyectoID);
                }
                else if (OrganizacionID != Guid.Empty)
                {
                    DocumentacionAD.ObtenerBaseRecursosOrganizacion(mDataWrapperDocumentacion, OrganizacionID);
                }
                else if (UsuarioID != Guid.Empty)
                {
                    DocumentacionAD.ObtenerBaseRecursosUsuario(mDataWrapperDocumentacion, UsuarioID);
                }

                mDataWrapperDocumentacion.Merge(DocumentacionAD.ObtenerDocumentosPorID(listaGuids, true, pProyectoID));
            }

            //Mensajes
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.Mensaje))
            {
                int tipoBandeja = 0;

                if ((ListaFiltros != null) && (ListaFiltros.Keys.Count != 0))
                {
                    if (ListaFiltros["dce:type"][0] == "Enviados")
                    {
                        tipoBandeja = 1;
                    }
                    else if (ListaFiltros["dce:type"][0] == "Eliminados")
                    {
                        tipoBandeja = 2;
                    }
                }

                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Mensaje]);
                //mCorreoDS = CorreoAD.ObtenerCorreoPorListaIDs(listaGuids, IdentidadBusquedaID, tipoBandeja);
            }
            //Invitaciones
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.Invitacion))
            {
                listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Invitacion]);
                mNotificacionDW = NotificacionAD.ObtenerInvitacionesPorIDConNombreCorto(listaGuids);
            }
            //Suscripciones
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.Suscripcion))
            {
                Dictionary<Guid, List<Guid>> elementoSuscripcion = new Dictionary<Guid, List<Guid>>();

                foreach (string elemSus in listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Suscripcion])
                {
                    Guid suscripID = new Guid(elemSus.Substring(elemSus.IndexOf("_") + 1));
                    Guid docID = new Guid(elemSus.Substring(0, elemSus.IndexOf("_")));

                    if (!elementoSuscripcion.ContainsKey(suscripID))
                    {
                        elementoSuscripcion.Add(suscripID, new List<Guid>());
                    }

                    elementoSuscripcion[suscripID].Add(docID);
                }

                //listaGuids = listaIdsPorTipoResultado[TiposResultadosMetaBuscador.Suscripcion];
                //mResultadosSuscripcionDS = ResultadosSuscripcionAD.ObtenerResultadosSuscripcionPorSuscripcionesIDYElementoesID(elementoSuscripcion);
            }
            ////GrupoContacto
            //if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.GrupoContacto))
            //{
            //    listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.GrupoContacto]);

            //    if (mAmigosDS != null)
            //        mAmigosDS.Merge(AmigosAD.ObtenerGrupoPorID(listaGuids), true);
            //    else
            //        mAmigosDS = AmigosAD.ObtenerGrupoPorID(listaGuids);

            //}
            //OrgContacto o PerContacto
            if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.OrgContacto) || mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.PerContacto))
            {
                if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.OrgContacto))
                {
                    listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.OrgContacto]);
                    if (mListaOrdenadaElementos.ContainsValue(TiposResultadosMetaBuscador.PerContacto))
                    {
                        listaGuids.AddRange(ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.PerContacto]));
                    }
                }
                else
                {
                    listaGuids = ObtenerListaGuidsDeStrings(listaIdsPorTipoResultado[TiposResultadosMetaBuscador.PerContacto]);
                }

                if (mDataWrapperIdentidad != null)
                    mDataWrapperIdentidad.Merge(IdentidadAD.ObtenerIdentidadesPorID(listaGuids, true));
                else
                    mDataWrapperIdentidad = IdentidadAD.ObtenerIdentidadesPorID(listaGuids, true);
            }


        }

        /// <summary>
        /// Obtiene una lista de GUIDs a partir de una de Strings.
        /// </summary>
        /// <param name="pListaStrings">Lista de Strings</param>
        /// <returns>Lista de GUIDs a partir de una de Strings</returns>
        private List<Guid> ObtenerListaGuidsDeStrings(List<string> pListaStrings)
        {
            List<Guid> listaGuids = new List<Guid>();

            foreach (string guid in pListaStrings)
            {
                Guid guidTemporal;
                if (Guid.TryParse(guid, out guidTemporal))
                {
                    listaGuids.Add(guidTemporal);
                }
            }

            return listaGuids;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~MetaBuscadorCN()
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
                    if (mOrganizacionAD != null)
                        mOrganizacionAD.Dispose();
                    if (mProyectoAD != null)
                        mProyectoAD.Dispose();
                    if (mDocumentacionAD != null)
                        mDocumentacionAD.Dispose();
                    if (mPersonaAD != null)
                        mPersonaAD.Dispose();
                    if (mComentarioAD != null)
                        mComentarioAD.Dispose();
                }

                mOrganizacionAD = null;
                mProyectoAD = null;
                mDocumentacionAD = null;
                mPersonaAD = null;
                mComentarioAD = null;
            }
        }

        #endregion
    }
}
