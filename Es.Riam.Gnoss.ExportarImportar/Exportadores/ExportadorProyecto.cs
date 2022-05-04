using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
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
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Es.Riam.Gnoss.ExportarImportar.Exportadores
{
    /// <summary>
    /// exportador de proyectos
    /// </summary>
    public class ExportadorProyecto : ExportadorElementoGnoss, IDisposable
    {

        #region Constructor

        /// <summary>
        /// Crea un nuevo exportador de grupo
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        public ExportadorProyecto(Ontologia pOntologia, string pIdiomaUsuario,  LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, UtilSemCms utilSemCms, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pOntologia, pIdiomaUsuario, loggingService, entityContext, configService, redisCacheWrapper, utilSemCms, servicesUtilVirtuosoAndReplication)
        {
        }

        #endregion

        #region Métodos de proyecto

        /// <summary>
        /// Obtiene la entidad pEntidad y todas sus relaciones.
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener.</param>
        /// <param name="pElementoGnoss">Elemento gnoss que representa la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra.</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public override void ObtenerEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, bool pEspecializacion, GestionGnoss pGestor)
        {
            if (pElementoGnoss is Elementos.ServiciosGenerales.Proyecto)
                base.ObtenerEntidadProyecto(pEntidad, pElementoGnoss, ((Elementos.ServiciosGenerales.Proyecto)pElementoGnoss).FilaProyecto, pEspecializacion, pGestor);
            else
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((ElementoGnoss)pElementoGnoss).FilaElemento, pEspecializacion, pGestor);
        }

        /// <summary>
        /// Obtiene los atributos.
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la que se buscan sus atributos.</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        public override void ObtenerAtributosEntidad(ElementoOntologia pEntidadBuscada, IElementoGnoss pElementoGnoss)
        {
            switch (pEntidadBuscada.TipoEntidad)
            {
                case TipoElementoGnoss.Proyecto:
                    UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Elementos.ServiciosGenerales.Proyecto)pElementoGnoss).FilaProyecto);
                    UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((GestionProyecto)((Elementos.ServiciosGenerales.Proyecto)pElementoGnoss).GestorGnoss).ParametrosGenerales);
                    break;
            }
            base.ObtenerAtributosEntidad(pEntidadBuscada, pElementoGnoss);
        }

        /// <summary>
        /// Generaliza un elemento.
        /// </summary>
        /// <param name="pEntidad">Entidad a generalizar.</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pFilaElemento">Fila de elemento que representa la entidad.</param>
        /// <param name="pGestor">Gestor de estructura.</param>
        protected override void GeneralizarEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, object pFilaElemento, GestionGnoss pGestor)
        {

        }

        /// <summary>
        /// Trata los casos especiales.
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la propiedad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pPropiedad">Propiedad que relaciona la entidad con otra entidad.</param>
        /// <param name="pGestor">Gestor.</param>
        /// <returns></returns>
        protected override bool TratarCasoEspecial(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            return true;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~ExportadorProyecto()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion
    }
}
