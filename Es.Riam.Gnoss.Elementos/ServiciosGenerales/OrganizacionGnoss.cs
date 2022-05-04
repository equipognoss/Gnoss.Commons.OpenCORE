using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Organizaci�n GNOSS
    /// </summary>
    public class OrganizacionGnoss : Organizacion, ICortarCopiarPegar, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Lista de los proyectos de la organizaci�n GNOSS
        /// </summary>
        private Dictionary<Guid, Proyecto> mListaProyectos = null;

        /// <summary>
        /// Fila de organizaci�n GNOSS
        /// </summary>
        private AD.EntityModel.Models.OrganizacionDS.OrganizacionGnoss mOrganizacionGnossRow = null;

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor vac�o
        /// </summary>
        public OrganizacionGnoss(LoggingService loggingService, EntityContext entityContext)
            : base(loggingService)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param OrganizacionGnoss="pOrganizacion">Fila de organizaci�n</param>
        /// <param OrganizacionGnoss="pOrganizacionGnossRow">Fila de organizaci�n GNOSS</param>
        public OrganizacionGnoss(AD.EntityModel.Models.OrganizacionDS.Organizacion pOrganizacion, AD.EntityModel.Models.OrganizacionDS.OrganizacionGnoss pOrganizacionGnossRow, GestionOrganizaciones pGestor, LoggingService loggingService, EntityContext entityContext) 
            : base(pOrganizacion, pGestor, loggingService)

        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            mOrganizacionGnossRow = pOrganizacionGnossRow;
        }

        #endregion

        #region M�todos Privados

        /// <summary>
        /// Carga los proyectos de la organizaci�n GNOSS
        /// </summary>
        private void CargarProyectosDeOrganizacionGnoss()
        {
            if (GestorOrganizaciones.GestorProyectos != null)
            {
                mListaProyectos = new Dictionary<Guid, Proyecto>();

                foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProyecto in GestorOrganizaciones.GestorProyectos.DataWrapperProyectos.ListaProyecto.Where(proyecto => proyecto.OrganizacionID.Equals(Clave)).OrderBy(proyecto => proyecto.Nombre))//Select("OrganizacionID = '" + Clave+"'"  ,"Nombre"))
                {
                    mListaProyectos.Add(filaProyecto.ProyectoID, GestorOrganizaciones.GestorProyectos.ListaProyectos[filaProyecto.ProyectoID]);
                }
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene los hijos de la organizaci�n (proyectos que cuelgan de la organizaci�n directamente)
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                if (mHijos == null)
                {
                    mHijos = new List<IElementoGnoss>();

                    if (GestorOrganizaciones.GestorProyectos != null)
                    {
                        foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProyecto in GestorOrganizaciones.GestorProyectos.DataWrapperProyectos.ListaProyecto.Where(proyecto => proyecto.ProyectoSuperiorID == null && proyecto.OrganizacionID.Equals(Clave)).ToList())//Select("ProyectoSuperiorID is NULL AND OrganizacionID = '" + Clave +"'"))
                        {
                            Proyecto hijo = null;

                            if (GestorOrganizaciones.GestorProyectos.ListaProyectos.ContainsKey(filaProyecto.ProyectoID))
                            {
                                hijo = GestorOrganizaciones.GestorProyectos.ListaProyectos[filaProyecto.ProyectoID];
                            }
                            else
                            {
                                hijo = new Proyecto(filaProyecto, GestorOrganizaciones.GestorProyectos, mLoggingService, mEntityContext);
                                GestorOrganizaciones.GestorProyectos.ListaProyectos.Add(filaProyecto.ProyectoID, hijo);
                            }
                            mHijos.Add(hijo);
                        }
                    }
                }
                return mHijos;
            }
        }

        /// <summary>
        /// Comprueba si la organizaci�n admite como hijo al elemento pasado por par�metro
        /// </summary>
        /// <param name="pHijoCandidato">Elemento a pegar</param>
        /// <returns>TRUE si el elemento actual admite a pHijoCandidato como hijo, FALSE en caso contrario</returns>
        public override bool AdmiteHijo(IElementoGnoss pHijoCandidato)
        {
            if (pHijoCandidato is Proyecto)
            {
                Proyecto proyectoHijo = (Proyecto)pHijoCandidato;

                return EsEditable && (pHijoCandidato.Padre != this) && proyectoHijo.TipoProyecto.Equals(TipoProyecto.Comunidad) && proyectoHijo.FilaProyecto.OrganizacionID.Equals(Clave);
            }
            return base.AdmiteHijo(pHijoCandidato);
        }

        /// <summary>
        /// Obtiene la lista de proyectos de la organizaci�n GNOSS
        /// </summary>
        public Dictionary<Guid, Proyecto> ListaProyectosDeOrganizacionGnoss
        {
            get
            {
                if (mListaProyectos == null)
                {
                    CargarProyectosDeOrganizacionGnoss();
                }
                return mListaProyectos;
            }
        }

        /// <summary>
        /// Obtiene la fila de la organizaci�n GNOSS
        /// </summary>
        public AD.EntityModel.Models.OrganizacionDS.OrganizacionGnoss FilaOrganizacionGnoss
        {
            get
            {
                return mOrganizacionGnossRow;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~OrganizacionGnoss()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se est� llamando desde el Dispose()</param>
        protected override void Dispose(bool pDisposing)
        {
            if (!this.mDisposed)
            {
                this.mDisposed = true;

                try
                {
                    if (pDisposing)
                    {
                        //this.mOrganizacionesColaboradoras.Clear();
                        //this.mOrganizacionesProveedoras.Clear();
                    }
                    //Libero todos los recursos nativos sin administrar que he a�adido a esta clase
                    mListaProyectos = null;
                    //this.mOrganizacionesColaboradoras = null;
                }
                finally
                {
                    // Llamo al dispose de la clase base
                    base.Dispose(pDisposing);
                }
            }
        }

        #endregion
    }
}
