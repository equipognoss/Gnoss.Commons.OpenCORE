using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{

    public enum ApplicationType
    {
		SERVICIO_ONTOLOGIAS, 
	    SERVICIO_INTERNO,
	    SERVICIO_DOCUMENTOS,
	    SERVICIO_API,
        SERVICIO_OAUTH,
        SERVICIO_RELATED_VIRTUOSO,
	    SERVICIO_SEARCH_GRAPH_GENERATOR,
	    RABBITMQ,
	    SERVICIO_REPLICACION_VIRTUOSO,
        SERVICIO_REPLICACION_BIDIRECCIONAL_VIRTUOSO,
        HAPROXY, 
        SERVICIO_THUMBNAIL_GENERATOR, 
        SERVICIO_AUTOCOMPLETE_API_LUCENE,
	    SERVICIO_VISIT_REGISTRY,
	    SERVICIO_VISIT_CLUSTER, 
	    REDIS, 
        SERVICIO_LOGIN,
        SERVICIO_DEPLOY, 
        SERVICIO_IDENTITYSERVER, 
        ELIMINAR_RECURSO, 
        SERVICIO_FACETAS,
        SERVICIO_RESULTS, 
        SERVICIO_AUTOCOMPLETE,
        SERVICIO_AUTOCOMPLETE_GENERATOR,
        SERVICIO_VIRTUOSO, 
        SERVICIO_LABELER,
        SERVICIO_CACHEREFRESH, 
        SERVICIO_COMMUNITYWALL, 
        SERVICIO_DISTRIBUTOR, 
        SERVICIO_MAIL,
        SERVICIO_SOCIALCACHEREFRESH, 
        SERVICIO_USERWALL


    }


    public enum ApplicationStatus
    {
        OK, 
        KO, 
        UNDEFINED
    }

    [Serializable]
    public class EstadoSistemasViewModel
    {
        // Códigos de error
        public const int NO_TESTEADO = 0;
        public const int KO = -1;
        public const int OK = 1;
        public const int NO_CONFIGURADO = 2;
        public const int DETENIDO = 3;
        public const int KO_GRAPH_GENERATOR = 4;
        public const int KO_RABBIT = 5;
        public const int KO_SERVICIO_AFINIDAD = 6;
        public const int KO_CONFIG_RELATEDVIRTUOSO_WEB = 7;

        // Estado de los servicios
        public int archivos { get; set; }
        public int sInterno { get; set; }
        public int documentos { get; set; }
        public int apiOAuth { get; set; }
        public int relatedVirtuoso { get; set; }
        public int rabbitGraphGenerator { get; set; }
        public int graphGenerator { get; set; }
        public int virtuoso { get; set; }
        public int replicacionVirtuoso { get; set; }
        public int HAProxy { get; set; }
        public int generadorMiniaturas { get; set; }
        public int luceneAutocomplete { get; set; }
        public int visitas { get; set; }
        public int liveUsuarios { get; set; }
        public int redis { get; set; }
        public int ontologies { get; set; }

        //Descripción de los errores
        public string descErrorArchivos { get; set; }
        public string descErrorSInterno { get; set; }
        public string descErrorDocumentos { get; set; }
        public string descErrorApiOAuth { get; set; }
        public string descErrorRelatedVirtuoso { get; set; }
        public string descErrorRabbitGraphGenerator { get; set; }
        public string descErrorGraphGenerator { get; set; }
        public string descErrorVirtuoso { get; set; }
        public string descErrorReplicacionVirtuoso { get; set; }
        public string descErrorHAProxy { get; set; }
        public string descErrorGeneradorMiniaturas { get; set; }
        public string descErrorLucene { get; set; }
        public string descErrorVisitas { get; set; }
        public string descErrorLiveUsuarios { get; set; }
        public string descErrorRedis { get; set; }
        public string descErrorOntologies { get; set; }
    }

    public class ServicesStatusModel
    {
        public string service;
        public string status;
        public string icon;
        public string description;
    }

	public class ServicesStatusModelAPI
	{
		public ApplicationType Service;
		public ApplicationStatus Status;
		public string ErrorDescription;
        public DateTime StatusCheckTime;
	}

	public class CreateResourceStatusServiceModelAPI
    {
        public string resourceID;
        public ServicesStatusModelAPI serviceStatusModelList;
    }

    public class ErrorDeleteResourceViewModel
    {
        public string description;
        public string url;
    }
}
