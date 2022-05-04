namespace Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua
{
    public class RemoteStorage
    {

        public string remoteStorageUrl { get; set; }
        public object authentication { get; set; }
        public object connectionSettings { get; set; }
    }

    public class Data
    {
        public string repoType { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public bool browseable { get; set; }
        public bool indexable { get; set; }
        public int notFoundCacheTTL { get; set; }
        public int artifactMaxAge { get; set; }
        public int metadataMaxAge { get; set; }
        public int itemMaxAge { get; set; }
        public string repoPolicy { get; set; }
        public string provider { get; set; }
        public string providerRole { get; set; }
        public bool downloadRemoteIndexes { get; set; }
        public bool autoBlockActive { get; set; }
        public bool fileTypeValidation { get; set; }
        public bool exposed { get; set; }
        public string checksumPolicy { get; set; }
        public RemoteStorage remoteStorage { get; set; }
    }

    public class RepositorioNexus
    {
        public Data data { get; set; }
    }
}
