using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// CMSPagina
    /// </summary>
    public class ProyectoPestanyaMenu : ElementoGnoss
    {
        /// <summary>
        /// Lista de Grupos
        /// </summary>
        private Dictionary<Guid, ProyectoPestanyaMenuRolGrupoIdentidades> mListaRolGrupoIdentidades;

        /// <summary>
        /// Lista de Perfiles
        /// </summary>
        private Dictionary<Guid, ProyectoPestanyaMenuRolIdentidad> mListaRolIdentidad;

        private ProyectoPestanyaMenu mFilaProyectoPestanyaMenu;
        private static object BLOQUEO_PRESENTACION_PESTANYAS = new object();

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;

        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de pagina de una pestaña y del gestor de Proyecto pasados por parámetro
        /// </summary>
        /// <param name="pFilaProyectoPestanyaMenu">Fila de pestaña</param>
        /// <param name="pFilaProyectoPestanyaMenu">Gestor de Proyecto</param>
        public ProyectoPestanyaMenu(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pFilaProyectoPestanyaMenu, GestionProyecto pGestorProyecto, LoggingService loggingService, EntityContext entityContext)
            : base(pGestorProyecto, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            this.FilaProyectoPestanyaMenu = pFilaProyectoPestanyaMenu;
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Carga los grupos lecores
        /// </summary>
        public void CargarRolGrupoIdentidades()
        {
            mListaRolGrupoIdentidades = new Dictionary<Guid, ProyectoPestanyaMenuRolGrupoIdentidades>();

            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaRolGrupoIdentidades in GestorProyecto.DataWrapperProyectos.ListaProyectoPestanyaMenuRolGrupoIdentidades)
            {
                if (filaRolGrupoIdentidades.PestanyaID == Clave)
                {
                    if (!mListaRolGrupoIdentidades.ContainsKey(filaRolGrupoIdentidades.GrupoID))
                    {
                        mListaRolGrupoIdentidades.Add(filaRolGrupoIdentidades.GrupoID, new ProyectoPestanyaMenuRolGrupoIdentidades(filaRolGrupoIdentidades, GestorProyecto, mLoggingService));
                    }
                }
            }
        }

        /// <summary>
        /// Carga los perfiles lecores
        /// </summary>
        public void CargarRolIdentidad()
        {
            mListaRolIdentidad = new Dictionary<Guid, ProyectoPestanyaMenuRolIdentidad>();

            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaRolIdentidad in GestorProyecto.DataWrapperProyectos.ListaProyectoPestanyaMenuRolIdentidad)
            {
                if (filaRolIdentidad.PestanyaID == Clave)
                {
                    if (!mListaRolIdentidad.ContainsKey(filaRolIdentidad.PerfilID))
                    {
                        mListaRolIdentidad.Add(filaRolIdentidad.PerfilID, new ProyectoPestanyaMenuRolIdentidad(filaRolIdentidad, GestorProyecto, mLoggingService));
                    }
                }
            }
        }
        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establedce si la página está activa
        /// </summary>
        public bool Visible
        {
            get
            {
                return FilaProyectoPestanyaMenu.Visible;
            }
            set
            {
                FilaProyectoPestanyaMenu.Visible = value;
            }
        }

        /// <summary>
        /// Obtiene si la pestaña es de tipo busqueda
        /// </summary>
        public bool EsPestanyaBusqueda
        {
            get
            {
                return (TipoPestanya == TipoPestanyaMenu.Recursos ||
                    TipoPestanya == TipoPestanyaMenu.Debates ||
                    TipoPestanya == TipoPestanyaMenu.Preguntas ||
                    TipoPestanya == TipoPestanyaMenu.Encuestas ||
                    TipoPestanya == TipoPestanyaMenu.BusquedaSemantica ||
                    TipoPestanya == TipoPestanyaMenu.PersonasYOrganizaciones ||
                    TipoPestanya == TipoPestanyaMenu.BusquedaAvanzada);
            }
        }

        /// <summary>
        /// Obtiene la meta-description
        /// </summar
        public string MetaDescription
        {
            get
            {
                return FilaProyectoPestanyaMenu.MetaDescription;
            }
            set
            {
                FilaProyectoPestanyaMenu.MetaDescription = value;
            }
        }

        /// <summary>
        /// Obtiene o establedce el tipo
        /// </summary>
        public TipoPestanyaMenu TipoPestanya
        {
            get
            {
                return (TipoPestanyaMenu)FilaProyectoPestanyaMenu.TipoPestanya;
            }
            set
            {
                FilaProyectoPestanyaMenu.TipoPestanya = (short)value;
            }
        }

        /// <summary>
        /// Obtiene o establedce el tipo
        /// </summary>
        public override string Nombre
        {
            get
            {
                return FilaProyectoPestanyaMenu.Nombre;
            }
            set
            {
                FilaProyectoPestanyaMenu.Nombre = value;
            }
        }

        /// <summary>
        /// Obtiene o establedce el tipo
        /// </summary>
        public string Ruta
        {
            get
            {
                return FilaProyectoPestanyaMenu.Ruta;
            }
            set
            {
                FilaProyectoPestanyaMenu.Ruta = value;
            }
        }

        /// <summary>
        /// Obtiene o establedce el titulo
        /// </summary>
        public string Titulo
        {
            get
            {
                return FilaProyectoPestanyaMenu.Titulo;
            }
            set
            {
                FilaProyectoPestanyaMenu.Titulo = value;
            }
        }


        /// <summary>
        /// Obtiene o establedce el si es visible para los que no tienen acceso
        /// </summary>
        public bool VisibleSinAcceso
        {
            get
            {
                return FilaProyectoPestanyaMenu.VisibleSinAcceso;
            }
            set
            {
                FilaProyectoPestanyaMenu.VisibleSinAcceso = value;
            }
        }

        /// <summary>
        /// Obtiene o establedce el titulo
        /// </summary>
        public string CSSBodyClassPestanya
        {
            get
            {
                return FilaProyectoPestanyaMenu.CSSBodyClass;
            }
            set
            {
                FilaProyectoPestanyaMenu.CSSBodyClass = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si está activa
        /// </summary>
        public bool Activa
        {
            get
            {
                return FilaProyectoPestanyaMenu.Activa;
            }
            set
            {
                FilaProyectoPestanyaMenu.Activa = value;
            }
        }


        /// <summary>
        /// Devuelve el gestor de Proyecto
        /// </summary>
        public GestionProyecto GestorProyecto
        {
            get
            {
                return (GestionProyecto)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila de la página
        /// </summary>
        public AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu FilaProyectoPestanyaMenu
        {
            get;
        }

        /// <summary>
        /// Obtiene la fila de busqueda
        /// </summary>
        public ProyectoPestanyaBusqueda FilaProyectoPestanyaBusqueda
        {
            get
            {
                ProyectoPestanyaBusqueda filasBusqueda = FilaProyectoPestanyaMenu.ProyectoPestanyaBusqueda;
                if (filasBusqueda != null)
                {
                    return filasBusqueda;
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene las filas de asistentes
        /// TFG FRAN
        /// </summary>
        public List<ProyectoPestanyaDashboardAsistente> FilasProyectoPestanyaDashboardAsistente
        {
            get
            {
                ICollection<ProyectoPestanyaDashboardAsistente> filasAsistentes = FilaProyectoPestanyaMenu.ProyectoPestanyaDashboardAsistente;
                if (filasAsistentes != null)
                {
                    return filasAsistentes.ToList();
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la fila de cms
        /// </summary>
        public ProyectoPestanyaCMS FilaProyectoPestanyaCMS
        {
            get
            {
                List<ProyectoPestanyaCMS> filasCMS = FilaProyectoPestanyaMenu.ProyectoPestanyaCMS.ToList();
                if (filasCMS.Count == 1)
                {
                    return filasCMS[0];
                }
                return null;
            }
        }

        /// <summary>
        /// Devuelve la clave de la pestaña
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaProyectoPestanyaMenu.PestanyaID;
            }
        }

        /// <summary>
        /// Obtiene el tipo de privacidad de la página
        /// </summary>
        public TipoPrivacidadPagina Privacidad
        {
            get
            {
                return (TipoPrivacidadPagina)FilaProyectoPestanyaMenu.Privacidad;
            }
            set
            {
                FilaProyectoPestanyaMenu.Privacidad = (short)value;
            }
        }

        /// <summary>
        /// Obtiene o establece el HTML alternativo en caso de no poder mostrar la página
        /// </summary>
        public string HTMLAlternativo
        {
            get
            {
                return FilaProyectoPestanyaMenu.HtmlAlternativo;
            }
            set
            {
                FilaProyectoPestanyaMenu.HtmlAlternativo = value;
            }
        }

        /// <summary>
        /// Obtiene los idiomas disponibles
        /// </summary>
        public List<string> ListaIdiomasDisponibles(Dictionary<string, string> listaIdiomas)
        {
            List<string> listaIdiomasDisponibles = new List<string>();
            foreach (string idioma in listaIdiomas.Keys)
            {
                if (string.IsNullOrEmpty(FilaProyectoPestanyaMenu.IdiomasDisponibles) || UtilCadenas.ObtenerTextoDeIdioma(FilaProyectoPestanyaMenu.IdiomasDisponibles, idioma, null, true) == "true")
                {
                    listaIdiomasDisponibles.Add(idioma);
                }
            }
            return listaIdiomasDisponibles;
        }

        /// <summary>
        /// Obtiene la lista de grupos lectores
        /// </summary>
        public Dictionary<Guid, ProyectoPestanyaMenuRolGrupoIdentidades> ListaRolGrupoIdentidades
        {
            get
            {
                if (mListaRolGrupoIdentidades == null)
                {
                    CargarRolGrupoIdentidades();
                }
                return mListaRolGrupoIdentidades;
            }
        }

        /// <summary>
        /// Obtiene la lista de perfiles lectores
        /// </summary>
        public Dictionary<Guid, ProyectoPestanyaMenuRolIdentidad> ListaRolIdentidad
        {
            get
            {
                if (mListaRolIdentidad == null)
                {
                    CargarRolIdentidad();
                }
                return mListaRolIdentidad;
            }
        }

        #endregion

        #region Agregar Editores/grupos

        /// <summary>
        /// Agrega un nuevo grupo de editores
        /// </summary>
        ///<param name="pGrupoID"></param>
        /// <returns></returns>
        public ProyectoPestanyaMenuRolGrupoIdentidades AgregarGrupoEditorAPagina(Guid pGrupoID)
        {
            AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaGrupoIdentidades = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades();
            filaGrupoIdentidades.PestanyaID = Clave;
            filaGrupoIdentidades.GrupoID = pGrupoID;

            GestorProyecto.DataWrapperProyectos.ListaProyectoPestanyaMenuRolGrupoIdentidades.Add(filaGrupoIdentidades);
            ProyectoPestanyaMenuRolGrupoIdentidades RolGrupoIdentidades = new ProyectoPestanyaMenuRolGrupoIdentidades(filaGrupoIdentidades, GestorProyecto, mLoggingService);

            ListaRolGrupoIdentidades.Add(pGrupoID, RolGrupoIdentidades);


            return RolGrupoIdentidades;
        }

        /// <summary>
        /// Elimina un grupo de editores
        /// </summary>
        ///<param name="pGrupoID"></param>
        /// <returns></returns>
        public void EliminarGrupoEditorDePagina(Guid pGrupoID)
        {
            AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaGrupoIdentidades = GestorProyecto.DataWrapperProyectos.ListaProyectoPestanyaMenuRolGrupoIdentidades.Find(proyectoPestanyaMenuRolIden => proyectoPestanyaMenuRolIden.PestanyaID.Equals(Clave) && proyectoPestanyaMenuRolIden.GrupoID.Equals(pGrupoID));//.FindByPestanyaIDGrupoID(Clave, pGrupoID);

            if (filaGrupoIdentidades != null)
            {
                GestorProyecto.DataWrapperProyectos.ListaProyectoPestanyaMenuRolGrupoIdentidades.Remove(filaGrupoIdentidades);
                mEntityContext.EliminarElemento(filaGrupoIdentidades);
                //filaGrupoIdentidades.Delete();
            }
        }

        /// <summary>
        /// Agrega un nuevo editor
        /// </summary>
        ///<param name="pGrupoID"></param>
        /// <returns></returns>
        public ProyectoPestanyaMenuRolIdentidad AgregarEditorAPagina(Guid pPerfilID)
        {
            AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaIdentidad = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad();
            filaIdentidad.PestanyaID = Clave;
            filaIdentidad.PerfilID = pPerfilID;

            GestorProyecto.DataWrapperProyectos.ListaProyectoPestanyaMenuRolIdentidad.Add(filaIdentidad);
            ProyectoPestanyaMenuRolIdentidad RolIdentidad = new ProyectoPestanyaMenuRolIdentidad(filaIdentidad, GestorProyecto, mLoggingService);

            ListaRolIdentidad.Add(pPerfilID, RolIdentidad);


            return RolIdentidad;
        }

        /// <summary>
        /// Elimina un nuevo editor
        /// </summary>
        ///<param name="pGrupoID"></param>
        /// <returns></returns>
        public void EliminarEditorDePagina(Guid pPerfilID)
        {
            AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaIdentidad = GestorProyecto.DataWrapperProyectos.ListaProyectoPestanyaMenuRolIdentidad.Find(pestanyaMenuRolIden => pestanyaMenuRolIden.PestanyaID.Equals(Clave) && pestanyaMenuRolIden.PerfilID.Equals(pPerfilID));//FindByPestanyaIDPerfilID(Clave,pPerfilID);

            if (filaIdentidad != null)
            {
                GestorProyecto.DataWrapperProyectos.ListaProyectoPestanyaMenuRolIdentidad.Remove(filaIdentidad);
                mEntityContext.EliminarElemento(filaIdentidad);
                mEntityContext.SaveChanges();
            }
        }

        public void EliminarLectores()
        {
            foreach (ProyectoPestanyaMenuRolIdentidad fila in ListaRolIdentidad.Values)
            {
                fila.FilaElemento.Delete();
            }
            foreach (ProyectoPestanyaMenuRolGrupoIdentidades fila in ListaRolGrupoIdentidades.Values)
            {
                fila.FilaElemento.Delete();
            }
            CargarRolIdentidad();
            CargarRolGrupoIdentidades();
        }

        #endregion

        /// <summary>
        /// 'Procesamos' las tablas de PestanyaPresentacion para ponerlas en el lugar de las tablas de Presentacion en función de la pestanya y la ontología
        /// </summary>
        /// <param name="pDWproyPresentacion">DataWrapper con las tablas de presentacion</param>
        public static void ProcesarPresentacionPestanyas(ref DataWrapperProyecto pDWproyPresentacion, Guid? pPestanyaID)
        {
            lock (BLOQUEO_PRESENTACION_PESTANYAS)
            {
                DataWrapperProyecto proyectoDWDevolver = new DataWrapperProyecto();
                //Las tablas de las 'pestanyas' deben de meterse en las normales siempre para que las propiedades cacheadas sean las mismas
                //Si la pestanya es nula, las filas correspondientes a pestanyas tienen que tener el nombre vacío
                //Si la pestanya tiene valor, las filas correspondientes a las normales tienen que tener el nombre vacío

                //Listado
                List<Guid> listaOntologiasPersonalizadasLista = new List<Guid>();
                short ordenLista = 0;
                foreach (PresentacionPestanyaListadoSemantico fila in pDWproyPresentacion.ListaPresentacionPestanyaListadoSemantico)
                {
                    if (pPestanyaID.HasValue && fila.PestanyaID == pPestanyaID.Value)
                    {
                        PresentacionListadoSemantico presentacionListadoSemantico = new PresentacionListadoSemantico();
                        presentacionListadoSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionListadoSemantico.ProyectoID = fila.ProyectoID;
                        presentacionListadoSemantico.OntologiaID = fila.OntologiaID;
                        presentacionListadoSemantico.Orden = ordenLista;
                        presentacionListadoSemantico.Ontologia = fila.Ontologia;
                        presentacionListadoSemantico.Propiedad = fila.Propiedad;
                        presentacionListadoSemantico.Nombre = fila.Nombre;
                        proyectoDWDevolver.ListaPresentacionListadoSemantico.Add(presentacionListadoSemantico);
                        ordenLista++;
                        if (!listaOntologiasPersonalizadasLista.Contains(fila.OntologiaID))
                        {
                            listaOntologiasPersonalizadasLista.Add(fila.OntologiaID);
                        }
                    }
                    else
                    {
                        PresentacionListadoSemantico presentacionListadoSemantico = new PresentacionListadoSemantico();
                        presentacionListadoSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionListadoSemantico.ProyectoID = fila.ProyectoID;
                        presentacionListadoSemantico.OntologiaID = fila.OntologiaID;
                        presentacionListadoSemantico.Orden = ordenLista;
                        presentacionListadoSemantico.Ontologia = fila.Ontologia;
                        presentacionListadoSemantico.Propiedad = fila.Propiedad;
                        presentacionListadoSemantico.Nombre = "";
                        proyectoDWDevolver.ListaPresentacionListadoSemantico.Add(presentacionListadoSemantico);
                        ordenLista++;
                    }
                }
                foreach (PresentacionListadoSemantico fila in pDWproyPresentacion.ListaPresentacionListadoSemantico)
                {
                    if (listaOntologiasPersonalizadasLista.Contains(fila.OntologiaID))
                    {
                        PresentacionListadoSemantico presentacionListadoSemantico = new PresentacionListadoSemantico();
                        presentacionListadoSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionListadoSemantico.ProyectoID = fila.ProyectoID;
                        presentacionListadoSemantico.OntologiaID = fila.OntologiaID;
                        presentacionListadoSemantico.Orden = ordenLista;
                        presentacionListadoSemantico.Ontologia = fila.Ontologia;
                        presentacionListadoSemantico.Propiedad = fila.Propiedad;
                        presentacionListadoSemantico.Nombre = "";
                        proyectoDWDevolver.ListaPresentacionListadoSemantico.Add(presentacionListadoSemantico);
                        ordenLista++;
                    }
                    else
                    {
                        PresentacionListadoSemantico presentacionListadoSemantico = new PresentacionListadoSemantico();
                        presentacionListadoSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionListadoSemantico.ProyectoID = fila.ProyectoID;
                        presentacionListadoSemantico.OntologiaID = fila.OntologiaID;
                        presentacionListadoSemantico.Orden = ordenLista;
                        presentacionListadoSemantico.Ontologia = fila.Ontologia;
                        presentacionListadoSemantico.Propiedad = fila.Propiedad;
                        presentacionListadoSemantico.Nombre = fila.Nombre;
                        proyectoDWDevolver.ListaPresentacionListadoSemantico.Add(presentacionListadoSemantico);
                        ordenLista++;
                    }
                }
                pDWproyPresentacion.ListaPresentacionListadoSemantico.Clear();
                pDWproyPresentacion.ListaPresentacionPestanyaListadoSemantico.Clear();
                foreach (PresentacionListadoSemantico fila in proyectoDWDevolver.ListaPresentacionListadoSemantico)
                {
                    PresentacionListadoSemantico presentacionListadoSemantico = new PresentacionListadoSemantico();
                    presentacionListadoSemantico.OrganizacionID = fila.OrganizacionID;
                    presentacionListadoSemantico.ProyectoID = fila.ProyectoID;
                    presentacionListadoSemantico.OntologiaID = fila.OntologiaID;
                    presentacionListadoSemantico.Orden = ordenLista;
                    presentacionListadoSemantico.Ontologia = fila.Ontologia;
                    presentacionListadoSemantico.Propiedad = fila.Propiedad;
                    presentacionListadoSemantico.Nombre = fila.Nombre;
                    pDWproyPresentacion.ListaPresentacionListadoSemantico.Add(presentacionListadoSemantico);
                }


                //Mosaico
                List<Guid> listaOntologiasPersonalizadasMosaico = new List<Guid>();
                short ordenMosaico = 0;
                foreach (PresentacionPestanyaMosaicoSemantico fila in pDWproyPresentacion.ListaPresentacionPestanyaMosaicoSemantico)
                {
                    if (pPestanyaID.HasValue && fila.PestanyaID == pPestanyaID.Value)
                    {
                        PresentacionMosaicoSemantico presentacionMosaicoSemantico = new PresentacionMosaicoSemantico();
                        presentacionMosaicoSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionMosaicoSemantico.ProyectoID = fila.ProyectoID;
                        presentacionMosaicoSemantico.OntologiaID = fila.OntologiaID;
                        presentacionMosaicoSemantico.Orden = ordenMosaico;
                        presentacionMosaicoSemantico.Ontologia = fila.Ontologia;
                        presentacionMosaicoSemantico.Propiedad = fila.Propiedad;
                        presentacionMosaicoSemantico.Nombre = fila.Nombre;
                        proyectoDWDevolver.ListaPresentacionMosaicoSemantico.Add(presentacionMosaicoSemantico);
                        ordenMosaico++;
                        if (!listaOntologiasPersonalizadasMosaico.Contains(fila.OntologiaID))
                        {
                            listaOntologiasPersonalizadasMosaico.Add(fila.OntologiaID);
                        }
                    }
                    else
                    {
                        PresentacionMosaicoSemantico presentacionMosaicoSemantico = new PresentacionMosaicoSemantico();
                        presentacionMosaicoSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionMosaicoSemantico.ProyectoID = fila.ProyectoID;
                        presentacionMosaicoSemantico.OntologiaID = fila.OntologiaID;
                        presentacionMosaicoSemantico.Orden = ordenMosaico;
                        presentacionMosaicoSemantico.Ontologia = fila.Ontologia;
                        presentacionMosaicoSemantico.Propiedad = fila.Propiedad;
                        presentacionMosaicoSemantico.Nombre = "";
                        proyectoDWDevolver.ListaPresentacionMosaicoSemantico.Add(presentacionMosaicoSemantico);
                        ordenMosaico++;
                    }
                }
                foreach (PresentacionMosaicoSemantico fila in pDWproyPresentacion.ListaPresentacionMosaicoSemantico)
                {
                    if (listaOntologiasPersonalizadasMosaico.Contains(fila.OntologiaID))
                    {
                        PresentacionMosaicoSemantico presentacionMosaicoSemantico = new PresentacionMosaicoSemantico();
                        presentacionMosaicoSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionMosaicoSemantico.ProyectoID = fila.ProyectoID;
                        presentacionMosaicoSemantico.OntologiaID = fila.OntologiaID;
                        presentacionMosaicoSemantico.Orden = ordenMosaico;
                        presentacionMosaicoSemantico.Ontologia = fila.Ontologia;
                        presentacionMosaicoSemantico.Propiedad = fila.Propiedad;
                        presentacionMosaicoSemantico.Nombre = "";
                        proyectoDWDevolver.ListaPresentacionMosaicoSemantico.Add(presentacionMosaicoSemantico);
                        ordenMosaico++;
                    }
                    else
                    {
                        PresentacionMosaicoSemantico presentacionMosaicoSemantico = new PresentacionMosaicoSemantico();
                        presentacionMosaicoSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionMosaicoSemantico.ProyectoID = fila.ProyectoID;
                        presentacionMosaicoSemantico.OntologiaID = fila.OntologiaID;
                        presentacionMosaicoSemantico.Orden = ordenMosaico;
                        presentacionMosaicoSemantico.Ontologia = fila.Ontologia;
                        presentacionMosaicoSemantico.Propiedad = fila.Propiedad;
                        presentacionMosaicoSemantico.Nombre = fila.Nombre;
                        proyectoDWDevolver.ListaPresentacionMosaicoSemantico.Add(presentacionMosaicoSemantico);
                        ordenMosaico++;
                    }
                }
                pDWproyPresentacion.ListaPresentacionMosaicoSemantico.Clear();
                pDWproyPresentacion.ListaPresentacionPestanyaMosaicoSemantico.Clear();
                foreach (PresentacionMosaicoSemantico fila in proyectoDWDevolver.ListaPresentacionMosaicoSemantico)
                {
                    PresentacionMosaicoSemantico presentacionMosaicoSemantico = new PresentacionMosaicoSemantico();
                    presentacionMosaicoSemantico.OrganizacionID = fila.OrganizacionID;
                    presentacionMosaicoSemantico.ProyectoID = fila.ProyectoID;
                    presentacionMosaicoSemantico.OntologiaID = fila.OntologiaID;
                    presentacionMosaicoSemantico.Orden = ordenMosaico;
                    presentacionMosaicoSemantico.Ontologia = fila.Ontologia;
                    presentacionMosaicoSemantico.Propiedad = fila.Propiedad;
                    presentacionMosaicoSemantico.Nombre = fila.Nombre;
                    pDWproyPresentacion.ListaPresentacionMosaicoSemantico.Add(presentacionMosaicoSemantico);
                }

                //Mapa
                List<Guid> listaOntologiasPersonalizadasMapa = new List<Guid>();
                short ordenMapa = 0;
                foreach (PresentacionPestanyaMapaSemantico fila in pDWproyPresentacion.ListaPresentacionPestanyaMapaSemantico)
                {
                    if (pPestanyaID.HasValue && fila.PestanyaID == pPestanyaID.Value)
                    {
                        PresentacionMapaSemantico presentacionMapaSemantico = new PresentacionMapaSemantico();
                        presentacionMapaSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionMapaSemantico.ProyectoID = fila.ProyectoID;
                        presentacionMapaSemantico.OntologiaID = fila.OntologiaID;
                        presentacionMapaSemantico.Orden = ordenMosaico;
                        presentacionMapaSemantico.Ontologia = fila.Ontologia;
                        presentacionMapaSemantico.Propiedad = fila.Propiedad;
                        presentacionMapaSemantico.Nombre = fila.Nombre;
                        proyectoDWDevolver.ListaPresentacionMapaSemantico.Add(presentacionMapaSemantico);
                        ordenMapa++;
                        if (!listaOntologiasPersonalizadasMapa.Contains(fila.OntologiaID))
                        {
                            listaOntologiasPersonalizadasMapa.Add(fila.OntologiaID);
                        }
                    }
                    else
                    {
                        PresentacionMapaSemantico presentacionMapaSemantico = new PresentacionMapaSemantico();
                        presentacionMapaSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionMapaSemantico.ProyectoID = fila.ProyectoID;
                        presentacionMapaSemantico.OntologiaID = fila.OntologiaID;
                        presentacionMapaSemantico.Orden = ordenMosaico;
                        presentacionMapaSemantico.Ontologia = fila.Ontologia;
                        presentacionMapaSemantico.Propiedad = fila.Propiedad;
                        presentacionMapaSemantico.Nombre = "";
                        proyectoDWDevolver.ListaPresentacionMapaSemantico.Add(presentacionMapaSemantico);
                        ordenMapa++;
                    }
                }
                foreach (PresentacionMapaSemantico fila in pDWproyPresentacion.ListaPresentacionMapaSemantico)
                {
                    if (listaOntologiasPersonalizadasMapa.Contains(fila.OntologiaID))
                    {
                        PresentacionMapaSemantico presentacionMapaSemantico = new PresentacionMapaSemantico();
                        presentacionMapaSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionMapaSemantico.ProyectoID = fila.ProyectoID;
                        presentacionMapaSemantico.OntologiaID = fila.OntologiaID;
                        presentacionMapaSemantico.Orden = ordenMosaico;
                        presentacionMapaSemantico.Ontologia = fila.Ontologia;
                        presentacionMapaSemantico.Propiedad = fila.Propiedad;
                        presentacionMapaSemantico.Nombre = "";
                        proyectoDWDevolver.ListaPresentacionMapaSemantico.Add(presentacionMapaSemantico);
                        ordenMapa++;
                    }
                    else
                    {
                        PresentacionMapaSemantico presentacionMapaSemantico = new PresentacionMapaSemantico();
                        presentacionMapaSemantico.OrganizacionID = fila.OrganizacionID;
                        presentacionMapaSemantico.ProyectoID = fila.ProyectoID;
                        presentacionMapaSemantico.OntologiaID = fila.OntologiaID;
                        presentacionMapaSemantico.Orden = ordenMosaico;
                        presentacionMapaSemantico.Ontologia = fila.Ontologia;
                        presentacionMapaSemantico.Propiedad = fila.Propiedad;
                        presentacionMapaSemantico.Nombre = fila.Nombre;
                        proyectoDWDevolver.ListaPresentacionMapaSemantico.Add(presentacionMapaSemantico);
                        ordenMapa++;
                    }
                }
                pDWproyPresentacion.ListaPresentacionMapaSemantico.Clear();
                pDWproyPresentacion.ListaPresentacionPestanyaMapaSemantico.Clear();
                foreach (PresentacionMapaSemantico fila in proyectoDWDevolver.ListaPresentacionMapaSemantico)
                {
                    PresentacionMapaSemantico presentacionMapaSemantico = new PresentacionMapaSemantico();
                    presentacionMapaSemantico.OrganizacionID = fila.OrganizacionID;
                    presentacionMapaSemantico.ProyectoID = fila.ProyectoID;
                    presentacionMapaSemantico.OntologiaID = fila.OntologiaID;
                    presentacionMapaSemantico.Orden = ordenMosaico;
                    presentacionMapaSemantico.Ontologia = fila.Ontologia;
                    presentacionMapaSemantico.Propiedad = fila.Propiedad;
                    presentacionMapaSemantico.Nombre = fila.Nombre;
                    pDWproyPresentacion.ListaPresentacionMapaSemantico.Add(presentacionMapaSemantico);
                }
            }
        }
    }

    /// <summary>
    /// ProyectoPestanyaRolGrupoIdentidades
    /// </summary>
    public class ProyectoPestanyaMenuRolGrupoIdentidades : ElementoGnoss
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de ProyectoPestanyaRolGrupoIdentidades y del gestor de proyecto pasados por parámetro
        /// </summary>
        /// <param name="pFilaCMSRolGrupoIdentidades">Fila de CMSRolGrupoIdentidades</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public ProyectoPestanyaMenuRolGrupoIdentidades(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades pFilaProyectoPestanyaMenuRolGrupoIdentidades, GestionProyecto pGestorProyecto, LoggingService loggingService)
            : base(pGestorProyecto, loggingService)
        {
            this.FilaProyectoPestanyaMenuRolGrupoIdentidades = pFilaProyectoPestanyaMenuRolGrupoIdentidades;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establedce el grupo
        /// </summary>
        public Guid GrupoID
        {
            get
            {
                return FilaProyectoPestanyaMenuRolGrupoIdentidades.GrupoID;
            }
            set
            {
                FilaProyectoPestanyaMenuRolGrupoIdentidades.GrupoID = value;
            }
        }

        /// <summary>
        /// Devuelve el gestor de Proyecto que contiene al componente
        /// </summary>
        public GestionProyecto GestorProyecto
        {
            get
            {
                return (GestionProyecto)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila de la página
        /// </summary>
        public AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades FilaProyectoPestanyaMenuRolGrupoIdentidades
        {
            get;
        }

        #endregion
    }

    /// <summary>
    /// ProyectoPestanyaRolIdentidad
    /// </summary>
    public class ProyectoPestanyaMenuRolIdentidad : ElementoGnoss
    {
        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de ProyectoPestanyaRolIdentidad y del gestor de Proyecto pasados por parámetro
        /// </summary>
        /// <param name="pFilaProyectoPestanyaRolIdentidad">Fila de ProyectoPestanyaRolIdentidad</param>
        /// <param name="pGestorProyectoPestanya">Gestor de ProyectoPestanya</param>
        public ProyectoPestanyaMenuRolIdentidad(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad pFilaProyectoPestanyaMenuRolIdentidad, GestionProyecto pGestorProyecto, LoggingService loggingService)
            : base(pGestorProyecto, loggingService)
        {
            this.FilaProyectoPestanyaMenuRolIdentidad = pFilaProyectoPestanyaMenuRolIdentidad;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el perfil
        /// </summary>
        public Guid PerfilID
        {
            get
            {
                return FilaProyectoPestanyaMenuRolIdentidad.PerfilID;
            }
            set
            {
                FilaProyectoPestanyaMenuRolIdentidad.PerfilID = value;
            }
        }

        /// <summary>
        /// Devuelve el gestor de Proyecto 
        /// </summary>
        public GestionProyecto GestorProyecto
        {
            get
            {
                return (GestionProyecto)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila de la página
        /// </summary>
        public AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad FilaProyectoPestanyaMenuRolIdentidad
        {
            get;
        }

        #endregion
    }

}
