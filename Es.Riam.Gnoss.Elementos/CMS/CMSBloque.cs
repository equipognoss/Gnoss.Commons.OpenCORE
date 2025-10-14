using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.CMS
{
    /// <summary>
    /// CMSBloque
    /// </summary>
    public class CMSBloque : ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Obtiene ls lita de componentes
        /// </summary>
        public SortedDictionary<short, CMSComponente> mComponentes = null;

        /// <summary>
        /// Obtiene las propiedades de un componente en un determinado bloque
        /// </summary>
        public Dictionary<Guid, Dictionary<TipoPropiedadCMS, string>> mPropiedadesComponentesBloque = null;


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de bloque de CMS y del gestor de CMS pasados por parámetro
        /// </summary>
        /// <param name="pFilaBloque">Fila de bloque</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSBloque(AD.EntityModel.Models.CMS.CMSBloque pFilaBloque, GestionCMS pGestorCMS)
            : base(pFilaBloque, pGestorCMS)
        {
        }

        #endregion

        /// <summary>
        /// Carga los subBloquescategorias
        /// </summary>
        public void CargarSubBloques()
        {

            List<CMSBloque> filasOrdenadas = this.GestorCMS.ListaBloques.Values.Where(item => item.BloquePadreID.HasValue && item.BloquePadreID.Value.Equals(FilaBloque.BloqueID)).OrderBy(item => item.Orden).ToList();

            mHijos = new List<IElementoGnoss>();

            foreach (CMSBloque bloque in filasOrdenadas)
            {
                CMSBloque bloq = this.GestorCMS.ListaBloques[bloque.Clave];
                bloq.Padre = this;
                if (!mHijos.Contains(bloq))
                {
                    mHijos.Add(bloq);
                }
                bloq.CargarSubBloques();
            }
        }

        /// <summary>
        /// Carga los Componentes
        /// </summary>
        public void CargarComponentes()
        {
            List<AD.EntityModel.Models.CMS.CMSBloqueComponente> filasOrdenadas = this.GestorCMS.CMSDW.ListaCMSBloqueComponente.Where(cmsBloqueCom => cmsBloqueCom.BloqueID.Equals(FilaBloque.BloqueID)).OrderBy(item => item.Orden).ToList();//FilaBloque.CMSBloqueComponente.OrderBy(item => item.Orden).ToList();

            mComponentes = new SortedDictionary<short, CMSComponente>();

            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente in filasOrdenadas)
            {
                if (this.GestorCMS.ListaComponentes.ContainsKey(filaBloqueComponente.ComponenteID))
                {
                    CMSComponente componente = this.GestorCMS.ListaComponentes[filaBloqueComponente.ComponenteID];
                    if (!mComponentes.ContainsValue(componente))
                    {
                        mComponentes.Add((short)mComponentes.Count, componente);
                    }
                }
            }
        }

        /// <summary>
        /// Carga las propiedades de un componente en un determinado bloque
        /// </summary>
        public void CargarPropiedadesComponentesBloque()
        {
            mPropiedadesComponentesBloque = new Dictionary<Guid, Dictionary<TipoPropiedadCMS, string>>();
            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaBloqueComponentePropiedadComponente in this.GestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Where(item => item.BloqueID.Equals(this.Clave)).ToList())//"BloqueID='"+this.Clave.ToString()+"'"))
            {
                if (!mPropiedadesComponentesBloque.ContainsKey(filaBloqueComponentePropiedadComponente.ComponenteID))
                {
                    Dictionary<TipoPropiedadCMS, string> listaPropiedades = new Dictionary<TipoPropiedadCMS, string>();
                    mPropiedadesComponentesBloque.Add(filaBloqueComponentePropiedadComponente.ComponenteID, listaPropiedades);
                }

                if (!mPropiedadesComponentesBloque[filaBloqueComponentePropiedadComponente.ComponenteID].ContainsKey((TipoPropiedadCMS)(filaBloqueComponentePropiedadComponente.TipoPropiedadComponente)))
                {
                    mPropiedadesComponentesBloque[filaBloqueComponentePropiedadComponente.ComponenteID].Add((TipoPropiedadCMS)(filaBloqueComponentePropiedadComponente.TipoPropiedadComponente), filaBloqueComponentePropiedadComponente.ValorPropiedad);
                }
            }
        }

        #region Propiedades

        /// <summary>
        /// Obtiene el orden del bloque
        /// </summary>
        public short Orden
        {
            get
            {
                return FilaBloque.Orden;
            }
        }

        /// <summary>
        /// Obtiene el los atributos del componente
        /// </summary>
        public Dictionary<string, string> Atributos
        {
            get
            {
                string[] atributos = FilaBloque.Estilos.Split(new string[] { "~~~" }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, string> Atr = new Dictionary<string, string>();
                foreach (string atributo in atributos)
                {
                    string[] clavevalor = atributo.Split(new string[] { "---" }, StringSplitOptions.RemoveEmptyEntries);
                    if (clavevalor.Length == 2)
                    {
                        Atr.Add(clavevalor[0], clavevalor[1]);
                    }
                    else
                    {
                        Atr.Add("class", clavevalor[0]);
                    }
                }
                return Atr;
            }
        }

        /// <summary>
        /// Devuelve el gestor de CMS que contiene al componente
        /// </summary>
        public GestionCMS GestorCMS
        {
            get
            {
                return (GestionCMS)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila del bloque
        /// </summary>
        public AD.EntityModel.Models.CMS.CMSBloque FilaBloque
        {
            get
            {
                return (AD.EntityModel.Models.CMS.CMSBloque)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene la lista de los hijos del bloque
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                if (mHijos == null)
                    CargarSubBloques();
                return mHijos;
            }
        }

        /// <summary>
        /// Obtiene el ID del bloque
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaBloque.BloqueID;
            }
        }

        /// <summary>
        /// Obtiene el ID del bloque padre
        /// </summary>
        public Guid? BloquePadreID
        {
            get
            {
                if (!FilaBloque.BloquePadreID.HasValue)
                {
                    return null;
                }
                return FilaBloque.BloquePadreID;
            }
        }

        /// <summary>
        /// Obtiene si es borrador
        /// </summary>
        public bool Borrador
        {
            get
            {
                return FilaBloque.Borrador;
            }
        }

        /// <summary>
        /// Obtiene o establece el componente que contiene
        /// </summary>
        public SortedDictionary<short, CMSComponente> Componentes
        {
            get
            {
                if (mComponentes == null)
                {
                    CargarComponentes();
                }
                return mComponentes;
            }
        }

        /// <summary>
        /// Obtiene las propiedades de un componente en un determinado bloque
        /// </summary>
        public Dictionary<Guid, Dictionary<TipoPropiedadCMS, string>> PropiedadesComponentesBloque
        {
            get
            {
                if (mPropiedadesComponentesBloque == null)
                {
                    CargarPropiedadesComponentesBloque();
                }
                return mPropiedadesComponentesBloque;
            }
        }

        /// <summary>
        /// Obtiene la ubicacion del componente
        /// </summary>
        public short TipoUbicacion
        {
            get
            {
                return FilaBloque.Ubicacion;
            }
        }

        #endregion
    }
}
