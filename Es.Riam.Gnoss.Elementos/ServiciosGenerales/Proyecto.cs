using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Proyecto
    /// </summary>
    public class Proyecto : ElementoGnoss, ICortarCopiarPegar, IDisposable
    {
        #region Miembros

        private Guid? mPersonalizacionID = null;

        private List<Persona> mListaAdministradores;

        private List<Guid> mListaAdministradoresIDs;

        private List<CategoriaTesauro> mListaCategoriasTesauro;

        private List<string> mListaTags;

        private List<Proyecto> mListaSubProyectos;

        /// <summary>
        /// Indica si se han cargado los niveles de certificación
        /// </summary>
        private bool mNivelesCertificacionCargados;

        /// <summary>
        /// Indice del elemento dentro del arbol
        /// </summary>
        private short mIndice = 0;

        /// <summary>
        /// Identidad que ha creado el proyecto.
        /// </summary>
        private Identidad.Identidad mIdentidadCreadoraProyecto;

        /// <summary>
        /// Lista de pestanyas
        /// </summary>
        private Dictionary<Guid, ProyectoPestanyaMenu> mListaPestanyasMenu = null;

        AD.EntityModel.Models.ProyectoDS.Proyecto mProyecto;


        private LoggingService mLoggingService;
        private EntityContext mEntityContext;


        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de proyecto
        /// </summary>
        /// <param name="pProyecto">Fila de proyecto</param>
        /// <param name="pGestionProyecto">Gestor de proyectos</param>
        public Proyecto(AD.EntityModel.Models.ProyectoDS.Proyecto pProyecto, GestionProyecto pGestionProyecto, LoggingService loggingService, EntityContext entityContext)
            : base(pGestionProyecto, loggingService)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            this.FilaProyecto = pProyecto;
        }

        /// <summary>
        /// Constructor de proyecto sin parámetros
        /// </summary>
        public Proyecto(LoggingService loggingService, EntityContext entityContext)
            : base(loggingService)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        #endregion

        #region Propiedades

        public Guid PersonalizacionID
        {
            get
            {
                if (!mPersonalizacionID.HasValue)
                {
                    mPersonalizacionID = Guid.Empty;

                    if (GestorProyectos.DataWrapperProyectos.ListaVistaVirtualProyecto.Where(vista => vista.ProyectoID.Equals(Clave)).Count() > 0)
                    {
                        mPersonalizacionID = GestorProyectos.DataWrapperProyectos.ListaVistaVirtualProyecto.Where(vista => vista.ProyectoID.Equals(Clave)).First().PersonalizacionID;
                    }
                }

                return mPersonalizacionID.Value;
            }
        }

        /// <summary>
        /// Devuelve una lista ordenada de niveles del proyecto actual
        /// </summary>
        public List<NivelCertificacion> ListaOrdenadaNivelesCertificacion
        {
            get
            {
                List<NivelCertificacion> resultado = new List<NivelCertificacion>();
                // Select("ProyectoID = '" + Clave + "'", "Orden ASC")
                foreach (NivelCertificacion fila in GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion.Where(nivelCertificacion => nivelCertificacion.ProyectoID.Equals(Clave)).OrderBy(nivelCertificacion => nivelCertificacion.ProyectoID))
                {
                    resultado.Add(fila);
                }

                return resultado;
            }
        }

        /// <summary>
        /// Obtiene o establece si se han cargado los niveles de certificación
        /// </summary>
        public bool NivelesCertificacionCargados
        {
            get
            {
                return mNivelesCertificacionCargados;
            }
            set
            {
                mNivelesCertificacionCargados = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el índice del elemento dentro del árbol
        /// </summary>
        public override short Indice
        {
            get
            {
                return mIndice;
            }
            set
            {
                mIndice = value;
            }
        }

        /// <summary>
        /// Devuelve si se puede copiar el elemento
        /// </summary>
        public override bool SePuedeCopiar
        {
            get
            {
                // Los proyectos NUNCA se pueden copiar
                return false;
            }
        }

        /// <summary>
        /// Devuelve si una comunidad permite comentarios a los recursos
        /// </summary>
        public bool ComentariosDisponibles
        {
            get
            {
                //ParametroGeneralDSEspacio.ParametroGeneralRow[] filasPG = (ParametroGeneralDSEspacio.ParametroGeneralRow [])GestorProyectos.ParametroGeneralDS.ParametroGeneral.Select("ProyectoID = '" + Clave + "'");
                List<ParametroGeneral> filasPG = GestorProyectos.ParametroGeneralDS.ListaParametroGeneral.Where(parametrosGeneral => parametrosGeneral.ProyectoID.Equals(Clave)).ToList();

                if (filasPG.Count > 0)
                {
                    return filasPG[0].ComentariosDisponibles;
                }
                return false;
            }
        }

        /// <summary>
        /// Devuelve si una comunidad permite votaciones a los recursos
        /// </summary>
        public bool VotacionesDisponibles
        {
            get
            {
                //ParametroGeneralDSEspacio.ParametroGeneralRow[] filasPG = (ParametroGeneralDSEspacio.ParametroGeneralRow[])GestorProyectos.ParametroGeneralDS.ParametroGeneral.Select("ProyectoID = '" + Clave + "'");
                List<ParametroGeneral> filasPG = GestorProyectos.ParametroGeneralDS.ListaParametroGeneral.Where(parametroGeneral => parametroGeneral.ProyectoID.Equals(Clave)).ToList();
                if (filasPG.Count > 0)
                {
                    return filasPG[0].VotacionesDisponibles;
                }
                return false;
            }
        }

        /// <summary>
        /// Devuelve si el proyecto se puede mover
        /// </summary>
        public override bool SePuedeCortarArrastrar
        {
            get
            {
                // David: Solo se pueden mover los proyectos de tipo comunidad y que sean editables
                return base.SePuedeCortarArrastrar && TipoProyecto.Equals(TipoProyecto.Comunidad);
            }
        }

        /// <summary>
        /// Obtiene o establece el elemento padre del proyecto
        /// </summary>
        public override IElementoGnoss Padre
        {
            get
            {
                if (mPadre == null)
                {
                    if (FilaProyecto.ProyectoSuperiorID != null && GestorProyectos.ListaProyectos.ContainsKey((Guid)FilaProyecto.ProyectoSuperiorID))
                    {
                        mPadre = GestorProyectos.ListaProyectos[(Guid)FilaProyecto.ProyectoSuperiorID];
                    }
                    else if (FilaProyecto.ProyectoSuperiorID == null && GestorProyectos.GestionOrganizaciones != null && GestorProyectos.GestionOrganizaciones.ListaOrganizaciones.ContainsKey(FilaProyecto.OrganizacionID))
                    {
                        mPadre = GestorProyectos.GestionOrganizaciones.ListaOrganizaciones[FilaProyecto.OrganizacionID];
                    }
                }
                return base.Padre;
            }
            set
            {
                if (value is Proyecto)
                {
                    if (FilaProyecto.ProyectoSuperiorID != ((Proyecto)value).Clave)
                        FilaProyecto.ProyectoSuperiorID = ((Proyecto)value).Clave;
                }
                base.Padre = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de subproyectos del proyecto
        /// </summary>
        public List<Proyecto> ListaSubProyectos
        {
            get
            {
                if (this.mListaSubProyectos == null)
                {
                    mListaSubProyectos = new List<Proyecto>();
                    List<AD.EntityModel.Models.ProyectoDS.Proyecto> subProyectos = GestorProyectos.DataWrapperProyectos.ListaProyecto.Where(proy => proy.ProyectoSuperiorID.Equals(FilaProyecto.ProyectoID)).ToList();
                    //foreach (AD.EntityModel.Models.ProyectoDS.Proyecto proyecto in FilaProyecto.GetProyectoRows())
                    foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in subProyectos)
                    {
                        mListaSubProyectos.Add(GestorProyectos.ListaProyectos[filaProy.ProyectoID]);
                    }
                }
                return mListaSubProyectos;
            }
        }

        /// <summary>
        /// Obtiene la lista de categorías del tesauro a las que pertenece el proyecto
        /// </summary>
        public List<CategoriaTesauro> ListaCategoriasTesauro
        {
            get
            {
                if (mListaCategoriasTesauro == null)
                {
                    mListaCategoriasTesauro = new List<CategoriaTesauro>();

                    foreach (ProyectoAgCatTesauro filaProyAgCat in FilaProyecto.ProyectoAgCatTesauro)
                    {
                        if (GestorProyectos.GestionTesauro.ListaCategoriasTesauro.ContainsKey(filaProyAgCat.CategoriaTesauroID))
                        {
                            mListaCategoriasTesauro.Add(GestorProyectos.GestionTesauro.ListaCategoriasTesauro[filaProyAgCat.CategoriaTesauroID]);
                        }
                    }
                }
                return mListaCategoriasTesauro;
            }
        }

        /// <summary>
        /// Obtiene la lista de categorías de tesauro a las que está agregado el proyecto
        /// </summary>
        public SortedList<Guid, CategoriaTesauro> CategoriasTesauro
        {
            get
            {
                SortedList<Guid, CategoriaTesauro> listaCategorias = new SortedList<Guid, CategoriaTesauro>();

                if (GestorProyectos.GestionTesauro != null)
                {
                    foreach (ProyectoAgCatTesauro filaCategoria in FilaProyecto.ProyectoAgCatTesauro)
                    {
                        if (GestorProyectos.GestionTesauro.ListaCategoriasTesauro.ContainsKey(filaCategoria.CategoriaTesauroID) && !listaCategorias.ContainsKey(filaCategoria.CategoriaTesauroID))
                        {
                            listaCategorias.Add(filaCategoria.CategoriaTesauroID, GestorProyectos.GestionTesauro.ListaCategoriasTesauro[filaCategoria.CategoriaTesauroID]);
                        }
                    }
                }
                return listaCategorias;
            }
        }

        /// <summary>
        /// Obtiene la lista de administradores del proyecto
        /// </summary>
        public List<Guid> ListaAdministradoresIDs
        {
            get
            {
                if (mListaAdministradoresIDs == null)
                {
                    mListaAdministradoresIDs = new List<Guid>();

                    foreach (AdministradorProyecto filaAdmin in GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto)
                    {
                        if (filaAdmin.Tipo.Equals((short)TipoRolUsuario.Administrador))
                        {
                            mListaAdministradoresIDs.Add(filaAdmin.UsuarioID);
                        }
                    }
                }
                return mListaAdministradoresIDs;
            }
        }

        /// <summary>
        /// Obtiene la lista de administradores del proyecto
        /// </summary>
        public List<Persona> ListaAdministradores
        {
            get
            {
                if (mListaAdministradores == null)
                {
                    mListaAdministradores = new List<Persona>();

                    foreach (AdministradorProyecto filaAdmin in GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto)
                    {
                        Guid usuarioID = filaAdmin.UsuarioID;
                        List<AD.EntityModel.Models.PersonaDS.Persona> personaRes = GestorProyectos.GestionPersonas.DataWrapperPersonas.ListaPersona.Where(persona => persona.UsuarioID.Equals(usuarioID)).ToList();
                        if (personaRes.Count > 0)
                        {
                            AD.EntityModel.Models.PersonaDS.Persona filaPersona = personaRes.First();
                            mListaAdministradores.Add(GestorProyectos.GestionPersonas.ListaPersonas[filaPersona.PersonaID]);
                        }
                    }
                }
                return mListaAdministradores;
            }
        }

        /// <summary>
        /// Obtiene la fila del proyecto
        /// </summary>
        public AD.EntityModel.Models.ProyectoDS.Proyecto FilaProyecto
        {
            get;
        }

        /// <summary>
        /// Obtiene o establece el nombre del proyecto
        /// </summary>
        public override string Nombre
        {
            get
            {
                return FilaProyecto.Nombre;
            }
            set
            {
                FilaProyecto.Nombre = value;
                base.Nombre = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre corto del proyecto
        /// </summary>
        public string NombreCorto
        {
            get
            {
                if (FilaProyecto.NombreCorto == "mygnoss")
                {
                    return "";
                }
                else
                {
                    return FilaProyecto.NombreCorto;
                }
            }
            set
            {
                FilaProyecto.NombreCorto = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el estado del proyecto
        /// </summary>
        public short Estado
        {
            get
            {
                return FilaProyecto.Estado;
            }
            set
            {
                FilaProyecto.Estado = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaProyecto.ProyectoID;
            }
        }

        /// <summary>
        /// Obtiene el tipo de proyecto
        /// </summary>
        public bool EsProyectoEduca
        {
            get
            {
                return this.TipoProyecto == TipoProyecto.EducacionExpandida || this.TipoProyecto == TipoProyecto.Universidad20;
            }
        }

        /// <summary>
        /// Obtiene verdad si el proyecto es público o de acceso restringido
        /// </summary>
        public bool EsPublico
        {
            get
            {
                return (this.TipoAcceso == TipoAcceso.Publico || this.TipoAcceso == TipoAcceso.Restringido);
            }
        }

        /// <summary>
        /// Obtiene el tipo de proyecto
        /// </summary>
        public TipoProyecto TipoProyecto
        {
            get
            {
                return (TipoProyecto)FilaProyecto.TipoProyecto;
            }
        }

        /// <summary>
        /// Obtiene el tipo de acceso al proyecto
        /// </summary>
        public TipoAcceso TipoAcceso
        {
            get
            {
                return (TipoAcceso)FilaProyecto.TipoAcceso;
            }
        }

        /// <summary>
        /// Obtiene el identificador del proceso vinculado al proyecto
        /// </summary>
        public Guid? ProcesoVinculadoID
        {
            get
            {
                //if (!FilaProyecto.IsProcesoVinculadoIDNull())
                if (!(FilaProyecto.ProcesoVinculadoID == null))
                {
                    return FilaProyecto.ProcesoVinculadoID;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Obtiene el gestor de proyectos
        /// </summary>
        public GestionProyecto GestorProyectos
        {
            get
            {
                return (GestionProyecto)GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la lista de elementos hijos (otros proyectos)
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                if (mHijos == null)
                {
                    mHijos = new List<IElementoGnoss>();
                    List<AD.EntityModel.Models.ProyectoDS.Proyecto> hijos = GestorProyectos.DataWrapperProyectos.ListaProyecto.Where(proy => proy.ProyectoSuperiorID.Equals(FilaProyecto.ProyectoID)).ToList();
                    //foreach (AD.EntityModel.Models.ProyectoDS.Proyecto proyecto in FilaProyecto.GetProyectoRows())
                    foreach (AD.EntityModel.Models.ProyectoDS.Proyecto proyecto in hijos)
                    {
                        Proyecto proy;

                        if (GestorProyectos.ListaProyectos.ContainsKey(proyecto.ProyectoID))
                        {
                            proy = (Proyecto)GestorProyectos.ListaProyectos[proyecto.ProyectoID];
                        }
                        else
                        {
                            proy = new Proyecto(proyecto, GestorProyectos, mLoggingService, mEntityContext);
                            GestorProyectos.ListaProyectos.Add(proyecto.ProyectoID, proy);
                        }
                        proy.Padre = this;
                        mHijos.Add(proy);
                    }
                }
                return mHijos;
            }
        }

        /// <summary>
        /// Obtiene la organización Gnoss del proyecto
        /// </summary>
        public OrganizacionGnoss Organizacion
        {
            get
            {
                if (GestorProyectos.GestionOrganizaciones != null)
                {
                    return (OrganizacionGnoss)GestorProyectos.GestionOrganizaciones.ListaOrganizaciones[FilaProyecto.OrganizacionID];
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la URL del proyecto en el idioma del usuario
        /// </summary>
        public string UrlPropia(string pIdiomaActual)
        {
            if (string.IsNullOrEmpty(pIdiomaActual))
            {
                pIdiomaActual = "es";
            }

            return UtilCadenas.ObtenerUrlPropiaDeIdioma(FilaProyecto.URLPropia, pIdiomaActual);
        }

        /// <summary>
        /// Identidad que ha creado el proyecto.
        /// </summary>
        public Identidad.Identidad IdentidadCreadoraProyecto
        {
            get
            {
                return mIdentidadCreadoraProyecto;
            }
            set
            {
                mIdentidadCreadoraProyecto = value;
            }
        }

        /// <summary>
        /// Número de recursos totales de una comunidad, los públicos + los privados para el usuario actual
        /// </summary>
        public int NumRecusosTotales
        {
            get
            {
                return FilaProyecto.NumeroRecursos.Value;
            }
        }

        /// <summary>
        /// Obtiene si el proyecto es un catálogo o un catalogo no social con un tipo de recurso
        /// </summary>
        public bool EsCatalogo
        {
            get
            {
                return this.TipoProyecto == TipoProyecto.Catalogo || this.TipoProyecto == TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso || this.TipoProyecto == TipoProyecto.CatalogoNoSocial;
            }
        }

        /// <summary>
        /// Obtiene la lista de pestañas
        /// </summary>
        public Dictionary<Guid, ProyectoPestanyaMenu> ListaPestanyasMenu
        {
            get
            {
                if (mListaPestanyasMenu == null)
                    RecargarPestanyasMenu();

                return mListaPestanyasMenu;
            }
        }

        /// <summary>
        /// Devuelve una lista las acciones de los eventos del proyecto
        /// </summary>
        public Dictionary<TipoProyectoEventoAccion, string> ListaTipoProyectoEventoAccion
        {
            get
            {
                Dictionary<TipoProyectoEventoAccion, string> listaTipoProyectoEventoAccion = new Dictionary<TipoProyectoEventoAccion, string>();

                foreach (ProyectoEventoAccion fila in GestorProyectos.DataWrapperProyectos.ListaProyectoEventoAccion.Where(proyecto => proyecto.ProyectoID.Equals(Clave)))
                {
                    listaTipoProyectoEventoAccion.Add((TipoProyectoEventoAccion)fila.Evento, fila.AccionJS);
                }

                return listaTipoProyectoEventoAccion;
            }
        }


        #endregion

        #region Métodos generales

        /// <summary>
        /// Carga las pestañas del menu
        /// </summary>
        public void RecargarPestanyasMenu()
        {
            if (mListaPestanyasMenu == null)
            {
                mListaPestanyasMenu = new Dictionary<Guid, ProyectoPestanyaMenu>();
            }
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaProyectoPestanya in GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu)
            {
                ProyectoPestanyaMenu proyectoPestanya = new ProyectoPestanyaMenu(filaProyectoPestanya, this.GestorProyectos, mLoggingService, mEntityContext);
                mListaPestanyasMenu.Add(proyectoPestanya.Clave, proyectoPestanya);
            }
        }

        /// <summary>
        /// Comprueba si el elemento actual admite el parámetro como hijo
        /// </summary>
        /// <param name="pHijoCandidato">Elemento que se está moviendo</param>
        /// <returns>TRUE si se admite como hijo</returns>
        public override bool AdmiteHijo(IElementoGnoss pHijoCandidato)
        {
            // David: Un proyecto solo admite los subproyectos que sean comunidad, 
            //        Es editable y no es MyGNOSS
            //        El proyecto que se está moviendo es de la misma organización

            if (TipoAcceso.Equals(TipoAcceso.Reservado))
            {
                return !TipoProyecto.Equals(TipoProyecto.MetaComunidad) && pHijoCandidato is Proyecto && ((Proyecto)pHijoCandidato).TipoAcceso.Equals(TipoAcceso.Reservado) && ((Proyecto)pHijoCandidato).FilaProyecto.OrganizacionID.Equals(this.FilaProyecto.OrganizacionID);
            }
            else
            {
                return EsEditable && !TipoProyecto.Equals(TipoProyecto.MetaComunidad) && pHijoCandidato is Proyecto && ((Proyecto)pHijoCandidato).TipoProyecto.Equals(TipoProyecto.Comunidad) && ((Proyecto)pHijoCandidato).FilaProyecto.OrganizacionID.Equals(this.FilaProyecto.OrganizacionID) && !((Proyecto)pHijoCandidato).TipoAcceso.Equals(TipoAcceso.Reservado);
            }
        }

        /// <summary>
        /// Compara dos proyectos por su nombre
        /// </summary>
        /// <param name="x">Proyecto x</param>
        /// <param name="y">Proyecto y</param>
        /// <returns>0 (iguales), 1 (x>y), -1 (y>x)</returns>
        public static int CompararProyectosPorNombre(Proyecto x, Proyecto y)
        {
            return x.Nombre.CompareTo(y.Nombre);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pusuarioID"></param>
        /// <returns></returns>
        public bool EsAdministradorUsuario(Guid pusuarioID)
        {
            List<AdministradorProyecto> filasAdministrador = GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(pusuarioID) && adminProy.ProyectoID.Equals(Clave)).ToList();//("UsuarioID='" + pusuarioID + "' AND ProyectoID='" + Clave + "'");

            if (filasAdministrador.Count > 0 && filasAdministrador[0].Tipo == (short)TipoRolUsuario.Administrador)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el orden del nivel de certificación del proyecto
        /// </summary>
        /// <param name="pNivelCertificacionId">Identificador del nivel de certificación del proyecto</param>
        /// <returns>Short con el orden del nivel de certificación. Si no lo encuentra devuelve -1</returns>
        public short ObtenerOrdenDeNivelCertificacion(Guid pNivelCertificacionId)
        {
            NivelCertificacion filaNivelCertificacion = GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion.Find(nivelCertificacion => nivelCertificacion.NivelCertificacionID.Equals(pNivelCertificacionId));//;FindByNivelCertificacionID(pNivelCertificacionId);

            if (filaNivelCertificacion != null)
            {
                return filaNivelCertificacion.Orden;
            }

            return -1;
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
        ~Proyecto()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                        if (this.mListaAdministradores != null)
                            this.mListaAdministradores.Clear();

                        if (this.mListaCategoriasTesauro != null)
                            this.mListaCategoriasTesauro.Clear();

                        if (this.mListaSubProyectos != null)
                            this.mListaSubProyectos.Clear();

                        if (this.mListaTags != null)
                            this.mListaTags.Clear();
                    }

                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase
                    this.mListaAdministradores = null;
                    this.mListaCategoriasTesauro = null;
                    this.mListaSubProyectos = null;
                    this.mListaTags = null;
                }
                finally
                {
                    // Llamo al dispose de la clase base
                    //base.Dispose(disposing);
                }
            }
        }

        #endregion
    }
}
