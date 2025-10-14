using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.InterfacesOpen;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open
{
    public class RDFSearchOpen : IRDFSearch
    {
        public RDFSearchOpen(EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IHttpContextAccessor httpContextAccessor, GnossCache gnossCache, EntityContextBASE entityContextBASE, ILogger<RDFSearchOpen> logger, ILoggerFactory loggerFactory) : base(entityContext, loggingService, configService, redisCacheWrapper, virtuosoAD, servicesUtilVirtuosoAndReplication, httpContextAccessor, gnossCache, entityContextBASE, logger, loggerFactory)
        {

        }

        public override byte[] CargarRDFListaResultados(ResultadoModel pListaResultados, Proyecto ProyectoSeleccionado, Ontologia OntologiaGnoss, string IdiomaUsuario, UtilSemCms UtilSemCms, UtilIdiomas UtilIdiomas, string BaseURLIdioma, string BaseURLContent, string UrlPerfil, string UrlPagina, GnossUrlsSemanticas UrlsSemanticas, Guid ProyectoPestanyaActual, string BaseURLFormulariosSem, Identidad IdentidadActual, GnossIdentity UsuarioActual, Guid ProyectoPrincipalUnico, List<string> ListaItemsBusquedas)
        {
            return Array.Empty<byte>();
        }
    }
}
