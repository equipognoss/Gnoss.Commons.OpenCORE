using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Interfaces.InterfacesOpen
{
	public interface IAvailableServices
	{
		public bool CheckIfServiceIsAvailable(ulong pServiceCode, ServiceType pServiceType);
		public ulong GetFrontServiceCode(FrontService pFrontService);
		public ulong GetBackServiceCode(BackgroundService pBackService);
	}

	public enum ServiceType
	{
		Front,
		Background
	}

	public enum FrontService
	{
		Api,
		ApiCheckService,
		ApiIntegracionContinua,
		Autocomplete,
		Context,
		Deploy,
		Documents,
		EtiquetadoAutomatico,
		Facets,
		FTP,
		IdentityServer,
		Intern,
		Keycloak,
		Login,
		LuceneApi,
		LuceneAutocomplete,
		OAuth,
		Ontologies,
		RelatedVirtuosoApi,
		Results,
		Web
	}

	public enum BackgroundService
	{
		AutocompleteGenerator,
		AutomaticSharing,
		BidirectionalReplication,
		CacheRefresh,
		CI,
		CommunityWall,
		Distributor,
		EventSwitcher,
		LinkedData,
		Mail,
		MassiveDataLoader,
		Newsletter,
		ProcessFilesModifiedOrDeletedResources,
		RefreshCacheHeavyQuery,
		Replication,
		SearchGraphGenerator,
		Sitemaps,
		SocialCacheRefresh,
		SocialSearchGraphGeneration,
		SubscriptionsMail,
		Thumbnail,
		UserWall,
		VisitCluster,
		VisitRegistry,
		Workflows,
		TranslateService
	}
}
