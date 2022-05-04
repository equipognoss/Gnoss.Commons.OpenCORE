using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.Util.General;

namespace Es.Riam.Gnoss.Elementos.CMS
{
    /// <summary>
    /// CMSPagina
    /// </summary>
    public class CMSPagina : ElementoGnoss
    {

        #region Constructor
        
        /// <summary>
        /// Constructor a partir de una fila de pagina de CMS y del gestor de CMS pasados por par�metro
        /// </summary>
        /// <param name="pFilaPagina">Fila de pagina</param>
        /// <param name="pGestorCMS">Gestor de CMS</param>
        public CMSPagina(AD.EntityModel.Models.CMS.CMSPagina pFilaPagina, GestionCMS pGestorCMS, LoggingService loggingService)
            : base(pFilaPagina, pGestorCMS, loggingService)
        {            
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establedce si la p�gina est� activa
        /// </summary>
        public bool Activa
        {
            get
            {
                return Filapagina.Activa;
            }
            set 
            {
                Filapagina.Activa = value;
            }
        }

        /// <summary>
        /// Obtiene el tipo de ubicacion de la p�gina
        /// </summary>
        public TipoUbicacionCMS TipoUbicacion
        {
            get
            {
                return (TipoUbicacionCMS)Filapagina.Ubicacion;
            }
        }

        /// <summary>
        /// Obtiene o establece si se mostrara toda la p�gina o solo el cuerpo
        /// </summary>
        public bool MostrarSoloCuerpo
        {
            get
            {
                return Filapagina.MostrarSoloCuerpo;
            }
            set
            {
                Filapagina.MostrarSoloCuerpo = value;
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
        /// Obtiene la fila de la p�gina
        /// </summary>
        public AD.EntityModel.Models.CMS.CMSPagina Filapagina
        {
            get
            {
                return (AD.EntityModel.Models.CMS.CMSPagina)FilaElementoEntity;
            }
        }

        #endregion

    }

}
