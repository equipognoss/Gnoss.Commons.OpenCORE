using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces.InterfacesOpen;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open
{
	public class AvailableServicesOpen : IAvailableServices
	{
		private LoggingService mLoggingService;
		private ConfigService mConfigService;
        private ILogger mlogger;

        public AvailableServicesOpen(LoggingService loggingService, ConfigService configService, ILogger<AvailableServicesOpen> logger)
		{
			mLoggingService = loggingService;
			mConfigService = configService;
			mlogger = logger;
		}

		public bool CheckIfServiceIsAvailable(ulong pServiceCode, ServiceType pServiceType)
		{
			try
			{
				ulong servicesAvailable = 0;
				List<ulong> allServicesAvailable = mConfigService.ObtenerServiciosDisponibles();

				if (allServicesAvailable != null && allServicesAvailable.Count > 0)
				{
					switch (pServiceType)
					{
						case ServiceType.Front:
							servicesAvailable = allServicesAvailable[0];
							break;
						case ServiceType.Background:
							servicesAvailable = allServicesAvailable[1];
							break;
					}

					ulong active = servicesAvailable & pServiceCode;

					return active == pServiceCode;
				}
				else
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				mLoggingService.GuardarLogError(ex, $"Error al comprobar si el servicio {pServiceCode} (Tipo:{pServiceType}) está disponible",mlogger);
				return true;
			}
		}

		public ulong GetFrontServiceCode(FrontService pFrontService)
		{
			Dictionary<FrontService, ulong> frontServices = new Dictionary<FrontService, ulong>
			{
				{ FrontService.FTP, 1 },
				{ FrontService.Ontologies, 2 },
				{ FrontService.Login, 4 },
				{ FrontService.Results, 8 },
				{ FrontService.Keycloak, 16 },
				{ FrontService.OAuth, 32 },
				{ FrontService.Documents, 64 },
				{ FrontService.RelatedVirtuosoApi, 128 },
				{ FrontService.Web, 256 },
				{ FrontService.Api, 512 },
				{ FrontService.LuceneAutocomplete, 1024 },
				{ FrontService.Intern, 2048 },
				{ FrontService.Context, 4096 },
				{ FrontService.ApiIntegracionContinua, 8192 },
				{ FrontService.LuceneApi, 16384 },
				{ FrontService.IdentityServer, 32768 },
				{ FrontService.Deploy, 65536 },
				{ FrontService.ApiCheckService, 131072 },
				{ FrontService.EtiquetadoAutomatico, 262144 },
				{ FrontService.Autocomplete, 524288 },
				{ FrontService.Facets, 1048576 }
			};

			return frontServices[pFrontService];
		}

		public ulong GetBackServiceCode(BackgroundService pBackService)
		{
			Dictionary<BackgroundService, ulong> backServices = new Dictionary<BackgroundService, ulong> 
			{
				{ BackgroundService.Thumbnail, 1 },
				{ BackgroundService.SocialSearchGraphGeneration, 2 },
				{ BackgroundService.Newsletter, 4 },
				{ BackgroundService.MassiveDataLoader, 8 },
				{ BackgroundService.Distributor, 16 },
				{ BackgroundService.Sitemaps, 32 },
				{ BackgroundService.CI, 64 },
				{ BackgroundService.VisitCluster, 128 },
				{ BackgroundService.Replication, 256 },
				{ BackgroundService.UserWall, 512 },
				{ BackgroundService.CacheRefresh, 1024 },
				{ BackgroundService.VisitRegistry, 2048 },
				{ BackgroundService.Mail, 4096 },
				{ BackgroundService.SubscriptionsMail, 8192 },
				{ BackgroundService.LinkedData, 16384 },
				{ BackgroundService.SearchGraphGenerator, 32768 },
				{ BackgroundService.CommunityWall, 65536 },
				{ BackgroundService.BidirectionalReplication, 131072 },
				{ BackgroundService.AutomaticSharing, 262144 },
				{ BackgroundService.RefreshCacheHeavyQuery, 524288 },
				{ BackgroundService.AutocompleteGenerator, 1048576 },
				{ BackgroundService.ProcessFilesModifiedOrDeletedResources, 2097152 },
				{ BackgroundService.EventSwitcher, 4194304 },
				{ BackgroundService.SocialCacheRefresh, 8388608 }
			};

			return backServices[pBackService];
		}
	}
}
