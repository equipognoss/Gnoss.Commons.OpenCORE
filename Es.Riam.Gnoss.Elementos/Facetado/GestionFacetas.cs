using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using Serilog.Core;
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
        #endregion

        #region Constructores

        public GestionFacetas() { }

        public GestionFacetas(DataWrapperFacetas pFacetaDW)
            : base(pFacetaDW)
        {
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
                return mListaFacetasPorClave;
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

        public List<string> OntologiasNoBuscables
        {
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
            mListaFacetas = new List<Faceta>();
            mListaFacetasPorClave = new Dictionary<string, Faceta>();

            if (MontarFacetasHome)
            {
                foreach (FacetaFiltroHome filaFiltroHome in FacetasDW.ListaFacetaFiltroHome)
                {
                    Faceta faceta = new Faceta(filaFiltroHome, this);

                    bool omitirCargado = mListaFacetas.Exists(facetaMismoFiltro => facetaMismoFiltro.FilaElementoEntity is FacetaFiltroHome && ((FacetaFiltroHome)facetaMismoFiltro.FilaElementoEntity).Filtro.Equals(((FacetaFiltroHome)faceta.FilaElementoEntity).Filtro));

                    if (!omitirCargado)
                    {
                        mListaFacetas.Add(faceta);
                        if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFaceta))
                        {
                            mListaFacetasPorClave.Add(faceta.ClaveFaceta, faceta);
                        }
                    }
                }

                foreach (FacetaHome filaHome in FacetasDW.ListaFacetaHome)
                {
                    Faceta faceta = new Faceta(filaHome, this);
                    if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFaceta))
                    {
                        mListaFacetas.Add(faceta);
                        mListaFacetasPorClave.Add(faceta.ClaveFaceta, faceta);
                    }
                }
            }

            if (!MontarFacetasHome || mListaFacetas.Count == 0)
            {

                foreach (FacetaFiltroProyecto filaFiltroProyecto in FacetasDW.ListaFacetaFiltroProyecto)
                {
                    Faceta faceta = new Faceta(filaFiltroProyecto, this);

                    bool omitirCargado = mListaFacetas.Exists(facetaMismoFiltro => facetaMismoFiltro.FilaElementoEntity is FacetaFiltroProyecto && ((string)UtilReflection.GetValueReflection(facetaMismoFiltro.FilaElementoEntity, "Filtro")).Equals((string)UtilReflection.GetValueReflection(faceta.FilaElementoEntity, "Filtro")) && ((string)UtilReflection.GetValueReflection(facetaMismoFiltro.FilaElementoEntity, "Faceta")).Equals((string)UtilReflection.GetValueReflection(faceta.FilaElementoEntity, "Faceta")));

                    if (!omitirCargado)
                    {
                        mListaFacetas.Add(faceta);
                        if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFaceta))
                        {
                            mListaFacetasPorClave.Add(faceta.ClaveFaceta, faceta);
                        }
                    }
                }

                if (pListaRdfType != null)
                {
                    foreach (string rdfType in pListaRdfType)
                    {
                        foreach (FacetaObjetoConocimientoProyecto filaFacetaObjetoConocimientoProyecto in FacetasDW.ListaFacetaObjetoConocimientoProyecto.Where(item => item.ObjetoConocimiento.ToLower().Equals(rdfType.ToLower())))
                        {
                            Faceta faceta = new Faceta(filaFacetaObjetoConocimientoProyecto, this);
                            if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFacetaEntity))
                            {
                                mListaFacetas.Add(faceta);
                                mListaFacetasPorClave.Add(faceta.ClaveFacetaEntity, faceta);
                            }
                        }
                    }
                }

                foreach (FacetaObjetoConocimientoProyecto filaFacetaObjetoConocimientoProyecto in FacetasDW.ListaFacetaObjetoConocimientoProyecto)
                {
                    Faceta faceta = new Faceta(filaFacetaObjetoConocimientoProyecto, this);
                    if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFacetaEntity))
                    {
                        mListaFacetas.Add(faceta);
                        mListaFacetasPorClave.Add(faceta.ClaveFacetaEntity, faceta);
                    }
                }

                List<FacetaObjetoConocimiento> listaFacetaObjetoConocimiento = FacetasDW.ListaFacetaObjetoConocimiento;

                if (pListaRdfType != null && pListaRdfType.Count > 0)
                {
                    listaFacetaObjetoConocimiento = FacetasDW.ListaFacetaObjetoConocimiento.Where(item => pListaRdfType.Contains(item.ObjetoConocimiento)).ToList();
                }

                foreach (FacetaObjetoConocimiento filaFacetaObjetoConocimiento in listaFacetaObjetoConocimiento)
                {
                    Faceta faceta = new Faceta(filaFacetaObjetoConocimiento, this);
                    if (faceta != null && !mListaFacetasPorClave.ContainsKey(faceta.ClaveFacetaEntity))
                    {
                        string p = faceta.ObjetoConocimiento;

                        mListaFacetas.Add(faceta);
                        mListaFacetasPorClave.Add(faceta.ClaveFacetaEntity, faceta);
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
            mListaFacetas = mListaFacetas.OrderBy(faceta => faceta.Orden).ToList();
        }

        private void CargarOntologiasProyecto()
        {
            mOntologiasNoBuscables = new List<string>();

            foreach (OntologiaProyecto fila in FacetasDW.ListaOntologiaProyecto)
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
            return ListaFacetas.Where(faceta => faceta.ObjetoConocimiento.Equals(ObjetoConocimiento)).ToList();           
        }

        public List<Faceta> ObtenerFacetasDeTipo(TipoPropiedadFaceta pTipo)
        {
            return ListaFacetas.Where(faceta => faceta.TipoPropiedad.Equals(pTipo)).ToList();
        }

        #endregion Publicos

        #endregion Métodos generales
    }
}
