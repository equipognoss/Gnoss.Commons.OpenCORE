using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Es.Riam.Gnoss.Elementos.Facetado
{
    /// <summary>
    /// Gestor de identidades
    /// </summary>
    [Serializable]
    public class GestionFacetas : GestionGnoss, ISerializable, IDisposable
    {
        #region Miembros

        private List<Faceta> mListaFacetas = null;
        private List<string> mOntologiasNoBuscables = null;      
        private Dictionary<string, Faceta> mListaFacetasPorClave = null;
        private bool mGestorCargado = false;
        private LoggingService mLoggingService;

        #endregion

        #region Constructores

        public GestionFacetas(DataWrapperFacetas pFacetaDW, LoggingService loggingService)
            : base(pFacetaDW, loggingService)
        {
            mLoggingService = loggingService;
            MontarFacetasHome = false;
        }

        #endregion

        #region Propiedades

        
        public DataWrapperFacetas FacetasDW
        {
            get
            {
                return (DataWrapperFacetas)DataWrapper;
        }
        }

        public Dictionary<string, Faceta> ListaFacetasPorClave
        {
            get
            {
                if (mListaFacetasPorClave == null)
                {
                    CargarGestorFacetas();
                }
                return this.mListaFacetasPorClave;
            }
        }

        /// <summary>
        /// Obtiene la lista de todas las Facetas
        /// </summary>
        public List<Faceta> ListaFacetas
        {
            get
            {
                if (mListaFacetas == null)
                {
                    CargarGestorFacetas();
                }
                return mListaFacetas;
            }
        }

        public List<string> OntologiasNoBuscables {
            get 
            {
                return mOntologiasNoBuscables;
            }
        }

        /// <summary>
        /// Obtiene o establece si se deben montar las facetas de la home
        /// </summary>
        public bool MontarFacetasHome
        {
            get;
            set;
        }

        #endregion

        #region Métodos generales

        #region Publicos

        /// <summary>
        /// Carga las facetas en ListaFacetas y también en DiccionarioFacetas
        /// </summary>
        public void CargarGestorFacetas(List<string> pListaRdfType = null)
        {
            this.mListaFacetas = new List<Faceta>();
            this.mListaFacetasPorClave = new Dictionary<string, Faceta>();

            if (MontarFacetasHome)
            {
                foreach (FacetaFiltroHome filaFiltroHome in this.FacetasDW.ListaFacetaFiltroHome)
                {
                    Faceta faceta = new Faceta(filaFiltroHome, this, mLoggingService);

                    bool omitirCargado = mListaFacetas.Exists(facetaMismoFiltro => facetaMismoFiltro.FilaElementoEntity is FacetaFiltroHome && ((FacetaFiltroHome)facetaMismoFiltro.FilaElementoEntity).Filtro.Equals(((FacetaFiltroHome)faceta.FilaElementoEntity).Filtro));

                    if (!omitirCargado)
                    {
                        this.mListaFacetas.Add(faceta);
                        if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFaceta))
                        {
                            this.mListaFacetasPorClave.Add(faceta.ClaveFaceta, faceta);
                        }
                    }
                }

                foreach (FacetaHome filaHome in FacetasDW.ListaFacetaHome)
                {
                    Faceta faceta = new Faceta(filaHome, this, mLoggingService);
                    if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFaceta))
                    {
                        this.mListaFacetas.Add(faceta);
                        this.mListaFacetasPorClave.Add(faceta.ClaveFaceta, faceta);
                    }
                }
            }

            if (!MontarFacetasHome || mListaFacetas.Count==0)
            {

                foreach (FacetaFiltroProyecto filaFiltroProyecto in this.FacetasDW.ListaFacetaFiltroProyecto)
                {
                    Faceta faceta = new Faceta(filaFiltroProyecto, this, mLoggingService);

                    bool omitirCargado = mListaFacetas.Exists(facetaMismoFiltro => facetaMismoFiltro.FilaElementoEntity is FacetaFiltroProyecto && ((string)UtilReflection.GetValueReflection(facetaMismoFiltro.FilaElementoEntity, "Filtro")).Equals((string)UtilReflection.GetValueReflection(faceta.FilaElementoEntity, "Filtro")));

                    if (!omitirCargado)
                    {
                        this.mListaFacetas.Add(faceta);
                        if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFaceta))
                        {
                            this.mListaFacetasPorClave.Add(faceta.ClaveFaceta, faceta);
                        }
                    }
                }

                if (pListaRdfType != null)
                {
                    foreach (string rdfType in pListaRdfType)
                    {
                        foreach (FacetaObjetoConocimientoProyecto filaFacetaObjetoConocimientoProyecto in this.FacetasDW.ListaFacetaObjetoConocimientoProyecto.Where(item => item.ObjetoConocimiento.ToLower().Equals(rdfType.ToLower())))
                        {
                            Faceta faceta = new Faceta(filaFacetaObjetoConocimientoProyecto, this, mLoggingService);
                            if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFacetaEntity))
                            {
                                this.mListaFacetas.Add(faceta);
                                this.mListaFacetasPorClave.Add(faceta.ClaveFacetaEntity, faceta);
                            }
                        }
                    }
                }

                foreach (FacetaObjetoConocimientoProyecto filaFacetaObjetoConocimientoProyecto in this.FacetasDW.ListaFacetaObjetoConocimientoProyecto)
                {
                    Faceta faceta = new Faceta(filaFacetaObjetoConocimientoProyecto, this, mLoggingService);
                    if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFacetaEntity))
                    {
                        this.mListaFacetas.Add(faceta);
                        this.mListaFacetasPorClave.Add(faceta.ClaveFacetaEntity, faceta);
                    }
                }

                foreach (AD.EntityModel.Models.Faceta.FacetaObjetoConocimiento filaFacetaObjetoConocimiento in this.FacetasDW.ListaFacetaObjetoConocimiento)
                {
                    Faceta faceta = new Faceta(filaFacetaObjetoConocimiento, this, mLoggingService);
                    if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFacetaEntity))
                    {
                        this.mListaFacetas.Add(faceta);
                        this.mListaFacetasPorClave.Add(faceta.ClaveFacetaEntity, faceta);
                    }
                }
            }

            //ordenamos la lista por el Orden
            ReordenarFacetas();

            if (!mGestorCargado)
            {
                mGestorCargado = true;
                CargarOntologiasProyecto();
            }
        }

        /// <summary>
        /// Reordena la lista de facetas por el campo Orden
        /// </summary>
        public void ReordenarFacetas()
        {
            this.mListaFacetas = this.mListaFacetas.OrderBy(faceta => faceta.Orden).ToList();
        }

        private void CargarOntologiasProyecto()
        {
            this.mOntologiasNoBuscables = new List<string>();

            foreach (AD.EntityModel.Models.Faceta.OntologiaProyecto fila in FacetasDW.ListaOntologiaProyecto)
            {
                if (!fila.EsBuscable)
                {
                    mOntologiasNoBuscables.Add(fila.OntologiaProyecto1);  
                }
            }
        }
        
        /// <summary>
        /// Obtiene una lista de facetas de un objeto de conocimiento para un proyecto determinado
        /// </summary>
        /// <param name="ObjetoConocimiento">Objeto de conocimiento</param>
        /// <returns>Lista de facetas</returns>
        public List<Faceta> ListaFacetasItemBuscado(string ObjetoConocimiento)
        {
            List<Faceta> listaFacetasItemBuscado = (List<Faceta>)ListaFacetas.Where(faceta => faceta.ObjetoConocimiento.Equals(ObjetoConocimiento)).ToList();
            return listaFacetasItemBuscado;
        }

        public List<Faceta> ObtenerFacetasDeTipo(TipoPropiedadFaceta pTipo)
        {
            return ListaFacetas.Where(faceta => faceta.TipoPropiedad.Equals(pTipo)).ToList();
        }

        #endregion Publicos

        #endregion Métodos generales

    }
}
